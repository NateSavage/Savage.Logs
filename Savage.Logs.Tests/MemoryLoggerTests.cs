using Microsoft.Extensions.Logging;

namespace Savage.Logs.Tests;

public class MemoryLoggerTests {
    
    (Logs.LogPipeline Pipeline, MemoryLogger Log) CreateLogPipeline() {
        var config = LogPipelineSettings.Default();
        var theme = Theme.DefaultDark();
        var inMemorySink = new MemoryLogger(null);
        var pipeline = new Savage.Logs.LogPipeline(config, theme);
        pipeline.WriteTo(inMemorySink);
        return (pipeline, inMemorySink);
    }

    [Test]
    public async Task InMemoryLogger_CanWriteMessage() {
        string message = "hello world";
        var tuple = CreateLogPipeline();
        tuple.Pipeline.LogInfo(message);
        await Task.Delay(50);
        await Assert.That(tuple.Log.LastEntry.Message).Contains(message);
    }
}