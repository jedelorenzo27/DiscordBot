using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SpecterAI.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecterAI.pokemonStuff
{
    public class PokemonRendererImageSharp
    {
        private TextRendererHelper textHelper;
        private static bool renderDebugLines = true;
        private FontCollection fontCollection;

        private Font defaultFont;

        public PokemonRendererImageSharp ()
        {
            textHelper = new TextRendererHelper();
            fontCollection = new FontCollection ();
            //FontFamily family = fontCollection.Add(Utilities.getResourceDirectory() + @"fonts\SummerLadiesDEMO.ttf");
            FontFamily family = fontCollection.Add(Utilities.getResourceDirectory() + @"fonts\Cabin-VariableFont.ttf");
            //FontFamily family = fontCollection.Add(Utilities.getResourceDirectory() + @"fonts\CheGuevaraText-Sans.ttf");
            defaultFont = family.CreateFont(42, FontStyle.Regular);

        }

        public void renderTest()
        {
            PokemonDefinition pokemonDefinition = new PokemonDefinition();
            pokemonDefinition.health = 10000;
            pokemonDefinition.name = "Bumper bump bumpy 100 boop 1234 chegu";
            pokemonDefinition.description = "Sample description";
            pokemonDefinition.flavorText = "In the heartland of America, where the sun sets behind fields of corn and the lights of the big city skyline are but a distant glimmer, the stage is set for a classic baseball rivalry. This tale unfolds with two teams: the Midland Mavericks, a team steeped in tradition, and the Coastal Clippers, known for their modern, analytics-driven approach. As the season progresses, these two teams find themselves locked in a fierce battle for the division title, each embodying opposing philosophies of the game they love.";
            pokemonDefinition.resistances = new HashSet<PokemonType> { PokemonType.FIRE, PokemonType.DARK, PokemonType.FIGHTING, PokemonType.ELECTRIC };
            pokemonDefinition.weaknesses =  new HashSet<PokemonType> { PokemonType.WATER };
            pokemonDefinition.type = PokemonType.WATER;
            pokemonDefinition.portraitFileName = "testPortrait.png";
            pokemonDefinition.retreatCost = 7;
            RenderPokemonCard(pokemonDefinition);
        }

        public string RenderPokemonCard(PokemonDefinition definition)
        {
            using (Image image = Image.Load(getBackgroundImage(definition)))
            {
                renderPortrait(image, definition);
                renderName(image, definition);
                renderHealth(image, definition);
                renderAttacks(image, definition);
                renderFlavorText(image, definition);
                renderResistances(image, definition);
                renderWeaknesses(image, definition);
                renderRetreatCost(image, definition);
                image.Save(Utilities.getOutputDirectory() + @"pokemon_cards\" + definition.name + ".png");
            }
            return Utilities.getOutputDirectory() + @"pokemon_cards\" + definition.name + ".png";
        }

        private string getBackgroundImage(PokemonDefinition definition)
        {
            string baseDirectory = Utilities.getResourceDirectory() + @"pokemon\cardBlanks\";
            switch (definition.type)
            {
                case PokemonType.FIRE:
                    return baseDirectory + "Fire.png";
                default:
                    return baseDirectory + "Fire_big.png";
            }
        }

        private string getTypeIconFilePath(PokemonType type)
        {
            string iconPath = Utilities.getResourceDirectory();
            switch(type)
            {
                case PokemonType.DARK:
                    iconPath += @"\pokemon\typeIcons\Dark.png";
                    break;
                case PokemonType.ELECTRIC:
                    iconPath += @"\pokemon\typeIcons\Electric.png";
                    break;
                case PokemonType.FIGHTING:
                    iconPath += @"\pokemon\typeIcons\Fighting.png";
                    break;
                case PokemonType.FIRE:
                    iconPath += @"\pokemon\typeIcons\Fire.png";
                    break;
                case PokemonType.GRASS:
                    iconPath += @"\pokemon\typeIcons\Grass.png";
                    break;
                case PokemonType.NORMAL:
                    iconPath += @"\pokemon\typeIcons\Normal.png";
                    break;
                case PokemonType.PSYCHIC:
                    iconPath += @"\pokemon\typeIcons\Psychic.png";
                    break;
                case PokemonType.STEEL:
                    iconPath += @"\pokemon\typeIcons\Steel.png";
                    break;
                case PokemonType.WATER:
                    iconPath += @"\pokemon\typeIcons\Water.png";
                    break;
                default:
                    iconPath += @"\pokemon\typeIcons\Normal.png";
                    break;
            }
            return iconPath;
        }

        private void renderName(Image image, PokemonDefinition definition)
        {
            RectangleF boundingBox = getBoundingBox(image, 25, 3, 65, 9.25f);
            textHelper.renderText(image, defaultFont, boundingBox, definition.name);
        }

        private void renderHealth(Image image, PokemonDefinition definition)
        {
            // HP
            RectangleF boundingBox = getBoundingBox(image, 71, 7, 75, 9.25f);
            Font font = new Font(defaultFont, 12);
            textHelper.renderText(image, font, boundingBox, "HP", HorizontalAlignment.Right, VerticalAlignment.Bottom);

            // Health Value
            boundingBox = getBoundingBox(image, 75, 3, 87, 9.25f);
            font = new Font(defaultFont, 100);
            textHelper.renderText(image, font, boundingBox, definition.health + "", HorizontalAlignment.Left, VerticalAlignment.Bottom);

        }

        private void renderPortrait(Image image, PokemonDefinition definition)
        {
            using (Image portrait = Image.Load(Utilities.getPokemonPortraitsDirectory() + definition.portraitFileName))
            {
                RectangleF boundingBox = getBoundingBox(image, 8.5f, 10, 91.5f, 49.5f);
                portrait.Mutate(o => o.Resize(new Size((int)boundingBox.Width, (int)boundingBox.Height)));
                image.Mutate(o => o.DrawImage(portrait, new Point((int)boundingBox.X, (int)boundingBox.Y), 1f));
            }
        }

        private void renderAttacks(Image image, PokemonDefinition definition)
        {
            RectangleF boundingBox = getBoundingBox(image,5f, 53, 94.5f, 83.5f);
        }

        private void renderFlavorText(Image image, PokemonDefinition definition)
        {
            RectangleF boundingBox = getBoundingBox(image, 53, 85, 83, 93);
            Font font = new Font(defaultFont, 100);
            textHelper.renderText(image, font, boundingBox, definition.flavorText, HorizontalAlignment.Right, VerticalAlignment.Bottom);
        }

        private void renderResistances(Image image, PokemonDefinition definition)
        {
            RectangleF boundingBox = getBoundingBox(image, 8.5f, 87, 26, 90);
            PokemonType[] resistances = definition.resistances.ToArray();
            int offset = (int)(boundingBox.Width) / resistances.Length;
            for (int i = 0; i < resistances.Length; i++)
            {
                using (Image retreatIcon = Image.Load(getTypeIconFilePath(resistances[i])))
                {
                    retreatIcon.Mutate(o => o.Resize(new Size((int)boundingBox.Height, (int)boundingBox.Height)));
                    image.Mutate(o => o.DrawImage(retreatIcon, new Point((int)boundingBox.X + (i * offset), (int)boundingBox.Y + 1), 1f));
                }
            }
        }

        private void renderWeaknesses(Image image, PokemonDefinition definition)
        {
            RectangleF boundingBox = getBoundingBox(image, 26.5f, 87, 44.5f, 90);
            PokemonType[] weaknesses = definition.weaknesses.ToArray();
            int offset = (int)(boundingBox.Width) / weaknesses.Length;
            for (int i = 0; i < weaknesses.Length; i++)
            {
                using (Image retreatIcon = Image.Load(getTypeIconFilePath(weaknesses[i])))
                {
                    retreatIcon.Mutate(o => o.Resize(new Size((int)boundingBox.Height, (int)boundingBox.Height)));
                    image.Mutate(o => o.DrawImage(retreatIcon, new Point((int)boundingBox.X + (i * offset), (int)boundingBox.Y + 1), 1f));
                }
            }
        }

        private void renderRetreatCost(Image image, PokemonDefinition definition)
        {
            RectangleF boundingBox = getBoundingBox(image, 17, 92, 43, 96);
            using (Image retreatIcon = Image.Load(getTypeIconFilePath(PokemonType.NORMAL)))
            {
                retreatIcon.Mutate(o => o.Resize(new Size((int)boundingBox.Height, (int)boundingBox.Height)));
                int offset = (int)(boundingBox.Width) / definition.retreatCost;
                for(int i = 0; i < definition.retreatCost; i++)
                {
                    image.Mutate(o => o.DrawImage(retreatIcon, new Point((int)boundingBox.X + (i * offset), (int)boundingBox.Y + 1), 1f));
                }
            }
        }

        /// <summary>
        /// Returns a RectangleF with Image corrected coordinates. pX1/py1/px2/py2 are provided as percentages of the image height/width 
        /// so boxes are correctly scaled to any card dimensions
        /// </summary>
        private RectangleF getBoundingBox(Image image, float px1, float py1, float px2, float py2)
        {
            px1 /= 100;
            py1 /= 100;
            px2 /= 100;
            py2 /= 100;

            int width = image.Width;
            int height = image.Height;

            RectangleF box = new RectangleF(); 
            box.X = width * px1;
            box.Y = height * py1;
            box.Width = (px2 * width) - box.X;
            box.Height = (py2 * height) - box.Y;

            if (renderDebugLines)
            {
                PatternPen pen = Pens.Dash(Color.Magenta, 1);
                image.Mutate(x => x.Draw(pen, box));
            }

            return box;
        }

        

    }
}
