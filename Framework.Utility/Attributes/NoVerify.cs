using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Utility.Attributes
{
    /// <summary>
    /// 不需要验证登录的自定义特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class NoVerifyAttribute : Attribute
    {
    }
}
