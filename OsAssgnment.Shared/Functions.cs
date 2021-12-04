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
