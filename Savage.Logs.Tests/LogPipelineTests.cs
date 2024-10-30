using System;
using System.Linq;

namespace Savage.Logs.Tests;

public class LogPipelineTests {
    
    (Logs.LogPipeline Pipeline, MemoryLogger Log) CreateLogPipeline() {
        var config = LogPipelineSettings.Default();
        var theme = Theme.DefaultDark();
        var inMemorySink = new MemoryLogger(null);
        var pipeline = new Savage.Logs.LogPipeline(config, theme);
        pipeline.WriteTo(inMemorySink);
        return (pipeline, inMemorySink);
    }

    [Test]
    public async Task LogPipeline_ConstructorThrowsNoErrors() {
        var tuple = CreateLogPipeline();
        await Assert.That(tuple.Pipeline).IsNotNull();
    }
    
    
    
    
}