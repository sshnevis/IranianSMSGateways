# FaraPayamak SMS API — Implementation Guide

Standalone reference for adding FaraPayamak (Payamak-panel) SMS sending to a **new** system.

This file is enough to implement sending. You do not need the rest of this repository.

- Panel: [https://farapayamak.ir/](https://farapayamak.ir/)
- Same backend as Payamak-panel / Melipayamak-style APIs: `rest.payamak-panel.com` and `api.payamak-panel.com`

---

## 1. What you need from the panel

| Item | Where | Used for |
|------|--------|----------|
| `username` | Panel login | Every request |
| `password` | Panel login (or API key if the panel requires it) | Every request |
| `from` (line / sender number) | Panel → numbers, e.g. `5000xxxxxxx` | Plain SMS, multiple, schedule |
| `bodyId` (pattern / template id) | Panel → approved pattern | OTP / transactional SMS via shared service line |

Store credentials in config or secrets. Never hard-code them.

**Plain SMS** (`from` line) usually cannot reach numbers on the operator advertising blacklist.

**Pattern SMS** (`bodyId`) uses a shared service line and **can** reach blacklist numbers. Use this for OTP, login codes, and order notices.

---

## 2. Base URLs

| Kind | Base URL |
|------|----------|
| REST | `https://rest.payamak-panel.com/api/SendSMS/` |
| SOAP / HTTP form | `https://api.payamak-panel.com/post/` |

Always use HTTPS.

| Operation | Method | Full URL | Body format |
|-----------|--------|----------|-------------|
| Send plain SMS | POST | `https://rest.payamak-panel.com/api/SendSMS/SendSMS` | JSON or `application/x-www-form-urlencoded` |
| Send pattern / OTP | POST | `https://rest.payamak-panel.com/api/SendSMS/BaseServiceNumber` | JSON or form |
| Send pattern / OTP (SOAP HTTP) | POST | `https://api.payamak-panel.com/post/Send.asmx/SendByBaseNumber2` | `application/x-www-form-urlencoded` → XML response |
| Send multiple (paired texts) | POST | `https://rest.payamak-panel.com/api/SendSMS/SendMultipleSMS` | JSON |
| Send scheduled | POST | `https://rest.payamak-panel.com/api/SendSMS/SendSchedule` | JSON |
| Get credit | POST | `https://rest.payamak-panel.com/api/SendSMS/GetCredit` | JSON or form |
| Get sender numbers | POST | `https://rest.payamak-panel.com/api/SendSMS/GetUserNumbers` | JSON or form |
| Get delivery status | POST | `https://rest.payamak-panel.com/api/SendSMS/GetDeliveries2` | JSON or form |

Both JSON (`Content-Type: application/json`) and form-urlencoded work for most REST methods. Official SDKs often use form-urlencoded. JSON is fine.

Recommended HTTP timeout: **7–15 seconds**.

---

## 3. Shared REST JSON response

Most REST methods return:

```json
{
  "Value": "1234567890123456789",
  "RetStatus": 1,
  "StrRetStatus": "Ok"
}
```

| Field | Meaning |
|-------|---------|
| `RetStatus` | Status code. **`1` = success** for REST |
| `StrRetStatus` | Human-readable status (`Ok` on success) |
| `Value` | On success: **recId** (long numeric message id). On credit: remaining credit. On failure: extra detail |

`SendMultipleSMS` may return `ReqStatus` instead of (or in addition to) `RetStatus`. Treat **`ReqStatus == 1` or `RetStatus == 1`** as success.

`GetUserNumbers` also returns a `Data` array of sender numbers. Some responses wrap status in `MyBase`:

```json
{
  "MyBase": {
    "Value": "...",
    "RetStatus": 1,
    "StrRetStatus": "Ok"
  },
  "Data": [
    { "Number": "5000xxxxxxx" }
  ]
}
```

Parse either shape. Success if `RetStatus == 1` (top-level or `MyBase`).

### Success rules (use these in code)

| API | Success |
|-----|---------|
| REST send / credit / numbers / schedule | HTTP 200 **and** `RetStatus == 1` |
| REST multiple | HTTP 200 **and** (`ReqStatus == 1` or `RetStatus == 1`) |
| Pattern SOAP `SendByBaseNumber2` | XML `<string>` inner text parses as a number **greater than 1000** (usually a 15+ digit recId) |
| Pattern REST `BaseServiceNumber` | `RetStatus == 1` **or** `Value` is a recId `> 1000` |

Do **not** treat HTTP 200 alone as success. The panel often returns 200 with `RetStatus` set to an error code.

Save `Value` (recId) if you need delivery reports later.

---

## 4. Minimal new-system integration

For a typical app (OTP + optional notifications), implement **two** methods:

1. **Pattern send** — login codes, password reset, order status (recommended default)
2. **Plain send** — marketing / custom text from your own line

Optional later: credit, sender numbers, multiple, schedule, delivery.

Suggested interface:

```csharp
Task<SmsResult> SendAsync(string to, string from, string text);
Task<SmsResult> SendPatternAsync(string to, string bodyId, string[] patternValues);

class SmsResult
{
    public bool Ok { get; set; }
    public string RecId { get; set; }      // Value on success
    public int Status { get; set; }        // RetStatus
    public string Message { get; set; }    // StrRetStatus or error text
}
```

Phone numbers: Iranian mobiles as `09xxxxxxxxx` (11 digits). Trim whitespace. One recipient per pattern call.

---

## 5. Send plain SMS

**POST** `https://rest.payamak-panel.com/api/SendSMS/SendSMS`

```json
{
  "username": "YOUR_USERNAME",
  "password": "YOUR_PASSWORD",
  "to": "09121234567",
  "from": "5000xxxxxxx",
  "text": "Your verification code: 12345",
  "isFlash": false
}
```

| Field | Required | Notes |
|-------|----------|--------|
| `username` | yes | Official name is lowercase `username`. `userName` also works on this endpoint |
| `password` | yes | |
| `to` | yes | One number, or a comma-separated list depending on panel |
| `from` | yes | Your dedicated line. Can be `null` only if the panel has a default line |
| `text` | yes | Escape JSON special characters (`\`, `"`, newlines) |
| `isFlash` | no | Default `false`. Flash SMS appears on screen without saving |

Escape `text` for JSON: `\`, `"`, `\n`, `\r`.

### curl

```bash
curl -X POST "https://rest.payamak-panel.com/api/SendSMS/SendSMS" \
  -H "Content-Type: application/json" \
  -d "{\"username\":\"USER\",\"password\":\"PASS\",\"to\":\"09121234567\",\"from\":\"5000xxxxxxx\",\"text\":\"hello\",\"isFlash\":false}"
```

### C# (.NET)

```csharp
using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };

var payload = new
{
    username,
    password,
    to,
    from,
    text,
    isFlash = false
};

var json = System.Text.Json.JsonSerializer.Serialize(payload);
var content = new StringContent(json, Encoding.UTF8, "application/json");
var http = await client.PostAsync("https://rest.payamak-panel.com/api/SendSMS/SendSMS", content);
var body = await http.Content.ReadAsStringAsync();
// deserialize { Value, RetStatus, StrRetStatus }
// success = RetStatus == 1
```

Form-urlencoded alternative (official SDK style):

```
username=USER&password=PASS&to=09121234567&from=5000xxxxxxx&text=hello&isFlash=false
```

`Content-Type: application/x-www-form-urlencoded`

---

## 6. Send pattern / OTP (use this for transactional SMS)

A pattern is an approved template in the panel, for example:

```
کد تایید شما: {0}
```

You do **not** send the full sentence. You send **only the variable values**, in order, separated by `;`.

Example: pattern `{0} عزیز، سفارش {1} ثبت شد`

```
text = "sina;12345;"
bodyId = "123456"   // the numeric pattern id from the panel
to = "09121234567"
```

Always end the value list with `;` if the panel expects it (this project uses that format: `sina;shiri;20;`).

One mobile number per call.

### 6.1 REST (recommended for new systems)

**POST** `https://rest.payamak-panel.com/api/SendSMS/BaseServiceNumber`

JSON:

```json
{
  "username": "YOUR_USERNAME",
  "password": "YOUR_PASSWORD",
  "to": "09121234567",
  "text": "12345;",
  "bodyId": "123456"
}
```

Form:

```
username=USER&password=PASS&to=09121234567&text=12345;&bodyId=123456
```

Success: `RetStatus == 1`, or `Value` is a recId `> 1000`.

### 6.2 SOAP HTTP POST (used in this repository, proven)

**POST** `https://api.payamak-panel.com/post/Send.asmx/SendByBaseNumber2`

Headers:

```
Content-Type: application/x-www-form-urlencoded
cache-control: no-cache
```

Body (not JSON):

```
username=USER&password=PASS&to=09121234567&text=12345;&bodyId=123456
```

URL-encode `text` if it contains `&`, `=`, or non-ASCII.

Response is **XML**, not JSON:

```xml
<?xml version="1.0" encoding="utf-8"?>
<string xmlns="http://tempuri.org/">1234567890123456789</string>
```

Parse the first `<string>` element.

```csharp
var xml = new XmlDocument();
xml.LoadXml(responseString);
var numberString = xml.GetElementsByTagName("string")[0].InnerText;

if (long.TryParse(numberString, out long recId) && recId > 1000)
{
    // success — recId is the message id
}
else
{
    // failure — numberString is an error code (0, 2, -4, ...)
}
```

If the value is **not** a large recId, it is an error code (see section 9).

---

## 7. Send multiple (paired messages)

Send **different texts** to **different numbers** in one call. Index `i` in `to` matches index `i` in `text`.

**POST** `https://rest.payamak-panel.com/api/SendSMS/SendMultipleSMS`

**Hard limit: 100 numbers and 100 texts per request.** Drop empty entries, then `Take(100)`.

```json
{
  "username": "YOUR_USERNAME",
  "password": "YOUR_PASSWORD",
  "from": "5000xxxxxxx",
  "to": ["09121234567", "09129876543"],
  "text": ["hello A", "hello B"]
}
```

Success: `ReqStatus == 1` (this is the field this repository checks). Also accept `RetStatus == 1`.

---

## 8. Send scheduled SMS

**POST** `https://rest.payamak-panel.com/api/SendSMS/SendSchedule`

```json
{
  "username": "YOUR_USERNAME",
  "password": "YOUR_PASSWORD",
  "to": "09121234567",
  "from": "5000xxxxxxx",
  "text": "reminder text",
  "scheduleDate": "2026-08-20 14:30:00",
  "period": 0
}
```

| Field | Format |
|-------|--------|
| `scheduleDate` | `yyyy-MM-dd HH:mm:ss` invariant culture, e.g. `2026-08-20 14:30:00` |
| `period` | Repeat. SOAP enum: `Once`, `Daily`, `Weekly`, `Monthly`, `Yearly`, `Custom`. This REST call sends an **int**; **`0` = once (no repeat)** |

Success: `RetStatus == 1`. `Value` is the schedule id.

Allowed send window on many Iranian panels is roughly **07:00–22:00**.

SOAP equivalent if REST is unavailable:

**POST** `https://api.payamak-panel.com/post/Schedule.asmx/AddSchedule`

```
username=...&password=...&to=...&from=...&text=...&isflash=false&scheduleDateTime=2026-08-20 14:30:00&period=Once
```

---

## 9. Get credit

**POST** `https://rest.payamak-panel.com/api/SendSMS/GetCredit`

```json
{
  "username": "YOUR_USERNAME",
  "password": "YOUR_PASSWORD"
}
```

Success: `RetStatus == 1`. Remaining credit is in `Value`.

---

## 10. Get sender numbers

**POST** `https://rest.payamak-panel.com/api/SendSMS/GetUserNumbers`

Same body as GetCredit.

On success, `Data` is a list of `{ "Number": "5000..." }`. Use one of these as `from` for plain SMS.

---

## 11. Delivery report (optional)

**POST** `https://rest.payamak-panel.com/api/SendSMS/GetDeliveries2`

```json
{
  "username": "YOUR_USERNAME",
  "password": "YOUR_PASSWORD",
  "recId": "1234567890123456789"
}
```

`recId` is the `Value` returned by a successful send.

---

## 12. Status and error codes

### REST `RetStatus` (plain send and most REST methods)

| Code | Meaning |
|------|---------|
| `1` | Success |
| `0` | Bad username/password, or could not connect |
| `2` | Insufficient credit |
| `3` | Daily send limit |
| `4` | Volume / count limit |
| `5` | Invalid sender (`from`) line |
| `6` | Panel is updating |
| `7` | Text contains filtered words |
| `8` | Below minimum send count |
| `9` | Cannot send from public lines via API |
| `10` | Account inactive or blocked |
| `11` | Not sent (often recipient on operator blacklist) |
| `12` | KYC / documents incomplete |
| `14` | This line cannot send links |
| `15` | Multi-recipient ads must end with `لغو11` |
| `16` | Recipient (`to`) missing |
| `17` | Empty text |
| `18` | Invalid recipient number |
| `19` | Hourly limit exceeded (pattern) |
| `35` | REST: recipient is on the operator blacklist |
| `-1` | Unknown error |
| `-108` | IP blocked after failed API attempts |
| `-109` | Allowed-IP must be set in the panel |
| `-110` | Must use API key instead of password |
| `-111` | Requester IP is not allowed |

### Pattern SOAP / `SendByBaseNumber2` (`<string>` value, not JSON)

If the value is a **large recId** (typically 15+ digits, treat **`> 1000` as success**), send succeeded.

Otherwise:

| Code | Meaning |
|------|---------|
| `0` | Bad credentials / no connection |
| `-1` | Pattern webservice access disabled |
| `-2` | Only **one** recipient per call |
| `-3` | Sender line not defined, or invalid recipient count |
| `-4` | Wrong `bodyId`, or pattern not approved yet |
| `-5` | Number of `text` values does not match pattern placeholders |
| `-6` | Internal / bad `{placeholder}` syntax |
| `-7` | Sender-number error |
| `-10` | Do not put URL, IP, or email in a pattern variable |
| plus the positive REST codes above (`2`, `7`, `10`, …) |

---

## 13. Implementation checklist (new system)

1. Put `username`, `password`, default `from`, and OTP `bodyId` in configuration.
2. Implement HTTP POST with JSON (or form) and a 10s timeout.
3. Implement **pattern send** first; verify with a real mobile.
4. Implement **plain send** if you need free-text messages.
5. Treat success only when `RetStatus == 1` (REST) or recId `> 1000` (SOAP pattern).
6. Log `RetStatus`, `StrRetStatus`, `Value`, and HTTP status. Do not log passwords.
7. Map common errors (`2` credit, `5` bad line, `7` filtered, `11`/`35` blacklist) to user-facing messages.
8. For OTP, generate the code in **your** app; pass it as the pattern variable. Do not send the full template text.
9. Use a real JSON library (`System.Text.Json` / Newtonsoft). Do not split JSON on commas.
10. If the panel enables IP whitelist, add the server public IP before going live.

---

## 14. Copy-paste C# client (minimal)

```csharp
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Xml;

public sealed class FaraPayamakClient
{
    private static readonly HttpClient Http = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(10)
    };

    private readonly string _username;
    private readonly string _password;

    public FaraPayamakClient(string username, string password)
    {
        _username = username;
        _password = password;
    }

    public async Task<(bool ok, string recId, int status, string message)> SendSmsAsync(
        string to, string from, string text)
    {
        var url = "https://rest.payamak-panel.com/api/SendSMS/SendSMS";
        var payload = new
        {
            username = _username,
            password = _password,
            to,
            from,
            text,
            isFlash = false
        };

        var json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await Http.PostAsync(url, content);
        var body = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(body);
        var root = doc.RootElement;
        var status = root.TryGetProperty("RetStatus", out var rs) ? rs.GetInt32() : 0;
        var value = root.TryGetProperty("Value", out var v) ? v.ToString() : "";
        var msg = root.TryGetProperty("StrRetStatus", out var m) ? m.GetString() : body;

        return (status == 1, value, status, msg ?? "");
    }

    public async Task<(bool ok, string recId, string message)> SendPatternAsync(
        string to, string bodyId, params string[] values)
    {
        var url = "https://api.payamak-panel.com/post/Send.asmx/SendByBaseNumber2";
        var text = string.Join(";", values) + ";";
        var form = $"username={Uri.EscapeDataString(_username)}" +
                   $"&password={Uri.EscapeDataString(_password)}" +
                   $"&to={Uri.EscapeDataString(to)}" +
                   $"&text={Uri.EscapeDataString(text)}" +
                   $"&bodyId={Uri.EscapeDataString(bodyId)}";

        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(form, Encoding.UTF8, "application/x-www-form-urlencoded")
        };
        request.Headers.TryAddWithoutValidation("cache-control", "no-cache");

        using var response = await Http.SendAsync(request);
        var xml = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(xml))
            return (false, "", "empty response");

        var doc = new XmlDocument();
        doc.LoadXml(xml);
        var numberString = doc.GetElementsByTagName("string")[0].InnerText;

        if (long.TryParse(numberString, out var recId) && recId > 1000)
            return (true, numberString, "Ok");

        return (false, "", numberString);
    }
}
```

REST pattern alternative (JSON, no XML):

```csharp
public async Task<(bool ok, string recId, int status, string message)> SendPatternRestAsync(
    string to, string bodyId, params string[] values)
{
    var url = "https://rest.payamak-panel.com/api/SendSMS/BaseServiceNumber";
    var payload = new
    {
        username = _username,
        password = _password,
        to,
        text = string.Join(";", values) + ";",
        bodyId
    };
    var json = JsonSerializer.Serialize(payload);
    using var content = new StringContent(json, Encoding.UTF8, "application/json");
    using var response = await Http.PostAsync(url, content);
    var body = await response.Content.ReadAsStringAsync();

    using var doc = JsonDocument.Parse(body);
    var root = doc.RootElement;
    var status = root.TryGetProperty("RetStatus", out var rs) ? rs.GetInt32() : 0;
    var value = root.TryGetProperty("Value", out var v) ? v.ToString() : "";
    var msg = root.TryGetProperty("StrRetStatus", out var m) ? m.GetString() : body;

    var recIdOk = long.TryParse(value, out var recId) && recId > 1000;
    return (status == 1 || recIdOk, value, status, msg ?? "");
}
```

---

## 15. Pitfalls

- HTTP 200 is not enough; always read `RetStatus` / recId.
- Pattern `text` is **variables only**, joined by `;`, not the template body.
- Pattern: **one** recipient per request.
- Multiple send: max **100** pairs; `to[i]` matches `text[i]`.
- JSON-serialize `text` properly; Persian and quotes will break a hand-rolled serializer.
- `SendSMS` accepts `username` or `userName`; other methods use `username`.
- Blacklist: use **pattern / service line**, not a promotional `from` line.
- Some accounts require API key (`-110`) or a fixed server IP (`-109`, `-111`).
- Do not put URLs inside pattern variables (`-10`).

---

## 16. Official extras (not required for basic sending)

SOAP catalog: [https://api.payamak-panel.com/](https://api.payamak-panel.com/)

| Service | URL |
|---------|-----|
| Send | `https://api.payamak-panel.com/post/send.asmx` |
| Schedule | `https://api.payamak-panel.com/post/Schedule.asmx` |
| Actions (bulk / multiple) | `https://api.payamak-panel.com/post/actions.asmx` |
| Receive | `https://api.payamak-panel.com/post/receive.asmx` |

Official REST helpers also include `GetBasePrice` and `GetMessages` on the same REST base.

Vendor SDKs: [https://github.com/Farapayamak](https://github.com/Farapayamak)
