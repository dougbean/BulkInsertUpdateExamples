using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Linq;
using Serilog;
using System.Text.Json;
using System.Configuration;
using BulkInsertExampleEF;
using BulkInsertUpdateExamples.Data;
using BulkInsertUpdateExamples.Models;
using BulkInsertUpdateExamples.Mappers;

namespace BulkInsertUpdateExamples
{
    internal class Program
    {  
        static void Main(string[] args)
        {
            ConfigureSerilog();
            string connectionString = ConfigurationManager.ConnectionStrings["database"].ConnectionString;
           
            //BulkInsertProductRecords();
            //BulkUpdateProducts();
            //BulkInsertParentChildTables();
            //BulkInsertUseSqlBulkCopy(connectionString);

            Console.WriteLine("press any key..");
            Console.ReadKey();
        }

        private static void ConfigureSerilog()
        {
            Log.Logger = new LoggerConfiguration()
                            .WriteTo.Console()
                            .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                            .CreateLogger();
        }

        //bulk product insert//
        /// <summary>
        /// Eleven seconds to insert one million records
        /// 2025-05-16 10:32:02.638 -05:00 [INF] start Inserting 1000000 records - 5/16/2025 10:32:02 AM
        //  2025-05-16 10:32:13.303 -05:00 [INF] end Inserting 1000000 records - 5/16/2025 10:32:13 AM
        /// </summary>
        private static void BulkInsertProductRecords()
        {
            int numberOfRecords = 1000000;
            var products = new List<Product>();
            for (int i = 1; i < numberOfRecords; i++)
            {
                var product = new Product() { Name = $"my name{i}", Price = i };
                products.Add(product);
            }

            var mapper = new ProductToDataSetMapper();
            DataTable dataTable = mapper.GetDataSetFroDtos(products);

            var parameter = new SqlParameter("@batch", SqlDbType.Structured);
            parameter.Value = dataTable;
            parameter.TypeName = "dbo.udt_product";

            using (var db = new SandboxDbContext())
            {
                string msg = $"start Inserting {numberOfRecords} records - {DateTime.Now.ToString()}";
                Log.Information(msg);
               
                var productList = db.Products.FromSqlRaw<Product>("exec dbo.usp_load_product @batch", parameter).ToList();
                
                msg = $"end Inserting {numberOfRecords} records - {DateTime.Now.ToString()}";
                Log.Information(msg);
            }
        }
        //bulk product insert//

        //bulk product update//
        /// <summary>
        /// Six seconds to update one million records
        /// 2025-05-16 10:44:53.346 -05:00 [INF] start updating 1000000 records - 5/16/2025 10:44:53 AM
        /// 2025-05-16 10:44:59.928 -05:00 [INF] end updating 1000000 records - 5/16/2025 10:44:59 AM
        /// </summary>
        private static void BulkUpdateProducts()
        {
            var products = GetProducts();
            int numberOfRecords = products.Count();
            int count = 0;
            foreach (var product in products)
            {
                count++;
                product.Price = (product.Price + count);
            }

            var mapper = new ProductToDataSetMapper();
            DataTable dataTable = mapper.GetDataSetFroDtos(products);

            var parameter = new SqlParameter("@batch", SqlDbType.Structured);
            parameter.Value = dataTable;
            parameter.TypeName = "dbo.udt_product";

            using (var db = new SandboxDbContext())
            {
                string msg = $"start updating {numberOfRecords} records - {DateTime.Now.ToString()}";
                Log.Information(msg);

                db.Database.ExecuteSqlRaw("exec dbo.usp_update_product @batch", parameter);

                msg = $"end updating {numberOfRecords} records - {DateTime.Now.ToString()}";
                Log.Information(msg);
            }
        }
        //bulk product update//

        private static IEnumerable<Product> GetProducts()
        {
            var products = new List<Product>();
            using (var context = new SandboxDbContext())
            {
                products = context.Products.ToList();
            }
            return products;
        }

