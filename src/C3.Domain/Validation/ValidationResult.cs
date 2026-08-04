using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace C3.Domain.Validation
{
    public sealed class ValidationResult
    {
        private static readonly ValidationResult valid = new ValidationResult(
            new ValidationIssue[0]);

        public ValidationResult(IEnumerable<ValidationIssue> issues)
        {
            if (issues == null)
            {
                throw new ArgumentNullException(nameof(issues));
            }

            var copy = issues.ToList();
            if (copy.Any(issue => issue == null))
            {
                throw new ArgumentException("Validation issues cannot contain null entries.", nameof(issues));
            }

            Issues = new ReadOnlyCollection<ValidationIssue>(copy);
        }

        public static ValidationResult Valid => valid;

        public bool IsValid => Issues.Count == 0;

        public ReadOnlyCollection<ValidationIssue> Issues { get; }
    }
}
