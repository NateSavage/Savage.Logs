using System.Diagnostics;

namespace Savage.Logs.ThemeTester;

class Program {
    static void Main(string[] args) {

        var logPipeline = LogPipelineExtensions.AttachCallerFileName(LogPipeline.Create())
                                               .AttachWriteTime()
                                               .AttachThreadIdWhenNotOnMainThread()
                                               .WriteToSystemConsole(theme: Theme.DefaultConsole());

        // these are for programmers
        Log.Trace("Trace");
        Log.Info("Info");
        Log.Warning("Warning");
        Log.Error("Error");
        Log.Fatal("Fatal");
        
        // these are for IT and sysadmins
        Log.Debug("Debug");
        Log.Audit("Audit");

        Task.Run(() => {
            Task.Delay(1000).Wait();
            Log.Info("I'm on a different thread!");
        });

        //Trace.Assert(false, "failed trace assertion");
        //Debug.Assert(false, "failed debug assertion");
        
        try { throw new Exception("caught exception"); }
        catch (Exception e) {}

        Console.ReadLine();
    }
    
    
}