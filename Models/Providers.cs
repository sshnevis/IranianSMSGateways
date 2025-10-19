namespace IranianSMSGateways.Models
{
    public class Providers
    {
        public Providers(string website, string sendUrl, string sendPatternUrl, ProvidesType providesType)
        {
            Website = website;
            SendUrl = sendUrl;
            SendPatternUrl = sendPatternUrl;
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
        public string Website { get; set; }
        public string SendUrl { get; set; }
        public string SendPatternUrl { get; set; }
        public string PatternUri { get; set; }
        public ProvidesType ProvidesType { get; set; }
    }

    public enum ProvidesType
    {
        ippanel = 0,
        farapayamak = 1,
        allsmssend = 2
    }


}
