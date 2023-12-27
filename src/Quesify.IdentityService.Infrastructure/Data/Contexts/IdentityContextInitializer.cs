using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quesify.IdentityService.Core.Constants;
using Quesify.IdentityService.Core.Entities;
using Quesify.IdentityService.Infrastructure.Data.Contexts;
using Quesify.IdentityService.Infrastructure.IntegrationEvents.Events;
using Quesify.SharedKernel.EventBus.Abstractions;
using Quesify.SharedKernel.Guids;
using Quesify.SharedKernel.Utilities.TimeProviders;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<IdentityContextInitializer>();
        await initializer.InitializeAsync();
        await initializer.SeedAsync();
    }
}

public class IdentityContextInitializer
{
    private readonly ILogger<IdentityContextInitializer> _logger;
    private readonly IdentityContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IDateTime _dateTime;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IEventBus _eventBus;

    public IdentityContextInitializer(
        ILogger<IdentityContextInitializer> logger,
        IdentityContext context,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IDateTime dateTime,
        IGuidGenerator guidGenerator,
        IEventBus eventBus)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _dateTime = dateTime;
        _guidGenerator = guidGenerator;
        _eventBus = eventBus;
    }

    public async Task InitializeAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred while initializing the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        await CreateRoleAsync(new Role(RoleConstants.Administrator));
        await CreateRoleAsync(new Role(RoleConstants.Moderator));
        await CreateRoleAsync(new Role(RoleConstants.User));

        var administrator = new User
        {
            UserName = "CustomAdmin",
            Email = "admin@quesify.com",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            CreationDate = _dateTime.Now
        };

        if (_userManager.Users.All(u => u.Email != administrator.Email))
        {
            var result = await _userManager.CreateAsync(administrator, "Enes123!");
            await _userManager.AddToRolesAsync(administrator, new[] { RoleConstants.Administrator });

            await _eventBus.PublishAsync(
                new UserCreatedIntegrationEvent(
                    administrator.Id,
                    administrator.UserName,
                    _guidGenerator.Generate(),
                    _dateTime.Now
                )
            );
        }
    }

    private async Task CreateRoleAsync(Role role)
    {
        if (_roleManager.Roles.All(r => r.Name != role.Name))
        {
            await _roleManager.CreateAsync(role);
        }
    }
}