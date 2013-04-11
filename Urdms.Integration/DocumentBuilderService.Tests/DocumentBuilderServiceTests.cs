using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using Urdms.DocumentBuilderService;
using FizzWare.NBuilder;
using NServiceBus.Testing;
using NSubstitute;
using NUnit.Framework;
using Urdms.DocumentBuilderService.Commands;
using Urdms.DocumentBuilderService.Database.Entities;
using Urdms.DocumentBuilderService.Database.Repositories;
using Urdms.DocumentBuilderService.Helpers;
using Urdms.DocumentBuilderService.Models.Enums;
//using WebSupergoo.ABCpdf8;
using File = System.IO.File;
using User = Urdms.DocumentBuilderService.Database.Entities.User;

namespace DocumentBuilderService.Tests
{
    [TestFixture]
    public class DocumentBuilderServiceShould
    {
        private const int ProjectId = 1;
        private const string SiteUri = @"http://urdms.yourdomain.edu.au/projects/researchproject1";
        private const string Tempdir = @"c:\Temp\";
        private IDataManagementPlanRepository _dataManagementPlanRepository;
        private ISharePointHelper _sharePointHelper;
        private DataManagementPlan _dmp;
        private IPdfHelper _pdfHelper;
        private IGeneratorHelper _generatorHelper;
        private IXmlHelper _xmlHelper;
        private IGenerator _generator;
        private IProjectRepository _projectRepository;
        private Project _depositProject;
        private Project _nonDepositProject;

        [TestFixtureSetUp]
        public void SetupFixture()
        {
            //Configure.With(AllAssemblies.Except("ABCpdf8-64.dll"));
            var assemblies = new[]
                         {
                             typeof(GenerateDmpCommand).Assembly,
                             typeof(GenerateDmpCommandHandler).Assembly,
                             Assembly.Load("NServiceBus"),
                             Assembly.Load("NServiceBus.Core")
                         };

            Test.Initialize(assemblies);
        }

