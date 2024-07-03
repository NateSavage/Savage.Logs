using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZLogger;

namespace Savage.Logs.Benchmarks {

    [MemoryDiagnoser] 
    /// <summary> Latency is a measurement of how quickly we get control returned to us after requesting something be logged. </summary>
    public class ConsoleSinkLatency {

        [Benchmark]
        public void Serilog10CharactersToConsole() {
            Serilog.Log.Logger.Information(RandomAsciiString(characterCount: 10));
        }

        [Benchmark]
        public void Serilog100CharactersToConsole() {
            Serilog.Log.Logger.Information(RandomAsciiString(characterCount: 100));
        }

        [Benchmark]
        public void Serilog1000CharactersToConsole() {
            Serilog.Log.Logger.Information(RandomAsciiString(characterCount: 1000));
        }



        [Benchmark]
        public void Loggy10CharactersToConsole() {
            Log.Info(RandomAsciiString(characterCount: 10));
        }

        [Benchmark]
        public void Loggy100CharactersToConsole() {
            Log.Info(RandomAsciiString(characterCount: 100));
        }

        [Benchmark]
        public void Loggy1000CharactersToConsole() {
            Log.Info(RandomAsciiString(characterCount: 1000));
        }


        [Benchmark]
        public void ZLogger10CharactersToConsole() {
            Program.ZLogger.LogInformation(RandomAsciiString(characterCount: 10));
        }

       [Benchmark]
        public void ZLogger100CharactersToConsole() {
            Program.ZLogger.LogInformation(RandomAsciiString(characterCount: 100));
        }

        [Benchmark]
        public void ZLogger1000CharactersToConsole() {
            Program.ZLogger.LogInformation(RandomAsciiString(characterCount: 1000));
        }



        [Benchmark]
        public void SystemConsole10CharactersToConsole() {
            System.Console.WriteLine(RandomAsciiString(characterCount: 10));
        }

        [Benchmark]
        public void SystemConsole100CharactersToConsole() {
            System.Console.WriteLine(RandomAsciiString(characterCount: 100));
        }

        [Benchmark]
        public void SystemConsole1000CharactersToConsole() {
            System.Console.WriteLine(RandomAsciiString(characterCount: 1000));
        }

        string RandomAsciiString(uint characterCount) {
            char[] characters = new char[characterCount];
            for (int i = 0; i < characterCount; ++i)
                characters[i] = (char)Random.Shared.Next('a', 'z');

            return new string(characters);
        }
    }
}
