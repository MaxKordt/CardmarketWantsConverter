namespace CardmarketConverterWeb.Models;

public class Expansion
{
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? SetCode { get; set; }
    public int? CardCount { get; set; }
    public string? ReleaseDate { get; set; }
    public string? ImageUrl { get; set; }
}

