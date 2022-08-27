namespace xjtf.d;

public class Init
{
    public Init
    (
        IHostApplicationLifetime hostApplicationLifetime
    )
    {
        Program.ApplicationLifetime = hostApplicationLifetime;
    }
}