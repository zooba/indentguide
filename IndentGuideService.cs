using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace IndentGuide
{
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
        /// The loaded themes.
        /// </summary>
        IDictionary<string, IndentTheme> Themes { get; }
        /// <summary>
        /// The default theme.
        /// </summary>
        IndentTheme DefaultTheme { get; }

        /// <summary>
        /// Raised when the collection of themes changes.
        /// </summary>
        event EventHandler ThemesChanged;
        /// <summary>
        /// Raised when the global visibility changes.
        /// </summary>
        event EventHandler VisibleChanged;
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
            // Force the options to be loaded. DisplayOptions will then
            // initialise the values here.
            dte.get_Properties("IndentGuide", "Display");
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

        public IDictionary<string, IndentTheme> Themes { get; set; }
        public IndentTheme DefaultTheme { get; set; }

        public void OnThemesChanged()
        {
            var evt = ThemesChanged;
            if (evt != null) evt(this, EventArgs.Empty);
        }

        public event EventHandler ThemesChanged;

        #endregion
    }
}
