# Xjtf.d

A Windows Service Monitor

## Overview

![Xjtf.d Dashboard](https://i.imgur.com/nxSx0py.png)

- monitor running and installed Windows services
- pin/unpin services
- see weekly uptime and last 30 minutes uptime

## Installation

- Download zip file, and unzip to install location
- Run command to install as a Windows Service

´´´powershell
New-Service -Name "xjtf daemon" -BinaryPathName "path-to-app.exe" -StartupType "Automatic"
´´´
