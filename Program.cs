using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using VectSharp;
using VectSharp.SVG;
using System.Drawing.Text;

namespace ModMap
{
    class Program
    {
        private static List<Province> provinces = new List<Province>();
        private static string modFolder;
        private const int textIndex = 0;

        private static Dictionary<Color, Rect> Borders = new Dictionary<Color, Rect>();


        static void Main(string[] args)
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            modFolder = File.ReadAllText("mod.txt");

            ReadCsv();
            FindColors();

            WriteBmp(Path.Combine(modFolder, "map", "provinces.bmp"), Color.White);
            WriteSvg(Path.Combine(modFolder, "map", "provinces.bmp"), Color.White);

            if (File.Exists("paths.txt"))
            {
                String[] files = File.ReadAllLines("paths.txt");
                for (int i = 1; i < files.Length; i++)
                {
                    if (String.IsNullOrWhiteSpace(files[i]))
                        continue;

                    string[] lineData = files[i].Split(";");
                    Color color = Color.FromArgb(int.Parse(lineData[1]), int.Parse(lineData[2]), int.Parse(lineData[3]));

                    WriteBmp(lineData[0], color);
                    WriteSvg(lineData[0], color);
                }
            }
        }

        public static void ReadCsv()
        {
            String[] lines = File.ReadAllLines(Path.Combine(modFolder, "map", "definition.csv"));
            for (int i = 1; i < lines.Length; i++)
            {
                provinces.Add(new Province(lines[i]));
            }
        }

        public static void FindColors()
        {
            using (Bitmap bitmap = new Bitmap(Path.Combine(modFolder, "map", "provinces.bmp")))
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

        public static void WriteBmp(string source, Color color)
        {
            Bitmap bitmap = new Bitmap(source);
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);

            var fontCollection = new PrivateFontCollection();
            fontCollection.AddFontFile("5x5_pixel.ttf");

            System.Drawing.Font font = new System.Drawing.Font(fontCollection.Families[0], 8, GraphicsUnit.Pixel);
            System.Drawing.Brush brush = new SolidBrush(color);

            graphics.SmoothingMode = SmoothingMode.None;
            graphics.CompositingQuality = CompositingQuality.HighQuality;

            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;

            foreach (Province province in provinces)
            {
                if (Borders.ContainsKey(province.ProvinceColor))
                {
                    graphics.DrawString(province.Id, font, brush, province.TextCenter, format);
                }
            }

            graphics.Save();
            bitmap.Save(Path.GetFileNameWithoutExtension(source) + "_filled.bmp");
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
