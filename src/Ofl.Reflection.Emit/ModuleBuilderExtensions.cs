using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Ofl.Reflection.Emit
{
    public static class ModuleBuilderExtensions
    {
        public static TypeBuilder DefinePoco(this ModuleBuilder moduleBuilder, string name,
            params KeyValuePair<string, Type>[] properties) => moduleBuilder.DefinePoco(name, properties.AsEnumerable());

        public static TypeBuilder DefinePoco(this ModuleBuilder moduleBuilder, string name,
            IEnumerable<KeyValuePair<string, Type>> properties)
        {
            // Validate parameters.
            if (moduleBuilder == null) throw new ArgumentNullException(nameof(moduleBuilder));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (properties == null) throw new ArgumentNullException(nameof(properties));

            // Create the type.
            TypeBuilder typeBuilder = moduleBuilder.DefineType(name, TypeAttributes.Public);

            // Create the set of names.  Need to compare at the byte
            // level.
            ISet<string> propertyNames = new HashSet<string>(StringComparer.Ordinal);

            // Cycle through the properties.
            foreach (KeyValuePair<string, Type> pair in properties)
            {
                // Get the name and the type.
                string propertyName = pair.Key;
                Type type = pair.Value;

                // If the name exists, throw.
                if (propertyNames.Contains(propertyName))
                    throw new InvalidOperationException($"Encountered a duplicate property name of \"{ propertyName }\"");

                // Add the property.
                typeBuilder.DefineAutoImplementedProperty(propertyName, type);

                // Add the name fo the set.
                propertyNames.Add(propertyName);
            }

            // Return the type builder.
            return typeBuilder;
        }
    }
}
