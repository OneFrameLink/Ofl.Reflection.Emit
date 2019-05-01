using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace Ofl.Reflection.Emit.Tests
{
    public class ModuleBuilderExtensionsTests
    {
        [Fact]
        public void Test_DefinePoco()
        {
            // Create the assembly.
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName("temp"), AssemblyBuilderAccess.RunAndCollect);

            // Create the module.
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("temp.dll");

            // Define the type
            TypeBuilder typeBuilder = moduleBuilder.DefinePoco("Temp",
                new KeyValuePair<string, Type>("TempIntProperty", typeof(int)),
                new KeyValuePair<string, Type>("TempStringProperty", typeof(string))
            );

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