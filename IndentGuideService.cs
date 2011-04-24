using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

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
    /// Provides settings storage and update notifications.
    /// </summary>
    [Guid("A65F346F-3F88-4949-BFC1-F5DD3848754F")]
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
        /// Raised when <see cref="Visible"/> changes.
        /// </summary>
        event EventHandler VisibleChanged;
        /// <summary>
        /// Raised when <see cref="LineStyle"/> changes.
        /// </summary>
        event EventHandler LineStyleChanged;
    }

    /// <summary>
    /// The service interface.
    /// </summary>
    [Guid("46A005F6-0E33-47A8-9791-190D4121678E")]
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
            Visible = (bool)props.Item("Visible").Value;
            LineStyle = (LineStyle)props.Item("Line").Value;
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

        #endregion
    }
}
