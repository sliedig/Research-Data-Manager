using System;
using System.Configuration;
using System.Data.SqlClient;
using NUnit.Framework;
using Urdms.DocumentBuilderService.Database.Repositories;
using Urdms.DocumentBuilderService.Models.Enums;

namespace DocumentBuilderService.Tests
{
    /// <summary>
    /// Summary description for ReposityTests
    /// </summary>
    [TestFixture]
    public class RepositoryShould
    {
        private DataManagementPlanRepository _repository;
       
        [TestFixtureSetUp]
        public void Setup()
        {
            _repository = new DataManagementPlanRepository();
        }
        
        [Test]
        [Ignore("Integration test not working on build.")]
        public void Get_a_empty_data_management_plan_for_a_invalid_project_id()
        {
            var dmp = _repository.GetDataManagementPlanByProjectId(-1);
            Assert.That(dmp, Is.Not.Null, "Could not read Data Management Plan or Project tables.");
        }

        // Todo: Fix this test so it works on CI
        [Ignore("The test is looking for a existing project with Urdms and Non-Urdms users, breaking the build on CI")]
        [Test]
        public void Get_a_get_principal_investigator_for_a_valid_project_id()
        {
            var dmp = _repository.GetDataManagementPlanByProjectId(1);
            Assert.That(dmp, Is.Not.Null, "Could not read Data Management Plan or Project tables.");
            Assert.That(dmp.UrdmsUsers, Is.Not.Null, "Urdms users not found for Project tables.");
            Assert.That(dmp.NonUrdmsUsers, Is.Not.Null, "Non-urdms users not found for Project tables.");
            var user = GetPrincipalInvestigatorFromProject(1);
            Assert.That(dmp.PricipalInvestigator.Equals(user, StringComparison.CurrentCultureIgnoreCase));
        }

        private string GetPrincipalInvestigatorFromProject(int id)
        {
            var connectionString = ConfigurationManager.AppSettings["DmpDBConnection"];

             const string queryString = "SELECT Party.FullName"
                                        + " FROM Party INNER JOIN ProjectParty ON Party.Id = ProjectParty.PartyId "
                                        + " WHERE (ProjectParty.ProjectId = {0}) AND (ProjectParty.Relationship = {1})";
            
            var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
           
            var sqlCommand =
                new SqlCommand(string.Format(queryString, id, (int)ProjectRelationship.PrincipalInvestigator),
                                sqlConnection);
            var sqlReader = sqlCommand.ExecuteReader();
            while (sqlReader.Read())
            {
                return (string) sqlReader["FullName"];
            }
            sqlReader.Close();
            
            return null;
        }
    }
}
