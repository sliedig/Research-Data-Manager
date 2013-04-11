::Run this file to install all of the Urdms services
@ECHO OFF
IF (%1)==() GOTO PARAMSREQ
IF (%2)==() GOTO PARAMSREQ
CALL ProvisioningService\InstallService.bat ProvisioningService %1 %2
CALL ProvisioningService.SharePoint\InstallService.bat ProvisioningService.SharePoint %1 %2
CALL ProvisioningService.ViewModelUpdater\InstallService.bat ProvisioningService.ViewModelUpdater %1 %2
CALL Approvals.ApprovalService\InstallService.bat Approvals.ApprovalService %1 %2
CALL Approvals.ViewModelUpdater\InstallService.bat Approvals.ViewModelUpdater %1 %2
CALL Approvals.VivoPublisher\InstallService.bat Approvals.VivoPublisher %1 %2
CALL NotificationService\InstallService.bat NotificationService %1 %2
CALL DocumentBuilderService\InstallService.bat DocumentBuilderService %1 %2
GOTO END
:PARAMSREQ
ECHO UserName and Password are required. Usage: Deploy [UserName] [Password]
:END