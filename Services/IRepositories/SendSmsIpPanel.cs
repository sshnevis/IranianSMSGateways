using IranianSMSGateways.Convertor;
using IranianSMSGateways.DTOs;
using IranianSMSGateways.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace IranianSMSGateways.Services.IRepositories
{
    public class SendSmsIpPanel : ISendSms
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

        public async Task<ResponseSMS> SendSmsAsync(SendSmsDTO ipPanel)
        {
            ResponseSMS responseSMS = new ResponseSMS();
            Providers providers = Providers.IpPanel;
            try
            {
                string url = providers.URL_Send;

                var rcpt_nm = new List<string> { ipPanel.To };
                ipPanel.Text = BadWords.CheckJomle(ipPanel.Text);
                var param = new Dictionary<string, string>
                  {
                      { "uname", ipPanel.UserName },
                      { "pass", ipPanel.Password },
                      { "from", ipPanel.From },
                      { "message", ipPanel.Text },
                      { "to", JsonConvert.ListToJson(rcpt_nm) },
                      { "op", "send" }
                  };

                using var client = new HttpClient();
                var content = new FormUrlEncodedContent(param);

                var response = await client.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                var responseData = JsonConvert.DeserializeJsonArray(responseString);
                if (responseData != null)
                {
                    if (responseData[0] == "0")
                    {
                        responseSMS.IsSuccess = true;
                        Result result = new Result()
                        {
                            Code = responseData?[0],
                            Message = responseData?[1]
                        };
                        responseSMS.ResCode = 200;
                        responseSMS.Result = result;
                    }
                    else
                    {
                        responseSMS.IsSuccess = false;
                        Result result = new Result()
                        {
                            Code = responseData?[0],
                            Message = responseData?[1]
                        };
                        responseSMS.ResCode = 200;
                        responseSMS.Result = result;
                    }
                }
            }
            catch (Exception ex)
            {
                responseSMS.Error = $"Error: {ex.Message}";
                responseSMS.IsSuccess = false;
                responseSMS.ResCode = 500;
            }
            return responseSMS;
        }

        /// <summary>
        /// text => { "name": "sina" , "lastname": "shiri" }
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseSMS> SendSmsByPatternAsync(SendSmsByPatternDTO dto)
        {
            ResponseSMS responseSMS = new ResponseSMS();
            Providers providers = Providers.IpPanel;
            try
            {
                HttpClient client = new HttpClient();
                string url = providers.URL_SendPattern +
                    $"username={dto.UserName}&password={dto.Password}&from={dto.From}&to=[\"{dto.To}\"]&" +
                    $"input_data={dto.Text}&pattern_code={dto.BodyId}";

                string baseUrl = "http://sms.rangine.ir/patterns/pattern";

                // ساخت کوئری با انکودینگ هر پارامتر
                var queryParams = new System.Collections.Generic.Dictionary<string, string>
                {
                    { "username", dto.UserName },
                    { "password", dto.Password },
                    { "from", dto.From },
                    { "to", $"[\"{dto.To}\"]" }, // اضافه کردن براکت و نقل قول
                    { "input_data", dto.Text }, // JSON را مستقیم پاس می‌دهیم
                    { "pattern_code", dto.BodyId }
                };

                // انکودینگ کوئری
                string queryString = string.Join("&", queryParams.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));

                string fullUrl = $"{baseUrl}?{queryString}";

                var responseString = await client.GetAsync(fullUrl).Result.Content.ReadAsStringAsync();//url=ghadim
                var responseData = responseString;
                responseSMS.IsSuccess = true;
                Result result = new Result()
                {
                    Code = "1",
                    Message = "Sent"
                };
                responseSMS.ResCode = 200;
                responseSMS.Result = result;
            }
            catch (Exception ex)
            {
                responseSMS.Error = $"Error: {ex.Message}";
                responseSMS.IsSuccess = false;
                responseSMS.ResCode = 500;
            }
            return responseSMS;
        }
    }
}
