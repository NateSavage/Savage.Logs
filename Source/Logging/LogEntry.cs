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

        public AttachmentContainer Attachments { get; private set; }


        #region Construction
        public LogEntry(string message, Verbosity verbosity) {
            Message = message;
            Verbosity = verbosity;
            Attachments = new AttachmentContainer();
        }

        public LogEntry(string message, Verbosity verbosity, IEnumerable<MessageAttachment> decorations) {
            Message = message;
            Verbosity = verbosity;
            if (decorations is null)
                Attachments = new AttachmentContainer();
            else
                Attachments = new AttachmentContainer(decorations);
        }


        #endregion Construction

        public override string ToString() {
            var entry = new StringBuilder();

            foreach (var attachment in Attachments.InlinePreceding) {
                if (attachment.ShowTag) {
                    entry.Append(attachment.Tag);
                    entry.Append(": ");
                }

                entry.Append(attachment.Value);
            }

            entry.Append(Message);

            foreach (var attachment in Attachments.InlineTrailing) {
                entry.Append(' ');
                if (attachment.ShowTag) {
                    entry.Append(attachment.Tag);
                    entry.Append(": ");
                }
                
                entry.Append(attachment.Value);
            }

            foreach (var attachment in Attachments.FollowingLine) {
                entry.Append('\n');
                if (attachment.ShowTag) {
                    entry.Append(attachment.Tag);
                    entry.Append(": ");
                }
                
                entry.Append(attachment.Value);
            }
            
            return entry.ToString();
        }


        #region XML Serialization
        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader) {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer) {
            writer.WriteStartElement(GetType().Name);
            string contents;
            string tag;

            foreach (var attachment in Attachments.InlinePreceding) {
                contents = new string(attachment.Value.SkipWhile(CharacterIsIllegalForXml).ToArray());
                tag = attachment.Tag.WithoutWhiteSpace();
                writer.WriteAttributeString(tag, contents);
            }

            contents = new string(Message.SkipWhile(CharacterIsIllegalForXml).ToArray());
            writer.WriteAttributeString(nameof(Message), contents);

            foreach (var attachment in Attachments.InlineTrailing) {
                contents = new string(attachment.Value.SkipWhile(CharacterIsIllegalForXml).ToArray());
                tag = attachment.Tag.WithoutWhiteSpace();
                writer.WriteAttributeString(tag, contents);
            }

            foreach (var attachment in Attachments.FollowingLine) {
                contents = new string(attachment.Value.SkipWhile(CharacterIsIllegalForXml).ToArray());
                tag = attachment.Tag.WithoutWhiteSpace();
                writer.WriteStartElement(tag);
                writer.WriteValue(contents);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        bool CharacterIsIllegalForXml(char c) {
            return c == 0x20 || c == '<' || c == '>' || c == '+' || c == '\'';
        }
        #endregion
    }
}
