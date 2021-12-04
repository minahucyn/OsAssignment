using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsAssgnment.Shared
{
    public static class Functions
    {
        public static List<ExportStatistics> CalculateStatistics(List<Process> allProcessBatches)
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
        public static void WriteCsvOutput(List<ExportStatistics> fcfsStatistics)
        {
            using (var writer = new StreamWriter("D:\\fcfs.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(fcfsStatistics);
            }
        }

        public static List<Process> GenerateInitialProcessSetup()
        {
            //new-up a array[100] of Process class
            var processes = new List<Process>();

            //iterate through and assign random burstTimes and Arrival Times for processes in the 
            //array
            var randomGenerator = new Random();
            int i = 1;
            while (processes.Count <= 100)
            {
                var process = new Process
                {
                    ProcessId = $"P{i}",
                    ArrivalTime = randomGenerator.Next(0,100),
                    BurstTime = randomGenerator.Next(1,100) //burst time must not be less than 1
                };
                i++;
                processes.Add(process);
            }

            return processes;
        }
    }
}
