using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Curtin.Framework.Common.UserService;
using Elmah.Contrib.Mvc;
using Urdms.Dmp.Controllers.Filters;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Mappers;
using Urdms.Dmp.Models;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Controllers
{
    [ElmahHandleError]
    public class ProjectController : ProjectBaseController
    {
        public ProjectController(ICurtinUserService lookupService, IProjectRepository projectRepository,
            IFieldOfResearchRepository fieldOfResearchRepository, ISocioEconomicObjectiveRepository socioEconomicObjectiveRepository)
            : base(lookupService, projectRepository, fieldOfResearchRepository, socioEconomicObjectiveRepository){}


        public ActionResult NewProjects()
        {
            return View();
        }


        public ActionResult New()
        {
            return RedirectToAction("Project");
        }


        public ActionResult Index()
        {
            return View(new List<ProjectListViewModel>().MapFrom(ProjectRepository.GetByPrincipalInvestigator(CurrentUser.CurtinId)));
        }

        protected override ActionResult ReturnOnPostForProject(Project project)
        {
            project.SourceProjectType = SourceProjectType.DMP;
            ProjectRepository.Save(project);
            return ReturnOnSaveForProject(project);
        }
        
        protected override ActionResult ReturnOnGetForProject(ProjectDetailsViewModel vm)
        {
            return View("Project", vm);
        }

        protected override ActionResult ReturnOnSaveForProject(Project project)
        {
            if (project.DataManagementPlan == null)
            {
                return RedirectToAction("CopyDmp", new {id = project.Id});
            }
            return RedirectToAction("Edit", "Dmp", new { project.DataManagementPlan.Id });
        }

        protected override bool VerifyProjectRequest(Project entity, out ActionResult result)
        {
            if (entity == null)
            {
                result = View("ProjectNotFound");
                return false;
            }
            if (!this.CurrentUser.IsPrincipalInvestigatorFor(entity))
            {
                result = View("NoProjectAccessRight");
                return false;
            }
            if (entity.SourceProjectType == SourceProjectType.DEPOSIT)
            {
                result = View("IncorrectProjectType");
                return false;
            }
            result = new EmptyResult();
            return true;
        } 

        [ProvideDataManagementPlanSteps]
        public ActionResult CopyDmp(int id)
        {
            var project = ProjectRepository.Get(id);
            if (project == null)
            {
                return View("ProjectNotFound");
            }
            if (!this.CurrentUser.IsPrincipalInvestigatorFor(project))
            {
                return View("NoProjectAccessRight");
            }
            if (project.DataManagementPlan != null && project.ProvisioningStatus != ProvisioningStatus.NotStarted)
            {
                return RedirectToAction("New", "Dmp", new { id });
            }

            var availableProjects = GetAvailableProjects(id);
            if (availableProjects.Count != 0)
            {
                var copyViewModel = new CopyDataManagementPlanProjectViewModel
                                        {
                                            DestinationProjectId = id,
                                            AvailableProjects = availableProjects
                                        };
                return View(copyViewModel);
            }
            return RedirectToAction("New", "Dmp", new { id });
        }
        
        [HttpPost]
        [ProvideDataManagementPlanSteps]
        public ActionResult CopyDmp(CopyDataManagementPlanProjectViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableProjects = GetAvailableProjects(model.DestinationProjectId);
                return View("CopyDmp", model);
            }

            var formParams = ControllerContext.HttpContext.Request.Form;
            var dmpList = formParams["ProjectList"];
            var proceedBtn = formParams["saveAndProceed"];
            if (proceedBtn != null)
            {
                if (model.CopyFromExistingDmp != null && (bool) model.CopyFromExistingDmp)
                {
                    int sourceProjectId;
                    if (int.TryParse(dmpList, out sourceProjectId))
                    {
                        var sourceProject = ProjectRepository.Get(sourceProjectId);
                        var destProject = ProjectRepository.Get(model.DestinationProjectId);
                        destProject.DataManagementPlan = sourceProject.DataManagementPlan.Clone();
                        destProject.DataManagementPlan.Id = 0;
                        ProjectRepository.Save(destProject);
                        return RedirectToAction("Edit", "Dmp",
                                                new {id = destProject.DataManagementPlan.Id, step = 1});
                    }
                }
                return RedirectToAction("New", "Dmp", new { id = model.DestinationProjectId, step = 1 });
            }
            return RedirectToAction("Project", "Project", new { id = model.DestinationProjectId });
        }

        private IList<ProjectListViewModel> GetAvailableProjects(int id)
        {
            var user = CurrentUser;
            var projects = ProjectRepository.GetByPrincipalInvestigator(user.CurtinId);
            var availableProjects = new List<ProjectListViewModel>().MapFrom(projects);

            return availableProjects
                    .Where(p => p.ProjectId != id && p.DmpId > 0)
                    .ToList();
        }
    }
}
