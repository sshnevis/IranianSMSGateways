namespace IranianSMSGateways.DTOs
{
    public class ResponseSMS
    {
        public bool IsSuccess { get; set; }
        public int ResCode { get; set; }
        public string Error { get; set; }
        public Result Result { get; set; }
    }
    public class Result
    {
        public string Data { get; set; }
        public string? Code { get; set; }
        public string Message { get; set; }
    }

}
