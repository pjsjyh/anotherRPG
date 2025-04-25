using UnityEngine;
using TMPro; // TextMeshPro 네임스페이스 추가
using UnityEngine.Networking;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic; //dictionary
using Newtonsoft.Json;
using ApiUtilities;
using MyServerManager;
using CharacterInfo;
namespace RegisterManager
{
    public class RegisterResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public ChaInfoOther playerinfo { get; set; }
    }
    public class Register
    {
        public static async Task<bool> CreateAccount(string id, string password, string username, TextMeshProUGUI duplicateErrorText)
        {
            bool isSuccess = await RegisterNewAccount(id, password, username, duplicateErrorText);
            return isSuccess;
        }
        private static async Task<bool> RegisterNewAccount(string id, string password, string username, TextMeshProUGUI duplicateErrorText)
        {
            var values = new Dictionary<string, string>
        {
            { "id", id },
            { "password", password },
            { "username", username }
        };

            try
            {
                //HttpResponseMessage response = await ServerManager.Instance.PostAsync(ApiUrls.RegisterUrl, content);
                UnityWebRequest response = await ServerManager.Instance.PostAsync(ApiUrls.RegisterUrl, values);
                Debug.Log(response.result);
                if (response == null)
                {
                    response.Dispose();
                    return false;
                }
                if (response.result == UnityWebRequest.Result.Success)
                {
                    string responseBody = response.downloadHandler.text;
                    Debug.Log(responseBody);
                    // JSON 응답을 분석하여 처리
                    if (responseBody.Contains("error_type"))
                    {
                        duplicateErrorText.gameObject.SetActive(true);
                        duplicateErrorText.text = "Username already exists";
                        response.Dispose();
                        return false;
                    }
                    else
                    {
                        GameManager.Instance.saveData(responseBody);
                        response.Dispose();

                        //await SettingAccount.DoSettingAccount(responseBody); // 비동기 메서드로 처리

                        return true;
                    }
                }
                else
                {
                    string responseBody = response.downloadHandler.text;
                    Debug.Log(responseBody);
                    if (responseBody.Contains("error_type"))
                    {
                        Debug.Log("Error detected in response.");
                        duplicateErrorText.gameObject.SetActive(true);
                        var errorResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBody);
                        if (errorResponse["error_type"] == "duplicate_id")
                        {
                            duplicateErrorText.text = "아이디가 이미 존재합니다.";
                        }
                        else if (errorResponse["error_type"] == "duplicate_username")
                        {
                            duplicateErrorText.text = "같은 닉네임이 존재합니다.";
                        }
                        response.Dispose();
                        return false;
                    }
                    else
                    {
                        Debug.LogError("Error: HTTP Response Code " + response.responseCode);
                        Debug.LogError("Error Details: " + response.error);
                        response.Dispose();
                        return false;
                    }
                    
                }
              
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"Request error: {e.Message}");
                return false;
            }
        }
    }

}
