using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace C3.Domain.Identity
{
    /// <summary>
    /// Reproducible generator for fixtures, migrations, and property tests.
    /// Production creation uses <see cref="RandomEntityIdGenerator"/>.
    /// </summary>
    public sealed class DeterministicEntityIdGenerator : IEntityIdGenerator
    {
        private readonly byte[] seed;
        private long sequence;

        public DeterministicEntityIdGenerator(string seed)
        {
            if (string.IsNullOrWhiteSpace(seed))
            {
                throw new ArgumentException("A deterministic generator seed is required.", nameof(seed));
            }

            this.seed = Encoding.UTF8.GetBytes(seed);
        }

        public EntityId<TAggregate> Next<TAggregate>()
        {
            checked
            {
                sequence++;
            }

            var typeName = typeof(TAggregate).AssemblyQualifiedName ?? typeof(TAggregate).FullName;
            var sequenceText = sequence.ToString(CultureInfo.InvariantCulture);
            var discriminator = Encoding.UTF8.GetBytes(typeName + "\n" + sequenceText);
            var input = new byte[seed.Length + 1 + discriminator.Length];
            Buffer.BlockCopy(seed, 0, input, 0, seed.Length);
            input[seed.Length] = 0;
            Buffer.BlockCopy(discriminator, 0, input, seed.Length + 1, discriminator.Length);

            byte[] hash;
            using (var sha256 = SHA256.Create())
            {
                hash = sha256.ComputeHash(input);
            }

            var bytes = new byte[16];
            Buffer.BlockCopy(hash, 0, bytes, 0, bytes.Length);
            bytes[7] = (byte)((bytes[7] & 0x0f) | 0x50);
            bytes[8] = (byte)((bytes[8] & 0x3f) | 0x80);
            return new EntityId<TAggregate>(new Guid(bytes));
        }
    }
}
