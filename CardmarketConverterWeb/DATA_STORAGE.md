# Data Storage - Cardmarket Converter

## Where Is Your Data Stored?

Your expansion data is **NOT stored as a file on your hard drive**. Instead, it's stored in your browser's **LocalStorage**, which is a built-in browser database.

### Browser Storage Location

The data is stored in browser-specific database files:

#### macOS:
- **Chrome/Edge**: `~/Library/Application Support/Google/Chrome/Default/Local Storage/leveldb/`
- **Firefox**: `~/Library/Application Support/Firefox/Profiles/[profile]/storage/default/`
- **Safari**: `~/Library/Safari/LocalStorage/`

#### Windows:
- **Chrome/Edge**: `%LocalAppData%\Google\Chrome\User Data\Default\Local Storage\leveldb\`
- **Firefox**: `%AppData%\Mozilla\Firefox\Profiles\[profile]\storage\default\`

#### Linux:
- **Chrome**: `~/.config/google-chrome/Default/Local Storage/leveldb/`
- **Firefox**: `~/.mozilla/firefox/[profile]/storage/default/`

**⚠️ Important**: These are binary database files, not human-readable JSON or text files. You cannot directly open or edit them.

## How to Access Your Data

### Method 1: Download as File (Recommended)

1. Open the application in your browser
2. Go to the "All Expansions" page
3. Click **"Download as JSON"** to save your data to your Downloads folder
4. The file will be named `expansions_YYYYMMDD_HHMMSS.json`

### Method 2: Browser Developer Tools

You can view your data directly in the browser:

1. Press `F12` to open Developer Tools
2. Go to the **Application** tab (Chrome/Edge) or **Storage** tab (Firefox)
3. Expand **Local Storage**
4. Click on your site URL
5. Look for the key `cardmarket_expansions_v2`
6. The value column shows your JSON data

### Method 3: Export via Console

1. Press `F12` to open Developer Tools
2. Go to the **Console** tab
3. Run this command:
```javascript
console.log(localStorage.getItem('cardmarket_expansions_v2'));
```
4. Copy the JSON output

## Backing Up Your Data

### Regular Backups

To prevent data loss, regularly download your expansions:

1. Open the "All Expansions" page
2. Click "Download as JSON"
3. Save the file somewhere safe (e.g., Documents, Dropbox, Google Drive)

### Recommended Backup Schedule

- After each major extraction session
- Once a week if you use it regularly
- Before clearing browser data
- Before switching browsers

## Restoring Data

Currently, the app doesn't have an import feature, but you can restore data manually:

1. Press `F12` to open Developer Tools
2. Go to the **Console** tab
3. Run this command (replace `YOUR_JSON_DATA` with the content from your backup file):
```javascript
localStorage.setItem('cardmarket_expansions_v2', 'YOUR_JSON_DATA');
```
4. Refresh the page

## Data Loss Scenarios

Your data will be lost if you:

- ❌ Clear browser cache/cookies/site data
- ❌ Uninstall the browser without backing up
- ❌ Use incognito/private browsing mode
- ❌ Switch to a different browser
- ❌ Use a different user profile in the same browser
- ❌ Use the app on a different computer

**💡 Solution**: Always download your JSON backup before any of these actions!

## Viewing Your Data in JSON Format

### In the Application

1. Go to "All Expansions" page
2. Click the **"JSON View"** tab
3. You'll see the formatted JSON structure

### Example JSON Structure

```json
[
  {
    "Name": "Alpha Edition",
    "FirstExtracted": "2025-12-06T14:30:00",
    "LastSeen": "2025-12-06T14:30:00",
    "TimesExtracted": 1
  },
  {
    "Name": "Beta Edition",
    "FirstExtracted": "2025-12-06T14:30:00",
    "LastSeen": "2025-12-06T15:45:00",
    "TimesExtracted": 2
  }
]
```

## Why LocalStorage Instead of Files?

This is a **browser-based WebAssembly application** that runs entirely in your browser. For security reasons, web applications cannot directly write files to arbitrary locations on your hard drive. The only options are:

1. **LocalStorage** (what we use) - Automatic, persistent, no user interaction needed
2. **Downloads** - Requires user to click download for each save
3. **Backend Server** - Would require hosting and database setup

LocalStorage provides the best user experience for a client-side app, but always remember to download backups!

## Future Enhancement Ideas

If you need better data management, possible improvements include:

- **Auto-export**: Automatically download a backup after each extraction
- **Import feature**: Upload a JSON file to restore data
- **Cloud sync**: Sync data across devices (requires backend)
- **Export to CSV**: For use in spreadsheets
- **Periodic reminders**: Remind you to backup your data

## Questions?

If you have questions about data storage or need help recovering data, check the Developer Console for the raw data, or download a fresh backup using the "Download as JSON" button.

