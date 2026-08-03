Namespace Updates

    ''' <summary>
    ''' Supplies one validated update-manifest read result.
    ''' Implementations own retrieval; the update service owns release evaluation.
    ''' </summary>
    Public Interface IUpdateManifestSource

        Function Read(
            feedUri As Uri,
            expectedChannel As String) As UpdateManifestReadResult

    End Interface

End Namespace
