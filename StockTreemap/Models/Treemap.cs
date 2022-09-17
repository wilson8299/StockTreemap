using System.Collections.Generic;

namespace StockTreemap.Models
{
    public class Treemap : Category
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public List<TreemapDetail> StockTreemapDetailList { get; set; }
    }
}
