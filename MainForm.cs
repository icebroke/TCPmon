using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCPmon
{
    public partial class MainForm : Form
    {
        private int rows;
        private int int_pid;
        private string PID;
        private string processName;

        private ProcProperties properties_form;
        private About about_form;
        private ProcTree proc_tree;
        string log_path = @".\\log.txt";
        string list_path = @".\\procList.txt";
        public MainForm()
        {
            InitializeComponent();
            run_cmd();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            run_cmd();
        }

        public void run_cmd()
        {
            try
            {
                Process p = new Process();

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.Arguments = "-a -n -o";
                psi.FileName = "netstat.exe";
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;

                p.StartInfo = psi;
                p.Start();

                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                p.Close();

                if (!File.Exists(list_path))
                {
                    File.WriteAllText(list_path, output);
                }
                else
                {
                    File.WriteAllText(list_path, output);
                }
                
                readProcList();
                tcp_log();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public void readProcList()
        {
            string line = null;
            string[] values = null;
            processName = null;
            rows = 0;

            StreamReader sr = new StreamReader(list_path);
            while ((line = sr.ReadLine()) != null)
            {
                values = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                if (values.Length == 5)
                {
                    if (values[4] != "0")
                        dataGridView1.Rows.Add(values);
                }
            }
            sr.Close();

            rows = dataGridView1.Rows.Count;

            for (int i = 0; i < rows; i++)
            {
                try
                {
                    PID = dataGridView1.Rows[i].Cells[4].Value.ToString();
                    int.TryParse(PID, out int_pid); //Parse string to int
                    processName = Process.GetProcessById(int_pid).ProcessName; // Get the process name from pid
                    if (dataGridView1.Rows[i].Cells[3].Value.ToString() == "ESTABLISHED") 
                    { 
                        dataGridView1.Rows[i].Cells[3].Style.BackColor = Color.Green; 
                    }
                    dataGridView1.Rows[i].Cells[5].Value = processName;
                }
                catch (Exception e)
                {
                    dataGridView1.Rows.RemoveAt(i);
                    rows = rows - 1;
                }
                
            }

            lCountTCP.Text = rows + " active connections";
        }

        public void tcp_log()
        {
            // Create log.txt file at execution app folder
            try
            {
                dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithAutoHeaderText;
                dataGridView1.SelectAll();
                Clipboard.SetDataObject(dataGridView1.GetClipboardContent());

                if (!File.Exists(log_path)) // Create file if not exist
                {
                    File.WriteAllText(log_path, DateTime.Now.ToString() + Environment.NewLine + Clipboard.GetText(TextDataFormat.Text) + Environment.NewLine);
                }

                // TODO: Clear log content after 30 days

                File.AppendAllText(log_path, Environment.NewLine + DateTime.Now.ToString() + Environment.NewLine + Clipboard.GetText(TextDataFormat.Text) + Environment.NewLine);

                dataGridView1.ClearSelection();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void aboutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            about_form = new About();
            about_form.Show();
        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // This function need to run as administrator for some processes
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                try
                {
                    PID = row.Cells[4].Value.ToString();
                    int.TryParse(PID, out int_pid);

                    properties_form = new ProcProperties();
                    properties_form.process_properties(int_pid);
                    properties_form.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void logToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Process.Start("notepad.exe", log_path); // Open text file with windows default text editor
        }

        private void procTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            proc_tree = new ProcTree();
            proc_tree.ShowDialog();
        }
    }
}
