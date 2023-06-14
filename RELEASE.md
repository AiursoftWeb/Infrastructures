# Release Notes

## Version 6.0.8

- Changed `AccessToken` to `AccessTokenAsync`.

## Version 6.0.9

- Merged project Archon into Gateway.

## Version 6.0.10

- Hard coded Aiursoft App ID in `appsettings`.

## Version 6.0.11

- Changed `Probe` to lazy load.

## Version 6.0.12

- Probe's SDK allows public access to server configuration information.
- Gateway's SDK lazy loads server information.

## Version 6.0.13

- Probe's SDK allows `Locator` to be serialized and deserialized.

## Version 6.0.14

- Renamed `Gateway` to `Directory`.
- Renamed `EventService` to `ObserverService`.
- Implemented a new configuration-based registration method for `Observer`, `Gateway`, and `Probe`.
- Used `CORS` for all pages to allow API calls from other pages.
- New registration syntax:

```csharp
services.AddAiursoftAuthentication(Configuration.GetSection("AiursoftAuthentication"));
services.AddAiursoftObserver(Configuration.GetSection("AiursoftObserver"));
services.AddAiursoftProbe(Configuration.GetSection("AiursoftProbe"));
```

## Version 6.0.15

- Renamed all `Gateway` instances to `Directory`.
- Merged all `Directory` migrations into one.

## Version 6.0.16

- Refactored registration method for `Stargate`.
- Renamed `AddAiursoftAuthentication` to `AddAiursoftAppAuthentication`.

## Version 6.0.17

- Refactored registration method for `Warpgate`.

## Version 6.0.18

- Stargate's SDK provides a new API for getting the websocket listening address.

## Version 6.0.19

- Allowed applications to disable default `CORS`.

## Version 6.0.20

- Fixed an issue that exception handler middleware not using correct scope.
- Add new handler: UseAiuroftHandler
- Rename middleware: UseAiursoftAppRouters\UseAiursoftAPIAppRouters
- Rename to APIExpHandler APIRemoteExceptionHandler

## Version 6.0.21

- Remove Developer related code.
