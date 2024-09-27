using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class UserInfo
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Name { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Country { get; set; } = string.Empty;
        public string? State { get; set; } = string.Empty;
        public string? District { get; set; } = string.Empty;
        public string? ZipCode { get; set; } = string.Empty;
        public double? LandSize { get; set; } = 0.0;
        public string? FarmAddress { get; set; } = string.Empty;
        public bool? DeviceStatus { get; set; } = false;
        public string? CropType { get; set; } = string.Empty;
        public string? SoilType { get; set; } = string.Empty;
        public DateTime? CropGrowingStartDate { get; set; } = DateTime.MinValue;
        public bool? OnBoardingStatus { get; set; } = false;
        public string? TenantId { get; set; } = string.Empty;
        public string? FireBaseToken { get; set; } = string.Empty;
        public double? Latitude { get; set; } = 0.0;
        public double? Longitude { get; set; } = 0.0;
        public double? BudgetAmount { get; set; } = 0.0;
        public DateTime? UserCreatedDate { get; set; } = DateTime.MinValue;
        public string? EstateName { get; set; } = string.Empty;
    }


}
