using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Urdms.ProvisioningService.Commands;
using Elmah.Contrib.Mvc;
using NServiceBus;
using Urdms.Dmp.Controllers.Helpers;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Models;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Controllers
{
    [ElmahHandleError]
	[Authorize(Roles = "QA-Approver")]
    public class AdminController : Controller
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ICsvHelper _csvHelper;
        private readonly IAppSettingsService _appSettingsService;
        private readonly IDataCollectionRepository _dataCollectionRepository;
        private readonly IBus _bus;


        public AdminController(IProjectRepository projectRepository, IDataCollectionRepository dataCollectionRepository, ICsvHelper csvHelper, IAppSettingsService appSettingsService, IBus bus)
        {
            _csvHelper = csvHelper;
            _appSettingsService = appSettingsService;
            _bus = bus;
            _projectRepository = projectRepository;
            _dataCollectionRepository = dataCollectionRepository;
        }


        public ActionResult Index()
        {
            var projects = _projectRepository.GetAll();
            var projectModels = projects.Where(p => p.DataManagementPlan != null).Select(p =>
                    new DmpListViewModel
                        {
                            Id = p.DataManagementPlan.Id,
                            Title = p.Title
                        }).ToList();
            var dataCollections = _dataCollectionRepository.GetAll();
            var dataCollectionModels =
                dataCollections.Select(d => new CollectionListViewModel { Id = d.Id, Title = d.Title }).ToList();
            var model = new CsvDumpViewModel { Projects = projectModels, DataCollections = dataCollectionModels };
            return View(model);
        }

        [HttpPost]
        [AcceptMethodByParameter(Name = "DataManagementPlanCsv")]
        [ActionName("Index")]
        public ActionResult DataManagementPlanToCsv(CsvDumpViewModel model)
        {
            var formParams = ControllerContext.HttpContext.Request.Form;
            var dmpList = formParams["ProjectList"];
            int sourceProjectId;
            if (int.TryParse(dmpList, out sourceProjectId))
            {
                var project = _projectRepository.GetByDataManagementPlanId(sourceProjectId);
                if (project != null)
                {
                    var dataManagementPlanTable = _csvHelper.DataManagementPlanToDataTable(project.DataManagementPlan, project.Parties);
                    var reponse = this.ControllerContext.RequestContext.HttpContext.Response;
                    var fileName = Regex.Replace(project.Title, @"\W+", "_").Trim('_');
                    reponse.AddHeader("Content-Disposition", "attachment;filename=" + fileName + "_DataManagementPlan.csv");
                    return File(Encoding.UTF8.GetBytes(_csvHelper.ExportToCsv(dataManagementPlanTable, _appSettingsService.CsvSeparator)), "text/csv");
                }
                return View("DmpNotFound");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [AcceptMethodByParameter(Name = "AllDataManagementPlansCsv")]
        [ActionName("Index")]
        public ActionResult AllDataManagementPlansToCsv(CsvDumpViewModel model)
        {
            var projects = _projectRepository.GetAll().Where(p => p.DataManagementPlan != null).ToList();
            if (projects.Count != 0)
            {
                var dataManagementPlanTable = _csvHelper.DataManagementPlansToDataTable(projects);
                var reponse = this.ControllerContext.RequestContext.HttpContext.Response;
                reponse.AddHeader("Content-Disposition", "attachment;filename=DataManagementPlans_" + DateTime.Now.ToShortDateString() + ".csv");
                return File(Encoding.UTF8.GetBytes(_csvHelper.ExportToCsv(dataManagementPlanTable, _appSettingsService.CsvSeparator)), "text/csv");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [AcceptMethodByParameter(Name = "DataCollectionCsv")]
        [ActionName("Index")]
        public ActionResult DataCollectionToCsv(CsvDumpViewModel model)
        {
            var formParams = ControllerContext.HttpContext.Request.Form;
            var collectionList = formParams["DataCollectionList"];
            int collectionId;
            if (int.TryParse(collectionList, out collectionId))
            {
                var dataCollection = _dataCollectionRepository.Get(collectionId);
                if (dataCollection != null)
                {
                    var dataCollectionTable = _csvHelper.DataCollectionToDataTable(dataCollection);
                    var reponse = this.ControllerContext.RequestContext.HttpContext.Response;
                    var fileName = Regex.Replace(dataCollection.Title, @"\W+", "_").Trim('_');
                    reponse.AddHeader("Content-Disposition", "attachment;filename=" + fileName + "_DataCollection.csv");
                    return File(Encoding.UTF8.GetBytes(_csvHelper.ExportToCsv(dataCollectionTable, _appSettingsService.CsvSeparator)), "text/csv");
                }
                return View("DataCollectionNotFound");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [AcceptMethodByParameter(Name = "AllDataCollectionsCsv")]
        [ActionName("Index")]
        public ActionResult AllDataCollectionsToCsv(CsvDumpViewModel model)
        {
            var dataCollections = _dataCollectionRepository.GetAll();
            if (dataCollections.Count != 0)
            {
                var dataCollectionTable = _csvHelper.DataCollectionsToDataTable(dataCollections);
                var reponse = this.ControllerContext.RequestContext.HttpContext.Response;
                reponse.AddHeader("Content-Disposition", "attachment;filename=DataCollections_" + DateTime.Now.ToShortDateString() + ".csv");
                return File(Encoding.UTF8.GetBytes(_csvHelper.ExportToCsv(dataCollectionTable, _appSettingsService.CsvSeparator)), "text/csv");
            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Provisioning()
        {
            var projects = _projectRepository.GetAllWhichFailedProvisioning();

            var vm = new ProvisioningViewModel { Projects = projects };

            return View(vm);
        }

        [HttpPost]
        public ActionResult Provisioning(ProvisioningViewModel vm)
        {
            if (!vm.ProvisioningInformation.Any(p => p.Select && string.IsNullOrEmpty(p.SiteUrl)))
            {
                var commands = vm.ProvisioningInformation.Where(p => p.Select).Select(p => new ForceProvisioningCompletionCommand { ProjectId = p.Id, SiteUrl = p.SiteUrl }).ToList();

                commands.ForEach(c => _bus.Send<ForceProvisioningCompletionCommand>
                    (m =>
                        {
                            m.ProjectId = c.ProjectId;
                            m.SiteUrl = c.SiteUrl;
                        }
                     ));


                ViewBag.Message = "Provisioning requests updated; refresh in a few minutes to see the result.";
            }
            else
            {
                ViewBag.Message = "Invalid request(s) selected; ensure all selected requests have a site url.";
            }

            vm.Projects = _projectRepository.GetAllWhichFailedProvisioning();
            return View(vm);
        }
    }
}
