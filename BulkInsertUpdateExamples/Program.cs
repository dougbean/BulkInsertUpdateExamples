// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Linq;
using Serilog;
using System.Text.Json;
using System.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Extensions.Configuration;
using BulkInsertExampleEF;
using BulkInsertUpdateExamples.Data;
using BulkInsertUpdateExamples.Models;
using System.Runtime.InteropServices;

namespace BulkInsertUpdateExamples
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConfigureSerilog();            

            //BulkInsertProductRecords();
            UpdateProducts();

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
        /// Eleven seconds to  insert one million records
        /// 2025-05-16 10:32:02.638 -05:00 [INF] start Inserting 1000000 records - 5/16/2025 10:32:02 AM
        //  2025-05-16 10:32:13.303 -05:00 [INF] end Inserting 1000000 records - 5/16/2025 10:32:13 AM
        /// </summary>
        private static void BulkInsertProductRecords()
        {
            int numberOfRecords = 1000000;//one million records
            var products = new List<Product>();
            for (int i = 0; i < numberOfRecords; i++)
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
        private static void UpdateProducts()
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
    }
}
