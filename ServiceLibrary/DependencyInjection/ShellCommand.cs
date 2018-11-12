using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

// link: https://gist.github.com/AlexMAS/276eed492bc989e13dcce7c78b9e179d

namespace ServiceLibrary.DependencyInjection
{
    internal static class ShellCommand
    {
        /// <example>ProcessResult result = Execute("cmd", "/c sqlcmd -S servername", 10000);</example>
        /// <remarks>Add "/c" at the beginning of an argument string for Command Prompt to exit just after execution.</remarks>
        public static ProcessResult Execute(string fileName, string arguments, int timeoutMilliseconds)
        {
            ProcessResult result = new ProcessResult() { IsCompleted = false };

            try
            {
                System.Diagnostics.ProcessStartInfo processStartInfo =
                    new System.Diagnostics.ProcessStartInfo(fileName, arguments); // "/c" execute and exit

                processStartInfo.CreateNoWindow = true;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.UseShellExecute = false;

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo = processStartInfo;
                process.Start();

                result.IsCompleted = process.WaitForExit(timeoutMilliseconds); // not needed due to following lines

                result.ExitCode = process.ExitCode;
                //if (process.StandardError.Peek() != -1)
                result.Error = process.StandardError.ReadToEnd();
                result.Output = process.StandardOutput.ReadToEnd();
            }
            catch (Exception ex)
            {
                result.IsCompleted = true;
                result.ExitCode = -1;
                result.Error = ex.Message;
            }

            return result;
        }

        /// <example>ProcessResult result = ExecuteAsync("cmd", "/c ping 8.8.8.8", 5000).GetAwaiter().GetResult();</example>
        /// <remarks>Add "/c" at the beginning of an argument string for Command Prompt to exit just after execution.</remarks>
        public static async Task<ProcessResult> ExecuteAsync(string fileName, string arguments, int timeoutMilliseconds)
        {
            ProcessResult result = new ProcessResult();

            using (var process = new System.Diagnostics.Process())
            {
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                TaskCompletionSource<bool> errorCloseEvent = new TaskCompletionSource<bool>();
                TaskCompletionSource<bool> outputCloseEvent = new TaskCompletionSource<bool>();

                StringBuilder errorBuilder = new StringBuilder();
                StringBuilder outputBuilder = new StringBuilder();

                process.OutputDataReceived += (sender, dataReceived) =>
                {
                    if (string.IsNullOrEmpty(dataReceived.Data))
                        outputCloseEvent.TrySetResult(true);
                    else
                        outputBuilder.AppendLine(dataReceived.Data);
                };

                process.ErrorDataReceived += (sender, dataReceived) =>
                {
                    if (string.IsNullOrEmpty(dataReceived.Data))
                        errorCloseEvent.TrySetResult(true);
                    else
                        errorBuilder.AppendLine(dataReceived.Data);
                };


                bool isStarted;
                try
                {
                    isStarted = process.Start();
                }
                catch (Exception ex)
                {
                    result.IsCompleted = true;
                    result.ExitCode = -1;
                    result.Error = ex.Message;

                    isStarted = false;
                }

                if (isStarted)
                {
                    process.BeginOutputReadLine(); // due to possible deadlocks read the output stream first
                    process.BeginErrorReadLine();  //

                    Task<bool> waitForExit =
                        Task.Run(() => process.WaitForExit(timeoutMilliseconds)); // asynchronously wait for process to exit

                    Task<bool[]> processTask =
                        Task.WhenAll(waitForExit, outputCloseEvent.Task, errorCloseEvent.Task); // ensure closing all tasks

                    if (await Task.WhenAny(Task.Delay(timeoutMilliseconds), processTask) == processTask && waitForExit.Result)
                    {
                        result.IsCompleted = true;
                        result.ExitCode = process.ExitCode;

                        //if (process.ExitCode != 0)
                        result.Error = errorBuilder.ToString();
                        result.Output = outputBuilder.ToString();
                    }
                    else
                    {
                        try
                        {
                            process.Kill();
                        }
                        catch { }
                    }
                }
            }

            return result;
        }

        #region Command Prompt utilities

        /// <summary>
        /// Lists all instance of SQL Server Express LocalDb owned by the current user. 
        /// </summary>
        /// <returns>List of LocalDb instances.</returns>
        /// <remarks>
        /// This method is intended for getting LocalDbVersion from the default LocalDb indentifier:
        /// 'sqlcmd' command is slow, resource consuming, and not present with all installed editions of SQL Server.
        /// 'sqllocaldb' command with 'versions' argument may in special cases fail to retrieve information from the System Registry.
        /// 'sqllocaldb' command with 'info' argument lists available local databases including the default one.
        /// </remarks>
        public static List<string> GetSqlLocalDbInstances()
        {
            string output = null;

            try
            {
                System.Diagnostics.ProcessStartInfo processStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c sqllocaldb info"); // "/c" execute and exit

                processStartInfo.CreateNoWindow = true;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.UseShellExecute = false;

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo = processStartInfo;
                process.Start();
                process.WaitForExit(); // not needed due to following lines

                if (process.StandardError.Peek() == -1) // not failed
                {
                    output = process.StandardOutput.ReadToEnd();
                }
            }
            catch { }


            if (String.IsNullOrWhiteSpace(output))
                return new List<string>();
            else
                return output.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        #endregion // Command Prompt utilities


        public struct ProcessResult
        {
            public bool IsCompleted;
            public int? ExitCode;
            public string Error;
            public string Output;
        }
    }
}
