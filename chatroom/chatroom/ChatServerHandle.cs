using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;

namespace chatroom
{
    class Client
    {
    //    //此类是同步接收客户端数据的类
    //    //public static int clientcount = 0;
    //    private Socket clientSocket;
    //    private byte[] data = new byte[2048];//存储数据
       
    //    public string User;
    //    public Client(Socket s)
    //    {

    //        clientSocket = s;
    //        //开启一个线程 处理客户端的数据接收
          
    //        Thread thread = new Thread(ReceiveMessage);
    //        thread.Start();
    //    }

    //    private void ReceiveMessage()
    //    {
    //          Program.ReadSql();//调用主程序中的函数
    //        //服务器一直接收客户端数据
    //        while (true)
    //        {
    //            //如果客户端掉线，直接出循环
    //            //if (clientSocket.Poll(100, SelectMode.SelectRead))
    //            //{
    //            //    clientSocket.Close();
    //            //    break;
    //            //}
    //            //接收信息  
    //            if (!Connected())
    //            {
    //                break;
    //            }
                
    //            int datalength = clientSocket.Receive(data);
    //            string message = Encoding.UTF8.GetString(data, 0, datalength);
    //            if (datalength == 0) break;
                
    //            //获得连接的用户名并且赋值给User
    //            string Message = message;
    //            int end1 = -1 ;
    //            try
    //            {
    //                end1 = Message.IndexOf("-已上线...");
    //                string online = Message.Substring(0, end1);
    //                if (online != ""||online==null)
    //                {
    //                    User = online;
    //                    online = "";
    //                }
    //            }
    //            catch
    //            {
                    
    //            }

    //            Program.BroadcastMessage(message,false);//广播信息
    //            Program.LinkMySql(User, message);
    //            Console.WriteLine(message);
    //        }
    //    }

        
        

    //    public void SendMessage(string message)
    //    {
    //        byte[] data = Encoding.UTF8.GetBytes(message);
    //        clientSocket.Send(data);
            
    //    }

    //    public bool Connected()
    //    {
    //        return clientSocket.Connected;
    //    }


       
    }
}  

