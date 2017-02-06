using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;

namespace fusionInsite.App.Console
{
    public static class Log4NetConfiguration
    {
        //static readonly string name = "Lasamba";
        static readonly Level Threshold = Level.Info;

        public static void Configure()
        {
            var layout = new PatternLayout
            {
                // see http://logging.apache.org/log4net/release/sdk/log4net.Layout.PatternLayout.html
                ConversionPattern = "%date{dd-MMM-yyyy HH:mm:ss}  %-5level %logger  %message%n%exception%n"
            };
            
            var fileAppender = new RollingFileAppender
            {
                File = Path.Combine(Environment.CurrentDirectory, "logs", "log.txt"),
                Layout = layout,
                AppendToFile = true,
                Threshold = Threshold
            };

            /*
            var eventLogAppender = new EventLogAppender
                {
                    ApplicationName = name,
                    Layout = layout,
                    Threshold = threshold
                };
            */

            var consoleAppender = new ConsoleAppender
            {
                Layout = layout,
                Threshold = Threshold
            };

            layout.ActivateOptions();
            
            fileAppender.ActivateOptions();
            BasicConfigurator.Configure(fileAppender);
            
            consoleAppender.ActivateOptions();
            BasicConfigurator.Configure(consoleAppender);
        }
    }
}
