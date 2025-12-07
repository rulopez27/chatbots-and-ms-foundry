namespace Roboto.Models.Dto
{
    public class LoginDto
    {
        public string UsernameOrEmail { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}