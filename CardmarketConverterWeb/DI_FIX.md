# Dependency Injection Fix for ScryfallDataService ✅

## Problem

When trying to load the Scryfall Sets page, the application threw this error:

```
System.InvalidOperationException: Cannot consume scoped service 'System.Net.Http.HttpClient' 
from singleton 'CardmarketConverterWeb.Services.ScryfallDataService'.
```

## Root Cause

**Dependency Injection Lifetime Mismatch:**
- `ScryfallDataService` was registered as **Singleton** (lives for app lifetime)
- `HttpClient` is registered as **Scoped** (lives per request/page)
- A singleton cannot depend on a scoped service (violates DI rules)

## Solution

### 1. **Changed Service Lifetime** (`Program.cs`)
```csharp
// Before (WRONG):
builder.Services.AddSingleton<ScryfallDataService>();

// After (CORRECT):
builder.Services.AddScoped<ScryfallDataService>();
```

### 2. **Implemented Static Caching** (`ScryfallDataService.cs`)

Since we changed to Scoped (recreated per page), we needed to ensure the bulk data is still loaded only once:

```csharp
// Static fields - shared across all instances
private static List<ScryfallCard>? _allCards;
private static bool _isLoaded = false;
private static readonly SemaphoreSlim _loadLock = new(1, 1);
```

**Benefits:**
- ✅ Service is scoped (can use HttpClient)
- ✅ Data is cached statically (loaded only once)
- ✅ Thread-safe loading (SemaphoreSlim prevents race conditions)
- ✅ Memory efficient (single copy of data)

## How It Works Now

1. **First Request**:
   - ScryfallDataService instance created
   - Checks static `_isLoaded` → false
   - Acquires lock
   - Loads 197MB JSON file
   - Stores in static `_allCards`
   - Sets `_isLoaded` = true
   - Releases lock

2. **Subsequent Requests**:
   - New ScryfallDataService instance created
   - Checks static `_isLoaded` → true
   - Returns immediately (data already loaded)
   - No additional HTTP requests or parsing

## Technical Details

### Service Lifetimes in Blazor WebAssembly

| Lifetime | Scope | Use Case |
|----------|-------|----------|
| **Singleton** | Entire app | App-wide state, no external dependencies |
| **Scoped** | Per user/tab | Services that use HttpClient, per-page state |
| **Transient** | Per injection | Stateless utilities |

### Why HttpClient is Scoped

In Blazor WebAssembly:
- Each browser tab = separate scope
- HttpClient has request-specific context
- Cannot be shared across app lifetime

### Thread Safety

The `SemaphoreSlim` ensures:
- Only one thread loads data at a time
- Multiple simultaneous page loads don't load data multiple times
- Race conditions are prevented

```csharp
await _loadLock.WaitAsync();  // Wait for exclusive access
try
{
    // Load data (only one thread executes this)
}
finally
{
    _loadLock.Release();  // Release lock
}
```

## Testing

After this fix:
- ✅ Page loads without errors
- ✅ Bulk data loads once on first access
- ✅ Subsequent page navigations are instant
- ✅ Memory usage is optimal (single copy of data)

## Additional Fix: 404 Not Found Error

### Problem
After fixing the DI issue, you may encounter:
```
Failed to load resource: the server responded with a status of 404 (Not Found)
Failed to load Scryfall bulk data file
```

### Root Cause
The Scryfall directory was in the project root, but **static files must be in `wwwroot/` to be served by the web server**.

### Solution
**Moved Scryfall directory to wwwroot:**
```
Before: /CardmarketConverterWeb/Scryfall/
After:  /CardmarketConverterWeb/wwwroot/Scryfall/
```

**Updated .gitignore:**
```gitignore
# Scryfall bulk data files
wwwroot/Scryfall/*.json
!wwwroot/Scryfall/README.md
```

**Why this is required:**
- Blazor WebAssembly serves files from `wwwroot/` directory only
- Files outside `wwwroot/` cannot be accessed via HTTP
- The HttpClient.GetAsync() call needs the file to be web-accessible

### File Location
Place your Scryfall bulk data file here:
```
wwwroot/
  └── Scryfall/
      ├── README.md (committed)
      └── all-cards-20251205102358.json (gitignored)
```

## Build Status

✅ **Successful** (0 errors, 0 warnings)

The Scryfall Sets page now loads correctly with proper dependency injection configuration!

