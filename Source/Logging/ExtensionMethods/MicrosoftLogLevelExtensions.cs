using Microsoft.Extensions.Logging;

namespace Savage.Logs {

public static class MicrosoftLogLevelExtensions {
    
    internal static Verbosity ToSavageLogsVerbosity(this LogLevel microsoftLogLevel) {
        switch (microsoftLogLevel) {
            case LogLevel.Trace:       return Verbosity.Trace;
            case LogLevel.Debug:       return Verbosity.Debug;
            case LogLevel.Information: return Verbosity.Info;
            case LogLevel.Warning:     return Verbosity.Warning;
            case LogLevel.Error:       return Verbosity.Error;
            case LogLevel.Critical:    return Verbosity.Fatal;
                    
            default: return Verbosity.Debug;
        }
    }
}

}