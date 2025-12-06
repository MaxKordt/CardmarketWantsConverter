using System.Text.RegularExpressions;
using CardmarketConverterWeb.Models;

namespace CardmarketConverterWeb.Services;

public class ExpansionExtractor
{
    public List<Expansion> ExtractExpansions(string content)
    {
        var expansions = new List<Expansion>();
        
        // Match each expansion-row div block
        var blockPattern = @"<div[^>]*class=""[^""]*expansion-row[^""]*""[^>]*>(.*?)</div>\s*</div>\s*</div>\s*</div>";
        var blocks = Regex.Matches(content, blockPattern, RegexOptions.Singleline);
        
        foreach (Match block in blocks)
        {
            var blockHtml = block.Value;
            var expansion = new Expansion();
            
            // Extract name from data-local-name
            var nameMatch = Regex.Match(blockHtml, @"data-local-name=""([^""]+)""");
            if (nameMatch.Success)
            {
                expansion.Name = nameMatch.Groups[1].Value;
            }
            else
            {
                continue; // Skip if no name found
            }
            
            // Extract URL from data-url
            var urlMatch = Regex.Match(blockHtml, @"data-url=""([^""]+)""");
            if (urlMatch.Success)
            {
                expansion.Url = urlMatch.Groups[1].Value;
                
                // Extract set code from URL (last part after /)
                var urlParts = expansion.Url.Split('/');
                expansion.SetCode = urlParts[urlParts.Length - 1];
            }
            
            // Extract card count (e.g., "75 Karten")
            var cardCountMatch = Regex.Match(blockHtml, @">(\d+)\s+Karten<");
            if (cardCountMatch.Success && int.TryParse(cardCountMatch.Groups[1].Value, out int cardCount))
            {
                expansion.CardCount = cardCount;
            }
            
            // Extract release date (e.g., "5. Dezember 2025")
            var dateMatch = Regex.Match(blockHtml, @">\s*(\d+\.\s+\w+\s+\d{4})\s*<");
            if (dateMatch.Success)
            {
                expansion.ReleaseDate = dateMatch.Groups[1].Value;
            }
            
            // Extract image URL from data-echo
            var imageMatch = Regex.Match(blockHtml, @"data-echo=""([^""]+)""");
            if (imageMatch.Success)
            {
                expansion.ImageUrl = imageMatch.Groups[1].Value;
            }
            
            expansions.Add(expansion);
        }
        
        return expansions;
    }
    
    // Legacy method for backward compatibility
    public List<string> ExtractExpansionNames(string content)
    {
        var names = new List<string>();
        
        var pattern = @"data-local-name=""([^""]+)""";
        var matches = Regex.Matches(content, pattern);
        
        foreach (Match match in matches)
        {
            if (match.Groups.Count > 1)
            {
                names.Add(match.Groups[1].Value);
            }
        }
        
        return names;
    }
    
    public string ConvertListToText(List<string> names)
    {
        return string.Join(Environment.NewLine, names);
    }
}

