# fusionInSite-push-server

Push notification server for Catalent

Schedule Configuration
----------------------
&lt;AppSettings&gt; &lt;add key="CronSchedule" value="" /&gt; &lt;/AppSettings&gt;   
The cron schedule when the application will check for new push notifications to send
 
Cron-Expressions are used to configure instances of CronTrigger. Cron-Expressions are strings that are actually made up of seven sub-expressions, that describe individual details of the schedule. These sub-expression are separated with white-space, and represent:

Seconds  
Minutes  
Hours  
Day-of-Month  
Month  
Day-of-Week  
Year (optional field)

- * = any value
- ? = any value used in Day-of-Week field

An example of a complete cron-expression is the string “0 0 12 * * WED” - which means “every Wednesday at 12:00 pm”.

Another example is the string “* 0/15 * * * ?” - which means “every 15 minutes”.


Logging Configuration
----------------------
Logging done by Log4net and is configred by editing the fusionInsite.App.Console.exe.config file  
See: https://logging.apache.org/log4net/release/config-examples.html

OneSignal Configuration
-----------------------
&lt;AppSettings&gt;  
&lt;add key="OneSignalAppId" value="" /&gt;   
&lt;add key="OneSignalApiKey" value="" /&gt;  
&lt;/AppSettings&gt;    

Installation
------------
Build the solution in Release mode in Visual Studio. This will produce a fusionInsite.App.Console/bin/Release folder.
This folder can then be deployed by copying this folder onto the target machine, opening an Administrative command prompt and typing

fusionInsite.App.Console install  
and then  
fusionInsite.App.Console start

The service will be visible in the Windows "Services" Management Console, and will start automatically whenever the system restarts.

Push Notifications
------------------

Push notifications are sent through OneSignal

An example payload is as follows:

{  
                app_id = '00000000-0000-0000-0000-000000000000',  
                contents = new { en = 'Notification text content'}
                include_player_ids = '00000000-0000-0000-0000-000000000000',  
                data = {  
                  notificationId = 123456
                },  
}



