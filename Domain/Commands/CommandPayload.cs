namespace xjtf.d;

public static class CommandPayload
{
    public static (Command CommandType, CommandArgs CommandArguments)? Unwrap(string base64String)
    {
        var bytes = Convert.FromBase64String(base64String);
        var str = Encoding.UTF8.GetString(bytes);
        var command = Command.Nop;
        var fs = str.Split("|");

        if (fs.Length > 0 &&
            int.TryParse(fs[0], out int cmdId) &&
            Enum.IsDefined(typeof(Command), (int)cmdId))
            command = (Command)cmdId;
        else
            return null;

        var mask = CommandMasks.ByType(command);
        var args = new List<object?>();
        var rest = fs[1..];

        for (int v = 0; v < rest.Length; v++)
        {
            var arg_literal = rest[v];
            if (arg_literal != null)
            {
                var constraint = mask._constraints[v];
                var arg_type = constraint.Type;

                if (arg_type == typeof(int)) args.Add(int.Parse(arg_literal));
                else if (arg_type == typeof(string)) args.Add(arg_literal);
                else if (arg_type == typeof(bool)) args.Add(bool.Parse(arg_literal));
            }
            else if (arg_literal != null && arg_literal == "null")
                args.Add(null);
                
            else throw new NullReferenceException(nameof(arg_literal));
        }
        return (command, new CommandArgs(args));
    }
}