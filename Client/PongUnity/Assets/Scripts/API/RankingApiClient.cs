using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class RankingApiClient : MonoBehaviour
{
    [SerializeField]
    private string baseUrl = "https://localhost:7257";

    public string BaseUrl => baseUrl;

    [Serializable]
    public class RankingUserResponse
    {
        public int rank;
        public string nickname;
        public int wins;
        public int losses;
        public int rankingScore;
    }

    [Serializable]
    public class RankingResponse
    {
        public bool success;
        public RankingUserResponse[] rankings;
        public RankingUserResponse myRanking;
        public string message;
    }

    public IEnumerator GetRanking(
       string token,
       Action<RankingResponse> onCompleted)
    {
        string url = $"{baseUrl}/api/ranking";

        using UnityWebRequest request =
            UnityWebRequest.Get(url);

        request.SetRequestHeader(
            "Authorization",
            $"Bearer {token}"
        );

        yield return request.SendWebRequest();

        if (request.result ==
            UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(
                $"Ranking request failed: {request.error}"
            );

            onCompleted?.Invoke(null);
            yield break;
        }

        string json = request.downloadHandler.text;

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError(
                $"Ranking response is empty. " +
                $"StatusCode: {request.responseCode}"
            );

            onCompleted?.Invoke(null);
            yield break;
        }

        RankingResponse response =
            JsonUtility.FromJson<RankingResponse>(json);

        onCompleted?.Invoke(response);
    }
}
