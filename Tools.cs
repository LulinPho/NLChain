using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Basement
{
    internal static class Tools
    {
        public static string GetDefaultTemplate(Type typeInfo)
        {
            try
            {
                ConstructorInfo constructor = typeInfo.GetConstructor(Type.EmptyTypes);

                return JsonSerializer.Serialize(constructor.Invoke(null));
            }

            catch (Exception ex)
            {
                throw new Exception("Failed to invoke empty constructors, please check the implement of given class.");
            }
        }
    }
}
