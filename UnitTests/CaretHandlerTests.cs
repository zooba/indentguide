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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IndentGuide;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text;

namespace UnitTests {
    [TestClass]
    public class CaretHandlerTests {
        static List<LineSpan> GetModifiedLines(
            string text,
            int caretPosition,
            Type handler,
            string contentType = "plaintext"
        ) {
            var da = DocumentAnalyzerTests.MakeAnalyzer(text, contentType: contentType);
            var caret = CaretHandlerBase.FromName(
                handler.FullName,
                new VirtualSnapshotPoint(da.Snapshot, caretPosition),
                da.TabSize
            );
            foreach (var line in da.Lines) {
                caret.AddLine(line, false);
            }
            return caret.GetModified().ToList();
        }

        [TestMethod]
        public void CaretNearestBlock() {
            var text = @"
    1
        2
            3
                4
            5

        7

        9
    10
";

            Assert.AreEqual(0, GetModifiedLines(text, text.IndexOf("1"), typeof(CaretNearestLeft)).Count);
            var line = GetModifiedLines(text, text.IndexOf("2"), typeof(CaretNearestLeft));
            Assert.AreEqual(new LineSpan(2, 9, 4, LineSpanType.Normal), line.Single(), line.ToFormattedString());
            line = GetModifiedLines(text, text.IndexOf("3"), typeof(CaretNearestLeft));
            Assert.AreEqual(new LineSpan(3, 6, 8, LineSpanType.Normal), line.Single(), line.ToFormattedString());
            line = GetModifiedLines(text, text.IndexOf("4"), typeof(CaretNearestLeft));
            Assert.AreEqual(new LineSpan(3, 6, 8, LineSpanType.Normal), line.Single(), line.ToFormattedString());
        }

        [TestMethod]
        public void CaretNearestLine() {
            var text = @"
    1
        2
            3
                4
            5

        7

        9
    10
";

            Assert.AreEqual(0, GetModifiedLines(text, text.IndexOf("1"), typeof(CaretNearestLeft2)).Count);
            var line = GetModifiedLines(text, text.IndexOf("2"), typeof(CaretNearestLeft2));
            Assert.AreEqual(new LineSpan(2, 9, 4, LineSpanType.Normal), line.Single(), line.ToFormattedString());
            line = GetModifiedLines(text, text.IndexOf("3"), typeof(CaretNearestLeft2));
            Assert.AreEqual(new LineSpan(3, 6, 8, LineSpanType.Normal), line.Single(), line.ToFormattedString());
            line = GetModifiedLines(text, text.IndexOf("4"), typeof(CaretNearestLeft2));
            Assert.AreEqual(new LineSpan(4, 4, 12, LineSpanType.Normal), line.Single(), line.ToFormattedString());
        }

        [TestMethod]
        public void CaretLinkedLines() {
            var text = @"
    1
        2
#           3
                4
#           5

#       7

        9
    10
";

            Assert.AreEqual(0, GetModifiedLines(text, text.IndexOf("1"), typeof(CaretNearestLeft), "csharp").Count);
            var line = GetModifiedLines(text, text.IndexOf("2"), typeof(CaretNearestLeft), "csharp");
            Assert.AreEqual(4, line.Count, line.ToFormattedString());
        }
    }
}
