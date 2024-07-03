
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Savage.Logs {

    /// <summary> Listens to events from the <see cref=Log"/> class and writes them to an xml file. </summary>
    /// <remarks> You can have as many <see cref="XmlFileLogger"/>s as you like. </remarks>
    public class XmlFileLogger : ILogSink, IDisposable {

        public LogSinkSettings Settings { get; set; }

        string absolutePath;
        private XmlWriter writer;
        private FileStream stream;

        /// <summary> Creates log file and begins writing to it immediately. </summary>
        /// <param name="logDirectory"> Absolute path to the folder that will contain the log. </param>
        /// <param name="logName"> Name of the file that will contain the log, do not include the file type. </param>
        public XmlFileLogger(Uri logDirectory, string logName, LogSinkSettings settings = null) {
            absolutePath = Path.Combine(logDirectory.AbsolutePath, $"{logName}.xml");
            try { stream = new FileStream(absolutePath, FileMode.Create); }
            catch (System.Exception exception) {
                Log.Error(exception.Message);
                Log.Error($"{nameof(XmlFileLogger)} will not be used.");
                return;
            }

            var xmlSettings = new XmlWriterSettings {
                Indent = true,
                ConformanceLevel = ConformanceLevel.Document
            };
            writer = XmlWriter.Create(stream, xmlSettings);
            writer.WriteStartDocument();
            writer.WriteStartElement("Log", @"data:loggy");
            //LogPipeline.MessageLogged += Write;
        }

        ~XmlFileLogger() {
            Dispose();
        }

        void OnProcessExit(object sender, EventArgs arguments) => Dispose();

        private bool alreadyDisposed;
        public void Dispose() {
            if (alreadyDisposed) 
                return;
            alreadyDisposed = true;

            //LogPipeline.MessageLogged -= Write;

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Dispose();
            stream.Dispose();
        }

        // we need to write the closing doc after every time
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(LogEntry entry) {
            entry.WriteXml(writer);
            writer.Flush();
        }

        public override bool Equals(object obj) {
            return obj is XmlFileLogger other && absolutePath == other.absolutePath;
        }
    }

    public static partial class LoggerExtensions {

        public static LogPipeline SinkXmlFile(this LogPipeline pipeline, Uri logDirectory, string logName) {
            pipeline.Add(new XmlFileLogger(logDirectory, logName));
            return pipeline;
        }

        public static LogPipeline SinkXmlFile(this LogPipeline pipeline, Uri logDirectory, string logName, LogSinkSettings settings) {
            pipeline.Add(new XmlFileLogger(logDirectory, logName, settings));
            return pipeline;
        }
    }
}
