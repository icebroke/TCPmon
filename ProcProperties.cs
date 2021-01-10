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
        private int pid;
        private string processName;
        string file_path;
        string file_description;
        public ProcProperties()
        {
            InitializeComponent();
        }

        public void process_properties(int int_pid)
        {
            pid = int_pid;
            processName = Process.GetProcessById(int_pid).ProcessName; // Get the process name from pid
            try
            {
                file_path = Process.GetProcessById(int_pid).MainModule.FileName;
                file_description = Path.GetFileName(file_path);

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

        private void endProcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Kill '" + processName + "' process ?", "End process", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    Process[] proc = Process.GetProcessesByName(processName);
                    proc[0].Kill();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                this.Close();
            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filePath = "";
            if (File.Exists(file_path))
            {
                filePath = Path.GetFullPath(file_path);
                Process.Start(file_description, string.Format("/select,\"{0}\"",filePath));
            }
            else
            {
                MessageBox.Show(string.Format("{0} The file does not exist!", file_path));
            }
        }
    }
}
