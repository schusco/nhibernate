using FluentNHibernate.Cfg;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using System.Reflection;

namespace NHibernate.Core.Fluent
{
    /// <summary>
    /// Provides configuration and management of NHibernate session factories using Fluent NHibernate for multiple
    /// database dialects.
    /// </summary>
    /// <remarks>Supports MySQL, PostgreSQL, SQL Server, and SQLite. Enables fluent configuration with custom
    /// mappings and settings.</remarks>
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
        /// <summary>
        /// Configures and builds an NHibernate session factory using Fluent NHibernate with the specified mapping.  Defaults to MySQL dialect and driver.  The mapping definitions are expected to be in the provided
        /// assembly.
        /// </summary>
        /// <param name="mappingAssembly">The assembly containing NHibernate mapping definitions.</param>
        /// <param name="cfg">An optional delegate to apply additional configuration settings.</param>
        /// <param name="connectionString">An optional database connection string.</param>
        /// <returns>An initialized ISessionFactory instance.</returns>
        public static ISessionFactory ConfigureFluently(Assembly mappingAssembly, Action<Configuration>? cfg = null, string? connectionString = null)
        {
            SessionFactory = GetFluentConfiguration<MySQL55Dialect, MySqlDataDriver>(mappingAssembly, cfg, connectionString).BuildSessionFactory();
            return SessionFactory;
        }
        /// <summary>
        /// Configures and builds an NHibernate session factory for a PostGresSql database connection using Fluent NHibernate with the specified mapping.  The mapping definitions are expected to be in the provided
        /// assembly.
        /// </summary>
        /// <param name="mappingAssembly">The assembly containing NHibernate mapping definitions.</param>
        /// <param name="cfg">An optional delegate to apply additional configuration settings.</param>
        /// <param name="connectionString">An optional database connection string.</param>
        /// <returns>An initialized ISessionFactory instance.</returns>
        public static ISessionFactory ConfigureFluentlyPostgres(Assembly mappingAssembly, Action<Configuration>? cfg = null, string? connectionString = null)
        {
            SessionFactory = GetFluentConfiguration<PostgreSQLDialect, NpgsqlDriver>(mappingAssembly, cfg, connectionString).BuildSessionFactory();
            return SessionFactory;
        }
        /// <summary>
        /// Configures and builds an NHibernate session factory for a SQL Server database connection using Fluent NHibernate with the specified mapping.  The mapping definitions are expected to be in the provided
        /// assembly.
        /// </summary>
        /// <param name="mappingAssembly">The assembly containing NHibernate mapping definitions.</param>
        /// <param name="cfg">An optional delegate to apply additional configuration settings.</param>
        /// <param name="connectionString">An optional database connection string.</param>
        /// <returns>An initialized ISessionFactory instance.</returns>
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

        /// <summary>
        /// Configures and builds an NHibernate session factory for a SQLLite database connection using Fluent NHibernate with the specified mapping.  The mapping definitions are expected to be in the provided
        /// assembly.
        /// </summary>
        /// <param name="mappingAssembly">The assembly containing NHibernate mapping definitions.</param>
        /// <param name="cfg">An optional delegate to apply additional configuration settings.</param>
        /// <param name="connectionString">An optional database connection string.</param>
        /// <returns>An initialized ISessionFactory instance.</returns>
        public static ISessionFactory ConfigureFluentlySQLite(Assembly mappingAssembly, Action<Configuration>? cfg = null, string? connectionString = null)
        {
            SessionFactory = GetFluentConfiguration<SQLiteDialect, SQLite20Driver>(mappingAssembly, cfg, connectionString).BuildSessionFactory();
            return SessionFactory;
        }
        /// <summary>
        /// Configures and builds an NHibernate session factory using Fluent NHibernate with the specified dialect,
        /// driver, and mapping assembly.
        /// </summary>
        /// <typeparam name="TDialect">The database dialect to use for the session factory.</typeparam>
        /// <typeparam name="TDriver">The database driver to use for the session factory.</typeparam>
        /// <param name="mappingAssembly">The assembly containing NHibernate mapping definitions.</param>
        /// <param name="cfg">An optional delegate to configure additional NHibernate settings.</param>
        /// <param name="connectionString">An optional database connection string.</param>
        /// <returns>An initialized instance of ISessionFactory.</returns>
        public static ISessionFactory ConfigureFluently<TDialect, TDriver>(Assembly mappingAssembly, Action<Configuration>? cfg = null, string? connectionString = null)
            where TDialect : Dialect.Dialect where TDriver : IDriver
        {
            SessionFactory = GetFluentConfiguration<TDialect, TDriver>(mappingAssembly, cfg, connectionString).BuildSessionFactory();
            return SessionFactory;
        }
    }
}
