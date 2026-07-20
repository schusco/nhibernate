using FluentNHibernate.Cfg;
using NHibernate.Bytecode;
using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Dialect;
using NHibernate.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NHibernate.Core
{
    public abstract class NHibernateSessionManagerBase
    {
        /// <summary>
        /// ISessionFactory implementation is thread safe...
        /// thus static allows all threads to share it.
        /// It is created in the static constructor below...
        /// </summary>
        public static ISessionFactory? SessionFactory;
        protected static string? _configuredConnectionString;

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
                    _configuredConnectionString = $"User Id={config.UserId};Password={config.Password};Host={config.Host};Database={config.Database}";
                }
            }
            catch (FileNotFoundException)
            {
                // Absorb the exception and allow the application to continue, user must provide connection string via Configure method.
                _configuredConnectionString = null;
            }
            catch (JsonException ex)
            {
                throw new Exception("Unable to parse config.json, fix file or provide connection string.", ex);
            }
        }
        protected static Configuration GetConfiguration<TDialect, TDriver>(Action<Configuration>? code = null, string? connectionString = null)
            where TDialect : Dialect.Dialect
            where TDriver : IDriver
        {
            try
            {
                if (string.IsNullOrEmpty(_configuredConnectionString) && connectionString is null)
                    throw new Exception("Nhibernate initialization failed.  Connection string must be provided or configured through a config.json file");
                var currentProcess = Process.GetCurrentProcess();
                string sessionContext = currentProcess.ProcessName == "w3wp" ? "web" : "thread_static";

                var cfg = new Configuration();
                cfg.DataBaseIntegration(x =>
                {
                    x.ConnectionString = _configuredConnectionString ?? connectionString;
                    x.Dialect<TDialect>();//MySQL55Dialect
                    x.Driver<TDriver>();//MySqlDataDriver
                });
                cfg.Proxy(p => p.ProxyFactoryFactory<StaticProxyFactoryFactory>());
                //cfg.SetProperty(NhEnvironment.DefaultSchema, NHibernateConfig.DefaultSchema ?? "TRPDTA160");
                //cfg.AddAssembly(Assembly.GetAssembly(typeof(NHibernateHelper)));

                //if (NHibernateConfig.DataAssemblyName is null)
                //..  throw new TmcConfigurationException("Key DataAssemblyName Not Present in config");

                //cfg.AddAssembly(NHibernateConfig.DataAssemblyName);

                cfg.Properties["current_session_context_class"] = sessionContext;
                cfg.Properties["show_sql"] = "true";
                code?.Invoke(cfg);
                return cfg;

            }
            catch (Exception ex)
            {
                //ExceptionPublisher.Publish(ex, "PSS.Web");
                throw new Exception("NHibernate initialization failed", ex);
            }
        }
        public static ISessionFactory Configure(Action<Configuration> code = null, string connectionString = null)
        {
            SessionFactory = GetConfiguration<MySQL55Dialect, MySqlDataDriver>(code, connectionString).BuildSessionFactory();
            return SessionFactory;
        }
        public static ISessionFactory Configure<TDialect, TDriver>(Action<Configuration> code = null, string connectionString = null)
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
        public static ISession OpenSession(bool appendToCurrentSessionContext = true)
        {
            if (SessionFactory == null)
                throw new Exception("SessionFactory is null. Make sure TmcDict.UseNHibernate is returning true for your app.");

            var dBSession = SessionFactory.OpenSession();

            if (appendToCurrentSessionContext)
                CurrentSessionContext.Bind(dBSession);
            return dBSession;
        }


        public static void DisposeOfCurrentSession(bool hasErrors, string appName)
        {
            var session = SessionFactory.GetCurrentSession();
            if (session == null)
            {
                //Tmc.Logging.ExceptionPublisher.Publish(new Exception("Unable to retrieve Current NHibernate session"), appName);
                return;
            }

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
                        //Tmc.Logging.ExceptionPublisher.Publish(ex, appName);
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

    public class NHibernateSessionManager : NHibernateSessionManagerBase
    {

    }
}
