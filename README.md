BulkInsertUpdateExamples

I'm using database first rather that model first for Entity Framework.
I used the following command to generate the DBContext class and the model classes.
PM>  Scaffold-DbContext "Server=YOURSERVER;Database=SandboxDB;Trusted_Connection=True;TrustServerCertificate=True;"  Microsoft.EntityFrameworkCore.SqlServer -f

One can use the following in a Powershell command prompt to test a connection to Sql Server.

$conn = New-Object System.Data.SqlClient.SqlConnection
$conn.ConnectionString = "Data Source=YOURSERVER;Initial Catalog=SandboxDB;User ID=xxxx;Password=xxxxxxx;TrustServerCertificate=True;"
$conn.Open()
$conn.State
$conn.Close()

