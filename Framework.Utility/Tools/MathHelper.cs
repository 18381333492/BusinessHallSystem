using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Utility.Tools
{
    /// <summary>
    /// 数学上面的一些计算封装
    /// </summary>
    public class MathHelper
    {
        /// <summary>
        /// 根据两点的经纬度计算距离
        /// </summary>
        /// <param name="xlat">X点的纬度</param>
        /// <param name="xlon">X点的经度</param>
        /// <param name="ylat">Y点的纬度</param>
        /// <param name="ylon">Y点的经度</param>
        /// <returns></returns>
        public static double GetDistanceByTude(double xlat, double xlon, double ylat, double ylon)
        {
             //地球半径单位Km
            double earth_r = 6378.137;
             double PI=Math.PI;
             double distance = 2 * earth_r *
                             Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(PI * (xlat - ylat) / 360), 2) + Math.Abs(Math.Cos(PI * xlon / 180)) * Math.Cos(ylat * PI / 180)
                             * Math.Pow(Math.Sin(PI * (xlon - ylon) / 360), 2)));
             return distance;
             
        }

        private static int rep = 0;
        /// <summary>
        /// 随机数字的生成(不重复).
        /// </summary>
        /// <param name="codeCount">随机码位数</param>
        /// <returns></returns>
        public static string RandomCodeNum(int codeCount)
        {
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks + rep;
            rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
            for (int i = 0; i < codeCount; i++)
            {
                int num = random.Next();
                str = str + ((char)(0x30 + ((ushort)(num % 10)))).ToString();
            }
            return str;
        }
    }
}
