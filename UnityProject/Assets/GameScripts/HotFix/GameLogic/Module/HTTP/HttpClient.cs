using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace GameLogic
{
    public class HttpClient
    {
        private readonly HttpSettings settings;

        public HttpClient(HttpSettings settings)
        {
            this.settings = settings;
        }

        public Req Req(string url)
        {
            return GameLogic.Req.Gen($"{settings.baseUrl}/{url}");
        }

        public async UniTask<Resp<T>> Get<T>(Req req, CancellationToken ct = default) where T : class
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(req.Uri()))
            {
                BuildupRequest(webRequest, req);

                await webRequest.SendWebRequest().WithCancellation(ct);

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    LogError($"GET request failed: {webRequest.error}");
                    return null;
                }

                string resp = webRequest.downloadHandler.text;
                return await DeserializeJsonAsync<Resp<T>>(resp, ct);
            }
        }

        public async UniTask<Resp<byte[]>> Get(Req req, CancellationToken ct = default)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(req.Uri()))
            {
                BuildupRequest(webRequest, req);

                await webRequest.SendWebRequest().WithCancellation(ct);

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    LogError($"GET request failed: {webRequest.error}");
                    return null;
                }

                Resp<byte[]> resp = new Resp<byte[]>
                {
                    data = webRequest.downloadHandler.data,
                    code = (int)webRequest.responseCode,
                    msg = webRequest.error
                };
                return resp;
            }
        }

        public async UniTask<Resp<T>> Post<T>(Req req, object body, CancellationToken ct = default) where T : class
        {
            string bodyStr = JsonConvert.SerializeObject(body);
            return await Post<T>(req, bodyStr, ct);
        }

        public async UniTask<Resp<T>> Post<T>(Req req, string body, CancellationToken ct = default) where T : class
        {
            return await Post<T>(req, body, settings.encoder, ct);
        }

        private async UniTask<Resp<T>> Post<T>(Req req, string body, IEncoder encoder = null, CancellationToken ct = default) where T : class
        {
            Log($"POST request body: {body}");

            byte[] fullBodyBytes = Encoding.UTF8.GetBytes(body);
            int offset = 0; // 假设偏移量为 0
            int length = fullBodyBytes.Length; // 假设长度为整个数组

            // 创建一个新的字节数组，只包含需要上传的部分
            byte[] bodyBytes = new byte[length];
            System.Array.Copy(fullBodyBytes, offset, bodyBytes, 0, length);

            using (UnityWebRequest webRequest = new UnityWebRequest(req.Uri(), UnityWebRequest.kHttpVerbPOST))
            {
                webRequest.uploadHandler = new UploadHandlerRaw(bodyBytes); // 使用部分字节数组
                webRequest.downloadHandler = new DownloadHandlerBuffer();

                BuildupRequest(webRequest, req);

                await webRequest.SendWebRequest().WithCancellation(ct);

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    LogError($"POST request failed: {webRequest.error}");
                    return null;
                }

                byte[] responseBytes = webRequest.downloadHandler.data;
                string responseStr = encoder?.Decode(responseBytes) ?? Encoding.UTF8.GetString(responseBytes);
                Log($"POST response: {responseStr}");

                return await DeserializeJsonAsync<Resp<T>>(responseStr, ct);
            }
        }


        private void BuildupRequest(UnityWebRequest request, Req req)
        {
            // Setup basic headers
            if (settings.headers != null)
            {
                foreach (var pair in settings.headers)
                {
                    request.SetRequestHeader(pair.Key, pair.Value);
                }
            }

            // Setup request headers
            if (req.headers != null)
            {
                foreach (var pair in req.headers)
                {
                    request.SetRequestHeader(pair.Key, pair.Value);
                }
            }
        }

        private async UniTask<T> DeserializeJsonAsync<T>(string json, CancellationToken ct) where T : class
        {
            return await UniTask.RunOnThreadPool(() => JsonConvert.DeserializeObject<T>(json), cancellationToken: ct);
        }

        private void Log(string log)
        {
            TEngine.Log.Info($"[HTTP] {log}");
        }

        private void LogError(string error)
        {
            TEngine.Log.Error($"[HTTP] {error}");
        }
    }
}
