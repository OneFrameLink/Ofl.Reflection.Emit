using System;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace Ofl.Reflection.Emit.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test_DefineAutoImplementedProperty()
        {
            // Create the assembly.
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName("temp"), AssemblyBuilderAccess.RunAndCollect);

            // Create the module.
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("temp.dll");

            // Define the type.
            TypeBuilder typeBuilder = moduleBuilder.DefineType("Temp", TypeAttributes.Public);

            // Define the properties.
            typeBuilder.DefineAutoImplementedProperty("TempIntProperty", typeof(int));
            typeBuilder.DefineAutoImplementedProperty("TempStringProperty", typeof(string));

            // Build the type.
            Type type = typeBuilder.CreateType();

            // Create an instance.
            dynamic instance = Activator.CreateInstance(type);

            // Check the default.
            Assert.Equal(default(int), instance.TempIntProperty);
            Assert.Null(instance.TempStringProperty);

            // The expected values.
            const int expectedInt = 10;
            const string expectedString = "string";

            // Set the values.
            instance.TempIntProperty = expectedInt;
            instance.TempStringProperty = expectedString;

            // Assert.
            Assert.Equal(expectedInt, instance.TempIntProperty);
            Assert.Equal(expectedString, instance.TempStringProperty);
        }
    }
}
