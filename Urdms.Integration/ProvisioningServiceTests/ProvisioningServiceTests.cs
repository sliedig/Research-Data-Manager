using System;
using System.Collections.Generic;
using NServiceBus.Testing;
using NUnit.Framework;
using Urdms.DocumentBuilderService.Commands;
using Urdms.NotificationService.Messages;
using Urdms.ProvisioningService;
using Urdms.ProvisioningService.Commands;
using Urdms.ProvisioningService.Events;
using Urdms.ProvisioningService.Messages;


namespace ProvisioningServiceTests
{
    [TestFixture]
    public class ProvisioningServiceShould
    {
        private string _siteUrl;
        private int _projectId;

        private Dictionary<string, string> _userRoles;
        private List<SiteUser> _userRolesList;


        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            Test.Initialize();

            _userRoles = new Dictionary<string, string>();
            _userRoles.Add("Owners", "WS12323");
            _userRoles.Add("Members", "FG93943,BB99990");
            _userRoles.Add("Visitors", "DH09887");

            _userRolesList = new List<SiteUser>
            {
                new SiteUser {Role = "Owners", UserId = "WS12323"},
                new SiteUser {Role = "Members", UserId = "FG93943"},
                new SiteUser {Role = "Members", UserId = "BB99990"},
                new SiteUser {Role = "Visitors", UserId = "DH09887"}
            };
        }

        [SetUp]
        public void TestSetUp()
        {
            _projectId = new Random().Next(1000);
            _siteUrl = "https://sharepoint.yourdomain.edu.au/reserach/project" + _projectId;
        }



        [Test]
        public void Complete_if_provisioning_is_complete()
        {
            var requestCommand = CreateRequestCommand();

            var createSiteResponse = new CreateSiteResponse
                        {
                            ProjectId = _projectId,
                            SiteUrl = _siteUrl,
                            RequestId = 1,
                            ProvisioningRequestStatus = ProvisioningRequestStatus.Provisioned
                        };

            Test.Saga<ProvisioningSaga>()
                .ExpectPublish<ProvisioningStatusChanged>(m =>
                    m.ProjectId == requestCommand.ProjectId &&
                    m.RequestId == -1 &&
                    m.SiteUrl == null &&
                    m.ProvisioningRequestStatusId == (int)ProvisioningRequestStatus.Pending)
                .ExpectSend<CreateSiteRequest>(m =>
                    m.ProjectId == requestCommand.ProjectId &&
                    m.SiteTitle == requestCommand.ProjectTitle &&
                    m.SiteDescription == requestCommand.ProjectDescription &&
                    m.UsersInRoles[0].UserId == _userRolesList[0].UserId &&
                    m.UsersInRoles[1].UserId == _userRolesList[1].UserId &&
                    m.UsersInRoles[2].UserId == _userRolesList[2].UserId &&
                    m.UsersInRoles[3].UserId == _userRolesList[3].UserId)
                .ExpectSend<NotifyRequestForSiteReceived>(m =>
                    m.ProjectId == requestCommand.ProjectId &&
                    m.ProjectName == requestCommand.ProjectTitle &&
					m.UserIds.Contains("WS12323"))
                .ExpectPublish<ProvisioningStatusChanged>(m =>
                    m.ProjectId == _projectId &&
                    m.SiteUrl == _siteUrl &&
                    m.ProvisioningRequestStatusId == (int)ProvisioningRequestStatus.Provisioned)
                .ExpectSend<GenerateDmpCommand>(m => m.ProjectId == requestCommand.ProjectId && m.SiteUrl == _siteUrl)
                .When(x =>
                {
                    x.Handle(requestCommand);
                    x.Handle(createSiteResponse);

                })
                .AssertSagaCompletionIs(true);
        }

