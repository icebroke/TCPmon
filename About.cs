using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TCPmon
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            fill_listview();
        }

        public void fill_listview()
        {
            lVersion.Text = "TCPmon v0.6";

            tbDescription.AppendText("TCPmon\r\n\r\n" +
                "A simple windows application that implement GUI to the netstat." +
                "The application helps to identify the process that is connecting to the Internet.\r\n" +
                "This application is under the terms of the GNU General Public License as published by the Free Software Foundation");

            lCreatedBy.Text = "IceBroke";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
