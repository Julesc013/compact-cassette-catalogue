using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace C3.Presentation.WinForms.Workspace
{
    public sealed class SelectionState : WorkspaceStateComponent
    {
        private ReadOnlyCollection<string> selectedIds =
            new List<string>().AsReadOnly();

        public string Feature { get; private set; }

        public ReadOnlyCollection<string> SelectedIds => selectedIds;

        public void SelectOnly(string feature, string entityId)
        {
            Select(feature, new[] { entityId });
        }

        public void Select(string feature, IEnumerable<string> entityIds)
        {
            if (string.IsNullOrWhiteSpace(feature))
            {
                throw new ArgumentException("A selection feature is required.", nameof(feature));
            }

            if (entityIds == null)
            {
                throw new ArgumentNullException(nameof(entityIds));
            }

            var values = entityIds.Select(value => (value ?? string.Empty).Trim()).ToList();
            if (values.Any(string.IsNullOrWhiteSpace) ||
                values.Distinct(StringComparer.Ordinal).Count() != values.Count)
            {
                throw new ArgumentException(
                    "Selected identifiers must be non-empty and unique.",
                    nameof(entityIds));
            }

            Feature = feature.Trim();
            selectedIds = values.AsReadOnly();
            RaiseChanged();
        }

        public void Clear()
        {
            if (Feature == null && selectedIds.Count == 0)
            {
                return;
            }

            Feature = null;
            selectedIds = new List<string>().AsReadOnly();
            RaiseChanged();
        }
    }
}
