using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IndentGuide;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests {
    static class AssertUtil {
        public static string ToFormattedString(this IEnumerable<LineSpan> lines) {
            return string.Join(",\r\n", lines.Select(line =>
                string.Format("new LineSpan({0}, {1}, {2}, {3})", line.FirstLine, line.LastLine, line.Indent, line.Type)
            ));
        }

        public static void AssertLinesInclude(this DocumentAnalyzer actual, params LineSpan[] expected) {
            var missingLines = new HashSet<LineSpan>(expected).Except(actual.Lines);
            if (missingLines.Any()) {
                Assert.Fail("Lines not found:\r\n{0}\r\n\r\nActual lines:\r\n{1}",
                    missingLines.ToFormattedString(),
                    actual.Lines.ToFormattedString());
            }
        }

        public static void AssertLinesIncludeExactly(this DocumentAnalyzer actual, params LineSpan[] expected) {
            var missingLines = new HashSet<LineSpan>(expected).Except(actual.Lines);
            var unexpectedLines = new HashSet<LineSpan>(actual.Lines).Except(expected);
            var message = new List<string>();
            if (missingLines.Any()) {
                message.Add("Lines not found:\r\n" + missingLines.ToFormattedString());
            }
            if (unexpectedLines.Any()) {
                message.Add("Unexpected lines:\r\n" + unexpectedLines.ToFormattedString());
            }
            if (message.Any()) {
                message.Add("Actual lines:\r\n" + actual.Lines.ToFormattedString());
                Assert.Fail(string.Join("\r\n\r\n", message));
            }
        }

        public static void ResetAndWait(this DocumentAnalyzer analyzer) {
            var task = analyzer.Reset();
            if (task != null) {
                task.Wait();
            }
        }

        public static void UpdateAndWait(this DocumentAnalyzer analyzer) {
            var task = analyzer.Update();
            if (task != null) {
                task.Wait();
            }
        }
    }
}
