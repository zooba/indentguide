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
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace IndentGuide.Utils {
    static class Errors {
        private static bool LogMessageBoxShown = false;

        public static void Log(Exception ex) {
            Log(string.Format(
                "Exception raised at {0}:{1}{2}{1}{1}",
                DateTime.UtcNow,
                Environment.NewLine,
                ex.ToString()
            ));
        }

        public static void Log(string detail) {
#if DEBUG
            if (Debugger.IsAttached) {
                Debugger.Break();
            } else {
                Debugger.Launch();
            }
#endif
            try {
                var path = Path.Combine(Path.GetTempPath(), "IndentGuide.log");
                File.AppendAllText(path, detail);

                if (!LogMessageBoxShown) {
                    LogMessageBoxShown = true;
                    var message = string.Format(
                        "An internal error occurred in Indent Guides.{0}{0}" +
                        "Details have been written to the file:{0}    {1}{0}{0}" +
                        "Please report this error at http://indentguide.codeplex.com/.",
                        Environment.NewLine,
                        path
                    );
                    MessageBox.Show(message, "Indent Guides for Visual Studio");
                }
            } catch {
            }
        }

    }
}
