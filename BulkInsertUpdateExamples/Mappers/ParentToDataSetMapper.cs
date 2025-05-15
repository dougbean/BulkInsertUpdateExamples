using System.Collections.Generic;
using System.Data;
using BulkInsertUpdateExamples.Models;

namespace BulkInsertExampleEF
{
    public class ParentToDataSetMapper
    {
        public DataTable GetDataSetFroDtos(IEnumerable<Parent> parentItems)
        {
            var dataTable = this.GetEmptyDataTable();

            foreach (var item in parentItems)
            {
                DataRow row = GetDataRow(dataTable, item);
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        private static DataRow GetDataRow(DataTable dataTable, Parent item)
        {
            DataRow row = dataTable.NewRow();

            row["Id"] = item.Id;
            row["Name"] = item.Name;
            return row;
        }
               
        private DataTable GetEmptyDataTable()
        {
            DataTable table = new DataTable();

            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));

            return table;
        }
    }
}
