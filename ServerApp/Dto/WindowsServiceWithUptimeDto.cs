namespace xjtf.d.ui._2;

public class WindowsServiceWithUptimeDto : WindowsServiceDto
{
    public required UptimeDto[] Uptime { get; set; }
    public required UptimeDto[] DetailedUptime { get; set; }
}
