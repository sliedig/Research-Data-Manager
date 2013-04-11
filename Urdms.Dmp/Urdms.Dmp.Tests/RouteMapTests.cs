using System.Web.Routing;
using Curtin.Framework.Test.Web;
using Urdms.Dmp.Web;
using Urdms.Dmp.Web.Controllers;
using NUnit.Framework;
using Urdms.Dmp.Controllers;

namespace Urdms.Dmp.Tests
{
    [TestFixture]
    public class RouteMapShouldMapTo
    {
        [SetUp]
        public void SetUp()
        {
            RouteTable.Routes.Clear();
            AppRoutes.Register(RouteTable.Routes);
            UrdmsApplication.RegisterDefaultRoutes(RouteTable.Routes);
        }

        [Test]
        public void Missing_controller()
        {
            "~/SomeRandomController/".Route().ShouldMapTo<ErrorController>(c => c.NotFound());
        }

        [Test]
        public void Homepage()
        {
            "~/".Route().ShouldMapTo<PageController>(c => c.Index());
        }

        [Test]
        public void Instructions()
        {
            "~/Instructions".Route().ShouldMapTo<PageController>(c => c.Instructions());
        }

        [Test]
        public void Project_introduction()
        {
            "~/Project/Introduction".Route().ShouldMapTo<ProjectController>(c => c.Introduction());
        }
        
        [Test]
        public void New_dmp()
        {
            "~/Project/3134/Dmp/New".Route().ShouldMapTo<DmpController>(c => c.New(3134));
        }

        [Test]
        public void Step1_to_dmp_edit()
        {
            "~/Project/Dmp/Edit/1/3".Route().ShouldMapTo<DmpController>(c => c.Edit(1, 3));
        }

        [Test]
        public void Dmp_review_page()
        {
            "~/Project/Dmp/Review/1".Route().ShouldMapTo<ConfirmController>(c => c.Review(1));
        }

        [Test]
        public void Dmp_submitted_page()
        {
            "~/Project/Dmp/Submitted/1".Route().ShouldMapTo<ConfirmController>(c => c.Submitted(1));
        }

        [Test]
        public void Data_deposit_review_page()
        {
            "~/DataDepositProject/1000/DataDeposit/Review".Route().ShouldMapTo<ConfirmController>(c => c.ReviewDataDeposit(1000));
        }

        [Test]
        public void Data_deposit_submitted_page()
        {
            "~/DataDepositProject/1000/DataDeposit/Submitted".Route().ShouldMapTo<ConfirmController>(c => c.SubmittedDataDeposit(1000));
        }

        [Test]
        public void My_projects()
        {
            "~/Projects".Route().ShouldMapTo<ProjectController>(c => c.Index());
        }

        [Test]
        public void New_projects()
        {
            "~/Projects/New".Route().ShouldMapTo<ProjectController>(c => c.NewProjects());
        }

        [Test]
        public void New_research_initiated_project()
        {
            "~/Project/New".Route().ShouldMapTo<ProjectController>(c => c.New());
        }

        [Test]
        public void Research_initiated_project()
        {
            "~/Project/".Route().ShouldMapTo<ProjectController>(c => c.Project((int?)null));
        }

        [Test]
        public void Existing_research_initiated_project()
        {
            "~/Project/1".Route().ShouldMapTo<ProjectController>(c => c.Project(1));
        }

        [Test]
        public void Copy_dmp()
        {
            "~/Project/CopyDmp/1".Route().ShouldMapTo<ProjectController>(c => c.CopyDmp(1));
        }

        [Test]
        public void List_collections()
        {
            "~/Project/1/DataCollections".Route().ShouldMapTo<DataCollectionController>(c => c.Index(1));
        }

        [Test]
        public void Collections_new()
        {
            "~/Project/1/DataCollection/New".Route().ShouldMapTo<DataCollectionController>(c => c.New(1));
        }

        [Test]
        public void Collections_Step1_Optional_Id()
        {
            "~/Project/1/DataCollection/Step1/".Route().ShouldMapTo<DataCollectionController>(c => c.Step1(1, null));
        }

        [Test]
        public void Collections_Step1()
        {
            const int id = 1;
            ("~/Project/1/DataCollection/Step1/" + id).Route().ShouldMapTo<DataCollectionController>(c => c.Step1(1, id));
        }

        [Test]
        public void Collections_Step2()
        {
            const int id = 2;
            ("~/Project/1/DataCollection/Step2/" + id).Route().ShouldMapTo<DataCollectionController>(c => c.Step2(1, id));
        }

        [Test]
        public void Ajax()
        {
            "~/Ajax/GetForList/".Route().ShouldMapTo<AjaxController>(c => c.GetForList(""));
        }

        [Test]
        public void AjaxGetUrdmsUser()
        {
            "~/Ajax/GetNewUrdmsUser/EE21168/project".Route().ShouldMapTo<AjaxController>(c => c.GetNewUrdmsUser("EE21168", "project"));
        }

        [Test]
        public void AjaxGetUrdmsUserForApprover()
        {
            "~/Ajax/GetNewUrdmsUserForApproval/EE21168/1".Route().ShouldMapTo<AjaxController>(c => c.GetNewUrdmsUserForApproval("EE21168", 1));
        }
        
        [Test]
        public void Approvals()
        {
            "~/Approvals/".Route().ShouldMapTo<ApprovalController>(c => c.Index());
        }

        [Test]
        public void Approval_confirmation()
        {
            "~/Approvals/Confirm/1".Route().ShouldMapTo<ApprovalController>(c => c.Confirm(1));
        }

        [Test]
        public void Collections_ReadOnly()
        {
            const int id = 1;
            ("~/Project/1/DataCollection/View/" + id).Route().ShouldMapTo<DataCollectionController>(c => c.ViewReadOnlyDataCollection(1, id));
        }


        [Test]
        public void Dmp_confirm_page()
        {
            "~/Project/Dmp/Republish/1".Route().ShouldMapTo<ConfirmController>(c => c.Republish(1));
        }

        [Test]
        public void Data_Deposit_Project()
        {
            "~/DataDepositProject/".Route().ShouldMapTo<DataDepositController>(c => c.Project((int?)null));
        }

        [Test]
        public void Data_Deposit()
        {
            "~/DataDepositProject/1/DataDeposit/".Route().ShouldMapTo<DataDepositController>(c => c.New(1));
        }

        [Test]
        public void Existing_Data_Deposit_Project()
        {
            "~/DataDepositProject/3".Route().ShouldMapTo<DataDepositController>(c => c.Project(3));
        }

        [Test]
        public void Lib_guide_item()
        {
            "~/LibGuide/100".Route().ShouldMapTo<LibGuideController>(c => c.Index(1470, 100));
        }

        [Test]
        public void Republished()
        {
            "~/Project/Dmp/Republished".Route().ShouldMapTo<ConfirmController>(c => c.Republished());
        }

    }

}
