using System.ComponentModel.DataAnnotations;

namespace Agri_Smart.data
{
    public class UserInfo
    {
        [Key]
        public Guid Id { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? District { get; set; }
        public string? ZipCode { get; set; }
        public double? LandSize { get; set; }
        public string? FarmAddress { get; set; }
        public bool? DeviceStatus { get; set; }
        public string? CropType { get; set; }
        public string? SoilType { get; set; }
        public DateTime? CropGrowingStartDate { get; set; }
        public bool? OnBoardingStatus { get; set; }
        public string? TenantId { get; set; }
        public string? FireBaseToken { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? BudgetAmount { get; set; }
        public DateTime? UserCreatedDate { get; set; }
    }
}
