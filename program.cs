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

namespace DangCompiler
{
    class Program
	{
		static void Main(string[] args)
        {
			if (args.Length >= 1)
			{
				string file = System.IO.File.ReadAllText(args[0]);
				string[] filesplit = file.Split(
					new string[] { Environment.NewLine },
					StringSplitOptions.None
				);

				foreach (var line in filesplit)
				{
					execute(line);
				}
			}
			else
			{
				Console.WriteLine("Error on args");
			}
		}
	}
}