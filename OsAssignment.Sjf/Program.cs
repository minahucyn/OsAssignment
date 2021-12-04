using OsAssgnment.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OsAssignment.Sjf
{
    class Program
    {

        // 1- Traverse until all process gets completely
        //   executed.
        //   a) Find process with minimum remaining time at
        //     every single time lap.
        //   b) Reduce its time by 1.
        //   c) Check if its remaining time becomes 0 
        //   d) Increment the counter of process completion.
        //   e) Completion time of current process = 
        //     current_time +1;
        //   e) Calculate waiting time for each completed
        //     process.
        //   wt[i]= Completion time - arrival_time-burst_time
        //   f)Increment time lap by one.
        //2- Find turnaround time(waiting_time+burst_time).

        private static List<Process> _allProcesses;
        private static List<Process> _stack;
        private static int _cpuCycleCurrent;
        private static event EventHandler ProcessingCompleted;
        private static bool _processingCompleted;
        private static bool _processingStarted;

        public static List<ExportStatistics> InitializeAndRunSjf(SjfType sjfType, int noOfRuns = 10)
        {

            int i = 0;
            int numberOfRuns = noOfRuns;
            var allProcessBatches = new List<Process>();

            while (i <= numberOfRuns)
            {
                var processBatch = RunSjf();
                allProcessBatches.AddRange(processBatch);
                i++;
            }

            //calculate average wait time and turn around time for each process
            return Functions.CalculateStatistics(allProcessBatches);
            
        }

        private static List<Process> RunSjf()
        {
            //generate a list of processes with random arrival and burst times
            _allProcesses = Functions.GenerateInitialProcessSetup();
            _processingCompleted = false;
            _processingStarted = false;
            _cpuCycleCurrent = 0;
            _stack = new List<Process>();

            ProcessingCompleted += OnProcessingCompleted;

            while (!_processingCompleted)
            {
                OnCpuCycle();
                //Thread.Sleep(500);
            }

            return _allProcesses;
        }

        private static void OnProcessingCompleted(object sender, EventArgs e)
        {
            _processingCompleted = true;
        }

        private static void OnCpuCycle()
        {
            //RunCpuCyle(SjfType.NonPreemptive);
            RunCpuCyle(SjfType.NonPreemptive);

        }

        private static void RunCpuCyle(SjfType sjftype)
        {
            Console.WriteLine("cpu cycle: " + _cpuCycleCurrent);
            //check all process list ===> condition: arrival time == current cpu cycle
            //add these to stack

            foreach (var process in _allProcesses)
            {
                if (process.ArrivalTime == _cpuCycleCurrent)
                {
                    if (_processingStarted == false) { _processingStarted = true; }
                    _stack.Add(process);
                }
            }
            Console.WriteLine("number of processes in stack: " + _stack.Count);
            //check stack, if stack.count == 0 and processing is started already. invoke ProcessingCompleted event
            if (_stack.Count == 0 && _processingStarted)
            {
                ProcessingCompleted?.Invoke(new object(), EventArgs.Empty);
                return;
            }

            if (_stack.Count == 0)
            {
                Console.WriteLine("No processes to process!");
                _cpuCycleCurrent++;
                return;
            }

            //continue with processing
            switch (sjftype)
            {
                case SjfType.Preemptive:
                    ProcessPreemptively();
                    break;
                case SjfType.NonPreemptive:
                    ProcessNonPreemptively();
                    break;
                default:
                    break;
            }

            _cpuCycleCurrent++;

        }

        private static void ProcessNonPreemptively()
        {
            //no need to sort, list[0]. bursttime--
            if (_stack.Count != 0)
            {
                var process = _stack[0];
                process.BurstTime--;

                //is burstTime == 0; True: remove from stack and, calculate TAT: waitTime+burstTime, assign wait time
                if (process.BurstTime == 0)
                {
                    var processOnAll = _allProcesses.FirstOrDefault(s => s.ProcessId == process.ProcessId);
                    processOnAll.WaitTime = process.WaitTime;
                    processOnAll.TurnAroundTime = processOnAll.WaitTime + processOnAll.BurstTime;
                    _stack.Remove(process);
                }

                //increment waitTime for all other processes
                foreach (var item in _stack)
                {
                    if (item.ProcessId != process.ProcessId)
                    {
                        item.WaitTime++;
                    }
                }
            }
        }

        private static void ProcessPreemptively()
        {
            //sort by burst time asc
            var pid = GetPidForLowestBurstTimeProcess();
            var process = _stack.FirstOrDefault(s => s.ProcessId == pid);
            //bursttime--
            process.BurstTime--;

            //increament wait time for all remaining processes
            foreach (var processOnStack in _stack)
            {
                if (processOnStack.ProcessId != pid)
                {
                    processOnStack.WaitTime++;
                }
            }

            //is burstTime == 0; True: remove from stack and, calculate TAT: waitTime+burstTime, assign wait time
            if (process.BurstTime == 0)
            {
                var processOnAll = _allProcesses.FirstOrDefault(s => s.ProcessId == pid);
                processOnAll.WaitTime = process.WaitTime;
                processOnAll.TurnAroundTime = processOnAll.WaitTime + processOnAll.BurstTime;
                _stack.Remove(process);
            }
        }

        private static string GetPidForLowestBurstTimeProcess()
        {
            var sortedProcesses = from p in _stack
                                  orderby p.BurstTime
                                  select p.ProcessId;
            return sortedProcesses.ToList().FirstOrDefault();
        }

        public enum SjfType
        {
            Preemptive, //pause and give to shortest
            NonPreemptive //completes current
        }
    }
}
