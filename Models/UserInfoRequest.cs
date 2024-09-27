namespace Agri_Smart.Models
{
    public class UserInfoRequest
    {
        public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? EstateName { get; set; }
        public bool? OnBoardingStatus { get; set; } = false;
    }
}
