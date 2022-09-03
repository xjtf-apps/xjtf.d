namespace xjtf.d;

[EnableCors]
public class AuthController : ControllerBase
{
    private readonly DaemonDbContext _dbContext;
    public AuthController(DaemonDbContext dbContext) => _dbContext = dbContext;

    [AllowAnonymous][HttpGet][Route("/Login")]
    public async Task<IActionResult> LoginAsync(string username, string passwordHash)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.UserName == username && u.PasswordHash == passwordHash);

        if (user == null)
            return Unauthorized();

        return LoginUser(user);
    }

    [Authorize][HttpGet][Route("/Logout")]
    public IActionResult Logout()
    {
        var anonymousUser = new ClaimsPrincipal();
        SetPrincipal(anonymousUser);
        return Ok();
    }

    #region helpers
    private IActionResult LoginUser(IdentityUser user)
    {
        var issuer = Program.JwtIssuer;
        var audience = Program.JwtAudience;
        var key = Encoding.UTF8.GetBytes(Program.JwtKey);
        var identity = new ClaimsIdentity(new[]
        {
            new Claim("Id", Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        });
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = identity,
            Expires = DateTime.UtcNow.AddMinutes(5),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature
            )
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        var principal = new ClaimsPrincipal();
        principal.AddIdentity(identity);
        SetPrincipal(principal);
        return Ok(jwtToken);
    }

    private void SetPrincipal(ClaimsPrincipal principal)
    {
        Thread.CurrentPrincipal = principal;
        HttpContext.User = principal;
    }
    #endregion
}
