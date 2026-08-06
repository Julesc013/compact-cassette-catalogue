Imports System.Data
Imports System.Reflection
Imports System.Text
Imports System.Threading
Imports Compact_Cassette_Catalogue

Module Program

    Private _failures As Integer
    Private _repositoryRoot As String

    Sub Main()
        _repositoryRoot = FindRepositoryRoot()

        RunTest("blank catalogue matches the v1.1 contract", AddressOf BlankCatalogueMatchesContract)
        RunTest("populated catalogue matches the v1.1 contract", AddressOf PopulatedCatalogueMatchesContract)
        RunTest("missing file version is detected", AddressOf MissingVersionIsDetected)
        RunTest("unsupported file version remains distinguishable", AddressOf UnsupportedVersionIsDetected)
        RunTest("historical prerelease suffix normalizes to v1.1.0", AddressOf HistoricalVersionSuffixNormalizes)
        RunTest("malformed XML is rejected", AddressOf MalformedXmlIsRejected)
        RunTest("external entities are rejected", AddressOf ExternalEntityIsRejected)
        RunTest("XML decimals remain culture independent", AddressOf XmlDecimalsAreCultureIndependent)
        RunTest("pending tape transitions fail closed", AddressOf PendingTapeTransitionsFailClosed)
        RunTest("catalogue transitions require a completed save", AddressOf CatalogueTransitionsRequireCompletedSave)
        RunTest("repeated clean close remains nonrecursive", AddressOf RepeatedCleanCloseRemainsNonrecursive)
        RunTest("failed temporary load preserves active catalogue", AddressOf FailedTemporaryLoadPreservesActiveCatalogue)
        RunTest("bounded loader rejects hostile and oversized XML", AddressOf BoundedLoaderRejectsHostileAndOversizedXml)
        RunTest("transactional save preserves destination on injected faults", AddressOf TransactionalSavePreservesDestinationOnFaults)
        RunTest("transactional save reopens bytes and retains backup", AddressOf TransactionalSaveReopensAndRetainsBackup)
        RunTest("external catalogue edits invalidate captured revision", AddressOf ExternalCatalogueEditsInvalidateRevision)
        RunTest("tape mapping is named and preserves identity", AddressOf TapeMappingIsNamedAndPreservesIdentity)
        RunTest("main form uses one-dialog nonrecursive persistence flow", AddressOf MainFormUsesClosedPersistenceFlow)
        RunTest("lifecycle state machine exhaustively fails closed", AddressOf LifecycleStateMachineExhaustivelyFailsClosed)
        RunTest("production schema loads every catalogue table", AddressOf ProductionSchemaLoadsEveryCatalogueTable)
        RunTest("typed loader rejects invalid field values", AddressOf TypedLoaderRejectsInvalidFieldValues)
        RunTest("creation planner closes every prerequisite graph", AddressOf CreationPlannerClosesPrerequisites)
        RunTest("catalogue choices preserve stable identity", AddressOf CatalogueChoicesPreserveStableIdentity)
        RunTest("creation dialogs expose explicit result contracts", AddressOf CreationDialogsExposeResultContracts)
        RunTest("guided creation has no prerequisite dead ends", AddressOf GuidedCreationHasNoDeadEnds)
        RunTest("tape prerequisite detours preserve the active form", AddressOf TapeDetoursPreserveActiveForm)

        If _failures > 0 Then
            Console.Error.WriteLine("{0} characterization test(s) failed.", _failures)
            Environment.ExitCode = 1
            Return
        End If

        Console.WriteLine("All C3 catalogue characterization tests passed.")
    End Sub

    Private Sub RunTest(name As String, test As Action)
        Try
            test()
            Console.WriteLine("PASS: " & name)
        Catch ex As Exception
            _failures += 1
            Console.Error.WriteLine("FAIL: {0}{1}{2}", name, Environment.NewLine, ex.ToString())
        End Try
    End Sub

    Private Sub BlankCatalogueMatchesContract()
        Dim path As String = FixturePath("valid", "blank.xml")
        ValidateAgainstSchema(path)

        Dim document As XmlDocument = LoadSecureDocument(path)
        AssertEqual("1.1.0", ReadFileVersion(document), "blank file version")
        AssertEqual(0, document.SelectNodes("/Catalogue/Brands").Count, "blank brand count")
        AssertEqual(0, document.SelectNodes("/Catalogue/Models").Count, "blank model count")
        AssertEqual(0, document.SelectNodes("/Catalogue/Decks").Count, "blank deck count")
        AssertEqual(0, document.SelectNodes("/Catalogue/Tapes").Count, "blank tape count")
    End Sub

    Private Sub PopulatedCatalogueMatchesContract()
        Dim path As String = FixturePath("valid", "populated.xml")
        ValidateAgainstSchema(path)

        Dim document As XmlDocument = LoadSecureDocument(path)
        AssertEqual("1.1.0", ReadFileVersion(document), "populated file version")
        AssertEqual(1, document.SelectNodes("/Catalogue/Brands").Count, "brand count")
        AssertEqual(1, document.SelectNodes("/Catalogue/Models").Count, "model count")
        AssertEqual(1, document.SelectNodes("/Catalogue/Decks").Count, "deck count")
        AssertEqual(1, document.SelectNodes("/Catalogue/Tapes").Count, "tape count")
        AssertEqual("MAX", NodeText(document, "/Catalogue/Brands/Code"), "brand code")
        AssertEqual("MAX-2-XLII", NodeText(document, "/Catalogue/Models/Identifier"), "model identifier")
        AssertEqual("MAX-2-XLII-1", NodeText(document, "/Catalogue/Tapes/IdentifierShort"), "tape identifier")
    End Sub

    Private Sub MissingVersionIsDetected()
        Dim document As XmlDocument = LoadSecureDocument(FixturePath("invalid", "missing-version.xml"))
        AssertEqual(Nothing, ReadFileVersion(document), "missing file version")
    End Sub

    Private Sub UnsupportedVersionIsDetected()
        Dim document As XmlDocument = LoadSecureDocument(FixturePath("invalid", "unsupported-version.xml"))
        AssertEqual("2.0.0", ReadFileVersion(document), "unsupported file version")
    End Sub

    Private Sub HistoricalVersionSuffixNormalizes()
        AssertEqual("1.1.0", NormalizeVersion(" 1.1.0b1 "), "historical version suffix")
    End Sub

    Private Sub MalformedXmlIsRejected()
        AssertThrowsXmlException(
            Sub() LoadSecureDocument(FixturePath("invalid", "malformed.xml")),
            "malformed catalogue")
    End Sub

    Private Sub ExternalEntityIsRejected()
        AssertThrowsXmlException(
            Sub() LoadSecureDocument(FixturePath("security", "external-entity.xml")),
            "external entity catalogue")
    End Sub

    Private Sub XmlDecimalsAreCultureIndependent()
        Dim originalCulture As CultureInfo = Thread.CurrentThread.CurrentCulture
        Try
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE")
            Dim document As XmlDocument = LoadSecureDocument(FixturePath("cultures", "decimal-dot.xml"))
            Dim parsed As Decimal = XmlConvert.ToDecimal(NodeText(document, "/Catalogue/Decks/WowFlutter"))
            AssertEqual(0.04D, parsed, "invariant decimal")
        Finally
            Thread.CurrentThread.CurrentCulture = originalCulture
        End Try
    End Sub

    Private Sub PendingTapeTransitionsFailClosed()
        AssertEqual(False, frmMain.TransitionCanContinue(True, frmMain.EditChoice.Cancel, False, False, frmMain.DocumentChoice.Discard, True), "pending cancel")
        AssertEqual(False, frmMain.TransitionCanContinue(True, frmMain.EditChoice.Apply, False, False, frmMain.DocumentChoice.Discard, True), "failed pending apply")
        AssertEqual(True, frmMain.TransitionCanContinue(True, frmMain.EditChoice.Discard, False, False, frmMain.DocumentChoice.Discard, True), "pending discard")
    End Sub

    Private Sub CatalogueTransitionsRequireCompletedSave()
        AssertEqual(False, frmMain.TransitionCanContinue(False, frmMain.EditChoice.Discard, False, True, frmMain.DocumentChoice.Cancel, False), "catalogue cancel")
        AssertEqual(False, frmMain.TransitionCanContinue(False, frmMain.EditChoice.Discard, False, True, frmMain.DocumentChoice.Save, False), "failed catalogue save")
        AssertEqual(True, frmMain.TransitionCanContinue(False, frmMain.EditChoice.Discard, False, True, frmMain.DocumentChoice.Save, True), "completed catalogue save")
    End Sub

    Private Sub RepeatedCleanCloseRemainsNonrecursive()
        For iteration As Integer = 1 To 1000
            AssertEqual(True, frmMain.TransitionCanContinue(False, frmMain.EditChoice.Discard, False, False, frmMain.DocumentChoice.Discard, True), "clean close " & iteration.ToString(CultureInfo.InvariantCulture))
        Next
    End Sub

    Private Sub FailedTemporaryLoadPreservesActiveCatalogue()
        Dim active As DataSet = CreateBlankCatalogue()
        Dim before As String = active.GetXml()
        Dim invalidPath As String = TemporaryPath("truncated.xml")
        File.WriteAllText(invalidPath, "<Catalogue><Information>")
        Try
            AssertThrowsAny(Sub() frmMain.LoadCatalogueSnapshot(invalidPath, active, New String() {"1.1.0"}), "truncated catalogue")
            AssertEqual(before, active.GetXml(), "active catalogue after failed load")
        Finally
            DeleteIfPresent(invalidPath)
        End Try
    End Sub

    Private Sub BoundedLoaderRejectsHostileAndOversizedXml()
        Dim schema As DataSet = CreateBlankCatalogue()
        AssertThrowsAny(Sub() frmMain.LoadCatalogueSnapshot(FixturePath("security", "external-entity.xml"), schema, New String() {"1.1.0"}), "external entity through production loader")

        Dim oversizedPath As String = TemporaryPath("oversized.xml")
        Try
            Using stream As New FileStream(oversizedPath, FileMode.CreateNew, FileAccess.Write, FileShare.None)
                stream.SetLength(frmMain.MaximumCatalogueBytes + 1L)
            End Using
            AssertThrowsInvalidData(Sub() frmMain.LoadCatalogueSnapshot(oversizedPath, schema, New String() {"1.1.0"}), "oversized catalogue")
        Finally
            DeleteIfPresent(oversizedPath)
        End Try
    End Sub

    Private Sub TransactionalSavePreservesDestinationOnFaults()
        Dim active As DataSet = CreateBlankCatalogue()
        Dim workingDirectory As String = TemporaryDirectory()
        Try
            Dim destination As String = Path.Combine(workingDirectory, "catalogue.xml")
            Dim original As Byte() = New UTF8Encoding(False).GetBytes("original catalogue bytes")
            File.WriteAllBytes(destination, original)
            For Each stage As String In New String() {"create", "write", "flush", "reopen", "backup", "replace", "cleanup"}
                File.WriteAllBytes(destination, original)
                AssertThrowsAny(Sub() frmMain.SaveCatalogueTransactional(active, destination, stage), "fault stage " & stage)
                AssertBytesEqual(original, File.ReadAllBytes(destination), "destination after " & stage)
                AssertEqual(0, Directory.GetFiles(workingDirectory, ".c3-save-*.tmp").Length, "temporary files after " & stage)
            Next
        Finally
            Directory.Delete(workingDirectory, True)
        End Try
    End Sub

    Private Sub TransactionalSaveReopensAndRetainsBackup()
        Dim active As DataSet = CreateBlankCatalogue()
        Dim workingDirectory As String = TemporaryDirectory()
        Try
            Dim destination As String = Path.Combine(workingDirectory, "catalogue.xml")
            File.WriteAllText(destination, "old catalogue")
            Dim revision As String = frmMain.SaveCatalogueTransactional(active, destination, Nothing)
            AssertEqual(True, revision.Length > 0, "saved revision")
            AssertEqual("old catalogue", File.ReadAllText(destination & ".bak"), "backup bytes")
            Dim reopened As DataSet = frmMain.LoadCatalogueSnapshot(destination, active, New String() {"1.1.0"})
            AssertEqual(active.Tables("Information").Rows.Count, reopened.Tables("Information").Rows.Count, "reopened information count")
        Finally
            Directory.Delete(workingDirectory, True)
        End Try
    End Sub

    Private Sub ExternalCatalogueEditsInvalidateRevision()
        Dim cataloguePath As String = TemporaryPath("revision.xml")
        Try
            File.Copy(FixturePath("valid", "blank.xml"), cataloguePath)
            Dim revision As String = frmMain.CaptureFileRevision(cataloguePath)
            AssertEqual(True, frmMain.FileRevisionMatches(cataloguePath, revision), "unchanged revision")
            File.AppendAllText(cataloguePath, " ")
            AssertEqual(False, frmMain.FileRevisionMatches(cataloguePath, revision), "external edit")
        Finally
            DeleteIfPresent(cataloguePath)
        End Try
    End Sub

    Private Sub TapeMappingIsNamedAndPreservesIdentity()
        Dim table As New DataTable("Tapes")
        table.Columns.Add("IdentifierShort", GetType(String))
        table.Columns.Add("Number", GetType(Integer))
        table.Columns.Add("Date", GetType(DateTime))
        table.Columns.Add("PeakA", GetType(Integer))
        table.Columns.Add("BiasCalA", GetType(Integer))
        table.Columns.Add("Notes", GetType(String))
        Dim created As New DateTime(2019, 8, 22, 12, 30, 0, DateTimeKind.Local)
        Dim row As DataRow = table.Rows.Add("TAPE-1", 1, created, 0, 0, "old")
        Dim values As New Dictionary(Of String, Object) From {
            {"PeakA", 7}, {"BiasCalA", -2}, {"Notes", "updated"}}
        frmMain.AssignTapeValues(row, values)
        AssertEqual("TAPE-1", CStr(row("IdentifierShort")), "short identity")
        AssertEqual(1, CInt(row("Number")), "sequence identity")
        AssertEqual(created, CDate(row("Date")), "creation date")
        AssertEqual(7, CInt(row("PeakA")), "peak mapping")
        AssertEqual(-2, CInt(row("BiasCalA")), "bias calibration mapping")
        AssertEqual("updated", CStr(row("Notes")), "notes mapping")

        Dim hostile As New Dictionary(Of String, Object) From {{"IdentifierShort", "changed"}}
        AssertThrowsInvalidOperation(Sub() frmMain.AssignTapeValues(row, hostile), "immutable tape identity")
    End Sub

    Private Sub MainFormUsesClosedPersistenceFlow()
        Dim sourcePath As String = Path.Combine(_repositoryRoot, "Compact Cassette Catalogue\frmMain.vb")
        Dim source As String = File.ReadAllText(sourcePath)
        AssertEqual(1, Regex.Matches(source, "dlgOpen\.ShowDialog\(\)").Count, "Open dialog count")
        AssertEqual(False, source.Contains("Application.Exit()"), "recursive Application.Exit")
        AssertEqual(False, source.Contains("catalogue.WriteXml(filePath)"), "direct active save")
        AssertEqual(False, source.Contains("catalogue.ReadXml(selectedPath)"), "direct active load")
        AssertEqual(False, source.Contains("Dim thisTape As Object()"), "positional tape write")
        AssertEqual(False, Regex.IsMatch(source, "models\.Rows\([^\r\n]+\)\(""Number""\)\s*=\s*number\s*-\s*1"), "decreasing model tape sequence")
        AssertEqual(True, source.Contains("tapeCount = tapes.Rows.Count"), "derived global tape count")
    End Sub

    Private Sub LifecycleStateMachineExhaustivelyFailsClosed()
        Dim editChoices As frmMain.EditChoice() = {
            frmMain.EditChoice.Apply, frmMain.EditChoice.Discard, frmMain.EditChoice.Cancel}
        Dim documentChoices As frmMain.DocumentChoice() = {
            frmMain.DocumentChoice.Save, frmMain.DocumentChoice.Discard, frmMain.DocumentChoice.Cancel}
        For Each pending As Boolean In New Boolean() {False, True}
            For Each editChoice As frmMain.EditChoice In editChoices
                For Each editSucceeded As Boolean In New Boolean() {False, True}
                    For Each dirty As Boolean In New Boolean() {False, True}
                        For Each documentChoice As frmMain.DocumentChoice In documentChoices
                            For Each saveSucceeded As Boolean In New Boolean() {False, True}
                                Dim expected As Boolean =
                                    (Not pending OrElse
                                     (editChoice <> frmMain.EditChoice.Cancel AndAlso
                                      (editChoice <> frmMain.EditChoice.Apply OrElse editSucceeded))) AndAlso
                                    (Not dirty OrElse
                                     (documentChoice <> frmMain.DocumentChoice.Cancel AndAlso
                                      (documentChoice <> frmMain.DocumentChoice.Save OrElse saveSucceeded)))
                                Dim actual As Boolean = frmMain.TransitionCanContinue(
                                    pending, editChoice, editSucceeded, dirty, documentChoice, saveSucceeded)
                                AssertEqual(expected, actual, "lifecycle combination")
                            Next
                        Next
                    Next
                Next
            Next
        Next
    End Sub

    Private Sub ProductionSchemaLoadsEveryCatalogueTable()
        Dim schema As DataSet = CreateProductionCatalogueSchema()
        Dim revision As String = Nothing
        Dim loaded As DataSet = frmMain.LoadCatalogueSnapshot(
            FixturePath("valid", "populated.xml"), schema, New String() {"1.1.0"}, revision)
        AssertEqual(6, loaded.Tables.Count, "production table count")
        AssertEqual(1, loaded.Tables("Decks").Rows.Count, "production deck count")
        AssertEqual(1, loaded.Tables("Brands").Rows.Count, "production brand count")
        AssertEqual(1, loaded.Tables("Models").Rows.Count, "production model count")
        AssertEqual(1, loaded.Tables("Tapes").Rows.Count, "production tape count")
        AssertEqual(GetType(Decimal), loaded.Tables("Tapes").Columns("LevelCalA").DataType, "level calibration type")
        AssertEqual(True, revision.Length = 64, "loaded revision shape")
    End Sub

    Private Sub TypedLoaderRejectsInvalidFieldValues()
        Dim schema As DataSet = CreateProductionCatalogueSchema()
        Dim invalidPath As String = TemporaryPath("invalid-type.xml")
        Try
            Dim xml As String = File.ReadAllText(FixturePath("valid", "populated.xml"))
            xml = xml.Replace("<Year>1990</Year>", "<Year>not-a-year</Year>")
            File.WriteAllText(invalidPath, xml, New UTF8Encoding(False))
            AssertThrowsAny(
                Sub() frmMain.LoadCatalogueSnapshot(invalidPath, schema, New String() {"1.1.0"}),
                "typed invalid value")
        Finally
            DeleteIfPresent(invalidPath)
        End Try
    End Sub

    Private Sub CreationPlannerClosesPrerequisites()
        AssertSteps(
            New CatalogueCreationStep() {
                CatalogueCreationStep.Brand,
                CatalogueCreationStep.Model,
                CatalogueCreationStep.Tape},
            CatalogueWorkflow.Plan(CatalogueCreationIntent.AddTape, False, False, False),
            "blank Add Tape")
        AssertSteps(
            New CatalogueCreationStep() {
                CatalogueCreationStep.Model,
                CatalogueCreationStep.Tape},
            CatalogueWorkflow.Plan(CatalogueCreationIntent.AddTape, True, False, False),
            "brand-only Add Tape")
        AssertSteps(
            New CatalogueCreationStep() {CatalogueCreationStep.Tape},
            CatalogueWorkflow.Plan(CatalogueCreationIntent.AddTape, True, True, False),
            "ready Add Tape")
        AssertSteps(
            New CatalogueCreationStep() {
                CatalogueCreationStep.Brand,
                CatalogueCreationStep.Model},
            CatalogueWorkflow.Plan(CatalogueCreationIntent.AddModel, False, False, False),
            "blank Add Model")
        AssertSteps(
            New CatalogueCreationStep() {CatalogueCreationStep.Model},
            CatalogueWorkflow.Plan(CatalogueCreationIntent.AddModel, True, False, False),
            "ready Add Model")
        AssertSteps(
            New CatalogueCreationStep() {
                CatalogueCreationStep.Deck,
                CatalogueCreationStep.RecordSide},
            CatalogueWorkflow.Plan(CatalogueCreationIntent.RecordSide, True, True, False),
            "record without deck")
        AssertSteps(
            New CatalogueCreationStep() {CatalogueCreationStep.RecordSide},
            CatalogueWorkflow.Plan(CatalogueCreationIntent.RecordSide, True, True, True),
            "record with deck")
    End Sub

    Private Sub CatalogueChoicesPreserveStableIdentity()
        Dim choice As New CatalogueChoice("MAX-2-XL", "Maxell XLII")
        AssertEqual("MAX-2-XL", choice.Key, "choice key")
        AssertEqual("Maxell XLII", choice.Text, "choice text")
        AssertEqual("Maxell XLII", choice.ToString(), "choice display")
    End Sub

    Private Sub CreationDialogsExposeResultContracts()
        For Each formType As Type In New Type() {
                GetType(frmBrandNew), GetType(frmModelNew),
                GetType(frmDeckNew), GetType(frmTapeNew)}
            AssertEqual(True, formType.GetProperty("CreatedKey") IsNot Nothing, formType.Name & " CreatedKey")
            AssertEqual(True, formType.GetProperty("CreatedDisplayName") IsNot Nothing, formType.Name & " CreatedDisplayName")
            AssertEqual(True, formType.GetProperty("SuppressSuccessMessage") IsNot Nothing, formType.Name & " SuppressSuccessMessage")
        Next
    End Sub

    Private Sub GuidedCreationHasNoDeadEnds()
        Dim mainSource As String = File.ReadAllText(Path.Combine(
            _repositoryRoot, "Compact Cassette Catalogue\frmMain.vb"))
        Dim workflowSource As String = File.ReadAllText(Path.Combine(
            _repositoryRoot, "Compact Cassette Catalogue\CatalogueWorkflow.vb"))
        AssertEqual(False, mainSource.Contains("Add at least one brand first."), "brand blocker removed")
        AssertEqual(False, mainSource.Contains("Add at least one model first."), "model blocker removed")
        AssertEqual(False, Regex.IsMatch(mainSource, "frm(Brand|Model|Deck|Tape)New\.Show\(\)"), "default creation forms removed")
        AssertEqual(True, workflowSource.Contains("ShowDialog(owner)"), "owned modal creation")
        AssertEqual(True, workflowSource.Contains("AddModelWithPrerequisites"), "model prerequisite journey")
        AssertEqual(True, workflowSource.Contains("Public Function AddTape"), "tape journey")
    End Sub

    Private Sub TapeDetoursPreserveActiveForm()
        Dim tapeSource As String = File.ReadAllText(Path.Combine(
            _repositoryRoot, "Compact Cassette Catalogue\frmTapeNew.vb"))
        AssertEqual(True, tapeSource.Contains("CreateModelForDetour(Me)"), "model detour owns tape form")
        AssertEqual(True, tapeSource.Contains("CreateDeckForDetour(Me)"), "deck detour owns tape form")
        AssertEqual(True, tapeSource.Contains("ReloadModelChoices(createdModel.Key)"), "new model selected")
        AssertEqual(True, tapeSource.Contains("ReloadDeckChoices(createdDeck.Key)"), "new deck selected")
        AssertEqual(False, tapeSource.Contains("Must add a deck before entering recordings."), "recording dead end removed")
    End Sub

    Private Sub AssertSteps(
            expected As CatalogueCreationStep(),
            actual As IList(Of CatalogueCreationStep),
            name As String)
        AssertEqual(expected.Length, actual.Count, name & " count")
        For index As Integer = 0 To expected.Length - 1
            AssertEqual(expected(index), actual(index), name & " step " & index.ToString(CultureInfo.InvariantCulture))
        Next
    End Sub

    Private Sub ValidateAgainstSchema(xmlPath As String)
        Dim validationMessages As New List(Of String)()
        Dim settings As XmlReaderSettings = CreateSecureReaderSettings()
        settings.ValidationType = ValidationType.Schema
        settings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings
        settings.Schemas.Add(Nothing, Path.Combine(_repositoryRoot, "spec\catalogue\v1.1.0\catalogue.xsd"))
        AddHandler settings.ValidationEventHandler,
            Sub(sender As Object, args As ValidationEventArgs)
                validationMessages.Add(args.Message)
            End Sub

        Using reader As XmlReader = XmlReader.Create(xmlPath, settings)
            While reader.Read()
            End While
        End Using

        If validationMessages.Count > 0 Then
            Throw New InvalidOperationException(String.Join(Environment.NewLine, validationMessages.ToArray()))
        End If
    End Sub

    Private Function LoadSecureDocument(path As String) As XmlDocument
        Dim document As New XmlDocument()
        document.XmlResolver = Nothing
        Using reader As XmlReader = XmlReader.Create(path, CreateSecureReaderSettings())
            document.Load(reader)
        End Using
        Return document
    End Function

    Private Function CreateSecureReaderSettings() As XmlReaderSettings
        Dim settings As New XmlReaderSettings()
        settings.DtdProcessing = DtdProcessing.Prohibit
        settings.XmlResolver = Nothing
        settings.MaxCharactersInDocument = 16L * 1024L * 1024L
        settings.MaxCharactersFromEntities = 0L
        Return settings
    End Function

    Private Function ReadFileVersion(document As XmlDocument) As String
        Dim node As XmlNode = document.SelectSingleNode(
            "/Catalogue/Information[normalize-space(Information)='File Version']/Value")
        If node Is Nothing Then
            Return Nothing
        End If
        Return NormalizeVersion(node.InnerText)
    End Function

    Private Function NormalizeVersion(value As String) As String
        If value Is Nothing Then
            Return Nothing
        End If

        Dim match As Match = Regex.Match(value.Trim(), "^(\d+)\.(\d+)\.(\d+)")
        If match.Success Then
            Return match.Groups(1).Value & "." & match.Groups(2).Value & "." & match.Groups(3).Value
        End If
        Return value.Trim()
    End Function

    Private Function NodeText(document As XmlDocument, xpath As String) As String
        Dim node As XmlNode = document.SelectSingleNode(xpath)
        If node Is Nothing Then
            Throw New InvalidOperationException("Missing expected node: " & xpath)
        End If
        Return node.InnerText
    End Function

    Private Function FixturePath(group As String, fileName As String) As String
        Return Path.Combine(_repositoryRoot, "fixtures\catalogues\v1.1.0", group, fileName)
    End Function

    Private Function TemporaryDirectory() As String
        Dim temporaryDirectoryPath As String = Path.Combine(Path.GetTempPath(), "c3-characterization-" & Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(temporaryDirectoryPath)
        Return temporaryDirectoryPath
    End Function

    Private Function CreateBlankCatalogue() As DataSet
        Dim result As New DataSet("Catalogue")
        Dim information As New DataTable("Information")
        information.Columns.Add("Information", GetType(String))
        information.Columns.Add("Value", GetType(String))
        information.PrimaryKey = New DataColumn() {information.Columns("Information")}
        Dim counters As New DataTable("Counters")
        counters.Columns.Add("Counter", GetType(String))
        counters.Columns.Add("Number", GetType(Integer))
        counters.PrimaryKey = New DataColumn() {counters.Columns("Counter")}
        result.Tables.Add(information)
        result.Tables.Add(counters)
        result.ReadXml(FixturePath("valid", "blank.xml"), XmlReadMode.IgnoreSchema)
        Return result
    End Function

    Private Function CreateProductionCatalogueSchema() As DataSet
        Dim result As New DataSet("Catalogue")
        Dim globalsType As Type = GetType(frmMain).Assembly.GetType("Compact_Cassette_Catalogue.varGlobals", True)
        For Each fieldName As String In New String() {"information", "counters", "decks", "brands", "models", "tapes"}
            Dim field As FieldInfo = globalsType.GetField(fieldName, BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Static)
            If field Is Nothing Then
                Throw New InvalidOperationException("Missing production table field: " & fieldName)
            End If
            result.Tables.Add(DirectCast(field.GetValue(Nothing), DataTable).Clone())
        Next
        Return result
    End Function

    Private Function TemporaryPath(fileName As String) As String
        Dim directory As String = TemporaryDirectory()
        Return Path.Combine(directory, fileName)
    End Function

    Private Sub DeleteIfPresent(fileToDelete As String)
        Dim parentDirectory As String = Path.GetDirectoryName(fileToDelete)
        If File.Exists(fileToDelete) Then
            File.Delete(fileToDelete)
        End If
        If Directory.Exists(parentDirectory) Then
            Directory.Delete(parentDirectory, True)
        End If
    End Sub

    Private Function FindRepositoryRoot() As String
        Dim directory As New DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)
        While directory IsNot Nothing
            If System.IO.File.Exists(Path.Combine(directory.FullName, "VERSION")) AndAlso
                    System.IO.Directory.Exists(Path.Combine(directory.FullName, "spec\catalogue")) Then
                Return directory.FullName
            End If
            directory = directory.Parent
        End While
        Throw New DirectoryNotFoundException("Could not locate the C3 repository root.")
    End Function

    Private Sub AssertThrowsXmlException(action As Action, name As String)
        Try
            action()
        Catch ex As XmlException
            Return
        End Try
        Throw New InvalidOperationException(name & " did not throw XmlException.")
    End Sub

    Private Sub AssertThrowsAny(action As Action, name As String)
        Try
            action()
        Catch
            Return
        End Try
        Throw New InvalidOperationException(name & " did not throw an exception.")
    End Sub

    Private Sub AssertThrowsInvalidData(action As Action, name As String)
        Try
            action()
        Catch ex As InvalidDataException
            Return
        End Try
        Throw New InvalidOperationException(name & " did not throw InvalidDataException.")
    End Sub

    Private Sub AssertThrowsInvalidOperation(action As Action, name As String)
        Try
            action()
        Catch ex As InvalidOperationException
            Return
        End Try
        Throw New InvalidOperationException(name & " did not throw InvalidOperationException.")
    End Sub

    Private Sub AssertBytesEqual(expected As Byte(), actual As Byte(), name As String)
        If expected.Length <> actual.Length Then
            Throw New InvalidOperationException(name & ": byte lengths differ.")
        End If
        For index As Integer = 0 To expected.Length - 1
            If expected(index) <> actual(index) Then
                Throw New InvalidOperationException(name & ": bytes differ at index " & index.ToString(CultureInfo.InvariantCulture) & ".")
            End If
        Next
    End Sub

    Private Sub AssertEqual(Of T)(expected As T, actual As T, name As String)
        If Not EqualityComparer(Of T).Default.Equals(expected, actual) Then
            Throw New InvalidOperationException(
                String.Format("{0}: expected '{1}', found '{2}'.", name, expected, actual))
        End If
    End Sub

End Module
