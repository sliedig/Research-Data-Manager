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
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Models.DataCollectionModels;
using Urdms.Dmp.Tests.Helpers;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Tests.Controllers.ModelBinders
{
    [TestFixture]
    class DataCollectionViewModelStep2BinderShould
    {
        private IDataCollectionRepository _dataCollectionRepository;
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
            _dataCollectionRepository = _autoSubstitute.Resolve<IDataCollectionRepository>();
        }

        [TearDown]
        public void TearDown()
        {
            _httpSimulator.Dispose();
        }

        [Test]
        public void Create_viewmodel_with_seocodes_and_forcodes()
        {
            var dataCollection = GetDataCollection();
            AddDataCollectionToFormCollection(dataCollection);
            var bindingContext = GetBindingContext();
            var binder = new DataCollectionViewModelStep2Binder {DataCollectionRepository = _dataCollectionRepository};
            var viewModel = binder.BindModel(_context, bindingContext) as DataCollectionViewModelStep2;

            Assert.That(viewModel, Is.Not.Null, "Viewmodel is null");
            Assert.That(dataCollection.FieldsOfResearch.Count, Is.EqualTo(viewModel.FieldsOfResearch.Count), "ForCodes is incorrect");
            Assert.That(dataCollection.SocioEconomicObjectives.Count, Is.EqualTo(viewModel.SocioEconomicObjectives.Count), "SeoCodes is incorrect");
            Assert.That(dataCollection.FieldsOfResearch.All(o => viewModel.FieldsOfResearch.SingleOrDefault(q => q.Id == o.Id && q.Code.Id == o.Code.Id && q.Code.Name == o.Code.Name) != null),
                Is.True, "Wrong binding of the form collection");
        }

        private ModelBindingContext GetBindingContext()
        {
            var valueProvider = new NameValueCollectionValueProvider(_form, null);
            var metadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(DataCollectionViewModelStep2));
            return new ModelBindingContext { ModelName = "", ModelMetadata = metadata, ValueProvider = valueProvider };
        }

        private void AddDataCollectionToFormCollection(DataCollection dataCollection)
        {

            foreach (var forCode in dataCollection.FieldsOfResearch)
            {
                var value = string.Format("{0}:{1}:{2}", forCode.Id, forCode.FieldOfResearch.Id, forCode.FieldOfResearch.Name);
                _form.Add("ForCodeRows", value);
            }

            foreach (var seoCode in dataCollection.SocioEconomicObjectives)
            {
                var value = string.Format("{0}:{1}:{2}", seoCode.Id, seoCode.SocioEconomicObjective.Id, seoCode.SocioEconomicObjective.Name);
                _form.Add("SeoCodeRows", value);
            }
        }

        private static DataCollection GetDataCollection()
        {
            var dataCollection = Builder<DataCollection>.CreateNew().Build();
            dataCollection.FieldsOfResearch.AddRange(GetForCodes());
            dataCollection.SocioEconomicObjectives.AddRange(GetSeoCodes());
            return dataCollection;
        }

        private static IEnumerable<DataCollectionFieldOfResearch> GetForCodes(int codeCount = 5)
        {
            var forCodes = Builder<FieldOfResearch>.CreateListOfSize(codeCount).Build();
            var projectForCodes = Builder<DataCollectionFieldOfResearch>.CreateListOfSize(codeCount).Build();
            for (int i = 0; i < codeCount; i++)
            {
                var projectForCode = projectForCodes[i];
                var forCode = forCodes[i];
                projectForCode.FieldOfResearch = forCode;
            }
            return projectForCodes;
        }

        private static IEnumerable<DataCollectionSocioEconomicObjective> GetSeoCodes(int codeCount = 3)
        {
            var seoCodes = Builder<SocioEconomicObjective>.CreateListOfSize(codeCount).Build();
            var projectSeoCodes = Builder<DataCollectionSocioEconomicObjective>.CreateListOfSize(codeCount).Build();
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
