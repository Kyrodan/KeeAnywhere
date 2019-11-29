using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeAnywhere.Json
{
    public class JsonEncryptException : Exception
    {
        public JsonEncryptException(Exception ex) : base("Could not encrypt property.", ex) { }
    }
}
