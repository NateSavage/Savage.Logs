using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Savage.Logs.Benchmarks {
    public class CallerAttribute {

        // the differences I'm seeing between these two methods might as well be immeasurable
        // | Method                 | Mean     | Error     | StdDev    |
        // |----------------------- |---------:|----------:|----------:|
        // | LogWithCallerAttribute | 1.052 ns | 0.0388 ns | 0.0531 ns |
        // | LogNoCallerAttribute   | 1.004 ns | 0.0386 ns | 0.0429 ns |

        [Benchmark]
        public void LogWithCallerAttribute() {
            Log.Message(Verbosity.Info, "Hello cruel world!");
        }

        [Benchmark]
        public void LogNoCallerAttribute() {
            Log.MessageNoAttribute(Verbosity.Info, "Hello cruel world!");
        }
    }
}
