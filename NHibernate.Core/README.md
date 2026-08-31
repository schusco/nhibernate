NHibernate.Core

NHibernate.Core is a .NET library that provides a simple way for developers to perform the most difficult part of working with NHibernate, configuring the ISessionFactory instance.  
It also provides projection classes for the most common data manipulations.

The connection string to connect to the database can be configured two ways: either by passing it to the constructor of the NHibernateHelper class, or by using a config.json file 
containing the connection details.

Sample config.json file:

{
  "UserId": "username", 
  "Password": "password",   
  "Host": "localhost", 
  "Database": "database name" 
}

This is a simple helper class implementation, that shows just how easy it is to configure NHibernate and use it in your application. 
It is not intended to be a complete solution, but rather a starting point for your own implementation.

public class NHibernateHelper
{
    public static ISessionFactory SessionFactory { get; private set; }

    public NHibernateHelper(string connectionString)
    {
        try
        {
            SessionFactory = NHibernateSessionManager.Configure(cfg => cfg.Properties["show_sql"] = "true", connectionString);
        }
        catch (Exception ex)
        {
            throw new Exception("NHibernate initialization failed", ex);
        }
    }
}