using SpecterAI.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecterAI.Utilities
{
    public static class HttpUtilities
    {
        public static async Task DownloadFileAsync(HttpClient client, string uri, string outputPath)
        {
            Console.WriteLine("Dowloading file");
            Uri uriResult;
            if (!Uri.TryCreate(uri, UriKind.Absolute, out uriResult))
            {
                await LoggingService.LogMessage(LogLevel.Error, $"[DownloadFileAsync] URI is invalid. Given URI: {uri}");
                throw new InvalidOperationException("URI is invalid.");
            }
            Console.WriteLine("URI seems valid");

            if (File.Exists(outputPath))
                File.Delete(outputPath);

            Console.WriteLine("Cleaned up old files");
            Console.WriteLine("Getting byte stream");

            byte[] fileBytes = await client.GetByteArrayAsync(uri);

            Console.WriteLine($"Writing file to: {outputPath}");
            File.WriteAllBytes(outputPath, fileBytes);

            Console.WriteLine("Done writing image");
        }
    }
}
