namespace Agri_Smart.Models
{
    public class MapTransmitterWithReceiverRequest
    {
        public List<string>? TransmitterMacIds { get; set; }
        public string? ReceiverMacId { get; set; }
    }
    public class Receiver 
    {
        public string? ReceiverMacId { get; set; }
    }
}
