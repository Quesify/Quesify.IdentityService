using Quesify.IdentityService.API.Models;
using Quesify.IdentityService.Core.Entities;

namespace Quesify.IdentityService.API.Mappers;

public class UserMapper
{
    public static UserResponse Map(User user, UserResponse response)
    {
        response.Id = user.Id;
        response.UserName = user.UserName!;
        response.Score = user.Score;
        response.About = user.About;
        response.Location = user.Location;
        response.BirthDate = user.BirthDate;
        response.WebSiteUrl = user.WebSiteUrl;
        response.ProfileImageUrl = user.ProfileImageUrl;
        response.CreationDate = user.CreationDate;
        return response;
    }

    public static User Map(UserForUpdateRequest request, User user)
    {
        user.About = request.About;
        user.Location = request.Location;
        user.BirthDate = request.BirthDate;
        user.WebSiteUrl = request.WebSiteUrl;
        user.ProfileImageUrl = request.ProfileImageUrl;
        return user;
    }

    public static UserForUpdateResponse Map(User user, UserForUpdateResponse response)
    {
        response.About = user.About;
        response.Location = user.Location;
        response.BirthDate = user.BirthDate;
        response.WebSiteUrl = user.WebSiteUrl;
        response.ProfileImageUrl = user.ProfileImageUrl;
        return response;
    }
}
