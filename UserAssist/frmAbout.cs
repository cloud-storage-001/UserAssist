using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserAssist
{
    public partial class frmAbout : Form
    {
        public frmAbout()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
            textBox1.Lines = new string[] 
            {
                @"程序UserAssist显示用户在Windows上运行的程序的列表.",
                @"",
                @"Windows资源管理器在标准XP“开始”菜单的左侧显示常用程序. ",
                @"有关经常使用的程序的数据保存在注册表中的此键下:",
                @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\UserAssist",
                @"",
                @"该程序解密并显示在UserAssist项下的注册表中找到的数据.",
                @"",
                @"启动后，程序将检索当前用户的数据并显示它. ",
                @"Windows资源管理器更新注册表项时，显示不会自动刷新. ",
                @"要刷新显示，请执行“从本地注册表加载”命令.",
                @"",
                @"列表视图中的列:",
                @"Key: ",
                @"它的值是 {0D6D4F41-2994-4BA0-8FEF-620E43CD2812}, {5E6AB780-7743-11CF-A12B-00AA004AE837} or {75048700-EF1F-11D0-9888-006097DEACF9}",
                @"这些是在UserAssist键下找到的键，并包含在列表视图中以区分条目.",
                @"",
                @"Index:",
                @"一个运行中的计数器，指示注册表中值的顺序",
                @"首先，条目按它们在注册表中出现的顺序列出。您可以通过单击标题对列进行排序.",
                @"要还原为原始顺序，请对“Index”列和“Key”列进行排序",
                @"",
                @"Name:",
                @"值注册表项的名称。这引用了所运行的程序。该密钥经过ROT13加密，显示的名称已解密.",
                @"有一个注册表设置可以防止日志加密，但是此程序不支持此设置.",
                @"",
                @"Unknown:",
                @"一个4字节整数，表示未知。它似乎仅对会话条目存在（UEME_CTLSESSION）.",
                @"",
                @"Session:",
                @"这是会话的ID（4字节整数）.",
                @"",
                @"Counter:",
                @"这是程序运行的次数（4字节整数）.",
                @"",
                @"Last:",
                @"这是程序最后一次运行（日期时间为8字节）. ",
                @"该值与运行此UserAssist工具的计算机的时区一起显示。",
                @"从具有不同区域设置的系统导入REG文件时，请注意时区差异.",
                @"",
                @"Last UTC:",
                @"这是该程序最后一次在UTC中运行（日期时间为8字节）. ",
                @"",
                @"命令:",
                @"",
                @"“从本地注册表加载”",
                @"显示当前用户的数据.",
                @"",
                @"“从REG文件加载”",
                @"加载REG文件并导入UserAssist密钥. ",
                @"此命令不检查UserAssist密钥的完整路径，因此允许分析NTUSER.DAT配置单元，并使用其他路径导出.",
                @"如果无法在要分析的计算机上运行程序，请使用此命令.",
                @"从REG文件加载数据会禁用编辑命令.",
                @"",
                @"“从DAT文件加载”",
                @"加载注册表配置单元文件（如NTUSER.DAT的DAT文件）并导入UserAssist项. ",
                @"DAT文件被临时加载到USERS \ LoadedHive项下的注册表中。确保具有本地管理员权限才能访问文件并加载它.",
                @"如果无法在要分析的计算机上运行程序，请使用此命令.",
                @"从DAT文件加载数据会禁用编辑命令.",
                @"",
                @"“高亮”",
                @"允许您输入搜索字符串（接受正则表达式），与该字符串匹配的每个条目将以红色突出显示.",
                @"在重新加载期间，突出显示保持活动状态。键入一个空字符串以禁用突出显示.",
                @"",
                @"“保存”",
                @"这会将数据另存为CSV文件或HTML文件（选择文件类型）.",
                @"",
                @"“全部清除”",
                @"这将删除 {5E6AB780-7743-11CF-A12B-00AA004AE837} 和 {75048700-EF1F-11D0-9888-006097DEACF9} 键.",
                @"重新启动Windows资源管理器之前，所有数据都将丢失，并且不会记录新数据.",
                @"这将影响“开始”菜单上经常运行的程序列表，可能还会影响其他内容。我的测试机没有其他副作用.",
                @"加载REG文件时禁用此命令.",
                @"",
                @"“禁用日志记录”",
                @"启用“禁用日志记录”开关，可以通过创建一个值来永久禁用UserAssist键中的用户活动记录",
                @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\UserAssist\Settings\NoLog equal to 1.",
                @"禁用“禁用日志记录”会删除NoLog值（显然，将其设置为0不会阻止日志记录）.",
                @"此设置仅在重新启动Windows资源管理器后才有效.",
                @"加载REG文件时禁用此命令.",
                @"",
                @"“启动时加载”",
                @"启用“在启动时加载”切换指示UserAssist在启动时从本地注册表加载密钥.",
                @"",
                @"右键单击一个条目将显示一个菜单:",
                @"",
                @"“清除”将删除所选的条目。其余条目的索引字段未更改，仅在重新加载注册表后才更改.",
                @"加载REG文件时禁用此命令.",
                @"",
                @"“说明”将分析名称字段的内容，并尝试解释其含义（基于经验数据）.",
                @"",
                @"该程序已经在Windows XP SP1，SP2，Windows 2003和Windows Vista上进行了测试. ",
                @"Microsoft不会发布UserAssist数据的官方文档。我在WWW（Google for UserAssist）上找到了信息，并且通过反复试验测试发现了二进制数据的含义.",
                @"换句话说：使用此程序需要您自担风险.",
                @"",
                @"重新启动Windows资源管理器的方法:",
                @"1) 任务管理器：终止explorer.exe进程并启动新任务explorer.exe",
                @"2) 注销/登录",
                @"3) 重启"
            };
            linkLabel1.Text = UserAssist.URL;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkLabel1.Text);
        }
    }
}