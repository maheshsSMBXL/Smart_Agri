namespace Agri_Smart.data
{
    public class Intractions
    {
        public Guid Id { get; set; }        
        public int? EmailOtp { get; set; }
        public int? SmsOtp { get; set; }
        public DateTime? NotBefore { get; set; }
        public DateTime? NotAfter { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? EmailVerified { get; set; }
        public bool? PhoneVerified { get; set; }
        public Guid? UserId { get; set; }
        public int? CustomerId { get; set; }
    }
}
