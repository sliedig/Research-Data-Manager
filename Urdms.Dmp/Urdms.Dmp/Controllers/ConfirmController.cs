using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Curtin.Framework.Common.UserService;
using Elmah.Contrib.Mvc;
using NServiceBus;
using Urdms.Dmp.Controllers.Filters;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Models;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Utils;
using Urdms.DocumentBuilderService.Commands;
using Urdms.ProvisioningService.Commands;

namespace Urdms.Dmp.Controllers
{
    [ElmahHandleError]
    public class ConfirmController : BaseController
    {

        private readonly IProjectRepository _projectRepository;
        private readonly IDataCollectionRepository _dataCollectionRepository;
        private readonly IBus _bus;

        /// <summary>
        /// Initializes a new instance of the ConfirmController class.
        /// </summary>
        public ConfirmController(IProjectRepository projectRepository, IDataCollectionRepository dataCollectionRepository, ICurtinUserService lookupService, IBus bus)
            : base(lookupService)
        {
            _projectRepository = projectRepository;
            _dataCollectionRepository = dataCollectionRepository;
            _bus = bus;
        }


        [ProvideDataManagementPlanSteps]
        public ActionResult Review(int id)
        {
            var project = _projectRepository.GetByDataManagementPlanId(id);
            if (project == null || project.DataManagementPlan == null)
            {
                return View("DmpNotFound");
            }
            if (!this.CurrentUser.IsPrincipalInvestigatorFor(project))
            {
                return View("NoProjectAccessRight");
            }
            if (project.ProvisioningStatus == ProvisioningStatus.Provisioned)
            {
                return View("DmpProvisioned");
            }

            var vm = new ConfirmDataManagementPlanViewModel { DataManagementPlanId = project.DataManagementPlan.Id, ProjectTitle = project.Title };
            return View("Review", vm);
        }

        public ActionResult ReviewDataDeposit(int projectId)
        {
            var project = _projectRepository.Get(projectId);
            if (project == null)
            {
                return View("ProjectNotFound");
            }
            if (project.SourceProjectType != SourceProjectType.DEPOSIT)
            {
                return View("IncorrectProjectType");
            }
            if (project.DataDeposit == null)
            {
                return View("DataDepositNotFound");
            }
            if (!this.CurrentUser.IsPrincipalInvestigatorFor(project))
            {
                return View("NoProjectAccessRight");
            }
            if (project.ProvisioningStatus == ProvisioningStatus.Provisioned)
            {
                return View("DataDepositProvisioned");
            }
            var vm = new ConfirmDataDepositViewModel { ProjectId = project.Id, ProjectTitle = project.Title };
            return View("ReviewDataDeposit", vm);
        }

        [HttpPost]
        [ProvideDataManagementPlanSteps]
        [DenyMethodByParameter(Name = "Previous,Republish")]
        public ActionResult Review(ConfirmDataManagementPlanViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get project information
            var project = _projectRepository.GetByDataManagementPlanId(model.DataManagementPlanId);
            if (project == null || project.DataManagementPlan == null)
            {
                return View("DmpNotFound");
            }
            if (project.SourceProjectType == SourceProjectType.DEPOSIT)
            {
                return View("IncorrectProjectType");
            }
            if (!this.CurrentUser.IsPrincipalInvestigatorFor(project))
            {
                return View("NoProjectAccessRight");
            }
            if (project.ProvisioningStatus == ProvisioningStatus.Provisioned)
            {
                return View("DmpProvisioned");
            }
            
			// TODO: You can use the URDMS.Integration solution to handle provisioning in a robust manner or just alter the database directly and implement 
			// further actions from the web application.
			_bus.Send<SiteRequestCommand>(m =>
				{
					m.ProjectId = project.Id;
					m.ProjectTitle = project.Title;
					m.ProjectDescription = project.Description;
					m.UserRoles = CreateUserRolesDictionary(project); ;
				});
        
            if (!project.DataCollections.Any(dc => dc.IsFirstCollection))
            {
                var dataCollection = project.CreateInitialDataCollection();
                _dataCollectionRepository.Save(dataCollection);
            }

            return RedirectToAction("Submitted", new { id = project.DataManagementPlan.Id });
        }

        private static Dictionary<string, string> CreateUserRolesDictionary(Project project)
        {
            var owners = project.Parties.Where(r => r.Role == AccessRole.Owners).ToList();
            var members = project.Parties.Where(r => r.Role == AccessRole.Members).ToList();
            var visitors = project.Parties.Where(r => r.Role == AccessRole.Visitors).ToList();

            var userRoles = new Dictionary<string, string>();
            if (owners.Count > 0)
            {
                userRoles.Add("Owners", string.Join(",", owners.Select(i => i.Party.UserId).ToArray()));
            }
            if (members.Count > 0)
            {
                userRoles.Add("Members", string.Join(",", members.Select(i => i.Party.UserId).ToArray()));
            }
            if (visitors.Count > 0)
            {
                userRoles.Add("Visitors", string.Join(",", visitors.Select(i => i.Party.UserId).ToArray()));
            }
            return userRoles;
        }

