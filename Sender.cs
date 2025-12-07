using IranianSMSGateways.DTOs;
using IranianSMSGateways.Models;
using IranianSMSGateways.Services.IRepositories;
using System.Threading.Tasks;

namespace IranianSMSGateways
{
    public class Sender
    {
        public async Task<ResponseSMS> GetCreditAsync(GetCreditDTO getCreditDTO)
        {
            ResponseSMS response = new ResponseSMS();

            if (getCreditDTO.ProvidesType == ProvidesType.farapayamak)
            {
                SendSmsFaraPayamk sendSmsFaraPayamk = new SendSmsFaraPayamk();
                response = await sendSmsFaraPayamk.GetCredit(getCreditDTO);
            }
            else if (getCreditDTO.ProvidesType == ProvidesType.allsmssend)
            {
                SendSmsAllSmsSender sendSmsAllSmsSender = new SendSmsAllSmsSender();
                response = await sendSmsAllSmsSender.GetCredit(getCreditDTO);
            }
            else if (getCreditDTO.ProvidesType == ProvidesType.ippanel)
            {
                SendSmsIpPanel sendSmsIpPanel = new SendSmsIpPanel();
                response = await sendSmsIpPanel.GetCredit(getCreditDTO);
            }
            return response;
        }
        public async Task<ResponseSMS> SendScheduleAsync(SendScheduleDTO sendScheduleDTO)
        {
            ResponseSMS response = new ResponseSMS();

            if (sendScheduleDTO.ProvidesType == ProvidesType.farapayamak)
            {
                SendSmsFaraPayamk sendSmsFaraPayamk = new SendSmsFaraPayamk();
                response = await sendSmsFaraPayamk.SendSchedule(sendScheduleDTO);
            }
            else if (sendScheduleDTO.ProvidesType == ProvidesType.allsmssend)
            {
                SendSmsAllSmsSender sendSmsAllSmsSender = new SendSmsAllSmsSender();
                response = await sendSmsAllSmsSender.SendSchedule(sendScheduleDTO);
            }
            else if (sendScheduleDTO.ProvidesType == ProvidesType.ippanel)
            {
                SendSmsIpPanel sendSmsIpPanel = new SendSmsIpPanel();
                response = await sendSmsIpPanel.SendSchedule(sendScheduleDTO);
            }
            return response;
        }
        public async Task<ResponseSMS> SendMultipleAsync(SendMultipleDTO sendMultipleDTO)
        {
            ResponseSMS response = new ResponseSMS();

            if (sendMultipleDTO.ProvidesType == ProvidesType.farapayamak)
            {
                SendSmsFaraPayamk sendSmsFaraPayamk = new SendSmsFaraPayamk();
                response = await sendSmsFaraPayamk.SendMultiple(sendMultipleDTO);
            }
            else if (sendMultipleDTO.ProvidesType == ProvidesType.allsmssend)
            {
                SendSmsAllSmsSender sendSmsAllSmsSender = new SendSmsAllSmsSender();
                response = await sendSmsAllSmsSender.SendMultiple(sendMultipleDTO);
            }
            else if (sendMultipleDTO.ProvidesType == ProvidesType.ippanel)
            {
                SendSmsIpPanel sendSmsIpPanel = new SendSmsIpPanel();
                response = await sendSmsIpPanel.SendMultiple(sendMultipleDTO);
            }
            return response;
        }

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
