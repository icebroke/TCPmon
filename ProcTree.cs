using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace TCPmon
{
    public partial class ProcTree : Form
    {
        private int numProcesses, numThreads;
        public ProcTree()
        {
            InitializeComponent();
        }

        private void ProcTree_Load(object sender, EventArgs e)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Dictionary<int, ProcInfo> process_dict = new Dictionary<int, ProcInfo>();

            // Get the processes
            foreach (Process process in Process.GetProcesses())
            {
                process_dict.Add(process.Id, new ProcInfo(process));
            }

            // Get the parent/child info
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT ProcessId, ParentProcessId FROM Win32_Process");
            ManagementObjectCollection collection = searcher.Get();

            // Create the child lists
            foreach (var item in collection)
            {
                // Find the parent and child in the dictionary
                int child_id = Convert.ToInt32(item["ProcessId"]);
                int parent_id = Convert.ToInt32(item["ParentProcessId"]);

                ProcInfo child_info = null;
                ProcInfo parent_info = null;

                if (process_dict.ContainsKey(child_id))
                {
                    child_info = process_dict[child_id];
                }
                if (process_dict.ContainsKey(parent_id))
                {
                    parent_info = process_dict[parent_id];
                }

                if (child_info == null)
                {
                    Console.WriteLine("Cannot find child " + child_id.ToString() + " for parent " + parent_id.ToString());
                }
                if (parent_info == null)
                {
                    Console.WriteLine("Cannot find parent " + parent_id.ToString() + " for child " + child_id.ToString());
                }

                if ((child_info != null) && (parent_info != null))
                {
                    parent_info.Children.Add(child_info);
                    child_info.Parent = parent_info;
                }
            }

            // Convert the dictionary into a list
            List<ProcInfo> info_list = process_dict.Values.ToList();
            info_list.Sort();

            // Populate the TreeView
            numProcesses = 0;
            numThreads = 0;

            foreach (ProcInfo info in info_list)
            {
                if (info.Parent != null) continue;

                AddInfoToTree(treeView.Nodes, info);
            }

            lblCounts.Text = "Processes: " + numProcesses.ToString() + ", "
                + "Threads: " + numThreads.ToString();

            watch.Stop();
            Console.WriteLine(string.Format("{0:0.00} seconds", watch.Elapsed.TotalSeconds));
        }

        private void AddInfoToTree(TreeNodeCollection nodes, ProcInfo info)
        {
            // Add the proces's node
            TreeNode proc_node = nodes.Add(info.ToString());
            proc_node.Tag = info;
            numProcesses++;

            // Add the node's threads
            if (info.theProcess.Threads.Count > 0)
            {
                TreeNode thread_node = proc_node.Nodes.Add("Threads");
                
                foreach (ProcessThread thread in info.theProcess.Threads)
                {
                    thread_node.Nodes.Add(string.Format("Thread {0}", thread.Id));
                    numThreads++;
                }
            }

            // Sort the children
            info.Children.Sort();

            foreach (ProcInfo child_info in info.Children)
            {
                AddInfoToTree(proc_node.Nodes, child_info);
            }

            if (info.Children.Count > 0)
            {
                proc_node.Expand();
            }
        }
    }
}
