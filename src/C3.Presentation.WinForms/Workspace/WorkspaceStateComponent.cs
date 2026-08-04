using System;

namespace C3.Presentation.WinForms.Workspace
{
    /// <summary>
    /// Provides one notification contract for the small state owners composed by
    /// a workspace. It deliberately contains no application or catalogue rules.
    /// </summary>
    public abstract class WorkspaceStateComponent
    {
        public event EventHandler Changed;

        protected void RaiseChanged()
        {
            var handler = Changed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
