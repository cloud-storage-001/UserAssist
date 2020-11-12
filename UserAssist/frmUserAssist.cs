using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace UserAssist
{
    public partial class frmUserAssist : Form
    {
        private UserAssistEntries UAE;
        private ColumnSorter cs = new ColumnSorter();
        private string highlightRegEx = "";
        private clsConfig Config = new clsConfig(true);

        public frmUserAssist()
        {
            InitializeComponent();
            UAE = new UserAssistEntries();
        }

        private void frmUserAssist_Load(object sender, EventArgs e)
        {
            Text = Banner();
            loadAtStartupToolStripMenuItem.Checked = Config.loadAtStartup;
            if (Config.loadAtStartup)
            {
                UAE.GetFromRegistry();
                PopulateListView(UAE.entries);
            }
            loggingToolStripMenuItem.Checked = UAE.GetLogging();
        }

        private void FormatListView()
        {
            listView1.Columns.Clear();
            listView1.Columns.Add("键");
            listView1.Columns[listView1.Columns.Count - 1].Name = "Key";
            listView1.Columns.Add("索引", 50, HorizontalAlignment.Right);
            listView1.Columns[listView1.Columns.Count - 1].Name = "Index";
            listView1.Columns.Add("名字", 400);
            listView1.Columns[listView1.Columns.Count - 1].Name = "Name";
            if (UAE.binaryDataFormat == UserAssistEntries.BinaryDataFormat.windows2000ThruVista)
            {
                listView1.Columns.Add("Unknown", 100, HorizontalAlignment.Right);
                listView1.Columns[listView1.Columns.Count - 1].Name = "Unknown";
                listView1.Columns.Add("Session", 100, HorizontalAlignment.Right);
                listView1.Columns[listView1.Columns.Count - 1].Name = "Session";
            }
            listView1.Columns.Add("计数", 100, HorizontalAlignment.Right);
            listView1.Columns[listView1.Columns.Count - 1].Name = "Counter";
            listView1.Columns.Add("最后访问时间", 180, HorizontalAlignment.Right);
            listView1.Columns[listView1.Columns.Count - 1].Name = "Last";
            listView1.Columns.Add("Last UTC", 10, HorizontalAlignment.Right);
            listView1.Columns[listView1.Columns.Count - 1].Name = "Last UTC";
            if (UAE.binaryDataFormat == UserAssistEntries.BinaryDataFormat.windows7)
            {
                listView1.Columns.Add("Focus counter?", 100, HorizontalAlignment.Right);
                listView1.Columns[listView1.Columns.Count - 1].Name = "Focus counter?";
                listView1.Columns.Add("Focus time?", 100, HorizontalAlignment.Right);
                listView1.Columns[listView1.Columns.Count - 1].Name = "Focus time?";
                listView1.Columns.Add("Flags?", 10, HorizontalAlignment.Right);
                listView1.Columns[listView1.Columns.Count - 1].Name = "Flags?";
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserAssistKey uak;
            foreach (ListViewItem lvi in listView1.SelectedItems)
            {
                UserAssistEntry uae = (UserAssistEntry)lvi.Tag;
                uak.key = lvi.SubItems[0].Text;
                uak.index = int.Parse(lvi.SubItems[1].Text);
                UAE.ClearValue(uak.key, uae.name);
                UAE.entries.Remove(uak);
                lvi.Remove();
            }
        }

        private void abotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout fa = new frmAbout();
            fa.ShowDialog();
            fa.Dispose();
        }

        private string Banner()
        {
            return UserAssist.Name + " " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void PopulateListView(Dictionary<UserAssistKey, UserAssistEntry> entries)
        {
            listView1.Items.Clear();
            FormatListView();
            foreach (UserAssistKey key in entries.Keys)
            {
                ListViewItem LVI = listView1.Items.Add(key.key);
                LVI.Tag = entries[key];
                LVI.SubItems.Add(key.index.ToString());
                LVI.SubItems.Add(entries[key].ReadableName);
                if (UAE.binaryDataFormat == UserAssistEntries.BinaryDataFormat.windows2000ThruVista)
                {
                    if (entries[key].unknown.HasValue)
                        LVI.SubItems.Add(entries[key].unknown.Value.ToString());
                    else
                        LVI.SubItems.Add("");
                    if (entries[key].session.HasValue)
                        LVI.SubItems.Add(entries[key].session.Value.ToString());
                    else
                        LVI.SubItems.Add("");
                }
                if (entries[key].count.HasValue)
                    LVI.SubItems.Add(FormatCount(entries[key].count.Value));
                else
                    LVI.SubItems.Add("");
                if (entries[key].last.HasValue)
                    LVI.SubItems.Add(entries[key].last.Value.ToString());
                else
                    LVI.SubItems.Add("");
                if (entries[key].lastutc.HasValue)
                    LVI.SubItems.Add(entries[key].lastutc.Value.ToString());
                else
                    LVI.SubItems.Add("");
                if (UAE.binaryDataFormat == UserAssistEntries.BinaryDataFormat.windows7)
                {
                    if (entries[key].countAll.HasValue)
                        LVI.SubItems.Add(entries[key].countAll.Value.ToString());
                    else
                        LVI.SubItems.Add("");
                    if (entries[key].totalRunningTime.HasValue)
                        LVI.SubItems.Add(entries[key].totalRunningTime.Value.ToString());
                    else
                        LVI.SubItems.Add("");
                    if (entries[key].flags.HasValue)
                        LVI.SubItems.Add(entries[key].flags.Value.ToString());
                    else
                        LVI.SubItems.Add("");
                }
            }
            highlightListView();
        }

        private string FormatCount(int count)
        {
            if (count == -5)
                return "从列表中移除";
            else
                return count.ToString();
        }
        private void loadFromRegistryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UAE.GetFromRegistry();
            PopulateListView(UAE.entries);
            loggingToolStripMenuItem.Checked = UAE.GetLogging();
            clearAllToolStripMenuItem.Enabled = true;
            loggingToolStripMenuItem.Enabled = true;
            clearToolStripMenuItem.Enabled = true;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "txt files (*.txt)|*.txt|html files (*.html)|*.html|All files (*.*)|*.*";
            sfd.FileName = "UserAssist.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                SaveFile sf;

                if (sfd.FilterIndex == 2)
                    sf = new SaveHTML();
                else
                    sf = new SaveCSV();

                sf.Begin();
                string[] data = new string[listView1.Columns.Count];
                for (int iter = 0; iter < listView1.Columns.Count; iter++)
                    data[iter] = listView1.Columns[iter].Text;
                sf.Header(data);
                foreach (ListViewItem iterItem in listView1.Items)
                {
                    data[0] = iterItem.Text;
                    for (int iter = 1; iter < iterItem.SubItems.Count; iter++)
                        data[iter] = iterItem.SubItems[iter].Text;
                    sf.AddRow(data);
                }
                sf.End();
                try
                {
                    File.WriteAllText(sfd.FileName, sf.Content());

                }
                catch (Exception error)
                {
                    MessageBox.Show(String.Format("写入文件错误 {0}\n信息: {1}", sfd.FileName, error.Message), "错误");
                }
            }
            sfd.Dispose();
        }

        private void loadFromREGFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "注册表文件 (*.reg)|*.reg|All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    UAE.GetFromStrings(File.ReadAllLines(ofd.FileName));
                }
                catch (Exception error)
                {
                    MessageBox.Show(String.Format("读取文件错误 {0}\n信息: {1}", ofd.FileName, error.Message), "错误");
                }
                PopulateListView(UAE.entries);
                clearAllToolStripMenuItem.Enabled = false;
                loggingToolStripMenuItem.Enabled = false;
                clearToolStripMenuItem.Enabled = false;
                if (UAE.entries.Count == 0)
                    MessageBox.Show("文件不包含 UserAssist 数据", "警告");

            }
            ofd.Dispose();
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            listView1.ListViewItemSorter = (System.Collections.IComparer)cs;
            if (listView1.Columns[e.Column].Tag == null)
                listView1.Columns[e.Column].Tag = SortOrder.None;
            listView1.Columns[e.Column].Tag = (SortOrder)listView1.Columns[e.Column].Tag == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            cs.CurrentColumn = e.Column;
            cs.order = (SortOrder)listView1.Columns[e.Column].Tag;
            listView1.Sort();
            listView1.ListViewItemSorter = null;
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("您确定要删除所有条目吗？", "警告", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                UAE.Clear();
                PopulateListView(UAE.entries);
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(listView1, e.Location);
            }
        }

        private void loggingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MyLibrary.IsWindowsXP() || MyLibrary.IsWindows2003())
            {
                ToggleLogging();
                MessageBox.Show("在此版本的Windows上，必须重新启动Windows资源管理器（explorer.exe）才能使设置生效。", "UserAssist日志记录", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (MyLibrary.IsWindowsVista())
                if (MessageBox.Show("在此版本的Windows上，建议通过“开始菜单”属性对话框（“存储并显示最近打开的程序列表”复选框）更改此设置。\n使用属性对话框将：\n 1）进行设置 立即生效（无需重新启动Windows资源管理器）\n 2）取消选中复选框时，删除所有UserAssist条目\n \n是否仍然要通过此工具代替“开始菜单属性”对话框来更改设置？", "UserAssist日志记录", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    ToggleLogging();
                }

        }

        private void ToggleLogging()
        {
            if (UAE.GetLogging())
                UAE.ConfigureLogging(false);
            else
                UAE.ConfigureLogging(true);
            loggingToolStripMenuItem.Checked = UAE.GetLogging();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void highlightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmHighlight fh = new frmHighlight();
            fh.highlight = highlightRegEx;
            fh.ShowDialog();
            highlightRegEx = fh.highlight;
            fh.Dispose();
            highlightListView();
        }

        private void highlightListView()
        {
            if (highlightRegEx == "")
            {
                foreach (ListViewItem iterItem in listView1.Items)
                    iterItem.BackColor = Color.White;
                return;
            }

            Regex re = new Regex(highlightRegEx, RegexOptions.IgnoreCase);

            foreach (ListViewItem iterItem in listView1.Items)
            {
                if (re.Match(iterItem.SubItems[2].Text).Success)
                    iterItem.BackColor = Color.Red;
                else
                    iterItem.BackColor = Color.White;
            }
        }

        private void loadFromDATFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "注册表配置单元文件 (*.dat)|*.dat|All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    UAE.GetFromLoadedHive(ofd.FileName);
                }
                catch (Exception error)
                {
                    MessageBox.Show(String.Format("Error reading file {0}\nreason: {1}", ofd.FileName, error.Message), "Error");
                }
                PopulateListView(UAE.entries);
                clearAllToolStripMenuItem.Enabled = false;
                loggingToolStripMenuItem.Enabled = false;
                clearToolStripMenuItem.Enabled = false;
                if (UAE.entries.Count == 0)
                    MessageBox.Show("文件不包含 UserAssist 数据", "警告");

            }
            ofd.Dispose();
        }

        private void explainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in listView1.SelectedItems)
            {
                UserAssistEntry uae = (UserAssistEntry)lvi.Tag;
                frmExplain fe = new frmExplain();
                fe.title = uae.ReadableName;
                fe.explanation = uae.Explain();
                fe.ShowDialog();
                fe.Dispose();
            }
        }

        private void loadAtStartupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Config.loadAtStartup = !Config.loadAtStartup;
            loadAtStartupToolStripMenuItem.Checked = Config.loadAtStartup;
        }
    }

    public class ColumnSorter : System.Collections.IComparer
    {
        public int CurrentColumn = 0;
        public SortOrder order = SortOrder.Ascending;
        public int Compare(object x, object y)
        {
            ListViewItem lviA = (ListViewItem)x;
            ListViewItem lviB = (ListViewItem)y;
            UserAssistEntry uaeA = (UserAssistEntry)lviA.Tag;
            UserAssistEntry uaeB = (UserAssistEntry)lviB.Tag;
            int result = 0;
            switch (lviA.ListView.Columns[CurrentColumn].Name)
            {
                case "Key":
                case "Name":
                case "Unknown":
                    result = String.Compare(lviA.SubItems[CurrentColumn].Text, lviB.SubItems[CurrentColumn].Text);
                    break;

                case "Session":
                    if (!uaeA.session.HasValue && !uaeB.session.HasValue)
                        result = 0;
                    else if (!uaeA.session.HasValue && uaeB.session.HasValue)
                        result = -1;
                    else if (uaeA.session.HasValue && !uaeB.session.HasValue)
                        result = 1;
                    else
                        result = uaeA.session.Value.CompareTo(uaeB.session.Value);
                    break;

                case "Counter":
                    if (!uaeA.count.HasValue && !uaeB.count.HasValue)
                        result = 0;
                    else if (!uaeA.count.HasValue && uaeB.count.HasValue)
                        result = -1;
                    else if (uaeA.count.HasValue && !uaeB.count.HasValue)
                        result = 1;
                    else
                        result = uaeA.count.Value.CompareTo(uaeB.count.Value);
                    break;

                case "Last":
                    if (!uaeA.last.HasValue && !uaeB.last.HasValue)
                        result = 0;
                    else if (!uaeA.last.HasValue && uaeB.last.HasValue)
                        result = -1;
                    else if (uaeA.last.HasValue && !uaeB.last.HasValue)
                        result = 1;
                    else
                        result = uaeA.last.Value.CompareTo(uaeB.last.Value);
                    break;

                case "Last UTC":
                    if (!uaeA.lastutc.HasValue && !uaeB.lastutc.HasValue)
                        result = 0;
                    else if (!uaeA.lastutc.HasValue && uaeB.lastutc.HasValue)
                        result = -1;
                    else if (uaeA.lastutc.HasValue && !uaeB.lastutc.HasValue)
                        result = 1;
                    else
                        result = uaeA.lastutc.Value.CompareTo(uaeB.lastutc.Value);
                    break;

                case "Focus counter?":
                    if (!uaeA.countAll.HasValue && !uaeB.countAll.HasValue)
                        result = 0;
                    else if (!uaeA.countAll.HasValue && uaeB.countAll.HasValue)
                        result = -1;
                    else if (uaeA.countAll.HasValue && !uaeB.countAll.HasValue)
                        result = 1;
                    else
                        result = uaeA.countAll.Value.CompareTo(uaeB.countAll.Value);
                    break;

                case "Focus time?":
                    if (!uaeA.totalRunningTime.HasValue && !uaeB.totalRunningTime.HasValue)
                        result = 0;
                    else if (!uaeA.totalRunningTime.HasValue && uaeB.totalRunningTime.HasValue)
                        result = -1;
                    else if (uaeA.totalRunningTime.HasValue && !uaeB.totalRunningTime.HasValue)
                        result = 1;
                    else
                        result = uaeA.totalRunningTime.Value.CompareTo(uaeB.totalRunningTime.Value);
                    break;

                case "Flags?":
                    if (!uaeA.flags.HasValue && !uaeB.flags.HasValue)
                        result = 0;
                    else if (!uaeA.flags.HasValue && uaeB.flags.HasValue)
                        result = -1;
                    else if (uaeA.flags.HasValue && !uaeB.flags.HasValue)
                        result = 1;
                    else
                        result = uaeA.flags.Value.CompareTo(uaeB.flags.Value);
                    break;

                default:
                    result = int.Parse(lviA.SubItems[CurrentColumn].Text).CompareTo(int.Parse(lviB.SubItems[CurrentColumn].Text));
                    break;
            }
            return result * (order == SortOrder.Ascending ? 1 : -1);
        }

        public ColumnSorter()
        {

        }

    }

    abstract public class SaveFile
    {
        abstract public void Begin();
        abstract public void Header(string[] headers);
        abstract public void AddRow(string[] data);
        abstract public void End();
        abstract public string Content();
    }

    public class SaveCSV : SaveFile
    {
        private StringBuilder sb = new StringBuilder();

        override public void Begin()
        {
        }

        override public void Header(string[] headers)
        {
            Add(headers);
        }

        override public void AddRow(string[] data)
        {
            Add(data);
        }

        override public void End()
        {
        }

        override public string Content()
        {
            return sb.ToString();
        }

        private void Add(string[] data)
        {
            string separator = "";

            foreach (string item in data)
            {
                sb.AppendFormat("{0}\"{1}\"", separator, item);
                separator = ",";
            }
            sb.AppendLine();
        }
    }

    public class SaveHTML : SaveFile
    {
        private StringBuilder sb = new StringBuilder();

        override public void Begin()
        {
            sb.AppendLine("<html><body><table border=\"1\">");
        }

        override public void Header(string[] headers)
        {
            sb.Append("<tr>");
            foreach (string header in headers)
            {
                sb.AppendFormat("<th>{0}</th>", header);
            }
            sb.AppendLine("</tr>");
        }

        override public void AddRow(string[] data)
        {
            sb.Append("<tr>");
            foreach (string item in data)
            {
                sb.AppendFormat("<td>{0}</td>", item);
            }
            sb.AppendLine("</tr>");
        }

        override public void End()
        {
            sb.AppendLine("</table></body></html>");
        }

        override public string Content()
        {
            return sb.ToString();
        }
    }
}