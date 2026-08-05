using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace C3.Domain.Profiles
{
    public sealed class CatalogueProfileCapabilities
    {
        private readonly HashSet<CatalogueProfileCapability> capabilities;

        public CatalogueProfileCapabilities(
            string profileCode,
            bool supportsDirectSave,
            IEnumerable<CatalogueProfileCapability> capabilities)
        {
            if (string.IsNullOrWhiteSpace(profileCode))
            {
                throw new ArgumentException(
                    "A stable catalogue profile code is required.",
                    nameof(profileCode));
            }
            if (capabilities == null)
            {
                throw new ArgumentNullException(nameof(capabilities));
            }

            ProfileCode = profileCode.Trim();
            SupportsDirectSave = supportsDirectSave;
            this.capabilities = new HashSet<CatalogueProfileCapability>(capabilities);
            Capabilities = new ReadOnlyCollection<CatalogueProfileCapability>(
                this.capabilities.OrderBy(item => item).ToList());
        }

        public string ProfileCode { get; }

        public bool SupportsDirectSave { get; }

        public ReadOnlyCollection<CatalogueProfileCapability> Capabilities { get; }

        public bool Supports(CatalogueProfileCapability capability)
        {
            return capabilities.Contains(capability);
        }
    }
}
