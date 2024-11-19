using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro 네임스페이스 추가
using UnityEngine.SceneManagement;
using CharacterInfo;
using LoginManager;
using RegisterManager;
public class LoginCheck : MonoBehaviour
{
    // Start is called before the first frame update
    bool isLogin = true;
    public TMP_InputField idInputField; // TextMeshPro의 TMP_InputField 사용
    public TMP_InputField usernameInputField; // TextMeshPro의 TMP_InputField 사용
    public TMP_InputField passwordInputField; // TextMeshPro의 TMP_InputField 사용
    public TextMeshProUGUI createAccountText;
    public TextMeshProUGUI loginBtnText;
    public TextMeshProUGUI signupBtnText;
    public TextMeshProUGUI duplicateErrorText;
    public GameObject createAccountSuccesPanel;

    public async void signAccount()
    {
        duplicateErrorText.gameObject.SetActive(false);
        if (isLogin)
        {
            string id = idInputField.text;
            string password = passwordInputField.text;
            if (id == "manager")
            {
                ChaInfoOther managerInfo = new ChaInfoOther
                {
                    _attack = 9999,
                    _defense = 9999,
                    _critical = 9999,
                    _speed = 100,
                    _luck = 9999
                };

                CharacterManager.Instance.InitializePlayer(managerInfo, "manager", 100, 100, 999999, 999);
                sceneChange();
            }
            else
            {
                bool isSuccess = await Login.GoLoginAccount(id, password, duplicateErrorText);
                if (isSuccess)
                {
                    sceneChange();
                }
            }

        }
        else
        {

            if (fieldFull())
            {

                string id = idInputField.text;
                string password = passwordInputField.text;
                string username = usernameInputField.text;
                bool isSuccess = await Register.CreateAccount(id, password, username, duplicateErrorText);
                if (isSuccess)
                {
                    createAccountSuccesPanel.SetActive(true);
                }
            }
            else
            {
                duplicateErrorText.gameObject.SetActive(true);
                duplicateErrorText.text = "Please fill in the blanks";
                Debug.Log("ID: " + idInputField.text + ", Username: " + usernameInputField.text + ", Password: " + passwordInputField.text);
            }
        }

    }
    public bool fieldFull()
    {
        return !string.IsNullOrEmpty(idInputField.text) &&
               !string.IsNullOrEmpty(usernameInputField.text) &&
               !string.IsNullOrEmpty(passwordInputField.text);
    }

    public void changeMode()
    {
        if (isLogin)
        {
            createAccountText.gameObject.SetActive(true);
            usernameInputField.gameObject.SetActive(true);
            loginBtnText.text = "create Account";
            signupBtnText.text = "BACK";
            isLogin = false;
        }
        else
        {
            createAccountText.gameObject.SetActive(false);
            usernameInputField.gameObject.SetActive(false);
            loginBtnText.text = "login";
            signupBtnText.text = "SIGN UP";
            isLogin = true;
        }
    }
    public void sceneChange()
    {
        SceneTransManager.Instance.FadeAndLoadScene("GameScene");
    }
    public void turnOffcreateAccountSuccesPanel()
    {
        createAccountSuccesPanel.SetActive(false);
    }
}