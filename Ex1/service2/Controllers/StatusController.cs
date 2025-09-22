using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;


namespace service2.Controllers;

[ApiController]
[Route("status")]
public class StatusController : ControllerBase
{
    private static readonly DateTime StartTime = DateTime.UtcNow;

    private string AnalyzeStatus()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

        var uptime = Math.Abs((DateTime.UtcNow - StartTime).TotalHours);
        var currentPath = Directory.GetCurrentDirectory();
        var drive = new DriveInfo(currentPath);
        var freeDiskMb = drive.AvailableFreeSpace / (1024 * 1024);
        return $"{timestamp}: uptime {uptime:F2} hours, free disk in root: {freeDiskMb} MBytes"; 
    }

    private async Task LogToStorageAsync(string record)
    {
        using var client = new HttpClient();

        try
        {
            var content = new StringContent(record, System.Text.Encoding.UTF8, "text/plain");
    
            var response = await client.PostAsync("http://storage:8002/log", content); 
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Log successfully sent to Storage service.");
            }
            else
            {
                Console.WriteLine($"Failed to log to Storage: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error logging to Storage: {ex.Message}");
        }
    }


    private void LogToVStorage(string record)
    {
        try
        {
            // write to the vstoragr 
            Console.WriteLine("Log successfully written to vStorage.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to vStorage: {ex.Message}");
        }
    }


    [HttpGet]
    public async Task<IActionResult> GetStatus()
    {
        var status = AnalyzeStatus();
        Console.WriteLine(status);
        await LogToStorageAsync(status);
        LogToVStorage(status);
        return Ok(status);
    }
}
