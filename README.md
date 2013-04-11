# Research Data Manager

The Research Data Manager is broken up into two solutions, an ASP MVC3 web 
application to allow the creation of DMPs (Data Management Plans) and DDs 
(Data Deposits) along with an integration solution which is a collection of 
NServiceBus services for coordinating site-provisioning, data collection approvals, 
document creation (for snapshots of the DMP) and publishing data to VIVO integrating 
with the open source Java Application (http://sourceforge.net/projects/vivo/).

This project is supported by the Australian National Data Service
(ANDS). ANDS is supported by the Australian Government through the
National Collaborative Research Infrastructure Strategy Program and the
Education Investment Fund (EIF) Super Science Initiative.

## URDMS DMP Application
URDMS stands for University Research Data Management System.
It is the program title for all ANDS projects at Curtin University.

### Overview

The DMP (Data Management Plan) is an ASP MVC3 web application is the starting point for 
researchers to complete/update their data management plans, data deposit forms, data 
collections and some project information. The web application kicks off SharePoint site 
provisioning by sending an NServiceBus message to the provisioning service on completion 
of a DMP/DD. Data Collection approvals are also completed via the web application, although 
the workflow is managed by the approvals service.

### Installation

- Create “Urdms” and “UrdmsTests” MS SQL databases.
- Open the Urdms.Dmp.sln solution file via Visual Studio.
- Change configuration items as required (See Web.Config setting section below).
- Setup URDMS application to run via the local IIS sever (see IIS Setup section below).
- Rebuild the application and run it to allow migrator.NET to create the tables.
- The Urdms Web application relies on the NService Bus queues being in place for the application to function. Please refer to the URDMS Integration documentation.

### Implementation
- Implement ICurtinUserService interface (currently a mock implementation is used – DummyUserLookupService in the DummyService.cs file). 
- Implement the IMemberService interface (currently a mock implementation is used – DummyMemberService in the DummyService.cs file). This service adds roles to the user.
- Implement the IDirectoryEntryService interface (currently a mock implementation is used – DummyDirectoryEntryService in the DummyService.cs file). This class is used for extracting user roles via directory service such as Active Directory. 
- Add user roles for controlling top navigation menu accessibility (needs to be one of the roles added to the user via the Member or DirectoryEntry service.

### Test run
The DummyService.cs file has mock implementations for the ICurtinUserService, IMemberService 
and DirectoryEntryService. In addition it also has sample IDs for a researcher and approvers 
that can be used for testing the applications functionality. To login to the site use any of 
these IDs as the Login ID and any string in the password.

### Web.config settings
Some of the web configuration items noted below will require transforms to be defined in the CI, 
Prod and Test environments.  
-	NServiceBus endpoints for Urdms.ProvisioningService.Commands, Urdms.Approvals.ApprovalService.Commands
and Urdms.DocumentBuilderService.Commands message type end point queue must be defined here (the web 
application endpoint is defined in the AutoFac NServiceBusModule (under Config/Autofac).
-	If directory (i.e. Active Directory) based roles are to be used for the application the set values 
for LdapURI, LdapUser and LdapPassword. These are used as parameters for the for the 
DirectoryEntryService helper class which is used in UrdmsRoleProvider.
-	CsvSeparator is used for the CSV exporter.
-	LibGuideSource is used as part of the LibGuideService. While the service and controller logic 
is in place it is not currently used in the site.

### Setting up IIS (Windows 7)
1.	Create a web application via the IIS manager console
  - a.	Under the machine name in the connections panel right click on “Sites” and select “Add web application”
  - b.	Add site name as “urdms.local”.
  - c.	Set the “Physical path” point to the location of the MVC application for URDMS.
  - d.	Set the host name as “urdms.local” 
  - e.	Click “Test Settings” if the user that site is running under has required access (in some instances you might have to set that user to administrative access to the machine)
  - f.	Make a note of the name of the application pool
2.	Set application pool settings
  - a.	Select (not already selected) “v4.0” as the “.NET Framework Version”.
  - b.	Select previously notes application pool name under the “Application Pools” section in connections panel.
  - c.	Right click on the pool and select “Advanced Settings”.
  - d.	Change the “enable 32-Bit Applications” to “False”.
  - e.	You might have to set the Identity to a user who has administrative rights to the machine.
3.	Add the following line to the host file located at C:\Windows\System32\drivers\etc\hosts 
    127.0.0.1		urdms.local

### Logging
Currently Elmah is set to log unhandled exceptions using in-memory storage and presenting a web 
view at ~/elmah.axd . Elmah is module-based and allows configuration via web.config - production 
settings will need to be addressed to eliminate security risks.

## URDMS Integration

### Overview
The URDMS Integration project is a collection of NServiceBus (http://nservicebus.com/) services 
for coordinating site-provisioning, data collection approvals, document creation (for snapshots 
of the DMP) and publishing data to VIVO integrating with the open source Java Application 
(http://sourceforge.net/projects/vivo/). The two main workflows considered in the application 
are detailed below.

### Provisioning
#### Provisioning services
The provisioning workflow is concerned with creating a SharePoint site collection for a particular 
project, adding users with the correct permissions, adding document artefacts and also notifying 
relevant parties along the way. It involves interactions between several services as detailed below.

#### ProvisioningService
The provisioning service handles messages to create a sharepoint site which are sent by the web 
application when a user completes a data management plan or data deposit form. The initial message 
contains a project name, description and id as well as all the users that need to be given access 
to a SharePoint site collection and their respective roles. The provisioning service notifies the 
sharepoint (provisioning service) to create a sharepoint site. It also sends a message to the 
notification service which then emails the user that the request to provision a site has been received.
 
Once the sharepoint provisioning service has set up the sharepoint site, it informs the provisioning 
service which in turn messages the notification service to send another email as well as the view model 
updater service. The view model updater service is responsible for updating the state of the data 
management plan or data deposit to “provisioned”. This ends the provisioning workflow.

#### ProvisioningService.SharePoint
As part of the provisioning workflow, the sharepoint provisioning service is responsible for 
communicating with the SharePoint installation via the Client Object Model to create a new site 
collection for a project, add users and allocate their access.

#### NotificationService
The notification service is made up of several handlers for messages it is subscribed to. The 
service is responsible for sending out emails to notify users of the relevant steps within the 
provisioning workflow. The service uses ActionMailer.Net (standalone) to send emails while the 
templates are generated with the RazorEngine standalone (currently does not support layouts hence 
common heater/footer/css needs to be on each and every template).

The service will email all users listed in a project to notify them that a site is currently being 
provisioned for the submitted DMP/DD. Similarly, once the site is provisioned, all users within that 
message are sent an email to notify them that the site has been provisioned and they've been given 
access to it (the site URL is also sent).

#### DocumentBuilderService
The document builder service is responsible for generating a snapshot of a DMP when a site is 
provisioned. Both PDF and XML versions are generated and then deposited in the newly provisioned 
SharePoint site using the client object model. All data is extracted directly from the web application 
database which means that any changes to the schema or content in the web application must be passed 
on to the repository method and final document outputs.

#### ProvisioningService.ViewModelUpdater
The view model updater service for the provisioning workflow is fairly simple, it is only subscribed 
to one message. When that event is raised the view model updater will directly update the status of a 
DMP or DD in the web application database to DmpStatus.Provisioned for the respective project.

### Approvals

#### Data collection approvals services
The data collection approval workflow is in charge of carrying a data collection through all approval 
steps until finally publishing the data collection by inserting a record into a VIVO database table 
(the VIVO application is then responsible for pushing the record to ANDS).

##### Approvals.ApprovalService
The approval service is the main service in charge of the long running approval workflow. It contains 
a saga that handles all messages sent from the web application. The initial message when a researcher 
completes a data collection and confirms it is ready to be published commences the saga. The service 
then publishes a message for the view model updater to update the data collection status directly in 
the web application database.
 
The handlers follow this basic pattern for all of the following messages within the workflow until the 
final message is received to publish the collection, at which point the VIVO publisher service is sent 
a message. Once the response to that message is returned, that view model updater is alerted to make 
the final status change and the workflow is complete.

##### Approvals.ViewModelUpdater
This service is very straight forward, it is only subscribed to an ApprovalStateChanged message which 
is published by the approval service. The message contains the new approval state of the data 
collection as well as the datetime of when it was changed and by whom. The service updates the web 
application database with this information.

##### Approvals.VivoPublisher
The VIVO publisher service handles only one message, the ExportToVivo message published by the approval 
service. Upon receiving this message, the publisher service retrieves the data collection that has been 
approved from the web application database and then saves it into a VIVO schema appropriately. The 
service is NOT responsible for the final publishing of the data to ANDS, the VIVO application handles 
this portion.

### Installation/Set Up
- Create “Urdms.Approvals”, “Urdms.Approvals.Subscriptions”, “Urdms.Provisioning”, “Urdms.Provisioning.Subscriptions”, MS SQL databases.
-	Set up Message Queuing
  -	Go to “Control Panel” -> “Programs and Features”
  -	Select “Turn Windows Features on or off”
  -	Under “MicroSoft Message Queue (MSMQ) Server” select “MicroSoft Message Queue (MSMQ) Server Core”.
  -	Restart your computer.
-	The following MSMQ private queues are needed “urdms.approvals.approvalservice”, 
“urdms.approvals.approvalservice.timeouts”, “urdms.approvals.viewmodelupdater”, 
“urdms.documentbuilderservice”, “urdms.approvals.vivopublisher”, “urdms.notificationservice”, 
“urdms.provisioningservice”, “urdms.provisioningservice.timeouts”, 
“urdms.provisioningservice.sharepoint”, “urdms.provisioningservice.viewmodelupdater” and 
“urdms.error”. The application should create these for you when it is first run but if for some 
reason this did not occur, see the next item for how to add them manually.
-	To add queues
  -	Right click on “Computer” from the start menu or desktop and select “Manage”.
  -	Open “Services and Applications” menu item
  -	Open “Message Queuing” menu item
  -	Right click on the “Private Queues” menu item and select “New” -> “Private Queue”.
  -	Enter in the queue name and click OK.

### Implementation
-	To run and debug the solution you will need to set some start up projects so all required services 
are up and running. Set the following start up projects; “Approvals.ApprovalService”, 
“Approvals.ViewModelUpdater”, “Approvals.VivoPublisher”, “DocumentBuilderService”, 
“NotificationService”, “ProvisioningService”, “ProvisioningService.SharePoint” and 
“ProvisioningService.ViewModelUpdater”. Now when you debug your solution you will see console 
windows for each service and output will inform you of the state of the service, queues, database, etc.
- Take note that the NServiceBus host is limited to 1 worker thread without a valid license. This 
may be enough to suit your needs but if it isn’t then you’ll need to acquire the appropriate licence.
-	The document builder service was built using the ABC PDF library to generate PDFs (although it also 
generates XML documents from the data). The code to do so has been left in place however you’ll need 
to acquire a license (if you intend to keep using this service), include the library and uncomment 
the relevant areas of code. You may also need an xcopy command to copy the library to the bin/Debug 
folder, post-build. E.g.
  -	xcopy "$(ProjectDir)..\lib\abcpdf\ABCpdf8-64.dll" "$(TargetDir)" /Y /C /D
-	The final step in creating a Data Managerment Plan is the provisioning of a sharepoint site. 
Once again the code has been commented and left in place in case you also choose a sharepoint solution, 
you can choose to reuse this code or implement another solution (or do nothing) at this stage.

###Deployment
To Deploy the NServiceBus services to test or production:
1.	Build the solution in Visual Studio in Release mode.
2.	Run the PackageForDeployment.bat file in the root of the solution folder.
3.	Zip the folder created with today's date/time in the _Deploy folder and send it to whoever can deploy to the required environment.
4.	The person with permissions to deploy should then unzip the file on the target server and running the Deploy.bat file providing the username and password of the service account used to run the services 
i.e., Deploy.bat service_account service_password
5.	This will install the services and start them automatically.
 App.config settings
-	All NServiceBus endpoints have already been configured appropriately although some extra details will need to be entered for some of the services.
-	All App.config files have transforms for the CI, Test and Prod servers.
-	You’ll need to update database connections, email addresses, URLs and mail servers to appropriate values.
