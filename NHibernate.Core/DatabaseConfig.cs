using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Core
{
    internal class DatabaseConfig    
    {
        public string UserId { get; set; } = "";
        public string Password { get; set; } = "";
        public string Host { get; set; } = "";
        public string Database { get; set; } = "";
    }
}
