using IranianSMSGateways.Models;

namespace IranianSMSGateways.DTOs
{
    public class SendSmsDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Text { get; set; }
        public ProvidesType ProvidesType { get; set; }
    }
    public class SendSmsByPatternDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Text { get; set; }
        public string BodyId { get; set; }
        public string[] Parameters { get; set; }
        public ProvidesType ProvidesType { get; set; }
    }
}
