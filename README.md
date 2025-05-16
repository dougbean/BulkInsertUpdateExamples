BulkInsertUpdateExamples

This project demonstrates an old school database technique for Inserts and Updates of large datasets to sql server, using stored procedures with left joins, user defined datatables, and System.Data.Datatable.

One can use SqlBulkCopy for bulk inserts, but this old school technique is more flexible. For example, one can return the primary keys of the records that are inserted.

I adapted this old school technique to use Entity Framework.

I've written T4 templates to generate the stored procedures, the user defined tables and mapper classes, but they need some work before I would check them in.

I'm using database first rather that model first for Entity Framework.
I used the following command to generate the DBContext class and the model classes.<br><br>
PM>  Scaffold-DbContext "Server=YOURSERVER;Database=SandboxDB;Trusted_Connection=True;TrustServerCertificate=True;"  Microsoft.EntityFrameworkCore.SqlServer -f

One can use the following in a Powershell command prompt to test a connection to Sql Server.

$conn = New-Object System.Data.SqlClient.SqlConnection<br>
$conn.ConnectionString = "Data Source=YOURSERVER;Initial Catalog=SandboxDB;User ID=xxxx;Password=xxxxxxx;TrustServerCertificate=True;"<br>
$conn.Open()<br>
$conn.State<br>
$conn.Close()<br>

