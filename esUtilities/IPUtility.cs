using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace esUtilities
{
    public class IPUtility
    {
        public static bool TheIpIsRanges(string ip, string ranges)
        {
            bool tmpRes = false;
            if (string.IsNullOrEmpty(ranges))
                return tmpRes;

            return TheIpIsRange(ip, ranges.Split(';'));
        }

        public static bool TheIpIsRange(string ip, params string[] ranges)
        {
            bool tmpRes = false;
            foreach (var item in ranges)
            {
                if (TheIpIsRange(ip, item))
                {
                    tmpRes = true; break;
                }
            }
            return tmpRes;
        }

        /// <summary>
        /// 判断指定的IP是否在指定的IP范围内   这里只能指定一个范围
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="ranges"></param>
        /// <returns></returns>
        public static bool TheIpIsRange(string ip, string ranges)
        {
            bool result = false;
            int count;
            string start_ip, end_ip;
            //检测指定的IP范围 是否合法
            TryParseRanges(ranges, out count, out start_ip, out end_ip);//检测ip范围格式是否有效
            if (ip == "::1")
                ip = "127.0.0.1";
            try
            {
                System.Net.IPAddress.Parse(ip);//判断指定要判断的IP是否合法
            }
            catch (Exception)
            {
                return false;
            }

            if (count == 1 && ip == start_ip)
                result = true;//如果指定的IP范围就是一个IP，那么直接匹配看是否相等
            else if (count == 2)//如果指定IP范围 是一个起始IP范围区间
            {
                byte[] start_ip_array = Get4Byte(start_ip);//将点分十进制 转换成 4个元素的字节数组
                byte[] end_ip_array = Get4Byte(end_ip);
                byte[] ip_array = Get4Byte(ip);

                bool tmpRes = true;
                for (int i = 0; i < 4; i++)
                {
                    //从左到右 依次比较 对应位置的 值的大小  ，一旦检测到不在对应的范围 那么说明IP不在指定的范围内 并将终止循环
                    if (ip_array[i] > end_ip_array[i] || ip_array[i] < start_ip_array[i])
                    {
                        tmpRes = false; break;
                    }
                }
                result = tmpRes;
            }
            return result;
        }

        //尝试解析IP范围  并获取闭区间的 起始IP   (包含)
        private static void TryParseRanges(string ranges, out int count, out string start_ip, out string end_ip)
        {
            string[] _r = ranges.Split('-');
            if (!(_r.Length == 2 || _r.Length == 1))
            {
                start_ip = "";
                end_ip = "";
                count = 0;
            }
            else
            {
                count = _r.Length;
                start_ip = _r[0];
                end_ip = "";
                try
                {
                    System.Net.IPAddress.Parse(_r[0]);
                }
                catch (Exception)
                {
                    return;
                }

                if (_r.Length == 2)
                {
                    end_ip = _r[1];
                    try
                    {
                        System.Net.IPAddress.Parse(_r[1]);
                    }
                    catch (Exception)
                    {
                        throw new Exception("The IP address is invalid");
                    }
                }
            }
        }


        /// <summary>
        /// 将IP四组值 转换成byte型
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        static byte[] Get4Byte(string ip)
        {
            string[] _i = ip.Split('.');

            List<byte> res = new List<byte>();
            foreach (var item in _i)
            {
                res.Add(Convert.ToByte(item));
            }

            return res.ToArray();
        }

        public static bool CheckIp(string ip)
        {
            System.Net.IPAddress ipaddress;
            return System.Net.IPAddress.TryParse(ip, out ipaddress);
        }


        public static long IpToLong(string ip)
        {
            string[] items = ip.Split('.');
            return long.Parse(items[0]) << 24
                    | long.Parse(items[1]) << 16
                    | long.Parse(items[2]) << 8
                    | long.Parse(items[3]);
        }

        public static bool IpIsWithinBoliviaRange(string ip, List<string> Ip_Range)
        {
            try
            {
                if (ip == "::1")
                    ip = "127.0.0.1";
                IPAddress incomingIp = IPAddress.Parse(ip);
                foreach (var subnet in Ip_Range)
                {
                    
                    if (!subnet.Contains("-"))
                    {
                        foreach (var eip in subnet.Split(';'))
                        {
                            IPNetwork network = IPNetwork.Parse(eip);
                            if (IPNetwork.Contains(network, incomingIp))
                                return true;
                        }
                    }
                    else
                    {
                        if (TheIpIsRanges(ip, subnet))
                            return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
