using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace INSTINCT_Extractor
{
   
    class Program
    {
        static void Main()
        {
            try
            {
                string updateDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "DOWStatTracker", "Updates");
                Directory.CreateDirectory(updateDir);

                string archivePath = Path.Combine(updateDir, "update.zip");
                       // your downloaded update
                string extractPath = AppDomain.CurrentDomain.BaseDirectory; // current folder
                string mainExe = "DOW Stat Tracker.exe";    // program to launch
                                                            // Force close the main app if it's running
                string appName = Path.GetFileNameWithoutExtension(mainExe);
                var runningProcesses = Process.GetProcessesByName(appName);
                foreach (var proc in runningProcesses)
                {
                    try
                    {
                        Console.WriteLine($"Closing running process: {proc.ProcessName} (PID: {proc.Id})");
                        proc.Kill();
                        proc.WaitForExit(); // ensure it has exited
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to close process {proc.Id}: {ex.Message}");
                    }
                }
                // Wait for main app to exit
                Console.WriteLine("Waiting for main app to close...");
                Thread.Sleep(2000);

                if (!File.Exists(archivePath))
                {
                    Console.WriteLine("No update.zip found.");
                    return;
                }

                Console.WriteLine("Extracting update...");

                string runningExe = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

                using (ZipArchive archive = ZipFile.OpenRead(archivePath))
                {
                    foreach (var entry in archive.Entries)
                    {
                        string destinationPath = Path.Combine(extractPath, entry.FullName);

                        // Skip the running extractor
                        if (string.Equals(destinationPath, runningExe, StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine($"Skipping running executable: {entry.Name}");
                            continue;
                        }

                        // Ensure directory exists
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                        // Extract file
                        entry.ExtractToFile(destinationPath, true);
                    }
                }


                // Optionally delete the update.zip
                File.Delete(archivePath);

                Console.WriteLine("Update complete! Launching program...");
                Process.Start(Path.Combine(extractPath, mainExe));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Update failed: " + ex.Message);
            }
            Console.WriteLine("You can safely close this updater.");
        }
    }
}

