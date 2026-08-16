using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class BackendConnectionTest : MonoBehaviour
{
    [SerializeField]
    private string testUrl;

    [System.Serializable]
    private class TestMessageRequest
    {
        public string message;
    }

    public void StratTest()
    {
        StartCoroutine(SendPostTestRequest());
    }

    private IEnumerator GetTestConnection()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(testUrl))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Connection test failed: {request.error}");
            }
            else
            {
                Debug.Log("Connection test succeeded!");
            }
        }
    }

    private IEnumerator SendPostTestRequest()
    {
        TestMessageRequest requestData = new TestMessageRequest
        {
            message = "Hello Backend"
        };

        string json = JsonUtility.ToJson(requestData);

        using UnityWebRequest request = new UnityWebRequest(testUrl, UnityWebRequest.kHttpVerbPOST);

        byte[] body = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Backend request failed: {request.error}");
            yield break;
        }

        Debug.Log($"Backend response: {request.downloadHandler.text}");
    }
}
