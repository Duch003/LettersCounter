using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
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
                var rawTitles = GatherArcticlesTitles();
                var titles = PrepareData(rawTitles);

                foreach (var title in titles)
                {
                    Console.WriteLine(title.Title);
                    foreach (var item in title.Letters)
                    {
                        Console.Write($"Letter {item.Key}: {item.Value}".PadRight(15));
                    }
                    Console.WriteLine();
                }
                Console.ReadLine();
            }

            
        }

        private static List<ArcticleTitleDataModel> PrepareData(List<string> input)
        {
            var output = new List<ArcticleTitleDataModel>();

            foreach (var item in input)
            {
                var arctileTitle = DecodeUnicode(DeocdeUTF8(item));
                output.Add(new ArcticleTitleDataModel(arctileTitle));
            }

            return output;
        }

        private static List<string> GatherArcticlesTitles()
        {
            var arcticles = new List<string>();
            var arcticlesLeft = 20;
            var page = 1;

            do
            {
                var site = DownloadWebsite(page);
                var titles = GatherArcticlesTitles(site, arcticlesLeft);

                arcticles = MergeLists(arcticles, titles);
                arcticlesLeft -= titles.Count;
                page++;
            } while (arcticlesLeft > 0);

            return arcticles;
        }

        private static HtmlDocument DownloadWebsite(int page)
        {
            var client = new WebClient();
            var site = new HtmlDocument();

            site.LoadHtml(client.DownloadString($"https://www.finai.pl/blog?page={page}"));

            return site;
        }

        private static List<string> GatherArcticlesTitles(HtmlDocument site, int amount)
        {
            var output = new List<string>();
            var nodes = site.DocumentNode.SelectNodes("//h2").Take(amount);

            foreach (var node in nodes)
            {
                output.Add(node.InnerText);
            }

            return output;
        }

        private static string DeocdeUTF8(string input)
        {
            var utf8bytes = Encoding.UTF8.GetBytes(System.Text.RegularExpressions.Regex.Unescape(@input));
            var decoder = Encoding.UTF8.GetDecoder();
            var decodedLetters = new char[decoder.GetCharCount(utf8bytes, 0, utf8bytes.Length)];

            decoder.GetChars(utf8bytes, 0, utf8bytes.Length, decodedLetters, 0);

            var decoded = new string(decodedLetters);

            decoded = WebUtility.HtmlDecode(@decoded);
            decoded = System.Text.RegularExpressions.Regex.Unescape(@decoded);
            return decoded;
        }

        private static string DecodeUnicode(string input)
        {
            var unicodeBytes = Encoding.Unicode.GetBytes(System.Text.RegularExpressions.Regex.Unescape(@input));
            var decoder = Encoding.Unicode.GetDecoder();
            var decodedLetters = new char[decoder.GetCharCount(unicodeBytes, 0, unicodeBytes.Length)];

            decoder.GetChars(unicodeBytes, 0, unicodeBytes.Length, decodedLetters, 0);

            var decoded = new string(decodedLetters);

            decoded = WebUtility.HtmlDecode(@decoded);
            decoded = System.Text.RegularExpressions.Regex.Unescape(@decoded);
            return decoded;
        }

        private static List<string> MergeLists(List<string> list1, List<string> list2)
        {
            foreach (var item in list2)
            {
                list1.Add(item);
            }

            return list1;
        }
        //private static void CreateStoreDocuments()
        //{
        //    for (var i = 1; i < 6; i++)
        //    {
        //        if (!File.Exists(i + ".txt"))
        //        {
        //            var file = File.Create(i + ".txt");
        //            file.Close();
        //        }
        //    }
        //    var backFile = File.Create("backup.txt");
        //    backFile.Close();
        //}

        //private static void SaveData(List<string> headers)
        //{ 
        //    for (int i = 4; i >= 1; i--)
        //    {
        //        var fs = File.OpenWrite(i + ".txt");
        //        var sw = new StreamWriter(fs);

        //        if (i == 1)
        //        {
        //            foreach (var header in headers)
        //            {
        //                sw.WriteLine(header);
        //            }
        //        }
        //        else
        //        {
        //            var fsPrevious = File.OpenRead(i - 1 + ".txt");
        //            var sr = new StreamReader(fsPrevious);

        //            var information = sr.ReadToEnd();
        //            sw.WriteLine(information);

        //            fsPrevious.Close();
        //            sr.Close();
        //        }
        //        sw.Close();
        //        fs.Close();
        //    }
        //}

        //private static void ShowResults(List<string> arcticlesHeaders)
        //{
        //    foreach (var title in arcticlesHeaders)
        //    {
        //        

        //        Console.WriteLine(title);

        //        foreach (var record in output)
        //            Console.WriteLine($"{record.Key}: {record.Value}");
        //    }
        //}

        //private static HtmlDocument DownloadWebsite(int page)
        //{
        //    var client = new WebClient();
        //    var document = new HtmlDocument();

        //    var content = client.DownloadString($"https://www.finai.pl/blog?page={ page }");
        //    document.LoadHtml(content);

        //    return document;
        //}

        

        //private static List<string> SelectHeaders(HtmlDocument document)
        //{
        //    var nodes = document.DocumentNode.SelectNodes("//h2").Take(20).ToList();

        //    var arcticlesHeaders = new List<string>();

        //    //There, because i don't know why C# html decoding doesnt work i add my own
        //    //decoding conditions. I will try find out why decoding doesn't work later.
        //    foreach (var node in nodes)
        //    {
        //        var title = WebUtility.HtmlDecode(node.InnerText);

        //        title = title.Replace("Ĺ\u0083", "Ń")
        //            .Replace("Ĺ„", "ń")
        //            .Replace("Ĺ‚", "ł")
        //            .Replace("\r", "")
        //            .Replace("  ", " ")
        //            .Replace("\n", "");         

        //        arcticlesHeaders.Add(title);
        //    }

        //    return arcticlesHeaders;
        //}

        
    }
}
