using StockTreemap.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockTreemap.Services
{
    public class TreemapService
    {
        private SliceService _stockSlice = new SliceService();
        private RectangleService _stockRectangle = new RectangleService();
        private List<Treemap> _stockTreemaps = new List<Treemap>();

        public List<Treemap> Create(int width, int height, List<Category> _stockCategory, List<Stock> _stockList)
        {
            var elements = new List<Element>();
            elements = _stockCategory
                .Select(x => new Element { Name = x.Name, CategoryId = x.CategoryId, Value = Math.Abs(x.ChangePercent) + 3, ChangePercent = x.ChangePercent })
                .OrderByDescending(x => x.Value)
                .ToList();


            double MinSliceRatio = 0.35;
            var slice = _stockSlice.GetSlice(elements, 1, MinSliceRatio);
            var rectangles = _stockRectangle.GetRectangles(slice, width, height).ToList();

            foreach (var item in rectangles)
            {
                var category = item.Slice.Elements.First().CategoryId;
                var stockList = _stockList.Where(sd => sd.CategoryId == category).ToList();
                var elementsList = stockList
               .Select(x => new Element { Value = Math.Abs(x.ChangePercent) + 5, Name = x.Name, Symbol = x.Symbol, Price = x.Price, Change = x.Change, ChangePercent = x.ChangePercent })
               .OrderByDescending(x => x.ChangePercent)
               .OrderByDescending(x => x.Value)
               .ToList();

                var sliceList = _stockSlice.GetSlice(elementsList, 1, MinSliceRatio);
                var stockRectangles = _stockRectangle.GetRectangles(sliceList, item.Width - 4, item.Height - 30).ToList();

                var treemapDetail = new List<TreemapDetail>();
                foreach (var detailItem in stockRectangles)
                {
                    treemapDetail.Add(new TreemapDetail
                    {
                        Width = detailItem.Width,
                        Height = detailItem.Height,
                        X = detailItem.X,
                        Y = detailItem.Y,
                        Symbol = detailItem.Slice.Elements.First().Symbol,
                        Name = detailItem.Slice.Elements.First().Name,
                        Price = detailItem.Slice.Elements.First().Price,
                        Change = detailItem.Slice.Elements.First().Change,
                        ChangePercent = detailItem.Slice.Elements.First().ChangePercent,
                        CategoryId = item.Slice.Elements.First().CategoryId,
                    });
                }

                _stockTreemaps.Add(new Treemap
                {
                    Width = item.Width,
                    Height = item.Height,
                    X = item.X,
                    Y = item.Y,
                    Name = item.Slice.Elements.First().Name,
                    ChangePercent = item.Slice.Elements.First().ChangePercent,
                    CategoryId = item.Slice.Elements.First().CategoryId,
                    StockTreemapDetailList = treemapDetail
                });
            }
            return _stockTreemaps;
        }
    }
}
