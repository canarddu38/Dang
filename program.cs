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
		public static void write_txt_to_file(string name, string text)
		{
			File.WriteAllText(name, text);
		}
		static void Main(string[] args)
        {
			// prerequirements
			string tempdir = Path.GetTempPath(); 
			string userprofile = System.Environment.GetEnvironmentVariable("USERPROFILE");
			if (! Directory.Exists(tempdir + "\\dang\\def") || ! Directory.Exists(userprofile + "\\Dang"))
			{
				Directory.CreateDirectory(tempdir + "\\dang\\def");
				Directory.CreateDirectory(tempdir + "\\dang\\libs");
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
			}
			if (args.Length >= 1)
			{
				string file = System.IO.File.ReadAllText(args[0]);
				string[] filesplit = file.Split(
					new string[] { Environment.NewLine },
					StringSplitOptions.None
				);
				int a = 0;
				foreach (var line in filesplit)
				{
					string[] temp = line.Split(' ');
					if (File.Exists(tempdir + "\\dang\\def\\" + temp[0]+".dang"))
					{
						Console.WriteLine("exist");
						string file2 = System.IO.File.ReadAllText(tempdir + "\\dang\\def\\" + temp[0] + ".dang");
						string[] filesplit2 = file2.Split(
							new string[] { Environment.NewLine },
							StringSplitOptions.None
						);
						foreach (var line2 in filesplit2)
						{
							Console.WriteLine(line2);
							executor exec = new executor();
							exec.execute(line2);
						}
					}
					else if (line.StartsWith("def "))
					{
						a++;
						string defname = temp[1];
						string defcontent = "# def header";
						while(! filesplit[a].StartsWith("}"))
						{
							defcontent = defcontent + "\n" + filesplit[a];
							filesplit[a] = "# "+filesplit[a];
							a++;
						}
						write_txt_to_file(tempdir+"\\dang\\def\\"+defname+ ".dang", defcontent);
					}
					else
					{
						executor exec = new executor();
						exec.execute(line);
						a++;
					}
				}
			}
			else
			{
				Console.WriteLine("Error on args, Usage: dang <file path>");
			}
		}
	}
}