using C3.Domain.Time;
using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace C3.Infrastructure.CatalogueFiles.Canonical
{
    internal sealed class CanonicalDigestWriter : IDisposable
    {
        private readonly MemoryStream stream = new MemoryStream();

        public void Boolean(bool value)
        {
            stream.WriteByte(value ? (byte)1 : (byte)0);
        }

        public void Decimal(decimal value)
        {
            String(value.ToString("G29", CultureInfo.InvariantCulture));
        }

        public void Int32(int value)
        {
            UInt32(unchecked((uint)value));
        }

        public void String(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var bytes = new UTF8Encoding(false, true).GetBytes(value);
            UInt32(checked((uint)bytes.Length));
            stream.Write(bytes, 0, bytes.Length);
        }

        public void Timestamp(UtcTimestamp value)
        {
            UInt64(unchecked((ulong)value.Value.Ticks));
        }

        public string Complete()
        {
            using (var algorithm = SHA256.Create())
            {
                var digest = algorithm.ComputeHash(stream.ToArray());
                var text = new StringBuilder(digest.Length * 2);
                foreach (var value in digest)
                {
                    text.Append(value.ToString("x2", CultureInfo.InvariantCulture));
                }

                return text.ToString();
            }
        }

        public void Dispose()
        {
            stream.Dispose();
        }

        private void UInt32(uint value)
        {
            stream.WriteByte((byte)(value >> 24));
            stream.WriteByte((byte)(value >> 16));
            stream.WriteByte((byte)(value >> 8));
            stream.WriteByte((byte)value);
        }

        private void UInt64(ulong value)
        {
            UInt32((uint)(value >> 32));
            UInt32((uint)value);
        }
    }
}