        [SetUp]
        public void SetUp()
        {
            var files = Directory.GetFiles(Tempdir);
            Array.ForEach(files, File.Delete);
            _dmp = Builder<DataManagementPlan>.CreateNew()
                .With(o => o.BackupLocations = BackupLocations.MicrosoftSharePoint | BackupLocations.PersonalHardDrives | BackupLocations.InstitutionalBackup | BackupLocations.Other)
                .And(o => o.BackupPolicyResponsibilities = DataResponsibilities.Other | DataResponsibilities.PrincipalInvestigator)
                .And(o => o.IsSensitive = Pick<bool>.RandomItemFrom(new[] { false, true }))
                .And(o => o.CreationDate = DateTime.Now)
                .And(o => o.UrdmsUsers = Builder<UrdmsUser>.CreateListOfSize(5).Build())
                .And(o => o.RelationshipBetweenExistingAndNewData = EnumValuesHelper.RandomItem(DataRelationship.None))
                .And(o => o.DataRetentionLocations = DataRetentionLocations.External | DataRetentionLocations.Internal | DataRetentionLocations.Other)
                .And(o => o.DataRetentionPeriod = EnumValuesHelper.RandomItem(DataRetentionPeriod.Short))
                .And(o => o.DataRetentionResponsibilities = DataResponsibilities.Other | DataResponsibilities.PrincipalInvestigator)
                .And(o => o.DepositToRepository = Pick<bool>.RandomItemFrom(new[] { false, true }))
                .And(o => o.DataSharingAvailability = DataSharingAvailability.AfterASpecifiedEmbargoPeriod)
                .And(o => o.DataSharingAvailabilityDate = DateTime.Today.AddYears(10))
                .And(o => o.DataLicensingType = EnumValuesHelper.RandomItem<DataLicensingType>())
                .And(o => o.ShareAccess = EnumValuesHelper.RandomItem(ShareAccess.None))
				.And(o => o.InstitutionalStorageTypes = InstitutionalStorageTypes.ProjectStorageSpace | InstitutionalStorageTypes.PersonalNetworkDrive | InstitutionalStorageTypes.PersonalNetworkDrive | InstitutionalStorageTypes.Other)
                .And(o => o.ExternalStorageTypes = ExternalStorageTypes.DataFabric | ExternalStorageTypes.IvecPetabyte | ExternalStorageTypes.Other)
                .And(o => o.MaxDataSize = EnumValuesHelper.RandomItem(MaxDataSize.None))
                .And(o => o.PersonalStorageTypes = PersonalStorageTypes.RemovableMedia | PersonalStorageTypes.InternalHardDrives | PersonalStorageTypes.Other)
                .And(o => o.VersionControl = EnumValuesHelper.RandomItem(VersionControl.None))
                .And(o => o.EthicRequiresClearance = Pick<bool>.RandomItemFrom(new[] { false, true }))
                .And(o => o.ExistingDataAccessTypes = ExistingDataAccessTypes.DataIsFreelyAvailable | ExistingDataAccessTypes.ObtainApprovalFromOwner | ExistingDataAccessTypes.Other | ExistingDataAccessTypes.Purchase)
                .And(o => o.UseExistingData = Pick<bool>.RandomItemFrom(new[] { false, true }))
                .And(o => o.DataOwners = DataOwners.MyInstitution | DataOwners.Researcher | DataOwners.Other)
                .And(o => o.DataActionFrequency = EnumValuesHelper.RandomItem(DataActionFrequency.None))
                .And(o => o.NonUrdmsUsers = Builder<User>.CreateListOfSize(4).Build())
                .And(o => o.PricipalInvestigator = Pick<string>.RandomItemFrom(new[] { "John Doe", "Joe Bloggs", "Jane Doe", "John Smith" }))
                .And(o => o.ProjectTitle = Pick<string>.RandomItemFrom(new[] { "Feeding Habits of Polar Bears", "The Ecosystem in the Sahara Desert" }))
                .And(o => o.DateModified = DateTime.Now)
                .Build();

            _dataManagementPlanRepository = Substitute.For<IDataManagementPlanRepository>();
            _dataManagementPlanRepository.GetDataManagementPlanByProjectId(Arg.Is(ProjectId)).Returns(_dmp);
            _sharePointHelper = Substitute.For<ISharePointHelper>();
            _pdfHelper = Substitute.For<IPdfHelper>();
            _generatorHelper = Substitute.For<IGeneratorHelper>();
            _xmlHelper = Substitute.For<IXmlHelper>();
            _generator = new DmpGenerator(_sharePointHelper, _dataManagementPlanRepository, _pdfHelper, _generatorHelper, _xmlHelper);

            _projectRepository = Substitute.For<IProjectRepository>();
            var projects = Builder<Project>.CreateListOfSize(2)
                .TheFirst(1)
                .With(o => o.SourceProjectType = EnumValuesHelper.RandomItem(SourceProjectType.None, SourceProjectType.DEPOSIT))
                .TheNext(1)
                .With(o => o.SourceProjectType = SourceProjectType.DEPOSIT)
                .Build()
                .ToList();

            _nonDepositProject = projects[0];
            _depositProject = projects[1];
            projects.ForEach(o => _projectRepository.Get(Arg.Is(o.Id)).Returns(o));

            // Initialise PDF library
            // XSettings.InstallRedistributionLicense(ConfigurationManager.AppSettings["ABCPDFLicenseKey"]);
        }

        [TearDown]
        public void TearDown()
        {
            _dataManagementPlanRepository = null;
            _dmp = null;
            _sharePointHelper = null;
            _pdfHelper = null;
            _generatorHelper = null;
            _generator = null;
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            var files = Directory.GetFiles(Tempdir);
            Array.ForEach(files, File.Delete);
        }


        [Test]
        public void Generate_a_pdf_of_the_dmp_given_a_regenerate_dmp_command()
        {
            var handler = new GenerateDmpCommandHandler(_generator, _projectRepository);
            Test.Handler(handler)
                .OnMessage<GenerateDmpCommand>(m =>
                {
                    m.ProjectId = ProjectId;
                    m.SiteUrl = SiteUri;
                });
        }


