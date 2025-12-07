using IranianSMSGateways.Models;
using System;
using System.Collections.Generic;

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
    public class GetCreditDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class SendScheduleDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Text { get; set; }
        public DateTime ScheduleDate { get; set; }
        public int Period { get; set; }
    }
    public class SendMultipleDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<string> To { get; set; }
        public string From { get; set; }
        public List<string> Text { get; set; }
    }
}
