using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AccountApiClient : MonoBehaviour
{
    [SerializeField]
    private string apiBaseUrl = "https://localhost:7257";

    public string ApiBaseUrl => apiBaseUrl;

    [Serializable]
    public class AvailabilityResponse
    {
        public bool available;
        public string message;
    }

    [Serializable]
    public class SignUpRequest
    {
        public string id;
        public string password;
        public string nickname;
    }

    [Serializable]
    public class SignUpResponse
    {
        public bool success;
        public string message;
    }

    public IEnumerator CheckId(
        string id,
        Action<bool, string> onCompleted,
        Action<string> onError)
    {
        string encodedId = UnityWebRequest.EscapeURL(id);

        string url =
            $"{ApiBaseUrl}/api/auth/check-id?id={encodedId}";

        using UnityWebRequest request =
            UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(request.error);
            yield break;
        }

        AvailabilityResponse response =
            JsonUtility.FromJson<AvailabilityResponse>(
                request.downloadHandler.text
            );

        onCompleted?.Invoke(
            response.available,
            response.message
        );
    }

    public IEnumerator CheckNickname(
        string nickname,
        Action<bool, string> onCompleted,
        Action<string> onError)
    {
        string encodedNickname =
            UnityWebRequest.EscapeURL(nickname);

        string url =
            $"{ApiBaseUrl}/api/auth/check-nickname?nickname={encodedNickname}";

        using UnityWebRequest request =
            UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(request.error);
            yield break;
        }

        AvailabilityResponse response =
            JsonUtility.FromJson<AvailabilityResponse>(
                request.downloadHandler.text
            );

        onCompleted?.Invoke(
            response.available,
            response.message
        );
    }

    public IEnumerator SignUp(
    string id,
    string password,
    string nickname,
    Action<bool, string> onCompleted,
    Action<string> onError)
    {
        string url = $"{apiBaseUrl}/api/auth/signup";

        SignUpRequest requestData = new SignUpRequest
        {
            id = id,
            password = password,
            nickname = nickname
        };

        string json = JsonUtility.ToJson(requestData);

        using UnityWebRequest request =
            new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);

        byte[] bodyRaw =
            System.Text.Encoding.UTF8.GetBytes(json);

        request.uploadHandler =
            new UploadHandlerRaw(bodyRaw);

        request.downloadHandler =
            new DownloadHandlerBuffer();

        request.SetRequestHeader(
            "Content-Type",
            "application/json"
        );

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(request.error);
            yield break;
        }

        SignUpResponse response =
            JsonUtility.FromJson<SignUpResponse>(
                request.downloadHandler.text
            );

        onCompleted?.Invoke(
            response.success,
            response.message
        );
    }
}
