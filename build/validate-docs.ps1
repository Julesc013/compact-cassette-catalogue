[CmdletBinding()]
param()

Set-StrictMode -Version 2.0
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$failures = New-Object Collections.Generic.List[String]
$markdownFiles = @(Get-ChildItem -LiteralPath $repositoryRoot -Recurse -File -Filter '*.md' |
    Where-Object {
        # Exclusions belong to this source tree. An exported checkout may itself
        # live below a parent directory named artifacts, bin, or obj.
        $repositoryRelativePath = $_.FullName.Substring($repositoryRoot.Length)
        $repositoryRelativePath -notmatch '[\\/](\.git|artifacts|bin|obj)[\\/]'
    })
$linkPattern = [regex]'\[[^\]]*\]\((?<target>[^)]+)\)'

if ($markdownFiles.Count -eq 0) {
    throw "Documentation validation found no Markdown files below '$repositoryRoot'."
}

foreach ($file in $markdownFiles) {
    $content = Get-Content -LiteralPath $file.FullName -Raw
    foreach ($match in $linkPattern.Matches($content)) {
        $target = [string]$match.Groups['target'].Value
        if ([string]::IsNullOrWhiteSpace($target) -or
                $target.StartsWith('#') -or
                $target -match '^[a-zA-Z][a-zA-Z0-9+.-]*:') {
            continue
        }

        $pathPart = ($target -split '#', 2)[0]
        if ($pathPart.StartsWith('/')) {
            $candidate = Join-Path $repositoryRoot $pathPart.TrimStart('/')
        }
        else {
            $candidate = Join-Path $file.DirectoryName $pathPart
        }

        if (-not (Test-Path -LiteralPath $candidate)) {
            $relativeFile = $file.FullName.Substring($repositoryRoot.Length + 1)
            $failures.Add("$relativeFile -> $target")
        }
    }
}

if ($failures.Count -gt 0) {
    throw ("Documentation contains broken local links:`n - " + ($failures -join "`n - "))
}

Write-Host "Documentation links verified across $($markdownFiles.Count) Markdown files."
