using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;
using Savage.Logs.Collections;

namespace Savage.Logs {

    /// <summary> A bundle of data to be recorded to each output sink. </summary>
    public struct LogEntry : IXmlSerializable {

        /// <summary> Main text to display for this log. </summary>
        public string Message { get; private set; }

        /// <inheritdoc cref="Verbosity"/>
        public Verbosity Verbosity { get; private set; }

        public DecorationContainer Decorations { get; private set; }


        #region Construction
        public LogEntry(string message, Verbosity verbosity) {
            Message = message;
            Verbosity = verbosity;
            Decorations = new DecorationContainer();
        }

        public LogEntry(string message, Verbosity verbosity, IEnumerable<LogDecoration> decorations) {
            Message = message;
            Verbosity = verbosity;
            Decorations = new DecorationContainer(decorations);
        }


        #endregion Construction


        #region XML Serialization
        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader) {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer) {
            writer.WriteStartElement(GetType().Name);
            string contents;
            string tag;

            foreach (var decoration in Decorations.InlinePreceding) {
                contents = new string(decoration.Value.SkipWhile(CharacterIsIllegal).ToArray());
                tag = decoration.Tag.WithoutWhiteSpace();
                writer.WriteAttributeString(tag, contents);
            }

            contents = new string(Message.SkipWhile(CharacterIsIllegal).ToArray());
            writer.WriteAttributeString(nameof(Message), contents);

            foreach (var decoration in Decorations.InlineTrailing) {
                contents = new string(decoration.Value.SkipWhile(CharacterIsIllegal).ToArray());
                tag = decoration.Tag.WithoutWhiteSpace();
                writer.WriteAttributeString(tag, contents);
            }

            foreach (var decoration in Decorations.FollowingLine) {
                contents = new string(decoration.Value.SkipWhile(CharacterIsIllegal).ToArray());
                tag = decoration.Tag.WithoutWhiteSpace();
                writer.WriteStartElement(tag);
                writer.WriteValue(contents);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        private bool CharacterIsIllegal(char c) {
            return c == 0x20 || c == '<' || c == '>' || c == '+' || c == '\'';
        }
        #endregion
    }
}
