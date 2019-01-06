using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using HtmlAgilityPack;

namespace LettersCounter
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var document = DownloadWebsite();
                var arcticlesHeaders = SelectHeaders(document);
                ShowResults(arcticlesHeaders);
                CreateStoreDocuments();
                SaveData(arcticlesHeaders);
                Thread.Sleep(1000 * 60 * 10);
            }
        }

        private static void CreateStoreDocuments()
        {
            for (var i = 1; i < 6; i++)
            {
                if (!File.Exists(i + ".txt"))
                {
                    var file = File.Create(i + ".txt");
                    file.Close();
                }
            }
            var backFile = File.Create("backup.txt");
            backFile.Close();
        }

        private static void SaveData(List<string> headers)
        { 
            for (int i = 4; i >= 1; i--)
            {
                var fs = File.OpenWrite(i + ".txt");
                var sw = new StreamWriter(fs);

                if (i == 1)
                {
                    foreach (var header in headers)
                    {
                        sw.WriteLine(header);
                    }
                }
                else
                {
                    var fsPrevious = File.OpenRead(i - 1 + ".txt");
                    var sr = new StreamReader(fsPrevious);

                    var information = sr.ReadToEnd();
                    sw.WriteLine(information);

                    fsPrevious.Close();
                    sr.Close();
                }
                sw.Close();
                fs.Close();
            }
        }

        private static void ShowResults(List<string> arcticlesHeaders)
        {
            foreach (var title in arcticlesHeaders)
            {
                var cleanTitle = title.Replace(" ", "");

                var output = cleanTitle.GroupBy(letter => letter)
                    .OrderBy(letter => letter.Key)
                    .Where(grp => grp.Any())
                    .ToDictionary(grp => grp.Key, grp => grp.Count());

                Console.WriteLine(title);

                foreach (var record in output)
                    Console.WriteLine($"{record.Key}: {record.Value}");
            }
        }

        private static HtmlDocument DownloadWebsite()
        {
            var client = new WebClient();
            var document = new HtmlDocument();

            var content = client.DownloadString("https://www.finai.pl/blog");
            document.LoadHtml(content);

            return document;
        }

        private static List<string> SelectHeaders(HtmlDocument document)
        {
            var nodes = document.DocumentNode.SelectNodes("//h2").Take(20).ToList();

            var arcticlesHeaders = new List<string>();

            //There, because i don't know why C# html decoding doesnt work i add my own
            //decoding conditions. I will try find out why decoding doesn't work later.
            foreach (var node in nodes)
            {
                var title = WebUtility.HtmlDecode(node.InnerText);

                title = title.Replace("Ĺ\u0083", "Ń")
                    .Replace("Ĺ„", "ń")
                    .Replace("Ĺ‚", "ł")
                    .Replace("\r", "")
                    .Replace("  ", " ")
                    .Replace("\n", "");         

                arcticlesHeaders.Add(title);
            }

            return arcticlesHeaders;
        }

        
    }
}
