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

namespace BulkInsertUpdateExamples
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConfigureSerilog();         

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
    }
}
