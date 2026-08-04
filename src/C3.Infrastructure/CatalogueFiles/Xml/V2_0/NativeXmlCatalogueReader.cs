using C3.Catalogue.Native;
using C3.Domain.Identity;
using C3.Domain.Time;
using C3.Domain.Values;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace C3.Infrastructure.CatalogueFiles.Xml.V2_0
{
    public sealed class NativeXmlCatalogueReader
    {
        public const long MaximumCatalogueBytes = 67108864L;
        private const int MaximumDepth = 16;
        private const int MaximumElements = 1000000;
        private const int MaximumAttributes = 8;
        private const int MaximumScalarCharacters = 1048576;

        public NativeCatalogue Read(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            if (bytes.LongLength > MaximumCatalogueBytes)
            {
                throw new NativeXmlCatalogueException(
                    NativeCatalogueFileFailure.FileTooLarge,
                    "The catalogue exceeds the 64 MiB safety limit.");
            }

            try
            {
                Preflight(bytes);
                var document = Load(bytes);
                return Parse(document);
            }
            catch (NativeXmlCatalogueException)
            {
                throw;
            }
            catch (XmlException exception)
            {
                var failure = exception.Message.IndexOf("DTD", StringComparison.OrdinalIgnoreCase) >= 0
                    ? NativeCatalogueFileFailure.UnsafeXml
                    : NativeCatalogueFileFailure.InvalidStructure;
                throw new NativeXmlCatalogueException(failure, exception.Message, exception);
            }
            catch (FormatException exception)
            {
                throw new NativeXmlCatalogueException(
                    NativeCatalogueFileFailure.InvalidValue,
                    exception.Message,
                    exception);
            }
            catch (ArgumentException exception)
            {
                var failure = NativeCatalogueFileFailure.InvalidValue;
                if (exception.Message.IndexOf("does not resolve", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    failure = NativeCatalogueFileFailure.UnresolvedReference;
                }
                else if (exception.Message.IndexOf("unique", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    failure = NativeCatalogueFileFailure.DuplicateIdentity;
                }
                throw new NativeXmlCatalogueException(failure, exception.Message, exception);
            }
        }

        private static XmlReaderSettings Settings()
        {
            return new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                MaxCharactersInDocument = MaximumCatalogueBytes,
                MaxCharactersFromEntities = 0,
                IgnoreComments = false,
                IgnoreProcessingInstructions = false,
                IgnoreWhitespace = false,
                CheckCharacters = true
            };
        }

        private static void Preflight(byte[] bytes)
        {
            var elements = 0;
            var scalarCharacters = new int[MaximumDepth + 2];
            using (var stream = new MemoryStream(bytes, false))
            using (var reader = XmlReader.Create(stream, Settings()))
            {
                while (reader.Read())
                {
                    if (reader.Depth > MaximumDepth)
                    {
                        throw new NativeXmlCatalogueException(
                            NativeCatalogueFileFailure.UnsafeXml,
                            "The XML nesting depth exceeds the profile limit.");
                    }
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        elements++;
                        if (elements > MaximumElements)
                        {
                            throw new NativeXmlCatalogueException(
                                NativeCatalogueFileFailure.UnsafeXml,
                                "The XML element count exceeds the profile limit.");
                        }
                        if (reader.AttributeCount > MaximumAttributes)
                        {
                            throw new NativeXmlCatalogueException(
                                NativeCatalogueFileFailure.UnsafeXml,
                                "An XML element exceeds the profile attribute limit.");
                        }
                        scalarCharacters[reader.Depth] = 0;
                    }
                    else if (reader.NodeType == XmlNodeType.Text ||
                        reader.NodeType == XmlNodeType.CDATA ||
                        reader.NodeType == XmlNodeType.SignificantWhitespace)
                    {
                        checked
                        {
                            scalarCharacters[reader.Depth - 1] += reader.Value.Length;
                        }
                        if (scalarCharacters[reader.Depth - 1] > MaximumScalarCharacters)
                        {
                            throw new NativeXmlCatalogueException(
                                NativeCatalogueFileFailure.UnsafeXml,
                                "An XML scalar exceeds the 1 MiB profile limit.");
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.DocumentType ||
                        reader.NodeType == XmlNodeType.EntityReference)
                    {
                        throw new NativeXmlCatalogueException(
                            NativeCatalogueFileFailure.UnsafeXml,
                            "DTD and entity content is prohibited.");
                    }
                }
            }
        }

        private static XmlDocument Load(byte[] bytes)
        {
            var document = new XmlDocument { XmlResolver = null };
            using (var stream = new MemoryStream(bytes, false))
            using (var reader = XmlReader.Create(stream, Settings()))
            {
                document.Load(reader);
            }
            return document;
        }

        private static NativeCatalogue Parse(XmlDocument document)
        {
            var root = document.DocumentElement;
            if (root == null || root.LocalName != "catalogue" ||
                root.NamespaceURI != NativeXmlCatalogueWriter.NamespaceUri)
            {
                Invalid("The root must be catalogue in the native-v2 namespace.");
            }
            RequireAttributes(root, true, "format", "id");
            var format = Attribute(root, "format");
            if (format != NativeXmlCatalogueWriter.FormatVersion)
            {
                throw new NativeXmlCatalogueException(
                    NativeCatalogueFileFailure.UnsupportedFormat,
                    "Native catalogue format " + format + " is not supported.");
            }

            var cursor = new ElementCursor(root);
            var metadata = ParseMetadata(cursor.Next("metadata"));
            var brands = ParseCollection(cursor.Next("brands"), "brand", 100000, ParseBrand);
            var models = ParseCollection(
                cursor.Next("cassetteModels"), "cassetteModel", 250000, ParseCassetteModel);
            var deckModels = ParseCollection(
                cursor.Next("deckModels"), "deckModel", 100000, ParseDeckModel);
            var deckUnits = ParseCollection(
                cursor.Next("deckUnits"), "deckUnit", 100000, ParseDeckUnit);
            var tapes = ParseCollection(cursor.Next("tapes"), "tape", 1000000, ParseTape);
            cursor.End();
            return new NativeCatalogue(
                EntityId<NativeCatalogue>.Parse(Attribute(root, "id")),
                metadata,
                brands,
                models,
                deckModels,
                deckUnits,
                tapes);
        }

        private static NativeCatalogueMetadata ParseMetadata(XmlElement element)
        {
            RequireAttributes(element, false);
            var cursor = new ElementCursor(element);
            var producer = Required(cursor.Next("producer"));
            var created = Timestamp(cursor.Next("createdUtc"));
            var modified = Timestamp(cursor.Next("modifiedUtc"));
            var provenanceElement = cursor.Optional("provenance");
            var provenance = Optional<NativeCatalogueProvenance>.None();
            if (provenanceElement != null)
            {
                RequireAttributes(provenanceElement, false);
                var provenanceCursor = new ElementCursor(provenanceElement);
                provenance = Optional<NativeCatalogueProvenance>.Some(
                    new NativeCatalogueProvenance(
                        Required(provenanceCursor.Next("sourceFormat")),
                        Required(provenanceCursor.Next("sourceRevision")),
                        Required(provenanceCursor.Next("migrationProfile"))));
                provenanceCursor.End();
            }
            cursor.End();
            return new NativeCatalogueMetadata(producer, created, modified, provenance);
        }

        private static NativeBrand ParseBrand(XmlElement element)
        {
            RequireAttributes(element, false, "id");
            var cursor = new ElementCursor(element);
            var value = new NativeBrand(
                EntityId<NativeBrand>.Parse(Attribute(element, "id")),
                Required(cursor.Next("name")),
                Required(cursor.Next("legacyCode")),
                Timestamp(cursor.Next("addedUtc")),
                Text(cursor.Next("notes")));
            cursor.End();
            return value;
        }

        private static NativeCassetteModel ParseCassetteModel(XmlElement element)
        {
            RequireAttributes(element, false, "id");
            var cursor = new ElementCursor(element);
            var value = new NativeCassetteModel(
                EntityId<NativeCassetteModel>.Parse(Attribute(element, "id")),
                EntityId<NativeBrand>.Parse(Required(cursor.Next("brandId"))),
                Integer(cursor.Next("typeNumber")),
                Required(cursor.Next("modelName")),
                Required(cursor.Next("legacyCode")),
                Required(cursor.Next("legacyIdentifier")),
                Required(cursor.Next("displayName")),
                Timestamp(cursor.Next("addedUtc")),
                Text(cursor.Next("notes")));
            cursor.End();
            return value;
        }

        private static NativeDeckModel ParseDeckModel(XmlElement element)
        {
            RequireAttributes(element, false, "id");
            var cursor = new ElementCursor(element);
            var id = EntityId<NativeDeckModel>.Parse(Attribute(element, "id"));
            var manufacturer = Required(cursor.Next("manufacturer"));
            var model = Required(cursor.Next("model"));
            var year = Integer(cursor.Next("year"));
            var capabilities = ParseCapabilities(cursor.Next("capabilities"));
            cursor.End();
            return new NativeDeckModel(id, manufacturer, model, year, capabilities);
        }

        private static NativeDeckCapabilities ParseCapabilities(XmlElement element)
        {
            RequireAttributes(element, false);
            var cursor = new ElementCursor(element);
            var result = new NativeDeckCapabilities(
                Boolean(cursor.Next("type1")), Boolean(cursor.Next("type2")),
                Boolean(cursor.Next("type3")), Boolean(cursor.Next("type4")),
                Boolean(cursor.Next("hx")), Boolean(cursor.Next("mpx")),
                Boolean(cursor.Next("dolbyB")), Boolean(cursor.Next("dolbyC")),
                Boolean(cursor.Next("dolbyS")), Boolean(cursor.Next("dbx1")),
                Boolean(cursor.Next("dbx2")), Boolean(cursor.Next("stereo")),
                Boolean(cursor.Next("programSearch")), Boolean(cursor.Next("reverse")),
                Boolean(cursor.Next("calibration")), Boolean(cursor.Next("azimuth")),
                Boolean(cursor.Next("dubbingSlow")), Boolean(cursor.Next("dubbingFast")),
                Integer(cursor.Next("frequencyLow")), Integer(cursor.Next("frequencyHigh")),
                Integer(cursor.Next("signalRatio")), Text(cursor.Next("signalRatioNoiseReduction")),
                Decimal(cursor.Next("wowFlutter")), Decimal(cursor.Next("distortion")),
                Integer(cursor.Next("heads")), Integer(cursor.Next("wells")),
                Boolean(cursor.Next("speedSlow")), Boolean(cursor.Next("speedNormal")),
                Boolean(cursor.Next("speedFast")));
            cursor.End();
            return result;
        }

        private static NativeDeckUnit ParseDeckUnit(XmlElement element)
        {
            RequireAttributes(element, false, "id");
            var cursor = new ElementCursor(element);
            var result = new NativeDeckUnit(
                EntityId<NativeDeckUnit>.Parse(Attribute(element, "id")),
                EntityId<NativeDeckModel>.Parse(Required(cursor.Next("deckModelId"))),
                Required(cursor.Next("name")),
                Required(cursor.Next("legacyKey")),
                Integer(cursor.Next("condition")),
                Timestamp(cursor.Next("addedUtc")),
                Text(cursor.Next("notes")));
            cursor.End();
            return result;
        }

        private static NativeTape ParseTape(XmlElement element)
        {
            RequireAttributes(element, false, "id");
            var cursor = new ElementCursor(element);
            var id = EntityId<NativeTape>.Parse(Attribute(element, "id"));
            var modelId = EntityId<NativeCassetteModel>.Parse(Required(cursor.Next("cassetteModelId")));
            var year = Integer(cursor.Next("year"));
            var length = Decimal(cursor.Next("lengthMinutes"));
            var region = Text(cursor.Next("region"));
            var number = Integer(cursor.Next("number"));
            var legacyIdentifier = Required(cursor.Next("legacyIdentifier"));
            var legacyShortIdentifier = Required(cursor.Next("legacyShortIdentifier"));
            var condition = Integer(cursor.Next("condition"));
            var packaged = Boolean(cursor.Next("packaged"));
            var added = Timestamp(cursor.Next("addedUtc"));
            var notes = Text(cursor.Next("notes"));
            var sidesElement = cursor.Next("sides");
            cursor.End();
            RequireAttributes(sidesElement, false);
            var sides = new ElementCursor(sidesElement);
            var sideA = ParseSide(sides.Next("side"), NativeTapeSidePosition.A);
            var sideB = ParseSide(sides.Next("side"), NativeTapeSidePosition.B);
            sides.End();
            return new NativeTape(
                id, modelId, year, length, region, number, legacyIdentifier,
                legacyShortIdentifier, condition, packaged, added, notes, sideA, sideB);
        }

        private static NativeTapeSide ParseSide(
            XmlElement element,
            NativeTapeSidePosition expectedPosition)
        {
            RequireAttributes(element, false, "position");
            if (Attribute(element, "position") != expectedPosition.ToString())
            {
                Invalid("Tape sides must occur exactly once in A, B order.");
            }
            var cursor = new ElementCursor(element);
            var name = Text(cursor.Next("name"));
            var recordingElement = cursor.Optional("recording");
            var recording = Optional<NativeRecording>.None();
            if (recordingElement != null)
            {
                recording = Optional<NativeRecording>.Some(ParseRecording(recordingElement));
            }
            cursor.End();
            return new NativeTapeSide(expectedPosition, name, recording);
        }

        private static NativeRecording ParseRecording(XmlElement element)
        {
            RequireAttributes(element, false, "id");
            var cursor = new ElementCursor(element);
            var deckElement = cursor.Optional("deckUnitId");
            var deckId = Optional<EntityId<NativeDeckUnit>>.None();
            if (deckElement != null)
            {
                deckId = Optional<EntityId<NativeDeckUnit>>.Some(
                    EntityId<NativeDeckUnit>.Parse(Required(deckElement)));
            }
            var result = new NativeRecording(
                EntityId<NativeRecording>.Parse(Attribute(element, "id")),
                deckId,
                Timestamp(cursor.Next("recordedUtc")),
                Text(cursor.Next("inputName")), Integer(cursor.Next("peakLevel")),
                Text(cursor.Next("noiseReduction")), Boolean(cursor.Next("hx")),
                Boolean(cursor.Next("mpx")), Boolean(cursor.Next("dubbed")),
                Text(cursor.Next("speed")), Integer(cursor.Next("bias")),
                Integer(cursor.Next("biasCalibration")), Text(cursor.Next("equalization")),
                Decimal(cursor.Next("level")), Decimal(cursor.Next("levelCalibration")),
                Text(cursor.Next("contents")), Text(cursor.Next("artist")),
                Text(cursor.Next("title")));
            cursor.End();
            return result;
        }

        private static List<T> ParseCollection<T>(
            XmlElement container,
            string childName,
            int maximum,
            Func<XmlElement, T> parse)
        {
            RequireAttributes(container, false);
            var children = ElementChildren(container);
            if (children.Count > maximum)
            {
                throw new NativeXmlCatalogueException(
                    NativeCatalogueFileFailure.UnsafeXml,
                    container.LocalName + " exceeds the profile entity limit.");
            }
            var result = new List<T>(children.Count);
            foreach (var child in children)
            {
                if (child.LocalName != childName)
                {
                    Invalid("Unexpected " + child.LocalName + " inside " + container.LocalName + ".");
                }
                result.Add(parse(child));
            }
            return result;
        }

        private static List<XmlElement> ElementChildren(XmlElement parent)
        {
            var result = new List<XmlElement>();
            foreach (XmlNode node in parent.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Whitespace)
                {
                    continue;
                }
                if (node.NodeType != XmlNodeType.Element)
                {
                    Invalid("Comments, processing instructions, CDATA, and mixed content are prohibited.");
                }
                var element = (XmlElement)node;
                if (element.NamespaceURI != NativeXmlCatalogueWriter.NamespaceUri)
                {
                    Invalid("Every native core element must use the native-v2 namespace.");
                }
                result.Add(element);
            }
            return result;
        }

        private static void RequireAttributes(
            XmlElement element,
            bool allowNamespaceDeclaration,
            params string[] names)
        {
            var expected = new HashSet<string>(names, StringComparer.Ordinal);
            foreach (XmlAttribute attribute in element.Attributes)
            {
                if (allowNamespaceDeclaration &&
                    attribute.NamespaceURI == "http://www.w3.org/2000/xmlns/")
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(attribute.NamespaceURI) || !expected.Remove(attribute.LocalName))
                {
                    Invalid("Unexpected attribute " + attribute.Name + " on " + element.LocalName + ".");
                }
            }
            if (expected.Count != 0)
            {
                Invalid("Missing required attribute " + string.Join(", ", new List<string>(expected).ToArray()) + ".");
            }
        }

        private static string Attribute(XmlElement element, string name)
        {
            return element.GetAttribute(name);
        }

        private static string Text(XmlElement element)
        {
            RequireAttributes(element, false);
            var builder = new StringBuilder();
            foreach (XmlNode node in element.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Text && node.NodeType != XmlNodeType.Whitespace)
                {
                    Invalid("Scalar " + element.LocalName + " cannot contain markup.");
                }
                builder.Append(node.Value);
            }
            if (builder.Length > MaximumScalarCharacters)
            {
                throw new NativeXmlCatalogueException(
                    NativeCatalogueFileFailure.UnsafeXml,
                    "Scalar " + element.LocalName + " exceeds the profile limit.");
            }
            return builder.ToString();
        }

        private static string Required(XmlElement element)
        {
            var value = Text(element);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new FormatException(element.LocalName + " requires non-whitespace text.");
            }
            return value;
        }

        private static int Integer(XmlElement element)
        {
            int value;
            if (!int.TryParse(Text(element), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value))
            {
                throw new FormatException(element.LocalName + " is not a valid integer.");
            }
            return value;
        }

        private static decimal Decimal(XmlElement element)
        {
            decimal value;
            if (!decimal.TryParse(
                Text(element),
                NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out value))
            {
                throw new FormatException(element.LocalName + " is not a valid invariant decimal.");
            }
            return value;
        }

        private static bool Boolean(XmlElement element)
        {
            var text = Text(element);
            if (text == "true") return true;
            if (text == "false") return false;
            throw new FormatException(element.LocalName + " must be true or false.");
        }

        private static UtcTimestamp Timestamp(XmlElement element)
        {
            DateTime value;
            if (!DateTime.TryParseExact(
                Text(element),
                "yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out value))
            {
                throw new FormatException(element.LocalName + " must be a canonical UTC timestamp.");
            }
            return new UtcTimestamp(DateTime.SpecifyKind(value, DateTimeKind.Utc));
        }

        private static void Invalid(string message)
        {
            throw new NativeXmlCatalogueException(
                NativeCatalogueFileFailure.InvalidStructure,
                message);
        }

        private sealed class ElementCursor
        {
            private readonly List<XmlElement> children;
            private int index;

            public ElementCursor(XmlElement parent)
            {
                children = ElementChildren(parent);
            }

            public XmlElement Next(string name)
            {
                if (index >= children.Count || children[index].LocalName != name)
                {
                    Invalid("Expected " + name + " in canonical element order.");
                }
                return children[index++];
            }

            public XmlElement Optional(string name)
            {
                if (index < children.Count && children[index].LocalName == name)
                {
                    return children[index++];
                }
                return null;
            }

            public void End()
            {
                if (index != children.Count)
                {
                    Invalid("Unexpected " + children[index].LocalName + " after the canonical element sequence.");
                }
            }
        }
    }

    public sealed class NativeXmlCatalogueException : Exception
    {
        public NativeXmlCatalogueException(
            NativeCatalogueFileFailure failure,
            string message)
            : base(message)
        {
            Failure = failure;
        }

        public NativeXmlCatalogueException(
            NativeCatalogueFileFailure failure,
            string message,
            Exception innerException)
            : base(message, innerException)
        {
            Failure = failure;
        }

        public NativeCatalogueFileFailure Failure { get; }
    }
}
