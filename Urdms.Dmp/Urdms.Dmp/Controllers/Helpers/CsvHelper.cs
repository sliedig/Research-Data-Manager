using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Entities.Components;

namespace Urdms.Dmp.Controllers.Helpers
{
    public interface ICsvHelper
    {
        string ExportToCsv(DataTable dataTable, string separator = CsvHelper.Separator);
        DataTable DataManagementPlanToDataTable(DataManagementPlan dmp, IList<ProjectParty> projectParties, string listSeparator = CsvHelper.ListSeparator);
        DataTable DataManagementPlansToDataTable(IList<Project> projects, string listSeparator = CsvHelper.ListSeparator);
        DataTable DataCollectionToDataTable(DataCollection dataCollection, string listSeparator = CsvHelper.ListSeparator);
        DataTable DataCollectionsToDataTable(IList<DataCollection> dataCollections, string listSeparator = CsvHelper.ListSeparator);
    }

    public class CsvHelper : ICsvHelper
    {
        public const string Separator = "|";
        public const string ListSeparator = "%";

        public string ExportToCsv(DataTable dataTable, string separator = Separator)
        {
            if (string.IsNullOrWhiteSpace(separator))
            {
                separator = Separator;
            }
            var fileData = new StringBuilder();
            var columns = dataTable.Columns.Cast<DataColumn>();
            var rows = dataTable.Rows.Cast<DataRow>();
            var headers = string.Join(separator, columns.Select(col => col.ColumnName).ToArray());
            var data = GetRowData(rows, columns).ToList();

            fileData.AppendLine(headers);
            fileData.AppendLine(string.Join(separator, data));
            return fileData.ToString();
        }

        public DataTable DataManagementPlanToDataTable(DataManagementPlan dmp, IList<ProjectParty> projectParties, string listSeparator = ListSeparator)
        {
            var dataTable = new DataTable();
            dataTable.AddDmpColumns(dmp);
            var dataRow = dataTable.NewRow();
            dataRow.AddDmpValues(dmp, projectParties, listSeparator);
            dataTable.Rows.Add(dataRow);
            return dataTable;
        }

