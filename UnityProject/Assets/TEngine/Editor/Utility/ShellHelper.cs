using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace TEngine.Editor
{
    public static class ShellHelper
    {
        public static void Run(string cmd, string workDirectory, List<string> environmentVars = null)
        {
            using (Process process = new Process())
            {
                try
                {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
                    string app = "bash";
                    string arguments = "-c";
#elif UNITY_EDITOR_WIN
                    string app = "cmd.exe";
                    string arguments = "/c";
#endif
                    ProcessStartInfo start = new ProcessStartInfo(app)
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        WorkingDirectory = workDirectory,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                    };

                    if (environmentVars != null)
                    {
                        foreach (string var in environmentVars)
                        {
                            start.EnvironmentVariables["PATH"] += ($";{var}"); // Windows路径分隔符
                        }
                    }

                    start.Arguments = $"{arguments} \"{cmd}\"";
                    process.StartInfo = start;

                    process.OutputDataReceived += (sender, args) =>
                    {
                        if (args.Data != null)
                        {
                            // 尝试使用不同的编码来解码输出
                            string output = DecodeOutput(args.Data);
                            UnityEngine.Debug.Log(output);
                        }
                    };

                    process.ErrorDataReceived += (sender, args) =>
                    {
                        if (args.Data != null)
                        {
                            // 尝试使用不同的编码来解码错误输出
                            string errorOutput = DecodeOutput(args.Data);
                            UnityEngine.Debug.LogError(errorOutput);
                        }
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.WaitForExit();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }
        
        private static string DecodeOutput(string data)
        {
            // 尝试使用 UTF-8 编码
            try
            {
                byte[] bytes = Encoding.Default.GetBytes(data);
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                // 如果 UTF-8 失败，尝试 GBK 编码
                try
                {
                    byte[] bytes = Encoding.Default.GetBytes(data);
                    return Encoding.GetEncoding("GBK").GetString(bytes);
                }
                catch
                {
                    // 如果 GBK 也失败，尝试 ISO-8859-1 编码
                    try
                    {
                        byte[] bytes = Encoding.Default.GetBytes(data);
                        return Encoding.GetEncoding("ISO-8859-1").GetString(bytes);
                    }
                    catch
                    {
                        return data; // 如果所有解码都失败，返回原始数据
                    }
                }
            }
        }

        public static void RunByPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                using (Process process = new Process())
                {
                    try
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo(path)
                        {
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                        };

                        process.StartInfo = startInfo;
                        process.OutputDataReceived += (_, args) =>
                        {
                            if (args.Data != null)
                            {
                                string output = DecodeOutput(args.Data);
                                UnityEngine.Debug.Log($"[Process Output:] {output}");
                            }
                        };
                        process.ErrorDataReceived += (_, args) =>
                        {
                            if (args.Data != null)
                            {
                                string errorOutput = DecodeOutput(args.Data);
                                UnityEngine.Debug.LogError($"[Process Error]: {errorOutput}");
                            }
                        };

                        process.Start();
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        UnityEngine.Debug.Log($"Started process with ID: {process.Id} for path: {path}");

                        process.WaitForExit();
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"Error starting process at path {path}: {e.Message}");
                        UnityEngine.Debug.LogException(e);
                    }
                }
            }
        }
    }
}
