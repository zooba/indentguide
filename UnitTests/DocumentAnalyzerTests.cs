/* ****************************************************************************
 * Copyright 2013 Steve Dower
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy 
 * of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 * ***************************************************************************/

using System;
using System.Linq;
using IndentGuide;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text;
using TestUtilities.Mocks;

namespace UnitTests {
    [TestClass]
    public class DocumentAnalyzerTests {
        internal static DocumentAnalyzer MakeAnalyzer(
            ITextSnapshot text,
            LineBehavior behavior,
            int indentSize,
            int tabSize,
            int chunkSize
        ) {
            return new DocumentAnalyzer(text, behavior, indentSize, tabSize, chunkSize);
        }

        internal static DocumentAnalyzer MakeAnalyzer(
            string text,
            LineBehavior behavior,
            int indentSize,
            int tabSize,
            string contentType,
            int chunkSize
        ) {
            var buffer = new MockTextBuffer(text);
            buffer.ContentType = new MockContentType(contentType, null);
            return MakeAnalyzer(buffer.CurrentSnapshot, behavior, indentSize, tabSize, chunkSize);
        }

        internal static DocumentAnalyzer MakeAnalyzer(
            string text, 
            bool ExtendInwardsOnly = true,
            bool VisibleAligned = true,
            bool VisibleUnaligned = false,
            bool VisibleAtTextEnd = false,
            bool VisibleEmpty = true,
            bool VisibleEmptyAtEnd = true,
            int indentSize = 4,
            int tabSize = 4,
            string contentType = "plaintext",
            int chunkSize = 5
        ) {

            return MakeAnalyzer(text.Replace("\\t", "\t"), new LineBehavior {
                ExtendInwardsOnly = ExtendInwardsOnly,
                VisibleAligned = VisibleAligned,
                VisibleUnaligned = VisibleUnaligned,
                VisibleAtTextEnd = VisibleAtTextEnd,
                VisibleEmpty = VisibleEmpty,
                VisibleEmptyAtEnd = VisibleEmptyAtEnd
            }, indentSize, tabSize, contentType, chunkSize);
        }

        private void BasicTest(string text, int tabSize = 4) {
            var da = MakeAnalyzer(
                text,
                VisibleAligned: true,
                VisibleEmpty: true,
                ExtendInwardsOnly: true,
                VisibleEmptyAtEnd: true,
                tabSize: tabSize
            );

            da.ResetAndWait();

            da.AssertLinesIncludeExactly(
                new LineSpan(1, 10, 0, LineSpanType.Normal),
                new LineSpan(2, 9, 4, LineSpanType.Normal),
                new LineSpan(3, 6, 8, LineSpanType.Normal),
                new LineSpan(4, 4, 12, LineSpanType.Normal),
                new LineSpan(8, 8, 8, LineSpanType.Normal)
            );

            da.Behavior.ExtendInwardsOnly = false;
            da.ResetAndWait();

            da.AssertLinesIncludeExactly(
                new LineSpan(0, 10, 0, LineSpanType.Normal),
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
                new LineSpan(1, 3, 0, LineSpanType.Normal),
                new LineSpan(2, 3, 4, LineSpanType.Normal),
                new LineSpan(3, 3, 8, LineSpanType.Normal),
                new LineSpan(5, 5, 0, LineSpanType.Normal),
                new LineSpan(5, 5, 4, LineSpanType.Normal),
                new LineSpan(5, 5, 8, LineSpanType.Normal),
                new LineSpan(7, 7, 0, LineSpanType.Normal),
                new LineSpan(7, 7, 4, LineSpanType.Normal),
                new LineSpan(9, 10, 0, LineSpanType.Normal),
                new LineSpan(9, 9, 4, LineSpanType.Normal)
            );
        }

