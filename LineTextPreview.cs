using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace IndentGuide {
    public partial class LineTextPreview : UserControl {
        public LineTextPreview() {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw | ControlStyles.Opaque, true);

            InitializeComponent();

            IndentSize = 4;
            _Theme = null;
        }

        public int IndentSize { get; set; }

        private IndentTheme _Theme;
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IndentTheme Theme {
            get { return _Theme; }
            set { _Theme = value; Invalidate(); }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text {
            get { return base.Text; }
            set { base.Text = (value ?? ""); Invalidate(); }
        }

        protected override void OnTextChanged(EventArgs e) {
            base.OnTextChanged(e);
            Invalidate();
        }

        private Pen GetLinePen(int formatIndex) {
            LineFormat format;

            if (!Theme.LineFormats.TryGetValue(formatIndex, out format))
                format = Theme.DefaultLineFormat;

            if (!format.Visible) return null;

            Pen pen = null;

            pen = new Pen(format.LineColor, (float)format.LineStyle.GetStrokeThickness());

            var pattern = format.LineStyle.GetDashPattern();
            if (pattern == null) {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Flat;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Flat;
            } else {
                pen.DashPattern = pattern;
            }
            return pen;
        }

        private class FakeLine : Microsoft.VisualStudio.Text.ITextSnapshotLine {
            internal readonly string _Text;
            internal readonly int _Line, _Start;
            public FakeLine(Microsoft.VisualStudio.Text.ITextSnapshot snapshot, string text, int line, int start) {
                Snapshot = snapshot;
                _Text = text;
                _Line = line;
                _Start = start;
            }

            public Microsoft.VisualStudio.Text.SnapshotPoint Start { get { return new Microsoft.VisualStudio.Text.SnapshotPoint(Snapshot, _Start); } }
            public Microsoft.VisualStudio.Text.SnapshotPoint End { get { return new Microsoft.VisualStudio.Text.SnapshotPoint(Snapshot, _Start + Length); } }
            public Microsoft.VisualStudio.Text.SnapshotPoint EndIncludingLineBreak { get { return new Microsoft.VisualStudio.Text.SnapshotPoint(Snapshot, _Start + LengthIncludingLineBreak); } }
            public Microsoft.VisualStudio.Text.SnapshotSpan Extent { get { return new Microsoft.VisualStudio.Text.SnapshotSpan(Start, End); } }
            public Microsoft.VisualStudio.Text.SnapshotSpan ExtentIncludingLineBreak { get { return new Microsoft.VisualStudio.Text.SnapshotSpan(Start, EndIncludingLineBreak); } }
            public string GetLineBreakText() { return "\n"; }
            public string GetText() { return _Text; }
            public string GetTextIncludingLineBreak() { return GetText() + GetLineBreakText(); }
            public int Length { get { return _Text.Length; } }
            public int LengthIncludingLineBreak { get { return Length + LineBreakLength; } }
            public int LineBreakLength { get { return GetLineBreakText().Length; } }
            public int LineNumber { get { return _Line; } }
            public Microsoft.VisualStudio.Text.ITextSnapshot Snapshot { get; set; }
        }

        private class FakeTextBuffer : Microsoft.VisualStudio.Text.ITextBuffer {
            public Microsoft.VisualStudio.Text.ITextSnapshot CurrentSnapshot { get; set; }

            public void ChangeContentType(Microsoft.VisualStudio.Utilities.IContentType newContentType, object editTag) { throw new NotImplementedException(); }
            public event EventHandler<Microsoft.VisualStudio.Text.TextContentChangedEventArgs> Changed { add { } remove { } }
            public event EventHandler<Microsoft.VisualStudio.Text.TextContentChangedEventArgs> ChangedHighPriority { add { } remove { } }
            public event EventHandler<Microsoft.VisualStudio.Text.TextContentChangedEventArgs> ChangedLowPriority { add { } remove { } }
            public event EventHandler<Microsoft.VisualStudio.Text.TextContentChangingEventArgs> Changing { add { } remove { } }
            public bool CheckEditAccess() { throw new NotImplementedException(); }
            public Microsoft.VisualStudio.Utilities.IContentType ContentType { get { throw new NotImplementedException(); } }
            public event EventHandler<Microsoft.VisualStudio.Text.ContentTypeChangedEventArgs> ContentTypeChanged { add { } remove { } }
            public Microsoft.VisualStudio.Text.ITextEdit CreateEdit() { throw new NotImplementedException(); }
            public Microsoft.VisualStudio.Text.ITextEdit CreateEdit(Microsoft.VisualStudio.Text.EditOptions options, int? reiteratedVersionNumber, object editTag) { throw new NotImplementedException(); }
            public Microsoft.VisualStudio.Text.IReadOnlyRegionEdit CreateReadOnlyRegionEdit() { throw new NotImplementedException(); }
            public Microsoft.VisualStudio.Text.ITextSnapshot Delete(Microsoft.VisualStudio.Text.Span deleteSpan) { throw new NotImplementedException(); }
            public bool EditInProgress { get { throw new NotImplementedException(); } }
            public Microsoft.VisualStudio.Text.NormalizedSpanCollection GetReadOnlyExtents(Microsoft.VisualStudio.Text.Span span) { throw new NotImplementedException(); }
            public Microsoft.VisualStudio.Text.ITextSnapshot Insert(int position, string text) { throw new NotImplementedException(); }
            public bool IsReadOnly(Microsoft.VisualStudio.Text.Span span, bool isEdit) { throw new NotImplementedException(); }
            public bool IsReadOnly(Microsoft.VisualStudio.Text.Span span) { throw new NotImplementedException(); }
            public bool IsReadOnly(int position, bool isEdit) { throw new NotImplementedException(); }
            public bool IsReadOnly(int position) { throw new NotImplementedException(); }
            public event EventHandler PostChanged { add { } remove { } }
            public event EventHandler<Microsoft.VisualStudio.Text.SnapshotSpanEventArgs> ReadOnlyRegionsChanged { add { } remove { } }
            public Microsoft.VisualStudio.Text.ITextSnapshot Replace(Microsoft.VisualStudio.Text.Span replaceSpan, string replaceWith) { throw new NotImplementedException(); }
            public void TakeThreadOwnership() { throw new NotImplementedException(); }
            public Microsoft.VisualStudio.Utilities.PropertyCollection Properties { get { throw new NotImplementedException(); } }
        }



        private class FakeSnapshot : Microsoft.VisualStudio.Text.ITextSnapshot {
            private string _Source;
            private List<FakeLine> _Lines;

            public FakeSnapshot(string source) {
                _Source = source.Replace("\\n", "\n").Replace("\r\n", "\n");
                if (!_Source.EndsWith("\n")) _Source += "\n";

                _Lines = new List<FakeLine>();
                for (int line = 0, start = 0, end = _Source.IndexOf('\n');
                    end >= start;
                    start = end + 1, end = _Source.IndexOf('\n', start), line += 1) {
                    _Lines.Add(new FakeLine(this, _Source.Substring(start, end - start), line, start));
                }
            }

            public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count) { _Source.CopyTo(sourceIndex, destination, destinationIndex, count); }

            public Microsoft.VisualStudio.Text.ITextSnapshotLine GetLineFromLineNumber(int lineNumber) { return _Lines[lineNumber]; }
            public Microsoft.VisualStudio.Text.ITextSnapshotLine GetLineFromPosition(int position) { return GetLineFromLineNumber(GetLineNumberFromPosition(position)); }

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
            public string GetText(Microsoft.VisualStudio.Text.Span span) { return GetText(span.Start, span.Length); }
            public int Length { get { return _Source.Length; } }
            public int LineCount { get { return _Lines.Count; } }

            public IEnumerable<Microsoft.VisualStudio.Text.ITextSnapshotLine> Lines { get { return _Lines; } }
            public Microsoft.VisualStudio.Text.ITextBuffer TextBuffer { get { return new FakeTextBuffer { CurrentSnapshot = this }; } }
            public char[] ToCharArray(int startIndex, int length) { return _Source.ToCharArray(startIndex, length); }
            public void Write(System.IO.TextWriter writer) { writer.Write(GetText()); }
            public void Write(System.IO.TextWriter writer, Microsoft.VisualStudio.Text.Span span) { writer.Write(GetText(span)); }
            public char this[int position] { get { return _Source[position]; } }

            public Microsoft.VisualStudio.Utilities.IContentType ContentType { get { throw new NotImplementedException(); } }
            public Microsoft.VisualStudio.Text.ITrackingPoint CreateTrackingPoint(int position, Microsoft.VisualStudio.Text.PointTrackingMode trackingMode, Microsoft.VisualStudio.Text.TrackingFidelityMode trackingFidelity) { throw new NotImplementedException(); }
            public Microsoft.VisualStudio.Text.ITrackingPoint CreateTrackingPoint(int position, Microsoft.VisualStudio.Text.PointTrackingMode trackingMode) { throw new NotImplementedException(); }
            public Microsoft.VisualStudio.Text.ITrackingSpan CreateTrackingSpan(int start, int length, Microsoft.VisualStudio.Text.SpanTrackingMode trackingMode, Microsoft.VisualStudio.Text.TrackingFidelityMode trackingFidelity) { throw new NotImplementedException(); }
            public Microsoft.VisualStudio.Text.ITrackingSpan CreateTrackingSpan(int start, int length, Microsoft.VisualStudio.Text.SpanTrackingMode trackingMode) { throw new NotImplementedException(); }
            public Microsoft.VisualStudio.Text.ITrackingSpan CreateTrackingSpan(Microsoft.VisualStudio.Text.Span span, Microsoft.VisualStudio.Text.SpanTrackingMode trackingMode, Microsoft.VisualStudio.Text.TrackingFidelityMode trackingFidelity) { throw new NotImplementedException(); }
            public Microsoft.VisualStudio.Text.ITrackingSpan CreateTrackingSpan(Microsoft.VisualStudio.Text.Span span, Microsoft.VisualStudio.Text.SpanTrackingMode trackingMode) { throw new NotImplementedException(); }
            public Microsoft.VisualStudio.Text.ITextVersion Version { get { throw new NotImplementedException(); } }
        }


        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.Clear(BackColor);

            float spaceLeft, spaceWidth;
            using (var sf = new StringFormat()) {
                RectangleF rect = ClientRectangle;
                rect.Width *= 2.0f;

                sf.SetMeasurableCharacterRanges(new[] { new CharacterRange(0, 8) });

                var rgns = e.Graphics.MeasureCharacterRanges("        {", Font, rect, sf);
                var bounds = rgns[0].GetBounds(e.Graphics);
                spaceLeft = bounds.Left;
                spaceWidth = bounds.Width / 8;

#if DEBUG
                sf.Alignment = StringAlignment.Far;
                rect.Width /= 2;
                using (var brush = new SolidBrush(ForeColor))
                    e.Graphics.DrawString(string.Format("{0}, {1}", spaceLeft, spaceWidth), Font, brush, rect, sf);
#endif
            }

            try {
                var snapshot = new FakeSnapshot(Text);
                using (var brush = new SolidBrush(ForeColor)) {
                    foreach (var line in snapshot.Lines) {
                        e.Graphics.DrawString(line.GetText(), Font, brush, 0, line.LineNumber * Font.Height);
                    }
                }

                if (Theme == null) return;

                var analysis = new DocumentAnalyzer(snapshot, Theme.Behavior, IndentSize);

                foreach (var line in analysis.Lines) {
                    int linePos = line.Indent;
                    if (!analysis.Behavior.VisibleUnaligned && (linePos % analysis.IndentSize) != 0)
                        continue;

                    float top = line.FirstLine * Font.Height;
                    float bottom = (line.LastLine + 1) * Font.Height;
                    float left = line.Indent * spaceWidth + spaceLeft;

                    int formatIndex = line.Indent / analysis.IndentSize;
                    if (line.Indent % analysis.IndentSize != 0)
                        formatIndex = IndentTheme.UnalignedFormatIndex;

                    var pen = GetLinePen(formatIndex);
                    if (pen != null) {
                        e.Graphics.DrawLine(pen, left, top, left, bottom);
                        pen.Dispose();
                    }
                }
            } catch (Exception ex) {
                Trace.WriteLine("LineTextPreview::OnPaint Error");
                Trace.WriteLine(" - Exception: " + ex.ToString());
                e.Graphics.DrawString(ex.ToString(), Font, Brushes.Black, 0.0f, 0.0f);
            }
        }
    }
}
