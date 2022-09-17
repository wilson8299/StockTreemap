using StockTreemap.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockTreemap.Services
{
    public class RectangleService
    {
        public IEnumerable<SliceRectangle> GetRectangles(Slice slice, double width, double height)
        {
            double total = slice.Elements.Sum(x => x.Value);
            var area = new SliceRectangle { Slice = slice, Width = width, Height = height };

            foreach (var rect in _GetRectangles(area))
            {
                if (rect.X + rect.Width > area.Width)
                {
                    rect.Width = area.Width - rect.X;
                }

                if (rect.Y + rect.Height > area.Height)
                {
                    rect.Height = area.Height - rect.Y;
                }

                yield return rect;
            }
        }

        private IEnumerable<SliceRectangle> _GetRectangles(SliceRectangle sliceRectangle)
        {
            var isHorizontalSplit = sliceRectangle.Width >= sliceRectangle.Height;
            double currentPos = 0;
            foreach (var subSlice in sliceRectangle.Slice.SubSlices)
            {
                var subRect = new SliceRectangle { Slice = subSlice };
                double rectSize;

                if (isHorizontalSplit)
                {
                    rectSize = sliceRectangle.Width * subSlice.Size;
                    rectSize = Math.Round(rectSize);
                    subRect.X = sliceRectangle.X + currentPos;
                    subRect.Y = sliceRectangle.Y;
                    subRect.Width = rectSize;
                    subRect.Height = sliceRectangle.Height;
                }
                else
                {
                    rectSize = sliceRectangle.Height * subSlice.Size;
                    rectSize = Math.Round(rectSize);
                    subRect.X = sliceRectangle.X;
                    subRect.Y = sliceRectangle.Y + currentPos;
                    subRect.Width = sliceRectangle.Width;
                    subRect.Height = rectSize;
                }

                currentPos += rectSize;

                if (subSlice.Elements.Count() > 1)
                {
                    foreach (var item in _GetRectangles(subRect))
                        yield return item;
                }
                else if (subSlice.Elements.Count() == 1)
                    yield return subRect;
            }
        }

    }
}
