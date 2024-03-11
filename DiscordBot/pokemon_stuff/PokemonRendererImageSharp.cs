using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SpecterAI.helpers;
using SpecterAI.Utilities;
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
        private static bool renderDebugLines = false;
        private FontCollection fontCollection;

        private Font defaultBold;
        private Font defaultRegular;
        private Font defaultItalics;

        public PokemonRendererImageSharp ()
        {
            textHelper = new TextRendererHelper();
            fontCollection = new FontCollection ();
            FontFamily family = fontCollection.Add(GeneralUtilities.resourceDirectory + @"fonts" + Path.DirectorySeparatorChar + "Gill-Sans-Condensed-Bold.otf");
            FontFamily sansItalics = fontCollection.Add(GeneralUtilities.resourceDirectory + @"fonts" + Path.DirectorySeparatorChar + "Sans-Condensed-BOLDITALIC.ttf");
            FontFamily sansRegular = fontCollection.Add(GeneralUtilities.resourceDirectory + @"fonts" + Path.DirectorySeparatorChar + "Sans-Condensed-Regular.ttf");

            defaultBold = family.CreateFont(1000, FontStyle.Regular);
            defaultRegular = sansRegular.CreateFont(1000, FontStyle.Regular);
            defaultItalics = sansItalics.CreateFont(1000, FontStyle.BoldItalic);
        }

        public async Task createTestPokemonCard(PokemonDefinition pokemonDefinition)
        {
            Console.WriteLine("Load background and render card");
            using (Image image = Image.Load(getBackgroundImage(pokemonDefinition)))
            {
                Console.WriteLine("Begin rendering card");
                await renderPokemonCard(image, pokemonDefinition);
                Console.WriteLine("Done rendering pokemon card. Time to Save card");

                string savePath = $"{GeneralUtilities.renderedPokemonCardsDirectory}{pokemonDefinition.name}.png";
                Console.WriteLine($"savePath: {savePath}");
                image.Save(savePath);
                Console.WriteLine("Done saving card to disk");
            }
        }

        public async Task createTestAnimatedPokemonCard_gif(PokemonDefinition pokemonDefinition)
        {
            using Image gif = Image.Load(getBackgroundImage(pokemonDefinition));

            var gifMetaData = gif.Metadata.GetGifMetadata();
            gifMetaData.RepeatCount = 0;

            GifFrameMetadata metadata = gif.Frames.RootFrame.Metadata.GetGifMetadata();
            int frameDelay = 10;
            metadata.FrameDelay = frameDelay;
            for (int i = 0; i < 10; i++)
            {
                pokemonDefinition.health = i * 10;
                pokemonDefinition.weaknessStrength = (i * 10) + "";
                pokemonDefinition.resistanceStrength = (i * 10) + "";
                pokemonDefinition.moveset[0].damage = (i * 10) + "";
                pokemonDefinition.moveset[1].damage = (i * 10) + "";
                using (Image image = Image.Load(getBackgroundImage(pokemonDefinition)))
                {
                    await renderPokemonCard(image, pokemonDefinition);

                    metadata = image.Frames.RootFrame.Metadata.GetGifMetadata();
                    metadata.FrameDelay = frameDelay;

                    gif.Frames.AddFrame(image.Frames.RootFrame);

                }
            }
            gif.Frames.RemoveFrame(0);
            // Save the final result.
            gif.SaveAsGif(GeneralUtilities.outputDirectory + @"pokemon_cards" + Path.DirectorySeparatorChar + pokemonDefinition.name + ".gif");
        }

        // Discord can't display .webp format so it'll appear as a downloadable file instead. 
        public async Task createTestAnimatedPokemonCard_webp(PokemonDefinition pokemonDefinition)
        {
            using Image gif = Image.Load(getBackgroundImage(pokemonDefinition));

            var gifMetaData = gif.Metadata.GetGifMetadata();
            gifMetaData.RepeatCount = 0;

            WebpFrameMetadata metadata = gif.Frames.RootFrame.Metadata.GetWebpMetadata();
            uint frameDelay = 10;
            metadata.FrameDelay = frameDelay;
            for (int i = 0; i < 10; i++)
            {
                pokemonDefinition.health = i * 10;
                pokemonDefinition.weaknessStrength = (i * 10) + "";
                pokemonDefinition.resistanceStrength = (i * 10) + "";
                pokemonDefinition.moveset[0].damage = (i * 10) + "";
                pokemonDefinition.moveset[1].damage = (i * 10) + "";
                using (Image image = Image.Load(getBackgroundImage(pokemonDefinition)))
                {
                    await renderPokemonCard(image, pokemonDefinition);

                    metadata = image.Frames.RootFrame.Metadata.GetWebpMetadata();
                    metadata.FrameDelay = frameDelay;

                    gif.Frames.AddFrame(image.Frames.RootFrame);

                }
            }
            gif.Frames.RemoveFrame(0);
            // Save the final result.
            gif.SaveAsWebp(GeneralUtilities.outputDirectory + @"pokemon_cards\" + pokemonDefinition.name + ".webp");
        }

        private async Task renderPokemonCard(Image image, PokemonDefinition definition)
        {
            renderPortrait(image, definition);
            await renderName(image, definition);
            await renderHealth(image, definition);
            await renderDetails(image, definition);
            await renderAttacks(image, definition);
            await renderFlavorText(image, definition);
            await renderResistances(image, definition);
            await renderWeaknesses(image, definition);
            renderRetreatCost(image, definition);
            await renderIllustrator(image, definition);
            renderRarity(image, definition);

        }

        private string getBackgroundImage(PokemonDefinition definition)
        {
            string backgroundPath = GeneralUtilities.resourceDirectory + @"pokemon\CardBlanks\";
            switch (definition.type)
            {
                case PokemonType.ELECTRIC:
                    backgroundPath += "Electric.png";
                    break;
                case PokemonType.FIGHTING:
                    backgroundPath += "Fighting.png";
                    break;
                case PokemonType.FIRE:
                    backgroundPath += "Fire.png";
                    break;
                case PokemonType.GRASS:
                    backgroundPath += "Grass.png";
                    break;
                case PokemonType.NORMAL:
                    backgroundPath += "Normal.png";
                    break;
                case PokemonType.PSYCHIC:
                    backgroundPath += "Psychic.png";
                    break;
                case PokemonType.WATER:
                    backgroundPath += "Water.png";
                    break;
                default:
                    backgroundPath += "Normal.png";
                    break;
            }
            return backgroundPath;
        }

        private string getTypeIconFilePath(PokemonType type)
        {
            string iconPath = GeneralUtilities.resourceDirectory + "pokemon" + Path.DirectorySeparatorChar + "typeIcons" + Path.DirectorySeparatorChar;
            switch(type)
            {
                case PokemonType.DARK:
                    iconPath += "Dark.png";
                    break;
                case PokemonType.ELECTRIC:
                    iconPath += "Electric.png";
                    break;
                case PokemonType.FIGHTING:
                    iconPath += "Fighting.png";
                    break;
                case PokemonType.FIRE:
                    iconPath += "Fire.png";
                    break;
                case PokemonType.GRASS:
                    iconPath += "Grass.png";
                    break;
                case PokemonType.NORMAL:
                    iconPath += "Normal.png";
                    break;
                case PokemonType.PSYCHIC:
                    iconPath += "Psychic.png";
                    break;
                case PokemonType.STEEL:
                    iconPath += "Steel.png";
                    break;
                case PokemonType.WATER:
                    iconPath += "Water.png";
                    break;
                default:
                    iconPath += "Normal.png";
                    break;
            }
            return iconPath;
        }

        private async Task renderName(Image image, PokemonDefinition definition)
        {
            RectangleF boundingBox = getBoundingBox(image, 10, 7, 65, 10.5f);
            await textHelper.renderText(image, Brushes.Solid(Color.Black), defaultBold, boundingBox, definition.name.ToUpper(), HorizontalAlignment.Left, VerticalAlignment.Bottom);
        }

        private async Task renderHealth(Image image, PokemonDefinition definition)
        {
            RectangleF boundingBox = getBoundingBox(image, 66, 7, 83, 10.5f);
            Font font = new Font(defaultBold, 100);
            await textHelper.renderText(image, Brushes.Solid(Color.Red), font, boundingBox, definition.health + " HP", HorizontalAlignment.Right, VerticalAlignment.Bottom);
        }

        private async Task renderDetails(Image image, PokemonDefinition definition)
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

            await textHelper.renderText(image, Brushes.Solid(Color.Black), font, boundingBox, description, HorizontalAlignment.Center, VerticalAlignment.Center);
        }

        private void renderPortrait(Image image, PokemonDefinition definition)
        {
            using (Image portrait = Image.Load(GeneralUtilities.pokemonPortraitsDirectory + definition.portraitFileName))
            {
                RectangleF boundingBox = getBoundingBox(image, 11.75f, 12.5f, 87.75f, 51.25f);
                portrait.Mutate(o => o.Resize(new Size((int)boundingBox.Width, (int)boundingBox.Height)));
                image.Mutate(o => o.DrawImage(portrait, new Point((int)boundingBox.X, (int)boundingBox.Y), 1f));
            }
        }

        private async Task renderAttacks(Image image, PokemonDefinition definition)
        {
            RectangleF totalAttackSpace = getBoundingBox(image,5.5f, 57, 94f, 81.75f);
            if (definition.moveset.Count == 1)
            {
                await renderAttack(image, totalAttackSpace, definition.moveset[0], 1);
            } else if (definition.moveset.Count == 2)
            {
                RectangleF attackMove1 = getBoundingBox(image, totalAttackSpace, 0, 0, 100, 50);
                await renderAttack(image, attackMove1, definition.moveset[0], 2);
                RectangleF attackMove2 = getBoundingBox(image, totalAttackSpace, 0, 50, 100, 100);
                await renderAttack(image, attackMove2, definition.moveset[1], 2);
            }
            else if (definition.moveset.Count > 2)
            {
                RectangleF attackMove1 = getBoundingBox(image, totalAttackSpace, 0, 0, 100, 33);
                await renderAttack(image, attackMove1, definition.moveset[0], 3);
                RectangleF attackMove2 = getBoundingBox(image, totalAttackSpace, 0, 33, 100, 66);
                await renderAttack(image, attackMove2, definition.moveset[1], 3);
                RectangleF attackMove3 = getBoundingBox(image, totalAttackSpace, 0, 66, 100, 100);
                await renderAttack(image, attackMove3, definition.moveset[2], 3);
            }
        }

        private async Task renderAttack(Image image, RectangleF boundingBox, PokemonAttack attack, int totalNumberOfAttacks)
        {

            RectangleF attackIcons;
            if (totalNumberOfAttacks == 1)
            {
                attackIcons = getBoundingBox(image, boundingBox, 0, 20, 15, 80);
            } else 
            {
                attackIcons = getBoundingBox(image, boundingBox, 0, 0, 15, 100);
            }


            if (attack.amount == 1)
            {
                attackIcons = getBoundingBox(image, attackIcons, 0, 29, 100, 61);
                renderIcon(image, attackIcons, attack.attackType);

            } else if (attack.amount == 2)
            {
                attackIcons = getBoundingBox(image, attackIcons, 0, 29, 100, 61);
                RectangleF icon1 = getBoundingBox(image, attackIcons, 0, 0, 50, 100);
                renderIcon(image, icon1, attack.attackType);

                RectangleF icon2 = getBoundingBox(image, attackIcons, 50, 0, 100, 100);
                renderIcon(image, icon2 , attack.attackType);

            }
            else if (attack.amount == 3)
            {
                attackIcons = getBoundingBox(image, attackIcons, 0, 20, 100, 80);
                RectangleF topRow = getBoundingBox(image, attackIcons, 5, 0, 95, 50);
                RectangleF icon1 = getBoundingBox(image, topRow, 0, 0, 50, 100);
                renderIcon(image, icon1, attack.attackType);
                RectangleF icon2 = getBoundingBox(image, topRow, 50, 0, 100, 100);
                renderIcon(image, icon2, attack.attackType);

                RectangleF bottomRow = getBoundingBox(image, attackIcons, 27.5f, 50, 72.5f, 100);
                renderIcon(image, bottomRow, attack.attackType);
            }
            else
            {
                attackIcons = getBoundingBox(image, attackIcons, 0, 20, 100, 80);
                RectangleF topRow = getBoundingBox(image, attackIcons, 5, 0, 95, 50);
                RectangleF icon1 = getBoundingBox(image, topRow, 0, 0, 50, 100);
                renderIcon(image, icon1, attack.attackType);
                RectangleF icon2 = getBoundingBox(image, topRow, 50, 0, 100, 100);
                renderIcon(image, icon2, attack.attackType);

                RectangleF bottomRow = getBoundingBox(image, attackIcons, 5, 50, 95, 100);
                RectangleF icon3 = getBoundingBox(image, bottomRow, 0, 0, 50, 100);
                renderIcon(image, icon3, attack.attackType);
                RectangleF icon4 = getBoundingBox(image, bottomRow, 50, 0, 100, 100);
                renderIcon(image, icon4, attack.attackType);
            }

            RectangleF name = getBoundingBox(image, boundingBox, 15, 0, 85, 30);
            Font font = new Font(defaultBold, 24);
            await textHelper.renderText(image, Brushes.Solid(Color.Black), font, name, attack.name, HorizontalAlignment.Left, VerticalAlignment.Bottom);

            RectangleF description = getBoundingBox(image, boundingBox, 15, 30, 85, 100);
            font = new Font(defaultRegular, 20);
            await textHelper.renderText(image, Brushes.Solid(Color.Black), font, description, attack.description, HorizontalAlignment.Left, VerticalAlignment.Top);

            RectangleF damage = getBoundingBox(image, boundingBox, 85, 0, 100, 100);
            font = new Font(defaultBold, 40);
            await textHelper.renderText(image, Brushes.Solid(Color.Black), font, damage, attack.damage + "", HorizontalAlignment.Right, VerticalAlignment.Center);
        }
        private void renderIcon(Image image, RectangleF boundingBox, PokemonType iconType)
        {
            using (Image icon = Image.Load(getTypeIconFilePath(iconType)))
            {
                float iconSize = Math.Min(boundingBox.Width, boundingBox.Height);
                icon.Mutate(o => o.Resize(new Size((int)iconSize, (int)iconSize)));

                float x = boundingBox.X + ((boundingBox.Width - iconSize) * 0.5f);
                float y = boundingBox.Y + ((boundingBox.Height - iconSize) * 0.5f);

                image.Mutate(o => o.DrawImage(icon, new Point((int)x, (int)y), 1f));
            }
        }


        private async Task renderFlavorText(Image image, PokemonDefinition definition)
        {
            RectangleF boundingBox = getBoundingBox(image, 9, 89, 89.5f, 94);
            Font font = new Font(defaultItalics, 16);
            await textHelper.renderText(image, Brushes.Solid(Color.Black), font, boundingBox, definition.flavorText, HorizontalAlignment.Center, VerticalAlignment.Center);
        }

        private async Task renderResistances(Image image, PokemonDefinition definition)
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
                await textHelper.renderText(image, Brushes.Solid(Color.Black), font, boundingBox, definition.resistanceStrength + "", HorizontalAlignment.Left, VerticalAlignment.Center);
            }
        }

        private async Task renderWeaknesses(Image image, PokemonDefinition definition)
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
                await textHelper.renderText(image, Brushes.Solid(Color.Black), font, boundingBox, definition.weaknessStrength + "", HorizontalAlignment.Left, VerticalAlignment.Center);
            }
        }

        private void renderRetreatCost(Image image, PokemonDefinition definition)
        {
            if (definition.retreatCost > 0)
            {
                RectangleF boundingBox = getBoundingBox(image, 70, 84.5f, 93, 88.5f);
                using (Image retreatIcon = Image.Load(getTypeIconFilePath(PokemonType.NORMAL)))
                {
                    retreatIcon.Mutate(o => o.Resize(new Size((int)boundingBox.Height, (int)boundingBox.Height)));
                    int offset = (int)(boundingBox.Width) / definition.retreatCost;
                    for (int i = 0; i < definition.retreatCost; i++)
                    {
                        image.Mutate(o => o.DrawImage(retreatIcon, new Point((int)boundingBox.X + (i * offset), (int)boundingBox.Y + 1), 1f));
                    }
                }
            }
        }

        private async Task renderIllustrator(Image image, PokemonDefinition definition)
        {
            if (definition.illustrator != null)
            {
                RectangleF boundingBox = getBoundingBox(image, 5.5f, 94.5f, 27, 96.25f);
                Font font = new Font(defaultBold, 100);
                await textHelper.renderText(image, Brushes.Solid(Color.Black), font, boundingBox, "Illus. " + definition.illustrator.Trim(), HorizontalAlignment.Left, VerticalAlignment.Center);
            }
        }

        private void renderRarity(Image image, PokemonDefinition definition)
        {
            RectangleF boundingBox = getBoundingBox(image, 92f, 94.5f, 95.25f, 96.5f);

            string iconPath = GeneralUtilities.resourceDirectory;
            switch (definition.rarity)
            {
                case CardRarity.COMMON:
                    iconPath += @"" + Path.DirectorySeparatorChar + "pokemon" + Path.DirectorySeparatorChar + "miscellaneous_icons" + Path.DirectorySeparatorChar + "rarity_common.png";
                    break;

                case CardRarity.UNCOMMON:
                    iconPath += @"" + Path.DirectorySeparatorChar + "pokemon" + Path.DirectorySeparatorChar + "miscellaneous_icons" + Path.DirectorySeparatorChar + "rarity_uncommon.png";
                    break;

                case CardRarity.RARE:
                    
                    iconPath += @"" + Path.DirectorySeparatorChar + "pokemon" + Path.DirectorySeparatorChar + "miscellaneous_icons" + Path.DirectorySeparatorChar + "rarity_rare.png";
                    break;
            }

            using (Image icon = Image.Load(iconPath))
            {
                float iconSize = Math.Min(boundingBox.Width, boundingBox.Height);
                icon.Mutate(o => o.Resize(new Size((int)iconSize, (int)iconSize)));

                float x = boundingBox.X + ((boundingBox.Width - iconSize) * 0.5f);
                float y = boundingBox.Y + ((boundingBox.Height - iconSize) * 0.5f);

                image.Mutate(o => o.DrawImage(icon, new Point((int)x, (int)y), 1f));
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

        private RectangleF getBoundingBox(Image image, RectangleF boundingBox, float px1, float py1, float px2, float py2)
        {
            RectangleF zeroedBox = new RectangleF(0,0, boundingBox.Width, boundingBox.Height);

            px1 /= 100;
            py1 /= 100;
            px2 /= 100;
            py2 /= 100;

            RectangleF box = new RectangleF();
            box.X = (zeroedBox.Width * px1) + boundingBox.X;
            box.Y = (zeroedBox.Height * py1) + boundingBox.Y;
            box.Width = (zeroedBox.Width * px2) - (zeroedBox.Width * px1);
            box.Height = (zeroedBox.Height * py2) - (py1 * zeroedBox.Height);

            if (renderDebugLines)
            {
                PatternPen pen = Pens.Dash(Color.Green, 1);
                image.Mutate(x => x.Draw(pen, box));
            }
            return box;
        }
    }
}
