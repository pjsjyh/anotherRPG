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
using SettingAccountManager;

namespace LoginManager
{
    public class Login
    {
        public static async Task<bool> GoLoginAccount(string id, string password, TextMeshProUGUI duplicateErrorText)
        {
            bool isSuccess = await LoginAccount(id, password, duplicateErrorText);
            return isSuccess;
        }
        private static async Task<bool> LoginAccount(string id, string password, TextMeshProUGUI duplicateErrorText)
        {
            var values = new Dictionary<string, string>
        {
            { "id", id },
            { "password", password }
        };

            //var content = new FormUrlEncodedContent(values);

            try
            {
                UnityWebRequest response = await ServerManager.Instance.PostAsync(ApiUrls.LoginUrl, values);
                if (response == null)
                {
                    Debug.LogError("UnityWebRequest response is null. Check the request initialization.");
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
                        Debug.Log("Error detected in response.");
                        duplicateErrorText.gameObject.SetActive(true);
                        duplicateErrorText.text = "failed login";
                        response.Dispose();
                        return false;
                    }
                    else
                    {
                        Debug.Log("No error found. Processing success response.");
                        await SettingAccount.DoSettingAccount(responseBody); // 비동기 메서드로 처리
                        response.Dispose();
                        return true;
                    }
                }
                else
                {
                    Debug.LogError("Error: HTTP Response Code " + response.responseCode);
                    Debug.LogError("Error Details: " + response.error);
                    response.Dispose();
                    return false;
                }
                return false;
                //HttpResponseMessage response = await ServerManager.Instance.PostAsync(ApiUrls.LoginUrl, content);

                //if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) // 401 Unauthorized 처리
                //{
                //    duplicateErrorText.gameObject.SetActive(true);
                //    duplicateErrorText.text = "failed login";
                //    return false;
                //}
                //else
                //{
                //    response.EnsureSuccessStatusCode();
                //    string responseBody = await response.Content.ReadAsStringAsync();
                //    Debug.Log("Response: 로그인 성공" + responseBody);
                //    //await SettingAccount(responseBody);
                //    await SettingAccount.DoSettingAccount(responseBody);

                //    return true;
                //}
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"Request error: {e.Message}");
                return false;
            }
        }
    }

}
