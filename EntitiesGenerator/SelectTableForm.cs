using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EntitiesGenerator
{
    public partial class SelectTableForm : Form
    {
        public SelectTableForm()
        {
            InitializeComponent();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView1_SizeChanged(object sender, EventArgs e)
        {
            this.columnHeader1.Width = this.listView1.Width - 30;
        }

        public ListView ListView
        {
            get
            {
                return this.listView1;
            }
        }

        public void AddListViewItems(ListView.ListViewItemCollection items)
        {
            this.ListView.Items.AddRange(items);
        }

        public ListViewItem AddListViewItem(ListViewItem item)
        {
            return this.ListView.Items.Add(item);
        }

        public ListView.CheckedListViewItemCollection GetCheckedItems()
        {
            return this.listView1.CheckedItems;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.listView1.Items)
            {
                item.Checked = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.listView1.Items)
            {
                item.Checked = !item.Checked;
            }
        }
    }
}
