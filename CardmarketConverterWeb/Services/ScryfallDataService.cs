using System.Text.Json;
using System.Text.Json.Serialization;
using CardmarketConverterWeb.Models;

namespace CardmarketConverterWeb.Services;

public class ScryfallDataService
{
    private static List<ScryfallCard>? _allCards;
    private static bool _isLoaded = false;
    private static readonly SemaphoreSlim _loadLock = new(1, 1);
    private static string _fileToLoad = "oracle-cards"; // Default
    
    private readonly HttpClient _httpClient;

    public ScryfallDataService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public void SetFileToLoad(string fileName)
    {
        // Remove .json extension if provided
        _fileToLoad = fileName.Replace(".json", "");
        
        // Clear existing data when switching files
        ClearData();
    }

    public void ClearData()
    {
        _allCards = null;
        _isLoaded = false;
    }

    public async Task<bool> LoadBulkDataAsync()
    {
        if (_isLoaded && _allCards != null)
            return true;

        await _loadLock.WaitAsync();
        try
        {
            // Double-check after acquiring lock
            if (_isLoaded && _allCards != null)
                return true;

            Console.WriteLine($"Starting to load Scryfall bulk data: {_fileToLoad}");
            
            // Try different possible filenames for the selected file
            var possibleFiles = new[]
            {
                $"Scryfall/{_fileToLoad}.json",
                $"Scryfall/{_fileToLoad}-20251205102358.json",
                $"Scryfall/{_fileToLoad}-20251206102358.json"
            };

            HttpResponseMessage? response = null;
            string? successfulFile = null;

            foreach (var file in possibleFiles)
            {
                try
                {
                    var testResponse = await _httpClient.GetAsync(file, HttpCompletionOption.ResponseHeadersRead);
                    if (testResponse.IsSuccessStatusCode)
                    {
                        response = testResponse;
                        successfulFile = file;
                        Console.WriteLine($"Found Scryfall data file: {file}");
                        break;
                    }
                }
                catch
                {
                    // Try next file
                    continue;
                }
            }

            if (response == null || !response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to load Scryfall bulk data file: {_fileToLoad}.json");
                Console.WriteLine("Please ensure the file is in wwwroot/Scryfall/");
                Console.WriteLine("Download from: https://scryfall.com/docs/api/bulk-data");
                Console.WriteLine($"Expected filename: {_fileToLoad}.json");
                return false;
            }

            Console.WriteLine($"Loading {successfulFile}, this may take a moment...");
            
            // Check content type and length
            var contentLength = response.Content.Headers.ContentLength;
            if (contentLength.HasValue)
            {
                Console.WriteLine($"File size: {contentLength.Value / 1024 / 1024}MB");
            }

            // Use stream-based deserialization for large files
            using var stream = await response.Content.ReadAsStreamAsync();
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            _allCards = await JsonSerializer.DeserializeAsync<List<ScryfallCard>>(stream, options);
            _isLoaded = _allCards != null && _allCards.Count > 0;

            if (_isLoaded)
            {
                Console.WriteLine($"Successfully loaded {_allCards?.Count ?? 0} cards from Scryfall bulk data");
            }
            else
            {
                Console.WriteLine("File loaded but contains no cards or is empty");
            }
            
            return _isLoaded;
        }
        catch (JsonException jsonEx)
        {
            Console.WriteLine($"JSON parsing error: {jsonEx.Message}");
            Console.WriteLine($"The file may be corrupted or too large for browser memory.");
            Console.WriteLine($"Try using Oracle Cards (~50MB) instead of All Cards (~200MB)");
            return false;
        }
        catch (OutOfMemoryException)
        {
            Console.WriteLine("Out of memory! The file is too large for the browser.");
            Console.WriteLine("Please download 'Oracle Cards' instead of 'All Cards'");
            Console.WriteLine("Oracle Cards: ~50MB, All Cards: ~200MB");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading Scryfall data: {ex.Message}");
            Console.WriteLine($"Exception type: {ex.GetType().Name}");
            return false;
        }
        finally
        {
            _loadLock.Release();
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

