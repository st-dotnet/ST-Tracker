using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.DapperUtility
{
    public static class ConnectionClass
    {
        public static string ConVal()
        {
            return ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        }
    }
}
