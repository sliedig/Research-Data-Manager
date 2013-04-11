::This batch should be run to package a deployment, it assumes a release build has been built beforehand
@ECHO OFF
SET DEPLOY=C:\Urdms\_Deploy
IF EXIST %DEPLOY% RMDIR /S /Q %DEPLOY%
MD %DEPLOY%
XCOPY ProvisioningService\bin\Release %DEPLOY%\ProvisioningService /I /Q /S
XCOPY ProvisioningService.SharePoint\bin\Release %DEPLOY%\ProvisioningService.SharePoint /I /Q /S
XCOPY ProvisioningService.ViewModelUpdater\bin\Release %DEPLOY%\ProvisioningService.ViewModelUpdater /I /Q /S
XCOPY Approvals.ApprovalService\bin\Release %DEPLOY%\Approvals.ApprovalService /I /Q /S
XCOPY Approvals.ViewModelUpdater\bin\Release %DEPLOY%\Approvals.ViewModelUpdater /I /Q /S
XCOPY Approvals.VivoPublisher\bin\Release %DEPLOY%\Approvals.VivoPublisher /I /Q /S
XCOPY NotificationService\bin\Release %DEPLOY%\NotificationService /I /Q /S
XCOPY DocumentBuilderService\bin\Release %DEPLOY%\DocumentBuilderService /I /Q /S
XCOPY Deploy.bat %DEPLOY% /I /Q
