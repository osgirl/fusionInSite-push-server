# fusionInSite-push-server

Push notification server for Catalent

Configuration settings
----------------------
CronSchedule - the cron schedule when the application will check for new push notifications to send
Cron-Expressions are used to configure instances of CronTrigger. Cron-Expressions are strings that are actually made up of seven sub-expressions, that describe individual details of the schedule. These sub-expression are separated with white-space, and represent:

Seconds
Minutes
Hours
Day-of-Month
Month
Day-of-Week
Year (optional field)
An example of a complete cron-expression is the string “0 0 12 ? * WED” - which means “every Wednesday at 12:00 pm”.
- * = any value
- ? = any value used in Day-of-Month or Day-of-Week fields
An other example is the string “* 0/15 * ? * ?” - which means “every 15 minutes”.

Installation
------------
Build the solution in Release mode in Visual Studio. This will produce a fusionInsite.App.Console/bin/Release folder.
This folder can then be deployed by copying this folder onto the target machine, opening an Administrative command prompt and typing

fusionInsite.App.Console install
and then
fusionInsite.App.Console start.

The service will be visible in the Windows "Services" Management Console, and will start automatically whenever the system restarts.


