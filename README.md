# xjtf.d

A .NET Windows service manager.

## Install

Install as a usual Windows service:

```powershell
New-Service -Name "xjtf daemon" -BinaryPathName "path-to-app.exe" -StartupType "Automatic"
```

Configure it to run as an Admin account, create a custom user if needed.