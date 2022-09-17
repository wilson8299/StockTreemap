using StockTreemap.Models;
using System.Collections.Generic;
using System.Linq;

namespace StockTreemap.Services
{
    public class SliceService
    {
        public Slice GetSlice(IEnumerable<Element> elements, double totalSize, double sliceWidth)
        {
            if (!elements.Any()) return null;
            if (elements.Count() == 1)
                return new Slice
                {
                    Elements = elements,
                    Size = totalSize,
                    SubSlices = new List<Slice> { new Slice { Size = 1, Elements = elements, SubSlices = null } }
                };

            var sliceResult = GetElementsForSlice(elements, sliceWidth);

            return new Slice
            {
                Elements = elements,
                Size = totalSize,
                SubSlices = new List<Slice>
                {
                   GetSlice(sliceResult.Elements, sliceResult.ElementsSize, sliceWidth),
                   GetSlice(sliceResult.RemainingElements, 1 - sliceResult.ElementsSize, sliceWidth)
                 }
            };
        }

        private SliceResult GetElementsForSlice(IEnumerable<Element> elements, double sliceWidth)
        {
            var elementsInSlice = new List<Element>();
            var remainingElements = new List<Element>();
            double current = 0;
            double total = elements.Sum(x => x.Value);

            foreach (var element in elements)
            {
                if (current > sliceWidth)
                    remainingElements.Add(element);
                else
                {
                    elementsInSlice.Add(element);
                    current += element.Value / total;
                }
            }

            return new SliceResult
            {
                Elements = elementsInSlice,
                ElementsSize = current,
                RemainingElements = remainingElements
            };
        }

    }
}
