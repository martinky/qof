using System;
using System.Windows.Forms;

namespace QuickOpenFile
{
    public class SolutionFile
    {
        public string Name { get; set; }
        //public string Path { get; set; }
        public string FilePath { get; set; }
        public string Project { get; set; }
        //public string Type { get; set; }
        //public string SubType { get; set; }
        public uint ItemId { get; set; }
        public int IconIndex { get; set; }
        public System.DateTime LastWriteTime { get; set; }

        public ListViewItem Item
        {
            get
            {
                if (lvItem == null)
                    CreateListViewItem();
                return lvItem;
            }
        }

        private void CreateListViewItem()
        {
            // This is intended to be invoked from the UI thread only. Because
            // indexing and searching has been moved to background thread, the
            // list view item (which is a GUI class) must be created from the UI
            // thread, just before adding to the listbox.
            lvItem = new ListViewItem(Name);
            lvItem.SubItems.Add(Project);
            lvItem.SubItems.Add(FilePath);
            lvItem.Tag = this;
        }

        private ListViewItem lvItem;
    }
}
