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
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace IndentGuide {
    class FakeLine : ITextSnapshotLine {
        internal readonly string _Text;
        internal readonly int _Line, _Start;
        public FakeLine(ITextSnapshot snapshot, string text, int line, int start) {
            Snapshot = snapshot;
            _Text = text;
            _Line = line;
            _Start = start;
        }

        public SnapshotPoint Start { get { return new SnapshotPoint(Snapshot, _Start); } }
        public SnapshotPoint End { get { return new SnapshotPoint(Snapshot, _Start + Length); } }
        public SnapshotPoint EndIncludingLineBreak { get { return new SnapshotPoint(Snapshot, _Start + LengthIncludingLineBreak); } }
        public SnapshotSpan Extent { get { return new SnapshotSpan(Start, End); } }
        public SnapshotSpan ExtentIncludingLineBreak { get { return new SnapshotSpan(Start, EndIncludingLineBreak); } }
        public string GetLineBreakText() { return "\n"; }
        public string GetText() { return _Text; }
        public string GetTextIncludingLineBreak() { return GetText() + GetLineBreakText(); }
        public int Length { get { return _Text.Length; } }
        public int LengthIncludingLineBreak { get { return Length + LineBreakLength; } }
        public int LineBreakLength { get { return GetLineBreakText().Length; } }
        public int LineNumber { get { return _Line; } }
        public ITextSnapshot Snapshot { get; set; }
    }

    class FakeContentType : IContentType {
        public FakeContentType(string name) { TypeName = name; }
        public IEnumerable<IContentType> BaseTypes { get { throw new NotImplementedException(); } }
        public string DisplayName { get { return TypeName; } }
        public bool IsOfType(string type) { return string.Equals(type, TypeName); }
        public string TypeName { get; set; }
    }


    class FakeTextBuffer : ITextBuffer {
        public ITextSnapshot CurrentSnapshot { get; set; }

        public void ChangeContentType(IContentType newContentType, object editTag) { throw new NotImplementedException(); }
        public event EventHandler<TextContentChangedEventArgs> Changed { add { } remove { } }
        public event EventHandler<TextContentChangedEventArgs> ChangedHighPriority { add { } remove { } }
        public event EventHandler<TextContentChangedEventArgs> ChangedLowPriority { add { } remove { } }
        public event EventHandler<TextContentChangingEventArgs> Changing { add { } remove { } }
        public bool CheckEditAccess() { throw new NotImplementedException(); }
        public IContentType ContentType { get { return new FakeContentType("CSharp"); } }
        public event EventHandler<ContentTypeChangedEventArgs> ContentTypeChanged { add { } remove { } }
        public ITextEdit CreateEdit() { throw new NotImplementedException(); }
        public ITextEdit CreateEdit(EditOptions options, int? reiteratedVersionNumber, object editTag) { throw new NotImplementedException(); }
        public IReadOnlyRegionEdit CreateReadOnlyRegionEdit() { throw new NotImplementedException(); }
        public ITextSnapshot Delete(Span deleteSpan) { throw new NotImplementedException(); }
        public bool EditInProgress { get { throw new NotImplementedException(); } }
        public NormalizedSpanCollection GetReadOnlyExtents(Span span) { throw new NotImplementedException(); }
        public ITextSnapshot Insert(int position, string text) { throw new NotImplementedException(); }
        public bool IsReadOnly(Span span, bool isEdit) { throw new NotImplementedException(); }
        public bool IsReadOnly(Span span) { throw new NotImplementedException(); }
        public bool IsReadOnly(int position, bool isEdit) { throw new NotImplementedException(); }
        public bool IsReadOnly(int position) { throw new NotImplementedException(); }
        public event EventHandler PostChanged { add { } remove { } }
        public event EventHandler<SnapshotSpanEventArgs> ReadOnlyRegionsChanged { add { } remove { } }
        public ITextSnapshot Replace(Span replaceSpan, string replaceWith) { throw new NotImplementedException(); }
        public void TakeThreadOwnership() { throw new NotImplementedException(); }
        public PropertyCollection Properties { get { throw new NotImplementedException(); } }
    }



    class FakeSnapshot : ITextSnapshot {
        private string _Source;
        private List<FakeLine> _Lines;

        public FakeSnapshot(string source) {
            _Source = source.Replace("\\t", "    ").Replace("\\n", "\n").Replace("\r\n", "\n");
            if (!_Source.EndsWith("\n")) _Source += "\n";

            _Lines = new List<FakeLine>();
            for (int line = 0, start = 0, end = _Source.IndexOf('\n');
                end >= start;
                start = end + 1, end = _Source.IndexOf('\n', start), line += 1) {
                _Lines.Add(new FakeLine(this, _Source.Substring(start, end - start), line, start));
            }
        }

        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count) { _Source.CopyTo(sourceIndex, destination, destinationIndex, count); }

        public ITextSnapshotLine GetLineFromLineNumber(int lineNumber) { return _Lines[lineNumber]; }
        public ITextSnapshotLine GetLineFromPosition(int position) { return GetLineFromLineNumber(GetLineNumberFromPosition(position)); }

        public int GetLineNumberFromPosition(int position) {
            for (int i = 1; i < LineCount; ++i) {
                if (position < _Lines[i].Start)
                    return i - 1;
                position -= _Lines[i].Start;
            }
            return -1;
        }

        public string GetText() { return _Source; }
        public string GetText(int startIndex, int length) { return _Source.Substring(startIndex, length); }
        public string GetText(Span span) { return GetText(span.Start, span.Length); }
        public int Length { get { return _Source.Length; } }
        public int LineCount { get { return _Lines.Count; } }

        public IEnumerable<ITextSnapshotLine> Lines { get { return _Lines; } }
        public ITextBuffer TextBuffer { get { return new FakeTextBuffer { CurrentSnapshot = this }; } }
        public char[] ToCharArray(int startIndex, int length) { return _Source.ToCharArray(startIndex, length); }
        public void Write(System.IO.TextWriter writer) { writer.Write(GetText()); }
        public void Write(System.IO.TextWriter writer, Span span) { writer.Write(GetText(span)); }
        public char this[int position] { get { return _Source[position]; } }

        public IContentType ContentType { get { return new FakeContentType("CSharp"); } }
        public ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode, TrackingFidelityMode trackingFidelity) { throw new NotImplementedException(); }
        public ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode) { throw new NotImplementedException(); }
        public ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode, TrackingFidelityMode trackingFidelity) { throw new NotImplementedException(); }
        public ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode) { throw new NotImplementedException(); }
        public ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode, TrackingFidelityMode trackingFidelity) { throw new NotImplementedException(); }
        public ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode) { throw new NotImplementedException(); }
        public ITextVersion Version { get { throw new NotImplementedException(); } }
    }

}
