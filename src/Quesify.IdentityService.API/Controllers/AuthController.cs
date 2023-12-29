using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Quesify.IdentityService.API.Models;
using Quesify.IdentityService.Core.Constants;
using Quesify.IdentityService.Core.Entities;
using Quesify.IdentityService.Infrastructure.IntegrationEvents.Events;
using Quesify.SharedKernel.AspNetCore.Controllers;
using Quesify.SharedKernel.AspNetCore.Filters;
using Quesify.SharedKernel.EventBus.Abstractions;
using Quesify.SharedKernel.Guids;
using Quesify.SharedKernel.Security.Tokens;
using Quesify.SharedKernel.Utilities.Exceptions;
using Quesify.SharedKernel.Utilities.TimeProviders;
using System.Text;

namespace Quesify.IdentityService.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : BaseController
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IDateTime _dateTime;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IEventBus _eventBus;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ITokenService tokenService,
        IDateTime dateTime,
        IGuidGenerator guidGenerator,
        IEventBus eventBus,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _dateTime = dateTime;
        _guidGenerator = guidGenerator;
        _eventBus = eventBus;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(UserForLoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new BusinessException(errors: UserForLoginResponse.Fail());
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);
        if (signInResult.Succeeded)
        {
            var accessToken = _tokenService.CreateAccessToken(new AccessTokenClaims()
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = (await _userManager.GetRolesAsync(user)).ToArray()
            });

            _logger.LogInformation("User {Email} logged in successfully.", request.Email);
            return OkResponse(data: UserForLoginResponse.Success(accessToken));
        }

        _logger.LogWarning("User {Email} failed to login.", request.Email);
        throw new BusinessException(errors: (UserForLoginResponse)signInResult);
    }


    [HttpPost("register")]
    [Transaction]
    public async Task<IActionResult> RegisterAsync(UserForRegisterRequest request)
    {
        var user = new User()
        {
            UserName = request.UserName,
            Email = request.Email,
            CreationDate = _dateTime.Now
        };

        var createUserResult = await _userManager.CreateAsync(user, request.Password);
        if (!createUserResult.Succeeded)
        {
            throw new BusinessException(errors: UserForRegisterResponse.Fail(createUserResult.Errors));
        }

        var addToRoleResult = await _userManager.AddToRolesAsync(user, new[] { RoleConstants.User });
        if (!addToRoleResult.Succeeded)
        {
            throw new BusinessException(errors: UserForRegisterResponse.Fail(addToRoleResult.Errors));
        }

        _logger.LogInformation("User {Email} created a new account with password.", request.Email);

        await _eventBus.PublishAsync(
            new UserCreatedIntegrationEvent(
                user.Id, user.UserName, _guidGenerator.Generate(), _dateTime.Now));

        return CreatedResponse(null, UserForRegisterResponse.Succees());
    }

    [HttpPatch("confirm-email")]
    public async Task<IActionResult> ConfirmEmailAsync(UserForConfirmEmailRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            throw new NotFoundException($"User {request.UserId} was not found.");
        }

        request.Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
        var confirmEmailRequest = await _userManager.ConfirmEmailAsync(user, request.Code);
        if (!confirmEmailRequest.Succeeded)
        {
            throw new BusinessException(errors: UserForConfirmEmailResponse.Fail(confirmEmailRequest.Errors));
        }

        return OkResponse(data: UserForConfirmEmailResponse.Succees());
    }
}
