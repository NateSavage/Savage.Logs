using System;

namespace Savage.Logs.Tests;

public class BasicUsageTests {

    LogPipeline CreateLogPipeline() {
        var config = LogPipelineSettings.Default();
        var theme = Theme.DefaultDark();
        return new Savage.Logs.LogPipeline(config, theme).WriteToMemory();
    }

    [Test]
    public async Task BasicUsage_GlobalLogPipelineIsFirstConstructedLogPipeline() {
        var firstPipeline = CreateLogPipeline();
        var secondPipeline = CreateLogPipeline();

        await Assert.That(Log.GlobalLogPipeline).IsNotNull();
        await Assert.That(ReferenceEquals(firstPipeline, Log.GlobalLogPipeline)).IsTrue();
        await Assert.That(ReferenceEquals(secondPipeline, Log.GlobalLogPipeline)).IsFalse();
    }
}