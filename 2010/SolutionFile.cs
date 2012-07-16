using System;
using System.Windows.Forms;

namespace QuickOpenFile
{
    public class SolutionFile
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public string Project { get; set; }
        public uint ItemId { get; set; }
        public int IconIndex { get; set; }
        public System.DateTime LastWriteTime { get; set; }

        public ListViewItem Item
        {
            get
            {
                if (listViewItem == null)
                {
                    CreateListViewItem();
                }

                return listViewItem;
            }
        }

        private void CreateListViewItem()
        {
            // This is intended to be invoked from the UI thread only. Because
            // indexing and searching has been moved to background thread, the
            // list view item (which is a GUI class) must be created from the UI
            // thread, just before adding to the listbox.
            listViewItem = new ListViewItem(Name);
            listViewItem.SubItems.Add(Project);
            listViewItem.SubItems.Add(FilePath);
            listViewItem.Tag = this;
        }

        private ListViewItem listViewItem;
    }
}
