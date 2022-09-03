namespace xjtf.d;

public class GetServicesWorker
{
    private readonly CommandRunnerRestAdapterFactory _commandRunnerFactory;

    public GetServicesWorker(CommandRunnerRestAdapterFactory commandRunnerFactory) => _commandRunnerFactory = commandRunnerFactory;

    public async Task ConfigureLongRunningTask(HttpContext httpContext)
    {
        if (httpContext.WebSockets.IsWebSocketRequest)
        {
            var lifetime = Program.ApplicationLifetime!;
            var cancellationToken = lifetime.ApplicationStopping;
            using var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();

            var webSocketTask = Task.Run(async () =>
            {
                while (true)
                {
                    await SendAsync(webSocket, cancellationToken);
                    await Task.Delay(5_000);
                }
            }
            , cancellationToken);
            
            await webSocketTask;
            await CloseAsync(webSocket);
        }
        else
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private async Task CloseAsync(WebSocket webSocket)
    {
        var status = WebSocketCloseStatus.EndpointUnavailable;
        await webSocket.CloseAsync(status, null, CancellationToken.None);
    }

    private async Task SendAsync(WebSocket webSocket, CancellationToken cancellationToken)
    {
        var data = await GetDataAsync();
        var messageType = WebSocketMessageType.Text;
        await webSocket.SendAsync(data, messageType, true, cancellationToken);
    }

    private async Task<byte[]> GetDataAsync()
    {
        var command = Command.GetServices;
        var commandArgs = new CommandArgs();
        var commandRunner = _commandRunnerFactory.GetNew();
        var commandResult = await commandRunner.RunAsync(command, commandArgs);

        var commandResultSerialized =
            JsonSerializer.Serialize(commandResult.Value);

        var bytes =
            Encoding.UTF8.GetBytes(commandResultSerialized);

        return bytes;
    }
}