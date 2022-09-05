namespace xjtf.d;

public class ServiceStatsTransformer : ICommandResultTransformer
{
    private readonly DaemonDbContext _dbContext;

    /// <summary>
    /// Transforms the command results of commands returning service information
    /// by enriching the returned PowerShell data with some latest stats about the resource(s).
    /// </summary>
    /// <param name="dbContext"></param>
    public ServiceStatsTransformer(DaemonDbContext dbContext) => _dbContext = dbContext;

    /// <summary>
    /// Performs the enrichment of service data by matching with related data from the database.
    /// </summary>
    /// <param name="commandResult"></param>
    /// <returns></returns>
    public object RunTransform(object commandResult)
    {
        if (commandResult is string str_commandResult)
            return str_commandResult;

        if (commandResult is IEnumerable<object> enb_commandResult)
            return enb_commandResult.Select(o => RunTransform(o)).ToArray();

        if (commandResult is IDictionary<string, object> obj_commandResult)
        {
            if (obj_commandResult.ContainsKey("ServiceName"))
            {
                var serviceName = (string)obj_commandResult["ServiceName"];
                var serviceStat = (_dbContext.ServiceStatistics
                    .Where(s => s.ServiceName == serviceName)).ToList()
                    .OrderByDescending(s => s.Measured)
                    .FirstOrDefault();

                if (serviceStat != null)
                {
                    var statsObject = new ExpandoObject() as IDictionary<string, object>;
                    statsObject.Add("Reported", serviceStat.Measured);
                    statsObject.Add("Mean", serviceStat.MeanUptime);
                    obj_commandResult.Add("Stats", statsObject);
                }
            }
        }
        return commandResult;
    }
}