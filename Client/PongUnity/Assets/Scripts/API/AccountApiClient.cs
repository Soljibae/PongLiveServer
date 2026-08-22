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
}
