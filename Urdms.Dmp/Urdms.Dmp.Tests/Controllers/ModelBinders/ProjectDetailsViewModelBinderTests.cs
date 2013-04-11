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
    class ProjectDetailsViewModelBinderShould
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

        [TearDown]
        public void TearDown()
        {
            _httpSimulator.Dispose();
        }

        [Test]
        public void Create_viewmodel_with_seocodes_and_forcodes()
        {
            var project = GetProjectDetailsForCodeTests();
            AddProjectToFormCollection(project);
            var bindingContext = GetBindingContext();
            var binder = new ProjectDetailsViewModelBinder();
            var viewModel = binder.BindModel(_context, bindingContext) as ProjectDetailsViewModel;
            
            Assert.That(viewModel,Is.Not.Null,"Viewmodel is null");
            Assert.That(project.FieldsOfResearch.Count,Is.EqualTo(viewModel.FieldsOfResearch.Count),"ForCodes is incorrect");
            Assert.That(project.SocioEconomicObjectives.Count,Is.EqualTo(viewModel.SocioEconomicObjectives.Count),"SeoCodes is incorrect");
            Assert.That(project.FieldsOfResearch.All(o => viewModel.FieldsOfResearch.SingleOrDefault(q => q.Id == o.Id && q.Code.Id == o.Code.Id && q.Code.Name == o.Code.Name) != null),
                Is.True,"Wrong binding of the form collection");
        }

        private ModelBindingContext GetBindingContext()
        {
            var valueProvider = new NameValueCollectionValueProvider(_form, null);
            var metadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(ProjectDetailsViewModel));
            return new ModelBindingContext { ModelName = "", ModelMetadata = metadata, ValueProvider = valueProvider };
        }

        private void AddProjectToFormCollection(Project project)
        {

            foreach (var forCode in project.FieldsOfResearch)
            {
                var value = string.Format("{0}:{1}:{2}", forCode.Id, forCode.FieldOfResearch.Id, forCode.FieldOfResearch.Name);
                _form.Add("ForCodeRows", value);
            }

            foreach (var seoCode in project.SocioEconomicObjectives)
            {
                var value = string.Format("{0}:{1}:{2}", seoCode.Id, seoCode.SocioEconomicObjective.Id, seoCode.SocioEconomicObjective.Name);
                _form.Add("SeoCodeRows", value);
            }
        }



        private static Project GetProjectDetailsForCodeTests()
        {
            var project = Builder<Project>.CreateNew().Build();
            project.FieldsOfResearch.AddRange(GetForCodes());
            project.SocioEconomicObjectives.AddRange(GetSeoCodes());
            return project;

        }

        private static IEnumerable<ProjectFieldOfResearch> GetForCodes(int codeCount = 5)
        {
            var forCodes = Builder<FieldOfResearch>.CreateListOfSize(codeCount).Build();
            var projectForCodes = Builder<ProjectFieldOfResearch>.CreateListOfSize(codeCount).Build();
            for (int i = 0; i < codeCount; i++)
            {
                var projectForCode = projectForCodes[i];
                var forCode = forCodes[i];
                projectForCode.FieldOfResearch = forCode;
            }
            return projectForCodes;
        }

        private static IEnumerable<ProjectSocioEconomicObjective> GetSeoCodes(int codeCount = 3)
        {
            var seoCodes = Builder<SocioEconomicObjective>.CreateListOfSize(codeCount).Build();
            var projectSeoCodes = Builder<ProjectSocioEconomicObjective>.CreateListOfSize(codeCount).Build();
            for (int i = 0; i < codeCount; i++)
            {
                var projectSeoCode = projectSeoCodes[i];
                var seoCode = seoCodes[i];
                projectSeoCode.SocioEconomicObjective = seoCode;
            }
            return projectSeoCodes;
        }
    }
}
