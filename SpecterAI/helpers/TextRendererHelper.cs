using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecterAI.helpers
{
    public class TextRendererHelper
    {
        public bool textFitsBox(RectangleF boundingBox, List<string> textLines, RichTextOptions options)
        {
            float maxHeight = 0;
            foreach (string line in textLines)
            {
                FontRectangle measurements = TextMeasurer.MeasureSize(line, options);
                if (measurements.Width > boundingBox.Width)
                {
                    return false;
                }
                if (measurements.Height > maxHeight)
                {
                    maxHeight = measurements.Height;
                }
            }
            return (maxHeight * textLines.Count) <= boundingBox.Height;
        }

        public float getMaxLineHeight(List<string> textLines, RichTextOptions options)
        {
            float maxLineHeight = 0;
            foreach(string line in textLines)
            {
                maxLineHeight = Math.Max(maxLineHeight, TextMeasurer.MeasureSize(line, options).Height);
            }
            return maxLineHeight;
        }

        public void renderText(Image image, Font font, RectangleF boundingBox, string text, HorizontalAlignment hAlign = HorizontalAlignment.Center, VerticalAlignment vAlign = VerticalAlignment.Top)
        {
            font = new Font(font, getCorrectedFontSize(image, (int)font.Size));
            RichTextOptions options = new(font){};

            Brush brush = Brushes.Solid(Color.Black);
            Pen pen = Pens.Solid(Color.Black, 1);

            List<string> textLines = splitText(boundingBox, options, text);
            while (!textFitsBox(boundingBox, textLines, options) && font.Size > 1)
            {
                font = new Font(font, (int)font.Size - 1);
                options = new(font){};
                textLines = splitText(boundingBox, options, text);
            }

            float maxLineHeight = getMaxLineHeight(textLines, options);

            for (int i = 0; i < textLines.Count; i++)
            {
                FontRectangle beans = TextMeasurer.MeasureSize(textLines[i], options);
                float newX = boundingBox.X, newY = boundingBox.Y + (maxLineHeight * i) ;

                
                switch (hAlign)
                {
                    case HorizontalAlignment.Center:
                        newX += (boundingBox.Width - beans.Width) * 0.5f;
                        break;
                    case HorizontalAlignment.Right:
                        newX += (boundingBox.Width - beans.Width);
                        break;
                }

                switch (vAlign)
                {
                    case VerticalAlignment.Center:
                        newY += (boundingBox.Height - (maxLineHeight * textLines.Count)) * 0.5f;
                        break;
                    case VerticalAlignment.Bottom:
                        newY += (boundingBox.Height - (maxLineHeight * textLines.Count));
                        break;
                }

                options.Origin = new PointF(newX, newY);
                image.Mutate(x => x.DrawText(options, textLines[i], brush));



                PatternPen pen2 = Pens.Dash(Color.Green, 1);
                RectangleF textBox = new RectangleF(newX, newY, beans.Width, beans.Height);

                image.Mutate(x => x.Draw(pen2, textBox));
            }
        }

        private List<string> splitText(RectangleF boundingBox, RichTextOptions options, string text)
        {
            List<string> lines = new List<string>();
            string workingLine = "";
            foreach(string word in text.Split(' '))
            {
                string testLine = workingLine + " " + word;
                testLine = testLine.Trim();
                FontRectangle bounds = TextMeasurer.MeasureSize(testLine, options);
                FontRectangle singleWordDimensions = TextMeasurer.MeasureSize(word, options);
                if (singleWordDimensions.Width >= boundingBox.Width)
                {
                    lines.Add(word);
                } else if (bounds.Width > boundingBox.Width)
                {
                    lines.Add(workingLine);
                    workingLine = word;
                } else
                {
                    workingLine = testLine;
                }
            }
            if (workingLine.Length > 0)
            {
                lines.Add(workingLine);
            }
            return lines;
        }

        // Used to scale font size based on initial card blank resolution. Could have been based on any random number 
        private float arbitraryWidthScale = 588f;
        private int getCorrectedFontSize(Image image, int fontSize)
        {
            float ratio = (image.Width + 0.0f) / arbitraryWidthScale;
            return Math.Max((int)Math.Round(ratio * fontSize), 1);
        }
    }
}
