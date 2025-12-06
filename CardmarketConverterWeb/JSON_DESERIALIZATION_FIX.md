# JSON Deserialization Fix ✅

## Problem

After restructuring the Scryfall Sets page, no sets were being displayed. The API was returning data, but it wasn't being deserialized properly.

## Root Cause

**Property Name Mismatch:**

Scryfall API uses `snake_case`:
```json
{
  "released_at": "2025-02-14",
  "card_count": 271,
  "set_type": "expansion"
}
```

C# models used `PascalCase` without mapping:
```csharp
public string? ReleasedAt { get; set; }  // Doesn't match "released_at"
public int CardCount { get; set; }        // Doesn't match "card_count"
```

**Result**: JSON deserializer couldn't map the fields, so all properties were empty/default values.

## Solution

Added `[JsonPropertyName]` attributes to map snake_case to PascalCase:

```csharp
private class ScryfallSet
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("released_at")]  // Maps to "released_at" in JSON
    public string? ReleasedAt { get; set; }
    
    [JsonPropertyName("card_count")]   // Maps to "card_count" in JSON
    public int CardCount { get; set; }
    
    [JsonPropertyName("set_type")]     // Maps to "set_type" in JSON
    public string? SetType { get; set; }
    
    [JsonPropertyName("scryfall_uri")] // Maps to "scryfall_uri" in JSON
    public string? ScryfallUri { get; set; }
    
    [JsonPropertyName("icon_svg_uri")] // Maps to "icon_svg_uri" in JSON
    public string? IconSvgUri { get; set; }
}
```

## How It Works

### Without Attributes (BROKEN):
```csharp
// API returns: { "released_at": "2025-02-14" }
public string? ReleasedAt { get; set; }  // Stays null - no match!
```

### With Attributes (FIXED):
```csharp
// API returns: { "released_at": "2025-02-14" }
[JsonPropertyName("released_at")]
public string? ReleasedAt { get; set; }  // Gets value "2025-02-14" ✓
```

## Testing

After the fix:

1. ✅ Navigate to Scryfall Sets page
2. ✅ Sets load from API successfully
3. ✅ All properties populated:
   - Set names displayed
   - Release dates shown
   - Card counts visible
   - Set types present
4. ✅ Year grouping works correctly
5. ✅ Collapsible sections functional

## Build Status

✅ **Successful** (0 errors, 0 warnings)

## Example Data Flow

### API Response:
```json
{
  "data": [
    {
      "code": "tmt",
      "name": "Teenage Mutant Ninja Turtles",
      "released_at": "2026-03-06",
      "card_count": 20,
      "set_type": "expansion",
      "scryfall_uri": "https://scryfall.com/sets/tmt"
    }
  ]
}
```

### Deserialized Object:
```csharp
ScryfallSet {
  Code = "tmt",
  Name = "Teenage Mutant Ninja Turtles",
  ReleasedAt = "2026-03-06",    // Mapped from "released_at"
  CardCount = 20,                // Mapped from "card_count"
  SetType = "expansion",         // Mapped from "set_type"
  ScryfallUri = "https://..."    // Mapped from "scryfall_uri"
}
```

### Displayed on Page:
```
2026
  • Teenage Mutant Ninja Turtles
    [tmt] [20 cards] [expansion]
    [Open on Scryfall]
```

## Why This Matters

**Convention Differences:**
- **C#**: Uses `PascalCase` (CardCount, SetType)
- **JSON/JavaScript**: Uses `snake_case` (card_count, set_type)
- **Scryfall API**: Follows JSON convention (snake_case)

**Without proper mapping:**
- Properties don't match
- Deserializer can't map fields
- Data appears empty
- Page shows "No sets found"

**With JsonPropertyName attributes:**
- Explicit mapping defined
- Deserializer knows how to map
- All data properly loaded
- Page displays correctly

## Alternative Solution

Could also use `JsonSerializerOptions` with `PropertyNamingPolicy`:

```csharp
var options = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
};

var response = await HttpClient.GetFromJsonAsync<ScryfallSetsResponse>(url, options);
```

However, `[JsonPropertyName]` attributes are more explicit and easier to understand.

---

**The fix is complete!** The Scryfall Sets page now correctly deserializes the API response and displays all sets. 🎉

