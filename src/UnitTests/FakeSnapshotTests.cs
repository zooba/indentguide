/* ****************************************************************************
 * Copyright 2015 Steve Dower
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

using IndentGuide;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestUtilities.Mocks;

namespace UnitTests {
    [TestClass]
    public class FakeSnapshotTests {
        [TestMethod]
        public void CompareToMockSnapshot() {
            const string Text = @"try
{
    if (a == 0)
    {
        return foo(a,
                   10);

    }
    else
    {
        while(a > 0)
            a -= 1;

    }
}
";
            var fake = new FakeSnapshot(Text);
            var mock = new MockTextBuffer(Text).CurrentSnapshot;

            Assert.AreEqual(mock.Length, fake.Length, "Length does not match");
            for (int i = 0; i < mock.LineCount && i < fake.LineCount; ++i) {
                var mockLine = mock.GetLineFromLineNumber(i);
                var fakeLine = fake.GetLineFromLineNumber(i);
                Assert.AreEqual(mockLine.GetText(), fakeLine.GetText(), string.Format("Line {0} text does not match", i));
                Assert.AreEqual(mockLine.Start.Position, fakeLine.Start.Position, string.Format("Line {0} start does not match", i));
                Assert.AreEqual(mockLine.End.Position, fakeLine.End.Position, string.Format("Line {0} end does not match", i));
            }
            Assert.AreEqual(mock.LineCount, fake.LineCount, "LineCount does not match");
        }
    }
}
