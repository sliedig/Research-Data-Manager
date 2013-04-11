::This batch file will install a service. It is called by Deploy.bat, you should not call it directly.
@ECHO OFF
ECHO Installing Urdms.%1
CD %1
NServiceBus.Host.exe /uninstall
NServiceBus.Host.exe /install /username:%2 /password:%3  NServiceBus.Production 
net start Urdms.%1
CD ..