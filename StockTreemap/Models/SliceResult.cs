using System.Collections.Generic;

namespace StockTreemap.Models
{
    public class SliceResult
    {
        public IEnumerable<Element> Elements { get; set; }
        public IEnumerable<Element> RemainingElements { get; set; }
        public double ElementsSize { get; set; }
    }
}
