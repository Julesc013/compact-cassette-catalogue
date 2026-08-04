using System;

namespace C3.Presentation.WinForms.Workspace
{
    public enum CatalogueCompatibilityMode
    {
        LegacyV1_1 = 1,
        NativeV2_0 = 2
    }

    public sealed class CompatibilityState : WorkspaceStateComponent
    {
        internal CompatibilityState(CatalogueCompatibilityMode mode, bool isReadOnly)
        {
            Set(mode, isReadOnly, false);
        }

        public CatalogueCompatibilityMode Mode { get; private set; }

        public bool IsReadOnly { get; private set; }

        internal void Reset(CatalogueCompatibilityMode mode, bool isReadOnly)
        {
            Set(mode, isReadOnly, true);
        }

        private void Set(
            CatalogueCompatibilityMode mode,
            bool isReadOnly,
            bool notify)
        {
            if (!Enum.IsDefined(typeof(CatalogueCompatibilityMode), mode))
            {
                throw new ArgumentOutOfRangeException(nameof(mode));
            }

            var changed = Mode != mode || IsReadOnly != isReadOnly;
            Mode = mode;
            IsReadOnly = isReadOnly;
            if (notify && changed)
            {
                RaiseChanged();
            }
        }
    }
}
