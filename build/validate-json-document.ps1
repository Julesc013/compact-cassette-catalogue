[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$SchemaPath,
    [Parameter(Mandatory = $true)]
    [string]$DocumentPath,
    [long]$MaximumBytes = 0
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Runtime.Serialization

$resolvedSchemaPath = [IO.Path]::GetFullPath($SchemaPath)
$resolvedDocumentPath = [IO.Path]::GetFullPath($DocumentPath)
foreach ($path in @($resolvedSchemaPath, $resolvedDocumentPath)) {
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
        throw "JSON validation input is missing: $path"
    }
}

function Read-StrictJsonDocument {
    param(
        [string]$Path,
        [long]$ByteLimit = 0,
        [switch]$CheckDuplicateProperties
    )

    $file = Get-Item -LiteralPath $Path
    if ($ByteLimit -gt 0 -and $file.Length -gt $ByteLimit) {
        throw "JSON document exceeds the $ByteLimit-byte limit: $Path"
    }
    $bytes = [IO.File]::ReadAllBytes($Path)
    # Recheck the bytes actually read so a concurrent replacement cannot evade
    # the allocation-independent FileInfo preflight.
    if ($ByteLimit -gt 0 -and $bytes.LongLength -gt $ByteLimit) {
        throw "JSON document exceeds the $ByteLimit-byte limit: $Path"
    }

    $strictUtf8 = New-Object Text.UTF8Encoding($false, $true)
    $quotas = New-Object System.Xml.XmlDictionaryReaderQuotas
    $quotas.MaxDepth = 128
    $quotas.MaxStringContentLength = 4 * 1024 * 1024
    $quotas.MaxArrayLength = 100000
    $quotas.MaxBytesPerRead = 4096
    $quotas.MaxNameTableCharCount = 1024 * 1024
    if ($CheckDuplicateProperties) {
        $reader = $null
        try {
            $reader = [System.Runtime.Serialization.Json.JsonReaderWriterFactory]::CreateJsonReader(
                $bytes,
                0,
                $bytes.Length,
                $strictUtf8,
                $quotas,
                $null)
            $objectProperties = @{}
            while ($reader.Read()) {
                if ($reader.NodeType -eq [Xml.XmlNodeType]::Element) {
                    $parentDepth = $reader.Depth - 1
                    if ($objectProperties.ContainsKey($parentDepth)) {
                        $propertyName = [string]$reader.LocalName
                        # JsonReaderWriterFactory escapes JSON member names that
                        # are not legal XML names as an `item` element and keeps
                        # the original spelling in the `item` attribute. Use the
                        # original spelling so `$schema` and `$id` do not collide,
                        # while genuinely duplicated escaped names still fail.
                        $escapedPropertyName = $reader.GetAttribute('item')
                        if ($propertyName -ceq 'item' -and
                            $null -ne $escapedPropertyName) {
                            $propertyName = [string]$escapedPropertyName
                        }
                        if (-not $objectProperties[$parentDepth].Add($propertyName)) {
                            throw "JSON object contains duplicate property '$propertyName'."
                        }
                    }

                    if ($reader.GetAttribute('type') -ceq 'object' -and
                        -not $reader.IsEmptyElement) {
                        $objectProperties[$reader.Depth] =
                            New-Object 'Collections.Generic.HashSet[String]' (
                                [StringComparer]::Ordinal)
                    }
                }
                elseif ($reader.NodeType -eq [Xml.XmlNodeType]::EndElement -and
                    $objectProperties.ContainsKey($reader.Depth)) {
                    $objectProperties.Remove($reader.Depth)
                }
            }
        }
        catch {
            throw "JSON transport validation failed for '$Path': $($_.Exception.Message)"
        }
        finally {
            if ($null -ne $reader) {
                $reader.Dispose()
            }
        }
    }

    try {
        return ($strictUtf8.GetString($bytes) | ConvertFrom-Json)
    }
    catch {
        throw "JSON parsing failed for '$Path': $($_.Exception.Message)"
    }
}

