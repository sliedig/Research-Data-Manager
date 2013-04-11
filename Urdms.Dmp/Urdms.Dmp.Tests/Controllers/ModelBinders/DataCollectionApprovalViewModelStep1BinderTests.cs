using System.Collections.Specialized;
using System.Web.Mvc;
using Autofac.Integration.Mvc;
using AutofacContrib.NSubstitute;
using FizzWare.NBuilder;
using NSubstitute;
using NUnit.Framework;
using Subtext.TestLibrary;
using Urdms.Dmp.Controllers.ModelBinders;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Models.DataCollectionModels;
using Urdms.Dmp.Tests.Helpers;

namespace Urdms.Dmp.Tests.Controllers.ModelBinders
{
    [TestFixture]
    class DataCollectionApprovalViewModelStep1BinderShould
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
        public void Create_viewmodel_with_projectid_set()
        {
            var dataCollection = Builder<DataCollection>.CreateNew().Build();
            _dataCollectionRepository.Get(dataCollection.Id).Returns(dataCollection);
            AddProjectIdToFormCollection(dataCollection);
            var bindingContext = GetBindingContext();
            var binder = new DataCollectionApprovalViewModelStep1Binder
                             {DataCollectionRepository = _dataCollectionRepository};
            var viewModel = binder.BindModel(_context, bindingContext) as DataCollectionViewModelStep1;

            Assert.That(viewModel, Is.Not.Null, "Viewmodel is null");
            Assert.That(viewModel.ProjectId, Is.EqualTo(dataCollection.ProjectId));
        }

        private ModelBindingContext GetBindingContext()
        {
            var valueProvider = new NameValueCollectionValueProvider(_form, null);
            var metadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(DataCollectionViewModelStep1));
            return new ModelBindingContext { ModelName = "", ModelMetadata = metadata, ValueProvider = valueProvider };
        }

        private void AddProjectIdToFormCollection(DataCollection dataCollection)
        {
            _form.Add("Id", dataCollection.ProjectId.ToString());
        }
       
    }
}
