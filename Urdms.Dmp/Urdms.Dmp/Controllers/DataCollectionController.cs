using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Curtin.Framework.Common.UserService;
using Elmah.Contrib.Mvc;
using NServiceBus;
using Urdms.Approvals.ApprovalService.Commands;
using Urdms.Dmp.Controllers.Filters;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Entities.Components;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Mappers;
using Urdms.Dmp.Models;
using Urdms.Dmp.Models.DataCollectionModels;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Controllers
{
    [ElmahHandleError]
    public class DataCollectionController : DataCollectionBaseController<DataCollectionViewModelStep1,DataCollectionViewModelStep2>
    {
        public const int MaxStep = 2;

        public DataCollectionController(ICurtinUserService lookupService, IDataCollectionRepository dataCollectionRepository,
            IProjectRepository projectRepository, IFieldOfResearchRepository fieldOfResearchRepository,
            ISocioEconomicObjectiveRepository socioEconomicObjectiveRepository, IDataCollectionHashCodeRepository hashCodeRepository, IBus bus)
            : base(lookupService, dataCollectionRepository, projectRepository, fieldOfResearchRepository, socioEconomicObjectiveRepository, hashCodeRepository, bus)
        {
            
        }

        public ActionResult Index(int id)
        {
            var project = ProjectRepository.Get(id);

            if (project == null)
            {
                return View("ProjectNotFound");
            }

            ActionResult result;
            if (!VerifyCreate(project, out result))
            {
                return result;
            }

            var collections = DataCollectionRepository.GetByProject(id);
            var vm = new DataCollectionListViewModel
                         {
                             ProjectId = id,
                             ProjectTitle = project.Title,
                             DataCollectionItems = collections.Select(c =>
                                                                            new DataCollectionItemViewModel
                                                                                {
                                                                                    Id = c.Id,
                                                                                    RecordCreationDate = c.RecordCreationDate.ToShortDateString(),
                                                                                    Title = c.Title,
                                                                                    Status = c.CurrentState.State
                                                                                }
                                 )
                         };

            return View(vm);
        }

        [HttpPost]
        public ActionResult Index(DataCollectionListViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var project = ProjectRepository.Get(vm.ProjectId);
            if (project == null)
            {
                return View("ProjectNotFound");
            }
            ActionResult result;
            if (!VerifyCreate(project, out result))
            {
                return result;
            }
            // Start provisioning workflow
            var dataCollectionsForApproval = vm.DataCollectionItems.Where(dc => dc.IsUserSubmitted).ToList();
            var dataCollectionsForApprovalIds = dataCollectionsForApproval.Select(dc => dc.Id).ToList();

            dataCollectionsForApprovalIds.ForEach(id =>
                {
                    var dataCollection = DataCollectionRepository.Get(id);
                    if (dataCollection != null)
                    {
                        var manager = dataCollection.Parties
                            .Where(p => p.Relationship == DataCollectionRelationshipType.Manager)
                            .Take(1)
                            .FirstOrDefault();

                        Debug.Assert(manager != null);

						// TODO: Implement integration with approvals service/application or alter database directly as below
						Bus.Send<SubmitForApproval>(m =>
						{
							m.ApprovedBy = manager.Party.UserId.ToString();
							m.ApprovedOn = DateTime.Now;
							m.DataCollectionId = dataCollection.Id;
						});
						//dataCollection.CurrentState = new DataCollectionState(DataCollectionStatus.Submitted, DateTime.Now);
						//DataCollectionRepository.Save(dataCollection);
                    }
                });

            var submittedDataCollectionsModels = dataCollectionsForApproval.Select(
                    dc => new DataCollectionsForApprovalItemViewModel { Id = dc.Id, SubmisionDate = DateTime.Now.ToShortDateString(), Title = dc.Title }).ToList();

            if (submittedDataCollectionsModels.Count > 0)
            {
                return View("Submitted",
                            new SubmittedDataCollectionsViewModel
                                {
                                    ProjectId = vm.ProjectId,
                                    PublishedDataCollectionItems = submittedDataCollectionsModels
                                });
            }
            return View(vm);
        }

        public ActionResult New(int projectId)
        {
            return RedirectToAction("Step1", "DataCollection", new { projectId });
        }

        [ProvideDataCollectionSteps]
        public ActionResult Step1(int projectId, int? id)
        {
            var project = ProjectRepository.Get(projectId);

            if (project == null)
            {
                return View("ProjectNotFound");
            }
            ActionResult result;
            if (!VerifyCreate(project, out result))
            {
                return result;
            }
            var viewModel = new DataCollectionViewModelStep1();
            
            if (id != null)
            {
                var collection = DataCollectionRepository.Get(id.Value);

                if (collection == null)
                {
                    return HttpNotFound();
                }
                if (collection.ProjectId != projectId)
                {
                    return View("DataCollectionNotFound");
                }
                if (collection.CurrentState.State != DataCollectionStatus.Draft)
                {
                    return RedirectToAction("ViewReadOnlyDataCollection",
                                        new { projectId = collection.ProjectId, id = collection.Id });
                }
                viewModel.MapFrom(project, collection);
            }
            else
            {
                viewModel.MapFrom(project);

            }
            return View("DataCollectionStep1", viewModel);
        }
        
        [ProvideDataCollectionSteps]
        public ActionResult Step2(int projectId, int id)
        {
            var project = ProjectRepository.Get(projectId);

            if (project == null)
            {
                return View("ProjectNotFound");
            }

            var viewModel = new DataCollectionViewModelStep2();

            var collection = DataCollectionRepository.Get(id);
            if (collection == null || collection.ProjectId != projectId)
            {
                return View("DataCollectionNotFound");
            }
            if (collection.CurrentState.State != DataCollectionStatus.Draft)
            {
                return RedirectToAction("ViewReadOnlyDataCollection",
                                        new {projectId = collection.ProjectId, id = collection.Id});
            }

            viewModel.MapFrom(collection);

            return View("DataCollectionStep2", viewModel);
        }


        protected override ActionResult OnSaveToStep2(DataCollection collection)
        {
            return RedirectToAction("Index", new { id = collection.ProjectId });
        }


        protected override ActionResult OnRedirectToStep1(DataCollection collection)
        {
            return RedirectToAction("Step1", new { projectId = collection.ProjectId, id = collection.Id });
        }

        protected override ActionResult OnRedirectToStep2(DataCollection collection)
        {
            return RedirectToAction("Step2", new { projectId = collection.ProjectId, id = collection.Id });
        }

        protected override ActionResult OnPostToStep1(DataCollection collection)
        {
            if(collection.CurrentState.State != DataCollectionStatus.Draft)
            {
                return View("DataCollectionInvalidState");
            }

            if (collection.Id == 0)
            {

                // copy current values from project
                var project = ProjectRepository.Get(collection.ProjectId);
                if (project == null)
                {
                    return View("ProjectNotFound");
                }
                ActionResult result;
                if (!VerifyCreate(project, out result))
                {
                    return result;
                }

                collection.Keywords = project.Keywords;
                
                // prevent duplicate values
                var objectives = from o in project.SocioEconomicObjectives
                                 where !collection.SocioEconomicObjectives.Any(q => q.SocioEconomicObjective.Id == o.SocioEconomicObjective.Id)
                                 select new DataCollectionSocioEconomicObjective {SocioEconomicObjective = o.SocioEconomicObjective};
                collection.SocioEconomicObjectives.AddRange(objectives);

                // prevent duplicate values
                var fieldsOfResearch = from o in project.FieldsOfResearch
                                       where !collection.FieldsOfResearch.Any(q => q.FieldOfResearch.Id == o.FieldOfResearch.Id)
                                       select new DataCollectionFieldOfResearch {FieldOfResearch = o.FieldOfResearch};
                collection.FieldsOfResearch.AddRange(fieldsOfResearch);

                var parties = project.GetDataCollectionParties(collection);
                collection.Parties.AddRange(parties);
            }
            return base.OnPostToStep1(collection);
        }

        protected override ActionResult OnPostToStep2(DataCollection collection)
        {
            if (collection.CurrentState.State != DataCollectionStatus.Draft)
            {
                return View("DataCollectionInvalidState");
            }

            return base.OnPostToStep2(collection);
        }

        public ActionResult ViewReadOnlyDataCollection(int projectId, int id)
        {
            var vm = new DataCollectionReadOnlyViewModel();
            var project = ProjectRepository.Get(projectId);
            if (project == null)
            {
                return View("ProjectNotFound");
            }
            var dataCollection = DataCollectionRepository.Get(id);
            if (dataCollection != null)
            {
                if (dataCollection.CurrentState.State != DataCollectionStatus.Draft)
                {
                    vm.MapFrom(dataCollection, project);
                    return View("ReadOnly", vm);
                }
                return RedirectToAction("Step1", new {projectId = dataCollection.ProjectId, id = dataCollection.Id});
            }
            return View("DataCollectionNotFound");
        }

        private bool VerifyCreate(Project project, out ActionResult result)
        {
            result = new EmptyResult();
            switch(project.SourceProjectType)
            {
                case SourceProjectType.DMP:
                    if (project.DataManagementPlan == null)
                    {
                        result = View("ProjectWithNoDmp");
                        return false;
                    }
                    break;
                case SourceProjectType.DEPOSIT:
                    if (project.DataDeposit == null)
                    {
                        result = View("ProjectWithNoDataDeposit");
                        return false;
                    }
                    break;
                default:
                    result = HttpNotFound();
                    return false;
            }

            if (project.DataCollections == null || project.DataCollections.SingleOrDefault(o => o.IsFirstCollection) == null)
            {
                switch (project.SourceProjectType)
                {
                    case SourceProjectType.DMP:
                        result = View("DmpNotSubmitted");
                        break;
                    case SourceProjectType.DEPOSIT:
                        result = View("DataDepositNotSubmitted");
                        break;
                }
                return false;
            }
            return true;
        }
    }
}
