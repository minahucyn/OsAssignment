using OsAssgnment.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
namespace OsAssignment.Fcfs
{
    public class Program
    {

        static void Main(string[] args)
        {
            int i = 0;
            int numberOfRuns = 10;
            var allProcessBatches = new List<Process>();

            while (i <= numberOfRuns)
            {
                var processBatch = RunFcfs();
                allProcessBatches.AddRange(processBatch);
                i++;
            }

            //calculate average wait time and turn around time for each process
            var exportStatistics = CalculateFcfsStatistics(allProcessBatches);
            Functions.WriteCsvOutput(exportStatistics);

        }


        private static List<ExportStatistics> CalculateFcfsStatistics(List<Process> allProcessBatches)
        {
            //list of fcfs statitsics to return
            var statistics = new List<ExportStatistics>();
            for (int i = 1; i <= 100; i++)
            {
                string pId = "P" + i;
                // IEnumerable<string> results = myList.Where(s => s == search);
                var matches = allProcessBatches.Where(p => p.ProcessId == pId);
                var averageWaitTime = matches.Select(x => x.WaitTime).Average();
                var averageTat = matches.Select(x => x.TurnAroundTime).Average();

                statistics.Add(new ExportStatistics() { Pid = pId, AverageTat = averageTat, AverageWaitTime = averageWaitTime });
            }
            //iterate from 1 to 100 looking for P1 to P100 respectively
            //find required averagres for each process... avgs of all P1 processes
            // add to return list

            //finally... return

            return statistics;
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

