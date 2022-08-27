namespace xjtf.d;

public class ServiceEntryState
{
    public int ServiceEntryStateId { get; set; }
    public ServiceEntry ServiceEntry { get; set; }
    public DateTime ChangedOnUtc { get; set; }
    public User ChangedBy { get; set; }
    public bool DaemonManaged { get; set; }
}
