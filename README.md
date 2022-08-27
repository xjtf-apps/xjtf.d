# xjtf.d

A .NET Windows service manager.

## Install

Install as a usual Windows service:

```powershell
New-Service -Name "xjtf daemon" -BinaryPathName "path-to-app.exe config.ini" -StartupType "Automatic"
```

Configure it to run as an Admin account, create a custom user if needed.

### Configure the services

Open config.ini and add entries per line, for example:

```
ServiceName<|>PathToApplication.exe<|>Arguments
```

### Other configuration

Check out appsettings.json