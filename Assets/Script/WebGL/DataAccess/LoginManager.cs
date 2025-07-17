using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    [System.Serializable]
    public class LoginData
    {
        public string accountId;
        public string password;
    }

    [System.Serializable]
    public class TokenResponseWrapper
    {
        public int statusCode;
        public TokenResponse data;
        public string message;
    }

    [System.Serializable]
    public class TokenResponse
    {
        public string accessToken;
        public string refreshToken;
    }

    [System.Serializable]
    public class TokenRefreshWrapper
    {
        public int statusCode;
        public TokenRefreshData data;
        public string message;
    }

    [System.Serializable]
    public class TokenRefreshData
    {
        public string accessToken;
    }

    public string accountId = "superAdmin";
    public string password = "Qwer1234!!";

    void Start()
    {
        StartCoroutine(LoginRequest(accountId, password));
    }

    IEnumerator LoginRequest(string accountId, string password)
    {
        string url = "https://rnn.korea.ac.kr/spring/api/auth/login";
        LoginData payload = new LoginData { accountId = accountId, password = password };
        string json = JsonUtility.ToJson(payload);

        UnityWebRequest req = new UnityWebRequest(url, "POST");
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(body);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            TokenResponseWrapper wrapper = JsonUtility.FromJson<TokenResponseWrapper>(req.downloadHandler.text);
            PlayerPrefs.SetString("AccessToken", wrapper.data.accessToken);
            PlayerPrefs.SetString("RefreshToken", wrapper.data.refreshToken);
            Debug.Log("✅ 로그인 성공! 토큰 저장됨");
            Debug.Log("🔐 AccessToken: " + wrapper.data.accessToken);
            Debug.Log("🔁 RefreshToken: " + wrapper.data.refreshToken);

            // 실제 API 요청 예시
            StartCoroutine(RequestProtectedData());
        }
        else
        {
            Debug.LogError("❌ 로그인 실패: " + req.error);
            Debug.LogError("서버 응답: " + req.downloadHandler.text);
        }
    }

    IEnumerator RequestProtectedData()
    {
        string url = "https://rnn.korea.ac.kr/express/locations/recent";
        string token = PlayerPrefs.GetString("AccessToken");

        UnityWebRequest req = UnityWebRequest.Get(url);
        req.SetRequestHeader("Authorization", "Bearer " + token);
        yield return req.SendWebRequest();

        if (req.responseCode == 401)
        {
            Debug.Log("⚠️ accessToken 만료됨 → refresh 시도");
            yield return RefreshAccessToken(success =>
            {
                if (success)
                {
                    Debug.Log("🔁 accessToken 재발급 성공 → 재요청");
                    StartCoroutine(RequestProtectedData());
                }
                else
                {
                    Debug.LogError("❌ refreshToken 만료 → 재로그인 필요");
                }
            });
            yield break;
        }

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("📦 데이터 수신: " + req.downloadHandler.text);
        }
        else
        {
            Debug.LogError("❌ 데이터 요청 실패: " + req.downloadHandler.text);
        }
    }

    public static IEnumerator RefreshAccessToken(System.Action<bool> onComplete)
    {
        string url = "https://rnn.korea.ac.kr/spring/api/auth/refresh";
        string refreshToken = PlayerPrefs.GetString("RefreshToken");

        UnityWebRequest req = UnityWebRequest.PostWwwForm(url, "");
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Authorization", "Bearer " + refreshToken);

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            TokenRefreshWrapper wrapper = JsonUtility.FromJson<TokenRefreshWrapper>(req.downloadHandler.text);
            PlayerPrefs.SetString("AccessToken", wrapper.data.accessToken);
            Debug.Log("✅ accessToken 재발급 완료");
            Debug.Log("🔐 새 accessToken: " + wrapper.data.accessToken);
            onComplete?.Invoke(true);
        }
        else
        {
            Debug.LogError("❌ accessToken 재발급 실패: " + req.downloadHandler.text);
            onComplete?.Invoke(false);
        }
    }
}
