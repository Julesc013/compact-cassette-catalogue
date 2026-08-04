namespace C3.Presentation.WinForms.Workspace
{
    /// <summary>
    /// Coordinates an already-defined catalogue mutation with history. Feature
    /// implementations call catalogue-owned services; controls are never captured.
    /// </summary>
    public interface IReversibleWorkspaceCommand
    {
        string Description { get; }

        WorkspaceCommandResult Execute();

        WorkspaceCommandResult Undo();
    }
}