$schema = Read-StrictJsonDocument `
    $resolvedSchemaPath `
    (1024 * 1024) `
    -CheckDuplicateProperties
$document = Read-StrictJsonDocument `
    $resolvedDocumentPath `
    $MaximumBytes `
    -CheckDuplicateProperties
$failures = New-Object Collections.Generic.List[String]

function Test-HasProperty {
    param(
        [object]$Value,
        [string]$Name
    )

    if ($null -eq $Value) {
        return $false
    }
    return @($Value.PSObject.Properties | Where-Object { $_.Name -ceq $Name }).Count -eq 1
}

function Get-PropertyValue {
    param(
        [object]$Value,
        [string]$Name
    )

    $property = @($Value.PSObject.Properties | Where-Object { $_.Name -ceq $Name })
    if ($property.Count -ne 1) {
        throw "JSON Schema reference points to missing property '$Name'."
    }
    $propertyValue = $property[0].Value
    if ($propertyValue -is [Array]) {
        return ,$propertyValue
    }
    return $propertyValue
}

function Get-JsonType {
    param([object]$Value)

    if ($null -eq $Value) { return 'null' }
    if ($Value -is [string]) { return 'string' }
    if ($Value -is [bool]) { return 'boolean' }
    if ($Value -is [Array]) { return 'array' }
    if ($Value -is [PSCustomObject] -or $Value -is [Collections.IDictionary]) { return 'object' }
    if ($Value -is [byte] -or $Value -is [sbyte] -or
        $Value -is [int16] -or $Value -is [uint16] -or
        $Value -is [int32] -or $Value -is [uint32] -or
        $Value -is [int64] -or $Value -is [uint64]) { return 'integer' }
    if ($Value -is [single] -or $Value -is [double] -or $Value -is [decimal]) { return 'number' }
    return 'unknown'
}

function Resolve-LocalReference {
    param([string]$Reference)

    if (-not $Reference.StartsWith('#/', [StringComparison]::Ordinal)) {
        throw "Only local JSON Schema references are supported: $Reference"
    }
    $value = $schema
    foreach ($encodedSegment in $Reference.Substring(2).Split('/')) {
        $segment = $encodedSegment.Replace('~1', '/').Replace('~0', '~')
        $value = Get-PropertyValue $value $segment
    }
    return $value
}

function Get-StructuralIdentity {
    param([object]$Value)

    $valueType = Get-JsonType $Value
    if ($valueType -ceq 'null') { return 'N' }
    if ($valueType -ceq 'boolean') {
        if ([bool]$Value) { return 'B1' }
        return 'B0'
    }
    if ($valueType -ceq 'string') {
        $text = [string]$Value
        return 'S' + $text.Length + ':' + $text
    }
    if ($valueType -in @('integer', 'number')) {
        $invariantCulture = [Globalization.CultureInfo]::InvariantCulture
        if ($Value -is [decimal]) {
            return 'D' + $Value.ToString('G29', $invariantCulture)
        }
        if ($Value -is [double] -or $Value -is [single]) {
            return 'D' + $Value.ToString('R', $invariantCulture)
        }
        return 'D' + [Convert]::ToString($Value, $invariantCulture)
    }
    if ($valueType -ceq 'array') {
        $parts = New-Object Collections.Generic.List[String]
        foreach ($item in @($Value)) {
            $identity = Get-StructuralIdentity $item
            [void]$parts.Add($identity.Length.ToString(
                    [Globalization.CultureInfo]::InvariantCulture) + ':' + $identity)
        }
        return 'A' + $parts.Count + '[' + ($parts -join '') + ']'
    }
    if ($valueType -ceq 'object') {
        $names = @($Value.PSObject.Properties | ForEach-Object { $_.Name })
        [Array]::Sort($names, [StringComparer]::Ordinal)
        $parts = New-Object Collections.Generic.List[String]
        foreach ($name in $names) {
            $childIdentity = Get-StructuralIdentity (Get-PropertyValue $Value $name)
            [void]$parts.Add(
                $name.Length.ToString([Globalization.CultureInfo]::InvariantCulture) +
                ':' + $name +
                $childIdentity.Length.ToString(
                    [Globalization.CultureInfo]::InvariantCulture) + ':' +
                $childIdentity)
        }
        return 'O' + $parts.Count + '{' + ($parts -join '') + '}'
    }
    throw "Cannot create a structural identity for JSON type '$valueType'."
}

