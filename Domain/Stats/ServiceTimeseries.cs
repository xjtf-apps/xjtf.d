namespace xjtf.d;

public class ServiceTimeseries
{
    private ServiceTimeseries() { }
    [JsonPropertyName("serviceName")] public string ServiceName { get; set; }
    [JsonPropertyName("dataPoints")] public List<DataPoint> DataPoints { get; set; }
    public record class DataPoint(int Score, DateTimeOffset Observed);

    [JsonPropertyName("mean")] public float Mean
    {
        get
        {
            var points = DataPoints.Select(p => p.Score).ToArray();
            var sum = (float)points.Aggregate((a, b) => a + b);
            var mean = sum / points.Length;
            return mean;
        }
    }

    [JsonPropertyName("median")] public int Median
    {
        get
        {
            var points = DataPoints
                .OrderBy(p => p.Score).Select(p => p.Score).ToArray();

            var pointsCount = points.Length;
            if (pointsCount == 0) return 0;
            if (pointsCount == 1) return points[0];
            return points[(points.Length - 1) / 2];
        }
    }

    [JsonPropertyName("mode")] public int Mode
    {
        get
        {
            var maxValue = 0;
            var maxOccurances = 0;
            var points = DataPoints.Select(p => p.Score).ToArray();
            var distinctPoints = points.Distinct();

            foreach (var distinctValue in distinctPoints)
            {
                var occurances = points.Count(p => p == distinctValue);
                if (occurances > maxOccurances)
                    maxValue = distinctValue;
            }
            return maxValue;
        }
    }

    public static ServiceTimeseries FromData(string serviceName, IEnumerable<DataPoint> dataPoints)
    {
        return new ServiceTimeseries() {
            ServiceName = serviceName,
            DataPoints = dataPoints.OrderByDescending(p => p.Observed).ToList()
        };
    }
}
