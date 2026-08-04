using System;
using C3.Domain.Identity;
using C3.Domain.Time;
using C3.Domain.Values;

namespace C3.Domain.Commands
{
    public sealed class CommandContext
    {
        public CommandContext(
            EntityId<CommandContext> commandId,
            UtcTimestamp issuedAt,
            Optional<long> expectedVersion)
        {
            if (commandId.IsEmpty)
            {
                throw new ArgumentException("A command identifier is required.", nameof(commandId));
            }

            if (expectedVersion.HasValue && expectedVersion.Value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(expectedVersion));
            }

            CommandId = commandId;
            IssuedAt = issuedAt;
            ExpectedVersion = expectedVersion;
        }

        public EntityId<CommandContext> CommandId { get; }

        public UtcTimestamp IssuedAt { get; }

        public Optional<long> ExpectedVersion { get; }
    }
}
