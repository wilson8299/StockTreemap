using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StockTreemap.Services;

namespace StockTreemap
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var width = 7680;
            var height = 4320;
            var imageName = $"stock_treemap_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}";
            var savePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var title = @"
 ____ _____ ___   ____ _  __  _____ ____  _____ _____ __  __    _    ____  
/ ___|_   _/ _ \ / ___| |/ / |_   _|  _ \| ____| ____|  \/  |  / \  |  _ \ 
\___ \ | || | | | |   | ' /    | | | |_) |  _| |  _| | |\/| | / _ \ | |_) |
 ___) || || |_| | |___| . \    | | |  _ <| |___| |___| |  | |/ ___ \|  __/ 
|____/ |_| \___/ \____|_|\_\   |_| |_| \_\_____|_____|_|  |_/_/   \_\_|    
            ";

            Console.WriteLine(title);
            Console.WriteLine("解析度過低會導致圖片文字模糊，寬高請大於 2000");
            Console.WriteLine();
            width = Convert.ToInt32(ReadLine.Read("輸入圖片寬度 (7680) : ", "7680"));
            height = Convert.ToInt32(ReadLine.Read("輸入圖片高度 (4320) : ", "4320"));
            imageName = ReadLine.Read($"圖片名稱 ({imageName}) : ", imageName) + ".png";
            savePath = Path.TrimEndingDirectorySeparator(ReadLine.Read($"儲存路徑 ({savePath}): ", savePath)) + Path.DirectorySeparatorChar;

            AnimateFrames(new string[] { "=>   ", "==>  ", "===> ", "====>" });

            var crawlerService =  new CrawlerService();
            var treemapService = new TreemapService();

            var crawler = await crawlerService.GetStockInfo();
            var stockCategory = crawler.Item1;
            var stockList = crawler.Item2;
            var treemap = treemapService.Create(width, height, stockCategory, stockList);
            new RenderService().CreateImg(width, height, imageName, savePath, treemap);

            _active = false;
            Console.WriteLine("");
            Console.WriteLine("完成");
            Console.ReadKey();
        }

        private static bool _active = true;
        private static void AnimateFrames(string[] frames)
        {
            Task.Run(() =>
            {
                Console.WriteLine("處理中");
                Console.CursorVisible = false;
                while (_active)
                {
                    foreach (var frame in frames)
                    {
                        Console.SetCursorPosition(10, Console.CursorTop - 1);
                        Console.WriteLine (frame);
                        Thread.Sleep(300);
                    }
                }
                Console.CursorVisible = true;
            });
        }
    }
}
