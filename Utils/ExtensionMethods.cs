/* ****************************************************************************
 * Copyright 2012 Steve Dower
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

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.Text.Editor;

namespace IndentGuide {
    internal static class ExtensionMethods {
        public static int LeadingWhitespace(this string source, int tabSize) {
            int count = 0;
            for (int i = 0; i < source.Length; ++i) {
                var c = source[i];
                if (c == '\t') {
                    count += tabSize - (count % tabSize);
                } else if (c == ' ') {
                    count += 1;
                } else {
                    break;
                }
            }
            return count;
        }

        public static int ActualLength(this string source, int tabSize) {
            int count = 0;
            for (int i = 0; i < source.Length; ++i) {
                var c = source[i];
                if (c == '\t') {
                    count += tabSize - (count % tabSize);
                } else {
                    count += 1;
                }
            }
            return count;
        }

        public static System.Drawing.Color ToSDC(this System.Windows.Media.Color source) {
            return System.Drawing.Color.FromArgb(source.A, source.R, source.G, source.B);
        }

        public static System.Windows.Media.Color ToSWMC(this System.Drawing.Color source) {
            return System.Windows.Media.Color.FromArgb(source.A, source.R, source.G, source.B);
        }

        public static System.Drawing.Color AsInverted(this System.Drawing.Color source) {
            return System.Drawing.Color.FromArgb(source.A, 255 - source.R, 255 - source.G, 255 - source.B);
        }

        public static float[] ToFloatArray(this IEnumerable<double> source) {
            return source.Select(i => (float)i).ToArray();
        }
    }
}
