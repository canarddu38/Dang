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
using DANGserver;

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
			// string tempdir = Path.GetTempPath();
			string tempdir = Directory.GetCurrentDirectory(); 
			
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
		public void execute(string line, int linecount, string type, string[] filesplit, string file_path)
        {
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc00);
			
			string tempdir = Path.GetTempPath(); 
			// string tempdir = Directory.GetCurrentDirectory();
			// string userprofile = System.Environment.GetEnvironmentVariable("USERPROFILE");
			string userprofile = Directory.GetCurrentDirectory();
			// string userprofile = tempdir;
			
			
			
			if (line.Trim() == "" || line == "" || line.StartsWith("#"))
			{}
			else
			{
				// write command
				if (line.StartsWith("write "))
				{
					if (File.Exists(tempdir + "\\dang\\vars\\"+line.Trim().Replace("write ", "")))
					{
						Console.WriteLine(File.ReadAllText(tempdir + "\\dang\\vars\\"+line.Trim().Replace("write ", "")));
					}
					else
					{
						Console.WriteLine(line.Replace("write ", ""));
					}
					
				}
				// download command
				else if (line.StartsWith("download "))
				{
					string[] tmppline = line.Split(' ');
					using (var client = new WebClient())
					{
						client.DownloadFile(tmppline[1], line.Replace("download ", "").Replace(tmppline[1], ""));
					}
				}
				// help command
				else if (line.StartsWith("help") && type == "script")
				{
					
					sendmsg(@"DuckpvpTeam - DANG V1.4", "green");
					sendmsg(@"DuckpvpTeam - 2023", "green");
					sendmsg("", "green");
					sendmsg(@"Avaliable commands: (Script)
	help                   | getting command list
	write <string>         | write somethin in the terminal
	system [-x] <string>   | execute system command (-x to no output)
	def <name> {<code>}    | define a new function
	download <url> <path>  | download a file to given path
	build_server           | setup the http server", "green");
				}
				// server build
				else if (line.StartsWith("build_server"))
				{
					dang_server_listener server = new dang_server_listener();
					// server.run();
					if(!File.Exists("config.dang"))
					{
						File.WriteAllText("config.dang", server.defaultconfig);
					}
					
				}
				// server run
				else if (line.StartsWith("run_server"))
				{
					dang_server_listener server = new dang_server_listener();
					server.run();
				}
				//vars
				else if (line.StartsWith("s "))
				{
					string[] temp54 = line.Split('=');
					string varname = temp54[0].Replace("s ", "");
					try
					{
						File.Delete(tempdir + "\\dang\\vars\\s\\"+varname);
					}
					catch (Exception e)
					{}
					File.AppendAllText(tempdir + "\\dang\\vars\\s\\"+varname, temp54[1].Trim());
				}
				else if (line.StartsWith("i "))
				{
					string[] temp54 = line.Split('=');
					string varname = temp54[0].Replace("i ", "");
					int n;
					bool isnumber = int.TryParse(temp54[1].Trim(), out n);
					if (isnumber)
					{
						try
						{
							File.Delete(tempdir + "\\dang\\vars\\i\\"+varname);
						}
						catch (Exception e)
						{}
						File.AppendAllText(tempdir + "\\dang\\vars\\i\\"+varname, temp54[1].Trim());
					}
					else
					{
						sendmsg("[x] Value is not a number l."+linecount+1, "red");
					}
				}
				else if (line.StartsWith("b "))
				{
					string[] temp54 = line.Split('=');
					string varname = temp54[0].Replace("b ", "");
					try
					{
						File.Delete(tempdir + "\\dang\\vars\\b\\"+varname);
					}
					catch (Exception e)
					{}
					File.AppendAllText(tempdir + "\\dang\\vars\\b\\"+varname, temp54[1].Trim());
				}
				else if (line.StartsWith("system -x "))
				{
					string callcommand = "/c " + line.Replace("system -x ", "").Replace("\"", "");
			
					ProcessStartInfo processInfo;
					Process process;
					
					processInfo = new ProcessStartInfo("cmd.exe", callcommand);
					processInfo.CreateNoWindow = true;
					processInfo.UseShellExecute = false;
					processInfo.RedirectStandardOutput = false;
					process = Process.Start(processInfo);
				}
				else if (line.StartsWith("system "))
				{
					string callcommand = "/c " + line.Replace("system ", "").Replace("\"", "");
			
					ProcessStartInfo processInfo;
					Process process;
					
					processInfo = new ProcessStartInfo("cmd.exe", callcommand);
					processInfo.CreateNoWindow = true;
					processInfo.UseShellExecute = false;
					processInfo.RedirectStandardOutput = true;
					process = Process.Start(processInfo);
					process.WaitForExit();
					Console.WriteLine(process.StandardOutput.ReadToEnd());
				}
				else
				{
					if (! line.StartsWith("}"))
					{
						string[] temp = line.Split(' ');
						sendmsg("[x] Unknown command \""+temp[0]+"\" at: l."+linecount+1, "red");
					}
				}
			}
		}
	}
}