using System;
using System.Collections.Generic;
using NServiceBus;
using Urdms.Approvals.ApprovalService.Commands;
using Urdms.Approvals.VivoPublisher.Messages;
using Urdms.DocumentBuilderService.Commands;
using Urdms.ProvisioningService.Commands;
using log4net;

namespace ClientEndpoint
{
    public class Program
    {
        private static Dictionary<string, string> _userRoles;

        private static IBus _bus;

        static void Main()
        {
            Init();

          CreateSitesTest();
            // SubmitDataCollection();
            //   Vivopublish();

           //  GerenateDmp();

        }

       

        private static void Init()
        {
            _userRoles = new Dictionary<string, string>();
            _userRoles.Add("Owners", "GH13579");
            _userRoles.Add("Members", "TY22223,XX12343");
            _userRoles.Add("Visitors", "FH13545");


            _bus = Configure
                .With()
                    .DefaultBuilder()
                    .UseTransport<Msmq>()
                    .PurgeOnStartup(false)
                                .InMemorySubscriptionStorage()
                            .UnicastBus()
                       
                                .LoadMessageHandlers()
                            .CreateBus()
                            .Start(() => Configure.Instance.ForInstallationOn<NServiceBus.Installation.Environments.Windows>().Install());
        }

        private static void CreateSitesTest()
        {
            try
            {
                Console.WriteLine("Press enter to create a provision a new project.\nTo exit, enter 'q'.");

                string line;

                while ((line = Console.ReadLine().ToLower()) != "q")
                {
                    var projectId = new Random().Next(1000);

                  _bus.Send<SiteRequestCommand>(m =>
                                                         {
                                                             m.ProjectId = projectId;
                                                             m.ProjectTitle = String.Format("Test Project {0}", projectId);
                                                             m.ProjectDescription = String.Format("This is a test description for Project {0}", projectId);
                                                             m.UserRoles = _userRoles;
                                                         });

                    Console.WriteLine(String.Format("Request sent for Project {0}", projectId));
                }
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex);
                Console.Read();
            }
        }

        [Obsolete]
        private static void SubmitDataCollection()
        {
            int id = 1;

            try
            {
                Console.WriteLine("Press enter to submit a data collection.\nTo exit, enter 'q'.");

                if (Console.ReadLine().ToLower() != "q")
                {
                    _bus.Send<SubmitForApproval>(m =>
                            {
                                m.ApprovedBy = "GH13579";
                                m.ApprovedOn = DateTime.Now;
                                m.DataCollectionId = id;
                            });

                    Console.WriteLine(String.Format("SubmitForApproval sent for Data Collection {0}. Press enter to send SubmitForSecondaryApproval.", id));
                }
                else
                {
                    return;
                }

                if (Console.ReadLine().ToLower() != "q")
                {
                    _bus.Send<SubmitForSecondaryApproval>(m =>
                            {
                                m.ApprovedBy = "787878u";
                                m.ApprovedOn = DateTime.Now.AddDays(1);
                                m.DataCollectionId = id;
                            });

                    Console.WriteLine(String.Format("SubmitForSecondaryApproval sent for Data Collection {0}. Press enter to send SubmitForFinalApproval.", id));
                }
                else
                {
                    return;
                }


                if (Console.ReadLine().ToLower() != "q")
                {
                    _bus.Send<SubmitForFinalApproval>(m =>
                    {
                        m.ApprovedBy = "454545p";
                        m.ApprovedOn = DateTime.Now.AddDays(1);
                        m.DataCollectionId = id;
                    });

                    Console.WriteLine(String.Format("SubmitForFinalApproval sent for Data Collection {0}.", id));
                }
                else
                {
                    return;
                }



                Console.Read();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.Read();
            }


        }


        private static void Vivopublish()
        {
            var datacollectionid = 1;

            Console.WriteLine("Press enter to send a message to the VivoPublisher");
            while (Console.ReadLine().ToLower() != "q")
            {
                if (datacollectionid > 22)
                    datacollectionid = 1;

                _bus.Send<ExportToVivo>(m =>
                {
                    m.DataCollectionId = datacollectionid;
                });

                datacollectionid++;
                Console.WriteLine("Sent ExportToVivo message for data collection {0}.", datacollectionid);
                Console.ReadLine();
            }
        }


        private static void GerenateDmp()
        {
            Console.WriteLine("Press enter to generate dmp.");
            while (Console.ReadLine().ToLower() != "q")
            {
                _bus.Send<GenerateDmpCommand>(m =>
                                                  {
                                                      m.ProjectId = 1;
                                                      m.SiteUrl = "https://sharepoint.yourdomain.edu.au/research/project485";
                                                  });

                Console.WriteLine("Sent");
                Console.ReadLine();
            }
        }

    }
}