        [Test]
        public void Not_complete_if_provisioning_is_failed_or_timed_out()
        {
            var requestCommand = CreateRequestCommand();

            var createSiteResponse = new CreateSiteResponse
            {
                ProjectId = _projectId,
                SiteUrl = null,
                RequestId = 1,
                ProvisioningRequestStatus = ProvisioningRequestStatus.TimeOut
            };

            Test.Saga<ProvisioningSaga>()
                .ExpectPublish<ProvisioningStatusChanged>(m =>
                    m.ProjectId == requestCommand.ProjectId &&
                    m.RequestId == -1 &&
                    m.SiteUrl == null &&
                    m.ProvisioningRequestStatusId == (int)ProvisioningRequestStatus.Pending)
                .ExpectSend<CreateSiteRequest>(m =>
                    m.ProjectId == requestCommand.ProjectId &&
                    m.SiteTitle == requestCommand.ProjectTitle &&
                    m.SiteDescription == requestCommand.ProjectDescription &&
                    m.UsersInRoles[0].UserId == _userRolesList[0].UserId &&
                    m.UsersInRoles[1].UserId == _userRolesList[1].UserId &&
                    m.UsersInRoles[2].UserId == _userRolesList[2].UserId &&
                    m.UsersInRoles[3].UserId == _userRolesList[3].UserId)
                .ExpectSend<NotifyRequestForSiteReceived>(m =>
                    m.ProjectId == requestCommand.ProjectId &&
                    m.ProjectName == requestCommand.ProjectTitle &&
					m.UserIds.Contains("WS12323"))
                .ExpectPublish<ProvisioningStatusChanged>(m =>
                    m.ProjectId == _projectId &&
                    m.RequestId == 1 &&
                    m.SiteUrl == null &&
                    m.ProvisioningRequestStatusId == (int)ProvisioningRequestStatus.TimeOut)
                .When(x =>
                {
                    x.Handle(requestCommand);
                    x.Handle(createSiteResponse);

                })
                .AssertSagaCompletionIs(false);
        }


        [Test]
        public void Complete_when_force_provisioning_completion_command_is_received()
        {
            var completeCommand = new ForceProvisioningCompletionCommand { ProjectId = 1, SiteUrl = _siteUrl };

            Test.Saga<ProvisioningSaga>()
                .ExpectPublish<ProvisioningStatusChanged>(m =>
                    m.ProjectId == 1 &&
                    m.RequestId == 1 &&
                    m.SiteUrl == _siteUrl &&
                    m.ProvisioningRequestStatusId == (int)ProvisioningRequestStatus.Provisioned)
                .ExpectSend<GenerateDmpCommand>(m => m.ProjectId == 1 && m.SiteUrl == _siteUrl)
                .When(x =>
                        {
                            x.Data.ProjectId = 1;
                            x.Data.SiteUrl = null;
                            x.Data.RequestId = 1;
                            x.Handle(completeCommand);
                        })
                .AssertSagaCompletionIs(true);
        }

        private SiteRequestCommand CreateRequestCommand()
        {
            return new SiteRequestCommand
                       {
                           ProjectId = _projectId,
                           ProjectTitle = "Kalman Smoothing to Achieve Parametric Continuity of Sample-Based Robot Motion Plans in the Presence of Obstacles and Kinodynamic Constraints",
                           ProjectDescription = "Sampling is an effective approach to robot motion planning but inherently produces jagged trajectories. We present a new approach to trajectory smoothing. Results suggest that this algorithm performs well for holonomic and kinodynamically-constrained robots and offers several advantages over shortcut methods, which cannot incorporate kinodynamic constraints. Given an initial trajectory, the algorithm combines Kalman Smoothing with a pseudo-dynamics model and specified smoothness constraints to compute a new trajectory that achieves parametric continuity of degree $k$. The input is treated as a noisy observation to ensure that the smoothed trajectory satisfies the kinodynamic constraints of the robot. A noise parameter, which determines the amount of smoothing, can be adapted automatically along the path based on local uncertainty and distance to the nearest obstacle, and can also incorporate a sensor model if one is available. We evaluate the algorithm on examples including a holonomic robot and a kinodynamic robot with car-like dynamics. The resulting trajectories are smooth, collision-free, and obey the kinodynamic robot constraints in environments with narrow passages and dense obstacles.",
                           UserRoles = _userRoles

                       };
        }
    }
}
