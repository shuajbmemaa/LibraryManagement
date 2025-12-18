namespace LMS.Application.DTO.Response.User
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public IList<string> Roles { get; set; }
    }
}
