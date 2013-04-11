::This batch file will uninstall a service. It is called by Deploy.bat, you should not call it directly.
@ECHO OFF
ECHO Uninstalling Urdms.%1
CD %1
NServiceBus.Host.exe /uninstall /serviceName:Urdms.%1
CD ..