using IranianSMSGateways.Convertor;
using IranianSMSGateways.DTOs;
using IranianSMSGateways.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IranianSMSGateways.Services.IRepositories
{
    public class SendSmsFaraPayamk : ISendSms
    {
        public async Task<ResponseSMS> SendSmsAsync(SendSmsDTO farapayamak)
        {
            ResponseSMS responseSMS = new ResponseSMS();
            try
            {
                farapayamak.Text = BadWords.CheckJomle(farapayamak.Text);
                using var client = new HttpClient();
                Providers providers = Providers.Farapayamak;
                string url = providers.SendUrl;
                var sendObj = new
                {
                    userName = farapayamak.UserName,
                    password = farapayamak.Password,
                    text = farapayamak.Text,
                    to = farapayamak.To,
                    from = farapayamak.From,
                    isFlash = false,
                };

                var json = JsonConvert.SerializeObject(sendObj);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<ApiResponseFaraPayamak>(responseString);
                responseSMS.IsSuccess = true;
                Result result = new Result()
                {
                    Code = responseData.RetStatus.ToString(),
                    Data = responseData.Value,
                    Message = responseData.StrRetStatus
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

        public async Task<ResponseSMS> SendSmsByPatternAsync(SendSmsByPatternDTO dto)
        {
            ResponseSMS responseSMS = new ResponseSMS();
            try
            {
                Providers providers = Providers.Farapayamak;
                var url = providers.SendPatternUrl;

                var data = $"username={dto.UserName}&password={dto.Password}&to={dto.To}text={dto.Text}&bodyId={dto.BodyId}";

                using var client = new HttpClient();

                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded")
                };

                request.Headers.Add("cache-control", "no-cache");
                var response = await client.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<ApiResponseFaraPayamak>(responseString);
                responseSMS.IsSuccess = true;
                Result result = new Result()
                {
                    Code = responseData.RetStatus.ToString(),
                    Data = responseData.Value,
                    Message = responseData.StrRetStatus
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

    public class ApiResponseFaraPayamak
    {
        public string Value { get; set; }
        public int RetStatus { get; set; }
        public string StrRetStatus { get; set; }
    }

}
