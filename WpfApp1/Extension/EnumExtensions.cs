using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment.Extension
{
    /// <summary>
    /// Enumerate extensions 
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Get the description for the value and enumerate it
        /// </summary>
        /// <param name="value"></param>
        /// <returns> null </returns>
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                        Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null) return attr.Description;
                }
            }

            return null;
        }
    }
}
