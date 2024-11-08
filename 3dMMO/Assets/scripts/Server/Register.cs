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

namespace RegisterManager
{
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

            //var content = new FormUrlEncodedContent(values);
           // string jsonData = JsonConvert.SerializeObject(values);//json문자열로 변환
            try
            {
                //HttpResponseMessage response = await ServerManager.Instance.PostAsync(ApiUrls.RegisterUrl, content);
                UnityWebRequest response = await ServerManager.Instance.PostAsync(ApiUrls.RegisterUrl, values);
                Debug.Log(response);
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
                        duplicateErrorText.text = "Username already exists";
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
                //if (response.StatusCode == System.Net.HttpStatusCode.Conflict) // 409 Conflict 처리
                //{
                //    string responseBody = await response.Content.ReadAsStringAsync();
                //    var errorResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBody);
                //    foreach (var kvp in errorResponse)
                //    {
                //        Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value}");
                //    }
                //    Debug.Log(errorResponse);

                //    // 키가 존재하는지 먼저 확인한 후 접근
                //    if (errorResponse.ContainsKey("error_type"))
                //    {
                //        if (errorResponse["error_type"] == "duplicate_id")
                //        {
                //            Debug.LogError("Error: ID already exists.");
                //            duplicateErrorText.gameObject.SetActive(true);
                //            duplicateErrorText.text = "ID already exists";
                //        }
                //        else if (errorResponse["error_type"] == "duplicate_name")
                //        {
                //            Debug.LogError("Error: Username already exists.");
                //            duplicateErrorText.gameObject.SetActive(true);
                //            duplicateErrorText.text = "Username already exists";
                //        }
                //    }
                //    else
                //    {
                //        Debug.LogError("Unknown error: No error_type found.");
                //    }

                //    return false;
                //}
                //else
                //{
                //    response.EnsureSuccessStatusCode();
                //    string responseBody = await response.Content.ReadAsStringAsync();
                //    //await SettingAccount(responseBody);
                //    await SettingAccount.DoSettingAccount(responseBody);
                //    //InitializePlayer(jsonResponse["playerinfo"]);
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
