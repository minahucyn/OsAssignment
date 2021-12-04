using OsAssgnment.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
namespace OsAssignment.Fcfs
{
    public class Program
    {

        public static List<ExportStatistics> InitilizeAndRunFcfs(int noOfRuns = 10)
        {
            int i = 0;
            int numberOfRuns = noOfRuns;
            var allProcessBatches = new List<Process>();

            while (i <= numberOfRuns)
            {
                var processBatch = RunFcfs();
                allProcessBatches.AddRange(processBatch);
                i++;
            }

            //calculate average wait time and turn around time for each process
            return Functions.CalculateStatistics(allProcessBatches);
        }

        private static List<Process> RunFcfs()
        {
            //new-up a array[100] of Process class
            var processes = Functions.GenerateInitialProcessSetup();

            //Order the processes by arrival time
            processes = SortProcessesByArrivalTimeAsc(processes);

            //var array = CalculateWaitTime(newed up Array[100]); // wait property will be filled.
            processes = CalculateWaitTime(processes);
            
            foreach (var item in processes)
            {
                Console.WriteLine($"Pid: {item.ProcessId} Burst time: {item.BurstTime}. arrival time: {item.ArrivalTime} Wait Time: {item.WaitTime} TAT: {item.TurnAroundTime}");
            }

            return processes;
        }

        private static List<Process> CalculateWaitTime(List<Process> processes)
        {
            var waitTime = 0;
            foreach (var process in processes)
            {
                process.WaitTime = waitTime;
                waitTime += process.BurstTime;
            }
            return processes;
        }

        static List<Process> SortProcessesByArrivalTimeAsc(List<Process> processes)
        {
            var sortedProcesses = from p in processes
                                  orderby p.ArrivalTime
                                  select p;
            return sortedProcesses.ToList();

        }
    }

}

