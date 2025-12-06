# Enhanced Data Extraction - Complete! ✅

## What Information Can Be Extracted?

From the HTML blocks like the one you provided, the application now extracts **7 pieces of data** per expansion:

### Extracted Fields

| Field | Example | Description |
|-------|---------|-------------|
| **Name** | `Magic: The Gathering - FINAL FANTASY Holiday Release` | Full expansion name |
| **URL** | `/de/Magic/Expansions/Magic-The-Gathering-FINAL-FANTASY-Holiday-Release` | Cardmarket URL path |
| **Set Code** | `Magic-The-Gathering-FINAL-FANTASY-Holiday-Release` | Derived from URL (last segment) |
| **Card Count** | `75` | Number of cards in the set |
| **Release Date** | `5. Dezember 2025` | German-formatted release date |
| **Image URL** | `https://product-images.s3.cardmarket.com/1/FINH/851643/851643.jpg` | Preview card image |
| **First Extracted** | `2025-12-06T14:30:00` | When you first added this expansion |
| **Last Seen** | `2025-12-06T15:45:00` | Last time it appeared in an extraction |
| **Times Extracted** | `3` | How many times you've extracted this expansion |

## How It Works

### HTML Pattern Analysis

The extractor looks for this structure:
```html
<div data-url="..." data-local-name="..." class="...expansion-row...">
    <!-- Content contains: -->
    <!-- - Card count in format: "75 Karten" -->
    <!-- - Release date in format: "5. Dezember 2025" -->
    <!-- - Image URL in data-echo attribute -->
</div>
```

### Regex Patterns Used

1. **Name**: `data-local-name="([^"]+)"`
2. **URL**: `data-url="([^"]+)"`
3. **Card Count**: `>(\d+)\s+Karten<`
4. **Release Date**: `>(\d+\.\s+\w+\s+\d{4})<`
5. **Image**: `data-echo="([^"]+)"`

## New Features

### 1. Enhanced Extraction
- Automatically extracts all available data from HTML
- Merges new data with existing expansions (updates fields if available)
- Preserves tracking data (FirstExtracted, LastSeen, TimesExtracted)

### 2. Detailed View Improvements
The "Detailed View" tab now shows:
- **Set Code** - In monospace font for easy copying
- **Card Count** - Number of cards in the set
- **Release Date** - In original German format
- **Preview Image** - Card image thumbnail (64x64)
- **View Button** - Direct link to Cardmarket page

### 3. Smart Data Merging
When you extract the same expansion multiple times:
- ✅ Updates `LastSeen` timestamp
- ✅ Increments `TimesExtracted` counter
- ✅ Updates data fields if new information is available
- ✅ Preserves `FirstExtracted` timestamp

### 4. Enhanced JSON Export
The exported JSON now includes all fields:
```json
[
  {
    "Name": "Magic: The Gathering - FINAL FANTASY Holiday Release",
    "Url": "/de/Magic/Expansions/Magic-The-Gathering-FINAL-FANTASY-Holiday-Release",
    "SetCode": "Magic-The-Gathering-FINAL-FANTASY-Holiday-Release",
    "CardCount": 75,
    "ReleaseDate": "5. Dezember 2025",
    "ImageUrl": "https://product-images.s3.cardmarket.com/1/FINH/851643/851643.jpg",
    "FirstExtracted": "2025-12-06T14:30:00",
    "LastSeen": "2025-12-06T14:30:00",
    "TimesExtracted": 1
  }
]
```

## UI Changes

### Converter Page
- Shows enhanced status messages indicating if extra data was extracted
- Example: `Successfully extracted 25 expansions (with card count & release date)! (10 new, 15 already in list)`

### All Expansions Page - Detailed View
Now shows a comprehensive table with columns:
1. Name (with image preview if available)
2. Set Code
3. Cards
4. Release Date
5. First Extracted
6. Last Seen
7. Times Extracted
8. Actions (View on Cardmarket + Delete)

## What Gets Extracted From Your HTML Sample

From your specific example:
```html
<div data-url="/de/Magic/Expansions/Magic-The-Gathering-FINAL-FANTASY-Holiday-Release" 
     data-local-name="Magic: The Gathering - FINAL FANTASY Holiday Release" 
     class="row g-0 set-as-link expansion-row align-items-center py-2">
```

**Extracted Data:**
- ✅ Name: "Magic: The Gathering - FINAL FANTASY Holiday Release"
- ✅ URL: "/de/Magic/Expansions/Magic-The-Gathering-FINAL-FANTASY-Holiday-Release"
- ✅ Set Code: "Magic-The-Gathering-FINAL-FANTASY-Holiday-Release"
- ✅ Card Count: 75
- ✅ Release Date: "5. Dezember 2025"
- ✅ Image: "https://product-images.s3.cardmarket.com/1/FINH/851643/851643.jpg"

## Benefits

### For Collectors
- 📊 **Track card counts** - Know the size of each set
- 📅 **Track release dates** - Organize by chronology
- 🖼️ **Visual references** - See card images
- 🔗 **Quick access** - Direct links to Cardmarket

### For Data Analysis
- 📁 **Rich JSON export** - Use in spreadsheets, databases, or scripts
- 🔄 **Data updates** - Re-extract to update missing fields
- 📈 **Historical tracking** - See when you first/last saw each set

### For Organization
- 🏷️ **Set codes** - Unique identifiers for each expansion
- 📊 **Statistics** - Track extraction frequency
- 🔍 **Searchable data** - All fields included in JSON

## Backward Compatibility

- ✅ Old data automatically upgraded to new format
- ✅ Missing fields shown as "-" in UI
- ✅ Legacy text export still works
- ✅ Existing LocalStorage data preserved

## Testing Your Data

To verify the extraction works with your HTML file:
1. Open the Converter page
2. Paste your HTML content (or upload the file)
3. Click "Extract Expansions"
4. Check the status message for confirmation
5. Go to "All Expansions" → "Detailed View" tab
6. Verify all columns are populated

## Future Enhancements

Possible additions based on available HTML data:
- Expansion type/category
- Language information
- Icon sprite coordinates
- Alternative names/translations
- Price information (if available in HTML)

