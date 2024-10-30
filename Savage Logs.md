
A logging library that lets you change your mind later.

Savage Logs is a structured logging library in the same vein as [Serilog](https://serilog.net/) and [Zlogger](https://github.com/Cysharp/ZLogger) with one major twist. Additional data can be attached to your logs

### Notable Features
- Create logging pipelines that attach metadata to your messages as they pass to one or many different output targets.
- Change your mind about what data you need to debug a system you wrote last year! Need to know what thread or frame count your log messages were created on? You can add that to some or all messages in one line.
- Built for .NET standard 2.0, runs even in the most constrained of dotnet environments.
- Multithreaded and queue based, printing thousands of messages to the system console isn't going to grind your worker thread to a standstill.
- Great for games, separate modules out of the box for integrating seamlessly with Unity and Godot.
- Audit logging verbosity for separating out a log stream for critical transactions like permission changes to servers or recording database transactions.
- Very easy to extend with your own custom metadata attachments.
- Simple theming system for output targets that can display color with multiple built in themes.
- System.Diagnostics Assertions and unhandled exceptions included in your log pipelines by default, easy to configure ignoring them if you choose.

### Warnings
Godot 4.x does not

### Speed


### Still on The Todo List
- Add integration for more C# ecosystem interfaces like Microsoft.Extensions.Logging.
- Restrict allocations inside log pipelines to near zero. Reducing garbage collection pressure in logging heavy applications is critical for games.
- Allow configuration for stripping unused log messages from release builds to reduce executable size.