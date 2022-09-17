using System.Collections.Generic;

namespace StockTreemap.Models
{
    public class Slice
    {
        public double Size { get; set; }
        public IEnumerable<Element> Elements { get; set; }
        public IEnumerable<Slice> SubSlices { get; set; }
    }
}
