using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;

namespace AsicServer.Infrastructure
{
    public static class Logger
    {
        internal static readonly string LOG_CONFIG_FILE = @"log4net.config";

        internal static readonly log4net.ILog _log = GetLogger(typeof(Logger));

        public static ILog GetLogger(Type type)
        {
            return LogManager.GetLogger(type);
        }

        public static void Debug(object message)
        {
            //SetLog4NetConfiguration();
            _log.Debug(message);
        }

        public static void Error(object message)
        {
            //SetLog4NetConfiguration();
            _log.Error(message);
        }

        public static void Info(object message)
        {
            //SetLog4NetConfiguration();
            _log.Info(message);
        }
        private static void SetLog4NetConfiguration()
        {
            if(_log != null)
            {
                return;
            }
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead(LOG_CONFIG_FILE));

            var repo = LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));

            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
        }


    }
}
