using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShopGenerator.Storefront.RegexMatching
{
    public class RegexHelper
    {
        public static RegexMatch MatchRegex(string input)
        {
            var regex = new Regex(@"(?:CID_)(\d+|A_\d+)(?:_.+)");
            var match = regex.Match(input);

            if (match.Success)
            {
                return new RegexMatch { Id = match.Groups[1].Value };
            }
            return null;
        }
    }
}
