using System;
using System.Text;
using Godot;

namespace Savage.Logs;

/// <summary> Output target for Godot's editor console. (using GD.Print) </summary>
/// <remarks> No theming support at this time. Godot's output window does not support BBCode at this time unfortunately. </remarks>
public class GodotEditorLogger : ILogSink {

    // parameters
    public LogSinkSettings Settings { get; }

    // state
    readonly StringBuilder logWriter = new StringBuilder();

    #region Construction
    public GodotEditorLogger(LogSinkSettings settings) {
        Settings = settings;
    }

    #endregion Construction

    public void Write(LogEntry entry) {

        InsertEntryInto(logWriter, entry);
        string message = logWriter.ToString();

        switch (entry.Verbosity) {
            default:
            case Verbosity.Trace:
            case Verbosity.Audit:
            case Verbosity.Debug:
            case Verbosity.Info:
                GD.Print(message);
                break;

            case Verbosity.Warning:
                GD.Print(message);
                GD.PushWarning(message);
                break;

            case Verbosity.Error:
            case Verbosity.Fatal:
                GD.PrintErr(message);
                GD.PushError(message);
                break;
        }

        logWriter.Clear();
    }

    private void InsertEntryInto(StringBuilder writer, LogEntry entry) {

        if (Settings.DisplayVerbosity)
            writer.Append($"{entry.Verbosity}: ");
        
        foreach (var decoration in entry.Decorations.InlinePreceding)
            WriteInlineDecorationNoColor(writer, decoration);

        writer.Append(entry.Message);

        foreach (var decoration in entry.Decorations.InlineTrailing)
            WriteInlineDecorationNoColor(writer, decoration);

        foreach (var decoration in entry.Decorations.FollowingLine)
            WriteFollowingLineNoColor(writer, decoration);
    }

    private void WriteInlineDecorationNoColor(StringBuilder writer, LogDecoration decoration) {
        if (decoration.ShowTag) {
            writer.Append($"[{decoration.Tag}] ");
        }

        writer.Append($"{decoration.Value}: ");
    }

    private void WriteFollowingLineNoColor(StringBuilder writer, LogDecoration decoration) {

        writer.Append($"\n    - {decoration.Tag}: ");

        int indentation = decoration.Tag.Length + 8;

        string[] lines = decoration.Value.Split('\n');
        writer.Append(lines[0]);
        for (int i = 1; i < lines.Length; ++i) {
            for (int x = 0; x < indentation; ++x)
                writer.Append(' ');
            writer.AppendLine(lines[i]);
        }
    }

    public override bool Equals(object obj) => obj is GodotEditorLogger;
}

public static partial class LogPipelineExtensions {

    /// <inheritdoc cref="GodotEditorLogger"/>
    public static LogPipeline SinkGodotEditorIfEditorBuild(this LogPipeline pipeline, LogSinkSettings settings = null) {
        if (OS.HasFeature("editor")) {
            if (settings is null)
                settings = new LogSinkSettings();
            
            pipeline.Add(new GodotEditorLogger(settings));
        }
           

        return pipeline;
    }
}