        [HttpPost]
        public ActionResult ReviewDataDeposit(ConfirmDataDepositViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get project information
            var project = _projectRepository.Get(model.ProjectId);
            if (project == null)
            {
                return View("ProjectNotFound");
            }
            if (project.SourceProjectType != SourceProjectType.DEPOSIT)
            {
                return View("IncorrectProjectType");
            }
            if (project.DataDeposit == null)
            {
                return View("DataDepositNotFound");
            }
            if (!this.CurrentUser.IsPrincipalInvestigatorFor(project))
            {
                return View("NoProjectAccessRight");
            }
            if (project.ProvisioningStatus == ProvisioningStatus.Provisioned)
            {
                return View("DataDepositProvisioned");
            }

            // Start provisioning workflow
			// TODO: You can use the URDMS.Integration solution to handle provisioning in a robust manner or just alter the database directly as in the
			// commented code below and implement further actions from the web application.
			_bus.Send<SiteRequestCommand>(m =>
			{
				m.ProjectId = project.Id;
				m.ProjectTitle = project.Title;
				m.ProjectDescription = project.Description;
				m.UserRoles = CreateUserRolesDictionary(project);
			});

			//project.ProvisioningStatus = ProvisioningStatus.Pending;
			//_projectRepository.Save(project);

            if (!project.DataCollections.Any(dc => dc.IsFirstCollection))
            {
                var dataCollection = project.CreateInitialDataCollection();
                _dataCollectionRepository.Save(dataCollection);
            }

            return RedirectToAction("SubmittedDataDeposit", new { projectId = model.ProjectId });
        }


        [HttpPost]
        [AcceptMethodByParameter(Name = "Previous")]
        [ActionName("Review")]
        public ActionResult Previous(int id)
        {
            return RedirectToAction("Edit", "Dmp", new { id, step = 5 });
        }

        [ProvideDataManagementPlanSteps]
        public ActionResult Republish(int id)
        {
            var project = _projectRepository.GetByDataManagementPlanId(id);

            if (project == null || project.DataManagementPlan == null)
            {
                return View("DmpNotFound");
            }

            if (project.SourceProjectType == SourceProjectType.DEPOSIT)
            {
                return View("IncorrectProjectType");
            }

            if (!this.CurrentUser.IsPrincipalInvestigatorFor(project))
            {
                return View("NoProjectAccessRight");
            }

            if (project.ProvisioningStatus != ProvisioningStatus.Provisioned)
            {
                return View("DmpNotProvisioned");
            }

            var vm = new DataManagementPlanInfoViewModel
                         {
                             ProjectId = project.Id,
                             ProjectTitle = project.Title,
                             DataManagementPlanId = project.DataManagementPlan.Id
                         };

            return View(vm);
        }

        [HttpPost]
        [AcceptMethodByParameter(Name = "Republish")]
        public ActionResult Republish(DataManagementPlanInfoViewModel vm)
        {
            var project = _projectRepository.Get(vm.ProjectId);
            if (project == null)
            {
                return View("ProjectNotFound");
            }
            if (project.DataManagementPlan == null)
            {
                return View("DmpNotFound");
            }
            if (project.SourceProjectType == SourceProjectType.DEPOSIT)
            {
                return View("IncorrectProjectType");
            }
            if (!this.CurrentUser.IsPrincipalInvestigatorFor(project))
            {
                return View("NoProjectAccessRight");
            }
            if (project.ProvisioningStatus != ProvisioningStatus.Provisioned)
            {
                return View("DmpNotProvisioned");
            }

			_bus.Send<GenerateDmpCommand>(m =>
												{
													m.ProjectId = project.Id;
													m.SiteUrl = project.SiteUrl;
												});

            return RedirectToAction("Republished", "Confirm");
        }

        [ProvideDataManagementPlanSteps]
        public ActionResult Republished()
        {
            return View("Republished");
        }


        [ProvideDataManagementPlanSteps]
        public ActionResult Submitted(int id)
        {
            var project = _projectRepository.GetByDataManagementPlanId(id);
            if (project == null)
            {
                return View("ProjectNotFound");
            }
            if (project.SourceProjectType == SourceProjectType.DEPOSIT)
            {
                return View("IncorrectProjectType");
            }
            if (project.DataManagementPlan == null)
            {
                return View("DmpNotFound");
            }
            if (!this.CurrentUser.IsPrincipalInvestigatorFor(project))
            {
                return View("NoProjectAccessRight");
            }
            return View(project);
        }


        public ActionResult SubmittedDataDeposit(int projectId)
        {
            var project = _projectRepository.Get(projectId);
            if (project == null)
            {
                return View("ProjectNotFound");
            }
            if (project.SourceProjectType != SourceProjectType.DEPOSIT)
            {
                return View("IncorrectProjectType");
            }
            if (project.DataDeposit == null)
            {
                return View("DataDepositNotFound");
            }
            if (!this.CurrentUser.IsPrincipalInvestigatorFor(project))
            {
                return View("NoProjectAccessRight");
            }
            return View(project);
        }

        [ProvideDataManagementPlanSteps]
        public ActionResult Pending(int id)
        {
            var project = _projectRepository.GetByDataManagementPlanId(id);
            if (project == null || project.DataManagementPlan == null)
            {
                return View("DmpNotFound");
            }
            if (project.SourceProjectType == SourceProjectType.DEPOSIT)
            {
                return View("IncorrectProjectType");
            }
            if (!this.CurrentUser.IsPrincipalInvestigatorFor(project))
            {
                return View("NoProjectAccessRight");
            }
            if (project.ProvisioningStatus != ProvisioningStatus.Pending)
            {
                return View("DmpNotInProgress");
            }
            var vm = new DataManagementPlanInfoViewModel { ProjectId = project.Id, DataManagementPlanId = project.DataManagementPlan.Id, ProjectTitle = project.Title };
            return View(vm);
        }

        [HttpPost]
        [AcceptMethodByParameter(Name = "Previous")]
        public ActionResult Pending(DataManagementPlanInfoViewModel vm)
        {
            return RedirectToAction("Edit", "Dmp", new { id = vm.DataManagementPlanId, step = 6 });
        }
    }
}
