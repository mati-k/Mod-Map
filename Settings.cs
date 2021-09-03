using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModMap
{
    public class Settings
    {
        public String ModFolder { get; set; }
        public bool AddMultiSized { get; set; }
        public int MultipliedSize { get; set; }
        public Color PrimaryColor { get; set; }
        public bool UseColorContrast { get; set; }
        public int ColorContrastThreshold { get; set; }
        public Color SecondaryColor { get; set; }
        public List<AdditionalBitmap> AdditionalBitmaps { get; set; } = new List<AdditionalBitmap>();
    }
}
