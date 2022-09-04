namespace xjtf.d;

public class ServiceStatsTransformer : ICommandResultTransformer
{
    private readonly DaemonDbContext _dbContext;

    public ServiceStatsTransformer(DaemonDbContext dbContext) => _dbContext = dbContext;

    public object RunTransform(object commandResult)
    {
        if (commandResult is string str_commandResult)
            return str_commandResult;

        if (commandResult is IEnumerable<object> enb_commandResult)
            return enb_commandResult.Select(o => RunTransform(o)).ToArray();

        if (commandResult is IDictionary<String,Object> obj_commandResult)
        {
            if (obj_commandResult.ContainsKey("ServiceName"))
            {
                var serviceName = (string)obj_commandResult["ServiceName"];
                var serviceStat = (_dbContext.ServiceStatistics
                    .Where(s => s.ServiceName == serviceName))
                    .ToList()
                    .OrderByDescending(s => s.Measured)
                    .FirstOrDefault();

                if (serviceStat != null)
                    obj_commandResult.Add("Mean", serviceStat.MeanUptime);
            }
        }
        return commandResult;
    }

    object ICommandResultTransformer.RunTransform(object commandResult) => RunTransform(commandResult);
    public async Task<object> RunTransformAsync(Task<object> commandResult) => RunTransform(await commandResult);
    async Task<object> ICommandResultTransformer.RunTransformAsync(Task<object> commandResult) => await RunTransformAsync(commandResult);
}