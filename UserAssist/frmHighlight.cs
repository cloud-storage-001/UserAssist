using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserAssist
{
    public partial class frmHighlight : Form
    {
        public string highlight;

        public frmHighlight()
        {
            InitializeComponent();
        }

        private void frmHighlight_Load(object sender, EventArgs e)
        {
            txtHighlight.Text = highlight;
 //           txtHighlight.SelectAll();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            highlight = txtHighlight.Text;
            this.Close();
        }
    }
}