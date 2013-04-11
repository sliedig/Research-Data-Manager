using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using Autofac.Integration.Mvc;
using AutofacContrib.NSubstitute;
using FizzWare.NBuilder;
using NUnit.Framework;
using Subtext.TestLibrary;
using Urdms.Dmp.Controllers.ModelBinders;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Models;
using Urdms.Dmp.Tests.Helpers;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Tests.Controllers.ModelBinders
{
    [TestFixture]
    class DataManagementPlanViewModelBinderShould
    {
        private AutoSubstitute _autoSubstitute;
        private ControllerContext _context;
        private HttpSimulator _httpSimulator;
        private NameValueCollection _form;

        [SetUp]
        public void Setup()
        {
            _autoSubstitute = AutoSubstituteContainer.Create();
            _context = _autoSubstitute.Resolve<ControllerContext>();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(_autoSubstitute.Container));
            _httpSimulator = new HttpSimulator().SimulateRequest();
            _form = _context.HttpContext.Request.Form;
        }

        [Test]
        public void Persist_users()
        {
            var project = GetProject();
            var parties = project.Parties;

            AddProjectToFormCollection(project);
            var bindingContext = GetBindingContext();
            var binder = new DataManagementPlanViewModelBinder();
            var viewModel = binder.BindModel(_context, bindingContext) as DataManagementPlanViewModel;

            Assert.That(viewModel, Is.Not.Null, "Viewmodel is null");
			Assert.That(viewModel.UrdmsUsers.Count, Is.EqualTo(parties.Count(o => o.Party.UserId != null)), "URDMS Users count is incorrect");
            Assert.That(viewModel.NonUrdmsUsers.Count, Is.EqualTo(parties.Count(o => o.Party.UserId == null)), "Non URDMS Users count is incorrect");

			Assert.That(viewModel.UrdmsUsers.All(o => parties.SingleOrDefault(q => q.Party.FullName == o.FullName && q.Party.UserId != null) != null), Is.True, "Invalid URDMS users");
			Assert.That(viewModel.NonUrdmsUsers.All(o => parties.SingleOrDefault(q => q.Party.FullName == o.FullName && q.Party.UserId == null) != null), Is.True, "Invalid URDMS users");

        }

        [Test]
        public void Delete_urdms_users()
        {
            var project = GetProject();
            var parties = project.Parties;

            var deletedParties = parties.Where(o => o.Party.UserId != null && o.Id == 0).Take(1)
                .Union(parties.Where(o => o.Party.UserId != null && o.Id != 0).Take(1))
                .ToList();

            var currentParties = parties.Except(deletedParties).ToList();

            AddProjectToFormCollection(project, deletedParties.Select(o => o.Party).ToList());
			_form.Add("DeleteUrdmsUser", "Remove users");
            
            var bindingContext = GetBindingContext();
            var binder = new DataManagementPlanViewModelBinder();
            var viewModel = binder.BindModel(_context, bindingContext) as DataManagementPlanViewModel;



            Assert.That(viewModel, Is.Not.Null, "Viewmodel is null");
            Assert.That(viewModel.UrdmsUsers.Count, Is.EqualTo(currentParties.Count(o => o.Party.UserId != null)), "Curtin Users count is incorrect");
            Assert.That(viewModel.NonUrdmsUsers.Count, Is.EqualTo(currentParties.Count(o => o.Party.UserId == null)), "Non Curtin Users count is incorrect");

            Assert.That(viewModel.UrdmsUsers.All(o => currentParties.SingleOrDefault(q => q.Party.FullName == o.FullName && q.Party.UserId != null) != null), Is.True, "Invalid curtin users");
            Assert.That(viewModel.NonUrdmsUsers.All(o => currentParties.SingleOrDefault(q => q.Party.FullName == o.FullName && q.Party.UserId == null) != null), Is.True, "Invalid curtin users");

        }

        [Test]
        public void Delete_non_urdms_users()
        {
            var project = GetProject();
            var parties = project.Parties;

            var deletedParties = parties.Where(o => o.Party.UserId == null && o.Id == 0).Take(1)
                .Union(parties.Where(o => o.Party.UserId == null && o.Id != 0).Take(1))
                .ToList();

            var currentParties = parties.Except(deletedParties).ToList();

            AddProjectToFormCollection(project, deletedParties.Select(o => o.Party).ToList());
            _form.Add("DeleteNonUrdmsUser", "Remove users");

            var bindingContext = GetBindingContext();
            var binder = new DataManagementPlanViewModelBinder();
            var viewModel = binder.BindModel(_context, bindingContext) as DataManagementPlanViewModel;

            Assert.That(viewModel, Is.Not.Null, "Viewmodel is null");
            Assert.That(viewModel.UrdmsUsers.Count, Is.EqualTo(currentParties.Count(o => o.Party.UserId != null)), "Curtin Users count is incorrect");
            Assert.That(viewModel.NonUrdmsUsers.Count, Is.EqualTo(currentParties.Count(o => o.Party.UserId == null)), "Non Curtin Users count is incorrect");

            Assert.That(viewModel.UrdmsUsers.All(o => currentParties.SingleOrDefault(q => q.Party.FullName == o.FullName && q.Party.UserId != null) != null), Is.True, "Invalid curtin users");
            Assert.That(viewModel.NonUrdmsUsers.All(o => currentParties.SingleOrDefault(q => q.Party.FullName == o.FullName && q.Party.UserId == null) != null), Is.True, "Invalid curtin users");

        }

        [TearDown]
        public void TearDown()
        {
            _httpSimulator.Dispose();
        }

        private ModelBindingContext GetBindingContext()
        {
            var valueProvider = new NameValueCollectionValueProvider(_form, null);
            var metadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(DataManagementPlanViewModel));
            return new ModelBindingContext { ModelName = "", ModelMetadata = metadata, ValueProvider = valueProvider };
        }


        private void AddProjectToFormCollection(Project project, IEnumerable<Party> deletedParties = null)
        {
            const string urdmsUsersRow = "urdms.users.row";
            const string nonUrdmsUserKey = "nonurdms.users.row";
            const string urdmsUserFormat = "{0},{1},{2},{3},{4}";
            const string nonUrdmsUserFormat = "{0},{1},{2},{3}";
			const string relationshipUrdmsUserKeyFormat = "UrdmsUserRelationship{0}";
			const string relationshipNonUrdmsUserKeyFormat = "NonUrdmsUserRelationship{0}";
            const string removeUrdmsUserKeyFormat = "RemoveUrdmsUser{0}";
			const string removeNonUrdmsUserKeyFormat = "RemoveNonUrdmsUser{0}";

            foreach (var projectParty in project.Parties)
            {
                var party = projectParty.Party;
                string userKey;
                string userValue;
                var isUrdmsUser = party.UserId != null;
                if (!isUrdmsUser)
                {
                    userKey = nonUrdmsUserKey;
                    userValue = string.Format(nonUrdmsUserFormat, projectParty.Id, party.FullName.GetHashCode(), party.FullName, projectParty.Role);
                }
                else
                {
                    userKey = urdmsUsersRow;
                    userValue = string.Format(urdmsUserFormat, projectParty.Id, party.FullName.GetHashCode(), party.FullName, party.UserId, projectParty.Role);
                }

                _form.Add(userKey, userValue);
                var relationshipKeyFormat = isUrdmsUser ? relationshipUrdmsUserKeyFormat : relationshipNonUrdmsUserKeyFormat;
                var removeUserKeyFormat = isUrdmsUser ? removeUrdmsUserKeyFormat : removeNonUrdmsUserKeyFormat;
                var relationshipKey = string.Format(relationshipKeyFormat, projectParty.Id == 0 ? party.FullName.GetHashCode() : party.Id);
                var relationshipValue = ((int)projectParty.Relationship).ToString();
                _form.Add(relationshipKey, relationshipValue);
                var removeUserKey = string.Format(removeUserKeyFormat, projectParty.Id == 0 ? party.FullName.GetHashCode() : projectParty.Id);
                _form.Add(removeUserKey, "false");
                if (deletedParties != null && deletedParties.Any(o => o.Id == party.Id))
                {
                    _form.Add(removeUserKey, "true");
                }

            }

        }




        private static Project GetProject()
        {

            var parties = Builder<Party>.CreateListOfSize(9)
                .TheFirst(4)
                .With(o => o.UserId = null)
                .And(o => o.FirstName = null)
                .And(o => o.LastName = null)
                .And(o => o.Organisation = null)
                .TheNext(5)
                .With(o => o.Organisation = "Your University")
                .Build();

            var project = Builder<Project>.CreateNew()
                .With(o => o.DataManagementPlan = Builder<DataManagementPlan>.CreateNew().Build())
                .Do(o => o.Parties.AddRange(parties.Select(q => new ProjectParty { Party = q, Project = o })))
                .Build();

            var newParties = project.Parties.Where(o => o.Party.UserId == null).Reverse().Take(2)
                .Union(project.Parties.Where(o => o.Party.UserId != null).Reverse().Take(2))
                .ToList();

            newParties.ForEach(o => o.Id = 0);

            var existingParties = project.Parties.Except(newParties).ToList();
            existingParties.ForEach(o => o.Id = existingParties.Max(q => q.Id) + 1);


            return project;
        }

        
    }
}
