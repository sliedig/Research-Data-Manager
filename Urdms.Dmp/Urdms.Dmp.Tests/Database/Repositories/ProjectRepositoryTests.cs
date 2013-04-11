using System.Linq;
using Curtin.Framework.Common.Extensions;
using FizzWare.NBuilder;
using NUnit.Framework;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Tests.Helpers;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Tests.Database.Repositories
{
    [TestFixture]
    class ProjectRepositoryShould : DbTestBase
    {
        private IProjectRepository _repository;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _repository = new ProjectRepository(CreateSession());
            
        }
        
        [Test]
        public void Return_a_empty_projects_list_for_a_invaild_userid()
        {
            var projects = _repository.GetByPrincipalInvestigator("31312");

            Assert.That(projects.All(o => o.Parties.IsEmpty()));
        }

        [Test]
        public void Return_a_list_of_projects_for_a_valid_userid_for_get_by_principal_investigator()
        {
            const string userId = "XX12345";
            var principalInvestigator = Builder<Party>.CreateNew().With(p => p.UserId = userId).And(p => p.Id = 0).Build();
            var otherParty = Builder<Party>.CreateNew().With(p => p.Id = 0).Build();
            var projects = Builder<Project>
                .CreateListOfSize(3)
                .All()
                .With(x => x.Id = 0)
                .And(x => x.Parties
                    .AddRange(Builder<ProjectParty>
                        .CreateListOfSize(2)
                        .TheFirst(1)
                            .With(m => m.Id = 0)
                            .And(m => m.Project = x)
                            .And(m => m.Party = principalInvestigator)
                            .And(m => m.Role = AccessRole.Owners)
                        .TheLast(1)
                            .With(m => m.Id = 0)
                            .And(m => m.Project = x)
                            .And(m => m.Party = otherParty)
                            .And(m => m.Role = AccessRole.Visitors)
                        .Build()))
                .Build();

            projects.Do(p => Session.Save(p));

            var savedProjects = _repository.GetByPrincipalInvestigator(userId);

            Assert.That(savedProjects.Count, Is.EqualTo(3));
            Assert.That(savedProjects[0].Parties.Count, Is.EqualTo(2));
        }

        [Test]
        public void Return_a_vaild_users_list_for_a_vaild_projectkey()
        {
            var project = new Project
                             {
                                 Title = "Project1",
                                 Description = "Project details",
                             };

            var projectParty = Builder<ProjectParty>.CreateNew()
                .With(p => p.Id = 0)
                .And(p => p.Project = project)
                .And(p => p.Party = Builder<Party>.CreateNew().With(u => u.Id = 0).Build())
                .And(p => p.Role = AccessRole.Owners)
                .Build();

            project.Parties.Add(projectParty);

            _repository.Save(project);

            var createdEntity = _repository.Get(project.Id);
            Assert.That(createdEntity, Is.Not.Null, "Data Management Plan not retrieved");
            Assert.That(createdEntity.Parties, Is.Not.Null, "User access list is null");
            Assert.That(createdEntity.Parties.Count, Is.EqualTo(1), "User access list size is not 1");
        }

        [Test]
        public void Return_a_project_with_valid_keywords_list_for_an_existing_project()
        {
            var entity = Builder<Project>.CreateNew()
                .With(o => o.Id = 0)
                .And(o => o.Keywords = "1,2")
                .Build();

            _repository.Save(entity);
            var savedEntity = _repository.Get(entity.Id);

            Assert.That(savedEntity, Is.Not.Null, "Project does not exist");
            Assert.That(savedEntity.Keywords, Is.Not.Null, "No keywords in project");
            Assert.That(savedEntity.Keywords.Split(',').Length, Is.EqualTo(2), "incorrect number of keywords");
        }

        [Test]
        public void Return_a_project_for_a_valid_Dmp_Id()
        {
            var entity = Builder<Project>.CreateNew()
                .With(o => o.Id = 0)
                .And(o => o.DataManagementPlan = new DataManagementPlan { Id = 0 })
                .Build();
            Session.Save(entity);

            var savedEntity = _repository.GetByDataManagementPlanId(entity.DataManagementPlan.Id);
            Assert.That(savedEntity.DataManagementPlan.Id, Is.EqualTo(entity.DataManagementPlan.Id));
        }

        [Test]
        public void Return_a_project_for_a_valid_data_deposit_id()
        {
            var entity = Builder<Project>
                .CreateNew()
                .With(o => o.Id = 0)
                .And(o => o.DataDeposit = new DataDeposit {Id = 0})
                .Build();
            Session.Save(entity);

            var savedEntity = _repository.GetByDataDepositId(entity.DataDeposit.Id);
            Assert.That(savedEntity.DataDeposit.Id, Is.EqualTo(entity.DataDeposit.Id));
        }

        [Test]
        public void Return_a_list_of_projects_which_failed_provisioning()
        {
            var projects = Builder<Project>.CreateListOfSize(5).Build();
            var status = ProvisioningStatus.Error;
            projects.Do(p => {
                Session.Save(p);
                Session.CreateSQLQuery(string.Format("UPDATE [Project] SET [ProvisioningStatus]={0} WHERE [Id] = {1}", (int)(status++), p.Id)).ExecuteUpdate();
            });
            Session.Flush();

            var retrievedProjects = _repository.GetAllWhichFailedProvisioning();
            
            Assert.That(retrievedProjects, Has.Count.EqualTo(2));
            Assert.That(retrievedProjects[0].Id, Is.EqualTo(projects[0].Id));
            Assert.That(retrievedProjects[1].Id, Is.EqualTo(projects[1].Id));
        }
    }
}
