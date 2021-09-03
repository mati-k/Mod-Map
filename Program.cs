using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using VectSharp;
using VectSharp.SVG;
using System.Drawing.Text;
using Newtonsoft.Json;

namespace ModMap
{
    class Program
    {
        private static List<Province> provinces = new List<Province>();
        private static Dictionary<Color, Rect> Borders = new Dictionary<Color, Rect>();

        private static Settings settings;

        static void Main(string[] args)
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("settings.json"));

            ReadCsv();
            FindColors();

            WriteBmp(Path.Combine(settings.ModFolder, "map", "provinces.bmp"), settings.UseColorContrast, false, Color.White);
            if (settings.AddMultiSized)
                WriteBmpMultiSize(Path.Combine(settings.ModFolder, "map", "provinces.bmp"), settings.UseColorContrast, false, Color.White, settings.MultipliedSize);
            WriteSvg(Path.Combine(settings.ModFolder, "map", "provinces.bmp"), Color.White);

            for (int i = 0; i < settings.AdditionalBitmaps.Count; i++)
            {
                if (!File.Exists(settings.AdditionalBitmaps[i].Path))
                {
                    Console.WriteLine("File doesn't exist: " + settings.AdditionalBitmaps[i].Path);
                    continue;
                }

                WriteBmp(Path.Combine(settings.AdditionalBitmaps[i].Path), settings.UseColorContrast, settings.AdditionalBitmaps[i].UseDifferentColor, settings.AdditionalBitmaps[i].Color);
                if (settings.AddMultiSized)
                    WriteBmpMultiSize(Path.Combine(settings.AdditionalBitmaps[i].Path), settings.UseColorContrast, settings.AdditionalBitmaps[i].UseDifferentColor, settings.AdditionalBitmaps[i].Color, settings.MultipliedSize);
                WriteSvg(Path.Combine(settings.AdditionalBitmaps[i].Path), Color.White);
            }
        }

        public static void ReadCsv()
        {
            String[] lines = File.ReadAllLines(Path.Combine(settings.ModFolder, "map", "definition.csv"));
            for (int i = 1; i < lines.Length; i++)
            {
                provinces.Add(new Province(lines[i]));
            }
        }

        public static void FindColors()
        {
            using (Bitmap bitmap = new Bitmap(Path.Combine(settings.ModFolder, "map", "provinces.bmp")))
            {
                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        if (!Borders.ContainsKey(bitmap.GetPixel(i, j)))
                            Borders.Add(bitmap.GetPixel(i, j), new Rect(bitmap.Width, bitmap.Height, 0, 0));

                        Rect rect = Borders[bitmap.GetPixel(i, j)];
                        if (i > rect.MaxX)
                            rect.MaxX = i;
                        else if (i < rect.MinX)
                            rect.MinX = i;

                        if (j > rect.MaxY)
                            rect.MaxY = j;
                        else if (j < rect.MinY)
                            rect.MinY = j;
                    }
                }
            }

            foreach (Province province in provinces)
            {
                if (Borders.ContainsKey(province.ProvinceColor))
                {
                    province.FindTextPosition(Borders[province.ProvinceColor]);
                }
                else
                {
                    Console.WriteLine("Not found province {0}", province.Id);
                }
            }
        }

        public static void WriteBmp(string source, bool switchColor, bool useOwnColor, Color color)
        {
            Bitmap bitmap = new Bitmap(source);
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);

            var fontCollection = new PrivateFontCollection();
            fontCollection.AddFontFile("5x5_pixel.ttf");

            System.Drawing.Font font = new System.Drawing.Font(fontCollection.Families[0], 8, GraphicsUnit.Pixel);

            System.Drawing.Brush ownBrush = new SolidBrush(color);
            System.Drawing.Brush primary = new SolidBrush(settings.PrimaryColor);
            System.Drawing.Brush secondary = new SolidBrush(settings.SecondaryColor);

            graphics.SmoothingMode = SmoothingMode.None;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;

            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;

            foreach (Province province in provinces)
            {
                if (Borders.ContainsKey(province.ProvinceColor))
                {
                    if (useOwnColor)
                    {
                        graphics.DrawString(province.Id, font, ownBrush, province.TextCenter, format);
                    }

                    else
                    {
                        if (switchColor && province.ColorSum() > 500)
                            graphics.DrawString(province.Id, font, secondary, province.TextCenter, format);
                        else
                            graphics.DrawString(province.Id, font, primary, province.TextCenter, format);
                    }
                }
            }

            graphics.Save();
            bitmap.Save(Path.GetFileNameWithoutExtension(source) + "_filled.bmp");
        }

        public static void WriteBmpMultiSize(string source, bool switchColor, bool useOwnColor, Color color, int size)
        {
            Bitmap originalBitmap = new Bitmap(source);
            Bitmap bitmap = new Bitmap(originalBitmap, new System.Drawing.Size(originalBitmap.Width * size, originalBitmap.Height * size));
            //Bitmap bitmap = new Bitmap(originalBitmap.Width * size, originalBitmap.Height * size);
            //for (int i = 0; i < bitmap.Width; i++)
            //{
            //    for (int j = 0; j < bitmap.Height; j++)
            //    {
            //        bitmap.SetPixel(i, j, originalBitmap.GetPixel(i / size, j / size));
            //    }
            //}

            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);

            var fontCollection = new PrivateFontCollection();
            fontCollection.AddFontFile("5x5_pixel.ttf");

            System.Drawing.Font font = new System.Drawing.Font(fontCollection.Families[0], 8, GraphicsUnit.Pixel);


            System.Drawing.Brush ownBrush = new SolidBrush(color);
            System.Drawing.Brush primary = new SolidBrush(settings.PrimaryColor);
            System.Drawing.Brush secondary = new SolidBrush(settings.SecondaryColor);

            graphics.SmoothingMode = SmoothingMode.None;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;

            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;

            foreach (Province province in provinces)
            {
                if (Borders.ContainsKey(province.ProvinceColor))
                {
                    PointF center = new PointF(province.TextCenter.X * size, province.TextCenter.Y * size);

                    if (useOwnColor)
                    {
                        graphics.DrawString(province.Id, font, ownBrush, center, format);
                    }

                    else
                    {
                        if (switchColor && province.ColorSum() > 500)
                            graphics.DrawString(province.Id, font, secondary, center, format);
                        else
                            graphics.DrawString(province.Id, font, primary, center, format);
                    }
                }
            }

            graphics.Save();
            bitmap.Save(Path.GetFileNameWithoutExtension(source) + "_" + size + "_filled.bmp");
        }

        public static void WriteSvg(string source, Color color)
        {
            Bitmap bitmap = new Bitmap(source);
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);
            var raster = new VectSharp.MuPDFUtils.RasterImageFile(source);

            Document document = new Document();
            Page page = new Page(bitmap.Width, bitmap.Height);
            bitmap.Dispose();

            document.Pages.Add(page);
            page.Graphics.DrawRasterImage(new VectSharp.Point(0, 0), raster);
            
            foreach (Province province in provinces)
            {
                if (Borders.ContainsKey(province.ProvinceColor))
                {
                    var p = new VectSharp.GraphicsPath();
                    p.MoveTo((double)province.TextCenter.X-30, (double)province.TextCenter.Y);
                    p.LineTo((double)province.TextCenter.X+30, (double)province.TextCenter.Y);
                    page.Graphics.FillTextOnPath(p, province.Id, new VectSharp.Font(new VectSharp.FontFamily(VectSharp.FontFamily.StandardFontFamilies.TimesRoman), 12), Colour.FromRgb(color.R, color.G, color.B), anchor: TextAnchors.Center, textBaseline: TextBaselines.Middle);
                    //page.Graphics.FillText((double)province.TextCenter.X, (double)province.TextCenter.Y, province.Id, new VectSharp.Font(new VectSharp.FontFamily(VectSharp.FontFamily.StandardFontFamilies.TimesRoman), 12), Colour.FromRgb(255, 255, 255), textBaseline: TextBaselines.Middle);
                }
            }

            document.Pages[0].SaveAsSVG(Path.GetFileNameWithoutExtension(source) + "_filled.svg");
        }
    }
}
