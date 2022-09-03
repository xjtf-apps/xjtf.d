namespace xjtf.d;

public static class CommandObject
{
    public static bool TryGenerateScript(this (Command Command, CommandArgs Arguments) CommandObject, out string script)
    {
        script = "";
        var cmd = CommandObject.Command;
        var args = CommandObject.Arguments._arguments;

        switch (cmd)
        {
            case Command.Nop: break;
            case Command.GetServices:
            {
                script = $"Get-Service";
                break;
            }
            case Command.GetService:
            {
                var serviceName = args[0];
                script = $"Get-Service -Name {serviceName}";
                break;
            }
            case Command.StartService:
            {
                var serviceName = args[0];
                script = $"Start-Service -Name {serviceName}";
                break;
            }
            case Command.StopService:
            {
                var serviceName = args[0];
                script = $"Stop-Service -Name {serviceName}";
                break;
            }
            case Command.RestartService:
            {
                var serviceName = args[0];
                script = $"Restart-Service -Name {serviceName}";
                break;
            }
            default:
                throw new NotSupportedException("Command not supported");
        }
        return script != "";
    }
}