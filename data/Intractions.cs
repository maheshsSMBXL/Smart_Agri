namespace Agri_Smart.data
{
    public class Intractions
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int? EmailOtp { get; set; } = 0;
        public int? SmsOtp { get; set; } = 0;
        public DateTime? NotBefore { get; set; } = DateTime.MinValue;
        public DateTime? NotAfter { get; set; } = DateTime.MinValue;
        public string? Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public bool? EmailVerified { get; set; } = false;
        public bool? PhoneVerified { get; set; } = false;
        public Guid? UserId { get; set; } = Guid.Empty;
        public int? CustomerId { get; set; } = 0;
    }

}
