namespace xjtf.d;

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }

    public List<Role> Roles { get; } = new();
}
