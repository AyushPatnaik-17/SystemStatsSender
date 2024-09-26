using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

class Program
{
    static void Main()
    {
        TcpListener server = new TcpListener(IPAddress.Any, 5000);
        server.Start();
        Console.WriteLine("Server started");
        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            NetworkStream stream = client.GetStream();
            Console.WriteLine("Conencting to client");
            while (client.Connected)
            {
                //Console.WriteLine("Client connected");
                string data = GetPerformanceStats();
                Console.WriteLine($"Sending buffer {data}");
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                stream.Write(buffer, 0, buffer.Length);
                Thread.Sleep(1000);
            }
        }
    }

    private static string GetPerformanceStats()
    {
        Random random = new Random();

        int cpuUsage = random.Next(0, 101);
        int gpuUsage = random.Next(0, 101);
        int ramUsage = random.Next(0, 101);
        int networkUsage = random.Next(0, 101);

        return $"CPU: {cpuUsage}%, GPU: {gpuUsage}%, RAM: {ramUsage}%, Network: {networkUsage}%";
    }
    #region Stats
    //static void Main(string[] args)
    //{
    //    Console.WriteLine("System Performance Stats:\n");
    //    double previousCpuUsage = 0;
    //    double previousTotalProecssorTime = 0;

    //    Process currentProcess = Process.GetCurrentProcess();
    //    while (true)
    //    {
    //        //Console.Clear();
    //        Console.WriteLine($"CPU Usage: {GetCpuUsage(currentProcess, ref previousTotalProecssorTime)}%");
    //        Console.WriteLine($"Available RAM: {GetAvailableRAM()}%");
    //        Console.WriteLine($"Total RAM: {GetTotalRAM()} MB");
    //        Console.WriteLine("Disk Usage:");
    //        foreach (var diskUsage in GetDiskUsage())
    //        {
    //            Console.WriteLine($"  {diskUsage.Key}: {diskUsage.Value}%");
    //        }
    //        Console.WriteLine($"Network Usage: {GetNetworkUsage()}%");
    //        Console.WriteLine($"GPU Usage: {GetGpuUsage()}%");
    //        Console.WriteLine("--------------------------------------------------");
    //        var system = 
    //        //Thread.Sleep(500);
    //    }
    //}

    //private static double GetCpuUsage(Process currentProcess, ref double previousTotalProcessorTime)
    //{
    //    double cpuUsage = 0;
    //    double currentProcessorTime = currentProcess.TotalProcessorTime.TotalMilliseconds;

    //    // Calculate the CPU usage percentage
    //    if (previousTotalProcessorTime > 0)
    //    {
    //        cpuUsage = (currentProcessorTime - previousTotalProcessorTime) / Environment.ProcessorCount;
    //    }

    //    previousTotalProcessorTime = currentProcessorTime;
    //    return cpuUsage;
    //}

    //private static float GetAvailableRAM()
    //{
    //    var ramCounter = new PerformanceCounter("Memory", "Available MBytes");
    //    float availableMemory = ramCounter.NextValue();
    //    float totalMemory = GetTotalRAM();
    //    return 100 - ((availableMemory / totalMemory) * 100);
    //}

    //private static float GetTotalRAM()
    //{
    //    ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
    //    ulong totalRam = 0;
    //    foreach (ManagementObject obj in searcher.Get())
    //    {
    //        totalRam += Convert.ToUInt64(obj["Capacity"]);
    //    }
    //    return (float)(totalRam / (1024 * 1024)); // Convert bytes to MB
    //}

    //private static Dictionary<string, float> GetDiskUsage()
    //{
    //    var diskUsage = new Dictionary<string, float>();
    //    var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk WHERE DriveType = 3");
    //    foreach (ManagementObject disk in searcher.Get())
    //    {
    //        string diskName = disk["DeviceID"].ToString();
    //        ulong freeSpace = (ulong)disk["FreeSpace"];
    //        ulong totalSpace = (ulong)disk["Size"];
    //        float usagePercent = ((totalSpace - freeSpace) / (float)totalSpace) * 100;
    //        diskUsage[diskName] = usagePercent;
    //    }
    //    return diskUsage;
    //}

    //private static float GetNetworkUsage()
    //{
    //    var networkCounter = new PerformanceCounter("Network Interface", "Bytes Total/sec", GetNetworkInterfaceName());
    //    return networkCounter.NextValue();
    //}

    //private static string GetNetworkInterfaceName()
    //{
    //    PerformanceCounterCategory category = new PerformanceCounterCategory("Network Interface");
    //    var instanceNames = category.GetInstanceNames();
    //    return instanceNames.Length > 0 ? instanceNames[0] : "Not Found";
    //}

    //private static float GetGpuUsage()
    //{
    //    try
    //    {
    //        var searcher = new ManagementObjectSearcher("SELECT LoadPercentage FROM Win32_VideoController");
    //        foreach (ManagementObject gpu in searcher.Get())
    //        {
    //            return Convert.ToSingle(gpu["LoadPercentage"]);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Error retrieving GPU usage: {ex.Message}");
    //    }
    //    return 0; // Return 0 if GPU usage cannot be determined
    //}
    #endregion
}
