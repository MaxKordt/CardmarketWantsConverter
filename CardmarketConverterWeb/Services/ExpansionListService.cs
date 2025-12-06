using Microsoft.JSInterop;
using System.Text.Json;
using CardmarketConverterWeb.Models;

namespace CardmarketConverterWeb.Services;

public class ExpansionListService
{
    private readonly Dictionary<string, Expansion> _expansions = new();
    private readonly IJSRuntime _jsRuntime;
    private const string StorageKey = "cardmarket_expansions_v2";
    private bool _isInitialized = false;

    public event Action? OnExpansionsChanged;

    public ExpansionListService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized) return;
        
        try
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", StorageKey);
            if (!string.IsNullOrEmpty(json))
            {
                var expansions = JsonSerializer.Deserialize<List<Expansion>>(json);
                if (expansions != null)
                {
                    foreach (var expansion in expansions)
                    {
                        _expansions[expansion.Name] = expansion;
                    }
                }
            }
            else
            {
                // Try to migrate from old format
                var oldJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "cardmarket_expansions");
                if (!string.IsNullOrEmpty(oldJson))
                {
                    var oldExpansions = JsonSerializer.Deserialize<List<string>>(oldJson);
                    if (oldExpansions != null)
                    {
                        foreach (var name in oldExpansions)
                        {
                            _expansions[name] = new Expansion
                            {
                                Name = name,
                                FirstExtracted = DateTime.Now,
                                LastSeen = DateTime.Now,
                                TimesExtracted = 1
                            };
                        }
                        await SaveToStorageAsync();
                        // Clean up old storage
                        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "cardmarket_expansions");
                    }
                }
            }
        }
        catch
        {
            // If loading fails, start with empty list
        }
        
        _isInitialized = true;
    }

    public async Task<int> AddExpansionsAsync(List<string> newExpansionNames)
    {
        int addedCount = 0;
        var now = DateTime.Now;
        
        foreach (var name in newExpansionNames)
        {
            if (_expansions.ContainsKey(name))
            {
                // Update existing expansion
                _expansions[name].LastSeen = now;
                _expansions[name].TimesExtracted++;
            }
            else
            {
                // Add new expansion
                _expansions[name] = new Expansion
                {
                    Name = name,
                    FirstExtracted = now,
                    LastSeen = now,
                    TimesExtracted = 1
                };
                addedCount++;
            }
        }
        
        if (newExpansionNames.Count > 0)
        {
            await SaveToStorageAsync();
            OnExpansionsChanged?.Invoke();
        }
        
        return addedCount;
    }

    public async Task<int> AddExpansionsWithDataAsync(List<Expansion> newExpansions)
    {
        int addedCount = 0;
        var now = DateTime.Now;
        
        foreach (var expansion in newExpansions)
        {
            if (_expansions.ContainsKey(expansion.Name))
            {
                // Update existing expansion - merge new data
                var existing = _expansions[expansion.Name];
                existing.LastSeen = now;
                existing.TimesExtracted++;
                
                // Update fields if new data is available
                if (!string.IsNullOrEmpty(expansion.Url))
                    existing.Url = expansion.Url;
                if (!string.IsNullOrEmpty(expansion.SetCode))
                    existing.SetCode = expansion.SetCode;
                if (expansion.CardCount.HasValue)
                    existing.CardCount = expansion.CardCount;
                if (!string.IsNullOrEmpty(expansion.ReleaseDate))
                    existing.ReleaseDate = expansion.ReleaseDate;
                if (!string.IsNullOrEmpty(expansion.ImageUrl))
                    existing.ImageUrl = expansion.ImageUrl;
            }
            else
            {
                // Add new expansion
                expansion.FirstExtracted = now;
                expansion.LastSeen = now;
                expansion.TimesExtracted = 1;
                _expansions[expansion.Name] = expansion;
                addedCount++;
            }
        }
        
        if (newExpansions.Count > 0)
        {
            await SaveToStorageAsync();
            OnExpansionsChanged?.Invoke();
        }
        
        return addedCount;
    }

    public List<Expansion> GetAllExpansions()
    {
        return _expansions.Values.OrderBy(e => e.Name).ToList();
    }

    public List<string> GetAll()
    {
        return _expansions.Keys.OrderBy(n => n).ToList();
    }

    public int Count => _expansions.Count;

    public async Task RemoveAsync(string expansionName)
    {
        if (_expansions.Remove(expansionName))
        {
            await SaveToStorageAsync();
            OnExpansionsChanged?.Invoke();
        }
    }

    public async Task ClearAllAsync()
    {
        _expansions.Clear();
        await SaveToStorageAsync();
        OnExpansionsChanged?.Invoke();
    }

    public string ExportAsJson()
    {
        var expansions = GetAllExpansions();
        return JsonSerializer.Serialize(expansions, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
    }

    private async Task SaveToStorageAsync()
    {
        try
        {
            var expansions = _expansions.Values.ToList();
            var json = JsonSerializer.Serialize(expansions);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
        }
        catch
        {
            // Silently fail if storage is not available
        }
    }
}

