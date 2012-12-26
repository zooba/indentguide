using System;
using IndentGuide;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text;
using TestUtilities.Mocks;

namespace UnitTests {
    [TestClass]
    public class DocumentAnalyzerTests {
        static private DocumentAnalyzer MakeAnalyzer(ITextSnapshot text, LineBehavior behavior, int indentSize, int tabSize) {
            return new DocumentAnalyzer(text, behavior, indentSize, tabSize);
        }

        static private DocumentAnalyzer MakeAnalyzer(string text, LineBehavior behavior, int indentSize, int tabSize, string contentType) {
            var buffer = new MockTextBuffer(text);
            buffer.ContentType = new MockContentType(contentType, null);
            return MakeAnalyzer(buffer.CurrentSnapshot, behavior, indentSize, tabSize);
        }

        static private DocumentAnalyzer MakeAnalyzer(string text, 
            bool ExtendInwardsOnly = true, bool VisibleAligned = true, bool VisibleUnaligned = false,
            bool VisibleAtTextEnd = false, bool VisibleEmpty = true, bool VisibleEmptyAtEnd = true,
            int indentSize = 4, int tabSize = 4,
            string contentType = "plaintext") {

            return MakeAnalyzer(text, new LineBehavior {
                ExtendInwardsOnly = ExtendInwardsOnly,
                VisibleAligned = VisibleAligned,
                VisibleUnaligned = VisibleUnaligned,
                VisibleAtTextEnd = VisibleAtTextEnd,
                VisibleEmpty = VisibleEmpty,
                VisibleEmptyAtEnd = VisibleEmptyAtEnd
            }, indentSize, tabSize, contentType);
        }
        
        [TestMethod]
        public void BasicTests() {
            var da = MakeAnalyzer(@"
    1
        2
            3

            5

        7

        9
    10
", VisibleAligned: true, VisibleEmpty: true, ExtendInwardsOnly: true, VisibleEmptyAtEnd: true);

            da.AssertLinesIncludeExactly(
                new LineSpan(2, 9, 4, LineSpanType.Normal),
                new LineSpan(3, 6, 8, LineSpanType.Normal),
                new LineSpan(4, 4, 12, LineSpanType.Normal),
                new LineSpan(8, 8, 8, LineSpanType.Normal)
            );

            da.Behavior.ExtendInwardsOnly = false;
            da.ResetAndWait();

            da.AssertLinesIncludeExactly(
                new LineSpan(0, 0, 4, LineSpanType.Normal),
                new LineSpan(2, 9, 4, LineSpanType.Normal),
                new LineSpan(3, 6, 8, LineSpanType.Normal),
                new LineSpan(4, 4, 12, LineSpanType.Normal),
                new LineSpan(6, 6, 12, LineSpanType.Normal),
                new LineSpan(8, 8, 8, LineSpanType.Normal)
            );

            da.Behavior.VisibleEmpty = false;
            da.ResetAndWait();

            da.AssertLinesIncludeExactly(
                new LineSpan(2, 3, 4, LineSpanType.Normal),
                new LineSpan(5, 5, 4, LineSpanType.Normal),
                new LineSpan(7, 7, 4, LineSpanType.Normal),
                new LineSpan(9, 9, 4, LineSpanType.Normal),
                new LineSpan(3, 3, 8, LineSpanType.Normal),
                new LineSpan(5, 5, 8, LineSpanType.Normal)
            );

        }

        [TestMethod]
        public void CSharpCppPragmaTests() {
            var da = MakeAnalyzer(@"
1
    2
        3
#pragma on line 4

#pragma on line 6
        7
    8
9", contentType: "csharp");

            da.AssertLinesIncludeExactly(
                new LineSpan(3, 3, 4, LineSpanType.Normal),
                new LineSpan(5, 5, 4, LineSpanType.Normal),
                new LineSpan(5, 5, 8, LineSpanType.Normal),
                new LineSpan(7, 7, 4, LineSpanType.Normal)
            );

            ((MockTextSnapshot)da.Snapshot).ContentType = new MockContentType("c/c++", null);
            da.ResetAndWait();

            da.AssertLinesIncludeExactly(
                new LineSpan(3, 3, 4, LineSpanType.Normal),
                new LineSpan(5, 5, 4, LineSpanType.Normal),
                new LineSpan(5, 5, 8, LineSpanType.Normal),
                new LineSpan(7, 7, 4, LineSpanType.Normal)
            );

            ((MockTextSnapshot)da.Snapshot).ContentType = new MockContentType("not treating pragmas specially", null);
            da.ResetAndWait();

            da.AssertLinesIncludeExactly(
                new LineSpan(3, 3, 4, LineSpanType.Normal),
                new LineSpan(7, 7, 4, LineSpanType.Normal)
            );
        }
    }
}
