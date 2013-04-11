using System;
using System.Collections.Generic;
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
using Urdms.Dmp.Models.ApprovalModels;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Controllers
{
    [ElmahHandleError]
    [RequiresAnyRoleType(ApplicationRole.QaApprover, ApplicationRole.SecondaryApprover)]
    public class ApprovalController : DataCollectionBaseController<DataCollectionApprovalViewModelStep1,DataCollectionApprovalViewModelStep2>
    {
        public ApprovalController(ICurtinUserService lookupService, IDataCollectionRepository dataCollectionRepository,
            IProjectRepository projectRepository, IFieldOfResearchRepository fieldOfResearchRepository,
            ISocioEconomicObjectiveRepository socioEconomicObjectiveRepository, IDataCollectionHashCodeRepository hashCodeRepository, IBus bus)
            : base(lookupService, dataCollectionRepository, projectRepository, fieldOfResearchRepository, socioEconomicObjectiveRepository, hashCodeRepository, bus)
        {

        }

        public ActionResult Index()
        {
            if (User.IsInRole(ApplicationRole.QaApprover.GetDescription()))
            {
                var ordCollections = DataCollectionRepository.GetByStatus(new List<DataCollectionStatus> {DataCollectionStatus.Submitted, DataCollectionStatus.SecondaryApproved});
                var ordModel = new DataCollectionApprovalListViewModel
                             {
                                 DataCollectionItems = MapDataCollectionItems(ordCollections)
                             };
                ViewBag.EmptyListMessage = "No Data Collections requiring QA approval.";
                return View(ordModel);
            }

            var qaCollections = DataCollectionRepository.GetByStatus(new List<DataCollectionStatus> { DataCollectionStatus.QaApproved, DataCollectionStatus.RecordAmended });
            var qaModel = new DataCollectionApprovalListViewModel
            {
                DataCollectionItems = MapDataCollectionItems(qaCollections)
            };
            ViewBag.EmptyListMessage = "No Data Collections requiring secondary approval.";
            return View(qaModel);
        }

        public ActionResult Detail(int id)
        {
            return RedirectToAction("Step1", new {id});
        }

        [ProvideDataCollectionSteps]
        public ActionResult Step1(int id)
        {
            var collection = DataCollectionRepository.Get(id);

            if (collection == null || !VerifyUserRoleWithDataCollectionStatus(collection))
            {
                return View("DataCollectionNotFound");
            }
            var project = ProjectRepository.Get(collection.ProjectId);
            if (project == null)
            {
                return View("ProjectNotFound");
            }
            var viewModel = new DataCollectionApprovalViewModelStep1();
            viewModel.MapFrom(project, collection);
            return View("DataCollectionStep1", viewModel);
        }

        [ProvideDataCollectionSteps]
        public ActionResult Step2(int id)
        {
            var collection = DataCollectionRepository.Get(id);

            if (collection == null || !VerifyUserRoleWithDataCollectionStatus(collection))
            {
                return View("DataCollectionNotFound");
            }
            var project = ProjectRepository.Get(collection.ProjectId);
            if (project == null)
            {
                return View("ProjectNotFound");
            }
            var viewModel = new DataCollectionApprovalViewModelStep2();
            viewModel.MapFrom(collection);
            return View("DataCollectionStep2", viewModel);
        }

        [HttpPost]
        [DenyMethodByParameter(Name = "confirm,saveAndPrevious,AddForCode,AddSeoCode,DeleteForCodes,DeleteSeoCodes,AddUrdmsUser,AddNonUrdmsUser,DeleteUrdmsUser,DeleteNonUrdmsUser")]
        [ProvideDataCollectionSteps]
        public override ActionResult Step2(DataCollectionApprovalViewModelStep2 vm)
        {
            return base.Step2(vm);
        }

        [HttpPost]
        [ProvideDataCollectionSteps]
        public override ActionResult Step1(DataCollectionApprovalViewModelStep1 vm)
        {
            return base.Step1(vm);
        }

        [ProvideDataCollectionSteps]
        public ActionResult Confirm(int id)
        {
            var dataCollection = DataCollectionRepository.Get(id);
            if (dataCollection != null)
            {
                if (VerifyUserRoleWithDataCollectionStatus(dataCollection))
                {
                    var vm = new ApprovalConfirmationViewModel { DataCollectionId = id };
                    PopulateModel(vm, dataCollection, HashCodeRepository.GetByDataCollectionId(dataCollection.Id));
                    return View(vm);
                }
                return View("DataCollectionInvalidState");
            }
            return View("DataCollectionNotFound");
        }

        [HttpPost]
        [DenyMethodByParameter(Name = "cancel,reapproval")]
        [ProvideDataCollectionSteps]
        public ActionResult Confirm(ApprovalConfirmationViewModel vm)
        {
           var collection = DataCollectionRepository.Get(vm.DataCollectionId);
            if (collection == null)
            {
                return View("DataCollectionNotFound");
            }

            if(!VerifyUserRoleWithDataCollectionStatus(collection))
            {
                return View("DataCollectionInvalidState");
            }

            var hashCode = HashCodeRepository.GetByDataCollectionId(collection.Id);
            PopulateModel(vm, collection, hashCode);

            switch (vm.State)
            {
                case DataCollectionStatus.Submitted:
                    if (!vm.IsQaApproved)
                    {
                        ModelState.AddModelError("IsQaApproved", "Please confirm approval");
                        return View(vm);
                    }

					// TODO: You can use the URDMS.Integration solution to handle approvals in a robust manner or just alter the database directly as in the
					// commented code below.
					Bus.Send<SubmitForSecondaryApproval>(m =>
													   {
														   m.DataCollectionId = collection.Id;
														   m.ApprovedBy = CurrentUser.CurtinId;
														   m.ApprovedOn = DateTime.Now;
													   });

					//collection.CurrentState = new DataCollectionState(DataCollectionStatus.QaApproved, DateTime.Now);
					//DataCollectionRepository.Save(collection);

                    vm.ProposedState = DataCollectionStatus.QaApproved;
                    return View("Approved", vm);

                case DataCollectionStatus.QaApproved:
                case DataCollectionStatus.RecordAmended:
                    if (!vm.DoesNotViolateAgreements || !vm.DoesNotViolateConfidentialityAndEthics)
                    {
                        if (!vm.DoesNotViolateAgreements)
                        {
                            ModelState.AddModelError("DoesNotViolateAgreements", "Please confirm that agreements are not violated");
                        }
                        if (!vm.DoesNotViolateConfidentialityAndEthics)
                        {
                            ModelState.AddModelError("DoesNotViolateConfidentialityAndEthics", "Please confirm that confidentiality and ethics requirements are met");
                        }
                        return View(vm);
                    }
					// TODO: You can use the URDMS.Integration solution to handle approvals in a robust manner or just alter the database directly as in the
					// commented code below.
					Bus.Send<SubmitForFinalApproval>(m =>
															 {
																 m.DataCollectionId = collection.Id;
																 m.ApprovedBy = CurrentUser.CurtinId;
																 m.ApprovedOn = DateTime.Now;
															 });

					//collection.CurrentState = new DataCollectionState(DataCollectionStatus.SecondaryApproved, DateTime.Now);
					//DataCollectionRepository.Save(collection);

                    vm.ProposedState = DataCollectionStatus.SecondaryApproved;
                    // save hashcode to capture the snapshot of the data
                    this.HashCodeRepository.SaveByDataCollection(collection);
                    return View("Approved", vm);

                case DataCollectionStatus.SecondaryApproved:
                    {
                        if (!vm.IsPublicationApproved)
                        {
                            ModelState.AddModelError("IsPublicationApproved", "Please confirm publication approval");
                            return View(vm);
                        }
						// TODO: You can use the URDMS.Integration solution to handle approvals in a robust manner or just alter the database directly as in the
						// commented code below.
						Bus.Send<PublishDataCollection>(m =>
															 {
																 m.DataCollectionId = collection.Id;
																 m.ApprovedBy = CurrentUser.CurtinId;
																 m.ApprovedOn = DateTime.Now;
															 });

						//collection.CurrentState = new DataCollectionState(DataCollectionStatus.Publishing, DateTime.Now);
						//DataCollectionRepository.Save(collection);

                        vm.ProposedState = DataCollectionStatus.Publishing;
                    }
                   
                    return View("Approved", vm);

                default:
                    throw new InvalidOperationException();
            }
        }

        [HttpPost]
        [ActionName("Confirm")]
        [AcceptMethodByParameter(Name = "cancel")]
        [ProvideDataCollectionSteps]
        public ActionResult CancelConfirmation(ApprovalConfirmationViewModel vm)
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("Confirm")]
        [AcceptMethodByParameter(Name = "reapproval")]
        [ProvideDataCollectionSteps]
        public ActionResult SubmitForReapproval(ApprovalConfirmationViewModel vm)
        {
            var collection = DataCollectionRepository.Get(vm.DataCollectionId);
            if (collection == null)
            {
                return View("DataCollectionNotFound");
            }
            if (!VerifyUserRoleWithDataCollectionStatus(collection))
            {
                return View("DataCollectionInvalidState");
            }

            var hashCode = HashCodeRepository.GetByDataCollectionId(collection.Id);
            PopulateModel(vm, collection, hashCode);

            if (vm.IsChanged)
            {
				// TODO: You can use the URDMS.Integration solution to handle approvals in a robust manner or just alter the database directly as in the
				// commented code below.
				Bus.Send<SubmitForSecondaryReApproval>(m =>
				{
					m.DataCollectionId = collection.Id;
					m.ApprovedBy = CurrentUser.CurtinId;
					m.ApprovedOn = DateTime.Now;
				});

				//collection.CurrentState = new DataCollectionState(DataCollectionStatus.RecordAmended, DateTime.Now);
				//DataCollectionRepository.Save(collection);

                // clear out hashcode at this point.
                this.HashCodeRepository.Delete(hashCode);
            }

            return RedirectToAction("Index");
        }


        private void PopulateModel(ApprovalConfirmationViewModel vm, DataCollection collection, DataCollectionHashCode hashCode)
        {

            vm.IsChanged = hashCode != null && hashCode.IsDifferentHashCode(collection);
            vm.State = collection.CurrentState.State;
            vm.Title = collection.Title;
        }


        protected override ActionResult OnSaveToStep2(DataCollection collection)
        {
            return RedirectToAction("Confirm", new { id = collection.Id});
        }

        protected override ActionResult OnRedirectToStep1(DataCollection collection)
        {
            return RedirectToAction("Step1", new { id = collection.Id });
        }

        protected override ActionResult OnRedirectToStep2(DataCollection collection)
        {
            return RedirectToAction("Step2", new { id = collection.Id });
        }

        protected override ActionResult OnPostToStep1(DataCollection collection)
        {
            if (!VerifyUserRoleWithDataCollectionStatus(collection))
            {
                return View("DataCollectionNotFound");
            }
            return base.OnPostToStep1(collection);
        }

        protected override ActionResult OnPostToStep2(DataCollection collection)
        {
            if (!VerifyUserRoleWithDataCollectionStatus(collection))
            {
                return View("DataCollectionNotFound");
            }
            return base.OnPostToStep2(collection);
        }

        private bool VerifyUserRoleWithDataCollectionStatus(DataCollection collection)
        {
            if (collection == null || collection.CurrentState == null)
            {
                return false;
            }
            if (User.IsInRole(ApplicationRole.QaApprover.GetDescription()))
            {
                var allowableStatus = new[] { DataCollectionStatus.Submitted, DataCollectionStatus.SecondaryApproved };
                return allowableStatus.Any(o => o == collection.CurrentState.State);
            }
            if (User.IsInRole(ApplicationRole.SecondaryApprover.GetDescription()))
            {
                var allowableStatus = new[] { DataCollectionStatus.QaApproved, DataCollectionStatus.RecordAmended };
                return allowableStatus.Any(o => o == collection.CurrentState.State);
            }
            return false;
        }

        private static IEnumerable<DataCollectionApprovalItemViewModel> MapDataCollectionItems(IEnumerable<DataCollection> dataCollections)
        {
            return dataCollections.Select(c =>
                                            new DataCollectionApprovalItemViewModel
                                            {
                                                Id = c.Id,
                                                DateSubmitted = c.CurrentState.StateChangedOn,
                                                Title = c.Title,
                                                SubmittedBy = c.Parties
                                                    .Where(p => p.Relationship == DataCollectionRelationshipType.Manager)
                                                    .Single()
                                                    .Party
                                                    .FullName,
                                                Status = c.CurrentState.State
                                            });
        }
    }
}
