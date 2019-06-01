using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
//using System.Collections.Generic;
using UnityEngine.UI;
using System;
//using System.Timers;
using MySql.Data.MySqlClient;
//using System.Drawing;
//using System.Drawing.Imaging;
using System.IO;


public class chatroom : MonoBehaviour
{



    //以下是同步程序代码
    public string ipaddress = "10.100.1.244";
    public int port = 7799;
    private Socket clientSocket;
    public InputField MessageInput;
    public InputField UserID;
    private Thread thread;
    public Text show;
    public Text click;
    public GameObject Content;
    public GameObject Image;
    private GameObject[] Friends;
    private byte[] data = new byte[1024 * 1024 * 1024];// 数据容器
    private string message = "";
    private int end = -1;
    //private string OtherUserID="";
    private int clientcount = 0;
    private bool islink = false;
    private string Ischat = "";
    private string[] OnlineFriends = new string[20];
    //private string imagePath;//

    void Start()
    {


    }

    void Update()
    {
        //只有在主线程才能更新UI

        if (message != "" && message != null)
        {
            show.text += "\n" + message;
            message = "";
        }
        if (end != -1)
        {
            ReadImage();
            end = -1;//初始化
        }
    }

    void ConnectToServer()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //跟服务器连接
        clientSocket.Connect(new IPEndPoint(IPAddress.Parse(ipaddress), port));
        // 客户端开启线程接收数据
        thread = new Thread(ReceiveMessage);
        thread.Start();
    }
    void ReceiveMessage()
    {
        while (true)
        {
            if (clientSocket.Connected == false)
            {
                break;
            }
            int datalength = clientSocket.Receive(data);
            message = Encoding.UTF8.GetString(data, 0, datalength);
            if (datalength == 0) break;
            string Message = message;
            AnalysisMessage(Message);
        }

    }

    //获取message中的数字，查看是否存在字符“图片已保存到数据库”
    private void AnalysisMessage(string Message)
    {
        end = Message.IndexOf("图片已存入数据库");
        //if (end != -1) ReadImage();
    }

    public void SendMessage(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        clientSocket.Send(data);
    }


    public void OnSendButtonClick()
    {
        string userid = UserID.text;
        string value = MessageInput.text;
        SendMessage(userid + ":" + "\n" + value);
        MessageInput.text = " ";
    }

    //打开文件夹，选择发送的文件或图片
    public void OpenFlie()
    {
        string bgimagePath = "";
        string extion = "png,jpg";
        string path = "";
        path = UnityEditor.EditorUtility.OpenFilePanel("Load Images of Directory", UnityEngine.Application.dataPath, extion);
        if (path != null)
        {
            Debug.Log("获取文件路径成功：" + path);
            bgimagePath = path;
        }
        //OnSendImage(bgimagePath);//将图片路径传给Onsendimage（）函数

        FileStream fs = new FileStream(bgimagePath, FileMode.Open);
        byte[] imagedata = new byte[fs.Length];
        fs.Read(imagedata, 0, imagedata.Length);
        fs.Close();
        AddImage(imagedata);//将图片的byte[]格式传入AddImage函数中，然后存入数据库

        //向服务器发送消息，服务器接收到消息后再发送给客户端，然后客户端再读取数据库中的图片信息
        SendMessage("图片已存入数据库");
    }
    public void AddImage(byte[] imagedata)//将图片的字节数格式保存进数据库
    {

        String connetStr = "server=127.0.0.1;port=3306;user=root;password=123456789;database=chatroomsql;Sslmode=none";//注意Sslmode要赋值为none，否则无法连接到数据库
        MySqlConnection conn = new MySqlConnection(connetStr);
        MySqlCommand command = null;
        try
        {
            conn.Open();
            command = conn.CreateCommand();
            command.CommandText = "INSERT INTO sendimage(userid,image) VALUES(@userid,@image)";
            command.Parameters.AddWithValue("@userid", UserID.text);
            command.Parameters.AddWithValue("@image", imagedata);
            command.ExecuteNonQuery();
        }
        catch
        {

        }
        finally { conn.Close(); }


    }

    public void ReadImage()
    {

        byte[] imagedata = new byte[0];
        String connetStr = "server=127.0.0.1;port=3306;user=root;password=123456789;database=chatroomsql;Sslmode=none";//注意Sslmode要赋值为none，否则无法连接到数据库
        MySqlConnection conn = new MySqlConnection(connetStr);
        conn.Open();
        MySqlCommand command = null;
        MySqlDataReader dataReader = null;
        try
        {
            command = conn.CreateCommand();
            command.CommandText = "SELECT * FROM sendimage";
            dataReader = command.ExecuteReader();
            clientcount = 0;
            while (dataReader.Read())
            {
                imagedata = (byte[])dataReader["image"];//image代表mysql中image列的字符
            }

        }
        catch
        {

        }
        finally { conn.Close(); }
        Image.transform.GetComponent<RawImage>().texture = null;//要想将图片显示框清空才能显示下一张图片
        Texture2D tex = new Texture2D(80, 80);
        tex.LoadImage(imagedata);
        Image.transform.GetComponent<RawImage>().texture = tex;
    }

    public void OnIsLink()
    {

        string userid = UserID.text;
        if (islink == false)
        {
            islink = true;
            string isclick = "断开连接";
            click.text = isclick;
            AddUserId();//向数据库中加入用户名称
            ConnectToServer();
            SendMessage(userid + "-已上线...");
        }

        else
        {
            islink = false;
            string notclick = "连接服务器";
            click.text = notclick;
            DeletUserId();//向数据库中删除用户名称
            SendMessage(userid + "-已下线...");
            //关闭连接，分接收功能和发送功能，both为两者均关闭
            OnDestroy();
        }


    }

    public void FriendsList()
    {
        ReadSql();
        for (int i = 0; i < clientcount; i++)
        {

            string x = i.ToString();
            Friends = new GameObject[clientcount];
            Friends[i] = Content.transform.Find("Friends" + x).gameObject;
            Friends[i].SetActive(true);
            Friends[i].transform.Find("Text").GetComponent<Text>().text = OnlineFriends[i];
            if (Friends[i].transform.Find("Toggle").GetComponent<Toggle>().isOn == true)
            {
                Ischat += OnlineFriends[i] + ",";
            }
        }
        Ischat += "-允许聊天的列表名单";
        SendMessage(Ischat);//向服务器发送聊天列表
        Ischat = "";//初始化ischat
    }

    //向数据库中加入客户端的id
    public void AddUserId()
    {
        String connetStr = "server=127.0.0.1;port=3306;user=root;password=123456789;database=chatroomsql;Sslmode=none";//注意Sslmode要赋值为none，否则无法连接到数据库
        MySqlConnection conn = new MySqlConnection(connetStr);
        MySqlCommand command = null;
        try
        {
            conn.Open();
            command = conn.CreateCommand();
            command.CommandText = "INSERT INTO user(userid) VALUES(@userid)";
            command.Parameters.AddWithValue("@userid", UserID.text);
            command.ExecuteNonQuery();
        }
        catch
        {
            //Console.WriteLine(ex.Message);
        }
        finally { conn.Close(); }


    }


    //向数据库中删除客户端的id
    public void DeletUserId()
    {

        String connetStr = "server=127.0.0.1;port=3306;user=root;password=123456789;database=chatroomsql;Sslmode=none";//注意Sslmode要赋值为none，否则无法连接到数据库
        MySqlConnection conn = new MySqlConnection(connetStr);
        MySqlCommand command = null;
        try
        {
            conn.Open();
            command = conn.CreateCommand();
            command.CommandText = "DELETE FROM user WHERE userid=@userid";
            command.Parameters.AddWithValue("@userid", UserID.text);
            command.ExecuteNonQuery();

        }
        catch
        {

        }
        finally { conn.Close(); }
    }


    public void ReadSql()
    {
        Array.Clear(OnlineFriends, 0, 20);//清空数组，否则会有残留的元素
        String connetStr = "server=127.0.0.1;port=3306;user=root;password=123456789;database=chatroomsql;Sslmode=none";//注意Sslmode要赋值为none，否则无法连接到数据库
        MySqlConnection conn = new MySqlConnection(connetStr);
        conn.Open();
        MySqlCommand command = null;
        MySqlDataReader dataReader = null;
        try
        {
            command = conn.CreateCommand();
            command.CommandText = "SELECT * FROM user";
            dataReader = command.ExecuteReader();
            clientcount = 0;
            while (dataReader.Read())
            {
                OnlineFriends[clientcount] = dataReader.GetString(0);
                clientcount++;
            }

        }
        catch
        {

        }
        finally { conn.Close(); }

    }

    /**
     * unity自带方法
     * 停止运行时会执行
     * */
    void OnDestroy()
    {
        string userid = UserID.text;
        if (islink == false)
        {

            //关闭连接，分接收功能和发送功能，both为两者均关闭
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

    }
}