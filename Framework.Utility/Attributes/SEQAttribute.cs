using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Utility.Attributes
{
    /// <summary>
    /// 设置序列的自定义特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class SEQAttribute : Attribute
    {
        /// <summary>
        /// 序列的名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 操作类型
        /// </summary>
        public SEQAttribute(string Name)
        {
            this.Name = Name;
        }
    }
}
