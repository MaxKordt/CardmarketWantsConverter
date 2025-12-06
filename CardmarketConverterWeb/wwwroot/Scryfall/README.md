# Scryfall Bulk Data

This directory should contain Scryfall bulk data files downloaded by each user.

## Location

This directory is in `wwwroot/Scryfall/` so that files can be served by the web application.

## How to Download

1. Visit [Scryfall Bulk Data API](https://scryfall.com/docs/api/bulk-data)
2. Download the **"All Cards"** JSON file (or "Default Cards")
3. Place the downloaded `.json` file in this directory (`wwwroot/Scryfall/`)

## Available Bulk Data Types

From Scryfall, you can download:
- **Default Cards** - All cards including digital-only and funny cards
- **Oracle Cards** - Unique card faces with Oracle text
- **Unique Artwork** - Cards with unique illustrations
- **All Cards** - Every card printing
- **Rulings** - Card rulings data

For this application, the **Default Cards** or **Oracle Cards** file is recommended.

## Important Notes

⚠️ **DO NOT commit these files to git!**
- These files are large (50-200MB+)
- They are excluded via `.gitignore`
- Each user should download them separately
- Scryfall updates these daily

## File Format

The downloaded file will be named something like:
```
default-cards-20251206100000.json
oracle-cards-20251206100000.json
```

The timestamp indicates when the data was generated.

## Usage

The application will automatically detect and use any `.json` files in this directory to populate card data.

## Attribution

Card data is provided by [Scryfall](https://scryfall.com/) under their terms of use.
Please respect their [API guidelines](https://scryfall.com/docs/api).

