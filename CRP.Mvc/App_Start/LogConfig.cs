using System;
using System.Configuration;
using System.Web;
using FluentNHibernate.Utils;
using Serilog;
using Serilog.Exceptions.Destructurers;
using Serilog.Sinks.Elasticsearch;
using SerilogWeb.Classic.Enrichers;

namespace CRP.Mvc
{
    public static class LogConfig
    {
        private static bool _loggingSetup;

        /// <summary>
        /// Configure Application Logging
        /// </summary>
        public static void ConfigureLogging()
        {
            if (_loggingSetup) return; //only setup logging once

            Log.Logger = new LoggerConfiguration()
                .WriteToElasticSearchCustom()
                //.WriteTo.Stackify()
                .Filter.ByExcluding(a => a.Exception != null && a.Exception.GetBaseException() is HttpException)
                .Enrich.With<HttpSessionIdEnricher>()
                .Enrich.With<UserNameEnricher>()
                .Enrich.With<ExceptionEnricher>()
                .Enrich.FromLogContext()
                .Enrich.WithClientIp()
                .Enrich.WithClientAgent()
                .CreateLogger();

            _loggingSetup = true;
        }

        public static LoggerConfiguration WriteToElasticSearchCustom(this LoggerConfiguration logConfig)
        {
            var esUrl = ConfigurationManager.AppSettings["Stackify.ElasticUrl"];

            // only continue if a valid http url is setup in the config
            if (esUrl == null || !esUrl.StartsWith("http"))
            {
                return logConfig;
            }

            logConfig.Enrich.WithProperty("Application", ConfigurationManager.AppSettings["Stackify.AppName"]);
            logConfig.Enrich.WithProperty("AppEnvironment", ConfigurationManager.AppSettings["Stackify.Environment"]);

            return logConfig.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(esUrl))
            {
                IndexFormat = "aspnet-registration-{0:yyyy.MM}"
            });
        }
    }
}