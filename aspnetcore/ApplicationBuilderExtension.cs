using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Niolog.AspNetCore.Middlewares;
using Niolog.Interfaces;

namespace Niolog.AspNetCore
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder UseNiolog(this IApplicationBuilder builder, Action<Options> config = null)
        {
            builder.UseMiddleware<NiologMiddleware>();

            Options options = new Options();
            if (config != null)
            {
                config(options);
            }
            List<ILogWriter> logWriters = new List<ILogWriter>();
            if (options.UseConsole)
            {
                logWriters.Add(new ConsoleLogWriter());
            }
            if (!string.IsNullOrWhiteSpace(options.FolderPath))
            {
                logWriters.Add(new FileLogWriter(options.FolderPath, options.FileBatch));
            }
            if (!string.IsNullOrWhiteSpace(options.HttpUrl))
            {
                logWriters.Add(new HttpLogWriter(options.HttpUrl, options.HttpBatch, options.HttpConcurrent));
            }
            NiologManager.DefaultWriters = logWriters.ToArray();
            var loggerFactory = builder.ApplicationServices.GetRequiredService<ILoggerFactory>();
            loggerFactory?.AddProvider(new LoggerProvider());

            return builder;
        }
    }
}
