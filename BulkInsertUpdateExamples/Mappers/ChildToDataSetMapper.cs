using BulkInsertUpdateExamples.Models;
using System.Collections.Generic;
using System.Data;

namespace BulkInsertUpdateExamples.Mappers
{
    public class ChildToDataSetMapper
    {
        public DataTable GetDataSetFromDtos(IEnumerable<Child> childItems)
        {
            var dataTable = this.GetEmptyDataTable();

            foreach (var item in childItems)
            {
                DataRow row = GetDataRow(dataTable, item);
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        private static DataRow GetDataRow(DataTable dataTable, Child item)
        {
            DataRow row = dataTable.NewRow();

            row["Id"] = item.Id;
            row["ParentId"] = item.ParentId;
            row["Name"] = item.Name;
            return row;
        }
      
        private DataTable GetEmptyDataTable()
        {
            DataTable table = new DataTable();

            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("ParentId", typeof(int));
            table.Columns.Add("Name", typeof(string));

            return table;
        }
    }
}
