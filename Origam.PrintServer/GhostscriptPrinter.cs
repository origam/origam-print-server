#region license
/*
Copyright 2005 - 2025 Advantage Solutions, s. r. o.

This file is part of ORIGAM (http://www.origam.org).

ORIGAM is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

ORIGAM is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with ORIGAM. If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

using System.Diagnostics;
using System.Text;

namespace Origam.PrintServer;

public class GhostscriptPrinter(
    IConfiguration config, 
    ILogger<GhostscriptPrinter> log)
{
    private static readonly int ProcessTimeout = 60_000; //ms
    
    private readonly string ghostscriptPath 
        = config["Ghostscript:ExecutablePath"] ?? "gswin64c.exe";
    private readonly string ghostscriptDevice
        = config["Ghostscript:Device"] ?? "pxlcolor";

    public void PrintPdf(string printerName, string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("PDF file not found", filePath);
        }
        var ghostscriptArguments
            = BuildGhostscriptArguments(printerName, filePath);
        using var ghostscriptProcess = new Process();
        ghostscriptProcess.StartInfo = new ProcessStartInfo
        {
            FileName = ghostscriptPath,
            Arguments = ghostscriptArguments,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true
        };
        var standardError = new StringBuilder();
        using var errorWaitHandle = new AutoResetEvent(false);
        ghostscriptProcess.ErrorDataReceived += (sender, e) =>
        {
            if (e.Data == null)
            {
                errorWaitHandle.Set();
            }
            else
            {
                standardError.AppendLine(e.Data);
            }
        };
        if (log.IsEnabled(LogLevel.Debug))
        {
            log.LogDebug(
                "Printing document with arguments: {GhostscriptArguments}", 
                ghostscriptArguments);
        }
        ghostscriptProcess.Start();
        ghostscriptProcess.BeginErrorReadLine();
        if (!ghostscriptProcess.WaitForExit(ProcessTimeout)
            || !errorWaitHandle.WaitOne(ProcessTimeout))
        {
            throw new Exception(
                $"Timeout while executing ghostscript: {ghostscriptArguments}");
        }
        if (standardError.Length > 0)
        {
            throw new Exception(
                $"Error while executing ghostscript: {ghostscriptArguments}\n{standardError}");
        }
    }
    
    private string BuildGhostscriptArguments(
        string printerName, string filePath)
    {
        return $"-dBATCH -dNOPAUSE -sDEVICE={ghostscriptDevice} -sOutputFile=\"%printer%{printerName}\" \"{filePath}\"";
    }
}