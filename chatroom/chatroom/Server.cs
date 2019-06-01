using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using MySql.Data.MySqlClient;
using System.IO;
using System.Drawing;

namespace chatroom
{
    class Server
    {
        //异步程序
        //监听嵌套字
        public Socket listenfd;
        //客户端连接
        public Conn[] conns;
        //最大连接数
        public int maxConn = 50;
        public static byte[] imagedata = new byte[1024 * 1024];
        //获取连接池索引，返回负数表示获取失败
        public int NewIndex()
        {
            if (conns == null)
                return -1;
            for (int i = 0; i < conns.Length; i++)
            {
                if (conns[i] == null)
                {
                    conns[i] = new Conn();
                    return i;
                }
                else if (conns[i].isuse == false)
                {
                    return i;
                }
            }
            return -1;
        }

        //开启服务器
        public void Start(string host, int port)
        {
            //连接池
            conns = new Conn[maxConn];
            for (int i = 0; i < maxConn; i++)
            {
                conns[i] = new Conn();
            }
            //socket
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Bind
            IPAddress ipadr = IPAddress.Parse(host);
            IPEndPoint ipep = new IPEndPoint(ipadr, port);
            listenfd.Bind(ipep);
            //listen
            listenfd.Listen(maxConn);
            //accept
            listenfd.BeginAccept(AcceptCb, null);
            Console.WriteLine("服务器启动成功！");
            Console.ReadKey();

        }
        private void AcceptCb(IAsyncResult ar)
        {
            try
            {
                Socket socket = listenfd.EndAccept(ar);
                int index = NewIndex();
                if (index < 0)
                {
                    socket.Close();
                    Console.WriteLine("连接人数已满");
                }
                else
                {
                    Conn conn = conns[index];
                    conn.Init(socket);
                    //string adr = conn.GetAdress();
                    //Console.WriteLine(adr+"已连接" + "ID:" + index);
                    conn.socket.BeginReceive(conn.readbuff, conn.buffcount, conn.BuffRemain(), SocketFlags.None, ReceiveCb, conn);

                }
                listenfd.BeginAccept(AcceptCb, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private void ReceiveCb(IAsyncResult ar)
        {
            Conn conn = (Conn)ar.AsyncState;
            try
            {
                int count = conn.socket.EndReceive(ar);
                //关闭信号
                if (count <= 0)
                {
                    //Console.WriteLine(conn.GetAdress() + "断开连接");
                    conn.Close();
                    return;
                }
                
                //数据处理
                string str = Encoding.UTF8.GetString(conn.readbuff,0,count);
                conn.chatnum++;
                if (conn.chatnum == 1)//chatnum为1时，证明客户端下一条发送的语句为客户端的用户的名称和聊天对象的名称
                {
                    string res=str.Replace("发起单人聊天","");
                    string[] name = res.Split('向');
                    conn.username = name[0];
                    conn.chatname = name[1];
                    Console.WriteLine(str);
                    //str = conn.ReadChatContent();//将数据库中的聊天纪录赋值给str，然后发送给客户端用户
                }
                else
                {
                    conn.SaveInMysql(str);//将信息存入数据库中
                    Console.WriteLine("接收消息："+str);
                }

                
                for(int i = 0; i < conns.Length; i++)
                {
                    if (conns[i] == null)
                        continue;
                    if (!conns[i].isuse)
                        continue;
                    if ((conns[i].username == conn.chatname) || (conns[i].username == conn.username))//如果客户端的用户名等于聊天对象名，则发送消息
                    {
                        if (conn.imagecount==2)
                        {
                            conns[i].socket.Send(imagedata);
                        }
                        else
                        {
                            conns[i].socket.Send(Encoding.UTF8.GetBytes(str));//发送消息给客户端
                        }
                        
                    }
                }
                
                conn.socket.BeginReceive(conn.readbuff, conn.buffcount, conn.BuffRemain(), SocketFlags.None, ReceiveCb, conn);
            
                }
            catch (Exception e)
            {
                //Console.WriteLine(conn.GetAdress() + "断开连接");
                conn.Close();
            }
        }

      
    }
}

