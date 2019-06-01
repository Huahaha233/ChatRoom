using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text;

public class Login : MonoBehaviour {
    public InputField Guser;
    public InputField Gpsw;
    Socket socket;
    public static string user_name;//用户名，静态变量
    // Use this for initialization
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public int ReadMysql(string name, string psw)
    {
        int find = 0;
        string connetStr = "server=localhost;user id=root;password=123456;database=chatroom";//注意Sslmode要赋值为none，否则无法连接到数据库   
        MySqlConnection conn = new MySqlConnection(connetStr);
        MySqlCommand command = null;
        MySqlDataReader dataReader = null;
        try
        {
            conn.Open();
            command = conn.CreateCommand();
            command.CommandText = "SELECT * FROM user";
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                if (name == dataReader.GetString(1) && psw == dataReader.GetString(2))
                {
                    find = 1;//find为1时，证明数据库中用户名已存在
                    break;
                }

            }
        }
        catch
        {
            Debug.Log("未能连接到数据库！");
        }
        finally { conn.Close(); }
        return find;

    }

    //连接服务器
    public void Link()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        string host = "10.100.120.254";
        int port = int.Parse("1234");
        socket.Connect(host, port);

        Send(user_name+":已连接！");
        //Send(user_name);
        socket.Close();//关闭服务器
    }

    //向服务器端发送消息
    public void Send(string str)
    {
        try
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            socket.Send(bytes);
        }
        catch { }
        
    }

    //用户登录成功后将信息添加到数据库
    public void AddInfo()
    {
        /*当存储的文件过大时，一定要在MySQL的my.ini文件中修改max_allowed_packet的值，并重新启动MySQL*/
        string nowtime = System.DateTime.Now.Year + "." + System.DateTime.Now.Month + "." + System.DateTime.Now.Day + "." + System.DateTime.Now.Hour + "." + System.DateTime.Now.Minute;
        string sql = "insert into user_login(user_id,user_name,login_time) values(NULL,'" + Guser.text + "','" + nowtime + "')";
        Debug.Log(sql);
        ChangeMysql(sql);
    }
   
    public void ChangeMysql(string Mysql_change)//对数据库进行操作，通过其他函数传入的参数进行操作
    {
        string Mysql_str = "server=localhost;user id=root;password=123456;database=chatroom";
        MySqlConnection myconn = new MySqlConnection(Mysql_str);
        try
        {
            myconn.Open();
            MySqlCommand mycomm = new MySqlCommand(Mysql_change, myconn);
            mycomm.ExecuteNonQuery();
            Debug.Log("写入成功");
        }
        catch (MySqlException ex)
        {
        }
        finally
        {
            myconn.Close();
        }
    }

   
    /*判断注册用户名是否存在*/
    public void Judge()
    {
        
        if (ReadMysql(Guser.text, Gpsw.text) == 1)
        {
            transform.Find("Tips").GetComponent<Text>().text = "登录成功！";
            //Collsion.isregiste = true;//给类LinkMysql中的值赋值，证明本用户不是注册用户
            user_name = Guser.text;//给静态变量赋值
            Link();
            AddInfo();
            Invoke("ToScene", 1f);//延迟1秒后跳转页面
        }
        else
        {
            transform.Find("Tips").GetComponent<Text>().text = "信息错误！";
            //VerificationCode.Click();//验证码错误使用需要刷新验证码

        }
    }
    public void ToScene()
    {
        SceneManager.LoadScene("FriendsList");
    }
    public void ToRegister()
    {
        SceneManager.LoadScene("Register");
    }

    
}
