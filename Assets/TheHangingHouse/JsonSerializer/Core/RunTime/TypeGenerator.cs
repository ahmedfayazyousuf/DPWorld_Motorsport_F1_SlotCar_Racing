using System;
using System.Reflection;
using System.Collections;
using System.Reflection.Emit;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;

namespace TheHangingHouse.JsonSerializer.Test
{
    public static class TypeGenerator
    {
        public const string ClassName = "DynamicClass";
        public static void CreateType()
        {
            var assemblyName = new AssemblyName("DynamicAssebmly");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
            var typeBuilder = moduleBuilder.DefineType(ClassName, TypeAttributes.Public);

            var constructorInfo = typeof(SerializableAttribute).GetConstructor(new Type[] { });
            var customAttributeBuilder = new CustomAttributeBuilder(constructorInfo, new object[] { });
            typeBuilder.SetCustomAttribute(customAttributeBuilder);

            var fieldBuilderNumber = typeBuilder.DefineField("number", typeof(int), FieldAttributes.Public);

            typeBuilder.CreateType();
        }
    }
}
