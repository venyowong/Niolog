# Niolog
Easy logger

```
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    ...

    NiologManager.DefaultWriters = new ILogWriter[]
    {
        new FileLogWriter(path, 10),
        new HttpLogWriter(url, 10, 1)
    };
    
    loggerFactory.AddProvider(new LoggerProvider());
    
    ...
}
```
