using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Types
{
    public class RegisterType
    {
        public string username { get; set; }
        public string password { get; set; }

        public int age { get; set; }
        public string gender { get; set; }
        public string city { get; set; }
    }
}