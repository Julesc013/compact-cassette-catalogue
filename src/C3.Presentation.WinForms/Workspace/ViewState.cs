using System;

namespace C3.Presentation.WinForms.Workspace
{
    public enum SortDirection
    {
        None = 0,
        Ascending = 1,
        Descending = 2
    }

    public sealed class ViewState : WorkspaceStateComponent
    {
        public string Feature { get; private set; }

        public string FilterText { get; private set; } = string.Empty;

        public string SortField { get; private set; } = string.Empty;

        public SortDirection SortDirection { get; private set; }

        public void Apply(
            string feature,
            string filterText,
            string sortField,
            SortDirection sortDirection)
        {
            if (string.IsNullOrWhiteSpace(feature))
            {
                throw new ArgumentException("A view feature is required.", nameof(feature));
            }

            if (!Enum.IsDefined(typeof(SortDirection), sortDirection))
            {
                throw new ArgumentOutOfRangeException(nameof(sortDirection));
            }

            var normalizedSortField = (sortField ?? string.Empty).Trim();
            if (sortDirection != SortDirection.None && normalizedSortField.Length == 0)
            {
                throw new ArgumentException(
                    "A sorted view requires a sort field.",
                    nameof(sortField));
            }

            Feature = feature.Trim();
            FilterText = (filterText ?? string.Empty).Trim();
            SortField = normalizedSortField;
            SortDirection = sortDirection;
            RaiseChanged();
        }

        public void Clear()
        {
            if (Feature == null && FilterText.Length == 0 &&
                SortField.Length == 0 && SortDirection == SortDirection.None)
            {
                return;
            }

            Feature = null;
            FilterText = string.Empty;
            SortField = string.Empty;
            SortDirection = SortDirection.None;
            RaiseChanged();
        }
    }
}
