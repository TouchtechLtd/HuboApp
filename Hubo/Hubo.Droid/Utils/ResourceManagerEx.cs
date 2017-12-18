// <copyright file="ResourceManagerEx.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo.Droid
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    internal static class ResourceManagerEx
    {
        internal static int IdFromTitle(string title, Type type)
        {
            string name = Path.GetFileNameWithoutExtension(title);
            int id = GetId(type, name);
            return id;
        }

        private static int GetId(Type type, string propertyName)
        {
            FieldInfo[] props = type.GetFields();
            FieldInfo prop = props.Select(p => p).FirstOrDefault(p => p.Name == propertyName);
            if (prop != null)
            {
                return (int)prop.GetValue(type);
            }

            return 0;
        }
    }
}