using IranianSMSGateways.DTOs;
using System.Threading.Tasks;

namespace IranianSMSGateways.Services
{
    public interface ISendSms
    {
        Task<ResponseSMS> SendSmsAsync(SendSmsDTO dto);
        Task<ResponseSMS> SendSmsByPatternAsync(SendSmsByPatternDTO dto);
    }
}
