using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsAssgnment.Shared
{
    public class Process
    {
        private int burstTime;
        private int waitTime;

        public string ProcessId { get; set; }
        public int BurstTime
        {
            get => burstTime;
            set
            {
                burstTime = value;
                TurnAroundTime = burstTime + WaitTime;
            }
        }
        public int ArrivalTime { get; set; }
        public int WaitTime
        {
            get => waitTime; set
            {
                waitTime = value;
                TurnAroundTime = waitTime + BurstTime;
            }
        }
        public int TurnAroundTime { get; set; }

    }
}
