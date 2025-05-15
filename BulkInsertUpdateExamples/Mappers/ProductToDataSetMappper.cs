using System;
using System.Collections.Generic;
using System.Data;
using BulkInsertUpdateExamples.Models;

namespace BulkInsertExampleEF
{ 
    public class ProductToDataSetMapper 
    {
       public DataTable GetDataSetFroDtos(IEnumerable<Product> productItems)
        {
            var dataTable = this.GetEmptyDataTable();

            foreach (var item in productItems)
            {
                DataRow row = GetDataRow(dataTable, item);
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        private static DataRow GetDataRow(DataTable dataTable, Product item)
        {
            DataRow row = dataTable.NewRow();
            row["Id"] = item.Id;
            row["Name"] = item.Name;
            row["Price"] = item.Price;
        
            return row;        
        }  
       
       private DataTable GetEmptyDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(int));  
            table.Columns.Add("Name", typeof(string));  
            table.Columns.Add("Price", typeof(decimal));  
         
            return table;
        }
    } 
}      
