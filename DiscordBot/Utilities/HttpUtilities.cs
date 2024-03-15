using SpecterAI.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public async static Task<string> downloadWebPage(string url)
        {
            // This doesn't work for sites (like leetcode). You'll get a 403 Forbidden if you're not authorized and I havent spent the time debug that.
            try
            {
                Program._httpClient.DefaultRequestHeaders.TryAddWithoutValidation("user-agent","Other");
                Program._httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
                Program._httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                Program._httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");
                string page = await Program._httpClient.GetStringAsync(url).ConfigureAwait(false);
                Console.WriteLine(page);
                return page;

            } catch (Exception ex)
            {
                string[] errorMessage = new string[]
                {
                    $"Failed to download webpage. Tried downloading from: {url}",
                    ex.Message
                };
                await LoggingService.LogMessage(LogLevel.Error, errorMessage);
            }
            return "";
            
        }
    }
}
