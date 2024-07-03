
using System.Runtime.CompilerServices;

namespace Savage.Logs {

    /// <summary> 
    /// Global state for <see cref="Logs"/>. <br/>
    /// </summary>
    public static class Log {

        /// <summary>
        /// A global log pipeline stored for convenience. <br/>
        /// You do not need to use this if you don't want to, and can create and pass around as many local pipelines as you like. 
        /// </summary>
        /// <remarks> You can send messages to the global pipeline with the other methods on this class.</remarks>
        public static LogPipeline GlobalLogPipeline;


        /// <summary> Logs a message with a verbosity of your choosing. </summary>
        public static void Message(Verbosity verbosity, string message, LogDecoration[] decorations = null, [CallerFilePath] string callerPath = null) => GlobalLogPipeline.BroadcastLog(verbosity, message, callerPath, decorations);

        public static void Trace(string message, LogDecoration[] decorations = null, [CallerFilePath] string callerPath = null) => GlobalLogPipeline.BroadcastLog(Verbosity.Trace, message, callerPath, decorations);
        public static void Debug(string message, LogDecoration[] decorations = null, [CallerFilePath] string callerPath = null) => GlobalLogPipeline.BroadcastLog(Verbosity.Debug, message, callerPath, decorations);
        public static void Info(string message, LogDecoration[] decorations = null, [CallerFilePath] string callerPath = null) => GlobalLogPipeline.BroadcastLog(Verbosity.Info, message, callerPath, decorations);
        public static void Warning(string message, LogDecoration[] decorations = null, [CallerFilePath] string callerPath = null) => GlobalLogPipeline.BroadcastLog(Verbosity.Warning, message, callerPath, decorations);
        public static void Error(string message, LogDecoration[] decorations = null, [CallerFilePath] string callerPath = null) => GlobalLogPipeline.BroadcastLog(Verbosity.Error, message, callerPath, decorations);
        public static void Fatal(string message, LogDecoration[] decorations = null, [CallerFilePath] string callerPath = null) => GlobalLogPipeline.BroadcastLog(Verbosity.Fatal, message, callerPath, decorations);


