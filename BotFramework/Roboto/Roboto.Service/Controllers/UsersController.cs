namespace Roboto.Service.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Roboto.Models.Dto;
    using Roboto.Repository;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using AutoMapper;
    using Roboto.Service.Services;

    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILinkService _linkService;
        private readonly LinkGenerator _linkGenerator;

        public UsersController(IUserRepository userRepository, 
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILinkService linkService,
            LinkGenerator linkGenerator)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _linkService = linkService;
            _linkGenerator = linkGenerator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            UserDto userDto = new UserDto();
            _mapper.Map(user, userDto);
            userDto.CreateLinks(_linkService, _linkGenerator, _httpContextAccessor);
            return Ok(userDto);
        }

    }
}