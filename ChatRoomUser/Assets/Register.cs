using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Register : MonoBehaviour {
    public InputField Guser;
    public InputField Gpsw;
    public InputField Grepsw;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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

    public void AddInfoToMysql(string name, string psw)//向数据库中添加数据
    {
        /*当存储的文件过大时，一定要在MySQL的my.ini文件中修改max_allowed_packet的值，并重新启动MySQL*/
        //string nowtime = System.DateTime.Now.Year + "." + System.DateTime.Now.Month + "." + System.DateTime.Now.Day + "." + System.DateTime.Now.Hour + "." + System.DateTime.Now.Minute;
        string sql = "insert into user(user_id,user_name,user_psw) values(NULL,'" + name + "','" + psw + "')";
        Debug.Log(sql);
        ChangeMysql(sql);
    }

    public int ReadMysql(string name)
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
                if (name == dataReader.GetString(1))
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



    /*判断注册用户名是都存在*/
    public void Judge()
    {
        if (ReadMysql(Guser.text) == 1&&Guser.text!=null)
        {
            transform.Find("Tips").GetComponent<Text>().text = "用户名已存在！";
            //VerificationCode.Click();//验证码错误使用需要刷新验证码
        }
        else if (Gpsw.text != Grepsw.text&&Gpsw.text!=null)
        {
            transform.Find("Tips").GetComponent<Text>().text = "密码输入错误！";
        }
        else
        {
            transform.Find("Tips").GetComponent<Text>().text = "注册成功！";
            AddInfoToMysql(Guser.text, Gpsw.text);
            //Collsion.isregiste = true;//给类LinkMysql中的值赋值，证明本用户不是注册用户
            Invoke("ToScene", 2f);//延迟2。秒后跳转页面
        }

    }

    public void ToScene()
    {
        SceneManager.LoadScene("Login");
    }
}
