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
            // runas admin
			ServicePointManager.Expect100Continue = true; 
			ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc00);
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
			startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
			startInfo.FileName = "cmd.exe";
			startInfo.Arguments = "/C "+cmd;
			// startInfo.Verb = "runas";
			process.StartInfo = startInfo;
			process.Start();
        }
		public static void Download(string url, string outPath)
		{
            ServicePointManager.Expect100Continue = true;
			ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc00);
			
			
			string tempdir = Path.GetTempPath();
			new WebClient().DownloadFile(url, outPath);
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
 # Basics
 def <name> {<code>}      | define a new function
 help                     | getting command list
 write <string>           | write somethin in the terminal
 use > <lib name>         | import a library to your script
	
 # Net
 download <url> <path>    | download a file to given path
 build_server             | setup the dang server
 run_server <config path> | run the dang server with builded config
	
 # System interaction
 system [-x] <string>     | execute system command (-x to no output)", "green");
				}
				// server build
				else if (line.StartsWith("build_server"))
				{
					DangServerListener server = new DangServerListener();
					if(!File.Exists("config.dang"))
					{
						File.WriteAllText("config.dang", server.DefaultConfig);
					}
					
				}
				// server run
				else if (line.StartsWith("run_server"))
				{
					DangServerListener server = new DangServerListener();
					
					string config_path = line.Replace("run_server", "").Trim();
					if(config_path == "")
					{
						sendmsg("[x] No config path, switching to 'config.dang'.", "red");
						config_path = "config.dang";
						if(!File.Exists("config.dang"))
						{
							File.WriteAllText("config.dang", server.DefaultConfig);
						}
					}
					string website_path = server.GetConfigItem("website_folder", config_path);
					if(!Directory.Exists(website_path))
					{
						Directory.CreateDirectory(website_path);
						Directory.CreateDirectory(website_path+@"\admin");
						File.WriteAllText(website_path+@"\admin\index.dang", @"<h1>Dang admin page</h1>
<p>Shutdown server</p>
<button onClick='shutdown()'>SHUTDOWN</button>
<script>
fetch('window.location.protocol + "//" + window.location.host', {
    method: 'POST',
    headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
    },
    body: JSON.stringify({ 'id': 78912 })
})
.then(response => response.json())
.then(response => console.log(JSON.stringify(response)))</script>");
						sendmsg("[x] Website directory does not exists, creating...", "red");
						File.WriteAllText(website_path+@"\index.dang", @"<h1>Dang default index page</h1>
<dang>
write Page executed
echo This is sended by dang processor :p
</dang>");
						File.WriteAllText(website_path+@"\404.dang", @"<h1>404, not found</h1>");
					}
					server.run(config_path);
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