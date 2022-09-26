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
using DangExecutor;

namespace DangCompiler
{
    class Program
	{
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
			string tempdir = "./";
			
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
		public static void write_txt_to_file(string name, string text)
		{
			File.WriteAllText(name, text);
		}
		public static void exec_dang(string dangfilepath)
		{
			// string tempdir = Path.GetTempPath(); 
			string tempdir = Directory.GetCurrentDirectory();
			// string userprofile = System.Environment.GetEnvironmentVariable("USERPROFILE");
			string userprofile = Directory.GetCurrentDirectory(); 
			
			
			string file = System.IO.File.ReadAllText(dangfilepath);
			
			char[] delims = new[] { '\r', '\n' };
			string[] filesplit = file.Split(delims, StringSplitOptions.RemoveEmptyEntries);
			
			
			int a = 0;
			foreach (var line in filesplit)
			{
				// sendmsg(dangfilepath+": "+line, "yellow");
				string[] temp = line.Split(' ');
				if (File.Exists(tempdir + "\\dang\\def\\" + temp[0]+".dang"))
				{
					string file2 = System.IO.File.ReadAllText(tempdir + "\\dang\\def\\" + temp[0] + ".dang");
						
						
					string[] filesplit2 = file2.Split(delims, StringSplitOptions.RemoveEmptyEntries);
							
					foreach (var line2 in filesplit2)
					{
						executor exec = new executor();
						exec.execute(line2, a, "script", filesplit2);
					}
				}
				
				else if (line.StartsWith("def "))
				{
					// a++;
					string defname = temp[1];
					// Console.WriteLine("New def: "+defname);
					string defcontent = "# def header";
					while(! filesplit[a].Trim().StartsWith("}"))
					{
						sendmsg(defname+": "+filesplit[a], "green");
						defcontent = defcontent + "\n" + filesplit[a];
						filesplit[a] = "# "+filesplit[a];
						a++;
					}
					defcontent = defcontent.Replace("def "+defname+" {", "");
					write_txt_to_file(tempdir+"\\dang\\def\\"+defname+ ".dang", defcontent);
				}
				else
				{
					executor exec = new executor();
					exec.execute(line, a, "script", filesplit);
					a++;
				}
			}
		}
		static void Main(string[] args)
        {
			// prerequirements
			// string tempdir = Path.GetTempPath(); 
			string tempdir = Directory.GetCurrentDirectory();
			// string userprofile = System.Environment.GetEnvironmentVariable("USERPROFILE");
			string userprofile = Directory.GetCurrentDirectory();
			if (! Directory.Exists(tempdir + "\\dang\\def") || ! Directory.Exists(userprofile + "\\Dang"))
			{
				Directory.CreateDirectory(tempdir + "\\dang\\def");
				Directory.CreateDirectory(tempdir + "\\dang\\libs");
				Directory.CreateDirectory(tempdir + "\\dang\\vars");
				Directory.CreateDirectory(tempdir + "\\dang\\vars\\b");
				Directory.CreateDirectory(tempdir + "\\dang\\vars\\s");
				Directory.CreateDirectory(tempdir + "\\dang\\vars\\i");
				Directory.CreateDirectory(userprofile + "\\Dang");
			}
			else
			{
				System.IO.DirectoryInfo di = new DirectoryInfo(tempdir + "\\dang\\def");

				foreach (FileInfo file in di.GetFiles())
				{
					file.Delete(); 
				}
				foreach (DirectoryInfo dir in di.GetDirectories())
				{
					dir.Delete(true); 
				}
				di = new DirectoryInfo(tempdir + "\\dang\\vars");

				foreach (FileInfo file in di.GetFiles())
				{
					file.Delete(); 
				}
				foreach (DirectoryInfo dir in di.GetDirectories())
				{
					dir.Delete(true); 
				}
				Directory.CreateDirectory(tempdir + "\\dang\\vars\\b");
				Directory.CreateDirectory(tempdir + "\\dang\\vars\\s");
				Directory.CreateDirectory(tempdir + "\\dang\\vars\\i");
			}
			if (args.Length >= 1)
			{
				string file = System.IO.File.ReadAllText(args[0]);
				string[] filesplit = file.Split(new string[] { Environment.NewLine },StringSplitOptions.None);
				int a = 0;
				foreach (var line in filesplit)
				{
					if (line.StartsWith("use >"))
					{
						int liblines = 0;
						string libname = line.Replace("use > ", "");
						if (File.Exists(tempdir + "\\dang\\libs\\" + libname))
						{
							sendmsg(libname+" exist", "green");
							string file2 = System.IO.File.ReadAllText(tempdir + "\\dang\\libs\\" + libname);
							Console.WriteLine(tempdir + "\\dang\\libs\\" + libname);
							exec_dang(tempdir + "\\dang\\libs\\" + libname);
						}
						else
						{
							Download("https://raw.githubusercontent.com/canarddu38/Dang/main/libs/" + libname, tempdir + "\\dang\\libs\\" + libname);
							if (File.Exists(tempdir + "\\dang\\libs\\" + libname))
							{
								sendmsg("[o] Downloaded library: " + libname, "green");
								string file2 = System.IO.File.ReadAllText(tempdir + "\\dang\\libs\\" + libname);
								Console.WriteLine("lib");
								exec_dang(tempdir + "\\dang\\libs\\" + libname);
							}
							else
							{
								sendmsg("[x] The library ("+libname+") does not exist in our database.", "red");
							}
						}
						write_txt_to_file(args[0], file.Replace("use > "+libname, "#  use > "+libname));
						
					}
				}
				exec_dang(args[0]);
				write_txt_to_file(args[0], file.Replace("#  use > ", "use > "));
			}
			else
			{	
				sendmsg("[x] Error, no file specified, Usage: dang <file path>", "red");
			}
		}
	}
}