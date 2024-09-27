using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System;

class Program
{
    static ManualResetEvent pausePerformanceDataThread = new ManualResetEvent(true);
    static readonly object consoleLock = new object();

    static void Main()
    {
        Thread udpBroadCastThread = new Thread(StartUdpBroadcast);
        udpBroadCastThread.Start();

        TcpListener server = new TcpListener(IPAddress.Any, 5000);
        server.Start();
        Console.WriteLine("Server started");
        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            NetworkStream stream = client.GetStream();

            Thread sendPerformanceDataThread = new Thread(() => SendPerformanceData(stream, client));
            sendPerformanceDataThread.Start();

            Thread handleCommandsThread = new Thread(() => HandleCommands(stream, sendPerformanceDataThread));
            handleCommandsThread.Start();
        }
    }

    private static void StartUdpBroadcast()
    {
        UdpClient udpServer = new UdpClient();
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 8888);

        while (true)
        {
            string message = "SERVER_IP=" + GetLocalIPAddress() + ";PORT=5000";
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            udpServer.Send(buffer, buffer.Length, endPoint);
            Thread.Sleep(1000);
        }
    }

    private static string GetLocalIPAddress()
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

    static bool shouldShowBuffer = true;
    static bool isTroubleshooted = false;

    private static void SendPerformanceData(NetworkStream stream, TcpClient client)
    {
        while (client.Connected && shouldShowBuffer)
        {
            //Console.WriteLine("\nClient Connected!");
            pausePerformanceDataThread.WaitOne();

            string data = GetPerformanceStats();
            lock (consoleLock)
            {
                Console.Clear();
                string[] split = data.Split(",");
                Console.WriteLine("Sending Buffer:");
                string[] stats = new string[] { "CPU", "GPU", "RAM1", "RAM2", "Disk" };
                int i = 0;
                foreach (var item in split)
                {
                    Console.WriteLine(stats[i] + " Usage:" + item);
                    i++;
                }
                //Console.WriteLine($"Sending buffer: {data}");
            }

            byte[] buffer = Encoding.ASCII.GetBytes(data);
            stream.Write(buffer, 0, buffer.Length);
            Thread.Sleep(1000);
        }
    }

    private static void HandleCommands(NetworkStream stream, Thread performanceDataThread)
    {
        while (true)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) break; // Handle disconnected clients

                string command = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                if (command == "LAUNCH_APP")
                {
                    LaunchTroubleshoot(stream);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error reading from stream: {e.Message}");
                break;
            }
        }
    }

    private static void LaunchTroubleshoot(NetworkStream stream)
    {
        Console.WriteLine("Starting troubleshooting...");

        pausePerformanceDataThread.Reset(); // Pause the thread
        shouldShowBuffer = false;

        for (int i = 0; i < 90; i += new Random().Next(6, 11))
        {
            lock (consoleLock)
            {
                Console.Clear();
                string troubleshootingData = $"Troubleshooting: {i}%";
                Console.Write(troubleshootingData);
            }
            var random = new Random();
            var waitTime = random.Next(600, 1001);
            Thread.Sleep(waitTime);
        }

        string failureMessage = "Troubleshooting failed!";
        lock (consoleLock)
        {
            Console.WriteLine($"\n\n{failureMessage}");
        }
        byte[] failureBuffer = Encoding.ASCII.GetBytes(failureMessage);
        isTroubleshooted = true;
        stream.Write(failureBuffer, 0, failureBuffer.Length);

        pausePerformanceDataThread.Set();
    }

    private static int previousCpuUsage = 50;
    private static int previousGpuUsage = 20;
    private static int previousRam1Usage = 50;
    private static int previousRam2Usage = 50;
    private static int previousDiskUsage = 5;

    private static string GetPerformanceStats()
    {
        Random random = new Random();

        int Fluctuate(int previousUsage, int lower = -2, int higher = 2)
        {
            int change = random.Next(lower, higher);
            int newValue = previousUsage + change;

            if (newValue < 0) newValue = 0;
            if (newValue > 100) newValue = 100;

            return newValue;
        }

        previousCpuUsage = Math.Clamp(Fluctuate(previousCpuUsage), 30, 65);
        previousGpuUsage = Math.Clamp(Fluctuate(previousGpuUsage), 30, 65);
        previousRam1Usage = Fluctuate(previousRam1Usage);
        previousRam2Usage = Fluctuate(previousRam2Usage);
        previousDiskUsage = Math.Clamp(Fluctuate(previousDiskUsage, -1, 1), 2, 7);

        string ram1Usage = isTroubleshooted ? previousRam1Usage.ToString() + "%" : "100%";
        string ram2Usage = isTroubleshooted ? previousRam2Usage.ToString() + "%" : "Unidentified";

        return $"{previousCpuUsage}%,{previousGpuUsage}%,{ram1Usage},{ram2Usage},{previousDiskUsage}%";
    }
}