using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ActButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ActiveButton();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    //附加脚本到UI上，并且开启按钮监听
    public void ActiveButton()
    {
        Button button = this.GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }
    private void OnClick()
    {
        //将聊天对象的名称传递给chatroom2中的参数
        chatroom2.chatname = this.transform.parent.name.Replace("friends_",null);

        SceneManager.LoadScene("chatroom2.0");//跳转
    }
}
