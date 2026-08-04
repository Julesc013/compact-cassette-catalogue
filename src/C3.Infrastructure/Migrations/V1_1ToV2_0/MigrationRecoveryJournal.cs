using C3.Infrastructure.FileOperations;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace C3.Infrastructure.Migrations.V1_1ToV2_0
{
    internal sealed class MigrationRecoveryJournal
    {
        internal string State { get; set; }
        internal string SourcePath { get; set; }
        internal string SourceRevision { get; set; }
        internal string DestinationPath { get; set; }
        internal string DestinationRevision { get; set; }
        internal string JsonReportPath { get; set; }
        internal string TextReportPath { get; set; }

        internal static MigrationRecoveryJournal Read(string path)
        {
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                MaxCharactersInDocument = 32768
            };
            var document = new XmlDocument { XmlResolver = null };
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = XmlReader.Create(stream, settings)) document.Load(reader);
            var root = document.DocumentElement;
            if (root == null || root.Name != "migrationRecovery" || root.GetAttribute("schemaVersion") != "1")
                throw new InvalidDataException("The migration recovery journal is not a supported C3 journal.");
            var source = RequiredChild(root, "source");
            var destination = RequiredChild(root, "destination");
            var reports = RequiredChild(root, "reports");
            return new MigrationRecoveryJournal
            {
                State = Required(root, "state"),
                SourcePath = Required(source, "path"),
                SourceRevision = Required(source, "revision"),
                DestinationPath = Required(destination, "path"),
                DestinationRevision = destination.GetAttribute("revision"),
                JsonReportPath = Required(reports, "jsonPath"),
                TextReportPath = Required(reports, "textPath")
            };
        }

        internal void Create(string path)
        {
            using (var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                Write(stream);
                stream.Flush(true);
            }
        }

        internal void Update(string path)
        {
            using (var temporary = OwnedSiblingTemporaryFile.Create(path))
            {
                Write(temporary.Stream);
                temporary.Stream.Flush(true);
                temporary.Stream.Dispose();
                File.Replace(temporary.Path, path, null, true);
            }
        }

        private void Write(Stream stream)
        {
            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false, true),
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\n",
                NewLineHandling = NewLineHandling.Replace,
                OmitXmlDeclaration = false,
                CloseOutput = false
            };
            using (var writer = XmlWriter.Create(stream, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("migrationRecovery");
                writer.WriteAttributeString("schemaVersion", "1");
                writer.WriteAttributeString("state", State);
                Element(writer, "source", SourcePath, SourceRevision);
                Element(writer, "destination", DestinationPath, DestinationRevision ?? string.Empty);
                writer.WriteStartElement("reports");
                writer.WriteAttributeString("jsonPath", JsonReportPath);
                writer.WriteAttributeString("textPath", TextReportPath);
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private static void Element(XmlWriter writer, string name, string path, string revision)
        {
            writer.WriteStartElement(name);
            writer.WriteAttributeString("path", path);
            writer.WriteAttributeString("revision", revision);
            writer.WriteEndElement();
        }

        private static XmlElement RequiredChild(XmlElement parent, string name)
        {
            var nodes = parent.GetElementsByTagName(name);
            if (nodes.Count != 1 || nodes[0].ParentNode != parent)
                throw new InvalidDataException("The migration recovery journal is missing " + name + ".");
            return (XmlElement)nodes[0];
        }

        private static string Required(XmlElement element, string name)
        {
            var value = element.GetAttribute(name);
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidDataException("The migration recovery journal is missing " + name + ".");
            return value;
        }
    }
}
