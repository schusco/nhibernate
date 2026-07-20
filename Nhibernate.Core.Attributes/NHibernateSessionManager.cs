using NHibernate;
using NHibernate.Cfg;
using NHibernate.Core;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.Attributes;
using System.Reflection;

namespace Nhibernate.Core.Attributes
{
    public class NHibernateSessionManager : NHibernateSessionManagerBase
    {
        private static Configuration GetAttributeMappingConfig<TDialect, TDriver>(Assembly mappingAssembly, string connectionString) where TDriver : IDriver where TDialect : Dialect
        {
            return GetConfiguration<TDialect, TDriver>(cfg => cfg.AddInputStream(HbmSerializer.Default.Serialize(mappingAssembly), connectionString));
        }
        public static ISessionFactory ConfigureWithAttributes<TDialect, TDriver>(Assembly mappingAssembly, string connectionString = null) where TDriver : IDriver where TDialect : Dialect
        {
            SessionFactory = GetAttributeMappingConfig<TDialect, TDriver>(mappingAssembly, connectionString).BuildSessionFactory();
            return SessionFactory;
        }
        public static ISessionFactory ConfigureWithAttributes(Assembly mappingAssembly, string connectionString = null)
        {
            SessionFactory = GetAttributeMappingConfig<MySQL55Dialect, MySqlDataDriver>(mappingAssembly, connectionString).BuildSessionFactory();
            return SessionFactory;
        }
    }
}
