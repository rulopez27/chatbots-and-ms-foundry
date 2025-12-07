using Roboto.Models.Dto;

namespace Roboto.Service.Services
{
    public static class UserDtoExtensions
    {
        public static void CreateUserLinks(this UserDto userDto, ILinkService linkService, LinkGenerator linkGenerator, IHttpContextAccessor context)
        {
            userDto.Links.Add(linkService.Generate("Get", "Users", new{id = userDto.Id}, "self","GET"));
            userDto.Links.Add(linkService.Generate("Put", "Users", null, "update", "PUT"));
        }
    }
}