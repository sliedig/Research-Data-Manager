using System;
using System.Collections.Generic;
using System.Linq;
using Curtin.Framework.Common.Extensions;
using FizzWare.NBuilder;
using NUnit.Framework;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Entities.Components;
using Urdms.Dmp.Integration.UserService;
using Urdms.Dmp.Mappers;
using Urdms.Dmp.Models;
using Urdms.Dmp.Models.DataDeposit;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Tests.Mappers
{
    [TestFixture]
    class ProjectMapperShould
    {
        private Project _project;
        private Party _principalInvestigator;
        private IList<Party> _urdmsUsers;
        private IList<Party> _nonUrdmsUsers;
        private ProjectFunder _funder;

        #region Test Setup

        [SetUp] 
        public void SetUp()
        {
            _project = Builder<Project>.CreateNew()
                .With(o => o.DataManagementPlan = Builder<DataManagementPlan>.CreateNew()
                                                      .With(
                                                          q =>
                                                          q.BackupPolicy =
                                                          Builder<BackupPolicy>.CreateNew().Build())
                                                      .And(
                                                          q =>
                                                          q.DataDocumentation =
                                                          Builder<DataDocumentation>.CreateNew().Build())
                                                      .And(
                                                          q =>
                                                          q.DataRelationshipDetail =
                                                          Builder<DataRelationshipDetail>.CreateNew().Build())
                                                      .And(
                                                          q =>
                                                          q.DataRetention =
                                                          Builder<DataRetention>.CreateNew().Build())
                                                      .And(
                                                          q =>
                                                          q.DataSharing =
                                                          Builder<DataSharing>.CreateNew().Build())
                                                      .And(
                                                          q =>
                                                          q.DataStorage =
                                                          Builder<DataStorage>.CreateNew().Build())
                                                      .And(q => q.Ethic = Builder<Ethic>.CreateNew().Build())
                                                      .And(
                                                          q =>
                                                          q.ExistingDataDetail =
                                                          Builder<ExistingDataDetail>.CreateNew().Build())
                                                      .And(
                                                          q =>
                                                          q.Confidentiality =
                                                          Builder<Confidentiality>.CreateNew().Build())
                                                      .And(
                                                          q =>
                                                          q.NewDataDetail =
                                                          Builder<NewDataDetail>.CreateNew().Build())
                                                      .Build())
                .Build();

            _project.SocioEconomicObjectives.AddRange(
                Builder<ProjectSocioEconomicObjective>.CreateListOfSize(4).All().With(
                    q => q.SocioEconomicObjective = Builder<SocioEconomicObjective>.CreateNew().Build()).Build());
            _project.FieldsOfResearch.AddRange(
                Builder<ProjectFieldOfResearch>.CreateListOfSize(5).All().With(
                    q => q.FieldOfResearch = Builder<FieldOfResearch>.CreateNew().Build()).Build());
            _project.Funders.AddRange(
                Builder<ProjectFunder>.CreateListOfSize(1).All().With(
                    q => q.Funder = Pick<Funder>.RandomItemFrom(new[] {Funder.ARC, Funder.NMHRC})).Build());
            _project.Parties.AddRange(Builder<ProjectParty>.CreateListOfSize(6)
                                          .TheFirst(3)
                                          .With(q => q.Party = Builder<Party>.CreateNew()
                                                                   .With(r => r.UserId = null)
                                                                   .Build())
                                          .And(q => q.Relationship = ProjectRelationship.ExternalResearcher)
                                          .And(q => q.Role = AccessRole.Visitors)
                                          .TheNext(2)
                                          .With(q => q.Role = AccessRole.Members)
                                          .And(q => q.Party = Builder<Party>.CreateNew()
                                                                   .With(r => r.UserId = null)
                                                                   .Build())
                                          .And(q => q.Relationship = ProjectRelationship.Investigator)
                                          .TheNext(1)
                                          .With(q => q.Party = Builder<Party>.CreateNew()
                                                                   .With(r => r.UserId = "123456A")
                                                                   .Build())
                                          .And(q => q.Role = AccessRole.Owners)

                                          .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                                          .Build());

            for (var i = 1; i < 7; i++)
                _project.Parties[i - 1].Party.Id = i;

            _funder = _project.Funders.First();
            _principalInvestigator = _project.Parties.Single(o => o.Role == AccessRole.Owners).Party;

            var users = _project.Parties.Where(pp => pp.Party.Id != _principalInvestigator.Id).ToList();

            _urdmsUsers =
                users.Where(o => !string.IsNullOrWhiteSpace(o.Party.UserId)).Select(o => o.Party).ToList();
            _nonUrdmsUsers =
                users.Where(o => string.IsNullOrWhiteSpace(o.Party.UserId)).Select(o => o.Party).ToList();
            
        }

        #endregion

        [Test]
        public void Map_to_a_basic_project_view_model()
        {
            var model = new ProjectViewModel();
            model.MapFrom(_project);

            Assert.That(model,Is.Not.Null,"View model is null");
            Assert.That(model.Id,Is.EqualTo(_project.Id),"View model has incorrect id");
            Assert.That(model.Title,Is.Not.Null,"Project name is empty");
            Assert.That(model.SocioEconomicObjectives.Count,Is.EqualTo(_project.SocioEconomicObjectives.Count),"Socio economic objectives count is incorrect");
            Assert.That(model.FieldsOfResearch.Count,Is.EqualTo(_project.FieldsOfResearch.Count),"Fields of research count is incorrect");
            Assert.That(model.SocioEconomicObjectives.All(o => _project.SocioEconomicObjectives.Any(q => q.SocioEconomicObjective.Id == o.Code.Id)),"Incorrect socio economic objectives");
            Assert.That(model.FieldsOfResearch.All(o => _project.FieldsOfResearch.Any(q => q.FieldOfResearch.Id == o.Code.Id)),"Incorrect fields of research");
            Assert.That(model.DataManagementPlan, Is.Null, "Data management plan is not null");
            Assert.That(model.PrincipalInvestigator, Is.Not.Null, "Principal Investigator is null");
            Assert.That(model.PrincipalInvestigator.UserId, Is.EqualTo(_principalInvestigator.UserId), "Principal investigator is incorrectly retrieved");
        }

        [Test]
        public void Map_to_full_project_view_model()
        {
            var model = ProjectViewModel.NewFullViewModel();
            model.MapFrom(_project);

            Assert.That(model,Is.Not.Null,"View model is null");
            Assert.That(model.Id, Is.EqualTo(_project.Id), "View model has incorrect id");
            Assert.That(model.Title, Is.Not.Null, "Project name is empty");
            Assert.That(model.SocioEconomicObjectives,Is.Not.Null,"Socio economic objectives is null");
            Assert.That(model.SocioEconomicObjectives.Count, Is.EqualTo(_project.SocioEconomicObjectives.Count), "Socio economic objectives count is incorrect");
            Assert.That(model.SocioEconomicObjectives.All(o => _project.SocioEconomicObjectives.Any(q => q.SocioEconomicObjective.Id == o.Code.Id)), "Incorrect socio economic objectives");
            Assert.That(model.FieldsOfResearch,Is.Not.Null,"Fields of research is null");
            Assert.That(model.FieldsOfResearch.Count, Is.EqualTo(_project.FieldsOfResearch.Count), "Fields of research count is incorrect");
            Assert.That(model.FieldsOfResearch.All(o => _project.FieldsOfResearch.Any(q => q.FieldOfResearch.Id == o.Code.Id)), "Incorrect fields of research");
            Assert.That(model.PrincipalInvestigator,Is.Not.Null,"Principal Investigator is null");
            Assert.That(model.PrincipalInvestigator.UserId,Is.EqualTo(_principalInvestigator.UserId),"Principal investigator is incorrectly retrieved");

            var dmp = model.DataManagementPlan;
            Assert.That(dmp,Is.Not.Null,"Data management plan is null");
            Assert.That(dmp.BackupPolicy,Is.Not.Null,"Backup policy is null");
            Assert.That(dmp.UrdmsUsers,Is.Not.Null,"Urdms users is null");
            Assert.That(dmp.UrdmsUsers.Count,Is.EqualTo(_urdmsUsers.Count),"Urdms users count is incorrect");
            Assert.That(dmp.UrdmsUsers.All(o => _urdmsUsers.Count(q => q.UserId == o.UserId) == 1),"Total Urdms users is incorrect");
            Assert.That(dmp.DataDocumentation,Is.Not.Null,"Data documentation is null");
            Assert.That(dmp.DataRelationshipDetail,Is.Not.Null,"Data relationship detail is null");
            Assert.That(dmp.DataRetention,Is.Not.Null,"Data retention is null");
            Assert.That(dmp.DataSharing,Is.Not.Null,"Data sharing is null");
            Assert.That(dmp.DataStorage,Is.Not.Null,"Data storage is null");
            Assert.That(dmp.Ethic,Is.Not.Null,"Ethic is null");
            Assert.That(dmp.ExistingDataDetail,Is.Not.Null,"Existing data detail is null");
            Assert.That(dmp.Confidentiality,Is.Not.Null,"Intellectual property is null");
            Assert.That(dmp.NewDataDetail, Is.Not.Null, "New data detail is null");
            Assert.That(dmp.NonUrdmsUsers,Is.Not.Null,"Non Urdms users is null");
            Assert.That(dmp.NonUrdmsUsers.Count,Is.EqualTo(_nonUrdmsUsers.Count),"Non Urdms users count is incorrect");
            Assert.That(dmp.NonUrdmsUsers.All(o => _nonUrdmsUsers.Count(q => q.Id == o.Id && q.FullName == o.FullName) == 1),"Total non Urdms users is incorrect");
            
            Assert.That(dmp.ProjectDescription,Is.EqualTo(_project.Description),"Project description is incorrect");
            Assert.That(dmp.ProjectId,Is.EqualTo(_project.Id),"Project id is incorrect");
            Assert.That(dmp.ProjectTitle,Is.EqualTo(_project.Title),"Project title is incorrect");

        }

        [Test]
        public void Map_to_project_details_view_model()
        {
            var model = new ProjectDetailsViewModel().MapFrom(_project);

            Assert.That(model,Is.Not.Null,"View model is null");
            Assert.That(model.Id,Is.EqualTo(_project.Id),"View model has incorrect id");
            Assert.That(model.SocioEconomicObjectives, Is.Not.Null, "Socio economic objectives is null");
            Assert.That(model.SocioEconomicObjectives.Count, Is.EqualTo(_project.SocioEconomicObjectives.Count), "Socio economic objectives count is incorrect");
            Assert.That(model.SocioEconomicObjectives.All(o => _project.SocioEconomicObjectives.Any(q => q.SocioEconomicObjective.Id == o.Code.Id)), "Incorrect socio economic objectives");
            Assert.That(model.FieldsOfResearch, Is.Not.Null, "Fields of research is null");
            Assert.That(model.FieldsOfResearch.Count, Is.EqualTo(_project.FieldsOfResearch.Count), "Fields of research count is incorrect");
            Assert.That(model.FieldsOfResearch.All(o => _project.FieldsOfResearch.Any(q => q.FieldOfResearch.Id == o.Code.Id)), "Incorrect fields of research");
            Assert.That(model.Funders.IsNotEmpty(),"Funders are empty");
            Assert.That(model.Funders.Count(o => o.IsFunded),Is.EqualTo(1),"No project funder found");

            var projectFunder = model.Funders.First(o => o.IsFunded);
            Assert.That(projectFunder.Funder,Is.EqualTo(_funder.Funder),"Funder incorrect");
            Assert.That(projectFunder.GrantNumber,Is.EqualTo(_funder.GrantNumber),"Funder grant number incorrect");
        }

        [Test]
        public void Map_to_data_deposit_view_model()
        {
            var user1 = Builder<UrdmsUser>.CreateNew().With(o => o.CurtinId = "AA12345").Build();
            var user2 = Builder<UrdmsUser>.CreateNew().With(o => o.CurtinId = "BB23123").Build();
            var user3 = Builder<UrdmsUser>.CreateNew().With(o => o.CurtinId = null).Build();
            var user4 = Builder<UrdmsUser>.CreateNew().With(o => o.CurtinId = "DD33453").Build();
            var principalInvestigator = Builder<Party>.CreateNew().With(o => o.UserId = user4.CurtinId).Build();
            var dataDepositVm = Builder<DataDepositViewModel>.CreateNew()
                .With(x => x.Id = 1)
                .And(x => x.PrincipalInvestigator = principalInvestigator)
                .And(x => x.UrdmsUsers = Builder<UrdmsUserViewModel>.CreateListOfSize(2)
                    .TheFirst(1)
                    .With(u => u.UserId = user1.CurtinId)
                    .And(u => u.FullName = user1.FirstName)
                    .And(u => u.Id = 0)
                    .TheNext(1)
                    .With(u => u.UserId = user2.CurtinId)
                    .And(u => u.FullName = user2.FirstName)
                    .And(u => u.Id = 0)
                    .Build())
                .And(x => x.NonUrdmsUsers = Builder<NonUrdmsUserViewModel>.CreateListOfSize(1)
                    .TheFirst(1)
                    .With(u => u.FullName = user3.FirstName)
                    .And(u => u.Id = 0)
                    .Build())
                .Build();
            var projectVm = Builder<ProjectViewModel>.CreateNew()
                .With(x => x.DataDeposit = dataDepositVm)
                .And(x => x.StartDate = DateTime.Now.ToShortDateString())
                .And(x => x.EndDate = DateTime.Now.AddDays(30).ToShortDateString())
                .Build();
            var project = Builder<Project>.CreateNew()
              .With(x => x.Id = 1)
              .And(x => x.Parties.Add(new ProjectParty{Id = 1, Party = principalInvestigator, Relationship = ProjectRelationship.PrincipalInvestigator}))
              .And(x => x.SourceProjectType = SourceProjectType.DEPOSIT)
              .Build();
            projectVm.MapFrom(project, false, false);
            project.MapFrom(projectVm);

            Assert.That(project.Parties, Is.Not.Null);
            Assert.That(project.Parties.Count, Is.EqualTo(4));
        }

        [Test]
        public void Remove_all_items_when_the_source_is_empty_and_destination_is_not()
        {
            var source = new List<ClassificationBase>();
            var destination = Builder<ClassificationBase>.CreateListOfSize(2)
                .TheFirst(1).Do(d => d.Code = new FieldOfResearch{Id = "010101", Name = "description"})
                .TheLast(1).Do(d => d.Code = new FieldOfResearch { Id = "020405", Name = "blah" })
                .Build();

            destination.MapFrom<ClassificationBase, FieldOfResearch>(source);

            Assert.That(destination.Count, Is.EqualTo(0));
        }
    }
}
