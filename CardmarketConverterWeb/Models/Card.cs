namespace CardmarketConverterWeb.Models;

public class Card
{
    public string Name { get; set; } = string.Empty;
    public string? SetCode { get; set; }
    public string? SetName { get; set; }
    public string? CollectorNumber { get; set; }
    public string? Rarity { get; set; }
    public string? ManaCost { get; set; }
    public string? TypeLine { get; set; }
    public string? OracleText { get; set; }
    public string? Power { get; set; }
    public string? Toughness { get; set; }
    public List<string>? Colors { get; set; }
    public List<string>? ColorIdentity { get; set; }
    public CardImageUris? ImageUris { get; set; }
    public CardPrices? Prices { get; set; }
    public string? ScryfallUri { get; set; }
}

public class CardImageUris
{
    public string? Small { get; set; }
    public string? Normal { get; set; }
    public string? Large { get; set; }
    public string? Png { get; set; }
    public string? ArtCrop { get; set; }
    public string? BorderCrop { get; set; }
}

public class CardPrices
{
    public string? Usd { get; set; }
    public string? UsdFoil { get; set; }
    public string? Eur { get; set; }
    public string? EurFoil { get; set; }
    public string? Tix { get; set; }
}

