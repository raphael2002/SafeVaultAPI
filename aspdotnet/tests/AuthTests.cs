using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeVault.Api.Controllers;
using SafeVault.Api.Data;
using SafeVault.Api.Models;
using SafeVault.Api.Services;
using Xunit;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

public class AuthTests
{
    private SafeVaultDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<SafeVaultDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;
        return new SafeVaultDbContext(options);
    }

    private AuthService CreateAuthService()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Jwt:Key", "test-key" },
                { "Jwt:Issuer", "test" },
                { "Jwt:Audience", "test" }
            })
            .Build();

        return new AuthService(config);
    }

    [Fact]
    public async void Register_ValidUser_ReturnsCreated()
    {
        var db = CreateDb();
        var auth = CreateAuthService();
        var controller = new AuthController(db, auth);

        var user = new UserAccount
        {
            Username = "alice",
            Email = "alice@example.com",
            PasswordHash = "Password123!",
            Role = "User"
        };

        var result = await controller.Register(user);

        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async void Login_InvalidCredentials_ReturnsUnauthorized()
    {
        var db = CreateDb();
        var auth = CreateAuthService();
        var controller = new AuthController(db, auth);

        var request = new LoginRequest
        {
            Username = "unknown",
            Password = "wrong"
        };

        var result = await controller.Login(request);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}
