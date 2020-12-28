﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
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
        public MainForm()
        {
            InitializeComponent();
            fill_dataGrid();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            fill_dataGrid();
        }

        public void fill_dataGrid()
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

                while (!p.StandardOutput.EndOfStream)
                {
                    string[] values = p.StandardOutput.ReadLine().Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                    if (values.Length == 5)
                    {
                        dataGridView1.Rows.Add(values);
                    }
                }

                rows = dataGridView1.Rows.Count;

                progressBar1.Visible = true;
                progressBar1.Minimum = 1;
                progressBar1.Maximum = rows;
                progressBar1.Value = 1;
                progressBar1.Step = 1;

                for (int i = 0; i < rows; i++)
                {
                    if (dataGridView1.Rows[i].Cells[3].Value.ToString() == "ESTABLISHED")
                    {
                        dataGridView1.Rows[i].Cells[3].Style.BackColor = Color.Green;
                    }

                    PID = dataGridView1.Rows[i].Cells[4].Value.ToString();
                    int.TryParse(PID, out int_pid); //Parse string to int
                    processName = Process.GetProcessById(int_pid).ProcessName; // Get the process name from pid

                    dataGridView1.Rows[i].Cells[5].Value = processName;

                    progressBar1.PerformStep();
                }

                lCountTCP.Text = rows + " active connections";
                progressBar1.Visible = false;
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
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
    }
}
