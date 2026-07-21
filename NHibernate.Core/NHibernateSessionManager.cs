using NHibernate.Bytecode;
using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Dialect;
using NHibernate.Driver;
using System.Diagnostics;
using System.Text.Json;

namespace NHibernate.Core
{
    /// <summary>
    /// Provides an abstract base class for managing NHibernate session factories and sessions, including configuration
    /// and session lifecycle operations.
    /// </summary>
    /// <remarks>Intended to be inherited to implement specific session management logic. Handles
    /// configuration from external files and supports custom NHibernate dialects and drivers.</remarks>
    public abstract class NHibernateSessionManagerBase
    {
        /// <summary>
        /// ISessionFactory implementation is thread safe...
        /// thus static allows all threads to share it.
        /// It is created in the static constructor below...
        /// </summary>
        protected static ISessionFactory? SessionFactory { get; set; }
        /// <summary>
        /// Represents the configured connection string for database access.
        /// </summary>
        protected static string? ConfiguredConnectionString { get; set; }

        /// <summary>
        /// Constructor creates the ISessionFactory implementation.
        /// Executes the first time the helper class is called.
        /// </summary>
        static NHibernateSessionManagerBase()
        {
            try
            {
                var configFile = System.Configuration.ConfigurationManager.AppSettings["ConnectionStringConfigFile"] ?? "config.json";
                var config = JsonSerializer.Deserialize<DatabaseConfig>(File.ReadAllText(configFile));
                if (config is not null)
                {
                    ConfiguredConnectionString = $"User Id={config.UserId};Password={config.Password};Host={config.Host};Database={config.Database}";
                }
            }
            catch (FileNotFoundException)
            {
                // Absorb the exception and allow the application to continue, user must provide connection string via Configure method.
                ConfiguredConnectionString = null;
            }
            catch (JsonException ex)
            {
                throw new Exception("Unable to parse config.json, fix file or provide connection string.", ex);
            }
        }
        /// <summary>
        /// Creates and configures an NHibernate Configuration instance using the specified dialect, driver, and
        /// optional settings.
        /// </summary>
        /// <typeparam name="TDialect">The database dialect to use for NHibernate configuration.</typeparam>
        /// <typeparam name="TDriver">The database driver to use for NHibernate configuration.</typeparam>
        /// <param name="code">An optional action to apply additional configuration to the NHibernate Configuration instance.</param>
        /// <param name="connectionString">An optional connection string for database access.</param>
        /// <param name="runExtraConfig">Indicates whether to execute the additional configuration action.</param>
        /// <returns>A configured NHibernate Configuration instance.</returns>
        /// <exception cref="Exception">Thrown when initialization fails due to a missing connection string.</exception>
        protected static Configuration GetConfiguration<TDialect, TDriver>(Action<Configuration>? code = null, string? connectionString = null, bool runExtraConfig = true)
            where TDialect : Dialect.Dialect
            where TDriver : IDriver
        {
            try
            {
                if (string.IsNullOrEmpty(ConfiguredConnectionString) && connectionString is null)
                    throw new Exception("Nhibernate initialization failed.  Connection string must be provided or configured through a config.json file");
                var currentProcess = Process.GetCurrentProcess();
                string sessionContext = currentProcess.ProcessName == "w3wp" ? "web" : "thread_static";

                var cfg = new Configuration();
                cfg.DataBaseIntegration(x =>
                {
                    x.ConnectionString = ConfiguredConnectionString ?? connectionString;
                    x.Dialect<TDialect>();
                    x.Driver<TDriver>();
                });
                cfg.Proxy(p => p.ProxyFactoryFactory<StaticProxyFactoryFactory>());
                cfg.Properties["current_session_context_class"] = sessionContext;
                cfg.Properties["show_sql"] = "true";
                if (runExtraConfig)
                    code?.Invoke(cfg);
                return cfg;

            }
            catch (Exception ex)
            {
                throw new Exception("NHibernate initialization failed", ex);
            }
        }
        /// <summary>
        /// Configures and builds an ISessionFactory instance for NHibernate using the specified options.  Uses MySQL dialect and driver by default.
        /// </summary>
        /// <param name="code">An optional action to customize the NHibernate configuration.</param>
        /// <param name="connectionString">An optional database connection string.</param>
        /// <returns>The configured ISessionFactory instance.</returns>
        public static ISessionFactory Configure(Action<Configuration>? code = null, string? connectionString = null)
        {
            SessionFactory = GetConfiguration<MySQL55Dialect, MySqlDataDriver>(code, connectionString).BuildSessionFactory();
            return SessionFactory;
        }
        /// <summary>
        /// Configures and builds an ISessionFactory using the specified dialect and driver types.
        /// </summary>
        /// <typeparam name="TDialect">The dialect type to use for configuration.</typeparam>
        /// <typeparam name="TDriver">The driver type to use for configuration.</typeparam>
        /// <param name="code">An optional action to customize the configuration.</param>
        /// <param name="connectionString">An optional database connection string.</param>
        /// <returns>A configured ISessionFactory instance.</returns>
        public static ISessionFactory Configure<TDialect, TDriver>(Action<Configuration>? code = null, string? connectionString = null)
            where TDialect : Dialect.Dialect
            where TDriver : IDriver
        {
            SessionFactory = GetConfiguration<TDialect, TDriver>(code, connectionString).BuildSessionFactory();
            return SessionFactory;
        }

        /// <summary>
        /// Factory method for new ISessions.
        /// A convenient method to shorten the code required for
        /// the most common usage of this class: opening new sessions.
        /// </summary>
        /// <param name="appendToCurrentSessionContext">Optional parameter indicating if the returned session should be bound the current session context.  Default is true</param>
        /// <returns>A configures ISession instance</returns>
        /// <exception cref="Exception">Thrown if the session factory is not configured prior to calling this method.</exception>
        public static ISession OpenSession(bool appendToCurrentSessionContext = true)
        {
            if (SessionFactory == null)
                throw new Exception("SessionFactory is null. Make sure you have configured the session factory through a configure method.");

            var dBSession = SessionFactory.OpenSession();

            if (appendToCurrentSessionContext)
                CurrentSessionContext.Bind(dBSession);
            return dBSession;
        }

        /// <summary>
        /// Disposes the current NHibernate session and manages the transaction based on the error state.
        /// </summary>
        /// <param name="hasErrors">True if errors occurred during the session; otherwise, false.</param>        
        public static void DisposeOfCurrentSession(bool hasErrors)
        {
            if (SessionFactory is null)
                return;
            var session = SessionFactory.GetCurrentSession();
            if (session == null)
                return;

            var trans = session.GetCurrentTransaction();
            if (trans != null)
            {
                if (!hasErrors && trans.IsActive)
                {
                    try
                    {
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        ex.Data.Add("Commit_Timestamp", DateTime.Now);
                        trans.Rollback();
                    }
                }
                else if (hasErrors && trans.IsActive)
                    trans.Rollback();
                trans.Dispose();
            }

            CurrentSessionContext.Unbind(SessionFactory);
            session.Dispose();
        }
    }
    /// <summary>
    /// Provides a session manager for handling NHibernate sessions.
    /// </summary>
    public class NHibernateSessionManager : NHibernateSessionManagerBase { }
}
