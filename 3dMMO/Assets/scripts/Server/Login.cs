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
using Newtonsoft.Json.Linq;
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
                    Debug.Log("서버 응답: " + responseBody); // 서버 응답 로그 출력

                    try
                    {
                        JObject jsonResponse = JObject.Parse(responseBody); // JSON 파싱

                        if (jsonResponse.ContainsKey("error_type")) // 서버에서 오류 발생
                        {
                            string errorMessage = jsonResponse["message"]?.ToString() ?? "로그인에 실패했습니다."; // 오류 메시지 가져오기
                            Debug.LogWarning("Error detected: " + errorMessage); // 로그 출력
                            duplicateErrorText.gameObject.SetActive(true);
                            duplicateErrorText.text = errorMessage; // 오류 메시지를 UI에 표시
                            response.Dispose();
                            return false;
                        }
                        else
                        {
                            await SettingAccount.DoSettingAccount(responseBody); // 성공 시 계정 설정
                            response.Dispose();
                            return true;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError("JSON 파싱 오류: " + ex.Message);
                        duplicateErrorText.gameObject.SetActive(true);
                        duplicateErrorText.text = "서버 응답 처리 중 오류 발생";
                        response.Dispose();
                        return false;
                    }
                }
                else
                {
                    string errorResponse = response.downloadHandler.text; // 서버의 오류 응답을 확인
                    Debug.LogError("HTTP Response Code: " + response.responseCode);
                    Debug.LogError("Error Details: " + response.error);
                    Debug.LogError("Server Response: " + errorResponse);

                    duplicateErrorText.gameObject.SetActive(true);
                    duplicateErrorText.text = "없는 정보입니다.";
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
