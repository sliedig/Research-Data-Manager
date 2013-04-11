using System;
using System.Linq;
using System.Web.Mvc;
using Curtin.Framework.Common.UserService;
using Elmah.Contrib.Mvc;
using Urdms.Dmp.Controllers.Filters;
using Urdms.Dmp.Controllers.Helpers;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Mappers;
using Urdms.Dmp.Models;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Controllers
{
    [ElmahHandleError]
    public class DmpController : BaseController
    {
        private readonly NavigationButton _navigationText;
        private readonly IProjectRepository _projectRepository;
        private readonly ITimerRepository _timerRepository;

        public const int MaxStep = 5;
        public const int UserManagementStep = 4;
        public DmpController(ICurtinUserService lookupService, IProjectRepository projectRepository, ITimerRepository timerRepository)
            : base(lookupService)
        {
            _projectRepository = projectRepository;
            _timerRepository = timerRepository;
            _navigationText = new NavigationButton();
            ViewBag.NavigationButton = _navigationText;
        }

        [ProvideDataManagementPlanSteps]
        public ActionResult New(int id)
        {
            try
            {
                var project = _projectRepository.Get(id);

                if (project == null)
                {
                    return View("ProjectNotFound");
                }
                if (project.SourceProjectType == SourceProjectType.DEPOSIT)
                {
                    return View("IncorrectProjectType");
                }
                if (!this.CurrentUser.IsPrincipalInvestigatorFor(project))
                {
                    return View("NoProjectAccessRight");
                }
                // project already exists, so no creation done
                // redirect to edit
                if (project.DataManagementPlan != null)
                {
                    return RedirectToAction("Edit", new { id = project.DataManagementPlan.Id, step = 1 });
                }
                project.DataManagementPlan = new DataManagementPlan();
              
                _projectRepository.Save(project);
                // redirect to edit now that the item has been created
                return RedirectToAction("Edit", new { id = project.DataManagementPlan.Id, step = 1 });


            }
            catch
            {
                return View("Error");
            }
        }

        [HttpGet]
        [ProvideDataManagementPlanSteps]
        public ActionResult Edit(int id, int step = 1)
        {
            var project = _projectRepository.GetByDataManagementPlanId(id);
            if (project == null)
            {
                return View("DmpNotFound");
            }
            if (!this.CurrentUser.IsPrincipalInvestigatorFor(project))
            {
                return View("NoProjectAccessRight");
            }
            if (step < 1 || step > MaxStep)
            {
                return View("PageNotFound");
            }

            var viewName = string.Format("Step{0}", step);
            // note that the view model for gets are fully instantiated and populated and
            // receives all information from the entity via mapping
            var vm = ProjectViewModel.NewFullViewModel();
            vm.DataManagementPlan.Step = step;
            vm.MapFrom(project);

            vm.DataManagementPlan.Start = DateTime.Now;

            return View(viewName, vm.DataManagementPlan);
        }

        [HttpPost]
        [ProvideDataManagementPlanSteps]
		[DenyMethodByParameter(Name = "AddUrdmsUser,AddNonUrdmsUser,DeleteUrdmsUser,DeleteNonUrdmsUser,Confirm")]
        public ActionResult Edit(DataManagementPlanViewModel vm)
        {
            var project = _projectRepository.GetByDataManagementPlanId(vm.Id);
            if (project == null)
            {
                return View("DmpNotFound");
            }
            if (!this.CurrentUser.IsPrincipalInvestigatorFor(project))
            {
                return View("NoProjectAccessRight");
            }
            if (vm.Step < 1 || vm.Step > MaxStep)
            {
                return View("PageNotFound");
            }
            if (!ModelState.IsValid)
            {
                return View(string.Format("Step{0}", vm.Step), vm);
            }
            var currentStep = vm.Step;
            vm.Step = GetStep(vm.Step);

            var projectModel = new ProjectViewModel { DataManagementPlan = vm };
            // Copy project details from project entity
            projectModel.MapFrom(project, false);
            // delete users when in the appropriate step
            project.MapFrom(projectModel, currentStep == UserManagementStep);
            var urdmsUsers = project.Parties.Where(o => !string.IsNullOrWhiteSpace(o.Party.UserId) && o.Party.Id == 0).Select(o => o.Party);
            // only update new parties
            this.UpdateUrdmsPartyDetails(urdmsUsers.ToList());

            _projectRepository.Save(project);

            //Dmp form completion timer for steps 1-4. Only on next button submit.
            if (currentStep < vm.Step)
            {
                _timerRepository.Save(new FormTimer
                                          {
                                              UserId = CurrentUser.CurtinId,
                                              EndTime = DateTime.Now,
                                              Id = vm.Id,
                                              StartTime = vm.Start,
                                              Step = vm.Step - 1
                                          });
            }

            return RedirectToAction("Edit", new { id = vm.Id, step = vm.Step });
        }


        [HttpPost]
        [ProvideDataManagementPlanSteps]
        [AcceptMethodByParameter(Name = "AddUrdmsUser")]
        [ActionName("Edit")]
        public ActionResult AddUrdmsUser(DataManagementPlanViewModel vm)
        {
            if (!string.IsNullOrWhiteSpace(vm.FindUserId))
            {
                var urdmsUser = GetUser(vm.FindUserId.Trim());
                if (urdmsUser != null)
                {
                    var project = _projectRepository.GetByDataManagementPlanId(vm.Id);

                    vm.AddUrdmsUser(urdmsUser, AccessRole.Members);

                    var entityUser = project.Parties
                        .Where(user => string.Compare(user.Party.UserId, urdmsUser.CurtinId, true) == 0)
                        .Take(1)
                        .FirstOrDefault();

                    var modelUser = vm.UrdmsUsers
                        .Where(user => string.Compare(user.UserId, urdmsUser.CurtinId, true) == 0)
                        .Take(1)
                        .FirstOrDefault();

                    if (entityUser != null && modelUser != null)
                    {
                        modelUser.Id = entityUser.Id;
                        modelUser.PartyId = entityUser.Party.Id;
                    }

                }

                vm.FindUserId = null;
            }

            var viewName = string.Format("Step{0}", vm.Step);
            return View(viewName, vm);
        }

        [HttpPost]
        [ProvideDataManagementPlanSteps]
        [AcceptMethodByParameter(Name = "AddNonUrdmsUser")]
        [ActionName("Edit")]
		public ActionResult AddNonUrdmsUser(DataManagementPlanViewModel vm)
        {
            if (!string.IsNullOrWhiteSpace(vm.NonUrdmsNewUserName))
            {
                var project = _projectRepository.GetByDataManagementPlanId(vm.Id);
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
            var viewName = string.Format("Step{0}", vm.Step);
            return View(viewName, vm);
        }

        [HttpPost]
        [ProvideDataManagementPlanSteps]
		[AcceptMethodByParameter(Name = "DeleteUrdmsUser")]
        [ActionName("Edit")]
		public ActionResult RemoveUrdmsUser(DataManagementPlanViewModel vm)
        {
            var viewName = string.Format("Step{0}", vm.Step);
            return View(viewName, vm);
        }

        [HttpPost]
        [ProvideDataManagementPlanSteps]
        [AcceptMethodByParameter(Name = "DeleteNonUrdmsUser")]
        [ActionName("Edit")]
		public ActionResult RemoveNonUrdmsUser(DataManagementPlanViewModel vm)
        {
            var viewName = string.Format("Step{0}", vm.Step);
            return View(viewName, vm);
        }

        [HttpPost]
        [AcceptMethodByParameter(Name = "Confirm")]
        [ActionName("Edit")]
        public ActionResult Confirm(DataManagementPlanViewModel vm)
        {
            var project = _projectRepository.GetByDataManagementPlanId(vm.Id);
            if (project == null)
            {
                return View("ProjectNotFound");
            }
            if (!this.CurrentUser.IsPrincipalInvestigatorFor(project))
            {
                return View("NoProjectAccessRight");
            }
            if (project.DataManagementPlan == null)
            {
                return View("DmpNotFound");
            }
            if (!ModelState.IsValid)
            {
                return View(string.Format("Step{0}", vm.Step), vm);
            }
            vm.Status = project.ProvisioningStatus;
            var projectModel = new ProjectViewModel().MapFrom(project);
            projectModel.DataManagementPlan = vm;
            project.MapFrom(projectModel);

            _projectRepository.Save(project);
            
            //Dmp form completion timer for step 5.
            _timerRepository.Save(new FormTimer
            {
                UserId = CurrentUser.CurtinId,
                EndTime = DateTime.Now,
                Id = vm.Id,
                StartTime = vm.Start,
                Step = vm.Step
            });
           
            switch (project.ProvisioningStatus)
            {
                case ProvisioningStatus.NotStarted:
                    return RedirectToAction("Review", "Confirm", new { id = vm.Id });
                case ProvisioningStatus.Provisioned:
                    return RedirectToAction("Republish", "Confirm", new { id = vm.Id });
                case ProvisioningStatus.Pending:
                    return RedirectToAction("Pending", "Confirm", new { id = vm.Id });
                default:
                    throw new InvalidOperationException("Uncatered Dmp Status");

            }

        }

        private int GetStep(int currentStep = 1)
        {
            var stepAction = ControllerContext.HttpContext.Request.Form["stepAction"];
            if (stepAction == _navigationText.Previous)
            {
                currentStep--;
            }
            else if (stepAction == _navigationText.Next)
            {
                currentStep++;
            }
            if (currentStep < 1)
            {
                currentStep = 1;
            }
            if (currentStep > MaxStep)
            {
                currentStep = MaxStep;
            }
            return currentStep;
        }


    }
}

