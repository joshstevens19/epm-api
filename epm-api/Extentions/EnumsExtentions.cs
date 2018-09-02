using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace epm_api.Extentions
{
    public static class EnumExtensions
    {
        public static string DisplayName(this Enum @enum)
        {
            string displayName = string.Empty;
            FieldInfo[] fields = @enum.GetType().GetFields();
            foreach (FieldInfo field in fields)
            {
                if (Attribute.GetCustomAttribute(field, typeof(DisplayNameAttribute)) is DisplayNameAttribute displayNameAttribute &&
                    field.Name.Equals(@enum.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    displayName = displayNameAttribute.DisplayName;
                    break;
                }
            }

            return displayName;
        }
    }
}
