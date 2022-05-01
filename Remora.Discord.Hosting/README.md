Remora.Discord.Hosting
======================

This package provides an implementation of a hosted service that runs the
gateway client in the background.

## Structure
The hosted service is quite simple, and merely runs the gateway client. If 
configured, critical errors in the gateway (which should only happen in the case
of a bug in Remora.Discord or your system configuration) may also gracefully 
terminate the application.

## Usage
Usage is relatively standard for applications that use a .NET Generic Host 
architecture.

```c#
private static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
    .AddDiscordService
    (
        services =>
        {
            // ...
        }
    )
```

Optionally, you may configure some behaviour by way of `DiscordServiceOptions`,
where additional options are exposed.
