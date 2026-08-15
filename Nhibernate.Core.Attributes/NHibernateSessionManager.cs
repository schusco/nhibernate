using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.Attributes;
using System.Reflection;

namespace NHibernate.Core.Attributes
{
    /// <summary>
    /// Manages NHibernate session creation and configuration using attribute-based mapping.
    /// </summary>
    /// <remarks>Provides static methods to configure and build NHibernate session factories with support for
    /// custom dialects and drivers.</remarks>
    public class NHibernateSessionManager : NHibernateSessionManagerBase
    {
        private static Configuration GetAttributeMappingConfig<TDialect, TDriver>(Assembly mappingAssembly, Action<Configuration>? cfg, string? connectionString) where TDriver : IDriver where TDialect : Dialect.Dialect
        {
            var test = connectionString;
            var config = GetConfiguration<TDialect, TDriver>(cfg => cfg.AddInputStream(HbmSerializer.Default.Serialize(mappingAssembly)), connectionString);
            cfg?.Invoke(config);
            return config;
        }
        /// <summary>
        /// Configures and builds an ISessionFactory using attribute-based mappings from the specified assembly.
        /// </summary>
        /// <typeparam name="TDialect">The SQL dialect to use for the session factory.</typeparam>
        /// <typeparam name="TDriver">The database driver to use for the session factory.</typeparam>
        /// <param name="mappingAssembly">The assembly containing attribute-based mapping definitions.</param>
        /// <param name="cfg">An optional delegate to apply additional configuration settings.</param>
        /// <param name="connectionString">The database connection string. If null, a default connection string is used.</param>
        /// <returns>A configured ISessionFactory instance.</returns>
        public static ISessionFactory ConfigureWithAttributes<TDialect, TDriver>(Assembly mappingAssembly, Action<Configuration>? cfg = null, string? connectionString = null) where TDriver : IDriver where TDialect : Dialect.Dialect
        {
            SessionFactory = GetAttributeMappingConfig<TDialect, TDriver>(mappingAssembly, cfg, connectionString).BuildSessionFactory();
            return SessionFactory;
        }
        /// <summary>
        /// Configures and returns an ISessionFactory using attribute-based mappings from the specified assembly.  Defaults to MySQL dialect and driver.
        /// </summary>
        /// <param name="mappingAssembly">The assembly containing attribute-based NHibernate mappings.</param>
        /// <param name="cfg">An optional delegate to apply additional configuration settings.</param>
        /// <param name="connectionString">The database connection string, or null to use the default.</param>
        /// <returns>A configured ISessionFactory instance.</returns>
        public static ISessionFactory ConfigureWithAttributes(Assembly mappingAssembly, Action<Configuration>? cfg = null, string? connectionString = null)
        {
            SessionFactory = GetAttributeMappingConfig<MySQL55Dialect, MySqlDataDriver>(mappingAssembly, cfg, connectionString).BuildSessionFactory();
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
        public static ISessionFactory ConfigureWithAttributesPostgres(Assembly mappingAssembly, Action<Configuration>? cfg = null, string? connectionString = null)
        {
            SessionFactory = GetAttributeMappingConfig<PostgreSQLDialect, NpgsqlDriver>(mappingAssembly, cfg, connectionString).BuildSessionFactory();
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
        public static ISessionFactory ConfigureWithAttributesSqlServer(Assembly mappingAssembly, Action<Configuration>? cfg = null, string? connectionString = null)
        {
            SessionFactory = GetAttributeMappingConfig<MsSql2012Dialect, MicrosoftDataSqlClientDriver>(mappingAssembly, cfg, connectionString).BuildSessionFactory();
            return SessionFactory;
        }
        /// <summary>
        /// Configures and builds an NHibernate session factory for a SQLLite database connection using Fluent NHibernate with the specified mapping.  The mapping definitions are expected to be in the provided
        /// assembly.
        /// </summary>
        /// <param name="mappingAssembly">The assembly containing NHibernate mapping definitions.</param>
        /// <param name="cfg">An optional delegate to apply additional configuration settings.</param>
        /// <param name="connectionString">An optional database connection string.</param>
        /// <returns>An initialized ISessionFactory instance.</returns>
        public static ISessionFactory ConfigureWithAttributesSQLite(Assembly mappingAssembly, Action<Configuration>? cfg = null, string? connectionString = null)
        {
            SessionFactory = GetAttributeMappingConfig<SQLiteDialect, SQLite20Driver>(mappingAssembly, cfg, connectionString).BuildSessionFactory();
            return SessionFactory;
        }        
    }
}
