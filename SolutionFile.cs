using System.Windows.Forms;
namespace QuickOpenFile
{
    public class SolutionFile
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string FilePath { get; set; }
        public string Project { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public uint ItemId { get; set; }
        public int IconIndex { get; set; }
        public System.DateTime LastWriteTime { get; set; }

        public ListViewItem Item { get; private set; }

        public void CreateListViewItem()
        {
            Item = new ListViewItem(Name);
            Item.SubItems.Add(Project);
            Item.SubItems.Add(FilePath);
            Item.Tag = this;
        }
    }
}
