using System;
using System.Collections.Generic;
using NServiceBus.Testing;
using NSubstitute;
using NUnit.Framework;
using Urdms.Approvals.ApprovalService.Events;
using Urdms.NotificationService;
using Urdms.NotificationService.Database.Repositories;
using Urdms.NotificationService.Handlers;
using Urdms.NotificationService.Messages;
using Urdms.NotificationService.Models;
using Urdms.NotificationService.UserSearch;

namespace NotificationService.Tests
{
    [TestFixture]
    public class NotificationServiceShould
    {
        private IMailer _mailer;
        private IApprovalStateChangedRepository _approvalStateChangedRepository;
        private IUrdmsUserService _urdmsUserService;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            Test.Initialize();
        }

        [SetUp]
        public void SetUp()
        {
            _urdmsUserService = Substitute.For<IUrdmsUserService>();
            _mailer = Substitute.For<IMailer>();
            _approvalStateChangedRepository = Substitute.For<IApprovalStateChangedRepository>();

        }

        [TearDown]
        public void TearDown()
        {
            _mailer = null;
            _approvalStateChangedRepository = null;
        }

        [Test]
        public void Respond_with_a_request_for_site_collection_received_given_a_request_for_site_collection_received_message()
        {
            var user = GetUrdmsUser("GH13579", "Jack", "Smith");
            _urdmsUserService.GetUser(Arg.Is(user.UserId)).Returns(user);

            Test.Handler(new NotifyRequestForSiteReceivedHandler(_mailer, _urdmsUserService))
                .OnMessage<NotifyRequestForSiteReceived>(m =>
                    {
                        m.ProjectId = 1234;
                        m.ProjectName = "Test Project";
                        m.UserIds = new List<string> { user.UserId };
                    });
        }


        [Test]
        public void Call_send_mail_given_an_approval_state_changed_message_with_data_collection_in_submitted_state()
        {
            var handler = new NotifyApprovalStateChangedHandler(_mailer, _approvalStateChangedRepository, _urdmsUserService);
            _approvalStateChangedRepository
                .Get(Arg.Is(1))
                .Returns(new ApprovalStateChangedEmailData { DataCollectionTitle = "Test", ProjectTitle = "Test Project", Manager = "Paul McTest" });
            var urdmsUser = GetUrdmsUser("XX12343", "John", "Doe");
            _urdmsUserService.GetUser(Arg.Is(urdmsUser.UserId)).Returns(urdmsUser);

            Test.Handler(handler)
                .OnMessage<NotifyApprovalStateChanged>(m =>
                                                     {
                                                         m.ApprovalState = DataCollectionApprovalState.Submitted.ToString();
                                                         m.DataCollectionId = 1;
                                                         m.Approver = urdmsUser.UserId;
                                                     });

            _mailer.Received().SendEmail(Arg.Any<ApprovalStateChangedEmail>(), "ApprovalStateChangedSubmitted");
            _approvalStateChangedRepository.Received().Get(Arg.Is(1));
        }

        [Test]
        public void Call_send_mail_given_an_approval_state_changed_message_with_data_collection_in_qa_approved_state()
        {
            var handler = new NotifyApprovalStateChangedHandler(_mailer, _approvalStateChangedRepository, _urdmsUserService);
            _approvalStateChangedRepository
                .Get(Arg.Is(1)).Returns(new ApprovalStateChangedEmailData { DataCollectionTitle = "Test", ProjectTitle = "Test Project", Manager = "Paul McTest" });
            var user = GetUrdmsUser("XX12343", "John", "Doe");
            _urdmsUserService.GetUser(Arg.Is(user.UserId)).Returns(user);

            Test.Handler(handler)
                .OnMessage<NotifyApprovalStateChanged>(m =>
                {
                    m.ApprovalState = DataCollectionApprovalState.QaApproved.ToString();
                    m.DataCollectionId = 1;
                    m.Approver = user.UserId;
                });

            _mailer.Received().SendEmail(Arg.Any<ApprovalStateChangedEmail>(), "ApprovalStateChangedQaApproved");
            _approvalStateChangedRepository.Received().Get(Arg.Is(1));

        }

