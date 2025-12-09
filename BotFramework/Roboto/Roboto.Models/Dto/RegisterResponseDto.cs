namespace Roboto.Models.Dto
{
    public class RegisterResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
