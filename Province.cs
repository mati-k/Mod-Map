using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModMap
{
    public class Province
    {
        public string Id { get; set; }
        public Color ProvinceColor { get; set; }
        public PointF TextCenter { get; set; }


        public Province(string line)
        {
            String[] lines = line.Split(";");

            this.Id = lines[0];
            this.ProvinceColor = Color.FromArgb(int.Parse(lines[1]), int.Parse(lines[2]), int.Parse(lines[3]));
        }

        public void FindTextPosition(Rect rect)
        {
            this.TextCenter = new PointF((rect.MinX + rect.MaxX) / 2, (rect.MinY + rect.MaxY) / 2);
        }
    }
}
