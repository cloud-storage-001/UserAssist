using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserAssist
{
    public partial class frmExplain : Form
    {
        public string explanation;
        public string title;

        public frmExplain()
        {
            InitializeComponent();
        }

        private void frmExplain_Load(object sender, EventArgs e)
        {
            textBox1.Lines = explanation.Split('\n');
            Text = title;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(explanation);
            }
#pragma warning disable 0168
            catch (Exception e2)
            {
            }
#pragma warning restore 0168
        }
    }
}