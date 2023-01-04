using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading.Tasks;
using Mono.Nat;
using DangExecutor;

namespace DANGserver
{
    class dang_server_listener
    {
		
		static string version = "1.4";
		
		static string newline = @"
";
		static string defaultconfig = "# Dang server config"+newline+newline+"# Debug"+newline+"s debug = false"+newline+newline+"# Listener"+newline+"s port = \"80\""+newline+newline+"# Server"+newline+"s website_folder = \"website\"";
 
        public static void DeviceFound(object sender, DeviceEventArgs args)
        {
			if(!File.Exists("config.dang"))
			{
				File.WriteAllText("config.dang", defaultconfig);
			}
			
            INatDevice device = device = args.Device;
            device.CreatePortMap(new Mapping(Protocol.Tcp, Int16.Parse(GetConfig("port")), Int16.Parse(GetConfig("port"))));
 
            foreach (Mapping portMap in device.GetAllMappings())
            {
                Console.WriteLine(portMap.ToString());
            }
 
            Console.WriteLine(device.GetExternalIP().ToString());
            Console.WriteLine(device.GetSpecificMapping(Protocol.Tcp, Int16.Parse(GetConfig("port"))).PublicPort);
        }
 
        public static void DeviceLost(object sender, DeviceEventArgs args)
        {
            INatDevice device = args.Device;
            // device.DeletePortMap(new Mapping(Protocol.Tcp, 8080, 8080));
            // on device disconnect code
        }
		
		
		public static string GetLocalIPAddress()
		{
			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					return ip.ToString();
				}
			}
			throw new Exception("No network adapters with an IPv4 address in the system!");
		}
		
		
        public static HttpListener listener;
        public static string url = "http://"+GetLocalIPAddress()+":8080/";
		public static string website_path = "website";

        public static async Task HandleIncomingConnections()
        {
            bool runServer = true;

            while (runServer)
            {
                HttpListenerContext ctx = await listener.GetContextAsync();

                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine();
				
				string pageData = "an error occured :(";
				
				if (File.Exists(website_path+req.Url.AbsolutePath.Replace("/", @"\")+".dang") | File.Exists(website_path+req.Url.AbsolutePath.Replace("/", @"\")))
				{
					try
					{
						Console.WriteLine(website_path+req.Url.AbsolutePath.Replace("/", @"\")+".dang");
						pageData = File.ReadAllText(website_path+req.Url.AbsolutePath.Replace("/", @"\")+".dang");
					}
					catch(Exception e)
					{
						Console.WriteLine(website_path+req.Url.AbsolutePath.Replace("/", @"\"));
						pageData = File.ReadAllText(website_path+req.Url.AbsolutePath.Replace("/", @"\"));
					}

					
					string[] templist = pageData.Split(new string[] {"<dang>"}, StringSplitOptions.RemoveEmptyEntries);
					
					char[] delims = new[] { '\r', '\n' };					
					
					int a = 1;
					for (int i = 0; i < templist.Length-1; i++)
					{
						string[] templist2 = templist[a].Split(delims, StringSplitOptions.RemoveEmptyEntries);
						int b = 0;
						foreach(string cont2 in templist2)
						{
							if(cont2.Contains("</dang>"))
							{
								break;
							}
							else
							{
								executor exec = new executor();
								exec.execute(cont2, b, "script", templist2);
							}
							b++;
						}
						a++;
					}
				}
				else if (req.Url.AbsolutePath == "" | req.Url.AbsolutePath == "/")
				{
					try
					{
						pageData = File.ReadAllText(website_path+"/index.dang");
					}
					catch (Exception e)
					{
						try
						{
							pageData = File.ReadAllText(website_path+"/index.html");
						}
						catch (Exception f)
						{
							try
							{
								pageData = File.ReadAllText(website_path+"/index.htm");
							}
							catch (Exception g)
							{
								
								string[] fileslist = Directory.GetFiles(website_path);
								
								pageData = "Files in "+req.Url.AbsolutePath;
								foreach(string file in fileslist)
								{
									pageData += "\n"+file;
								}
							}
						}
					}
				}
				else
				{
					try
					{
						pageData = File.ReadAllText(website_path+"/404.dang");
					}
					catch (Exception e)
					{
						try
						{
							pageData = File.ReadAllText(website_path+"/404.html");
						}
						catch (Exception f)
						{
							try
							{
								pageData = File.ReadAllText(website_path+"/404.htm");
							}
							catch (Exception g)
							{
								pageData = "404, file not found";
							}
						}
					}
					
				}
				
                if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown"))
                {
                    Console.WriteLine("Shutdown requested");
                    runServer = false;
                }

                // Write the response info
                byte[] data = Encoding.UTF8.GetBytes(pageData);
                // resp.ContentType = "text/html";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                // Write out to the response stream (asynchronously), then close it
                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }


        public static void run(string[] args)
        {
            // NatUtility.Logger = new StreamWriter(File.OpenWrite("logfile.txt"));
			
			if(!File.Exists("config.dang"))
			{
				File.WriteAllText("config.dang", defaultconfig);
			}
			
			if(GetConfig("public") == "true")
			{
				Console.WriteLine("public > enabled");
				NatUtility.DeviceFound += DeviceFound;
				NatUtility.DeviceLost += DeviceLost;
				NatUtility.StartDiscovery();
			}
			else
			{
				Console.WriteLine("public > disabled");
			}
			// url = "http://"+GetLocalIPAddress()+":"+GetConfig("port")+"/";
			url = "http://localhost:"+GetConfig("port")+"/";
			website_path = GetConfig("website_folder");
			
			Console.WriteLine("website path: "+website_path);
			
			
			// Create a Http server and start listening for incoming connections
			listener = new HttpListener();
			listener.Prefixes.Add(url);
			listener.Start();
			Console.WriteLine("Listening for connections on {0}", url);

			// Handle requests
			Task listenTask = HandleIncomingConnections();
			listenTask.GetAwaiter().GetResult();

			// Close the listener
			listener.Close();
        }
		public static string GetConfig(string item)
        {
			string result = "";
			if(File.Exists("config.dang"))
			{
				string[] temp = File.ReadAllLines("config.dang");
				foreach(var line in temp)
				{
					if(!line.StartsWith("#") && line.Trim() != "")
					{
						string line2 = line.Replace("s ", "");
						line2 = line2.Replace(" ", "");
						line2 = line2.Replace("\"", "");
						string[] temp2 = line2.Split('=');
						// Console.WriteLine(temp2[0]+" | "+temp2[1]);

						
						if(temp2[0] == item)
						{
							result = temp2[1];
						}
					}
				}
			}
			else
			{
				Console.WriteLine("[x] Config file error");
				result = "null";
			}
			return result;
        }
    }
}
