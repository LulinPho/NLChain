using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basement.Models
{
    public abstract class ResponseContent
    { }

    public abstract class RequestContent
    { }

    internal interface ILLMModel
    {
        public ResponseContent Post(RequestContent content);
        public List<string> Invoke(RequestContent content);


    }
}