        [TestMethod]
        public void Basic_SpacesOnly() {
            BasicTest(@"
    1
        2
            3

            5

        7

        9
    10
");
        }

        [TestMethod]
        public void Basic_TabsOnly() {
            BasicTest(@"
\t1
\t\t2
\t\t\t3

\t\t\t5

\t\t7

\t\t9
\t10
");
        }

        [TestMethod]
        public void Basic_MixedSpacesTabs() {
            BasicTest(@"
\t  1
  \t\t  2
\t\t\t  \t  3

 \t       \t  5

  \t\t  7
  \t\t\t    \t\t\t
    \t \t9
\t\t10
", 2);
        }

        private void DeepIndentTest(string text, int tabSize = 4) {
            var da = MakeAnalyzer(
                text,
                VisibleAligned: true,
                VisibleEmpty: true,
                ExtendInwardsOnly: true,
                VisibleEmptyAtEnd: false,
                tabSize: tabSize
            );

            da.ResetAndWait();

            da.AssertLinesIncludeExactly(
                Enumerable.Range(0, 20).Select(i => new LineSpan(1, 1, i * 4, LineSpanType.Normal)).ToArray()
            );

            da.Behavior.ExtendInwardsOnly = false;
            da.ResetAndWait();

            da.AssertLinesIncludeExactly(
                Enumerable.Range(0, 20).Select(i => new LineSpan(0, 1, i * 4, LineSpanType.Normal)).ToArray()
            );

            da.Behavior.VisibleEmpty = false;
            da.ResetAndWait();

            da.AssertLinesIncludeExactly(
                Enumerable.Range(0, 20).Select(i => new LineSpan(1, 1, i * 4, LineSpanType.Normal)).ToArray()
            );
        }

        [TestMethod]
        public void DeepIndent_SpacesOnly() {
            DeepIndentTest(string.Format(@"
{0}text
", new string(' ', 20 * 4)));
        }

        [TestMethod]
        public void DeepIndent_TabsOnly() {
            DeepIndentTest(string.Format(@"
{0}text
", new string('\t', 20)));
        }

        [TestMethod]
        public void DeepIndent_MixedSpacesTabs() {
            DeepIndentTest(string.Format(@"
{0}text
", string.Join("", Enumerable.Repeat("    \t", 10))));
        }

        [TestMethod]
        public void MismatchedTabIndentSize() {
            var text = @"0
\t1

\t\t3

\t\t\t\t5

\t\t7

\t9

11";

            var da = MakeAnalyzer(text, indentSize: 3, tabSize: 4);
            da.ResetAndWait();
            da.AssertLinesIncludeExactly(
                new LineSpan(1, 10, 0, LineSpanType.Normal),
                new LineSpan(1, 9, 3, LineSpanType.Normal),
                new LineSpan(3, 7, 6, LineSpanType.Normal),
                new LineSpan(5, 5, 9, LineSpanType.Normal),
                new LineSpan(5, 5, 12, LineSpanType.Normal),
                new LineSpan(5, 5, 15, LineSpanType.Normal)
            );

            da = MakeAnalyzer(text, indentSize: 3, tabSize: 2);
            da.ResetAndWait();
            da.AssertLinesIncludeExactly(
                new LineSpan(1, 10, 0, LineSpanType.Normal),
                new LineSpan(3, 7, 3, LineSpanType.Normal),
                new LineSpan(5, 5, 6, LineSpanType.Normal)
            );
            
            da = MakeAnalyzer(text, indentSize: 6, tabSize: 4);
            da.ResetAndWait();
            da.AssertLinesIncludeExactly(
                new LineSpan(1, 10, 0, LineSpanType.Normal),
                new LineSpan(3, 7, 6, LineSpanType.Normal),
                new LineSpan(5, 5, 12, LineSpanType.Normal)
            );
        }

        [TestMethod]
        public void LineLength() {
            var text = " a b c d";
            Assert.AreEqual(8, text.ActualLength(4));
            Assert.AreEqual(8, text.ActualLength(8));

            text = "\ta\tb\tc\td";
            Assert.AreEqual(8, text.ActualLength(1));
            Assert.AreEqual(9, text.ActualLength(2));
            Assert.AreEqual(17, text.ActualLength(4));

            text = "   \t123\t456\t789";
            Assert.AreEqual(15, text.ActualLength(1));
            Assert.AreEqual(15, text.ActualLength(4));
            Assert.AreEqual(27, text.ActualLength(8));
        }

        [TestMethod]
        public void UpdateSnapshot() {
            var buffer = new MockTextBuffer("");

            var behavior = new LineBehavior();
            var da = MakeAnalyzer(buffer.CurrentSnapshot, behavior, 4, 4, 5);
            da.ResetAndWait();

            da.AssertLinesIncludeExactly();

            var prevSnapshot = da.Snapshot;

            {
                var edit = buffer.CreateEdit();
                edit.Insert(0, @"0
    1
        2
    3
4");
                edit.Apply();
            }

            da.UpdateAndWait(null);
            Assert.AreNotSame(prevSnapshot, da.Snapshot);
            Assert.AreNotEqual(prevSnapshot.Version.VersionNumber, da.Snapshot.Version.VersionNumber);
            da.AssertLinesIncludeExactly(
                new LineSpan(1, 3, 0, LineSpanType.Normal),
                new LineSpan(2, 2, 4, LineSpanType.Normal)
            );

            prevSnapshot = da.Snapshot;
            {
                var edit = buffer.CreateEdit();
                edit.Delete(prevSnapshot.GetLineFromLineNumber(2).ExtentIncludingLineBreak.Span);
                edit.Apply();
            }

            da.UpdateAndWait(null);
            Assert.AreNotSame(prevSnapshot, da.Snapshot);
            Assert.AreNotEqual(prevSnapshot.Version.VersionNumber, da.Snapshot.Version.VersionNumber);
            da.AssertLinesIncludeExactly(
                new LineSpan(1, 2, 0, LineSpanType.Normal)
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
            
            da.ResetAndWait();

            da.AssertLinesIncludeExactly(
                new LineSpan(2, 3, 0, LineSpanType.Normal),
                new LineSpan(3, 3, 4, LineSpanType.Normal),
                new LineSpan(5, 5, 0, LineSpanType.Normal),
                new LineSpan(5, 5, 4, LineSpanType.Normal),
                new LineSpan(5, 5, 8, LineSpanType.Normal),
                new LineSpan(7, 8, 0, LineSpanType.Normal),
                new LineSpan(7, 7, 4, LineSpanType.Normal)
            );

            Assert.AreEqual(2, da.GetAllLines().First(ls => ls.FirstLine == 3).LinkedLines.Count());
            Assert.AreEqual(2, da.GetAllLines().First(ls => ls.FirstLine == 5 && ls.Indent == 4).LinkedLines.Count());
            Assert.AreEqual(0, da.GetAllLines().First(ls => ls.FirstLine == 5 && ls.Indent == 8).LinkedLines.Count());
            Assert.AreEqual(2, da.GetAllLines().First(ls => ls.FirstLine == 7).LinkedLines.Count());

            ((MockTextSnapshot)da.Snapshot).ContentType = new MockContentType("c/c++", null);
            da.ResetAndWait();

            da.AssertLinesIncludeExactly(
                new LineSpan(2, 3, 0, LineSpanType.Normal),
                new LineSpan(3, 3, 4, LineSpanType.Normal),
                new LineSpan(5, 5, 0, LineSpanType.Normal),
                new LineSpan(5, 5, 4, LineSpanType.Normal),
                new LineSpan(5, 5, 8, LineSpanType.Normal),
                new LineSpan(7, 8, 0, LineSpanType.Normal),
                new LineSpan(7, 7, 4, LineSpanType.Normal)
            );

            Assert.AreEqual(2, da.GetAllLines().First(ls => ls.FirstLine == 3).LinkedLines.Count());
            Assert.AreEqual(2, da.GetAllLines().First(ls => ls.FirstLine == 5 && ls.Indent == 4).LinkedLines.Count());
            Assert.AreEqual(0, da.GetAllLines().First(ls => ls.FirstLine == 5 && ls.Indent == 8).LinkedLines.Count());
            Assert.AreEqual(2, da.GetAllLines().First(ls => ls.FirstLine == 7).LinkedLines.Count());

            ((MockTextSnapshot)da.Snapshot).ContentType = new MockContentType("not treating pragmas specially", null);
            da.ResetAndWait();

            da.AssertLinesIncludeExactly(
                new LineSpan(2, 3, 0, LineSpanType.Normal),
                new LineSpan(3, 3, 4, LineSpanType.Normal),
                new LineSpan(5, 5, 0, LineSpanType.Normal),
                new LineSpan(7, 8, 0, LineSpanType.Normal),
                new LineSpan(7, 7, 4, LineSpanType.Normal)
            );

            Assert.AreEqual(0, da.GetAllLines().Sum(ls => ls.LinkedLines.Count()));
        }
    }
}
