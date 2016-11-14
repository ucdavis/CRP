using System.Web;
using FluentNHibernate.Utils;
using Serilog;
using Serilog.Exceptions.Destructurers;
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
                .WriteTo.Stackify()
                .Filter.ByExcluding(a => a.Exception != null && (a.Exception.GetBaseException() is HttpUnhandledException || a.Exception.GetBaseException() is HttpException))
                .Enrich.With<HttpSessionIdEnricher>()
                .Enrich.With<UserNameEnricher>()
                .Enrich.With<ExceptionEnricher>()
                .Enrich.FromLogContext()
                .CreateLogger();

            _loggingSetup = true;
        }
    }
}