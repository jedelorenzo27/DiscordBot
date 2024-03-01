using Discord;
using Discord.Interactions;
using DiscordBot.services;
using SixLabors.ImageSharp.PixelFormats;
using SpecterAI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.commands
{
    public class LeetcodeCommands : InteractionModuleBase<SocketInteractionContext>
    {

        [SlashCommand("dev-image-to-2d-array", "hello")]
        public async Task takeImage([Choice("image", "Upload")] Attachment image, [Choice("C#", "C#"), Choice("Java", "Java"), Choice("Python", "Python")] string output_language)
        {
            await DeferAsync();

            string fileName = "tempImageLeetCodeImage.png";
            string outputPath = $"{GeneralUtilities.outputDirectory}{Path.DirectorySeparatorChar}misc{Path.DirectorySeparatorChar}";

            if (File.Exists(outputPath + fileName))
            {
                File.Delete(outputPath + fileName);
            }
            await HttpUtilities.DownloadFileAsync(Program._httpClient, image.Url, outputPath + fileName);

            SixLabors.ImageSharp.Image<Rgba32> downloaded_image = SixLabors.ImageSharp.Image.Load<Rgba32>(outputPath + fileName);

            int[][] grid = LeetcodeServices.ConvertImageTo2DArray(downloaded_image);

            List<string> rows = new List<string>();

            switch(output_language)
            {
                case "C#":
                    rows = TwoDIntArrayToCodeLines.CSharp(grid);
                    break;
                case "Java":
                    rows = TwoDIntArrayToCodeLines.Java(grid);
                    break;
                case "Python":
                    rows = TwoDIntArrayToCodeLines.CSharp(grid);
                    break;
            }

            string textFileName = "tempTextFile.txt";
            if (File.Exists(outputPath + textFileName))
            {
                File.Delete(outputPath + textFileName);
            }
            File.WriteAllLines(outputPath + textFileName, rows.ToArray());

            FileAttachment renderedCard = new FileAttachment(outputPath + textFileName);
            LinkedList<FileAttachment> list = new LinkedList<FileAttachment>();
            list.AddFirst(renderedCard);
            Action<MessageProperties> action = (x) => { x.Attachments = list; x.Content = "Done"; };
            await ModifyOriginalResponseAsync(action);
        }
    }
}
