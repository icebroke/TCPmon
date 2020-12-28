using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TCPmon
{
    public partial class ProcProperties : Form
    {
        public ProcProperties()
        {
            InitializeComponent();
        }

        public void process_properties(int int_pid)
        {
            try
            {
                string file_path = Process.GetProcessById(int_pid).MainModule.FileName;
                string file_description = Path.GetFileName(file_path);

                FileInfo fi = new FileInfo(file_path);

                string file_type = fi.Extension;
                long file_size = fi.Length / 1024; // Bytes to Kilo bytes
                DateTime file_created = fi.CreationTime;
                DateTime file_modified = fi.LastWriteTime;
                DateTime file_accessed = fi.LastAccessTime;

                tbType.Text = file_type;
                tbDescription.Text = file_description;
                tbLocation.Text = file_path;
                tbSize.Text = file_size.ToString() + " Kb";
                tbCreated.Text = file_created.ToString();
                tbModified.Text = file_modified.ToString();
                tbAccessed.Text = file_accessed.ToString();

                this.Show();
            }
            catch (IOException e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
