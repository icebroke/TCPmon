using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace TCPmon
{
    class ProcInfo : IComparable<ProcInfo>
    {
        public Process theProcess;
        public ProcInfo Parent;
        public List<ProcInfo> Children = new List<ProcInfo>();

        public ProcInfo(Process the_process)
        {
            theProcess = the_process;
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", theProcess.ProcessName, theProcess.Id);
        }

        public int CompareTo(ProcInfo other)
        {
            return theProcess.ProcessName.CompareTo(other.theProcess.ProcessName);
        }
    }
}
