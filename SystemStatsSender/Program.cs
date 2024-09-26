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

    //private static string GetPerformanceStats()
    //{
    //    Random random = new Random();

    //    int cpuUsage = random.Next(0, 101);
    //    int gpuUsage = random.Next(0, 101);
    //    int ramUsage = random.Next(0, 101);
    //    int networkUsage = random.Next(0, 101);

    //    return $"CPU: {cpuUsage}%, GPU: {gpuUsage}%, RAM: {ramUsage}%, Network: {networkUsage}%";
    //}
    private static int previousCpuUsage = 50;
    private static int previousGpuUsage = 50;
    private static int previousRamUsage = 50;
    private static int previousNetworkUsage = 50;
    private static string GetPerformanceStats()
    {
        Random random = new Random();

        // Function to simulate a natural fluctuation
        int Fluctuate(int previousUsage)
        {
            int change = random.Next(-5, 6); // Random change between -5 and +5
            int newValue = previousUsage + change;

            // Ensure values are within 0-100 range
            if (newValue < 0) newValue = 0;
            if (newValue > 100) newValue = 100;

            return newValue;
        }

        // Generate new usage stats
        previousCpuUsage = Fluctuate(previousCpuUsage);
        previousGpuUsage = Fluctuate(previousGpuUsage);
        previousRamUsage = Fluctuate(previousRamUsage);
        previousNetworkUsage = Fluctuate(previousNetworkUsage);

        return $"CPU: {previousCpuUsage}%, GPU: {previousGpuUsage}%, RAM: {previousRamUsage}%, Network: {previousNetworkUsage}%";
    }
    #region Stats
    //private static PerformanceCounter cpuCounter;
    //private static TimeSpan lastTotalProcessorTime;
    //private static DateTime lastCpuCheckTime;
    //static void Main(string[] args)
    //{
    //    //cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
    //    Console.WriteLine("System Performance Stats:\n");
    //    //cpuCounter.NextValue();
    //    //Thread.Sleep(500);
    //    lastTotalProcessorTime = Process.GetCurrentProcess().TotalProcessorTime;
    //    lastCpuCheckTime = DateTime.UtcNow;

    //    while (true)
    //    {
    //        Console.Clear();
    //        Console.WriteLine($"CPU Usage: {GetCpuUsage()}%");
    //        Console.WriteLine($"RAM USage: {GetAvailableRAM()}%");
    //        //Console.WriteLine("Disk Usage:");
    //        //foreach (var diskUsage in GetDiskUsage())
    //        //{
    //        //    Console.WriteLine($"  {diskUsage.Key}: {diskUsage.Value}%");
    //        //}
    //        Console.WriteLine($"Network Usage: {GetNetworkUsage()}%");
    //        Console.WriteLine($"GPU Usage: {GetGpuUsage()}%");
    //        Console.WriteLine("--------------------------------------------------");
    //        Thread.Sleep(500);
    //    }
    //}

    ////private static float GetCpuUsage()
    ////{
    ////    //var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
    ////    var currentCpuUsage = cpuCounter.NextValue();
    ////    return currentCpuUsage;
    ////}

    //private static float GetCpuUsage()
    //{
    //    Process process = Process.GetCurrentProcess();

    //    TimeSpan currentTotalProcessorTime = process.TotalProcessorTime;
    //    DateTime currentTime = DateTime.UtcNow;

    //    // Calculate the time difference
    //    double cpuUsedMs = (currentTotalProcessorTime - lastTotalProcessorTime).TotalMilliseconds;
    //    double totalPassedMs = (currentTime - lastCpuCheckTime).TotalMilliseconds;

    //    // Update the stored values
    //    lastTotalProcessorTime = currentTotalProcessorTime;
    //    lastCpuCheckTime = currentTime;

    //    // Number of logical processors
    //    int cpuCores = Environment.ProcessorCount;

    //    // Calculate the CPU usage as a percentage of the total processing time used by this app
    //    float cpuUsage = (float)(cpuUsedMs / (totalPassedMs * cpuCores) * 100);
    //    return cpuUsage;
    //}
    //// Get available RAM in percentage
    //private static float GetAvailableRAM()
    //{
    //    var ramCounter = new PerformanceCounter("Memory", "Available MBytes");
    //    float availableMemory = ramCounter.NextValue();
    //    float totalMemory = GetTotalRAM();
    //    return 100 - ((availableMemory / totalMemory) * 100);
    //}

    //// Get total RAM in MB
    //private static float GetTotalRAM()
    //{
    //    ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
    //    ulong totalRam = 0;
    //    foreach (ManagementObject obj in searcher.Get())
    //    {
    //        totalRam += Convert.ToUInt64(obj["Capacity"]);
    //    }
    //    return (float)(totalRam / (1024 * 1024));
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
    //    float bytesPerSecond = networkCounter.NextValue();

    //    // Assuming a maximum network bandwidth of 1 Gbps (Gigabit per second)
    //    float maxBandwidth = 1_000_000_000 / 8; // Convert bits to bytes
    //    //return (bytesPerSecond / maxBandwidth) * 100;
    //    return bytesPerSecond;
    //}

    //// Helper to get the first network interface name
    //private static string GetNetworkInterfaceName()
    //{
    //    PerformanceCounterCategory category = new PerformanceCounterCategory("Network Interface");
    //    var instanceNames = category.GetInstanceNames();
    //    return instanceNames.Length > 0 ? instanceNames[0] : "Not Found";
    //}

    //// Get GPU usage (if supported)
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
