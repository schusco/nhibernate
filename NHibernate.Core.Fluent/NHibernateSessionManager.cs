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
            var config = GetConfiguration<TDialect, TDriver>(cfg, connectionString);
            return Fluently.Configure(config).Mappings(m => m.FluentMappings.AddFromAssembly(mappingAssembly));
        }
        public static ISessionFactory ConfigureFluently(Assembly mappingAssembly, Action<Configuration>? cfg = null, string? connectionString = null)
        {
            SessionFactory = GetFluentConfiguration<MySQL55Dialect, MySqlDataDriver>(mappingAssembly, cfg, connectionString).BuildSessionFactory();
            return SessionFactory;
        }
        public static ISessionFactory ConfigureFluentlyPostgres(Assembly mappingAssembly, Action<Configuration>? cfg = null, string? connectionString = null)
        {
            SessionFactory = GetFluentConfiguration<PostgreSQLDialect, NpgsqlDriver>(mappingAssembly, cfg).BuildSessionFactory();
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
