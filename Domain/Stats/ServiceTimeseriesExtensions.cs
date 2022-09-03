namespace xjtf.d;

internal static class ServiceTimeseriesExtensions
{
    public static ServiceTimeseries.DataPoint FromData(string status, DateTimeOffset observed)
    {
        static int GetScore(string status)
        {
            return status switch
            {
                "Stopped"         => 0,
                "StartPending"    => 0,
                "StopPending"     => 0,
                "Running"         => 100,
                "ContinuePending" => 0,
                "PausePending"    => 0,
                "Paused"          => 0,

                _ => throw new NotSupportedException("Status not supported")
            };
        }

        return new ServiceTimeseries.DataPoint(
            Score: GetScore(status), Observed: observed
        );
    }
}