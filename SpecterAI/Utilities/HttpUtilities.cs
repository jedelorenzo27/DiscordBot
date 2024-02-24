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
            Uri uriResult;
            if (!Uri.TryCreate(uri, UriKind.Absolute, out uriResult))
                throw new InvalidOperationException("URI is invalid.");

            if (File.Exists(outputPath))
                File.Delete(outputPath);

            byte[] fileBytes = await client.GetByteArrayAsync(uri);
            File.WriteAllBytes(outputPath, fileBytes);
        }
    }
}
