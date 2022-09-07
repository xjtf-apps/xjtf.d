namespace xjtf.d;

public class ServiceHostTransformer : ICommandResultTransformer
{

    private IEnumerable<string>? _hostedFilesCache;
    private IEnumerable<string> _hostedFiles
    {
        get
        {
            if (_hostedFilesCache == null)
            {
                _hostedFilesCache = Directory
                    .EnumerateFiles(_serviceStore.StorageRootName, "*", SearchOption.AllDirectories);
            }
            return _hostedFilesCache;
        }
    }

    private readonly ServiceStore _serviceStore;
    /// <summary>
    /// Transforms the command results of commands returning service information
    /// by enriching the returned PowerShell data with the information whether this data represents a daemon hosted service.
    /// </summary>
    public ServiceHostTransformer(ServiceStore serviceStore) => _serviceStore = serviceStore;

    /// <summary>
    /// Performs the enrichment of service data by matching with related data from the environment.
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
            if (obj_commandResult.ContainsKey("BinaryPathName"))
            {
                // TODO: this is slow, hosted service should ideally be identifiable by a database record

                var serviceBinaryPathName = (string)obj_commandResult["BinaryPathName"];
                var serviceDaemonManaged = _hostedFiles
                    .Where(f => f.Contains(serviceBinaryPathName))
                    .Any();

                obj_commandResult.Add("DaemonManaged", serviceDaemonManaged);
            }
        }
        return commandResult;
    }
}