        //bulk insert Parent Child records with transaction scope//
        /// <summary>
        /// Twenty seconds to insert one million parent and one million child records.
        /// 2025-05-16 11:51:10.928 -05:00 [INF] start Inserting 1000000 Parent records - 5/16/2025 11:51:10 AM
        /// 2025-05-16 11:51:10.966 -05:00 [INF] Populate Child records with ParentId as a foreign key. - 5/16/2025 11:51:10 AM
        /// 2025-05-16 11:51:24.392 -05:00 [INF] start Inserting Child records with ParentId as a foreign key. - 5/16/2025 11:51:24 AM
        /// 2025-05-16 11:51:30.146 -05:00 [INF] end Inserting Child records with ParentId as a foreign key. - 5/16/2025 11:51:30 AM
        /// </summary>
        private static void BulkInsertParentChildTables()
        {
            int numberOfRecords = 1000000;
            var parents = new List<Parent>();
            for (int i = 0; i < numberOfRecords; i++)
            {
                var parent = new Parent() { Name = $"my name{i}" };
                parents.Add(parent);
            }

            var mapper = new ParentToDataSetMapper();
            DataTable dataTable = mapper.GetDataSetFroDtos(parents);

            var parameter = new SqlParameter("@batch", SqlDbType.Structured);
            parameter.Value = dataTable;
            parameter.TypeName = "dbo.udt_parent";

            using (var db = new SandboxDbContext())
            {                
                using (var transaction = db.Database.BeginTransaction()) 
                {

                    try
                    {
                        string msg = $"start Inserting {numberOfRecords} Parent records - {DateTime.Now.ToString()}";
                        Log.Information(msg);

                        IQueryable<Parent> inserted = db.Parents.FromSqlRaw<Parent>("exec dbo.usp_load_parent @batch", parameter);

                        msg = $"Populate Child records with ParentId as a foreign key. - {DateTime.Now.ToString()}";
                        Log.Information(msg);

                        var children = new List<Child>();
                        int index = 0;
                        foreach (var parent in inserted)
                        {                            
                            var child = new Child() { ParentId = parent.Id, Name = $"my name child - {index}" };
                            children.Add(child);
                            index++;
                        }

                        var childMapper = new ChildToDataSetMapper();
                        DataTable dataTableChildren = childMapper.GetDataSetFromDtos(children);

                        var sqlParameter = new SqlParameter("@batch", SqlDbType.Structured);
                        sqlParameter.Value = dataTableChildren;
                        sqlParameter.TypeName = "dbo.udt_child";

                        msg = $"start Inserting Child records with ParentId as a foreign key. - {DateTime.Now.ToString()}";
                        Log.Information(msg);

                        db.Database.ExecuteSqlRaw("exec dbo.usp_load_child @batch", sqlParameter);

                        msg = $"end Inserting Child records with ParentId as a foreign key. - {DateTime.Now.ToString()}";
                        Log.Information(msg);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Log.Error(ex.ToString());
                    }
                }
            }
        }
        //bulk insert Parent Child records with transaction scope//

        //use SqlBulkCopy for bulk insert//
        /// <summary>
        /// Two seconds to insert one million records, using SqlBulkCopy       
        /// 2025-05-16 14:51:02.409 -05:00 [INF] start sqlBulkCopy - 1000000 Product records - 5/16/2025 2:51:02 PM
        /// 2025-05-16 14:51:04.817 -05:00 [INF] end sqlBulkCopy - 1000000 Product records - 5/16/2025 2:51:04 PM
        /// </summary>
        private static void BulkInsertUseSqlBulkCopy(string connectionString)
        {
            int numberOfRecords = 10000;
            var products = new List<Product>();
            for (int i = 1; i < numberOfRecords; i++)
            {
                var product = new Product() { Name = $"my name{i}", Price = i };
                products.Add(product);
            }

            var mapper = new ProductToDataSetMapper();
            DataTable dataTable = mapper.GetDataSetFroDtos(products);
           
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
            {
                string msg = $"start sqlBulkCopy - {numberOfRecords} Product records - {DateTime.Now.ToString()}";
                Log.Information(msg);

                // The table I'm loading the data to  
                bulkCopy.DestinationTableName = "Product";

                // How many records to send to the database in one go (all of them)  
                bulkCopy.BatchSize = dataTable.Rows.Count;

                // Load the data to the database  
                bulkCopy.WriteToServer(dataTable);               

                // Close up            
                bulkCopy.Close();

                msg = $"end sqlBulkCopy - {numberOfRecords} Product records - {DateTime.Now.ToString()}";
                Log.Information(msg);
            }
        }
        //use SqlBulkCopy for bulk insert//
    }
}
