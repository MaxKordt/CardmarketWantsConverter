namespace CardmarketConverterWeb.Models;

public class Expansion
{
    public string Name { get; set; } = string.Empty;
    public DateTime FirstExtracted { get; set; }
    public DateTime LastSeen { get; set; }
    public int TimesExtracted { get; set; } = 1;
    
    // Future properties can be added here, such as:
    // public string? SetCode { get; set; }
    // public DateTime? ReleaseDate { get; set; }
    // public int? CardCount { get; set; }
    // public string? Type { get; set; }
}

