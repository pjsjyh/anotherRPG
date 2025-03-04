using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using UnityEngine.Networking;
using System.Text;
namespace MyServerManager
{
    public interface IServerManager
    {
        Task<UnityWebRequest> PostAsync(string url, Dictionary<string, string> formData);
    }

    public class ServerManager : IServerManager
    {
        private static ServerManager _instance;
        private static readonly object _lock = new object(); // 스레드 안전성 보장

        // HttpClient는 재사용할 수 있도록 정적으로 설정
        //private readonly HttpClient client;

        private ServerManager()
        {
        }

        // Singleton 인스턴스 접근자
        public static ServerManager Instance
        {
            get
            {
                // 스레드 안전성 보장을 위해 lock을 사용
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ServerManager();
                    }
                    return _instance;
                }
            }
        }

        // IServerManager 인터페이스의 메서드 구현
        //public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        //{
        //    return await client.PostAsync(url, content);
        //}
        public async Task<UnityWebRequest> PostAsync(string url, Dictionary<string, string> formData)
        {
            string encodedData = string.Join("&", formData.Select(kv => $"{UnityWebRequest.EscapeURL(kv.Key)}={UnityWebRequest.EscapeURL(kv.Value)}"));
            byte[] bodyRaw = Encoding.UTF8.GetBytes(encodedData);
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            return request; // 이 경우 호출한 쪽에서 Dispose()를 호출해야 함

        }
        public async Task<UnityWebRequest> PostJsonAsyncJson(string url, string jsonData)
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json"); // 🔹 JSON 전송

            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            return request; // 🚀 호출한 쪽에서 Dispose() 필요
        }

    }
}

