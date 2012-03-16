using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private static Dictionary<string, Type> LoadedCaretHandlers = new Dictionary<string, Type>();

        public static CaretHandlerBase FromName(string name, VirtualSnapshotPoint location, int tabSize) {
            Type type;
            if (name == null) {
                return new CaretNearestLeft(location, tabSize);
            }

            if (!LoadedCaretHandlers.TryGetValue(name, out type)) {
                type = Type.GetType(name);
                LoadedCaretHandlers[name] = type;
                Trace.WriteLine("Loaded caret handler " + type.AssemblyQualifiedName);
            }
            
            return type.InvokeMember(null, System.Reflection.BindingFlags.CreateInstance, null, null,
                new object[] { location, tabSize }) as CaretHandlerBase;
        }
    }
}
