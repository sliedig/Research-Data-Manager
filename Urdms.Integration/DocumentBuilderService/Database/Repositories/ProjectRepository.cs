using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Urdms.DocumentBuilderService.Database.Entities;
using Urdms.DocumentBuilderService.Helpers;
using Urdms.DocumentBuilderService.Models.Enums;

namespace Urdms.DocumentBuilderService.Database.Repositories
{
    public interface IProjectRepository
    {
        Project Get(int id);
    }

    public class ProjectRepository : IProjectRepository
    {
        private readonly SqlConnection _connection;
        private const string CommandText = @"SELECT Id, Title, SourceProjectType FROM Project WHERE Id = @Id";
        public ProjectRepository()
        {
            var connectionString = ConfigurationManager.AppSettings["DmpDBConnection"];
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("DmpDBConnection value could not be found");
            }
            _connection = new SqlConnection(connectionString);
        }

        public Project Get(int id)
        {
            try
            {
                _connection.Open();
                using(var command = _connection.CreateCommand())
                {
                    command.CommandText = CommandText;
                    command.Parameters.AddWithValue("Id", id);
                    using(var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var entity = new Project
                                             {
                                                 Id = reader.GetValue<int>("Id"),
                                                 SourceProjectType = reader.GetEnumValue("SourceProjectType", SourceProjectType.None),
                                                 Title = reader.GetStringValue("Title","")
                                             };
                            return entity;
                        }
                        reader.Close();
                    }
                }
                return null;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }
    }
}
