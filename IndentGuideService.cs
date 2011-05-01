using System;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace IndentGuide
{
    /// <summary>
    /// The supported styles of guideline.
    /// </summary>
    public enum LineStyle
    {
        Solid,
        Thick,
        Dotted,
        Dashed
    }

    /// <summary>
    /// The supported modes for handling empty lines.
    /// </summary>
    public enum EmptyLineMode
    {
        NoGuides,
        SameAsLineAboveActual,
        SameAsLineAboveLogical,
        SameAsLineBelowActual,
        SameAsLineBelowLogical
    }
    
    /// <summary>
    /// Provides settings storage and update notifications.
    /// </summary>
    [Guid(Guids.IIndentGuideGuid)]
    [ComVisible(true)]
    public interface IIndentGuide
    {
        /// <summary>
        /// Whether guides are shown or not.
        /// </summary>
        bool Visible { get; set; }
        /// <summary>
        /// The style of guides shown.
        /// </summary>
        LineStyle LineStyle { get; set; }
        /// <summary>
        /// The color to use for guide lines.
        /// </summary>
        Color LineColor { get; set; }
        /// <summary>
        /// The mode to use for empty lines.
        /// </summary>
        EmptyLineMode EmptyLineMode { get; set; }

        /// <summary>
        /// Raised when <see cref="Visible"/> changes.
        /// </summary>
        event EventHandler VisibleChanged;
        /// <summary>
        /// Raised when <see cref="LineStyle"/> changes.
        /// </summary>
        event EventHandler LineStyleChanged;
        /// <summary>
        /// Raised when <see cref="LineColor"/> changes.
        /// </summary>
        event EventHandler LineColorChanged;
        /// <summary>
        /// Raised when <see cref="EmptyLineMode"/> changes.
        /// </summary>
        event EventHandler EmptyLineModeChanged;
    }

    /// <summary>
    /// The service interface.
    /// </summary>
    [Guid(Guids.SIndentGuideGuid)]
    public interface SIndentGuide
    {
        void Initialize(EnvDTE.DTE dte);
    }

    /// <summary>
    /// Implementation of the service supporting Indent Guides.
    /// </summary>
    class IndentGuideService : SIndentGuide, IIndentGuide
    {
        public IndentGuideService() { }
        
        void SIndentGuide.Initialize(EnvDTE.DTE dte)
        {
            var props = dte.get_Properties("IndentGuide", "Display");

            try { Visible = (bool)props.Item("Visible").Value; }
            catch { }

            try { LineStyle = (LineStyle)props.Item("LineStyle").Value; }
            catch { }

            try { LineColor = (Color)props.Item("LineColor").Value; }
            catch { }

            try { EmptyLineMode = (EmptyLineMode)props.Item("EmptyLineMode").Value; }
            catch { }
        }

        #region IIndentGuide Members

        private bool _Visible = true;
        public bool Visible
        {
            get { return _Visible; }
            set
            {
                if (_Visible != value)
                {
                    _Visible = value;

                    var evt = VisibleChanged;
                    if (evt != null) evt(this, EventArgs.Empty);
                }
            }
        }
        
        public event EventHandler VisibleChanged;

        private LineStyle _LineStyle = LineStyle.Dotted;
        public LineStyle LineStyle
        {
            get { return _LineStyle; }
            set
            {
                if (_LineStyle != value)
                {
                    _LineStyle = value;

                    var evt = LineStyleChanged;
                    if (evt != null) evt(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler LineStyleChanged;

        private Color _LineColor = Colors.Teal;
        public Color LineColor
        {
            get { return _LineColor; }
            set
            {
                if (_LineColor != value)
                {
                    _LineColor = value;

                    var evt = LineColorChanged;
                    if (evt != null) evt(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler LineColorChanged;

        private EmptyLineMode _EmptyLineMode = EmptyLineMode.SameAsLineAboveLogical;
        public EmptyLineMode EmptyLineMode
        {
            get { return _EmptyLineMode; }
            set
            {
                if (_EmptyLineMode != value)
                {
                    _EmptyLineMode = value;

                    var evt = EmptyLineModeChanged;
                    if (evt != null) evt(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler EmptyLineModeChanged;

        #endregion
    }
}
