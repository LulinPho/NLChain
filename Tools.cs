using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Basement
{
    internal static class Tools
    {
        /// <summary>
        /// Generate a default template of json serilization of given type.
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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
        /// <summary>
        /// Attribute checker, return true if every property of the given type applied the given attribute.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool AreAllPropertiesRequired(Type type, Type attribute)
        {
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                if (!property.IsDefined(attribute, inherit: true))

                    return false;
            }
            return true;
        }


    }
}

