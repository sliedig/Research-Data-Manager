using System;
using System.Linq;
using System.Web.Mvc;
using Curtin.Framework.Common.UserService;
using Omu.ValueInjecter;
using Urdms.Dmp.Controllers.Helpers;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Mappers;
using Urdms.Dmp.Models;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Controllers
{
    public abstract class ProjectBaseController : BaseController
    {
        protected readonly IProjectRepository ProjectRepository;
        protected readonly IFieldOfResearchRepository FieldOfResearchRepository;
        protected readonly ISocioEconomicObjectiveRepository SocioEconomicObjectiveRepository;

        protected ProjectBaseController(ICurtinUserService lookupService, IProjectRepository projectRepository, IFieldOfResearchRepository fieldOfResearchRepository, ISocioEconomicObjectiveRepository socioEconomicObjectiveRepository) 
            : base(lookupService)
        {
            ProjectRepository = projectRepository;
            FieldOfResearchRepository = fieldOfResearchRepository;
            SocioEconomicObjectiveRepository = socioEconomicObjectiveRepository;
        }

        public ActionResult Project(int? id)
        {
            var vm = new ProjectDetailsViewModel();

            if (id > 0)
            {
                var savedProject = ProjectRepository.Get(id.Value);
                ActionResult result;
                if (!VerifyProjectRequest(savedProject, out result))
                {
                    return result;
                }
                vm.InjectFrom(savedProject);
                vm.StartDate = savedProject.StartDate != null ? savedProject.StartDate.Value.ToShortDateString() : string.Empty;
                vm.EndDate = savedProject.EndDate != null ? savedProject.EndDate.Value.ToShortDateString() : string.Empty;
                vm.Keywords = savedProject.Keywords;
                vm.MapFrom(savedProject);
            }

            return ReturnOnGetForProject(vm);
        }

        protected abstract ActionResult ReturnOnGetForProject(ProjectDetailsViewModel vm);

        protected abstract bool VerifyProjectRequest(Project entity, out ActionResult result);
        
        [HttpPost]
        [DenyMethodByParameter(Name = "AddForCode,AddSeoCode,DeleteForCodes,DeleteSeoCodes")]
        public ActionResult Project(ProjectDetailsViewModel vm)
        {
            if (vm.Id > 0)
            {
                var savedProject = this.ProjectRepository.Get(vm.Id);
                ActionResult result;
                if (!VerifyProjectRequest(savedProject, out result))
                {
                    return result;
                }
            }
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var project = GetProjectFromViewModel(vm);
            project.MapFrom(vm);
            if (!project.Parties.Any(o => o.Relationship == ProjectRelationship.PrincipalInvestigator))
            {
                project.Parties.Add(new ProjectParty
                {
                    Party = new Party
                    {
                        UserId = CurrentUser.CurtinId,
                        Email = CurrentUser.EmailAddress,
                        FirstName = CurrentUser.FirstName,
                        FullName = CurrentUser.FullName,
                        LastName = CurrentUser.LastName,
						Organisation = ""	// TODO: Insert your organisation here
                    },
                    Project = project,
                    Role = AccessRole.Owners,
                    Relationship = ProjectRelationship.PrincipalInvestigator
                });
            }
            return ReturnOnPostForProject(project);
        }

        protected virtual ActionResult ReturnOnPostForProject(Project project)
        {
            ProjectRepository.Save(project);
            return ReturnOnSaveForProject(project);
        }

        protected abstract ActionResult ReturnOnSaveForProject(Project project);
        
        [HttpPost]
        [AcceptMethodByParameter(Name = "AddForCode")]
        [ActionName("Project")]
        public ActionResult AddForCode(ProjectDetailsViewModel vm)
        {
            ModelState.Clear();

            try
            {
                FieldOfResearchRepository.PopulateForCodes<ProjectFieldOfResearch>(vm, ControllerContext.HttpContext.Request.Form);
                ClearAllCodes(vm);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(e.Source, e.Message);
            }

            return View("Project", vm);
        }

        [HttpPost]
        [AcceptMethodByParameter(Name = "AddSeoCode")]
        [ActionName("Project")]
        public ActionResult AddSeoCode(ProjectDetailsViewModel vm)
        {
            ModelState.Clear();

            try
            {
                SocioEconomicObjectiveRepository.PopulateSeoCodes<ProjectSocioEconomicObjective>(vm, ControllerContext.HttpContext.Request.Form);
                ClearAllCodes(vm);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(e.Source, e.Message);
            }

            return View("Project", vm);
        }

        [HttpPost]
        [AcceptMethodByParameter(Name = "DeleteForCodes")]
        [ActionName("Project")]
        public ActionResult DeleteForCodes(ProjectDetailsViewModel vm)
        {
            ModelState.Clear();
            try
            {
                vm.DeleteForCodes(ControllerContext.HttpContext.Request.Form);
                ClearAllCodes(vm);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(e.Source, e.Message);
            }
            return View("Project", vm);
        }


        [HttpPost]
        [AcceptMethodByParameter(Name = "DeleteSeoCodes")]
        [ActionName("Project")]
        public ActionResult DeleteSeoCodes(ProjectDetailsViewModel vm)
        {
            ModelState.Clear();
            try
            {
                vm.DeleteSeoCodes(ControllerContext.HttpContext.Request.Form);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(e.Source, e.Message);
            }
            return View("Project", vm);
        }

        private static void ClearAllCodes(ProjectDetailsViewModel vm)
        {
            vm.FieldOfResearchCode = null;
            vm.SocioEconomicObjectiveCode = null;
        }


        private Project GetProjectFromViewModel(dynamic viewModel)
        {
            var id = (int)viewModel.Id;
            var project = ProjectRepository.Get(id) ?? new Project();
            project.InjectFrom((object)viewModel);
            return project;
        }

        public ActionResult Introduction()
        {
            return View("ProjectIntroduction");
        }

        [HttpPost]
        public ActionResult Introduction(FormCollection data)
        {
            return RedirectToAction("Project");
        }
    }
}
