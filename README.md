# IranianSMSGateways

این یک پروژه بر اساس net standard 2 هست که قابلیت استفاده در تمام ورژن های دات نت و دات نت کور را دارد. فقط با یک خط کد ساده قابل فراخوانی است:
```
SendSmsDTO sendSmsDTO = new SendSmsDTO()
{
    UserName = "talashnet",
    Password = "00000",
    From = null,
    Text = "hello",
    To = "09121234567",
    ProvidesType = ProvidesType.farapayamak,
};

Sender senderFara = new Sender();
await senderFara.SendSmsAsync(sendSmsDTO);
```

# Providers support till now:
- 1.IPPanel
- 2.FaraPayamak (Payamak-panel)
- 3.AllSMSSend

> اگر میتوانید پنل خود را اضافه کنید، فورک کنید و اضافه کنید و پول ریکوئست بدهید قبول میکنم.
