using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO;
using System.Reflection;

namespace Hubo.Droid
{
    internal static class ResourceManagerEx
    {
        internal static int IdFromTitle(string title, Type type)
        {
            string name = Path.GetFileNameWithoutExtension(title);
            int id = GetId(type, name);
            return id;
        }

        static int GetId(Type type, string propertyName)
        {
            FieldInfo[] props = type.GetFields();
            FieldInfo prop = props.Select(p => p).FirstOrDefault(p => p.Name == propertyName);
            if (prop != null)
                return (int)prop.GetValue(type);
            return 0;
        }
    }
}