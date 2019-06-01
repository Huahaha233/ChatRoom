using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace chatroom
{
    class Program
    {
        static void Main(string[] args)
        {
            CreatDatabase();
            CreatTable();
            Server server = new Server();
            server.Start("192.168.137.1", 1234);
            
        }
        //创建数据库
        private static void CreatDatabase()//创建数据库
        {
            string creatdatabase = "Data Source=localhost;Persist Security Info=yes;UserId=root;PWD=123456;SslMode = none";
            string MysqlStatment = "CREATE DATABASE IF NOT EXISTS chatroom DEFAULT CHARSET utf8 COLLATE utf8_general_ci;";
            MySqlConnection conn = new MySqlConnection(creatdatabase);
            try
            {
                conn.Open();
                MySqlCommand comm = new MySqlCommand(MysqlStatment, conn);
                comm.ExecuteNonQuery();
                Console.WriteLine("数据库建立完成");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                conn.Close();
            }

        }

        private static void CreatTable()//创建数据库表
        {
            //下表为用户注册信息，存放用户的用户名密码等信息
            string sql = "CREATE TABLE IF NOT EXISTS `user` (`user_id` INT UNSIGNED AUTO_INCREMENT,  `user_name` CHAR(10) NOT NULL, `user_psw` CHAR(10) NOT NULL,PRIMARY KEY(`user_id`)) DEFAULT CHARSET = utf8";
            ChangeMysql(sql);

            //小表为用户在线表，记录在线用户
            string sql1 = "CREATE TABLE IF NOT EXISTS `user_login` (`user_id` INT UNSIGNED AUTO_INCREMENT,  `user_name` CHAR(10) NOT NULL, `login_time` DATETIME NOT NULL,PRIMARY KEY(`user_id`)) DEFAULT CHARSET = utf8";
            ChangeMysql(sql1);

            //小表为用户在线表，记录在线用户
            string sql2 = "CREATE TABLE IF NOT EXISTS `user_chat` (`chat_id` INT UNSIGNED AUTO_INCREMENT,  `user_name` CHAR(10) NOT NULL, `chat_name` CHAR(10) NOT NULL,`chatcontent` CHAR(255) NOT NULL,`imagedata` MEDIUMBLOB,`chat_time` CHAR(20) NOT NULL,PRIMARY KEY(`chat_id`)) DEFAULT CHARSET = utf8";
            ChangeMysql(sql2);
            //string sql2 = "CREATE TABLE IF NOT EXISTS `User_registe` (`registe_num` INT UNSIGNED AUTO_INCREMENT,  `User_name` CHAR(10) NOT NULL, `User_psw` CHAR(20) NOT NULL,`registe_time` DATETIME NOT NULL,PRIMARY KEY(`registe_num`)) DEFAULT CHARSET = utf8";
            //ChangeMysql(sql2);

        }

        private static void ChangeMysql(string Mysql_change)//对数据库进行操作，通过其他函数传入的参数进行操作
        {
            string Mysql_str = "server=localhost;user id=root;password=123456;database=chatroom;SslMode = none";
            MySqlConnection myconn = new MySqlConnection(Mysql_str);
            try
            {
                myconn.Open();
                MySqlCommand mycomm = new MySqlCommand(Mysql_change, myconn);
                mycomm.ExecuteNonQuery();
                Console.WriteLine("建表完成");
            }
            catch (MySqlException ex)
            {
            }
            finally
            {
                myconn.Close();
            }
        }
        //同步程序
        //    public static string UID="";
        //    public static int clientcount=0;
        //    private static byte[] buffer = new byte[2048];//存储数据
        //    static List<Client> clientlists = new List<Client>();
        //    public static string[] onchat = new string[20];//允许聊天的名单列表数组
        //    //public static bool IsRestar = false;//判断是否将onchat数组初始化（为了在onchat中重新加入元素）
        //    /// <summary>
        //    /// 广播信息
        //    /// </summary>
        //    /// <param name="message"></param>
        //    public static void BroadcastMessage(string message,bool IsSQL)
        //    {
        //        string online = "";
        //        string Message = message;
        //        try
        //        {
        //            int end1 = Message.IndexOf("-允许聊天的列表名单");
        //            online = Message.Substring(0, end1);
        //            Array.Clear(onchat,0,onchat.Length);//清空数组
        //            message = "";//将message内容清空，这样客户端就不会现实“-允许聊天的列表名单”这样的内容
        //            if (online != "")
        //            {
        //                onchat = online.Split(',');
        //            }
        //        }
        //        catch
        //        {

        //        }

        //        var NotConnectClient = new List<Client>();//掉线线客户端集合
        //        foreach (var client in clientlists)
        //        {
        //            if (client.Connected())//判断是否在线
        //            { 
        //                for (int i = 0; i < clientlists.Count; i++)
        //                {
        //                    if ((onchat[i] == client.User) ||IsSQL == true)
        //                       //ISSQL如果为true的话，证明发送的数据为数据库中读取的数据
        //                    {
        //                        client.SendMessage(message);
        //                    }
        //                    else
        //                    {
        //                        client.SendMessage("");
        //                    }
        //                }
        //                //client.SendMessage(message);
        //            }
        //            else
        //            {
        //                NotConnectClient.Add(client);
        //            }
        //        }
        //        //将掉线的客户端从集合里移除
        //        foreach (var temp in NotConnectClient)
        //        {
        //            clientlists.Remove(temp);
        //        }
        //    }

        //    /*连接数据库*/
        //    public static void LinkMySql(string User, string message)
        //    { 
        //        String connetStr = "server=127.0.0.1;port=3306;user=root;password=123456789;database=chatroomsql;Sslmode=none";//注意Sslmode要赋值为none，否则无法连接到数据库
        //        MySqlConnection conn = new MySqlConnection(connetStr);
        //        MySqlCommand command=null;
        //        try
        //        {
        //            conn.Open();
        //            command = conn.CreateCommand();
        //            command.CommandText = "INSERT INTO chatroom(userid,chatcontent) VALUES(@userid,@chatcontent)";
        //            command.Parameters.AddWithValue("@userid", User);
        //            command.Parameters.AddWithValue("@chatcontent", message);
        //            command.ExecuteNonQuery();
        //        }
        //        catch
        //        {
        //            //Console.WriteLine(ex.Message);
        //        }
        //        finally { conn.Close(); }

        //    }

        //    public static void ReadSql()
        //    {
        //        String connetStr = "server=127.0.0.1;port=3306;user=root;password=123456789;database=chatroomsql;Sslmode=none";//注意Sslmode要赋值为none，否则无法连接到数据库
        //        MySqlConnection conn = new MySqlConnection(connetStr);
        //        conn.Open();
        //        MySqlCommand command=null;
        //        MySqlDataReader dataReader = null;
        //        try
        //        {

        //            command = conn.CreateCommand();
        //            command.CommandText = "SELECT * FROM chatroom";
        //            dataReader = command.ExecuteReader();
        //            while (dataReader.Read())
        //            {
        //                //由于dataReader.GetString(1)中已经包含了userid与message，所以这里不需要用到dataReader.GetString(0)
        //                Console.WriteLine("{0}",dataReader.GetString(1));
        //                string message =dataReader.GetString(1);
        //                BroadcastMessage(message, true);
        //            }

        //        }
        //        catch
        //        {

        //        }
        //        finally { conn.Close(); }

        //    }


        //    static void Main(string[] args)
        //    {

        //        string ipaddres = "";
        //        foreach (IPAddress _ipaddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        //        {
        //            if (_ipaddress.AddressFamily.ToString() == "InterNetwork")
        //            {  
        //               ipaddres = _ipaddress.ToString();
        //            }
        //        }

        //        Socket tcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //        IPAddress ipAddress = IPAddress.Parse(ipaddres);//10.100.4.133
        //        IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 7799);
        //        tcpServer.Bind(ipEndPoint);
        //        Console.WriteLine("服务器开启....");
        //        tcpServer.Listen(100);
        //        //循环，每连接一个客户端建立一个Client对象

        //        while (true)
        //        {
        //            Socket clientSocket = tcpServer.Accept();//暂停等待客户端连接，连接后执行后面的代码
        //            Client client = new Client(clientSocket);//连接后，客户端与服务器的操作封装到Client类中
        //            clientlists.Add(client);//放入集合
        //            clientcount = clientlists.Count;
        //        }

        //    }

        //}
    }
}
