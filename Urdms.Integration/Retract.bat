::Run this file to uninstall all of the Urdms services
@ECHO OFF
SET DEPLOY=C:\Urdms\_Deploy
IF NOT EXIST %DEPLOY% GOTO END
CD %DEPLOY%
CALL ProvisioningService\UninstallService.bat ProvisioningService
CALL ProvisioningService.SharePoint\UninstallService.bat ProvisioningService.SharePoint
CALL ProvisioningService.ViewModelUpdater\UninstallService.bat ProvisioningService.ViewModelUpdater
CALL Approvals.ApprovalService\UninstallService.bat Approvals.ApprovalService
CALL Approvals.ViewModelUpdater\UninstallService.bat Approvals.ViewModelUpdater
CALL Approvals.VivoPublisher.\UninstallService.bat Approvals.VivoPublisher
CALL NotificationService\UninstallService.bat NotificationService
CALL DocumentBuilderService\UninstallService.bat DocumentBuilderService
CD ..
:END