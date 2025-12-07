using IranianSMSGateways.Convertor;
using IranianSMSGateways.DTOs;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IranianSMSGateways.Services.IRepositories
{
    public class SendSmsAllSmsSender : ISendSms
    {
        public async Task<ResponseSMS> GetCredit(GetCreditDTO dto)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseSMS> SendMultiple(SendMultipleDTO dto)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseSMS> SendSchedule(SendScheduleDTO dto)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseSMS> SendSmsAsync(SendSmsDTO allSmsSend)
        {
            ResponseSMS responseSMS = new ResponseSMS();
            try
            {
                using var client = new HttpClient();
                string url = "https://allsmssend.ir/api/apicore/SendIPPanel";

                var json = JsonConvert.SerializeObject(allSmsSend);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeJsonArray(responseString);
                    var res_code = responseData?[0];
                    var res_data = responseData?[1];
                }

            }
            catch (Exception ex)
            {
                responseSMS.Error = ex.Message;
            }
            return responseSMS;
        }

        public async Task<ResponseSMS> SendSmsByPatternAsync(SendSmsByPatternDTO allSmsSend)
        {
            ResponseSMS responseSMS = new ResponseSMS();
            try
            {
                using var client = new HttpClient();
                string url = "https://allsmssend.ir/api/apicore/SendIPPanel";

                var json = JsonConvert.SerializeObject(allSmsSend);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeJsonArray(responseString);
                    var res_code = responseData?[0];
                    var res_data = responseData?[1];
                }

            }
            catch (Exception ex)
            {
                responseSMS.Error = ex.Message;
            }
            return responseSMS;
        }
    }
}
