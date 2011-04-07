using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace QuickOpenFile
{
    /// <summary>
    /// Summary description for MyControl.
    /// </summary>
    public partial class QuickOpenFileControl : UserControl, IVsUIWin32Element
    {
        private SolutionReader solutionReader = new SolutionReader();
        private List<SolutionFile> solutionFiles;

        public QuickOpenFileControl()
        {
            InitializeComponent();
        }

        // This method is called when the Open Resource window is shown.
        // Creates an index of all solution items for subsequent incremental searches.
        public void InitOnShow()
        {
            uxSearch.Focus();
            uxSearch.SelectAll();
            uxFiles.Items.Clear();

            IndexSolutionItems(); // this fills the index list

            uxSearch_TextChanged(this, EventArgs.Empty); // this causes to repeat the last search, fills the list view
        }

        private void uxSearch_TextChanged(object sender, System.EventArgs e)
        {
            // incrementally search the index
            string query = uxSearch.Text.Trim();
            if (query.Length < 2) return;

            // replace ? and * wildcards in the string with proper regex syntax
            string queryRegex = query.Replace('?', '.').Replace("*", ".*");

            uxFiles.Items.Clear();
            uxOpen.Enabled = false;
            uxStatus.Text = "";

            try
            {
                var found = FindResources(queryRegex);
                uxFiles.BeginUpdate();
                foreach (SolutionFile sr in found)
                {
                    ListViewItem lvi = uxFiles.Items.Add(sr.Item);
                }
                uxFiles.EndUpdate();

                if (uxFiles.Items.Count > 0)
                {
                    uxFiles.Items[0].Selected = true;
                    uxOpen.Enabled = true;
                }
            }
            catch (ArgumentException)
            {
                ListViewItem lvi = uxFiles.Items.Add("Invalid regular expression.");
            }
        }

        /// <summary>
        /// Returns list of resources that match the query string
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IEnumerable<SolutionFile> FindResources(string query)
        {
            var regexes = query.Split().Select(q => new Regex(q, RegexOptions.IgnoreCase));

            //TODO: show most recently used resources first
            //NOTE: for now, just sorts alphabetically (by Name, then by FilePath)
            return solutionFiles.
                Where(sr =>
                {
                    if (string.IsNullOrEmpty(sr.Name) || string.IsNullOrEmpty(sr.FilePath))
                        return false;

                    // Exclude canonical names that appear to be directories
                    if (sr.FilePath.EndsWith("\\") || sr.FilePath.EndsWith("/"))
                        return false;

                    return regexes.All(r => r.IsMatch(sr.Name));
                }).
                Distinct(new DistinctByFilePath()).
                OrderBy(item => item.LastWriteTime);
        }

        class DistinctByFilePath : IEqualityComparer<SolutionFile>
        {
            public bool Equals(SolutionFile x, SolutionFile y)
            {
                if (x == null ^ y == null) return false;
                return x.FilePath.Equals(y.FilePath, StringComparison.InvariantCultureIgnoreCase);
            }

            public int GetHashCode(SolutionFile file)
            {
                return file.FilePath.GetHashCode();
            }
        }

        private IEnumerable<SolutionFile> GetSelectedSolutionFiles()
        {
            IEnumerable<ListViewItem> files;
            if (uxFiles.CheckedItems.Count > 0)
            {
                files = uxFiles.CheckedItems.Cast<ListViewItem>();
            }
            else
            {
                files = uxFiles.SelectedItems.Cast<ListViewItem>();
            }

            return files.
                Where(i => i.Tag != null).
                Select(i => i.Tag as SolutionFile);
        }

        private bool OpenSolutionResource(SolutionFile sr, string editor)
        {
            if (sr == null) return false;
            if (string.IsNullOrEmpty(sr.FilePath)) return false;
            IVsCommandWindow cmw = (IVsCommandWindow)GetService(typeof(SVsCommandWindow));
            if (cmw == null) return false;

            if (editor == null)
            {
                cmw.ExecuteCommand("of \"" + sr.FilePath + "\"");
            }
            else if (editor.Length == 0)
            {
                cmw.ExecuteCommand("of \"" + sr.FilePath + "\" /editor");
            }
            else
            {
                cmw.ExecuteCommand("of \"" + sr.FilePath + "\" /e:\"" + editor + "\"");
            }

            return true;
        }

        private void IndexSolutionItems()
        {
            var start = DateTime.Now;
            solutionFiles = solutionReader.GetSolutionFiles((IVsSolution)GetService(typeof(SVsSolution)));
            uxStatus.Text = "Indexed " + solutionFiles.Count + " solution files in " + (DateTime.Now - start) + ".";
        }

        private void OnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) // close tool window
            {
                HideToolWindow();
            }
        }

        private void uxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    uxFiles.Focus();
                    e.SuppressKeyPress = true;
                    break;
                case Keys.PageDown:
                    uxFiles.Focus();
                    SendKeys.Send("{PGDN}");
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Return:
                    OpenSelectedFiles(e.Shift ? string.Empty : null);
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Escape:
                    HideToolWindow();
                    break;
            }
        }

        private void uxFiles_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != ' ')
            {
                uxSearch.Focus();
                SendKeys.Send(e.KeyChar.ToString());
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void uxFiles_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.PageUp:
                case Keys.Up:
                    if (uxFiles.SelectedIndices.Contains(0))
                    {
                        uxSearch.Focus();
                        e.SuppressKeyPress = true;
                    }
                    break;
                case Keys.Return:
                    OpenSelectedFiles(e.Shift ? string.Empty : null);
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Escape:
                    HideToolWindow();
                    break;
                default:
                    e.SuppressKeyPress = false;
                    break;
            }
        }


        private void uxOpen_Click(object sender, EventArgs e)
        {
            OpenSelectedFiles();
        }

        private void uxFiles_DoubleClick(object sender, EventArgs e)
        {
            OpenSelectedFiles();
        }

        private void uxFiles_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            uxStatus.Text = "";
            uxStatus.Tag = null;
            if (uxFiles.SelectedItems.Count > 0)
            {
                uxOpen.Enabled = true;
                if (uxFiles.SelectedItems[0].Tag != null)
                {
                    SolutionFile sr = (SolutionFile)uxFiles.SelectedItems[0].Tag;
                    uxStatus.Tag = sr;
                    uxStatus.Text = sr.FilePath;
                }
            }
            else
            {
                uxOpen.Enabled = false;
            }
        }

        private void uxOpenWith_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (uxFiles.SelectedItems.Count == 0)
                e.Cancel = true;
            else
            {
                //TODO: maybe directly fill in list of available editors for the item in the future
                openWithToolStripMenuItem.DropDownItems.Clear();
            }
        }

        private void OpenSelectedFiles(string editor = null)
        {
            bool success = true;
            // open the selected item in default editor using command window and close this toolbox
            foreach (var file in GetSelectedSolutionFiles())
            {
                if (!OpenSolutionResource(file, editor))
                {
                    success = false;
                }
            }

            if (success)
            {
                HideToolWindow();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSelectedFiles();
        }

        private void openWithToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSelectedFiles(string.Empty);
        }

        private void uxOpenWith_MouseDown(object sender, MouseEventArgs e)
        {
            uxOpenWithMenu.Show(uxOpen, new System.Drawing.Point(0, 0));
        }

        private void uxOpen_EnabledChanged(object sender, EventArgs e)
        {
            uxOpenWith.Enabled = uxOpen.Enabled;
        }

        private void HideToolWindow()
        {
            ToolWindowPane window = QuickOpenFilePackage.Instance.FindToolWindow(typeof(QuickOpenFileToolWindow), 0, false);
            if ((null != window) && (null != window.Frame))
            {
                IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
                windowFrame.Hide();
            }
        }

        /// <summary> 
        /// Let this control process the mnemonics.
        /// </summary>
        [UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
        protected override bool ProcessDialogChar(char charCode)
        {
            // If we're the top-level form or control, we need to do the mnemonic handling
            if (charCode != ' ' && ProcessMnemonic(charCode))
            {
                return true;
            }
            return base.ProcessDialogChar(charCode);
        }

        /// <summary>
        /// Enable the IME status handling for this control.
        /// </summary>
        protected override bool CanEnableIme
        {
            get
            {
                return true;
            }
        }

        #region IVsUIWin32Element Members

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        public int Create(IntPtr parent, out IntPtr pHandle)
        {
            pHandle = this.Handle;
            SetParent(this.Handle, parent);
            return 1;
            //throw new NotImplementedException();
        }

        public int Destroy()
        {
            return 1;
            //throw new NotImplementedException();
        }

        public int GetHandle(out IntPtr pHandle)
        {
            pHandle = this.Handle;
            return 1;
        }

        public int ShowModal(IntPtr parent, out int pDlgResult)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
