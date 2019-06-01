using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FriendsList : MonoBehaviour {
    public GameObject friend;//UI预制体
    public GameObject content;//挂载好友列表UI
    private int count=0;//user用户表中已注册用户的数量
	// Use this for initialization
	void Start () {
        //Clone();
        ReadSql();
        ReadOnline();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    //点击退处按钮
    public void Out()
    {
        DeleteInfo();
        SceneManager.LoadScene("Login");
    }
    //删除信息
    private void DeleteInfo()
    {
        /*当存储的文件过大时，一定要在MySQL的my.ini文件中修改max_allowed_packet的值，并重新启动MySQL*/
        //string nowtime = System.DateTime.Now.Year + "." + System.DateTime.Now.Month + "." + System.DateTime.Now.Day + "." + System.DateTime.Now.Hour + "." + System.DateTime.Now.Minute;
        string sql = "DELETE FROM user_login WHERE user_name = "+Login.user_name+"";
        ChangeMysql(sql);
    }

    private void ChangeMysql(string Mysql_change)//对数据库进行操作，通过其他函数传入的参数进行操作
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

    //读取数据库表
    public void ReadSql()
    {
        string connetStr = "server=localhost;user id=root;password=123456;database=chatroom";//注意Sslmode要赋值为none，否则无法连接到数据库
        MySqlConnection conn = new MySqlConnection(connetStr);
        conn.Open();
        MySqlCommand command = null;
        MySqlDataReader dataReader = null;
        try
        {
            command = conn.CreateCommand();
            command.CommandText = "SELECT * FROM user";
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                count++;
                Clone(dataReader.GetString(1));
            }

        }
        catch
        {

        }
        finally { conn.Close(); }

    }
    public void Clone(string name)
    {
        GameObject pre = Instantiate(friend);
        pre.transform.parent =content.transform ;
        pre.name = "friends_"+name;//给克隆后的UI命名，方便管理
        pre.transform.GetComponent<Text>().text = name;

    }
   
    //读取用户在线数据库表，然后将在线用户的状态改为在线
    public void ReadOnline()
    {
        string connetStr = "server=localhost;user id=root;password=123456;database=chatroom";//注意Sslmode要赋值为none，否则无法连接到数据库
        MySqlConnection conn = new MySqlConnection(connetStr);
        conn.Open();
        MySqlCommand command = null;
        MySqlDataReader dataReader = null;
        try
        {
            command = conn.CreateCommand();
            command.CommandText = "SELECT * FROM user_login";
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                string login_name = dataReader.GetString(1);//将在线用户的名单赋值给login_name
                Oniline(login_name);
            }

        }
        catch
        {

        }
        finally { conn.Close(); }
    }
    private void Oniline(string name)
    {   //通过查找将状态改为在线
        GameObject.Find("Canvas/Scroll View/Viewport/Content/friends_" + name + "/online").GetComponent<Text>().text = "在线";
        //transform.Find("Content/friends_" + name+"/online").GetComponent<Text>().text = "在线";无用
    }
}
