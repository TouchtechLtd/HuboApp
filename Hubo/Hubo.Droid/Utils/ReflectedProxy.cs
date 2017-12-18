// <copyright file="ReflectedProxy.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo.Droid
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class ReflectedProxy<T>
        where T : class
    {
        private readonly Dictionary<string, PropertyInfo> cachedPropertyInfo;
        private readonly Dictionary<string, MethodInfo> cachedMethodInfo;

        private readonly IEnumerable<PropertyInfo> targetPropertyInfoList;
        private readonly IEnumerable<MethodInfo> targetMethodInfoList;

        private object target;

        public ReflectedProxy(T target)
        {
            this.target = target;

            this.cachedPropertyInfo = new Dictionary<string, PropertyInfo>();
            this.cachedMethodInfo = new Dictionary<string, MethodInfo>();

            TypeInfo typeInfo = typeof(T).GetTypeInfo();
            this.targetPropertyInfoList = typeInfo.GetRuntimeProperties();
            this.targetMethodInfoList = typeInfo.GetRuntimeMethods();
        }

        public void SetPropertyValue(object value, [CallerMemberName] string propertyName = "")
        {
            this.GetPropertyInfo(propertyName).SetValue(this.target, value);
        }

        public TPropertyValue GetPropertyValue<TPropertyValue>([CallerMemberName] string propertyName = "")
        {
            return (TPropertyValue)this.GetPropertyInfo(propertyName).GetValue(this.target);
        }

        public object Call([CallerMemberName] string methodName = "", object[] parameters = null)
        {
            if (!this.cachedMethodInfo.ContainsKey(methodName))
            {
                this.cachedMethodInfo[methodName] = this.targetMethodInfoList.Single(mi => mi.Name == methodName || mi.Name.Contains("." + methodName));
            }

            return this.cachedMethodInfo[methodName].Invoke(this.target, parameters);
        }

        private PropertyInfo GetPropertyInfo(string propertyName)
        {
            if (!this.cachedPropertyInfo.ContainsKey(propertyName))
            {
                this.cachedPropertyInfo[propertyName] = this.targetPropertyInfoList.Single(pi => pi.Name == propertyName || pi.Name.Contains("." + propertyName));
            }

            return this.cachedPropertyInfo[propertyName];
        }
    }
}