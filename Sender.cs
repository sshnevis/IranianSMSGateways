using IranianSMSGateways.DTOs;
using IranianSMSGateways.Models;
using IranianSMSGateways.Services.IRepositories;
using System.Threading.Tasks;

namespace IranianSMSGateways
{
    public class Sender
    {
        public async Task<ResponseSMS> SendSmsAsync(SendSmsDTO sendSmsDTO)
        {
            ResponseSMS response = new ResponseSMS();

            if (sendSmsDTO.ProvidesType == ProvidesType.farapayamak)
            {
                SendSmsFaraPayamk sendSmsFaraPayamk = new SendSmsFaraPayamk();
                response = await sendSmsFaraPayamk.SendSmsAsync(sendSmsDTO);
            }
            else if (sendSmsDTO.ProvidesType == ProvidesType.allsmssend)
            {
                SendSmsAllSmsSender sendSmsAllSmsSender = new SendSmsAllSmsSender();
                response = await sendSmsAllSmsSender.SendSmsAsync(sendSmsDTO);
            }
            else if (sendSmsDTO.ProvidesType == ProvidesType.ippanel)
            {
                SendSmsIpPanel sendSmsIpPanel = new SendSmsIpPanel();
                response = await sendSmsIpPanel.SendSmsAsync(sendSmsDTO);
            }
            return response;
        }

        public async Task<ResponseSMS> SendSmsByPatternAsync(SendSmsByPatternDTO sendSmsbyPatternDTO)
        {
            ResponseSMS response = new ResponseSMS();

            if (sendSmsbyPatternDTO.ProvidesType == ProvidesType.farapayamak)
            {
                SendSmsFaraPayamk sendSmsFaraPayamk = new SendSmsFaraPayamk();
                response = await sendSmsFaraPayamk.SendSmsByPatternAsync(sendSmsbyPatternDTO);
            }
            else if (sendSmsbyPatternDTO.ProvidesType == ProvidesType.allsmssend)
            {
                SendSmsAllSmsSender sendSmsAllSmsSender = new SendSmsAllSmsSender();
                response = await sendSmsAllSmsSender.SendSmsByPatternAsync(sendSmsbyPatternDTO);
            }
            else if (sendSmsbyPatternDTO.ProvidesType == ProvidesType.ippanel)
            {
                SendSmsIpPanel sendSmsIpPanel = new SendSmsIpPanel();
                response = await sendSmsIpPanel.SendSmsByPatternAsync(sendSmsbyPatternDTO);
            }
            return response;
        }

    }
}
