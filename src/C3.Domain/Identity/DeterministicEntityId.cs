using System;
using System.Security.Cryptography;
using System.Text;

namespace C3.Domain.Identity
{
    /// <summary>
    /// Creates a stable typed identity from a versioned namespace and canonical
    /// key. Unlike a sequence generator, this mapping is independent of input
    /// enumeration order and is suitable for repeatable migrations.
    /// </summary>
    public static class DeterministicEntityId
    {
        public static EntityId<TAggregate> FromCanonicalKey<TAggregate>(
            string namespaceId,
            string canonicalKey)
        {
            if (string.IsNullOrWhiteSpace(namespaceId))
            {
                throw new ArgumentException("A versioned identity namespace is required.", nameof(namespaceId));
            }
            if (canonicalKey == null)
            {
                throw new ArgumentNullException(nameof(canonicalKey));
            }

            var typeName = typeof(TAggregate).FullName;
            var input = Encoding.UTF8.GetBytes(namespaceId + "\n" + typeName + "\n" + canonicalKey);
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
