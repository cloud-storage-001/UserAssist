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
                @"����UserAssist��ʾ�û���Windows�����еĳ�����б�.",
                @"",
                @"Windows��Դ�������ڱ�׼XP����ʼ���˵��������ʾ���ó���. ",
                @"�йؾ���ʹ�õĳ�������ݱ�����ע����еĴ˼���:",
                @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\UserAssist",
                @"",
                @"�ó�����ܲ���ʾ��UserAssist���µ�ע������ҵ�������.",
                @"",
                @"�����󣬳��򽫼�����ǰ�û������ݲ���ʾ��. ",
                @"Windows��Դ����������ע�����ʱ����ʾ�����Զ�ˢ��. ",
                @"Ҫˢ����ʾ����ִ�С��ӱ���ע������ء�����.",
                @"",
                @"�б���ͼ�е���:",
                @"Key: ",
                @"����ֵ�� {0D6D4F41-2994-4BA0-8FEF-620E43CD2812}, {5E6AB780-7743-11CF-A12B-00AA004AE837} or {75048700-EF1F-11D0-9888-006097DEACF9}",
                @"��Щ����UserAssist�����ҵ��ļ������������б���ͼ����������Ŀ.",
                @"",
                @"Index:",
                @"һ�������еļ�������ָʾע�����ֵ��˳��",
                @"���ȣ���Ŀ��������ע����г��ֵ�˳���г���������ͨ������������н�������.",
                @"Ҫ��ԭΪԭʼ˳����ԡ�Index���к͡�Key���н�������",
                @"",
                @"Name:",
                @"ֵע���������ơ��������������еĳ��򡣸���Կ����ROT13���ܣ���ʾ�������ѽ���.",
                @"��һ��ע������ÿ��Է�ֹ��־���ܣ����Ǵ˳���֧�ִ�����.",
                @"",
                @"Unknown:",
                @"һ��4�ֽ���������ʾδ֪�����ƺ����ԻỰ��Ŀ���ڣ�UEME_CTLSESSION��.",
                @"",
                @"Session:",
                @"���ǻỰ��ID��4�ֽ�������.",
                @"",
                @"Counter:",
                @"���ǳ������еĴ�����4�ֽ�������.",
                @"",
                @"Last:",
                @"���ǳ������һ�����У�����ʱ��Ϊ8�ֽڣ�. ",
                @"��ֵ�����д�UserAssist���ߵļ������ʱ��һ����ʾ��",
                @"�Ӿ��в�ͬ�������õ�ϵͳ����REG�ļ�ʱ����ע��ʱ������.",
                @"",
                @"Last UTC:",
                @"���Ǹó������һ����UTC�����У�����ʱ��Ϊ8�ֽڣ�. ",
                @"",
                @"����:",
                @"",
                @"���ӱ���ע������ء�",
                @"��ʾ��ǰ�û�������.",
                @"",
                @"����REG�ļ����ء�",
                @"����REG�ļ�������UserAssist��Կ. ",
                @"��������UserAssist��Կ������·���������������NTUSER.DAT���õ�Ԫ����ʹ������·������.",
                @"����޷���Ҫ�����ļ���������г�����ʹ�ô�����.",
                @"��REG�ļ��������ݻ���ñ༭����.",
                @"",
                @"����DAT�ļ����ء�",
                @"����ע������õ�Ԫ�ļ�����NTUSER.DAT��DAT�ļ���������UserAssist��. ",
                @"DAT�ļ�����ʱ���ص�USERS \ LoadedHive���µ�ע����С�ȷ�����б��ع���ԱȨ�޲��ܷ����ļ���������.",
                @"����޷���Ҫ�����ļ���������г�����ʹ�ô�����.",
                @"��DAT�ļ��������ݻ���ñ༭����.",
                @"",
                @"��������",
                @"���������������ַ����������������ʽ��������ַ���ƥ���ÿ����Ŀ���Ժ�ɫͻ����ʾ.",
                @"�����¼����ڼ䣬ͻ����ʾ���ֻ״̬������һ�����ַ����Խ���ͻ����ʾ.",
                @"",
                @"�����桱",
                @"��Ὣ��������ΪCSV�ļ���HTML�ļ���ѡ���ļ����ͣ�.",
                @"",
                @"��ȫ�������",
                @"�⽫ɾ�� {5E6AB780-7743-11CF-A12B-00AA004AE837} �� {75048700-EF1F-11D0-9888-006097DEACF9} ��.",
                @"��������Windows��Դ������֮ǰ���������ݶ�����ʧ�����Ҳ����¼������.",
                @"�⽫Ӱ�조��ʼ���˵��Ͼ������еĳ����б������ܻ���Ӱ���������ݡ��ҵĲ��Ի�û������������.",
                @"����REG�ļ�ʱ���ô�����.",
                @"",
                @"��������־��¼��",
                @"���á�������־��¼�����أ�����ͨ������һ��ֵ�����ý���UserAssist���е��û����¼",
                @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\UserAssist\Settings\NoLog equal to 1.",
                @"���á�������־��¼����ɾ��NoLogֵ����Ȼ����������Ϊ0������ֹ��־��¼��.",
                @"�����ý�����������Windows��Դ�����������Ч.",
                @"����REG�ļ�ʱ���ô�����.",
                @"",
                @"������ʱ���ء�",
                @"���á�������ʱ���ء��л�ָʾUserAssist������ʱ�ӱ���ע���������Կ.",
                @"",
                @"�Ҽ�����һ����Ŀ����ʾһ���˵�:",
                @"",
                @"���������ɾ����ѡ����Ŀ��������Ŀ�������ֶ�δ���ģ��������¼���ע�����Ÿ���.",
                @"����REG�ļ�ʱ���ô�����.",
                @"",
                @"��˵���������������ֶε����ݣ������Խ����京�壨���ھ������ݣ�.",
                @"",
                @"�ó����Ѿ���Windows XP SP1��SP2��Windows 2003��Windows Vista�Ͻ����˲���. ",
                @"Microsoft���ᷢ��UserAssist���ݵĹٷ��ĵ�������WWW��Google for UserAssist�����ҵ�����Ϣ������ͨ������������Է����˶��������ݵĺ���.",
                @"���仰˵��ʹ�ô˳�����Ҫ���Ե�����.",
                @"",
                @"��������Windows��Դ�������ķ���:",
                @"1) �������������ֹexplorer.exe���̲�����������explorer.exe",
                @"2) ע��/��¼",
                @"3) ����"
            };
            linkLabel1.Text = UserAssist.URL;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkLabel1.Text);
        }
    }
}