namespace StockTreemap.Models
{
    public class Stock
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double Change { get; set; }
        public double ChangePercent { get; set; }
        public string CategoryId { get; set; }
    }
}
