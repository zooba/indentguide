using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IndentGuide;
using IndentGuide.Utils;
using TestUtilities.Mocks;

namespace PerformanceTests {
    class Program {
        static void Main(string[] args) {
            new Program(args.Where(a => File.Exists(a)).ToList()).Run();
        }

        private readonly List<string> _testFiles;
        private List<List<PerformanceLogger.EventTime>> _events;

        private Program(List<string> files) {
            _testFiles = files;
            _events = new List<List<PerformanceLogger.EventTime>>();
        }

        private void PrintEventSummary(int repeatCount) {
            List<List<PerformanceLogger.EventTime>> events;
            lock (_events) {
                events = _events;
                _events = new List<List<PerformanceLogger.EventTime>>();
                events.Add(PerformanceLogger.Take().ToList());
            }

            Console.WriteLine("{0,-20}{1,8}{2,8}{3,8}{4,8}",
                "Event", "25%", "50%", "75%", "Count"
            );

            var categories = events.SelectMany(i => i).GroupBy(e => e.Name).ToList();
            foreach (var category in categories.OrderBy(g => g.Key)) {
                var times = category.Select(e => e.Duration).ToList();
                times.Sort();

                Console.WriteLine("{0,-20}{1,8}{2,8}{3,8}{4,8}",
                    category.Key,
                    times[times.Count / 4],
                    times[times.Count / 2],
                    times[times.Count / 4 * 3],
                    times.Count / repeatCount
                );
            }
        }

        private void Run() {
            PerformanceLogger.DumpEvents += PerformanceLogger_DumpEvents;
            try {
                foreach (var file in _testFiles) {
                    var buffer = new MockTextBuffer(File.ReadAllText(file));
                    var snapshot = buffer.CurrentSnapshot;

                    var behaviour = new LineBehavior {
                        VisibleEmpty = true,
                        VisibleEmptyAtEnd = true,
                        VisibleAligned = true,
                        VisibleAtTextEnd = false,
                        ExtendInwardsOnly = true,
                        VisibleUnaligned = true
                    };

                    foreach (var chunkSize in new[] { 5, 10, 30, 50, 100, 150, 200 }) {
                        var da = new DocumentAnalyzer(snapshot, behaviour, 4, 4, chunkSize);
                        var sw = Stopwatch.StartNew();
                        for (int repeats = 1000; repeats > 0; --repeats) {
                            da.Reset().GetAwaiter().GetResult();
                        }

                        for (int line = 0; line < da.Snapshot.LineCount; line += 30) {
                            var lines = da.GetLines(line, line + 35).ToList();
                        }

                        sw.Stop();

                        Console.WriteLine("ChunkSize = {0}", chunkSize);
                        Console.WriteLine("Duration = {0}", sw.ElapsedTicks / 1000);
                        PrintEventSummary(1000);
                        Console.WriteLine();
                    }
                }
            } finally {
                PerformanceLogger.DumpEvents -= PerformanceLogger_DumpEvents;
            }
        }

        void PerformanceLogger_DumpEvents(object sender, EventArgs e) {
            var events = PerformanceLogger.Take().ToList();
            lock (_events) {
                _events.Add(events);
            }
        }
    }
}
