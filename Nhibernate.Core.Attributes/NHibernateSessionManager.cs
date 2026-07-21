using NHibernate;
using NHibernate.Cfg;
using NHibernate.Core;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.Attributes;
using System.Reflection;

namespace Nhibernate.Core.Attributes
{
    /// <summary>
    /// Manages NHibernate session creation and configuration using attribute-based mapping.
    /// </summary>
    /// <remarks>Provides static methods to configure and build NHibernate session factories with support for
    /// custom dialects and drivers.</remarks>
    public class NHibernateSessionManager : NHibernateSessionManagerBase
    {
        private static Configuration GetAttributeMappingConfig<TDialect, TDriver>(Assembly mappingAssembly, string? connectionString) where TDriver : IDriver where TDialect : Dialect
        {
            return GetConfiguration<TDialect, TDriver>(cfg => cfg.AddInputStream(HbmSerializer.Default.Serialize(mappingAssembly), connectionString));
        }
        /// <summary>
        /// Configures and builds an ISessionFactory using attribute-based mappings from the specified assembly.
        /// </summary>
        /// <typeparam name="TDialect">The SQL dialect to use for the session factory.</typeparam>
        /// <typeparam name="TDriver">The database driver to use for the session factory.</typeparam>
        /// <param name="mappingAssembly">The assembly containing attribute-based mapping definitions.</param>
        /// <param name="connectionString">The database connection string. If null, a default connection string is used.</param>
        /// <returns>A configured ISessionFactory instance.</returns>
        public static ISessionFactory ConfigureWithAttributes<TDialect, TDriver>(Assembly mappingAssembly, string? connectionString = null) where TDriver : IDriver where TDialect : Dialect
        {
            SessionFactory = GetAttributeMappingConfig<TDialect, TDriver>(mappingAssembly, connectionString).BuildSessionFactory();
            return SessionFactory;
        }
        /// <summary>
        /// Configures and returns an ISessionFactory using attribute-based mappings from the specified assembly.  Defaults to MySQL dialect and driver.
        /// </summary>
        /// <param name="mappingAssembly">The assembly containing attribute-based NHibernate mappings.</param>
        /// <param name="connectionString">The database connection string, or null to use the default.</param>
        /// <returns>A configured ISessionFactory instance.</returns>
        public static ISessionFactory ConfigureWithAttributes(Assembly mappingAssembly, string? connectionString = null)
        {
            SessionFactory = GetAttributeMappingConfig<MySQL55Dialect, MySqlDataDriver>(mappingAssembly, connectionString).BuildSessionFactory();
            return SessionFactory;
        }
    }
}
