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


        public Providers(string website, string sendUrl, string sendPatternUrl, ProvidesType providesType)
        {
            Website = website;
            URL_Send = sendUrl;
            URL_SendPattern = sendPatternUrl;
            ProvidesType = providesType;
        }
        public readonly static Providers Farapayamak = new Providers("https://farapayamak.ir/",
            "https://rest.payamak-panel.com/api/SendSMS/SendSMS",
            "https://api.payamak-panel.com/post/Send.asmx/SendByBaseNumber2",
            ProvidesType.farapayamak);

        public readonly static Providers IpPanel = new Providers("https://ippanel.com/",
            "https://ippanel.com/services.jspd",
            "http://sms.rangine.ir/patterns/pattern?",
            ProvidesType.ippanel);

        public readonly static Providers AllSmsSend = new Providers("https://allsmssend.ir/",
            "https://allsmssend.ir/api/SendSMS/SendSMS",
            "https://allsmssend.ir/api/SendSMS/SendSMS",
            ProvidesType.allsmssend);
    }

    public enum ProvidesType
    {
        ippanel = 0,
        farapayamak = 1,
        allsmssend = 2
    }


}
