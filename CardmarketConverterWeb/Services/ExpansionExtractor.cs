using System.Text.RegularExpressions;
using CardmarketConverterWeb.Models;

namespace CardmarketConverterWeb.Services;

public class ExpansionExtractor
{
    public List<Expansion> ExtractExpansions(string content)
    {
        var expansions = new List<Expansion>();
        
        // Find all divs with data-url and data-local-name attributes
        // Note: data-url comes BEFORE data-local-name in the HTML
        var pattern = @"<div[^>]*data-url=""([^""]+)""[^>]*data-local-name=""([^""]+)""[^>]*>";
        var matches = Regex.Matches(content, pattern);
        
        foreach (Match match in matches)
        {
            var expansion = new Expansion();
            
            // Extract URL from capture group 1
            expansion.Url = match.Groups[1].Value;
            
            // Extract name from capture group 2
            expansion.Name = match.Groups[2].Value;
            
            // Extract set code from URL (last part after /)
            var urlParts = expansion.Url.Split('/');
            expansion.SetCode = urlParts[urlParts.Length - 1];
            
            // Get a small chunk of content after this match to extract other fields
            // Only look ahead 500 characters to keep it fast
            int startPos = match.Index;
            int length = Math.Min(500, content.Length - startPos);
            string blockContent = content.Substring(startPos, length);
            
            // Extract card count (e.g., "75 Karten")
            var cardCountMatch = Regex.Match(blockContent, @">(\d+)\s+Karten<");
            if (cardCountMatch.Success && int.TryParse(cardCountMatch.Groups[1].Value, out int cardCount))
            {
                expansion.CardCount = cardCount;
            }
            
            // Extract release date (e.g., "5. Dezember 2025")
            var dateMatch = Regex.Match(blockContent, @">\s*(\d+\.\s+\w+\s+\d{4})\s*<");
            if (dateMatch.Success)
            {
                expansion.ReleaseDate = dateMatch.Groups[1].Value;
            }
            
            // Extract image URL from data-echo
            var imageMatch = Regex.Match(blockContent, @"data-echo=""([^""]+)""");
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

