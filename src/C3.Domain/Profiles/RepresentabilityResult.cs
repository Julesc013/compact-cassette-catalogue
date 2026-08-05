using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace C3.Domain.Profiles
{
    public sealed class RepresentabilityResult
    {
        public RepresentabilityResult(
            CatalogueProfileCapabilities profile,
            RepresentationPurpose purpose,
            IEnumerable<RepresentationIssue> issues)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }
            if (issues == null)
            {
                throw new ArgumentNullException(nameof(issues));
            }

            var copy = issues.ToList();
            if (copy.Any(issue => issue == null))
            {
                throw new ArgumentException(
                    "Representation issues cannot contain null entries.",
                    nameof(issues));
            }

            Profile = profile;
            Purpose = purpose;
            Issues = new ReadOnlyCollection<RepresentationIssue>(copy);
        }

        public CatalogueProfileCapabilities Profile { get; }

        public RepresentationPurpose Purpose { get; }

        public ReadOnlyCollection<RepresentationIssue> Issues { get; }

        public bool CanRepresent
        {
            get
            {
                if (Purpose == RepresentationPurpose.DirectSave &&
                    !Profile.SupportsDirectSave)
                {
                    return false;
                }

                return !Issues.Any(
                    issue => issue.Effect == RepresentationEffect.Unsupported);
            }
        }

        public bool IsLossless => CanRepresent &&
            !Issues.Any(issue => issue.Effect == RepresentationEffect.InformationLoss);
    }
}
