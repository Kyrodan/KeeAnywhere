using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeAnywhere.Json
{
    public class JsonDecryptException : Exception
    {
        public JsonDecryptException(Exception ex) : base("Could not decrypt property.", ex) { }
    }
}