        public DataTable DataManagementPlansToDataTable(IList<Project> projects, string listSeparator = CsvHelper.ListSeparator)
        {
            var dataTable = new DataTable();
            dataTable.AddDmpColumns(projects.Take(1).Single().DataManagementPlan);
            foreach (var project in projects)
            {
                var dataRow = dataTable.NewRow();
                dataRow.AddDmpValues(project.DataManagementPlan, project.Parties, listSeparator);
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }

        public DataTable DataCollectionToDataTable(DataCollection dataCollection, string listSeparator = ListSeparator)
        {
            var dataTable = new DataTable();
            dataTable.AddDataCollectionColumns(dataCollection);
            var dataRow = dataTable.NewRow();
            dataRow.AddDataCollectionValues(dataCollection, listSeparator);
            dataTable.Rows.Add(dataRow);
            return dataTable;
        }

        public DataTable DataCollectionsToDataTable(IList<DataCollection> dataCollections, string listSeparator = ListSeparator)
        {
            var dataTable = new DataTable();
            dataTable.AddDataCollectionColumns(dataCollections.Take(1).Single());
            foreach (var dataCollection in dataCollections)
            {
                var dataRow = dataTable.NewRow();
                dataRow.AddDataCollectionValues(dataCollection, listSeparator);
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }

        private static IEnumerable<string> GetRowData(IEnumerable<DataRow> rows, IEnumerable<DataColumn> columns)
        {
            foreach (var row in rows)
            {
                var values = new List<string>();
                Debug.Assert(columns != null, "columns != null");
                foreach (var column in columns)
                {
                    values.Add(row.IsNull(column) ? "" : row[column].ToString());
                }
                yield return string.Join(Separator, values);
            }
        }
    }

    internal static class CsvHelperExtensions
    {
        private const string CreationDateColumn = "CreationDate";
        private const string ProjectAccessRolesColumn = "AccessRoles";

        internal static void AddDmpColumns(this DataTable dataTable, DataManagementPlan dmp)
        {
            dataTable.AddColumns(false, typeof(DataStorage), typeof(NewDataDetail),
                                 typeof(ExistingDataDetail), typeof(DataDocumentation), typeof(Ethic), typeof(Confidentiality),
                                 typeof(BackupPolicy), typeof(DataRetention), typeof(DataSharing), typeof(DataRelationshipDetail), typeof(DataOrganisation));

            dataTable.Columns.Add(CreationDateColumn, typeof(DateTime));
            dataTable.Columns.Add(ProjectAccessRolesColumn, typeof(string));
        }

        internal static void AddDmpValues(this DataRow dataRow, DataManagementPlan dmp, IList<ProjectParty> projectParties, string listSeparator)
        {
            dataRow.AddValues(listSeparator, false, dmp.DataStorage, dmp.NewDataDetail,
                              dmp.ExistingDataDetail, dmp.DataDocumentation, dmp.Ethic, dmp.Confidentiality,
                              dmp.BackupPolicy, dmp.DataRetention, dmp.DataSharing, dmp.DataRelationshipDetail);
            dataRow[CreationDateColumn] = dmp.CreationDate;
            dataRow[ProjectAccessRolesColumn] = ((IList)projectParties).ToStringValue(listSeparator);
        }

        internal static void AddDataCollectionColumns(this DataTable dataTable, DataCollection dataCollection)
        {
            dataTable.AddColumns(false, typeof(DataCollection));
            dataTable.Columns.Add(CreationDateColumn, typeof(DateTime));
        }

        internal static void AddDataCollectionValues(this DataRow dataRow, DataCollection dataCollection, string listSeparator)
        {
            dataRow.AddValues(listSeparator, false, dataCollection);
        }

        private static void AddColumns(this DataTable dataTable, bool prependColumnWithObjectType = false, params Type[] types)
        {
            foreach (var type in types)
            {
                var properties = type.GetProperties();
                var prepend = type.Name;
                foreach (var pi in properties)
                {
                    Type dataType;
                    var name = prependColumnWithObjectType ? prepend + "_" + pi.Name : pi.Name;
                    var pt = pi.PropertyType;
                    if (pt.IsEnum)
                    {
                        dataType = typeof(string);
                    }
                    else if (pt.IsGenericType)
                    {
                        if (pt.GetGenericTypeDefinition() == typeof(IList<>) || pt.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            dataType = typeof(string);
                        }
                        else if (pt.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            dataType = pi.PropertyType.GetGenericArguments().First();
                        }
                        else
                        {
                            dataType = typeof(string);
                        }
                    }
                    else
                    {
                        dataType = pi.PropertyType;
                    }
                    var dc = new DataColumn { ColumnName = name, DataType = dataType };
                    dataTable.Columns.Add(dc);
                }
            }
        }

        private static void AddValues(this DataRow dataRow, string listSeparator, bool prependColumnWithObjectType = false, params object[] args)
        {
            foreach (var arg in args.Where(obj => obj != null))
            {
                var properties = arg.GetType().GetProperties();
                var prepend = arg.GetType().Name;
                foreach (var pi in properties)
                {
                    var name = prependColumnWithObjectType ? prepend + "_" + pi.Name : pi.Name;
                    object value = null;
                    var pt = pi.PropertyType;
                    if (pt.IsGenericType
                        && (pt.GetGenericTypeDefinition() == typeof(IList<>) || pt.GetGenericTypeDefinition() == typeof(List<>)))
                    {
                        var list = (IList)pi.GetValue(arg, null);
                        if (list != null && list.Count != 0)
                        {
                            value = list.ToStringValue(listSeparator);
                        }
                    }
                    else
                    {
                        value = pi.GetValue(arg, null);
                    }
                    if (value != null)
                    {
                        dataRow[name] = value.GetType().IsEnum ? value.ToString() : value;
                    }
                }
            }
        }

        private static string ToStringValue(this IList list, string separator)
        {
            if (list is IList<DataCollectionSocioEconomicObjective> || list is IList<DataCollectionFieldOfResearch> ||
                list is IList<ProjectSocioEconomicObjective> || list is IList<ProjectFieldOfResearch>)
            {
                var classification = list.Cast<ClassificationBase>();
                return string.Join(separator, classification.Select(o => o.Code.Id).ToArray());
            }
            if (list is IList<ProjectParty>)
            {
                var projectParties = (IList<ProjectParty>)list;
                return string.Join(separator, projectParties.Select(o => o.Party.FullName + "_" + o.Role).ToArray());
            }
            if (list is IList<DataCollectionParty>)
            {
                var collectionParties = (IList<DataCollectionParty>)list;
                return string.Join(separator, collectionParties.Select(o => o.Party.FullName + "_" + o.Relationship).ToArray());
            }
            return null;
        }

    }
}