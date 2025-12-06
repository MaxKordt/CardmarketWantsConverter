using System.Text.Json;
using System.Text.Json.Serialization;
using CardmarketConverterWeb.Models;

namespace CardmarketConverterWeb.Services;

public class ScryfallDataService
{
    private List<ScryfallCard>? _allCards;
    private readonly HttpClient _httpClient;
    private bool _isLoaded = false;

    public ScryfallDataService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> LoadBulkDataAsync()
    {
        if (_isLoaded && _allCards != null)
            return true;

        try
        {
            // Try to load from local Scryfall directory
            var response = await _httpClient.GetAsync("Scryfall/all-cards-20251205102358.json");
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed to load Scryfall bulk data file");
                return false;
            }

            var json = await response.Content.ReadAsStringAsync();
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            _allCards = JsonSerializer.Deserialize<List<ScryfallCard>>(json, options);
            _isLoaded = _allCards != null;

            Console.WriteLine($"Loaded {_allCards?.Count ?? 0} cards from Scryfall bulk data");
            return _isLoaded;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading Scryfall data: {ex.Message}");
            return false;
        }
    }

    public List<Card> GetCardsForSet(string setCode)
    {
        if (_allCards == null || !_isLoaded)
            return new List<Card>();

        var normalizedSetCode = setCode.ToLowerInvariant();
        
        var scryfallCards = _allCards
            .Where(c => c.Set?.ToLowerInvariant() == normalizedSetCode)
            .OrderBy(c => c.CollectorNumber)
            .ToList();

        return scryfallCards.Select(sc => new Card
        {
            Name = sc.Name ?? string.Empty,
            SetCode = sc.Set,
            SetName = sc.SetName,
            CollectorNumber = sc.CollectorNumber,
            Rarity = sc.Rarity,
            ManaCost = sc.ManaCost,
            TypeLine = sc.TypeLine,
            OracleText = sc.OracleText,
            Power = sc.Power,
            Toughness = sc.Toughness,
            Colors = sc.Colors,
            ColorIdentity = sc.ColorIdentity,
            ScryfallUri = sc.ScryfallUri,
            ImageUris = sc.ImageUris != null ? new CardImageUris
            {
                Small = sc.ImageUris.Small,
                Normal = sc.ImageUris.Normal,
                Large = sc.ImageUris.Large,
                Png = sc.ImageUris.Png,
                ArtCrop = sc.ImageUris.ArtCrop,
                BorderCrop = sc.ImageUris.BorderCrop
            } : null,
            Prices = sc.Prices != null ? new CardPrices
            {
                Usd = sc.Prices.Usd,
                UsdFoil = sc.Prices.UsdFoil,
                Eur = sc.Prices.Eur,
                EurFoil = sc.Prices.EurFoil,
                Tix = sc.Prices.Tix
            } : null
        }).ToList();
    }

    public async Task<List<string>> GetAvailableSetsAsync()
    {
        if (!_isLoaded)
            await LoadBulkDataAsync();

        if (_allCards == null)
            return new List<string>();

        return _allCards
            .Where(c => !string.IsNullOrEmpty(c.Set))
            .Select(c => c.Set!)
            .Distinct()
            .OrderBy(s => s)
            .ToList();
    }
}

// Internal Scryfall JSON structure
internal class ScryfallCard
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("set")]
    public string? Set { get; set; }

    [JsonPropertyName("set_name")]
    public string? SetName { get; set; }

    [JsonPropertyName("collector_number")]
    public string? CollectorNumber { get; set; }

    [JsonPropertyName("rarity")]
    public string? Rarity { get; set; }

    [JsonPropertyName("mana_cost")]
    public string? ManaCost { get; set; }

    [JsonPropertyName("type_line")]
    public string? TypeLine { get; set; }

    [JsonPropertyName("oracle_text")]
    public string? OracleText { get; set; }

    [JsonPropertyName("power")]
    public string? Power { get; set; }

    [JsonPropertyName("toughness")]
    public string? Toughness { get; set; }

    [JsonPropertyName("colors")]
    public List<string>? Colors { get; set; }

    [JsonPropertyName("color_identity")]
    public List<string>? ColorIdentity { get; set; }

    [JsonPropertyName("image_uris")]
    public ScryfallImageUris? ImageUris { get; set; }

    [JsonPropertyName("prices")]
    public ScryfallPrices? Prices { get; set; }

    [JsonPropertyName("scryfall_uri")]
    public string? ScryfallUri { get; set; }
}

internal class ScryfallImageUris
{
    [JsonPropertyName("small")]
    public string? Small { get; set; }

    [JsonPropertyName("normal")]
    public string? Normal { get; set; }

    [JsonPropertyName("large")]
    public string? Large { get; set; }

    [JsonPropertyName("png")]
    public string? Png { get; set; }

    [JsonPropertyName("art_crop")]
    public string? ArtCrop { get; set; }

    [JsonPropertyName("border_crop")]
    public string? BorderCrop { get; set; }
}

internal class ScryfallPrices
{
    [JsonPropertyName("usd")]
    public string? Usd { get; set; }

    [JsonPropertyName("usd_foil")]
    public string? UsdFoil { get; set; }

    [JsonPropertyName("eur")]
    public string? Eur { get; set; }

    [JsonPropertyName("eur_foil")]
    public string? EurFoil { get; set; }

    [JsonPropertyName("tix")]
    public string? Tix { get; set; }
}

