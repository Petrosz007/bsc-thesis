using System;
using BenchmarkDotNet.Running;

namespace IWA_Backend.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<IWABenchmarks>();
        }
    }
}