function Test-PrimitiveEquality {
    param(
        [object]$Left,
        [object]$Right
    )

    $leftType = Get-JsonType $Left
    $rightType = Get-JsonType $Right
    $bothNumeric = $leftType -in @('integer', 'number') -and
        $rightType -in @('integer', 'number')
    if ($leftType -cne $rightType -and -not $bothNumeric) {
        return $false
    }
    if ($leftType -ceq 'null') {
        return $true
    }
    if ($leftType -in @('object', 'array')) {
        return (Get-StructuralIdentity $Left) -ceq (Get-StructuralIdentity $Right)
    }
    if ($leftType -ceq 'string') {
        return [string]$Left -ceq [string]$Right
    }
    return $Left -eq $Right
}

$supportedSchemaKeywords = New-Object 'Collections.Generic.HashSet[String]' (
    [StringComparer]::Ordinal)
$schemaReferenceStack = New-Object 'Collections.Generic.HashSet[String]' (
    [StringComparer]::Ordinal)
foreach ($keyword in @(
        '$schema', '$id', '$defs', '$ref', 'title',
        'type', 'const', 'enum', 'required', 'properties',
        'additionalProperties', 'items', 'minItems', 'maxItems', 'uniqueItems',
        'minLength', 'maxLength', 'pattern', 'format', 'minimum', 'maximum',
        'allOf', 'oneOf', 'if', 'then', 'else')) {
    [void]$supportedSchemaKeywords.Add($keyword)
}

