using System;
using System.Diagnostics;
using TUnit.Core.Logging;

namespace Savage.Logs.Tests;

public class NotableEventsCaptured {
    
    (Logs.LogPipeline Pipeline, MemoryLogger Log) CreateLogPipeline() {
        var config = LogPipelineSettings.Default();
        var theme = Theme.DefaultDark();
        var inMemorySink = new MemoryLogger(null);
        var pipeline = new Savage.Logs.LogPipeline(config, theme);
        pipeline.WriteTo(inMemorySink);
        return (pipeline, inMemorySink);
    }
    
    
    [Test]
    public async Task General_FailedAssertionsAreLogged() {
    #if !DEBUG
        TestContext.Current!.GetDefaultLogger().LogWarning($"debug assertion test cannot run outside of debug mode");
        return;
    #endif
        
        var tuple = CreateLogPipeline();
        System.Diagnostics.Debug.Assert(1 < 0, "my assertion failed!");
        await Assert.That(tuple.Log.LastEntry.Message).Contains("my assertion");
    }
    
    /*
    [Test]
    public async Task General_UnhandledExceptionsAreLogged() {
        (Logs.LogPipeline Pipeline, InMemoryLogger Log) pipeline = CreateLogPipeline();
        Assert.Throws(() => throw new Exception("exception thrown! this should be logged"));
        await Assert.That(pipeline.Log.LastEntry.Message).IsNotNull()
                    .And.Contains("this should be logged");
    }
    */
}