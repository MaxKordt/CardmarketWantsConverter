# Cardmarket Converter - Blazor WebAssembly

A web-based tool to extract expansion names from Cardmarket HTML files and convert them to a downloadable text format.

## Features

- 📤 Upload HTML files or paste HTML content directly
- 🔄 Extract expansion names using regex pattern matching
- 📥 Download results as a TXT file with timestamp
- 🌐 Runs entirely in your browser (Blazor WebAssembly)
- 💻 Localhost only - no deployment needed

## How to Run

### Start the Application

```bash
cd CardmarketConverterWeb
dotnet run
```

The application will start and display a URL like:
- `https://localhost:5001` or `http://localhost:5000`

Open this URL in your browser.

### Stop the Application

Press `Ctrl+C` in the terminal to stop the server.

## How to Use

1. **Upload or Paste HTML Content**
   - Click "Upload HTML File" to select an HTML file from your computer
   - OR paste HTML content directly into the text area

2. **Extract Expansions**
   - Click the "Extract Expansions" button
   - The tool will parse the HTML and extract all expansion names

3. **Download Results**
   - View the extracted expansions in the output area
   - Click "Download as TXT" to save the results
   - File will be named `expansions_YYYYMMDD_HHmmss.txt`

## Technical Details

- **Platform**: Blazor WebAssembly (.NET 8)
- **Extraction Pattern**: `data-local-name="([^"]+)"`
- **Max File Size**: 10MB
- **Supported Formats**: .html, .htm

## Project Structure

```
CardmarketConverterWeb/
├── Pages/
│   └── Home.razor          # Main converter UI
├── Services/
│   └── ExpansionExtractor.cs  # Conversion logic
├── wwwroot/
│   └── index.html          # Download script
└── CardmarketConverterWeb.csproj
```

## Future Enhancements

You can easily extend this application by:
- Adding more conversion formats/patterns
- Creating additional tabs for different conversion modes
- Adding custom regex pattern input
- Implementing batch file processing
- Adding export formats (CSV, JSON, etc.)

## Development

To make changes:
1. Edit the files in your IDE
2. Save changes
3. The app will hot-reload automatically if running with `dotnet watch`

```bash
dotnet watch run
```

