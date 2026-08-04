using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace C3.Domain.Commands
{
    public sealed class ChangeSet
    {
        public ChangeSet(long versionBefore, long versionAfter, IEnumerable<Change> changes)
        {
            if (versionBefore < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(versionBefore));
            }

            if (versionAfter <= versionBefore)
            {
                throw new ArgumentOutOfRangeException(nameof(versionAfter));
            }

            if (changes == null)
            {
                throw new ArgumentNullException(nameof(changes));
            }

            var copy = changes.ToList();
            if (copy.Count == 0 || copy.Any(change => change == null))
            {
                throw new ArgumentException("A change set requires one or more non-null changes.", nameof(changes));
            }

            VersionBefore = versionBefore;
            VersionAfter = versionAfter;
            Changes = new ReadOnlyCollection<Change>(copy);
        }

        public long VersionBefore { get; }

        public long VersionAfter { get; }

        public ReadOnlyCollection<Change> Changes { get; }
    }
}
