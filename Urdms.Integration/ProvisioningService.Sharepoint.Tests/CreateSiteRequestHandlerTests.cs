using System;
using System.Collections.Generic;
using System.Reflection;
using NServiceBus.Testing;
using NUnit.Framework;
using Urdms.ProvisioningService;
using Urdms.ProvisioningService.Commands;
using Urdms.ProvisioningService.Messages;
using Urdms.ProvisioningService.SharePoint;

namespace ProvisioningService.SharePoint.Tests
{
    [TestFixture]
    [Ignore("This test should be run manually.")]
    internal class CreateSiteRequestHandlerTests
    {
        List<SiteUser> _usersInRoles;

        [TestFixtureSetUp]
        public void Setup()
        {
            var assemblies = new[]
                     {
                         typeof (ProvisioningSaga).Assembly,
                         typeof (SiteRequestCommand).Assembly,
                         Assembly.Load("NServiceBus"),
                         Assembly.Load("NServiceBus.Core")
                     };

            Test.Initialize(assemblies);

            _usersInRoles = new List<SiteUser>
                                {
                                    new SiteUser {UserId = "TY22223", Role = "Owners"},
                                    new SiteUser {UserId = "GH13579", Role = "Owners"},
                                    new SiteUser {UserId = "TY22223", Role = "Members"},
                                    new SiteUser {UserId = "WW55443", Role = "Visitors"},
                                    new SiteUser {UserId = "GH13579", Role = "Members"}
                                };
        }



        [Test]
        [Ignore("This test should be run manually.")]
        public void CreateProjectSiteHandlerTest()
        {
            var projectId = new Random().Next(1000);

            Test.Handler<CreateSiteRequestHandler>()
                .ExpectReply<CreateSiteResponse>(m =>
                    m.ProjectId == projectId &&
                    m.SiteUrl.StartsWith(string.Format("https://sharepoint.yourdomain.edu.au/research/project{0}",projectId.ToString())) &&
                    m.ProvisioningRequestStatus == ProvisioningRequestStatus.Provisioned && 
                    m.RequestId > 0)
                .OnMessage<CreateSiteRequest>(m =>
                {
                    m.ProjectId = projectId;
                   // m.SiteTitle = "Kalman Smoothing to Achieve Parametric Continuity of Sample-Based Robot Motion Plans in the Presence of Obstacles and Kinodynamic Constraints";
                    m.SiteTitle = "Project "+ projectId +" Site Title";
                    m.SiteDescription = "Sampling is an effective approach to robot motion planning but inherently produces jagged trajectories. We present a new approach to trajectory smoothing. Results suggest that this algorithm performs well for holonomic and kinodynamically-constrained robots and offers several advantages over shortcut methods, which cannot incorporate kinodynamic constraints. Given an initial trajectory, the algorithm combines Kalman Smoothing with a pseudo-dynamics model and specified smoothness constraints to compute a new trajectory that achieves parametric continuity of degree $k$. The input is treated as a noisy observation to ensure that the smoothed trajectory satisfies the kinodynamic constraints of the robot. A noise parameter, which determines the amount of smoothing, can be adapted automatically along the path based on local uncertainty and distance to the nearest obstacle, and can also incorporate a sensor model if one is available. We evaluate the algorithm on examples including a holonomic robot and a kinodynamic robot with car-like dynamics. The resulting trajectories are smooth, collision-free, and obey the kinodynamic robot constraints in environments with narrow passages and dense obstacles.";
                    m.UsersInRoles = _usersInRoles;
                });
        }

    }
}
