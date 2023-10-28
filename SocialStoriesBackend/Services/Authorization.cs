using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SocialStoriesBackend.Services;

public static class Authorization
{ 
    public static readonly SymmetricSecurityKey SecurityKey = new(Encoding.UTF8.GetBytes("jK23xmicwr$s^V*P^3^jypPmG8BaCzfVvkLumWHGor3cfABY3QYTE$vs&9%%rUkQRiUcVxNvNioN%g$9SBF2uBpzkDz8XBZGYxDjGJ!RQW&k5WQw^&27FEbq5nGH%#uJ"));
    public static readonly SigningCredentials SecurityCredentials = new(SecurityKey, SecurityAlgorithms.HmacSha512);
    public const string Issuer = "SunsetStories";
    public const string Audience = "SunsetStories users";
    public const string UserPolicy = "UserPolicy";
    public const string AdminPolicy = "AdminPolicy";
    public const string UserRole = "User";
    public const string AdminRole = "Admin";
}