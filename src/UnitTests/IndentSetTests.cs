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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IndentGuide;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests {
    [TestClass]
    public class IndentSetTests {

        [TestMethod]
        public void SetSingleTest() {
            for (int i = 0; i < 129; ++i) {
                var set = DocumentAnalyzer.IndentSet.Empty;
                set.Set(i);
                try {
                    var values = set.GetAll().ToList();
                    if (i < 128) {
                        Assert.IsTrue(set.Any());
                        Assert.AreEqual(1, values.Count);
                        Assert.AreEqual(i, values[0]);
                    } else {
                        Assert.IsFalse(set.Any());
                        Assert.AreEqual(0, values.Count);
                    }
                } catch (Exception) {
                    Console.WriteLine("i = {0}", i);
                    Console.WriteLine(set.Dump());
                    throw;
                }
            }
        }

        [TestMethod]
        public void SetManyTest() {
            var set = DocumentAnalyzer.IndentSet.Empty;
            var expected = new List<int>();

            for (int i = 0; i < 129; ++i) {
                set.Set(i);
                if (i < 128) {
                    expected.Add(i);
                }
                try {
                    var values = set.GetAll().ToList();
                    Assert.IsTrue(set.Any(), "set.Any() != true");
                    Assert.AreEqual(expected.Count, values.Count, "Counts do not match");
                    Assert.IsTrue(values.All(j => expected.Contains(j)), "Not all values are there");
                } catch (Exception) {
                    Console.WriteLine("i = {0}", i);
                    Console.WriteLine(set.Dump());
                    throw;
                }
            }
        }
    }
}
