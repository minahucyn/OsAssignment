using OsAssgnment.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OsAssignment.RoundRobin
{
    class Program
    {
        private static List<Process> _allProcesses;
        private static List<Process> _stack;
        private static int _cpuCycleCurrent;
        private static event EventHandler ProcessingCompleted;
        private static bool _processingCompleted;
        private static bool _processingStarted;
        private static int _tobeProcessedIndex;
        private readonly static int _timeQuantum = 3;
        private static int _reminingCyclesForProcess;

        public static List<ExportStatistics> InitalizeAndRunRoundRobin(int noOfRuns)
        {
            int i = 0;
            int numberOfRuns = noOfRuns;
            var allProcessBatches = new List<Process>();

            while (i <= numberOfRuns)
            {
                var processBatch = RunRoundRobin(); ;
                allProcessBatches.AddRange(processBatch);
                i++;
            }

            //calculate average wait time and turn around time for each process
            return Functions.CalculateStatistics(allProcessBatches);
        }

        private static List<Process> RunRoundRobin()
        {
            //generate a list of processes with random arrival and burst times
            _allProcesses = Functions.GenerateInitialProcessSetup();
            _processingCompleted = false;
            _processingStarted = false;
            _cpuCycleCurrent = 0;
            _stack = new List<Process>();
            _tobeProcessedIndex = 0;
            ResetRemainingCyclesOnProcessToQuantumTime();

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
            //do exporting or what ever

            _processingCompleted = true;
        }

        private static void OnCpuCycle()
        {
            RunCpuCyle();
        }

        private static void RunCpuCyle()
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

            ////checking the stack and printing to the console
            //foreach (var processonstack in _stack)
            //{
            //    Console.WriteLine($"Arrival Time: {processonstack.ArrivalTime} | Burst Time: {processonstack.BurstTime}");
            //}
            //Console.WriteLine("-----------------------------------------------");

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

            //reached at the end of queue, set to start over from 0 index
            if (_tobeProcessedIndex > _stack.Count-1)
            {
                _tobeProcessedIndex = 0;
                ResetRemainingCyclesOnProcessToQuantumTime();
                return;
            }
            else
            {
                var process = _stack[_tobeProcessedIndex];
                process.BurstTime--;
                _reminingCyclesForProcess--;

                if (process.BurstTime==0)
                {
                    ResetRemainingCyclesOnProcessToQuantumTime();
                    var currenntProcessFromAllProcessList = _allProcesses.FirstOrDefault(p => p.ProcessId == process.ProcessId);
                    currenntProcessFromAllProcessList.TurnAroundTime = process.WaitTime + currenntProcessFromAllProcessList.BurstTime;
                    _stack.Remove(process);
                }
                //increment waitTime for all other processes
                foreach (var stackProcess in _stack)
                {
                    if (stackProcess.ProcessId != process.ProcessId)
                    {
                        stackProcess.WaitTime++;
                    }
                }

                //if set amount of quantum time is given for process, move to the next process in queue
                if (_reminingCyclesForProcess == 0)
                {
                    _tobeProcessedIndex++;
                    ResetRemainingCyclesOnProcessToQuantumTime();
                }
            }


            //process round robin


            /*
             * Class scoped variables
             * _lastProcessedIndex
             * _timeQuantum ===> readonly
             * _reminingCyclesForProcess
             * 
             * 
             * condition: Is Arrivaltime == current CPU cycle?
             *      TRUE: add all matches to stack
             * 
             * condition: _processingStarted == true and Stack count == 0
             *          TRUE: completed processing, raise the event, return
             * 
             * condition: _lastProcessedIndex > stack count - 1 ===> 
             *      TRUE: reached the end of stack. reset _lastProcessedIndex to 0
             *      FALSE:
             *            burstTime --
             *            Is burstTime == 0
             *                  TRUE: calculate wait time, TAT ===> allprocess, reset _reminingCyclesForProcess to _timeQuantum
             *                  remove from stack
             *                  dont touch the _lastProcessedIndex
             *             
             */

            _cpuCycleCurrent++;

        }

        private static void ResetRemainingCyclesOnProcessToQuantumTime()
        {
            _reminingCyclesForProcess = _timeQuantum;
        }
    }
}