        [Test]
        public void Generate_and_upload_pdf_and_xml_documents_of_the_share_point_document_library_given_a_site_collection_provisioned_message_for_non_deposit_projects()
        {
            var handler = new GenerateDmpCommandHandler(_generator, _projectRepository);
            Test.Handler(handler)
                .OnMessage<GenerateDmpCommand>(m =>
                {
                    m.ProjectId = _nonDepositProject.Id;
                    m.SiteUrl = SiteUri;
                });

            _generatorHelper.Received().CreateFilePath(Arg.Any<string>());
            _dataManagementPlanRepository.Received().GetDataManagementPlanByProjectId(Arg.Is(_nonDepositProject.Id));
            //_pdfHelper.Received().GeneratePdf(Arg.Any<string>(), Arg.Any<DataManagementPlan>());
            _xmlHelper.Received().Save(Arg.Any<string>(), Arg.Any<DataManagementPlan>());

        }

        [Test]
        public void Not_generate_and_upload_pdf_and_xml_documents_of_the_share_point_document_library_given_a_site_collection_provisioned_message_for_deposit_projects()
        {
            var handler = new GenerateDmpCommandHandler(_generator, _projectRepository);
            Test.Handler(handler)
                .OnMessage<GenerateDmpCommand>(m =>
                {
                    m.ProjectId = _depositProject.Id;
                    m.SiteUrl = SiteUri;
                });

            _generatorHelper.DidNotReceive().CreateFilePath(Arg.Any<string>());
            _dataManagementPlanRepository.DidNotReceive().GetDataManagementPlanByProjectId(Arg.Is(_nonDepositProject.Id));
            //_pdfHelper.DidNotReceive().GeneratePdf(Arg.Any<string>(), Arg.Any<DataManagementPlan>());
            _xmlHelper.DidNotReceive().Save(Arg.Any<string>(), Arg.Any<DataManagementPlan>());
        }

        [Test]
        public void Generate_xml_and_handle_null_string_fields()
        {
            var dmp = new DataManagementPlan();
            var xmlHelper = new XmlHelper();
            var path = string.Format(@"{0}DmpDocument-{1}-{2:yyyyMMddhhmmss}.xml", Tempdir, dmp.Id, DateTime.Now);
            xmlHelper.Save(path, dmp);
            Assert.That(File.Exists(path), Is.True);
            File.Delete(path);
        }

        [Test]
        [ExpectedException]
        public void Throw_an_exception_when_no_project_is_retrieved_from_the_repository()
        {
            var handler = new GenerateDmpCommandHandler(_generator, _projectRepository);
            Test.Handler(handler)
                .OnMessage<GenerateDmpCommand>(m =>
                {
                    m.ProjectId = int.MaxValue;
                    m.SiteUrl = SiteUri;
                });

        }

        [Test]
		[Ignore("Restore once you have connected ABC PDF library")]
        public void Generate_pdf_for_a_dmp()
        {
			//var pdfHelper = new PdfHelper();
			//var path = string.Format(@"{0}DmpDocument-{1}-{2:yyyyMMddhhmmss}.pdf", Tempdir, _dmp.Id, DateTime.Now);
			//_dmp.NonUrdmsUsers.Clear();
			//var doc = pdfHelper.GeneratePdf("DmpPdfTemplate", _dmp);
			//Assert.That(doc, Is.Not.Null);
			//doc.Save(path);
			//Assert.That(File.Exists(path), Is.True);
			//Assert.That(_dmp.DateModified, Is.Not.Null);
			//File.Delete(path);
        }

        [Test]
        public void Generate_xml_for_a_dmp()
        {
            var xmlHelper = new XmlHelper();
            var path = string.Format(@"{0}DmpDocument-{1}-{2:yyyyMMddhhmmss}.xml", Tempdir, _dmp.Id, DateTime.Now);
            xmlHelper.Save(path, _dmp);
            Assert.That(File.Exists(path), Is.True);
            File.Delete(path);
        }

    }

    internal static class EnumValuesHelper
    {
        public static IList<TEnum> GetValues<TEnum>(params TEnum[] exclusions) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new InvalidOperationException();
            }
            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
            if (exclusions == null)
            {
                exclusions = new TEnum[] { };
            }
            var list = values.Except(exclusions).ToList();
            return list;
        }

        public static TEnum RandomItem<TEnum>(params TEnum[] exclusions) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new InvalidOperationException();
            }
            var list = GetValues(exclusions);
            return Pick<TEnum>.RandomItemFrom(list);
        }
    }
}
