
namespace Savage.Logs.Tests;

public class SystemConsoleLoggerTests {
    
    Logs.LogPipeline CreateLogPipeline() {
        var config = LogPipelineSettings.Default();
        var theme = Theme.DefaultDark();

        return new Savage.Logs.LogPipeline(config, theme)
                              .WriteToSystemConsole();
    }

    [Test]
    public async Task SystemConsoleLogger_WritesToStandardError() {
        string message = "Hello world!";
        var pipeline = CreateLogPipeline();
        pipeline.LogInfo(message);
        
        // the message may not immediately be written to std err
        await Task.Delay(50);
        
        string output = TestContext.Current!.GetErrorOutput();
        await Assert.That(output).Contains(message);
    }
    
    [Test]
    public async Task SystemConsoleLogger_DoesNotWriteToStandardOutput() {
        string message = "Hello world!";
        var pipeline = CreateLogPipeline();
        pipeline.LogInfo(message);
        
        // the message may not immediately be written to std err
        await Task.Delay(50);
        
        string output = TestContext.Current!.GetStandardOutput();
        await Assert.That(output).DoesNotContain(message);
    }
}
