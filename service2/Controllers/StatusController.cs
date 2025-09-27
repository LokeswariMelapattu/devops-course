using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics; 
using System.Net.Http;


namespace service2.Controllers;

[ApiController]
[Route("status")]
public class StatusController : ControllerBase
{
    private static readonly DateTime StartTime = DateTime.UtcNow;

    private readonly string vStoragePath = "/app/vstorage";
    private readonly string storagePath = "http://storage:8082/log";
    private readonly HttpClient _client;
    
    public StatusController(HttpClient client)
    {
        _client = client;
    }
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
        try
        {
            var content = new StringContent(record, System.Text.Encoding.UTF8, "text/plain");
    
            var response = await _client.PostAsync(storagePath, content); 
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


    private  async Task LogToVStorage(string record)
    {
        try
        {          
            // Append log
            await System.IO.File.AppendAllTextAsync(vStoragePath, record + Environment.NewLine);
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
        await LogToVStorage(status);
        return Ok(status);
    }
}
