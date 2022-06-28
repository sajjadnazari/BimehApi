using Common.Bimeh.Constants.Enumerations;
using System.Collections.Generic;

namespace Stub.Bimeh
{
    public class ApiAddressConstantValue
    {
        public string Url { get; set; }
        public HttpMethodType HttpMethodType { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}
