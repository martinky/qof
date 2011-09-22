using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace QuickOpenFile
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    ///
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane, 
    /// usually implemented by the package implementer.
    ///
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its 
    /// implementation of the IVsUIElementPane interface.
    /// </summary>
    [Guid("ea72eed9-3d87-4b35-a79a-eedec76be2a4")]
    public class QuickOpenFileToolWindow : ToolWindowPane
    {
        // This is the user control hosted by the tool window; it is exposed to the base class 
        // using the Content property. Note that, even if this class implements IDispose, we are
        // not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
        // the object returned by the Content property.
        private QuickOpenFileControl control;
        private const int WM_KEYDOWN = 0x0100;
        private const int VK_ESCAPE = 0x1B;

        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public QuickOpenFileToolWindow() :
            base(null)
        {
            // Set the window title reading it from the resources.
            this.Caption = Resources.ToolWindowTitle;
            // Set the image that will appear on the tab of the window frame
            // when docked with an other window
            // The resource ID correspond to the one defined in the resx file
            // while the Index is the offset in the bitmap strip. Each image in
            // the strip being 16x16.
            this.BitmapResourceID = 301;
            this.BitmapIndex = 2;
        }

        // Initializes the search UI and indexes the opened solution for fast search.
        public void InitControl()
        {
            control.InitControl();
        }

        // Hides the tool window, except when docked.
        public void HideToolWindow()
        {
            if (null != Frame)
            {
                IVsWindowFrame parentWindowFrame = (IVsWindowFrame)Frame;
                object frameMode;
                parentWindowFrame.GetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, out frameMode);
                VSFRAMEMODE frameModeEnum = (VSFRAMEMODE)frameMode;
                if (frameModeEnum == VSFRAMEMODE.VSFM_Float || frameModeEnum == VSFRAMEMODE.VSFM_FloatOnly)
                    parentWindowFrame.Hide();
            }
        }

        protected override bool PreProcessMessage(ref Message m)
        {
            if (m.Msg == WM_KEYDOWN && m.WParam.ToInt32() == VK_ESCAPE)
            {
                HideToolWindow();
                return true;
            }
            else
            {
                return base.PreProcessMessage(ref m);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (control != null)
            {
                control.CleanUp();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// This property returns the control that should be hosted in the Tool Window.
        /// It can be either a FrameworkElement (for easy creation of toolwindows hosting WPF content), 
        /// or it can be an object implementing one of the IVsUIWPFElement or IVsUIWin32Element interfaces.
        /// </summary>
        public override object Content
        {
            get
            {
                if (control == null)
                {
                    control = new QuickOpenFileControl();
                    control.parentWindowPane = this;
                }

                return control;
            }
        }

        /// <summary>
        /// This property returns the handle to the user control that should
        /// be hosted in the Tool Window.
        /// </summary>
        public override IWin32Window Window
        {
            get
            {
                return (IWin32Window)control;
            }
        }

        public void SetPackage(QuickOpenFilePackage package)
        {
            control.SetPackage(package);
        }
    }
}
