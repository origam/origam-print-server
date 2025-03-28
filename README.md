# Origam Print Server

The **Origam Print Server** is a lightweight Windows Service that enables printing of PDF files via an HTTP API. It uses [Ghostscript](https://www.ghostscript.com/) to render PDFs and sends them to a printer installed on the host machine.

This service is typically used within the [Origam](https://github.com/origam/origam) low-code platform but can be integrated into any system that needs reliable, local PDF printing.

---

## Features

- Print PDFs via HTTP POST request
- Runs as a Windows Service
- Uses [Ghostscript](https://www.ghostscript.com/) for PDF rendering
- Accepts file paths to existing PDFs on disk
- No file data is transferred over the network
- Built-in [Swagger UI](https://swagger.io/tools/swagger-ui/) for API documentation and testing

---

## Requirements

- Windows OS
- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download)
- [Ghostscript](https://www.ghostscript.com/download/gsdnld.html) installed and accessible via PATH or full path
- At least one printer installed on the host machine

---

## Installation

1. **Clone the Repository**
   ```bash
   git clone https://github.com/origam/origam-print-server.git
   cd origam-print-server
   ```

2. **Build the Project**
   ```bash
   dotnet publish -c Release -o ./publish
   ```

3. **Install as a Windows Service**

   Run from an elevated terminal (PowerShell or CMD):

   ```powershell
   sc create OrigamPrintServer binPath= "C:\Path\To\publish\Origam.PrintServer.exe"
   ```

4. **Start the Service**
   ```powershell
   sc start OrigamPrintServer
   ```

---

## Usage

The service exposes a single HTTP POST endpoint:

```
POST http://localhost:5000/api/print-pdf
```

### Request Body (JSON)

```json
{
  "path": "C:\\path\\to\\file.pdf",
  "printer": "Microsoft Print to PDF"
}
```

### Response

- `200 OK` — printing started successfully
- `400 Bad Request` — invalid input
- `500 Internal Server Error` — printing failed

---

## Swagger UI

After starting the service, you can access the interactive API documentation at:

```
http://localhost:5000/swagger
```

From the Swagger UI, you can explore available endpoints, see schema definitions, and send test requests directly from your browser.

---

## Configuration

The service reads configuration from `appsettings.json`. Below is a sample:

```json
{
  "Ghostscript": {
    "ExecutablePath": "gswin64c.exe",
    "Device": "pxlcolor"
  },
  "Urls": "http://localhost:5000"
}
```

### Configuration Options

- `Ghostscript.ExecutablePath`: Name or full path to the Ghostscript executable (e.g. `gswin64c.exe`)
- `Ghostscript.Device`: Ghostscript output device (e.g. `pxlcolor`, `mswinpr2`)
- `Urls`: Address the HTTP server listens on (default: `http://localhost:5000`)

### Logging

Logging is configured using [log4net](https://logging.apache.org/log4net/). See `log4net.config` to change output format, level, and log destinations.

---

## Development

To run the service locally without installing it:

```bash
dotnet run --project Origam.PrintServer
```

Then access the Swagger UI at [http://localhost:5000/swagger](http://localhost:5000/swagger) and test the API directly.

---

## License

This project is licensed under the GNU General Public License v3.0. See [LICENSE](LICENSE) for details.

---

## Acknowledgments

- [Ghostscript](https://www.ghostscript.com/) for high-performance PDF rendering
- [Swagger UI](https://swagger.io/tools/swagger-ui/) for seamless API testing
