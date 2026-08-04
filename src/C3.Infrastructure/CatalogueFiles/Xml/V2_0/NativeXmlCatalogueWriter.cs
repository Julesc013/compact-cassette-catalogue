using C3.Catalogue.Native;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace C3.Infrastructure.CatalogueFiles.Xml.V2_0
{
    public sealed class NativeXmlCatalogueWriter
    {
        public const string FormatVersion = "2.0.0";
        public const string NamespaceUri = "urn:c3:catalogue:2";

        public byte[] Write(NativeCatalogue document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            using (var stream = new MemoryStream())
            {
                Write(stream, document);
                return stream.ToArray();
            }
        }

        public void Write(Stream destination, NativeCatalogue document)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false),
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\n",
                NewLineHandling = NewLineHandling.Replace,
                OmitXmlDeclaration = false,
                CloseOutput = false
            };
            using (var writer = XmlWriter.Create(destination, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("catalogue", NamespaceUri);
                writer.WriteAttributeString("format", FormatVersion);
                writer.WriteAttributeString("id", document.Id.ToString());
                WriteMetadata(writer, document.Metadata);
                WriteBrands(writer, document);
                WriteCassetteModels(writer, document);
                WriteDeckModels(writer, document);
                WriteDeckUnits(writer, document);
                WriteTapes(writer, document);
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            destination.WriteByte((byte)'\n');
        }

        private static void WriteMetadata(XmlWriter writer, NativeCatalogueMetadata metadata)
        {
            writer.WriteStartElement("metadata", NamespaceUri);
            Text(writer, "producer", metadata.Producer);
            Timestamp(writer, "createdUtc", metadata.CreatedAt.Value);
            Timestamp(writer, "modifiedUtc", metadata.ModifiedAt.Value);
            if (metadata.Provenance.HasValue)
            {
                var provenance = metadata.Provenance.Value;
                writer.WriteStartElement("provenance", NamespaceUri);
                Text(writer, "sourceFormat", provenance.SourceFormat);
                Text(writer, "sourceRevision", provenance.SourceRevision);
                Text(writer, "migrationProfile", provenance.MigrationProfile);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private static void WriteBrands(XmlWriter writer, NativeCatalogue document)
        {
            writer.WriteStartElement("brands", NamespaceUri);
            foreach (var brand in document.Brands)
            {
                writer.WriteStartElement("brand", NamespaceUri);
                writer.WriteAttributeString("id", brand.Id.ToString());
                Text(writer, "name", brand.Name);
                Text(writer, "legacyCode", brand.LegacyCode);
                Timestamp(writer, "addedUtc", brand.AddedAt.Value);
                Text(writer, "notes", brand.Notes);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private static void WriteCassetteModels(XmlWriter writer, NativeCatalogue document)
        {
            writer.WriteStartElement("cassetteModels", NamespaceUri);
            foreach (var model in document.CassetteModels)
            {
                writer.WriteStartElement("cassetteModel", NamespaceUri);
                writer.WriteAttributeString("id", model.Id.ToString());
                Text(writer, "brandId", model.BrandId.ToString());
                Integer(writer, "typeNumber", model.TypeNumber);
                Text(writer, "modelName", model.ModelName);
                Text(writer, "legacyCode", model.LegacyCode);
                Text(writer, "legacyIdentifier", model.LegacyIdentifier);
                Text(writer, "displayName", model.DisplayName);
                Timestamp(writer, "addedUtc", model.AddedAt.Value);
                Text(writer, "notes", model.Notes);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private static void WriteDeckModels(XmlWriter writer, NativeCatalogue document)
        {
            writer.WriteStartElement("deckModels", NamespaceUri);
            foreach (var model in document.DeckModels)
            {
                writer.WriteStartElement("deckModel", NamespaceUri);
                writer.WriteAttributeString("id", model.Id.ToString());
                Text(writer, "manufacturer", model.Manufacturer);
                Text(writer, "model", model.Model);
                Integer(writer, "year", model.Year);
                WriteCapabilities(writer, model.Capabilities);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private static void WriteCapabilities(XmlWriter writer, NativeDeckCapabilities value)
        {
            writer.WriteStartElement("capabilities", NamespaceUri);
            Boolean(writer, "type1", value.Type1); Boolean(writer, "type2", value.Type2);
            Boolean(writer, "type3", value.Type3); Boolean(writer, "type4", value.Type4);
            Boolean(writer, "hx", value.Hx); Boolean(writer, "mpx", value.Mpx);
            Boolean(writer, "dolbyB", value.DolbyB); Boolean(writer, "dolbyC", value.DolbyC);
            Boolean(writer, "dolbyS", value.DolbyS); Boolean(writer, "dbx1", value.Dbx1);
            Boolean(writer, "dbx2", value.Dbx2); Boolean(writer, "stereo", value.Stereo);
            Boolean(writer, "programSearch", value.ProgramSearch); Boolean(writer, "reverse", value.Reverse);
            Boolean(writer, "calibration", value.Calibration); Boolean(writer, "azimuth", value.Azimuth);
            Boolean(writer, "dubbingSlow", value.DubbingSlow); Boolean(writer, "dubbingFast", value.DubbingFast);
            Integer(writer, "frequencyLow", value.FrequencyLow); Integer(writer, "frequencyHigh", value.FrequencyHigh);
            Integer(writer, "signalRatio", value.SignalRatio);
            Text(writer, "signalRatioNoiseReduction", value.SignalRatioNoiseReduction);
            Decimal(writer, "wowFlutter", value.WowFlutter); Decimal(writer, "distortion", value.Distortion);
            Integer(writer, "heads", value.Heads); Integer(writer, "wells", value.Wells);
            Boolean(writer, "speedSlow", value.SpeedSlow); Boolean(writer, "speedNormal", value.SpeedNormal);
            Boolean(writer, "speedFast", value.SpeedFast);
            writer.WriteEndElement();
        }

        private static void WriteDeckUnits(XmlWriter writer, NativeCatalogue document)
        {
            writer.WriteStartElement("deckUnits", NamespaceUri);
            foreach (var deck in document.DeckUnits)
            {
                writer.WriteStartElement("deckUnit", NamespaceUri);
                writer.WriteAttributeString("id", deck.Id.ToString());
                Text(writer, "deckModelId", deck.DeckModelId.ToString());
                Text(writer, "name", deck.Name);
                Text(writer, "legacyKey", deck.LegacyKey);
                Integer(writer, "condition", deck.Condition);
                Timestamp(writer, "addedUtc", deck.AddedAt.Value);
                Text(writer, "notes", deck.Notes);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private static void WriteTapes(XmlWriter writer, NativeCatalogue document)
        {
            writer.WriteStartElement("tapes", NamespaceUri);
            foreach (var tape in document.Tapes)
            {
                writer.WriteStartElement("tape", NamespaceUri);
                writer.WriteAttributeString("id", tape.Id.ToString());
                Text(writer, "cassetteModelId", tape.CassetteModelId.ToString());
                Integer(writer, "year", tape.Year);
                Decimal(writer, "lengthMinutes", tape.LengthMinutes);
                Text(writer, "region", tape.Region);
                Integer(writer, "number", tape.Number);
                Text(writer, "legacyIdentifier", tape.LegacyIdentifier);
                Text(writer, "legacyShortIdentifier", tape.LegacyShortIdentifier);
                Integer(writer, "condition", tape.Condition);
                Boolean(writer, "packaged", tape.Packaged);
                Timestamp(writer, "addedUtc", tape.AddedAt.Value);
                Text(writer, "notes", tape.Notes);
                writer.WriteStartElement("sides", NamespaceUri);
                WriteSide(writer, tape.SideA);
                WriteSide(writer, tape.SideB);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private static void WriteSide(XmlWriter writer, NativeTapeSide side)
        {
            writer.WriteStartElement("side", NamespaceUri);
            writer.WriteAttributeString("position", side.Position.ToString());
            Text(writer, "name", side.Name);
            if (side.Recording.HasValue)
            {
                var recording = side.Recording.Value;
                writer.WriteStartElement("recording", NamespaceUri);
                writer.WriteAttributeString("id", recording.Id.ToString());
                if (recording.DeckUnitId.HasValue)
                {
                    Text(writer, "deckUnitId", recording.DeckUnitId.Value.ToString());
                }
                Timestamp(writer, "recordedUtc", recording.RecordedAt.Value);
                Text(writer, "inputName", recording.InputName);
                Integer(writer, "peakLevel", recording.PeakLevel);
                Text(writer, "noiseReduction", recording.NoiseReduction);
                Boolean(writer, "hx", recording.Hx); Boolean(writer, "mpx", recording.Mpx);
                Boolean(writer, "dubbed", recording.Dubbed); Text(writer, "speed", recording.Speed);
                Integer(writer, "bias", recording.Bias); Integer(writer, "biasCalibration", recording.BiasCalibration);
                Text(writer, "equalization", recording.Equalization); Decimal(writer, "level", recording.Level);
                Decimal(writer, "levelCalibration", recording.LevelCalibration); Text(writer, "contents", recording.Contents);
                Text(writer, "artist", recording.Artist); Text(writer, "title", recording.Title);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private static void Text(XmlWriter writer, string name, string value)
        {
            writer.WriteElementString(name, NamespaceUri, value ?? string.Empty);
        }

        private static void Integer(XmlWriter writer, string name, int value)
        {
            Text(writer, name, value.ToString(CultureInfo.InvariantCulture));
        }

        private static void Decimal(XmlWriter writer, string name, decimal value)
        {
            Text(writer, name, value.ToString("0.############################", CultureInfo.InvariantCulture));
        }

        private static void Boolean(XmlWriter writer, string name, bool value)
        {
            Text(writer, name, value ? "true" : "false");
        }

        private static void Timestamp(XmlWriter writer, string name, DateTime value)
        {
            Text(writer, name, value.ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'", CultureInfo.InvariantCulture));
        }
    }
}
