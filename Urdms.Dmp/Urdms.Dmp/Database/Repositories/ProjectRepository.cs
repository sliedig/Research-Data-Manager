using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Database.Repositories
{
    public interface IProjectRepository
    {
        IList<Project> GetAll();
        IList<Project> GetAllWhichFailedProvisioning();
        Project Get(int id);
        IList<Project> GetByPrincipalInvestigator(string curtinId);
        Project GetByDataDepositId(int dataDepositId);
        Project GetByDataManagementPlanId(int dataManagementPlanId);
        void Save(Project project);
    }

    public class ProjectRepository : IProjectRepository
    {
        private readonly ISession _session;

        public ProjectRepository(ISession session)
        {
            _session = session;
        }

        public IList<Project> GetAll()
        {
            return _session.Query<Project>().ToList();
        }

        public IList<Project> GetAllWhichFailedProvisioning()
        {
            var statuses = new[] {ProvisioningStatus.Error, ProvisioningStatus.TimeOut};

            return _session.QueryOver<Project>()
                .WhereRestrictionOn(p => p.ProvisioningStatus).IsIn(statuses)
                .List();
        }
        
        public Project Get(int id)
        {
            return _session.Get<Project>(id);
        }

        public Project GetByDataDepositId(int dataDepositId)
        {
            var entity = _session.QueryOver<Project>()
                .Where(p => p.DataDeposit.Id == dataDepositId)
                .Take(1)
                .SingleOrDefault();
            return entity;
        }

        public Project GetByDataManagementPlanId(int dataManagementPlanId)
        {
            var entity = _session.QueryOver<Project>()
                .Where(p => p.DataManagementPlan.Id == dataManagementPlanId)
                .Take(1)
                .SingleOrDefault();
            return entity;
        }

        public IList<Project> GetByPrincipalInvestigator(string curtinId)
        {
            if (string.IsNullOrWhiteSpace(curtinId))
                return new List<Project>();

            return _session.QueryOver<Project>()
                .JoinQueryOver<ProjectParty>(p => p.Parties)
                .Where(p => p.Role == AccessRole.Owners)
                .Inner.JoinQueryOver(p => p.Party)
                .Where(m => m.UserId == curtinId)
                .List();
        }
        
        private void EnsureNonDuplicationOfParties(IEnumerable<ProjectParty> projectParties)
        {
            var newUrdmsParties = (from o in projectParties
                                    where o.Party.Id < 1 && !string.IsNullOrWhiteSpace(o.Party.UserId)
                                    select o.Party).ToList();
            if (newUrdmsParties.Count != 0)
            {
                var parties = _session.QueryOver<Party>().List();

                foreach (var newUrdmsParty in newUrdmsParties)
                {
                    var key = newUrdmsParty.UserId;
                    var existingParty = parties.Where(o => o.UserId == key).Take(1).FirstOrDefault();
                    if (existingParty != null)
                    {
                        existingParty.FirstName = newUrdmsParty.FirstName;
                        existingParty.LastName = newUrdmsParty.LastName;
                        existingParty.FullName = newUrdmsParty.FullName;
                        existingParty.Email = newUrdmsParty.Email;
                        var projectParty = projectParties.Single(o => o.Party.UserId == key);
                        projectParty.Party = existingParty;
                    }
                }
            }
        }

        public void Save(Project project)
        {
            EnsureNonDuplicationOfParties(project.Parties);

            _session.SaveOrUpdate(project);
            _session.Flush();
        }
    }
}

