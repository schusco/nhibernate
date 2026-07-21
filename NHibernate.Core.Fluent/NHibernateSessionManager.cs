using FluentNHibernate.Cfg;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using System.Reflection;

namespace NHibernate.Core.Fluent
{
    public class NHibernateSessionManager : NHibernateSessionManagerBase
    {
        private static FluentConfiguration GetFluentConfiguration<TDialect, TDriver>(Assembly mappingAssembly, Action<Configuration>? cfg = null, string? connectionString = null)
            where TDialect : Dialect.Dialect where TDriver : IDriver
        {
            var config = GetConfiguration<TDialect, TDriver>(cfg, connectionString, false);
            var fluentConfig = Fluently.Configure(config).Mappings(m => m.FluentMappings.AddFromAssembly(mappingAssembly));
            fluentConfig.ExposeConfiguration(c => cfg?.Invoke(c));
            return fluentConfig;
        }
        public static ISessionFactory ConfigureFluently(Assembly mappingAssembly, Action<Configuration>? cfg = null, string? connectionString = null)
        {
            SessionFactory = GetFluentConfiguration<MySQL55Dialect, MySqlDataDriver>(mappingAssembly, cfg, connectionString).BuildSessionFactory();
            return SessionFactory;
        }
        public static ISessionFactory ConfigureFluentlyPostgres(Assembly mappingAssembly, Action<Configuration>? cfg = null, string? connectionString = null)
        {
            SessionFactory = GetFluentConfiguration<PostgreSQLDialect, NpgsqlDriver>(mappingAssembly, cfg, connectionString).BuildSessionFactory();
            return SessionFactory;
        }
        public static ISessionFactory ConfigureFluentlySqlServer(Assembly mappingAssembly, Action<Configuration>? cfg = null, string? connectionString = null)
        {
            SessionFactory = GetFluentConfiguration<MsSql2012Dialect, MicrosoftDataSqlClientDriver>(mappingAssembly, cfg, connectionString).BuildSessionFactory();
            return SessionFactory;
        }
        //public static ISessionFactory ConfigureFluentlyOracle(Assembly mappingAssembly, Action<Configuration>? cfg = null, string? connectionString = null)
        //{
        //    SessionFactory = GetFluentConfiguration<Oracle10gDialect, OracleManagedDataClientDriver>(mappingAssembly, cfg, connectionString).BuildSessionFactory();
        //    return SessionFactory;
        //}
        public static ISessionFactory ConfigureFluentlySQLite(Assembly mappingAssembly, Action<Configuration>? cfg = null, string? connectionString = null)
        {
            SessionFactory = GetFluentConfiguration<SQLiteDialect, SQLite20Driver>(mappingAssembly, cfg, connectionString).BuildSessionFactory();
            return SessionFactory;
        }
        public static ISessionFactory ConfigureFluently<TDialect, TDriver>(Assembly mappingAssembly, Action<Configuration>? cfg = null, string? connectionString = null)
            where TDialect : Dialect.Dialect where TDriver : IDriver
        {
            SessionFactory = GetFluentConfiguration<TDialect, TDriver>(mappingAssembly, cfg, connectionString).BuildSessionFactory();
            return SessionFactory;
        }
    }
}
