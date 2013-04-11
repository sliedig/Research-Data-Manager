using System.Linq;
using System.Web.Mvc;
using Urdms.Dmp.Controllers.Helpers;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Models.DataDeposit;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Controllers.ModelBinders
{
    public class DataDepositViewModelBinder : DefaultModelBinder
    {
        protected IProjectRepository ProjectRepository {get; set; }

        public DataDepositViewModelBinder()
        {
            ProjectRepository = DependencyResolver.Current.GetService<IProjectRepository>();
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var form = controllerContext.HttpContext.Request.Form;
            var model = new DataDepositViewModel();
            model.DeserializeUrdmsUsers<AccessRole>(form);
            model.DeserializeNonUrdmsUsers<AccessRole>(form);
            int projectId;
            if (int.TryParse(form["ProjectId"], out projectId))
            {
                var project = ProjectRepository.Get(projectId);
                model.PrincipalInvestigator = project.Parties
                    .Where(
                        p =>
                        !string.IsNullOrWhiteSpace(p.Party.UserId) &&
                        p.Relationship == ProjectRelationship.PrincipalInvestigator)
                    .Single()
                    .Party;
            }
            bindingContext.ModelMetadata.Model = model;
            return base.BindModel(controllerContext, bindingContext); 
        }
    }
}