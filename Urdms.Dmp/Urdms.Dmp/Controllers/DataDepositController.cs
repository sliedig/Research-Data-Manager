using System;
using System.Linq;
using System.Web.Mvc;
using Curtin.Framework.Common.UserService;
using Elmah.Contrib.Mvc;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Mappers;
using Urdms.Dmp.Models;
using Urdms.Dmp.Models.DataDeposit;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Controllers
{
    [ElmahHandleError]
    public class DataDepositController : ProjectBaseController
    {
        public DataDepositController(ICurtinUserService lookupService, IProjectRepository projectRepository, IFieldOfResearchRepository fieldOfResearchRepository, ISocioEconomicObjectiveRepository socioEconomicObjectiveRepository)
            : base(lookupService, projectRepository, fieldOfResearchRepository, socioEconomicObjectiveRepository)
        {
        }

        public ActionResult New(int projectId)
        {
            var project = ProjectRepository.Get(projectId);
            if (project == null)
            {
                return View("ProjectNotFound");
            }

            if (!this.CurrentUser.IsPrincipalInvestigatorFor(project))
            {
                return View("NoProjectAccessRight");
            }

            if (project.SourceProjectType != SourceProjectType.DEPOSIT)
            {
                return View("CannotCreateDataDeposit");
            }

            if (project.DataDeposit != null)
            {
                return View("DataDepositAlreadyExists");
            }

            var vm = new DataDepositViewModel
            {
                ProjectId = project.Id,
                ProjectTitle = project.Title,
                PrincipalInvestigator = project.Parties
                    .Where(p => !string.IsNullOrWhiteSpace(p.Party.UserId) && p.Relationship == ProjectRelationship.PrincipalInvestigator)
                    .Single()
                    .Party
            };
            return View(vm);
        }

        protected override bool VerifyProjectRequest(Project entity, out ActionResult result)
        {
            // allows child classes to override behaviour
            if (entity == null)
            {
                result = View("ProjectNotFound");
                return false;
            }
            if (entity.SourceProjectType != SourceProjectType.DEPOSIT)
            {
                result = View("IncorrectProjectType");
                return false;
            }
            if (!this.CurrentUser.IsPrincipalInvestigatorFor(entity))
            {
                result = View("NoProjectAccessRight");
                return false;
            }
            result = new EmptyResult();
            return true;
        }

        [HttpPost]
		[DenyMethodByParameter(Name = "AddUrdmsUser,AddNonUrdmsUser,DeleteUrdmsUser,DeleteNonUrdmsUser,Confirm")]
        public ActionResult New(DataDepositViewModel vm)
        {
            var project = ProjectRepository.Get(vm.ProjectId);
            if (project == null)
            {
                return View("ProjectNotFound");
            }

            if (!this.CurrentUser.IsPrincipalInvestigatorFor(project))
            {
                return View("NoProjectAccessRight");
            }

            if (project.SourceProjectType != SourceProjectType.DEPOSIT)
            {
                return View("CannotCreateDataDeposit");
            }

            if (project.DataDeposit != null)
            {
                return View("DataDepositAlreadyExists");
            }
            var projectModel = new ProjectViewModel { DataDeposit = vm };
            // Copy project details from project entity
            projectModel.MapFrom(project, false, false);
            project.MapFrom(projectModel);
            var urdmsUsers = project.Parties.Where(o => !string.IsNullOrWhiteSpace(o.Party.UserId) && o.Party.Id == 0).Select(o => o.Party);
            // only update new parties
            this.UpdateUrdmsPartyDetails(urdmsUsers.ToList());

            ProjectRepository.Save(project);
            switch (project.ProvisioningStatus)
            {
                case ProvisioningStatus.NotStarted:
                    return RedirectToAction("ReviewDataDeposit", "Confirm", new { projectId = vm.ProjectId });
                default:
                    throw new InvalidOperationException("Uncatered Dmp Status");

            }
        }

        public ActionResult Edit(int id)
        {
            var project = ProjectRepository.GetByDataDepositId(id);

            if (project == null)
            {
                return View("ProjectNotFound");
            }

            if (!this.CurrentUser.IsPrincipalInvestigatorFor(project))
            {
                return View("NoProjectAccessRight");
            }

            if (project.SourceProjectType != SourceProjectType.DEPOSIT)
            {
                return View("CannotCreateDataDeposit");
            }

            if (project.ProvisioningStatus != ProvisioningStatus.NotStarted)
            {
                return View("CannotEditDataDeposit");
            }

            var vm = new DataDepositViewModel();
            vm.MapFrom(project);

            return View("New", vm);
        }

        [HttpPost]
		[DenyMethodByParameter(Name = "AddUrdmsUser,AddNonUrdmsUser,DeleteUrdmsUser,DeleteNonUrdmsUser,Confirm")]
        public ActionResult Edit(DataDepositViewModel model)
        {
            var project = ProjectRepository.GetByDataDepositId(model.Id);
            if (project == null)
            {
                return View("ProjectNotFound");
            }

            if (!this.CurrentUser.IsPrincipalInvestigatorFor(project))
            {
                return View("NoProjectAccessRight");
            }

            if (project.SourceProjectType != SourceProjectType.DEPOSIT)
            {
                return View("CannotCreateDataDeposit");
            }

            if (project.ProvisioningStatus != ProvisioningStatus.NotStarted)
            {
                return View("CannotEditDataDeposit");
            }

            var projectModel = new ProjectViewModel { DataDeposit = model };
            // Copy project details from project entity
            projectModel.MapFrom(project, false, false);
            project.MapFrom(projectModel);
            var urdmsUsers = project.Parties.Where(o => !string.IsNullOrWhiteSpace(o.Party.UserId) && o.Party.Id == 0).Select(o => o.Party);
            // only update new parties
            this.UpdateUrdmsPartyDetails(urdmsUsers.ToList());
            ProjectRepository.Save(project);

            switch (project.ProvisioningStatus)
            {
                case ProvisioningStatus.NotStarted:
                    return RedirectToAction("ReviewDataDeposit", "Confirm", new { projectId = model.ProjectId });
                case ProvisioningStatus.Pending:
                    return RedirectToAction("Index", "Project");
                default:
                    throw new InvalidOperationException("Uncatered Dmp Status");

            }
        }

        [HttpPost]
        [AcceptMethodByParameter(Name = "AddUrdmsUser")]
        [ActionName("New")]
        public ActionResult AddUrdmsUser(DataDepositViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.FindUserId))
            {
                return View("New", vm);
            }

            var project = ProjectRepository.Get(vm.ProjectId);
            if (project == null)
            {
                return View("ProjectNotFound");
            }

            if (!this.CurrentUser.IsPrincipalInvestigatorFor(project))
            {
                return View("NoProjectAccessRight");
            }

            if (project.SourceProjectType != SourceProjectType.DEPOSIT)
            {
                return View("CannotCreateDataDeposit");
            }

            if (project.DataDeposit != null)
            {
                return View("DataDepositAlreadyExists");
            }

            var urdmsUsers = GetUser(vm.FindUserId.Trim());
            if (urdmsUsers != null)
            {
                vm.AddUrdmsUser(urdmsUsers, AccessRole.Members);
                var entityUser = project.Parties
                    .Where(user => string.Compare(user.Party.UserId, urdmsUsers.CurtinId, true) == 0)
                    .Take(1)
                    .FirstOrDefault();
                var modelUser = vm.UrdmsUsers
                    .Where(user => string.Compare(user.UserId, urdmsUsers.CurtinId, true) == 0)
                    .Take(1)
                    .FirstOrDefault();
                if (entityUser != null && modelUser != null)
                {
                    modelUser.Id = entityUser.Id;
                    modelUser.PartyId = entityUser.Party.Id;
                }

            }
            vm.FindUserId = null;

            return View("New", vm);
        }

        [HttpPost]
        [AcceptMethodByParameter(Name = "AddNonUrdmsUser")]
        [ActionName("New")]
		public ActionResult AddNonUrdmsUser(DataDepositViewModel vm)
        {
            var project = ProjectRepository.Get(vm.ProjectId);
            if (project == null)
            {
                return View("ProjectNotFound");
            }

            if (!this.CurrentUser.IsPrincipalInvestigatorFor(project))
            {
                return View("NoProjectAccessRight");
            }

            if (project.SourceProjectType != SourceProjectType.DEPOSIT)
            {
                return View("CannotCreateDataDeposit");
            }

            if (project.DataDeposit != null)
            {
                return View("DataDepositAlreadyExists");
            }
            if (!string.IsNullOrWhiteSpace(vm.NonUrdmsNewUserName))
            {
                vm.AddNonUrdmsUser(vm.NonUrdmsNewUserName.Trim(), AccessRole.Visitors);

                var entityUser = project.Parties
                        .Where(user => string.Compare(user.Party.FullName.Trim(), vm.NonUrdmsNewUserName.Trim(), true) == 0)
                        .Take(1)
                        .FirstOrDefault();
                var modelUser = vm.NonUrdmsUsers
                    .Where(user => string.Compare(user.FullName.Trim(), vm.NonUrdmsNewUserName.Trim(), true) == 0)
                    .Take(1)
                    .FirstOrDefault();
                if (entityUser != null && modelUser != null)
                {
                    modelUser.Id = entityUser.Party.Id;
                }
                vm.NonUrdmsNewUserName = null;
            }
            return View("New", vm);
        }

        [HttpPost]
		[AcceptMethodByParameter(Name = "DeleteUrdmsUser")]
        [ActionName("New")]
        public ActionResult RemoveUrdmsUser(DataDepositViewModel vm)
        {
            return View("New", vm);
        }

        [HttpPost]
        [AcceptMethodByParameter(Name = "DeleteNonUrdmsUser")]
        [ActionName("New")]
		public ActionResult RemoveNonUrdmsUser(DataDepositViewModel vm)
        {
            return View("New", vm);
        }

        protected override ActionResult ReturnOnGetForProject(ProjectDetailsViewModel vm)
        {
            vm.SourceProjectType = SourceProjectType.DEPOSIT;
            vm.Status = ProjectStatus.Completed;

            return View("Project", vm);
        }

        protected override ActionResult ReturnOnPostForProject(Project project)
        {
            project.SourceProjectType = SourceProjectType.DEPOSIT;
            project.Status = ProjectStatus.Completed;
            project.Funders.Clear();
            ProjectRepository.Save(project);

            return ReturnOnSaveForProject(project);
        }

        protected override ActionResult ReturnOnSaveForProject(Project project)
        {
            if (project.DataDeposit != null)
            {
                return RedirectToAction("Index", "Project");
            }
            return RedirectToAction("New", new { projectId = project.Id });
        }
    }
}
