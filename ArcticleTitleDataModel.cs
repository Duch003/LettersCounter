using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LettersCounter
{
    public class ArcticleTitleDataModel
    {
        public string Title { get; set; } = "";
        public Dictionary<char, int> Letters { get; set; }
    

        public ArcticleTitleDataModel(string title)
        {
            var cleanTitle = title.Replace("  ", " ").Replace("\r", "").Replace("\n", "");

            Title = cleanTitle;
            Letters = cleanTitle.GroupBy(letter => letter)
                .OrderBy(letter => letter.Key)
                .Where(grp => grp.Any())
                .ToDictionary(grp => grp.Key, grp => grp.Count());
        }
    }
}
