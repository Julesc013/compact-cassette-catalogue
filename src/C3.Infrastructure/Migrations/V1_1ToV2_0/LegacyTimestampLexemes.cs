using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace C3.Infrastructure.Migrations.V1_1ToV2_0
{
    internal sealed class LegacyTimestampLexemes
    {
        private readonly Dictionary<string, string> values =
            new Dictionary<string, string>(StringComparer.Ordinal);

        public static LegacyTimestampLexemes Load(string path)
        {
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                MaxCharactersInDocument = 67108864L,
                MaxCharactersFromEntities = 0
            };
            var document = new XmlDocument { XmlResolver = null };
            using (var reader = XmlReader.Create(path, settings))
            {
                document.Load(reader);
            }

            var result = new LegacyTimestampLexemes();
            result.ReadRows(document, "Information", "Information", "Value", "Information");
            result.ReadRows(document, "Brands", "Code", "Date", "Brands");
            result.ReadRows(document, "Models", "Identifier", "Date", "Models");
            result.ReadRows(document, "Decks", "Name", "Date", "Decks");
            result.ReadRows(document, "Tapes", "IdentifierShort", "Date", "Tapes");
            result.ReadRows(document, "Tapes", "IdentifierShort", "RecordedA", "Tapes");
            result.ReadRows(document, "Tapes", "IdentifierShort", "RecordedB", "Tapes");
            return result;
        }

        public string Get(string table, string key, string field)
        {
            string value;
            return values.TryGetValue(Key(table, key, field), out value) ? value : string.Empty;
        }

        private void ReadRows(
            XmlDocument document,
            string rowName,
            string keyName,
            string valueName,
            string table)
        {
            var nodes = document.SelectNodes("/Catalogue/" + rowName);
            if (nodes == null)
            {
                return;
            }
            foreach (XmlNode node in nodes)
            {
                var keyNode = node.SelectSingleNode(keyName);
                var valueNode = node.SelectSingleNode(valueName);
                if (keyNode != null && valueNode != null)
                {
                    values[Key(table, keyNode.InnerText, valueName)] = valueNode.InnerText;
                }
            }
        }

        private static string Key(string table, string key, string field)
        {
            return table + "\n" + key + "\n" + field;
        }
    }
}
