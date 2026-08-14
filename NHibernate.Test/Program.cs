using NHibernate;
using NHibernate.Core.Attributes;
using NHibernate.Test;

ISessionFactory factory;
if (args.Length == 0)
    factory = ConfigureMySql();
else
{
    switch (args[0].ToLower())
    {
        case "mysql":
            factory = ConfigureMySql();
            break;
        case "postgres":
            factory = ConfigurePostGres();
            break;
        case "sqlserver":
            factory = ConfigureSqlServer();
            break;
        case "sqlite":
            factory = ConfigureSqlLite();
            break;
        default:
            Console.WriteLine($"Unknown database type: {args[0]}");
            return;
    }
}
var dbSession = factory.OpenSession();
Console.WriteLine("Setting Initial data in baseball_test table:");
SeedData(dbSession);
Console.WriteLine("Initial data in baseball_test table:");
var test = dbSession.QueryOver<BaseballTest>().List();
foreach (var item in test)
{
    Console.WriteLine($"Id: {item.Id}, Team: {item.Team}, Wins: {item.Wins}");
}
var newTest = new BaseballTest { Team = "Sox", Wins = 5 };
Console.WriteLine($"Adding new team: {newTest.Team} with Wins: {newTest.Wins}");
dbSession.Save(newTest);
Console.WriteLine();
Console.WriteLine("Updating team: Sox to Wins: 10");
newTest.Wins = 10;
dbSession.Update(newTest);
Console.WriteLine();
var updatedTest = dbSession.QueryOver<BaseballTest>().List();
Console.WriteLine("updated data in baseball_test table:");
foreach (var item in updatedTest)
{
    Console.WriteLine($"Id: {item.Id}, Team: {item.Team}, Wins: {item.Wins}");
}
Console.WriteLine();
Console.WriteLine("Deleting team: Blazers");
using (var trx = dbSession.BeginTransaction())
{
    var deleteTest = dbSession.QueryOver<BaseballTest>().Where(x => x.Team == "Blazers").SingleOrDefault();
    if (deleteTest != null)
    {
        dbSession.Delete(deleteTest);
    }
    trx.Commit();
}
var finalCount = dbSession.QueryOver<BaseballTest>().RowCount();
Console.WriteLine($"Count of teams in baseball_test table: {finalCount}");
Console.WriteLine();
Console.WriteLine("Deleting seed data");
DeleteData(dbSession);
Console.WriteLine("Press any key to exit...");
Console.ReadKey();

ISessionFactory ConfigureMySql()
{
    return NHibernateSessionManager.ConfigureWithAttributes(typeof(Program).Assembly, connectionString: "Server=localhost;Database=my_first_app_development;Username=pi;Password=test;");
}
ISessionFactory ConfigurePostGres()
{
    return NHibernateSessionManager.ConfigureWithAttributesPostgres(typeof(Program).Assembly, connectionString: "Host=192.168.1.88;Port=5432;Database=test;Username=test;Password=test");
}
ISessionFactory ConfigureSqlServer()
{
    var connectionString = "Data Source=localhost;Persist Security Info=False;User ID=schusco;Password=test;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name=\"NHibernate.Test\";Command Timeout=0";
    return NHibernateSessionManager.ConfigureWithAttributesSqlServer(typeof(Program).Assembly, connectionString: connectionString);
}
ISessionFactory ConfigureSqlLite()
{
    return NHibernateSessionManager.ConfigureWithAttributesSQLite(typeof(Program).Assembly, connectionString: "Data Source=test.db;Version=3;");
}
//void SqlLiteConfig(Configuration cfg)
//{
//    new NHibernate.Tool.hbm2ddl.SchemaExport(cfg).Create(false, true);
//}
void SeedData(ISession session)
{
    var test1 = new BaseballTest { Team = "Electrons", Wins = 15 };
    var test2 = new BaseballTest { Team = "Hounds", Wins = 12 };
    var test3 = new BaseballTest { Team = "Blazers", Wins = 8 };
    session.Save(test1);
    session.Save(test2);
    session.Save(test3);
}
void DeleteData(ISession session)
{
    using var trx = session.BeginTransaction();
    var test = session.QueryOver<BaseballTest>().List();
    foreach (var team in test)
        session.Delete(team);
    trx.Commit();
}
