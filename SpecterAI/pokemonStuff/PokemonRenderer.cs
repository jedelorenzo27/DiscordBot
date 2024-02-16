using ImageMagick;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecterAI.pokemonStuff
{
    public class PokemonRenderer
    {

        public void renderTest ()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            String Root = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            MagickNET.SetTempDirectory(Root + "/testOutput/");
            

            using (MagickImage image = new MagickImage())
            {
                MagickReadSettings settings = new MagickReadSettings()
                {
                    BackgroundColor = MagickColors.LightBlue, // -background lightblue
                    FillColor = MagickColors.Black, // -fill black
                    Font = "Arial", // -font Arial 
                    Width = 530, // -size 530x
                    Height = 175 // -size x175
                };

                image.Read("caption:This is a test.", settings); // caption:"This is a test."
                image.Write("caption_long_en.png"); // caption_long_en.png

            }
            Console.WriteLine("hello");

            /*
            using (MagickImage image = new MagickImage("input.svg"))
            {
                image.Scale(new Percentage(60));
                image.Write("output.png");
            }
            */
        }


    }
}
