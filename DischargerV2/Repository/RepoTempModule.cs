using Serial.Client.TempModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DischargerV2.Repository
{
    public class RepoTempModule
    {
        public static Dictionary<string, SerialClientTempModule> Clients = new Dictionary<string, SerialClientTempModule>();
    }
}
