# File Size Issue - Solution Guide ✅

## Problem

When trying to load the Scryfall Sets page, you may encounter errors if the bulk data file is empty, corrupted, or too large.

## New Solution: File Selection UI

The Scryfall Sets page now includes **buttons to select which bulk data file to load**. You no longer need to rename files or edit code!

### How It Works

1. **Navigate to Scryfall Sets page**
2. **See three buttons**:
   - **Oracle Cards** (~50MB) - Recommended
   - **Default Cards** (~80MB) - Alternative
   - **All Cards** (~200MB) - May fail due to size
3. **Download the file you want** from [Scryfall Bulk Data](https://scryfall.com/docs/api/bulk-data)
4. **Place in `wwwroot/Scryfall/`** with the exact name:
   - `oracle-cards.json` for Oracle Cards button
   - `default-cards.json` for Default Cards button
   - `all-cards.json` for All Cards button
5. **Click the corresponding button** on the page
6. **Wait for data to load** (3-30 seconds depending on file size)

### Custom Files

There's also an "Advanced" option to load custom filenames:
- Expand the "Advanced: Load custom file" section
- Enter your filename (without path, just the name.json)
- Click "Load Custom File"

## Recommended Setup (Oracle Cards)

**Download the smaller file:**
1. Go to [Scryfall Bulk Data](https://scryfall.com/docs/api/bulk-data)
2. Download **"Oracle Cards"** (~50MB) instead of "All Cards"
3. Place in `wwwroot/Scryfall/` directory
4. Rename to `oracle-cards.json` for consistency

**Benefits:**
- Much smaller file size (~50MB vs ~197MB)
- Contains all unique card faces with Oracle text
- Sufficient for most use cases
- Loads reliably in browser

**Oracle Cards vs All Cards:**
| Feature | Oracle Cards | All Cards |
|---------|-------------|-----------|
| Size | ~50MB | ~197MB |
| Cards | Unique faces | Every printing |
| Load time | Fast | Slow/fails |
| Memory | Low | High |

### Option 2: Use Default Cards

**Alternative smaller file:**
1. Download **"Default Cards"** from Scryfall
2. File size varies (~80-100MB)
3. Contains most cards without all variations
4. Better compatibility than "All Cards"

### Option 3: Filter the File (Advanced)

If you absolutely need specific data from "All Cards":
1. Download the file locally
2. Use a script to filter it (e.g., Python/Node.js)
3. Create a smaller JSON with only needed sets
4. Upload the filtered version

## What I Changed

### 1. Flexible File Detection

The service now tries multiple filenames:
```csharp
var possibleFiles = new[]
{
    "Scryfall/oracle-cards.json",
    "Scryfall/default-cards.json",
    "Scryfall/all-cards.json",
    // ... timestamped versions
};
```

### 2. Better Error Messages

More helpful console output:
- Shows which file was found
- Displays file size
- Suggests alternatives on failure
- Explains memory issues

### 3. Stream-Based Loading

Uses `ReadAsStreamAsync()` for better memory handling:
- Doesn't load entire file into string first
- More efficient for large files
- Still may fail if file is too large

## Recommended Setup

### Step-by-Step:

1. **Remove current file** (if you have all-cards):
   ```bash
   rm wwwroot/Scryfall/all-cards-*.json
   ```

2. **Download Oracle Cards**:
   - Visit https://scryfall.com/docs/api/bulk-data
   - Find "Oracle Cards" (not "All Cards")
   - Download the JSON file

3. **Place and rename**:
   ```bash
   # Move downloaded file to project
   mv ~/Downloads/oracle-cards-*.json wwwroot/Scryfall/oracle-cards.json
   ```

4. **Rebuild and run**:
   ```bash
   dotnet build
   dotnet run
   ```

## File Comparison

### Oracle Cards (Recommended)
```
✅ Size: ~50MB
✅ Load time: 3-5 seconds
✅ Memory: ~200-300MB browser usage
✅ Content: All unique card Oracle texts
✅ Updates: Daily
```

### All Cards (Not Recommended for Web)
```
❌ Size: ~197MB
❌ Load time: May timeout
❌ Memory: >500MB browser usage
❌ Content: Every card printing/variation
⚠️ May fail to load in browser
```

## Technical Details

### Why Browsers Struggle with Large Files

1. **Memory limits**: Browsers have memory caps
2. **String duplication**: JSON parsing creates copies
3. **Garbage collection**: Large objects cause GC pressure
4. **Request size limits**: HTTP response size limits

### What Happens During Load

```
1. HttpClient requests file (197MB)
2. Browser downloads to memory
3. Stream created from response
4. JSON deserializer parses stream
5. Creates .NET objects (more memory!)
6. Total memory: 2-3x file size
```

With 197MB file:
- Download: 197MB
- Parsing: +200MB
- Objects: +300MB
- **Total: ~700MB** (may exceed browser limits)

## Testing

After switching to Oracle Cards:
- ✅ Page should load without errors
- ✅ Console shows successful load message
- ✅ Card count will be lower (unique cards only)
- ✅ All sets still available
- ✅ Faster performance

## Troubleshooting

### Still getting errors?

1. **Check file location**:
   ```
   wwwroot/Scryfall/oracle-cards.json
   ```

2. **Verify file is valid JSON**:
   ```bash
   head -c 100 wwwroot/Scryfall/oracle-cards.json
   ```
   Should start with `[{`

3. **Check file size**:
   ```bash
   ls -lh wwwroot/Scryfall/*.json
   ```
   Should be ~50MB for Oracle Cards

4. **Clear browser cache**:
   - Hard refresh (Ctrl+Shift+R)
   - Clear site data in DevTools

## Build Status

✅ **Successful** (0 errors, 0 warnings)

The application now handles file loading more robustly and provides better error messages. **Use Oracle Cards for best results!**

