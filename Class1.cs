using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using NATUPNPLib;
using System.Net;
using System.Net.Sockets;
namespace libUpnp
{
    public class Upnp_Mgr
    {
        private int m_internal_port = 0; //内网端口
        private int m_ex_port = 0;          //外网端口
        private string m_sType = null;      //网口类型
        private string m_des = null;        //描述
        public Upnp_Mgr(int nLanPort, int nEport, string sType, string sDes)
        {
            m_internal_port = nLanPort;
            m_ex_port = nEport;
            m_sType = sType;
            m_des = sDes;
        }
        public int MappingNetProt() // 返回映射的实际端口
        {
            //创建COM类型
            var upnpnat = new UPnPNAT();
            var mappings = upnpnat.StaticPortMappingCollection;
            //错误判断
            if (mappings == null)
            {
                //Console.WriteLine("没有检测到路由器，或者路由器不支持UPnP功能。");
                return 0;
            }

            //检测端口是否已经被映射 绑定的话端口加一 再次测试
            try
            {
                while (true)
                {

                    IStaticPortMapping mappings1 = mappings[m_ex_port, "UDP"];
                    //Console.WriteLine("检测到端口{0}已经绑定。。", aa);
                    m_ex_port++;
                }
            }
            catch
            {
                //抛异常说明 外网端口没有被映射
            }
            string name = Dns.GetHostName();
            //从当前Host Name解析IP地址，筛选IPv4地址是本机的内网IP地址。

            if (!string.IsNullOrEmpty(name))
            {
                //var ipv4 = Dns.GetHostEntry(name).AddressList.Where(i => i.AddressFamily == AddressFamily.InterNetwork).FirstOrDefault();
                IPHostEntry ipv4 = Dns.GetHostEntry(name);
                 if (ipv4.AddressList.Length >= 1)
                 {
                     IPAddress lanIp = ipv4.AddressList[0];

                     IStaticPortMapping mapping_ = mappings.Add(m_ex_port, m_sType, m_internal_port, lanIp.ToString(), true, m_des);
                     if (mapping_ != null)
                         return m_ex_port;
                 }
                     return 0;
            }
            else
            {
                return 0;
            }
        }
        public void RemoveMappingProt() // 删除端口映射
        {
            var upnpnat = new UPnPNAT();
            var mappings = upnpnat.StaticPortMappingCollection;
            //错误判断
            if (mappings == null)
            {
                //Console.WriteLine("没有检测到路由器，或者路由器不支持UPnP功能。");
                return;
            }
            try
            {
                IStaticPortMapping mappings1 = mappings[m_ex_port, m_sType];
                mappings.Remove(m_ex_port, m_sType);

            }
            catch
            {

            }
            return;
        }
    }
}
