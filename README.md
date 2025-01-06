# Xjtf.d

A Windows Service Monitor

## Overview

- monitor running and installed Windows services
- see recent changes in service status
- pin/unpin services
- see weekly uptime and last 30 minutes uptime

## Screenshot

![Xjtf.d Dashboard](https://i.imgur.com/nxSx0py.png)

## Installation

- Download zip file, and unzip to install location
- Run command to install as a Windows Service
- Start the service
- Open the address in your browser: [http://localhost:7631/](http://localhost:7631/)

```powershell
New-Service -Name "xjtf daemon" -BinaryPathName "path-to-app.exe" -StartupType "Automatic"
Start-Service -Name "xjtf daemon"
```
