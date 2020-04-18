using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsicServer.Infrastructure
{
    public class AppSettings
    {
        public string SecretKey { get; set; }
        public int TokenExpireTime { get; set; }
        public string SaveDirectory { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string FirebaseConfigurationFile { get; set; }
        public string PythonExeFullPath { get; set; }
    }
}
