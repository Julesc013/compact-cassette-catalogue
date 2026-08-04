using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using C3.Domain.Validation;

namespace C3.Domain.Commands
{
    public sealed class CommandResult<T>
    {
        private CommandResult(
            bool isSuccess,
            T value,
            ChangeSet changeSet,
            IEnumerable<ValidationIssue> issues)
        {
            var issueCopy = issues.ToList();
            if (issueCopy.Any(issue => issue == null))
            {
                throw new ArgumentException("Command issues cannot contain null entries.", nameof(issues));
            }

            IsSuccess = isSuccess;
            Value = value;
            ChangeSet = changeSet;
            Issues = new ReadOnlyCollection<ValidationIssue>(issueCopy);
        }

        public bool IsSuccess { get; }

        public T Value { get; }

        public ChangeSet ChangeSet { get; }

        public ReadOnlyCollection<ValidationIssue> Issues { get; }

        public static CommandResult<T> Success(T value, ChangeSet changeSet)
        {
            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (changeSet == null)
            {
                throw new ArgumentNullException(nameof(changeSet));
            }

            return new CommandResult<T>(true, value, changeSet, new ValidationIssue[0]);
        }

        public static CommandResult<T> Rejected(IEnumerable<ValidationIssue> issues)
        {
            if (issues == null)
            {
                throw new ArgumentNullException(nameof(issues));
            }

            var copy = issues.ToList();
            if (copy.Count == 0)
            {
                throw new ArgumentException("A rejected command requires at least one issue.", nameof(issues));
            }

            return new CommandResult<T>(false, default(T), null, copy);
        }
    }
}