function Assert-SchemaDefinition {
    param(
        [object]$Rule,
        [string]$Path
    )

    if ($Rule -is [bool]) { return }
    if ($Rule -isnot [PSCustomObject]) {
        throw "JSON Schema rule at '$Path' is not an object or Boolean."
    }

    foreach ($property in $Rule.PSObject.Properties) {
        if (-not $supportedSchemaKeywords.Contains([string]$property.Name)) {
            throw "Unsupported JSON Schema keyword '$($property.Name)' at '$Path'."
        }
    }

    foreach ($metadataName in @('$schema', '$id', 'title', '$ref')) {
        if ((Test-HasProperty $Rule $metadataName) -and
            (Get-PropertyValue $Rule $metadataName) -isnot [string]) {
            throw "JSON Schema keyword '$metadataName' at '$Path' must be a string."
        }
    }
    if (Test-HasProperty $Rule '$ref') {
        $reference = [string](Get-PropertyValue $Rule '$ref')
        if ($reference -cnotmatch '^#/\$defs/[^/]+$') {
            throw "Only direct local JSON Schema definition references are supported at '$Path': $reference"
        }
        if (-not $schemaReferenceStack.Add($reference)) {
            throw "Cyclic JSON Schema references are not supported at '$Path': $reference"
        }
        try {
            $referencedRule = Resolve-LocalReference $reference
            Assert-SchemaDefinition $referencedRule "$Path/`$ref($reference)"
        }
        finally {
            [void]$schemaReferenceStack.Remove($reference)
        }
    }

    if (Test-HasProperty $Rule 'type') {
        $typeRule = Get-PropertyValue $Rule 'type'
        if ($typeRule -isnot [string] -and $typeRule -isnot [Array]) {
            throw "JSON Schema type at '$Path' must be a string or array."
        }
        $seenTypes = New-Object 'Collections.Generic.HashSet[String]' (
            [StringComparer]::Ordinal)
        foreach ($declaredType in @($typeRule)) {
            if ($declaredType -isnot [string] -or
                @('null', 'boolean', 'object', 'array', 'string', 'integer', 'number') `
                    -cnotcontains [string]$declaredType) {
                throw "Unsupported JSON Schema type '$declaredType' at '$Path'."
            }
            if (-not $seenTypes.Add([string]$declaredType)) {
                throw "Duplicate JSON Schema type '$declaredType' at '$Path'."
            }
        }
        if ($seenTypes.Count -eq 0) {
            throw "JSON Schema type array at '$Path' cannot be empty."
        }
    }

    foreach ($arrayKeyword in @('enum', 'required', 'allOf', 'oneOf')) {
        if (Test-HasProperty $Rule $arrayKeyword) {
            $arrayRule = Get-PropertyValue $Rule $arrayKeyword
            if ($arrayRule -isnot [Array]) {
                throw "JSON Schema keyword '$arrayKeyword' at '$Path' must be an array."
            }
        }
    }
    foreach ($branchKeyword in @('allOf', 'oneOf')) {
        if (Test-HasProperty $Rule $branchKeyword) {
            $branches = Get-PropertyValue $Rule $branchKeyword
            if ($branches.Count -eq 0) {
                throw "JSON Schema keyword '$branchKeyword' at '$Path' cannot be empty."
            }
            for ($index = 0; $index -lt $branches.Count; $index++) {
                Assert-SchemaDefinition $branches[$index] "$Path/$branchKeyword/$index"
            }
        }
    }

    if (Test-HasProperty $Rule 'required') {
        $seenRequired = New-Object 'Collections.Generic.HashSet[String]' (
            [StringComparer]::Ordinal)
        $requiredNames = Get-PropertyValue $Rule 'required'
        foreach ($requiredName in @($requiredNames)) {
            if ($requiredName -isnot [string] -or
                [string]::IsNullOrEmpty([string]$requiredName)) {
                throw "JSON Schema required names at '$Path' must be non-empty strings."
            }
            if (-not $seenRequired.Add([string]$requiredName)) {
                throw "Duplicate required name '$requiredName' at '$Path'."
            }
        }
    }
    if (Test-HasProperty $Rule 'enum') {
        $enumValues = Get-PropertyValue $Rule 'enum'
        if ($enumValues.Count -eq 0) {
            throw "JSON Schema enum at '$Path' cannot be empty."
        }
        $seenEnumValues = New-Object 'Collections.Generic.HashSet[String]' (
            [StringComparer]::Ordinal)
        foreach ($enumValue in @($enumValues)) {
            $enumIdentity = Get-StructuralIdentity $enumValue
            if (-not $seenEnumValues.Add($enumIdentity)) {
                throw "JSON Schema enum at '$Path' contains a duplicate value."
            }
        }
    }

    foreach ($containerKeyword in @('$defs', 'properties')) {
        if (Test-HasProperty $Rule $containerKeyword) {
            $container = Get-PropertyValue $Rule $containerKeyword
            if ($container -isnot [PSCustomObject]) {
                throw "JSON Schema keyword '$containerKeyword' at '$Path' must be an object."
            }
            foreach ($childProperty in $container.PSObject.Properties) {
                Assert-SchemaDefinition $childProperty.Value (
                    "$Path/$containerKeyword/$($childProperty.Name)")
            }
        }
    }

    foreach ($childKeyword in @('items', 'if', 'then', 'else')) {
        if (Test-HasProperty $Rule $childKeyword) {
            Assert-SchemaDefinition (Get-PropertyValue $Rule $childKeyword) (
                "$Path/$childKeyword")
        }
    }
    if (Test-HasProperty $Rule 'additionalProperties') {
        $additionalRule = Get-PropertyValue $Rule 'additionalProperties'
        if ($additionalRule -isnot [bool] -and
            $additionalRule -isnot [PSCustomObject]) {
            throw "JSON Schema additionalProperties at '$Path' must be Boolean or an object."
        }
        if ($additionalRule -is [PSCustomObject]) {
            Assert-SchemaDefinition $additionalRule "$Path/additionalProperties"
        }
    }

    foreach ($booleanKeyword in @('uniqueItems')) {
        if ((Test-HasProperty $Rule $booleanKeyword) -and
            (Get-PropertyValue $Rule $booleanKeyword) -isnot [bool]) {
            throw "JSON Schema keyword '$booleanKeyword' at '$Path' must be Boolean."
        }
    }
    foreach ($nonNegativeIntegerKeyword in @(
            'minItems', 'maxItems', 'minLength', 'maxLength')) {
        if (Test-HasProperty $Rule $nonNegativeIntegerKeyword) {
            $constraint = Get-PropertyValue $Rule $nonNegativeIntegerKeyword
            if ((Get-JsonType $constraint) -cne 'integer' -or
                [decimal]$constraint -lt 0) {
                throw "JSON Schema keyword '$nonNegativeIntegerKeyword' at '$Path' must be a non-negative integer."
            }
        }
    }
    foreach ($numericKeyword in @('minimum', 'maximum')) {
        if ((Test-HasProperty $Rule $numericKeyword) -and
            (Get-JsonType (Get-PropertyValue $Rule $numericKeyword)) `
                -notin @('integer', 'number')) {
            throw "JSON Schema keyword '$numericKeyword' at '$Path' must be numeric."
        }
    }
    if ((Test-HasProperty $Rule 'minItems') -and
        (Test-HasProperty $Rule 'maxItems') -and
        [decimal](Get-PropertyValue $Rule 'minItems') -gt
            [decimal](Get-PropertyValue $Rule 'maxItems')) {
        throw "JSON Schema minItems exceeds maxItems at '$Path'."
    }
    if ((Test-HasProperty $Rule 'minLength') -and
        (Test-HasProperty $Rule 'maxLength') -and
        [decimal](Get-PropertyValue $Rule 'minLength') -gt
            [decimal](Get-PropertyValue $Rule 'maxLength')) {
        throw "JSON Schema minLength exceeds maxLength at '$Path'."
    }
    if ((Test-HasProperty $Rule 'minimum') -and
        (Test-HasProperty $Rule 'maximum') -and
        [decimal](Get-PropertyValue $Rule 'minimum') -gt
            [decimal](Get-PropertyValue $Rule 'maximum')) {
        throw "JSON Schema minimum exceeds maximum at '$Path'."
    }

    if (Test-HasProperty $Rule 'pattern') {
        $patternRule = Get-PropertyValue $Rule 'pattern'
        if ($patternRule -isnot [string]) {
            throw "JSON Schema pattern at '$Path' must be a string."
        }
        try { [void](New-Object Text.RegularExpressions.Regex ([string]$patternRule)) }
        catch { throw "Invalid JSON Schema pattern at '$Path': $($_.Exception.Message)" }
    }
    if (Test-HasProperty $Rule 'format') {
        $formatRule = Get-PropertyValue $Rule 'format'
        if ($formatRule -isnot [string] -or [string]$formatRule -cne 'uri') {
            throw "Unsupported JSON Schema format '$formatRule' at '$Path'."
        }
    }
}

function Add-SchemaFailure {
    param(
        [string]$Path,
        [string]$Message
    )

    [void]$failures.Add("${Path}: $Message")
}

function Test-SchemaBranch {
    param(
        [object]$Value,
        [object]$Rule,
        [string]$Path
    )

    $failureCount = $failures.Count
    Test-SchemaNode $Value $Rule $Path
    $matches = $failures.Count -eq $failureCount
    if ($failures.Count -gt $failureCount) {
        $failures.RemoveRange($failureCount, $failures.Count - $failureCount)
    }
    return $matches
}

function Test-SchemaNode {
    param(
        [object]$Value,
        [object]$Rule,
        [string]$Path
    )

    if ($Rule -is [bool]) {
        if (-not $Rule) {
            Add-SchemaFailure $Path 'is prohibited by the schema'
        }
        return
    }
    if ($Rule -isnot [PSCustomObject]) {
        throw "JSON Schema rule at '$Path' is not an object or Boolean."
    }

    if (Test-HasProperty $Rule '$ref') {
        $referencedRule = Resolve-LocalReference ([string](Get-PropertyValue $Rule '$ref'))
        Test-SchemaNode $Value $referencedRule $Path
    }

    if (Test-HasProperty $Rule 'allOf') {
        $allOfBranches = Get-PropertyValue $Rule 'allOf'
        foreach ($branch in @($allOfBranches)) {
            Test-SchemaNode $Value $branch $Path
        }
    }
    if (Test-HasProperty $Rule 'oneOf') {
        $matchingBranches = 0
        $oneOfBranches = Get-PropertyValue $Rule 'oneOf'
        foreach ($branch in @($oneOfBranches)) {
            if (Test-SchemaBranch $Value $branch $Path) {
                $matchingBranches++
            }
        }
        if ($matchingBranches -ne 1) {
            Add-SchemaFailure $Path "matches $matchingBranches oneOf branches; expected exactly one"
        }
    }
    if (Test-HasProperty $Rule 'if') {
        $conditionMatches = Test-SchemaBranch $Value (Get-PropertyValue $Rule 'if') $Path
        if ($conditionMatches -and (Test-HasProperty $Rule 'then')) {
            Test-SchemaNode $Value (Get-PropertyValue $Rule 'then') $Path
        }
        elseif (-not $conditionMatches -and (Test-HasProperty $Rule 'else')) {
            Test-SchemaNode $Value (Get-PropertyValue $Rule 'else') $Path
        }
    }

    $actualType = Get-JsonType $Value
    if (Test-HasProperty $Rule 'type') {
        $typeValue = Get-PropertyValue $Rule 'type'
        $allowedTypes = @($typeValue)
        $typeMatches = $allowedTypes -ccontains $actualType
        if (-not $typeMatches -and $actualType -ceq 'integer') {
            $typeMatches = $allowedTypes -ccontains 'number'
        }
        if (-not $typeMatches) {
            Add-SchemaFailure $Path "has type '$actualType'; expected $($allowedTypes -join '|')"
            return
        }
    }

    if (Test-HasProperty $Rule 'const') {
        $constant = Get-PropertyValue $Rule 'const'
        if (-not (Test-PrimitiveEquality $Value $constant)) {
            Add-SchemaFailure $Path "does not equal the required constant '$constant'"
        }
    }
    if (Test-HasProperty $Rule 'enum') {
        $enumValue = Get-PropertyValue $Rule 'enum'
        $allowed = @($enumValue)
        $matched = $false
        foreach ($candidate in $allowed) {
            if (Test-PrimitiveEquality $Value $candidate) {
                $matched = $true
                break
            }
        }
        if (-not $matched) {
            Add-SchemaFailure $Path "is not one of the allowed values"
        }
    }

    if ($actualType -ceq 'object') {
        $propertyNames = @($Value.PSObject.Properties | ForEach-Object { $_.Name })
        if (Test-HasProperty $Rule 'required') {
            $requiredValue = Get-PropertyValue $Rule 'required'
            foreach ($requiredName in @($requiredValue)) {
                if ($propertyNames -cnotcontains [string]$requiredName) {
                    Add-SchemaFailure $Path "is missing required property '$requiredName'"
                }
            }
        }

        $propertyRules = $null
        $definedPropertyNames = @()
        if (Test-HasProperty $Rule 'properties') {
            $propertyRules = Get-PropertyValue $Rule 'properties'
            $definedPropertyNames = @($propertyRules.PSObject.Properties | ForEach-Object { $_.Name })
            foreach ($propertyName in $definedPropertyNames) {
                if ($propertyNames -ccontains $propertyName) {
                    $childRule = Get-PropertyValue $propertyRules $propertyName
                    $childValue = Get-PropertyValue $Value $propertyName
                    Test-SchemaNode $childValue $childRule "$Path/$propertyName"
                }
            }
        }
        if (Test-HasProperty $Rule 'additionalProperties') {
            $additionalProperties = Get-PropertyValue $Rule 'additionalProperties'
            if ($additionalProperties -is [bool] -and
                -not [bool]$additionalProperties) {
                foreach ($propertyName in $propertyNames) {
                    if ($definedPropertyNames -cnotcontains $propertyName) {
                        Add-SchemaFailure "$Path/$propertyName" 'is an unsupported property'
                    }
                }
            }
            elseif ($additionalProperties -is [PSCustomObject]) {
                foreach ($propertyName in $propertyNames) {
                    if ($definedPropertyNames -cnotcontains $propertyName) {
                        Test-SchemaNode (Get-PropertyValue $Value $propertyName) `
                            $additionalProperties "$Path/$propertyName"
                    }
                }
            }
        }
    }

    if ($actualType -ceq 'array') {
        $items = @($Value)
        if (Test-HasProperty $Rule 'minItems') {
            $minItems = [int](Get-PropertyValue $Rule 'minItems')
            if ($items.Count -lt $minItems) {
                Add-SchemaFailure $Path 'has too few items'
            }
        }
        if (Test-HasProperty $Rule 'maxItems') {
            $maxItems = [int](Get-PropertyValue $Rule 'maxItems')
            if ($items.Count -gt $maxItems) {
                Add-SchemaFailure $Path 'has too many items'
            }
        }
        if (Test-HasProperty $Rule 'items') {
            $itemRule = Get-PropertyValue $Rule 'items'
            for ($index = 0; $index -lt $items.Count; $index++) {
                Test-SchemaNode $items[$index] $itemRule "$Path/$index"
            }
        }
        if ((Test-HasProperty $Rule 'uniqueItems') -and
            [bool](Get-PropertyValue $Rule 'uniqueItems')) {
            $seenItems = New-Object 'Collections.Generic.HashSet[String]' (
                [StringComparer]::Ordinal)
            for ($index = 0; $index -lt $items.Count; $index++) {
                $itemKey = Get-StructuralIdentity $items[$index]
                if (-not $seenItems.Add($itemKey)) {
                    Add-SchemaFailure "$Path/$index" 'duplicates an earlier array item'
                }
            }
        }
    }

    if ($actualType -ceq 'string') {
        $text = [string]$Value
        if (Test-HasProperty $Rule 'minLength') {
            $minLength = [int](Get-PropertyValue $Rule 'minLength')
            if ($text.Length -lt $minLength) {
                Add-SchemaFailure $Path 'is shorter than minLength'
            }
        }
        if (Test-HasProperty $Rule 'maxLength') {
            $maxLength = [int](Get-PropertyValue $Rule 'maxLength')
            if ($text.Length -gt $maxLength) {
                Add-SchemaFailure $Path 'is longer than maxLength'
            }
        }
        if (Test-HasProperty $Rule 'pattern') {
            $pattern = [string](Get-PropertyValue $Rule 'pattern')
            if ($text -cnotmatch $pattern) {
                Add-SchemaFailure $Path "does not match the required pattern"
            }
        }
        if (Test-HasProperty $Rule 'format') {
            $format = [string](Get-PropertyValue $Rule 'format')
            if ($format -ceq 'uri') {
                $parsedUri = $null
                if (-not [Uri]::TryCreate($text, [UriKind]::Absolute, [ref]$parsedUri)) {
                    Add-SchemaFailure $Path 'is not an absolute URI'
                }
            }
        }
    }

    if ($actualType -in @('integer', 'number') -and (Test-HasProperty $Rule 'minimum')) {
        $minimum = [decimal](Get-PropertyValue $Rule 'minimum')
        if ([decimal]$Value -lt $minimum) {
            Add-SchemaFailure $Path "is below the required minimum"
        }
    }
    if ($actualType -in @('integer', 'number') -and (Test-HasProperty $Rule 'maximum')) {
        $maximum = [decimal](Get-PropertyValue $Rule 'maximum')
        if ([decimal]$Value -gt $maximum) {
            Add-SchemaFailure $Path "is above the required maximum"
        }
    }
}

Assert-SchemaDefinition $schema '$schema'
Test-SchemaNode $document $schema '$'

if ($failures.Count -gt 0) {
    throw ("JSON Schema validation failed:`n - " + ($failures -join "`n - "))
}

Write-Host "JSON Schema verified: $resolvedDocumentPath"
