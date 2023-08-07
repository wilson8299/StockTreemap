using Newtonsoft.Json;
using StockTreemap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StockTreemap.Services
{
    public class CrawlerService
    {
        public async Task<Tuple<List<Category>, List<Stock>>> GetStockInfo()
        {
            var price = await SendRequest("https://www.wantgoo.com/investrue/all-quote-info");
            var info = await SendRequest("https://www.wantgoo.com/investrue/all-alive");

            var stockCategory = price
                .Where(p => p.id.Value.Substring(0, 1) == "^")
                .Select(p => new Category()
                {
                    CategoryId = p.id.Value,
                    Name = info.Where(i => i.id.Value == p.id.Value).Select(i => i.name.Value).FirstOrDefault(),
                    ChangePercent = ((p.close.Value - p.flat.Value) / p.flat.Value * 100)
                }).ToList();
 
            var stockList = price
                .Where(p => p.id.Value.Substring(0, 1) != "#")
                .Select(p => new Stock()
                {
                    Symbol = p.id.Value,
                    Name = info.Where(i => i.id.Value == p.id.Value).Select(i => i.name.Value).FirstOrDefault(),
                    CategoryId = info.Where(i => i.id.Value == p.id.Value).Select(i => i.industries.Count != 0 ? i.industries[0].id.Value : null).FirstOrDefault(),
                    Price = p.close.Value,
                    Change = (p.close.Value - p.flat.Value),
                    ChangePercent = ((p.close.Value - p.flat.Value) / p.flat.Value * 100)
                }).ToList();

            stockCategory = stockCategory.Where(c => stockList.Any(s => s.CategoryId == c.CategoryId)).ToList();

            return Tuple.Create(stockCategory, stockList);
        }

        private async Task<List<dynamic>> SendRequest(string url)
        {
            var response = await GetResponse(url);
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<dynamic>>(result);
        }

        private async Task<HttpResponseMessage> GetResponse(string url)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.101 Safari/537.36");
                var response = await client.SendAsync(request);
                return response;
            }
        }
    }
}
