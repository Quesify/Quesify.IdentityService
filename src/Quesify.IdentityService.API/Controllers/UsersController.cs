using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Quesify.IdentityService.API.IntegrationEvents.Events;
using Quesify.IdentityService.API.Mappers;
using Quesify.IdentityService.API.Models;
using Quesify.IdentityService.Core.Entities;
using Quesify.SharedKernel.AspNetCore.Controllers;
using Quesify.SharedKernel.AspNetCore.Filters;
using Quesify.SharedKernel.EventBus.Abstractions;
using Quesify.SharedKernel.Guids;
using Quesify.SharedKernel.Security.Users;
using Quesify.SharedKernel.Utilities.Exceptions;
using Quesify.SharedKernel.Utilities.TimeProviders;

namespace Quesify.IdentityService.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class UsersController : BaseController
{
    private readonly UserManager<User> _userManager;
    private readonly ICurrentUser _currentUser;
    private readonly IDateTime _dateTime;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IEventBus _eventBus;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        UserManager<User> userManager,
        ICurrentUser currentUser,
        IDateTime dateTime,
        IGuidGenerator guidGenerator,
        IEventBus eventBus,
        ILogger<UsersController> logger)
    {
        _userManager = userManager;
        _currentUser = currentUser;
        _dateTime = dateTime;
        _guidGenerator = guidGenerator;
        _eventBus = eventBus;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            throw new NotFoundException($"User {id} was not found.");
        }

        return OkResponse(data: UserMapper.Map(user, new UserResponse()));
    }

    [HttpPut]
    [Authorize]
    [Transaction]
    public async Task<IActionResult> UpdateAsync(UserForUpdateRequest request)
    {
        var user = await _userManager.FindByEmailAsync(_currentUser.Email!);
        if (user == null)
        {
            throw new NotFoundException($"User {_currentUser.Email} was not found.");
        }

        UserMapper.Map(request, user);

        var updateUserResult = await _userManager.UpdateAsync(user);
        if (!updateUserResult.Succeeded)
        {
            throw new BusinessException(errors: updateUserResult);
        }

        await _eventBus.PublishAsync(
            new UserUpdatedIntegrationEvent(
                user.Id, user.ProfileImageUrl, _guidGenerator.Generate(), _dateTime.Now));

        return OkResponse(data: UserMapper.Map(user, new UserForUpdateResponse()));
    }
}
