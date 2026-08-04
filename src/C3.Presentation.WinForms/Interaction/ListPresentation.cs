using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace C3.Presentation.WinForms.Interaction
{
    public sealed class ListPresentation<T>
    {
        private ReadOnlyCollection<T> items = new List<T>().AsReadOnly();

        public ReadOnlyCollection<T> Items => items;

        public int Count => items.Count;

        public EmptyStatePresentation EmptyState { get; private set; } =
            EmptyStatePresentation.Hidden();

        public void Replace(
            IEnumerable<T> values,
            string emptyTitle,
            string emptyGuidance)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            items = values.ToList().AsReadOnly();
            EmptyState = items.Count == 0
                ? EmptyStatePresentation.Show(emptyTitle, emptyGuidance)
                : EmptyStatePresentation.Hidden();
        }
    }
}
