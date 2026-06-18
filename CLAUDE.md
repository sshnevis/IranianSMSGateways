# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build Commands

```bash
dotnet build
dotnet build --configuration Release
```

There are no tests and no lint step. The project has no external NuGet dependencies — all JSON handling is done with hand-rolled serializers in `Convertor/JsonConvert.cs`.

## Architecture

This is a .NET Standard 2.0 **class library** (not an executable) that provides a unified API for Iranian SMS gateways.

### Request flow

```
Consumer code
    └─► Sender  (Sender.cs — public facade, the only class consumers instantiate)
            └─► ISendSms  (Services/ISendSms.cs — interface all providers implement)
                    ├─► SendSmsFaraPayamk   (Services/IRepositories/)
                    ├─► SendSmsIpPanel      (Services/IRepositories/)
                    └─► SendSmsAllSmsSender (Services/IRepositories/)
```

`Sender` dispatches on the `ProvidesType` enum field present in every DTO using a chain of `if/else if` blocks — there is no DI or factory pattern. Each provider is instantiated directly inside each branch.

### DTOs (`DTOs/`)

Every public method on `Sender` takes a dedicated DTO (e.g. `SendSmsDTO`, `SendScheduleDTO`). All DTOs include a `ProvidesType` field that determines which provider implementation is used. The shared response type `ResponseSMS` wraps `IsSuccess`, `ResCode`, `Error`, and a `Result` sub-object whose fields (`Data`, `Code`, `Message`, `ResultData`) are populated differently per provider.

### Provider implementations

- **FaraPayamak** — most complete; implements all six interface methods using REST/JSON and one XML-based SOAP endpoint for `SendSmsByPatternAsync`.
- **IpPanel** — only `SendSmsAsync` and `SendSmsByPatternAsync` are implemented; all other methods throw `NotImplementedException`.
- **AllSmsSend** — only `SendSmsAsync` and `SendSmsByPatternAsync` have any implementation (partial); all others throw `NotImplementedException`.

Endpoint URLs are stored as `static readonly` fields on `Models/Providers.cs` (e.g. `Providers.Farapayamak`, `Providers.IpPanel`).

### Key internals

- **`Convertor/JsonConvert.cs`** — two static classes: `JsonConvert` (simple reflection-based serializer, splits on `,` which breaks on values containing commas) and `JsonConvert2` (a recursive descent parser used for complex/nested responses like `GetUserNumbers`). Do not use `JsonConvert` for responses with nested JSON or array values in strings — use `JsonConvert2`.
- **`Convertor/BadWords.cs`** — `BadWords.CheckJomle()` censors Persian profanity to `"**"` using regex. It is called on outgoing SMS text in FaraPayamak and IpPanel before the HTTP request.
- **`Convertor/LogFile.cs`** — `internal` logging helper that appends to a file. Currently commented out in production code; useful for debugging by uncommenting the `Logging.LogFile(...)` call in provider methods.

### Adding a new provider

1. Add a value to the `ProvidesType` enum in `Models/Providers.cs`.
2. Add a `static readonly Providers` field with endpoint URLs in `Models/Providers.cs`.
3. Create a new class in `Services/IRepositories/` implementing `ISendSms`.
4. Add `else if` branches for the new provider in each method of `Sender.cs`.
