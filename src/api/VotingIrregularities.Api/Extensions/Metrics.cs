using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Serilog.Context;

namespace VotingIrregularities.Api.Extensions
{
    public class Metrics
    {
        public static IDisposable Time(string context)
        {
            return new Times(context);
        }

        private class Times : IDisposable
        {
            private readonly string _context;
            public Stopwatch Stopwatch { get; }

            public Times (string context)
            {
                _context = context;
                Stopwatch = Stopwatch.StartNew();
            }
            public void Dispose()
            {
                Stopwatch.Stop();
                LogContext.PushProperty($"{_context}-duration:", $"{Stopwatch.ElapsedMilliseconds} ms.");
            }
        }
    }
}
