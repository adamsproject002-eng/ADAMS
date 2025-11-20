docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=My$tr0ngP@ssw0rd!"  -p 1433:1433 --name sqlserver-adams -d mcr.microsoft.com/mssql/server:2022-latest


dotnet tool uninstall --global dotnet-ef
dotnet tool install --global dotnet-ef --version 8.0.22

dotnet clean
dotnet restore
dotnet build

修改 Model 或 DbContext

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
}

然後在 AppDbContext.cs 中加上：

public DbSet<User> Users { get; set; }


dotnet ef migrations add AddFryRecordPondRelation



