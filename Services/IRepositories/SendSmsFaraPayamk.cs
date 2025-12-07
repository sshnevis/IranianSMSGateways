using IranianSMSGateways.Convertor;
using IranianSMSGateways.DTOs;
using IranianSMSGateways.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IranianSMSGateways.Services.IRepositories
{
    public class SendSmsFaraPayamk : ISendSms
    {
        public async Task<ResponseSMS> GetCredit(GetCreditDTO dto)
        {
            ResponseSMS responseSMS = new ResponseSMS();

            try
            {
                using var client = new HttpClient();
                Providers providers = Providers.Farapayamak;

                string url = providers.URL_GetCreadit;

                var sendObj = new
                {
                    username = dto.UserName, 
                    password = dto.Password
                };

                var json = JsonConvert.SerializeObject(sendObj);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                var responseData = JsonConvert.DeserializeObject<ApiResponseFaraPayamak>(responseString);

                if (responseData != null && responseData.RetStatus == 1)
                {
                    responseSMS.IsSuccess = true;
                }
                else
                {
                    responseSMS.IsSuccess = false;
                }

                Result result = new Result()
                {
                    Code = responseData?.Value, 
                    Data = responseData?.RetStatus.ToString(),
                    Message = responseData?.StrRetStatus
                };

                responseSMS.ResCode = (int)response.StatusCode;
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

        public async Task<ResponseSMS> SendMultiple(SendMultipleDTO dto)
        {
            var responseSMS = new ResponseSMS();

            try
            {
                using var client = new HttpClient();
                Providers providers = Providers.Farapayamak;

                string url = providers.URL_SendMutiple;

                var cleanedTexts = new List<string>();
                foreach (var txt in dto.Text)
                {
                    if (string.IsNullOrWhiteSpace(txt)) continue;
                    cleanedTexts.Add(BadWords.CheckJomle(txt));
                }
                var cleanedNumbers = new List<string>();
                foreach (var mobile in dto.To)
                {
                    if (string.IsNullOrWhiteSpace(mobile)) continue;

                    string cleaned = mobile.Trim();
                    cleanedNumbers.Add(cleaned);
                }

                // محدودیت 100 مورد طبق داک
                cleanedNumbers = cleanedNumbers.Take(100).ToList();
                cleanedTexts = cleanedTexts.Take(100).ToList();

                // ۳) ساخت Body درخواست
                var sendObj = new
                {
                    username = dto.UserName,
                    password = dto.Password,
                    from = dto.From,
                    to = cleanedNumbers.ToArray(),     
                    text = cleanedTexts.ToArray()       
                };

                var json = JsonConvert.SerializeObject(sendObj);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                var responseData = JsonConvert.DeserializeObject<ApiResponseFaraPayamak>(responseString);

                if (responseData != null && responseData.RetStatus == 1)
                {
                    responseSMS.IsSuccess = true;
                }
                else
                {
                    responseSMS.IsSuccess = false;
                }

                Result result = new Result()
                {
                    Code = responseData?.Value,
                    Data = responseData?.RetStatus.ToString(),
                    Message = responseData?.StrRetStatus
                };

                responseSMS.ResCode = (int)response.StatusCode;
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

        public async Task<ResponseSMS> SendSchedule(SendScheduleDTO dto)
        {
            ResponseSMS responseSMS = new ResponseSMS();

            try
            {
                using var client = new HttpClient();
                Providers providers = Providers.Farapayamak;

                string url = providers.URL_SendSchedule;

                dto.Text = BadWords.CheckJomle(dto.Text);

                string cleanedTo = dto.To?.Trim();

                string scheduleDateString = dto.ScheduleDate
                    .ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                var sendObj = new
                {
                    username = dto.UserName,
                    password = dto.Password,
                    to = cleanedTo,              
                    from = dto.From,
                    text = dto.Text,
                    scheduleDate = scheduleDateString,
                    period = dto.Period     
                };

                var json = JsonConvert.SerializeObject(sendObj);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                var responseData = JsonConvert.DeserializeObject<ApiResponseFaraPayamak>(responseString);

                if (responseData != null && responseData.RetStatus == 1)
                {
                    responseSMS.IsSuccess = true;
                }
                else
                {
                    responseSMS.IsSuccess = false;
                }

                Result result = new Result()
                {
                    Code = responseData?.Value,               
                    Data = responseData?.RetStatus.ToString(),
                    Message = responseData?.StrRetStatus
                };

                responseSMS.ResCode = (int)response.StatusCode;
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

        

        public async Task<ResponseSMS> SendSmsAsync(SendSmsDTO farapayamak)
        {
            ResponseSMS responseSMS = new ResponseSMS();
            try
            {
                farapayamak.Text = BadWords.CheckJomle(farapayamak.Text);
                using var client = new HttpClient();
                Providers providers = Providers.Farapayamak;
                string url = providers.URL_Send;
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

                if (responseData.RetStatus == 1)
                {
                    responseSMS.IsSuccess = true;
                }

                Result result = new Result()
                {
                    Code = responseData.Value,
                    Data = responseData.RetStatus.ToString(),
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

        /// <summary>
        /// text =>   sina;shiri;20;
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseSMS> SendSmsByPatternAsync(SendSmsByPatternDTO dto)
        {
            ResponseSMS responseSMS = new ResponseSMS();
            try
            {
                Providers providers = Providers.Farapayamak;
                var url = providers.URL_SendPattern;

                var data = $"username={dto.UserName}&password={dto.Password}&to={dto.To}&text={dto.Text}&bodyId={dto.BodyId}";

                using var client = new HttpClient();

                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded")
                };

                request.Headers.Add("cache-control", "no-cache");
                var response = await client.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responseString))
                {
                    Result result = new Result();

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(responseString);

                    string numberString = xmlDoc.GetElementsByTagName("string")[0].InnerText;

                    // حالا می‌تونی به long تبدیل کنی (چون عدد بزرگه)
                    if (long.TryParse(numberString, out long number))
                    {
                        if (number > 1000)
                        {
                            result.Code = numberString;
                            responseSMS.IsSuccess = true;
                            responseSMS.ResCode = 200;
                            responseSMS.Result = result;
                        }
                    }
                    else
                    {
                        result.Message = responseString;

                        responseSMS.IsSuccess = false;
                        responseSMS.Error = $"Error: عدم پاسخ مناسب";
                        responseSMS.ResCode = 350;
                        responseSMS.Result = result;
                    }
                }
            }
            catch (Exception ex)
            {
                responseSMS.IsSuccess = false;
                responseSMS.Error = $"Error: {ex.Message}";
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
