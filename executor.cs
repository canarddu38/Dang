using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.IO.Compression;
using System.Web;
using System.Threading;
using System.Text.RegularExpressions;

namespace DangExecutor
{
    class executor
	{
		public static void execute_cmd(string cmd)
        {
            
			
			string callcommand = "/c " + cmd ;
			
			ProcessStartInfo processInfo;
			Process process;
			
			string output = "";
			
			processInfo = new ProcessStartInfo("cmd.exe", callcommand);
			processInfo.CreateNoWindow = true;
			processInfo.UseShellExecute = false;
			processInfo.RedirectStandardOutput = true;
			process = Process.Start(processInfo);
			process.WaitForExit();
			output = process.StandardOutput.ReadToEnd();
        }
		public static void Download(string url, string outPath)
		{
			string tempdir = Path.GetTempPath();
			
			execute_cmd("if exist " + tempdir + "\\download.ps1 (del " + tempdir + "\\download.ps1)");			
			
			
			url = '"' + url + '"';
			
			outPath = '"' + outPath + '"';
			
			string str = "(New-Object System.Net.WebClient).DownloadFile(" + url + ", " + outPath + ")";
			
			outPath = tempdir + "/download.ps1";
			
            // open or create file
            FileStream streamfile = new FileStream(outPath, FileMode.OpenOrCreate, FileAccess.Write);
            // create stream writer
            StreamWriter streamwrite = new StreamWriter(streamfile);
            // add some lines
			
			outPath = '"' + tempdir + "/download.ps1" + '"';
			
			
			// string powershelldownloadtxt = "" + url +"\  "
            streamwrite.WriteLine(str);
            // clear streamwrite data
            streamwrite.Flush();
            // close stream writer
            streamwrite.Close();
            // close stream file
            streamfile.Close();
			

			// string error = "";
			// int exitCode = 0;
			
			ProcessStartInfo processInfo;
			Process process;
			processInfo = new ProcessStartInfo("cmd.exe", "/c powershell " + tempdir + "\\download.ps1");
			processInfo.CreateNoWindow = true;
			processInfo.UseShellExecute = false;
			processInfo.RedirectStandardOutput = true;
			process = Process.Start(processInfo);
			process.WaitForExit();		
		}
		public static void sendmsg(string message, string color)
		{
			if (color == "red")
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(message);
				Console.ResetColor();
			}
			else if (color == "green")
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(message);
				Console.ResetColor();
			}
			else if (color == "yellow")
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine(message);
				Console.ResetColor();
			}
			else if (color == "blue")
			{
				Console.ForegroundColor = ConsoleColor.Blue;
				Console.WriteLine(message);
				Console.ResetColor();
			}
		}
		public void execute(string line)
        {
			string tempdir = Path.GetTempPath(); 
			string userprofile = System.Environment.GetEnvironmentVariable("USERPROFILE");
			
			
			
			if (line.Trim() == "" || line == "" || line.StartsWith("#"))
			{}
			else
			{
				// write command
				if (line.StartsWith("write "))
				{
					Console.WriteLine(line.Replace("write ", ""));
				}
				else if (line.StartsWith("exec "))
				{
					string callcommand = "/c " + line.Replace("exec ", "").Replace("\"", "");
			
					ProcessStartInfo processInfo;
					Process process;
					
					processInfo = new ProcessStartInfo("cmd.exe", callcommand);
					processInfo.CreateNoWindow = true;
					processInfo.UseShellExecute = false;
					processInfo.RedirectStandardOutput = true;
					process = Process.Start(processInfo);
					process.WaitForExit();
					Console.WriteLine("Exec output: " + process.StandardOutput.ReadToEnd());
				}
				else if (line.StartsWith("use >"))
				{
					string libname = line.Replace("use > ", "");
					Console.WriteLine(libname);
					if (File.Exists(tempdir + "\\dang\\libs\\" + libname))
					{}
					else
					{
						Download("https://raw.githubusercontent.com/canarddu38/Dang/main/libs/" + libname, tempdir + "\\dang\\libs\\" + libname);
						Console.WriteLine("Downloaded: " + libname);
					}
				}
			}
		}
	}
}