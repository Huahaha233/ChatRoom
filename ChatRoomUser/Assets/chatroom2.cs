using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using MySql.Data.MySqlClient;
using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using UnityEngine.SceneManagement;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

public class chatroom2 : MonoBehaviour {

    //以下是异步程序代码
    //public InputField hostinput;//IP地址
    //public InputField portinput;//端口号
    private Text recvText;//信息显示UI
    public GameObject image;//图片显示框
    public GameObject content;//挂载UI
    public GameObject Text;//文本显示预制体
    //private int imagecount;//图片数量
    public string recvStr;//接收到的信息
    public InputField textInput;//聊天输入框
    public static string chatname;//聊天对象的名称
    private byte[] imgdata=new byte[1024*1024];//发送图片字节流的形式
    private byte[] imagedata=new byte[1024*1024];//发送图片字节流的形式
    Socket socket;
    const int BUFFER_SIZE = 1024;
    public byte[] readBuff = new byte[BUFFER_SIZE];
    private int imagelenth=0;
    private bool isend=false;//判断自己是否全部存入imagedata中
    //private int iscreat = 2;
    private int imagebytecount=0;
    private int imagecount;//接收到的图片的字节长度
    private int imagenum = 1;//图片的序号
    private int textnum = 1;//文本框的序号

    private void Start()
    {
        ShowName();//在聊天框顶部显示聊天对象的名称
        CreatText();//创建聊天字符框，是为了不让图片覆盖住文字
        Connetion();//连接服务器 
        SendName();//向客户端发送用户昵称和聊天对象的昵称
    }
    private void Update()
    {
        recvText.text = recvStr;
        if (isend==true)
        {
            ReadImage();
            Array.Clear(imagedata,0,imagedata.Length);//将imagedata初始化，给下一张图片做准备
            textnum++;
            CreatText();//在图片显示完成后要立马再建造一个文本框
            isend = false;
        }
    }
    //在chatroom的顶部显示聊天对象的名称
    public void ShowName()
    {
        transform.Find("name").GetComponent<Text>().text = chatname;
    }
    public void CreatText()
    {
        recvStr = "\n";
        GameObject pre = Instantiate(Text);
        pre.transform.parent = content.transform;
        pre.name = "RecText_"+textnum;
        recvText = GameObject.Find("Canvas/ShowInfo/Viewport/Content/RecText_" + textnum).GetComponent<Text>();
        recvStr = "\n";
    }

    //连接服务器
    public void Connetion()
    {
        //transform.Find("Link/Text").GetComponent<Text>().text = "断开";
        //清空聊天框
        //recvText.text = "";
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        string host = "192.168.137.1";
        int port = int.Parse("1234");
        socket.Connect(host, port);
        //clientText.text = "客户端地址：" + socket.LocalEndPoint.ToString();
        socket.BeginReceive(readBuff,0,BUFFER_SIZE,SocketFlags.None,ReceiveCb, null);
    }

    private void ReceiveCb(IAsyncResult ar)
    {
        try
        {
            int count = socket.EndReceive(ar);
            string str = Encoding.UTF8.GetString(readBuff, 0, count);//数据处理
            if (imagelenth != 0)
            {
                readBuff.CopyTo(imagedata, imagebytecount);
                imagebytecount += 1024;
                imagelenth--;
                if (imagelenth == 0)
                {
                    isend = true;
                    imagebytecount =0;//初始化
                }
                str = "";
            }
            int strint = str.IndexOf("@用户发送图片@");
            if (strint != -1)
            {
                
                imagecount = int.Parse(str.Replace("@用户发送图片@", ""));
                //imagelenth = (imagecount / 1024) + 1;
                imagelenth = 1024;
                recvStr += "图片：" + "\n";
            }

            Debug.Log("imagelenth:"+imagelenth);
            if (imagelenth == 0)
            {
                recvStr +=str + "\n";;
            }
            

            //Debug.Log(recvStr);
            //继续接收
            socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
        }
        catch(Exception e)
        {
            //recvText.text += "连接已断开";
            socket.Close();
        }
    }
    public void Send()//发送按钮
    {
        string str = Login.user_name+":"+textInput.text;
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        try
        {
            socket.Send(bytes);
            //清空聊天框
            textInput.text = "";
        }
        catch
        {

        }
    }

    //向客户端发送用户昵称和聊天对象的昵称
    public void SendName()
    {
        string str = Login.user_name + "向" + chatname+"发起单人聊天";
        SendChange(str);
    }

    public void SendChange(string str)//转换,发送函数
    {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        try
        {
            socket.Send(bytes);
        }
        catch
        {

        }
    }


    //打开文件夹，选择发送的文件或图片
    public void OpenFlie()
    {
        string bgimagePath = "";
        string extion = "png,jpg";
        string path = "";
        path = UnityEditor.EditorUtility.OpenFilePanel("Load Images of Directory", Application.dataPath, extion);
        if (path != null)
        {
            Debug.Log("获取文件路径成功：" + path);
            bgimagePath = path;
        }
        //OnSendImage(bgimagePath);//将图片路径传给Onsendimage（）函数

        FileStream fs = new FileStream(bgimagePath, FileMode.OpenOrCreate, FileAccess.Read);
        byte[] imagedata = new byte[fs.Length];
        BinaryReader strread = new BinaryReader(fs);
        strread.Read(imagedata, 0, imagedata.Length);
        
        fs.Close();
        SendChange("@用户发送图片@"+imagedata.Length);//在发送图片字节流前向服务器发出提示
        imgdata = imagedata;
        Debug.Log("sendimagedata"+imgdata.Length);
        //Invoke("SendByteImg",);//延迟执行
        SendByteImg();
    }
    public void SendByteImg()
    {
        socket.Send(imgdata);//向服务器发送图片字节流
    }

    //将从服务器中接受到的图片字节流转换成为Texture格式显示在客户端UI中
    public void ReadImage()
    {
        
        Texture2D tex = new Texture2D(80, 80);
        tex.LoadImage(ByteCut(imagedata));
      
        GameObject pre = Instantiate(image);
        pre.transform.parent = content.transform;
        pre.name = "images_" + imagenum;//给克隆后的UI命名，方便管理
        pre.transform.GetComponent<RawImage>().texture = tex;
        imagenum++;
        

        //imagedata = new byte[1024 * 1024];//将imagedata初始化，给下一张图片做准备

        //byte[] imagedata = new byte[0];
        //string connetStr = "server=127.0.0.1;port=3306;user=root;password=123456789;database=chatroomsql;Sslmode=none";//注意Sslmode要赋值为none，否则无法连接到数据库
        //MySqlConnection conn = new MySqlConnection(connetStr);
        //conn.Open();
        //MySqlCommand command = null;
        //MySqlDataReader dataReader = null;
        //try
        //{
        //    command = conn.CreateCommand();
        //    command.CommandText = "SELECT * FROM sendimage";
        //    dataReader = command.ExecuteReader();
        //    while (dataReader.Read())
        //    {
        //        imagedata = (byte[])dataReader["image"];//image代表mysql中image列的字符
        //    }

        //}
        //catch
        //{

        //}
        //finally { conn.Close(); }

    }

    public byte[] ByteCut(byte[] b)
    {
        
        byte[] lastbyte = new byte[imagecount];
        for (var i = 0; i < imagecount; i++)
        {
            lastbyte[i] = b[i];
        }
        Debug.Log("lastbyte"+lastbyte.Length);
        return lastbyte;
    }
    //返回好友列表
    public void Out()
    {
        SceneManager.LoadScene("FriendsList");
    }
}
