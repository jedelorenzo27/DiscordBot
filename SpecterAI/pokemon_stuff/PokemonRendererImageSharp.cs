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

        private Font defaultBold;
        private Font defaultItalics;

        public PokemonRendererImageSharp ()
        {
            textHelper = new TextRendererHelper();
            fontCollection = new FontCollection ();
            FontFamily family = fontCollection.Add(Utilities.getResourceDirectory() + @"fonts\Gill-Sans-Condensed-Bold.otf");
            FontFamily sansItalics = fontCollection.Add(Utilities.getResourceDirectory() + @"fonts\Sans-Condensed-BOLDITALIC.ttf");
            defaultBold = family.CreateFont(100, FontStyle.Regular);
            defaultItalics = sansItalics.CreateFont(100, FontStyle.BoldItalic);
        }

        public void renderTest()
        {
            PokemonDefinition pokemonDefinition = new PokemonDefinition();
            pokemonDefinition.health = 100;
            pokemonDefinition.name = "Bumper bump bumpy";
            pokemonDefinition.length = 55;
            pokemonDefinition.weight = 12;
            pokemonDefinition.speciality = "Bumper bump bumpy";
            pokemonDefinition.flavorText = "In the heartland of America, the stage is set for a classic baseball rivalry. This tale unfolds with two teams: the Midland Mavericks tradition,  they love.";
            pokemonDefinition.resistances = new HashSet<PokemonType> { PokemonType.FIRE };
            pokemonDefinition.resistanceStrength = "-10";
            pokemonDefinition.weaknesses =  new HashSet<PokemonType> { PokemonType.WATER };
            pokemonDefinition.weaknessStrength = "+20";
            pokemonDefinition.type = PokemonType.FIRE;
            pokemonDefinition.portraitFileName = "testPortrait.png";
            pokemonDefinition.retreatCost = 4;
            pokemonDefinition.illustrator = "Jason";
            RenderPokemonCard(pokemonDefinition);
        }

        public string RenderPokemonCard(PokemonDefinition definition)
        {
            using (Image image = Image.Load(getBackgroundImage(definition)))
            {
                renderPortrait(image, definition);
                renderName(image, definition);
                renderHealth(image, definition);
                renderDescription(image, definition);
                //renderAttacks(image, definition);
                renderFlavorText(image, definition);
                renderResistances(image, definition);
                renderWeaknesses(image, definition);
                renderRetreatCost(image, definition);
                renderIllustrator(image, definition);
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
            RectangleF boundingBox = getBoundingBox(image, 10, 7, 65, 10.5f);
            textHelper.renderText(image, Brushes.Solid(Color.Black), defaultBold, boundingBox, definition.name.ToUpper(), HorizontalAlignment.Left, VerticalAlignment.Bottom);
        }

        private void renderHealth(Image image, PokemonDefinition definition)
        {
            RectangleF boundingBox = getBoundingBox(image, 66, 7, 83, 10.5f);
            Font font = new Font(defaultBold, 100);
            textHelper.renderText(image, Brushes.Solid(Color.Red), font, boundingBox, definition.health + " HP", HorizontalAlignment.Right, VerticalAlignment.Bottom);

        }

        private void renderDescription(Image image, PokemonDefinition definition)
        {
            // TODO: font should be italics
            RectangleF boundingBox = getBoundingBox(image, 13.75f, 53.75f, 85.5f, 56.5f);
            Font font = new Font(defaultBold, 100);

            string description = "";

            if (definition.speciality != null)
            {
                description += definition.speciality.Trim() + " Pokemon. ";
            }

            if (definition.length > 0)
            {
                description += "Length: " + definition.length;

                if (definition.weight > 0)
                {
                    description += ", ";
                } else
                {
                    description += ".";
                }
            }

            if (definition.weight > 0 )
            {
                description += "Weight: " + definition.weight + " lbs.";
            }
            description = description.Trim();

            textHelper.renderText(image, Brushes.Solid(Color.Black), font, boundingBox, description, HorizontalAlignment.Center, VerticalAlignment.Center);
        }

        private void renderPortrait(Image image, PokemonDefinition definition)
        {
            using (Image portrait = Image.Load(Utilities.getPokemonPortraitsDirectory() + definition.portraitFileName))
            {
                RectangleF boundingBox = getBoundingBox(image, 11.75f, 12.5f, 87.75f, 51.25f);
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
            RectangleF boundingBox = getBoundingBox(image, 9, 89, 89.5f, 94);
            Font font = new Font(defaultItalics, 100);
            textHelper.renderText(image, Brushes.Solid(Color.Black), font, boundingBox, definition.flavorText, HorizontalAlignment.Center, VerticalAlignment.Center);
        }

        private void renderResistances(Image image, PokemonDefinition definition)
        {
            // render icon
            RectangleF boundingBox = getBoundingBox(image, 46, 84.5f, 52, 88.5f);
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

            // render strength value
            if (resistances.Length > 0)
            {
                boundingBox = getBoundingBox(image, 52.25f, 84.5f, 64, 88.5f);
                Font font = new Font(defaultBold, 24);
                textHelper.renderText(image, Brushes.Solid(Color.Black), font, boundingBox, definition.resistanceStrength + "", HorizontalAlignment.Left, VerticalAlignment.Center);
            }
        }

        private void renderWeaknesses(Image image, PokemonDefinition definition)
        {
            // Render Icon
            RectangleF boundingBox = getBoundingBox(image, 12.5f, 84.5f, 18.5f, 88.5f);
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

            // render strength value
            if (weaknesses.Length > 0)
            {
                boundingBox = getBoundingBox(image, 18.75f, 84.5f, 30f, 88.5f);
                Font font = new Font(defaultBold, 24);
                textHelper.renderText(image, Brushes.Solid(Color.Black), font, boundingBox, definition.weaknessStrength + "", HorizontalAlignment.Left, VerticalAlignment.Center);
            }
        }

        private void renderRetreatCost(Image image, PokemonDefinition definition)
        {
            RectangleF boundingBox = getBoundingBox(image, 70, 84.5f, 93, 88.5f);
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

        private void renderIllustrator(Image image, PokemonDefinition definition)
        {
            if (definition.illustrator != null)
            {
                RectangleF boundingBox = getBoundingBox(image, 5.5f, 94.5f, 27, 96.25f);
                Font font = new Font(defaultBold, 100);
                textHelper.renderText(image, Brushes.Solid(Color.Black), font, boundingBox, "Illus. " + definition.illustrator.Trim(), HorizontalAlignment.Left, VerticalAlignment.Center);
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
