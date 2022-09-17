using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using StockTreemap.Models;

namespace StockTreemap.Services
{
    public class RenderService
    {
        public void CreateImg(int width, int height, string imageName, string savePath, List<Treemap> treemapinfo)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("StockTreemap.Font.msjh.ttf");
            StreamReader reader = new StreamReader(stream);
            var collection = new FontCollection();
            FontFamily family = collection.Add(reader.BaseStream);
            Font font = family.CreateFont(20, FontStyle.Regular);

            using var image = new Image<Rgba32>(width, height);

            image.Mutate(i => i.BackgroundColor(Color.Black));

            foreach (var category in treemapinfo)
            {
                var x = (float)category.X + 4;
                var y = (float)category.Y + 4;
                var w = (float)category.Width - 6;
                var h = 26f;
                var rect = new RectangularPolygon(x, y, w, h);

                image.Mutate(i => i.Fill(ColorConverter(category.ChangePercent), rect)
                                                       .DrawText(category.Name, font, Color.White, new PointF(x + 5, y)));

                foreach (var stock in category.StockTreemapDetailList)
                {
                    var stdx = (float)category.X + (float)stock.X + 4;
                    var stdy = (float)category.Y + (float)stock.Y + 30 + 2;
                    var stdw = (float)stock.Width - 2;
                    var stdh = (float)stock.Height - 2;

                    var diff = (float)stock.Width * 0.2f;
                    var nameTextOptions = TextOpsionSetting(font, stock.Name, stdx + (diff / 2), stdy, (float)stock.Width -  diff, (float)stock.Height / 2);
                    var percentTextOptions = TextOpsionSetting(font, stock.ChangePercent.ToString("f2") + "%", stdx + (diff / 2), stdy + (stdh / 2), (float)stock.Width - diff, (float)stock.Height / 2);
                    var stdrect = new RectangularPolygon(stdx, stdy, stdw, stdh);

                    image.Mutate(i => i.Fill(ColorConverter(stock.ChangePercent), stdrect)
                                                         .DrawText(nameTextOptions, stock.Name, Color.White)
                                                         .DrawText(percentTextOptions, stock.ChangePercent.ToString("f2") + "%", Color.White));
                }
            }

            image.Save(savePath + imageName);
        }

        private TextOptions TextOpsionSetting(Font font,string name, float x, float y, float width, float height)
        {
            var size = TextMeasurer.Measure(name, new TextOptions(font));
            var scalingFactor = Math.Min(width / size.Width, height / size.Height);
            var scaledFont = new Font(font, scalingFactor * font.Size);
            var center = new PointF(width / 2, height / 2);

            var textOptions = new TextOptions(scaledFont)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Origin = new System.Numerics.Vector2(x + center.X, y + center.Y)
            };

            return textOptions;
        }

        private Rgba32 ColorConverter(double percent)
        {
            var colorP6p = new Rgba32(246, 53, 56);
            var colorP4p = new Rgba32(191, 64, 69);
            var colorP2p = new Rgba32(139, 68, 78);
            var colorN2p = new Rgba32(53, 118, 78);
            var color0p = new Rgba32(65, 69, 84);
            var colorN4p = new Rgba32(47, 158, 79);
            var colorN6p = new Rgba32(48, 204, 90);

            return percent switch
            {
                var _ when percent > 4.5 => SolidColorBrush(percent, colorP6p, colorP6p),
                var _ when percent > 3 => SolidColorBrush(percent, colorP6p, colorP4p),
                var _ when percent > 1.5 => SolidColorBrush(percent, colorP4p, colorP2p),
                var _ when percent > 0 => SolidColorBrush(percent, colorP2p, color0p),
                var _ when percent < -4.5 => SolidColorBrush(percent, colorN6p, colorN6p),
                var _ when percent < -3 => SolidColorBrush(percent, colorN6p, colorN4p),
                var _ when percent < -1.5 => SolidColorBrush(percent, colorN4p, colorN2p),
                var _ when percent < 0 => SolidColorBrush(percent, colorN2p, color0p),
                _ => SolidColorBrush(percent, color0p, color0p)
            };
        }

        private Rgba32 SolidColorBrush(double percent, Rgba32 colorUpper, Rgba32 colorLower)
        {
            double absPercent = Math.Abs(percent);
            double fraction;

            if (absPercent > 4.5)
            {
                fraction = 1;
            }
            else
            {
                fraction = (absPercent / 1.5) % 1;
            }

            var color =new Rgba32(
                (byte)(colorUpper.R * fraction + colorLower.R * (1 - fraction)),
                (byte)(colorUpper.G * fraction + colorLower.G * (1 - fraction)),
                (byte)(colorUpper.B * fraction + colorLower.B * (1 - fraction))
            );

            return color;
        }
    }
}
