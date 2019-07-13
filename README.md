# Niolog
Easy logger

```
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    ...

    NiologManager.DefaultWriters = new ILogWriter[]
    {
        new FileLogWriter(appSettings.Value.Niolog.Path, 10),
        new HttpLogWriter(appSettings.Value.Niolog.Url, 10, 1)
    };
    
    loggerFactory.AddProvider(new LoggerProvider());
    
    ...
}
```
