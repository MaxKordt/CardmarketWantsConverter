# Cardmarket Wants Converter

A web-based tool to extract and manage Magic: The Gathering expansion information from Cardmarket and retrieve card data from Scryfall.

## Features

- 📋 Extract expansion lists from Cardmarket HTML
- 🗂️ Organize expansions by release year (collapsible sections)
- 🃏 Fetch card data via Scryfall API
- 💾 Persistent storage in browser (LocalStorage)
- 📥 Export to JSON or TXT format
- 🔍 View expansion details (card count, release date, set codes)

## Usage

### Extract Expansions
1. Go to [Cardmarket Expansions](https://www.cardmarket.com/de/Magic/Expansions)
2. Copy the page HTML (View Source or Inspect)
3. Paste into the converter
4. Click "Extract Expansions"

### View & Manage
- Browse expansions organized by year
- Click year headers to expand/collapse
- Export your collection as JSON or TXT
- Open expansions directly on Cardmarket

### Fetch Card Data
- Card data retrieved from [Scryfall API](https://scryfall.com/docs/api)
- Click "Fetch Cards" to retrieve singles for any expansion
- Data persists in browser storage

## Technology Stack

- **Frontend**: Blazor WebAssembly (.NET 8)
- **Storage**: Browser LocalStorage
- **APIs**: Scryfall API (card data)
- **Styling**: Bootstrap 5

## Legal & Attribution

### Scryfall API
This application uses the [Scryfall API](https://scryfall.com/) to retrieve Magic: The Gathering card data.
- Scryfall API is free and requires no authentication
- Please respect Scryfall's [rate limits](https://scryfall.com/docs/api#rate-limits-and-good-citizenship)
- Card images and data are used in accordance with Scryfall's terms

### Cardmarket
This tool parses publicly available HTML from [Cardmarket](https://www.cardmarket.com/).
- This is a **personal, non-commercial tool**
- No private APIs are accessed
- No data is redistributed or sold
- Users must comply with Cardmarket's Terms of Service

### Wizards of the Coast
Magic: The Gathering and all related content are copyrighted by Wizards of the Coast LLC.
- This is an unofficial fan-made tool
- Complies with [WotC Fan Content Policy](https://company.wizards.com/en/legal/fancontentpolicy)
- Not affiliated with, endorsed, or sponsored by Wizards of the Coast
- Card data is sourced through Scryfall's licensed API

## Disclaimer

This is a **personal project** for organizing and managing Magic: The Gathering collection data. It is provided "as-is" without warranty of any kind.

**Not for commercial use or redistribution.**

By using this tool, you agree to:
- Use it for personal, non-commercial purposes only
- Respect all applicable Terms of Service (Cardmarket, Scryfall, WotC)
- Not overload or abuse any external services
- Not redistribute or sell any extracted data

## Privacy

- All data is stored **locally in your browser** (LocalStorage)
- No data is sent to any server (except API requests to Scryfall)
- No analytics or tracking
- No user accounts or authentication

## Development

### Setup

1. **Download Scryfall Bulk Data** (Required)
   - Go to [Scryfall Bulk Data](https://scryfall.com/docs/api/bulk-data)
   - Download the latest "Default Cards" JSON file
   - Create a `Scryfall/` directory in the project root
   - Place the downloaded JSON file in the `Scryfall/` directory
   - The file is NOT included in the repository (users must download it themselves)

2. **Build & Run**
   ```bash
   dotnet build
   dotnet run
   ```

### Project Structure
- `Models/` - Data models (Expansion, Card)
- `Services/` - Business logic (extraction, API calls, storage)
- `Pages/` - Blazor components (UI pages)
- `wwwroot/` - Static assets
- `Scryfall/` - Bulk data files (excluded from git, download separately)

## Contributing

This is a personal project primarily for private use. If sharing with friends:
- Remind them of the legal considerations above
- Ensure they understand it's for personal use only
- Respect rate limits when using Scryfall API

## License

This project is **not licensed for public distribution**.

For personal use only. All Magic: The Gathering content is property of Wizards of the Coast LLC.

---

**Attribution:**
- Card data provided by [Scryfall](https://scryfall.com/)
- Expansion information from [Cardmarket](https://www.cardmarket.com/)
- Magic: The Gathering © Wizards of the Coast LLC

**Contact:** For questions about this project, please contact the repository owner directly.

