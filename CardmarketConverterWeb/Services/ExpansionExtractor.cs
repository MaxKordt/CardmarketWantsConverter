using System.Text.RegularExpressions;
using CardmarketConverterWeb.Models;

namespace CardmarketConverterWeb.Services;

public class ExpansionExtractor
{
    public List<Expansion> ExtractExpansions(string content)
    {
        var expansions = new List<Expansion>();
        
        // Find all positions where expansion divs start
        var startPattern = @"<div[^>]*data-url=""([^""]+)""[^>]*data-local-name=""([^""]+)""[^>]*>";
        var matches = Regex.Matches(content, startPattern);
        
        foreach (Match match in matches)
        {
            var expansion = new Expansion();
            
            // Extract URL and Name from the opening tag
            expansion.Url = match.Groups[1].Value;
            expansion.Name = match.Groups[2].Value;
            
            // Extract set code from URL
            var urlParts = expansion.Url.Split('/');
            expansion.SetCode = urlParts[urlParts.Length - 1];
            
            // Find the complete expansion block by matching div tags
            string blockContent = ExtractCompleteBlock(content, match.Index);
            
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
    
    /// <summary>
    /// Extracts the complete expansion block by finding the matching closing div tag.
    /// Starts at the position of the opening div and finds its matching closing tag.
    /// </summary>
    private string ExtractCompleteBlock(string content, int startPosition)
    {
        int openDivCount = 0;
        int currentPos = startPosition;
        
        // Find the end of the opening tag first
        while (currentPos < content.Length)
        {
            if (content[currentPos] == '>')
            {
                openDivCount = 1; // We've found the opening tag
                currentPos++;
                break;
            }
            currentPos++;
        }
        
        // Now find the matching closing tag
        while (currentPos < content.Length && openDivCount > 0)
        {
            // Check for opening div tags
            if (currentPos + 4 < content.Length && 
                content.Substring(currentPos, 4) == "<div")
            {
                // Make sure it's a proper tag (followed by space or >)
                if (currentPos + 4 < content.Length && 
                    (content[currentPos + 4] == ' ' || content[currentPos + 4] == '>'))
                {
                    openDivCount++;
                    currentPos += 4;
                    continue;
                }
            }
            
            // Check for closing div tags
            if (currentPos + 6 < content.Length && 
                content.Substring(currentPos, 6) == "</div>")
            {
                openDivCount--;
                if (openDivCount == 0)
                {
                    // Found the matching closing tag
                    currentPos += 6;
                    break;
                }
                currentPos += 6;
                continue;
            }
            
            currentPos++;
        }
        
        // Extract the complete block
        int blockLength = currentPos - startPosition;
        if (blockLength > 0 && startPosition + blockLength <= content.Length)
        {
            return content.Substring(startPosition, blockLength);
        }
        
        // Fallback to a reasonable size if something went wrong
        int fallbackLength = Math.Min(2000, content.Length - startPosition);
        return content.Substring(startPosition, fallbackLength);
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