        [Test]
        public void Call_send_mail_given_an_approval_state_changed_message_with_data_collection_in_ord_approved_state()
        {
            var handler = new NotifyApprovalStateChangedHandler(_mailer, _approvalStateChangedRepository, _urdmsUserService);
            _approvalStateChangedRepository
                .Get(Arg.Is(1)).Returns(new ApprovalStateChangedEmailData { DataCollectionTitle = "Test", ProjectTitle = "Test Project", Manager = "Paul McTest" });
            var user = GetUrdmsUser("XX12343", "John", "Doe");
            _urdmsUserService.GetUser(Arg.Is(user.UserId)).Returns(user);

            Test.Handler(handler)
                .OnMessage<NotifyApprovalStateChanged>(m =>
                {
                    m.ApprovalState = DataCollectionApprovalState.SecondaryApproved.ToString();
                    m.DataCollectionId = 1;
                    m.Approver = user.UserId;
                });

            _mailer.Received().SendEmail(Arg.Any<ApprovalStateChangedEmail>(), "ApprovalStateChangedSecondaryApproved");
            _approvalStateChangedRepository.Received().Get(Arg.Is(1));
        }

        [Test]
        public void Call_send_mail_given_an_approval_state_changed_message_with_data_collection_in_record_amended_state()
        {
            var handler = new NotifyApprovalStateChangedHandler(_mailer, _approvalStateChangedRepository, _urdmsUserService);
            _approvalStateChangedRepository
                .Get(Arg.Is(1))
                .Returns(new ApprovalStateChangedEmailData { DataCollectionTitle = "Test", ProjectTitle = "Test Project", Manager = "Paul McTest" });
            var user = GetUrdmsUser("XX12343", "John", "Doe");
            _urdmsUserService.GetUser(Arg.Is(user.UserId)).Returns(user);


            Test.Handler(handler)
                           .OnMessage<NotifyApprovalStateChanged>(m =>
                           {
                               m.ApprovalState = DataCollectionApprovalState.RecordAmended.ToString();
                               m.DataCollectionId = 1;
                               m.Approver = user.UserId;
                           });

            _mailer.Received().SendEmail(Arg.Any<ApprovalStateChangedEmail>(), "ApprovalStateChangedRecordAmended");
            _approvalStateChangedRepository.Received().Get(Arg.Is(1));
        }

        [Test]
        public void Call_send_mail_given_an_approval_state_changed_message_with_data_collection_in_published_state()
        {
            var handler = new NotifyApprovalStateChangedHandler(_mailer, _approvalStateChangedRepository, _urdmsUserService);
            _approvalStateChangedRepository
                .Get(Arg.Is(1))
                .Returns(new ApprovalStateChangedEmailData
                             {
                                 DataCollectionTitle = "Test",
                                 ProjectTitle = "Test Project",
                                 Manager = "Paul McTest",
								 ManagerId = "WD32423"
                             });

            var user = GetUrdmsUser("WD32423", "John", "Doe");

            _urdmsUserService.GetUser(Arg.Is(user.UserId)).Returns(user);


            Test.Handler(handler)
                           .OnMessage<NotifyApprovalStateChanged>(m =>
                           {
                               m.ApprovalState = DataCollectionApprovalState.Published.ToString();
                               m.DataCollectionId = 1;
                               m.Approver = user.UserId;
                           });

            _mailer.Received().SendEmail(Arg.Any<ApprovalStateChangedEmail>(), "ApprovalStateChangedPublished");
            _approvalStateChangedRepository.Received().Get(Arg.Is(1));
        }

        [Test]
        public void Return_formatted_string_on_call_to_base_email_to_string()
        {
            var emailMessage = new ApprovalStateChangedEmail
                                   {
                                       To = new List<string> { "s@t.com", "w@x.com", "d@e.com" },
                                       Subject = "Data Collection Approved"
                                   };

            Assert.That(emailMessage.ToString(), Is.EqualTo("Subject: Data Collection Approved; Recipients: s@t.com, w@x.com, d@e.com;"));
        }

        private static UrdmsUser GetUrdmsUser(string userId, string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("parameter cannot be empty", "userId");
            }
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("parameter cannot be empty", "firstName");
            }
            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("parameter cannot be empty", "lastName");
            }
            var user = new UrdmsUser
                           {
                               UserId = userId,
                               FirstName = firstName,
                               LastName = lastName,
                               FullName = string.Format("{0} {1}", firstName, lastName),
                               EmailAddress = string.Format("{0}.{1}@yourdomain.edu.au", firstName.ToLower(), lastName.ToLower())
                           };
            return user;
        }
    }
}
