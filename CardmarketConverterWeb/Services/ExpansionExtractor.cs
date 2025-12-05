using System.Text.RegularExpressions;

namespace CardmarketConverterWeb.Services;

public class ExpansionExtractor
{
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

