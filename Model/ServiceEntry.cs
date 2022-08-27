namespace xjtf.d;

public class ServiceEntry
{
    public int ServiceEntryId { get; set; }
    public string ServiceName { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public string ExecutablePath { get; set; }
    public string? Arguments { get; set; }
}

// SERVICE_STOPPED = 0;
// SERVICE_START_PENDING = 1;
// SERVICE_STOP_PENDING = 2;
// SERVICE_RUNNING = 3;
// SERVICE_CONTINUE_PENDING = 4;
// SERVICE_PAUSE_PENDING = 5;
// SERVICE_PAUSED = 6;
