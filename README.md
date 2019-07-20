# Niolog
Niolog is a easy logger for light-weight application.

## Install

`dotnet add package Niolog --version 0.0.4`

## In Console
```
NiologManager.DefaultWriters = new ILogWriter[]
{
    new FileLogWriter(path, 10)
};

var logger = NiologManager.CreateLogger();
logger = NiologManager.CreateLogger(new FileLogWriter(path, 10));

logger.Info()
    .Message("test")
    .Write();
```

## In Asp.Net Core

```
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    ...

    NiologManager.DefaultWriters = new ILogWriter[]
    {
        new FileLogWriter(path, 10),
        new HttpLogWriter("http://localhost:9615/{project}/store", 10, 1)
    };
    
    loggerFactory.AddProvider(new LoggerProvider());
    
    ...
}
```

## Niolog.Web

The web page for Niolog is used to search log produced from applications.

You can use HttpLogWriter to store logs into Niolog.Web.

```
cd web
dotnet run
```

[Online Example](https://venyo.cn/niolog/)

Write `hotoke` in project input, and click search button, you will get some logs.