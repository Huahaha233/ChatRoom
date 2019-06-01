using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using MySql.Data.MySqlClient;


namespace chatroom
{   //此类是异步socket程序
    class Conn
    {
        public const int BUFFER_SIZE = 1024*1024;//常量
        public Socket socket;
        public bool isuse = false;//是否使用
        public byte[] readbuff = new byte[BUFFER_SIZE];
        public int buffcount = 0;
        public string username;//连接的客户端的用户名称
        public string chatname;//聊天对象名称
        //聊天消息的序号，可以借此来获取用户昵称和聊天对象昵称
        public int chatnum = 0;
        public int imagecount = 3;
        private int count=0;
        //构造函数
        public Conn()
        {
            readbuff = new byte[BUFFER_SIZE];
        }
        //初始化
        public void Init(Socket socket)
        {
            this.socket = socket;
            isuse = true;
            buffcount = 0;
            
        }
        //缓冲区剩余字节数
        public int BuffRemain()
        {
            return BUFFER_SIZE - buffcount;
        }
        //获取客户端地址
        public string GetAdress()
        {
            if (!isuse)
                return "无法获取地址";
            return socket.RemoteEndPoint.ToString();
        }
        public void Close()
        {
            if (!isuse)
                return;
            //Console.WriteLine(GetAdress()+"断开连接");

            chatnum = 0;//chatnum清零，初始化

            socket.Close();
            isuse = false;
        }

        public  void SaveInMysql(string str)
        {
            //imagedata = null;//初始化
            //if (imagelenth != 0)
            //{
            //    readbuff.CopyTo(Server.imagedata, imagebytecount);
            //    imagebytecount += 1024;
            //    str = username + "已发送照片";
            //    imagelenth--;
            //    Console.WriteLine(readbuff.Length);
            //    if (imagelenth == 0)
            //    {
            //        isend = true;
            //        Server.imagedata=ByteCut(Server.imagedata);
            //    }
            //}
            
                int strint = str.IndexOf("@用户发送图片@");
                if (strint != -1)
                {
                    count = int.Parse(str.Replace("@用户发送图片@", ""));
                    imagecount = 0;
                }
            if (imagecount == 1)
            {
                //Array.Clear(Server.imagedata, 0, Server.imagedata.Length);//将imagedata初始化，给下一张图片做准备
                Server.imagedata = readbuff;
                str = username+"用户发送图片";
            }
            imagecount++;
            
                string connetStr = "server=127.0.0.1;port=3306;user=root;password=123456;database=chatroom;Sslmode=none";//注意Sslmode要赋值为none，否则无法连接到数据库
                MySqlConnection conn = new MySqlConnection(connetStr);
                MySqlCommand command = null;
                try
                {
                    conn.Open();
                    command = conn.CreateCommand();
                    command.CommandText = "INSERT INTO user_chat(chat_id,user_name,chat_name,chatcontent,imagedata,chat_time) VALUES(@chat_id,@user_name,@chat_name,@chatcontent,@imagedata,@chat_time)";
                    command.Parameters.AddWithValue("@chat_id", null);
                    command.Parameters.AddWithValue("@user_name", username);
                    command.Parameters.AddWithValue("@chat_name", chatname);
                    command.Parameters.AddWithValue("@chatcontent", str);
                    command.Parameters.AddWithValue("@imagedata", Server.imagedata);
                    string nowtime = DateTime.Now.Year + "." + DateTime.Now.Month + "." + DateTime.Now.Day + "-" + DateTime.Now.Hour + ":" + DateTime.Now.Minute;
                    command.Parameters.AddWithValue("@chat_time", nowtime);
                    command.ExecuteNonQuery();
                }
                catch
                {
                    //Console.WriteLine(ex.Message);
                }
                finally
                {
                    conn.Close();
                }

            
        }
        //public byte[] ByteCut(byte[] b)
        //{

        //    byte[] lastbyte = new byte[count];
        //    for (var i = 0; i < count; i++)
        //    {
        //        lastbyte[i] = b[i];
        //    }
        //    return lastbyte;
        //}

        //在数据库中读取数据
        public string ReadChatContent()
        {
            string connetStr = "server=127.0.0.1;port=3306;user=root;password=123456;database=chatroom;Sslmode=none";//注意Sslmode要赋值为none，否则无法连接到数据库
            MySqlConnection conn = new MySqlConnection(connetStr);
            conn.Open();
            MySqlCommand command = null;
            MySqlDataReader dataReader = null;
            try
            {
                string str="";//注意""与null是有区别的，若是return的值为null则会报错。
                command = conn.CreateCommand();
                command.CommandText = "SELECT * FROM user_chat";
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                   
                    if((dataReader.GetString(1)==username)||(dataReader.GetString(1)==chatname))
                    {
                        if (dataReader.GetString(2) == username || (dataReader.GetString(2) == chatname))
                        {
                            str += dataReader.GetString(3) + "——" + dataReader.GetString(4) + "\n";
                            
                        }
                        
                    }
                    

                }
                return str;
            }
            catch
            {
                return "";
            }
            finally { conn.Close(); }
        }
    }
}
