namespace IranianSMSGateways.Models
{
    public class Providers
    {
        public string Website { get; set; }
        public string URL_GetCreadit { get; set; }
        public string URL_Send { get; set; }
        public string URL_SendMutiple { get; set; }
        public string URL_SendPattern { get; set; }
        public string URL_SendSchedule { get; set; }
        public ProvidesType ProvidesType { get; set; }


        public Providers(string website, string sendUrl, string sendPatternUrl,
            string? getCreditUrl,string? sendMutipleUrl,string? sendSchedule, ProvidesType providesType)
        {
            Website = website;
            URL_Send = sendUrl;
            URL_SendPattern = sendPatternUrl;
            ProvidesType = providesType;
            URL_GetCreadit = getCreditUrl;
            URL_SendMutiple =sendMutipleUrl;
            URL_SendSchedule =sendSchedule;
        }
        public readonly static Providers Farapayamak = new Providers("https://farapayamak.ir/",
            "https://rest.payamak-panel.com/api/SendSMS/SendSMS",
            "https://api.payamak-panel.com/post/Send.asmx/SendByBaseNumber2",
            "https://rest.payamak-panel.com/api/SendSMS/GetCredit",
            "https://rest.payamak-panel.com/api/SendSMS/SendMultipleSMS",
            "https://rest.payamak-panel.com/api/SendSMS/SendSchedule",
            ProvidesType.farapayamak);

        public readonly static Providers IpPanel = new Providers("https://ippanel.com/",
            "https://ippanel.com/services.jspd",
            "http://sms.rangine.ir/patterns/pattern?",
            null,
            null,
            null,
            ProvidesType.ippanel);

        public readonly static Providers AllSmsSend = new Providers("https://allsmssend.ir/",
            "https://allsmssend.ir/api/SendSMS/SendSMS",
            "https://allsmssend.ir/api/SendSMS/SendSMS",
            null,
            null,
            null,
            ProvidesType.allsmssend);
    }

    public enum ProvidesType
    {
        ippanel = 0,
        farapayamak = 1,
        allsmssend = 2
    }


}
