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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Text;

namespace IndentGuide {
    /// <summary>
    /// Derived classes provide highlighting logic for guides based on the
    /// caret positien.
    /// </summary>
    public abstract class CaretHandlerBase {
        /// <summary>
        /// Updates the internal state based on the line. All lines should be
        /// passed.
        /// </summary>
        /// <param name="lineSpan">
        /// The line to use and potentially update.
        /// </param>
        /// <param name="willUpdateImmediately">
        /// If True, lines modified directly will not be returned again by
        /// GetModified(). However, some lines may be modified indirectly, so a
        /// call to GetModified() is still required.</param>
        public abstract void AddLine(LineSpan lineSpan, bool willUpdateImmediately);

        /// <summary>
        /// When called after updating for each line, this returns a sequence
        /// of lines that require updating.
        /// </summary>
        public abstract IEnumerable<LineSpan> GetModified();

        protected readonly List<LineSpan> Modified;
        protected readonly int LineNumber;
        protected readonly int Position;

        protected CaretHandlerBase(VirtualSnapshotPoint location, int tabSize) {
            if (location.Position.Snapshot != null) {
                var line = location.Position.GetContainingLine();
                LineNumber = line.LineNumber;
                Position = location.Position - line.Start.Position + location.VirtualSpaces;

                int bufferIndent = 0;
                int visualIndent = 0;
                foreach (var c in line.GetText().Take(Position)) {
                    if (c == '\t')
                        bufferIndent += tabSize - (bufferIndent % tabSize);
                    else if (c == ' ')
                        bufferIndent += 1;
                    else
                        break;
                    visualIndent += 1;
                }
                Position += bufferIndent - visualIndent;
                Modified = new List<LineSpan>();
            }
        }

        private static Dictionary<string, Type> LoadedCaretHandlers = new Dictionary<string, Type>();

        public static CaretHandlerBase FromName(string name, VirtualSnapshotPoint location, int tabSize) {
            Type type;
            if (name == null) {
                return new CaretNearestLeft(location, tabSize);
            }
            try {
                if (!LoadedCaretHandlers.TryGetValue(name, out type)) {
                    type = Type.GetType(name);
                    LoadedCaretHandlers[name] = type;
                    Trace.WriteLine("Loaded caret handler " + type.AssemblyQualifiedName);
                }

                return type.InvokeMember(null, System.Reflection.BindingFlags.CreateInstance, null, null,
                    new object[] { location, tabSize }) as CaretHandlerBase;
            } catch (Exception ex) {
                Trace.WriteLine(string.Format("CaretHandler::FromName: {0}", ex));
                return new CaretNearestLeft(location, tabSize);
            }
        }

        internal static ICaretHandlerMetadata MetadataFromName(string name) {
            Type type;
            if (name == null) {
                return null;
            }
            try {
                if (!LoadedCaretHandlers.TryGetValue(name, out type)) {
                    type = Type.GetType(name);
                    LoadedCaretHandlers[name] = type;
                    Trace.WriteLine("Loaded caret handler " + type.AssemblyQualifiedName);
                }

                return type.InvokeMember(null, System.Reflection.BindingFlags.CreateInstance, null, null,
                    new object[] { default(VirtualSnapshotPoint), 0 }) as ICaretHandlerMetadata;
            } catch (Exception ex) {
                Trace.WriteLine(string.Format("CaretHandler::MetadataFromName: {0}", ex));
                return null;
            }
        }
    }

    /// <summary>
    /// Provides metadata for a caret handler.
    /// </summary>
    /// <remarks>
    /// Classes implementing this method must support the normal two parameter
    /// constructor and gracefully handle being passed a null snapshot.
    /// </remarks>
    public interface ICaretHandlerMetadata {
        int GetSortOrder(CultureInfo culture);
        string GetDisplayName(CultureInfo culture);
        string GetDocumentation(CultureInfo culture);
    }
}
