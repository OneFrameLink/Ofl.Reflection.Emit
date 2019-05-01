using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Ofl.Reflection.Emit
{
    public static class TypeBuilderExtensions
    {
        // Property set and property get methods require a special
        // set of attributes.
        private const MethodAttributes PropertyGetSetMethodAttributes =
            MethodAttributes.Public | MethodAttributes.SpecialName |
            MethodAttributes.HideBySig;

        public static (PropertyBuilder PropertyBuilder, FieldBuilder FieldBuilder) DefineAutoImplementedProperty(
            this TypeBuilder typeBuilder, string name, Type propertyType)
        {
            // Validate parameters.
            if (typeBuilder == null) throw new ArgumentNullException(nameof(typeBuilder));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (propertyType == null) throw new ArgumentNullException(nameof(propertyType));

            // Create the field.
            FieldBuilder fieldBuilder = typeBuilder.DefineField($"_${name}$",
                propertyType, FieldAttributes.Private);

            // Create the property.
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(name,
                PropertyAttributes.HasDefault, propertyType, null);

            // Get the get and set methods.
            MethodBuilder getMethodBuilder = typeBuilder.DefineAutoImplementedPropertyGetMethodBuilder(name,
                propertyType, fieldBuilder);
            MethodBuilder setMethodBuilder = typeBuilder.DefineAutoImplementedPropertySetMethodBuilder(name,
                propertyType, fieldBuilder);

            // Set the methods for the property accessors.
            propertyBuilder.SetGetMethod(getMethodBuilder);
            propertyBuilder.SetSetMethod(setMethodBuilder);

            // Return the property and the field builders.
            return (propertyBuilder, fieldBuilder);
        }

        private static MethodBuilder DefineAutoImplementedPropertyGetMethodBuilder(this TypeBuilder typeBuilder,
            string name, Type propertyType, FieldBuilder fieldBuilder)
        {
            // Validate parameters.
            if (typeBuilder == null) throw new ArgumentNullException(nameof(typeBuilder));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (propertyType == null) throw new ArgumentNullException(nameof(propertyType));
            if (fieldBuilder == null) throw new ArgumentNullException(nameof(fieldBuilder));

            // Define the "get" accessor method for the property.
            MethodBuilder methodBuilder = typeBuilder.DefineMethod($"get_{name}",
                PropertyGetSetMethodAttributes, propertyType, Type.EmptyTypes);

            // Generate the Il for the get method.
            ILGenerator ig = methodBuilder.GetILGenerator();

            // Emit the 
            ig.Emit(OpCodes.Ldarg_0);
            ig.Emit(OpCodes.Ldfld, fieldBuilder);
            ig.Emit(OpCodes.Ret);

            // Return the method builder.
            return methodBuilder;
        }

        private static MethodBuilder DefineAutoImplementedPropertySetMethodBuilder(this TypeBuilder typeBuilder,
            string name, Type propertyType, FieldBuilder fieldBuilder)
        {
            // Validate parameters.
            if (typeBuilder == null) throw new ArgumentNullException(nameof(typeBuilder));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (propertyType == null) throw new ArgumentNullException(nameof(propertyType));
            if (fieldBuilder == null) throw new ArgumentNullException(nameof(fieldBuilder));

            // Define the "get" accessor method for the property.
            MethodBuilder methodBuilder = typeBuilder.DefineMethod($"set_{name}",
                PropertyGetSetMethodAttributes, null, new Type[] { propertyType });

            // Generate the Il for the get method.
            ILGenerator ig = methodBuilder.GetILGenerator();

            // Emit the 
            ig.Emit(OpCodes.Ldarg_0);
            ig.Emit(OpCodes.Ldarg_1);
            ig.Emit(OpCodes.Stfld, fieldBuilder);
            ig.Emit(OpCodes.Ret);

            // Return the method builder.
            return methodBuilder;
        }
    }
}