        #region Expression Tree Test
        /*
        private static Action<string, string, Verbosity, LogDecoration[]> ConstructBroadcastExpression(LoggerSettings settings) {

            var instructions = new List<Expression>(8);
            //Expression expression = null;

            // method parameters
            var messageParameter = Expression.Parameter(typeof(string), "message");
            var callerPathParameter = Expression.Parameter(typeof(string), "callerPath");
            var verbosityParameter = Expression.Parameter(typeof(Verbosity), "verbosity");
            var dataParameter = Expression.Parameter(typeof(LogDecoration[]), "data");


            // check minimum verbosity
            /*
            if (settings.MinimumVerbosity < Verbosity.Trace) { // we can skip verbosity filtering if the minimum allowed logging level allows all logs
                var minimumVerbosityByte = Expression.Constant((byte)settings.MinimumVerbosity);
                var verbosityByte = Expression.Convert(verbosityParameter, typeof(byte));
                instructions.Add(VerbosityFiltering(minimumVerbosityByte, verbosityByte));
            }
            */ /*

            // everything that uses this needs to be farther down the tree
            // create log entry with attached verbosity and user provided data
            var logEntry = Expression.Variable(typeof(LogEntry), "logEntry");
            var logEntryConstructor = typeof(LogEntry).GetConstructor(new[] { typeof(Verbosity), typeof(LogDecoration[]) });
            var logEntryNew = Expression.New(logEntryConstructor, verbosityParameter, dataParameter);
            var logEntryAssign = Expression.Assign(logEntry, logEntryNew);
            instructions.Add(logEntryAssign);


            var LogDecorationConstructor = typeof(LogDecoration).GetConstructor(new[] { typeof(string), typeof(object) });
            Expression<Action<LogEntry, LogDecoration>> addDecorationToLog = (LogEntry logEntry, LogDecoration decoration) => logEntry.Decorations.Add(decoration);
            var logEntryDecorationsProperty = Expression.Property(logEntry, nameof(LogEntry.Decorations));

            // if we're including the path that should be bundled into the log
            var callerName = Expression.Variable(typeof(string), "caller");
            var callerDecoration = Expression.Variable(typeof(LogDecoration), "callerDecoration");
            if (settings.IncludeCaller) {

                var extractCallerNameMethod = typeof(Path).GetMethod(nameof(Path.GetFileNameWithoutExtension), new[] { typeof(string) });
                var getFileNameWithoutExtension = Expression.Call(extractCallerNameMethod, callerPathParameter);
                var assignFileNameToCallerVariable = Expression.Assign(callerName, getFileNameWithoutExtension);

                instructions.Add(assignFileNameToCallerVariable);


                var callerString = Expression.Constant("Caller");
                var callerDecorationNew = Expression.New(LogDecorationConstructor, new Expression[] { callerString, callerName });
                var callerDecorationAssign = Expression.Assign(callerDecoration, callerDecorationNew);

                instructions.Add(callerDecorationAssign);

                instructions.Add(Expression.Call(logEntryDecorationsProperty, "Add", null, callerDecoration));
            }

            // attach message to log
            var messageDecorationVariable = Expression.Variable(typeof(LogDecoration), "messageDecoration");
            var messageString = Expression.Constant("Message");
            var messageDecorationNew = Expression.New(LogDecorationConstructor, new Expression[] { messageString, messageParameter });
            var messageDecorationAssign = Expression.Assign(messageDecorationVariable, messageDecorationNew);
            instructions.Add(messageDecorationAssign);



            instructions.Add(Expression.Call(logEntryDecorationsProperty, "Add", null, messageDecorationVariable));


            // check if we should include write time
            /*
            if (settings.IncludeWriteTime) {
                var dateTimeNowProperty = typeof(DateTime).GetProperty(nameof(DateTime.Now), BindingFlags.Public | BindingFlags.Static);
                var currentTime = Expression.Property(null, dateTimeNowProperty);

                var writeTimeDecorationDeclaration = Expression.Variable(typeof(LogDecoration), "writeTimeDecoration");
                var writeTimeString = Expression.Constant("WriteTime");
                var writeTimeDecorationNew = Expression.New(LogDecorationConstructor, new Expression[] { writeTimeString, Expression.Convert(currentTime, typeof(object)) });
                var writeTimeDecorationAssign = Expression.Assign(writeTimeDecorationDeclaration, writeTimeDecorationNew);
                instructions.Add(writeTimeDecorationAssign);

                instructions.Add(Expression.Call(logEntryDecorationsProperty, "Add", null, writeTimeDecorationDeclaration));
            }
            */ /*

            // broadcast our log
            var broadcastLog = typeof(Log).GetMethod(nameof(BroadCastLog), BindingFlags.Static | BindingFlags.NonPublic);
            instructions.Add(Expression.Call(broadcastLog, logEntry));

            var method = Expression.Block(
                variables: new[] { messageParameter, callerPathParameter, verbosityParameter, dataParameter, logEntry, callerName, callerDecoration, messageDecorationVariable },
                instructions
                );

            //var broadcastAction = Expression.Lambda<Action<string, string, Verbosity, LogDecoration[]>>(expression, messageParameter, callerPathParameter, verbosityParameter, dataParameter);
            var broadcastAction = Expression.Lambda<Action<string, string, Verbosity, LogDecoration[]>>(method, messageParameter, callerPathParameter, verbosityParameter, dataParameter);
            return broadcastAction.Compile();
        }
        private static ConditionalExpression VerbosityFiltering(Expression minimumVerbosity, Expression verbosityToCheck) {
            GotoExpression returnVoid = Expression.Return(Expression.Label());
            var versionComparison = Expression.GreaterThan(verbosityToCheck, minimumVerbosity);
            return Expression.Condition(versionComparison, returnVoid, Expression.Empty());
        }

        private static void BroadCastLog(LogEntry entry) => MessageLogged?.Invoke(entry);
         */
        #endregion Expression Tree Test


        #region Benchmark Methods


        /// <summary> Used to AB text the performance of the <see cref="CallerFilePathAttribute"/>. </summary>
        internal static void MessageNoAttribute(Verbosity verbosity, string message, LogDecoration[] decorations = null) => GlobalLogPipeline.BroadcastLog(verbosity, message, null);
        #endregion Benchmark Methods
    }
}