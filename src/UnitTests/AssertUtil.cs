using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IndentGuide;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text.Editor;

namespace UnitTests {
    static class AssertUtil {
        public static string ToFormattedString(this IEnumerable<LineSpan> lines) {
            return string.Join(",\r\n", lines
                .OrderBy(line => line.FirstLine)
                .ThenBy(line => line.Indent)
                .Select(line =>
                string.Format("new LineSpan({0}, {1}, {2}, LineSpanType.{3})", line.FirstLine, line.LastLine, line.Indent, line.Type)
            ));
        }

        public static void AssertLinesInclude(this DocumentAnalyzer actual, params LineSpan[] expected) {
            var missingLines = new HashSet<LineSpan>(expected).Except(actual.GetAllLines());
            if (missingLines.Any()) {
                Assert.Fail("Lines not found:\r\n{0}\r\n\r\nActual lines:\r\n{1}",
                    missingLines.ToFormattedString(),
                    actual.GetAllLines().ToFormattedString());
            }
        }

        public static void AssertLinesIncludeExactly(this DocumentAnalyzer actual, params LineSpan[] expected) {
            var missingLines = new HashSet<LineSpan>(expected).Except(actual.GetAllLines());
            var unexpectedLines = new HashSet<LineSpan>(actual.GetAllLines()).Except(expected);
            var message = new List<string>();
            if (missingLines.Any()) {
                message.Add("Lines not found:\r\n" + missingLines.ToFormattedString());
            }
            if (unexpectedLines.Any()) {
                message.Add("Unexpected lines:\r\n" + unexpectedLines.ToFormattedString());
            }
            if (message.Any()) {
                message.Add("Actual lines:\r\n" + actual.GetAllLines().Distinct().ToFormattedString());
                Assert.Fail(string.Join("\r\n\r\n", message));
            }
        }

        public static void ResetAndWait(this DocumentAnalyzer analyzer) {
            analyzer.ResetAsync().GetAwaiter().GetResult();
        }

        public static void UpdateAndWait(this DocumentAnalyzer analyzer, TextViewLayoutChangedEventArgs changes) {
            analyzer.UpdateAsync(changes).GetAwaiter().GetResult();
        }
    }
}
