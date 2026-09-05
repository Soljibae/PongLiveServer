using UnityEngine;
using static RankingApiClient;

public class RankingUI : MonoBehaviour
{
    private RankingApiClient rankingApiClient;
    private AuthManager authManager;
    [SerializeField] private PopUpUI popUpUI;
    [SerializeField] private GameObject rankingPanel;
    [SerializeField] private Transform rankingList;
    [SerializeField] private RankingRowUI myRankingRow;

    private RankingRowUI[] rankingRows;

    private void Awake()
    {
        rankingApiClient = NetworkManagerInstance.Instance.RankingApiClient;

        authManager = AuthManager.Instance;

        rankingRows = rankingList.GetComponentsInChildren<RankingRowUI>(true);
    }

    public void OnRankingButtonClicked()
    {
        if(!authManager.IsLoggedIn)
        {
            popUpUI.gameObject.SetActive(true);
            popUpUI.popUpText.text = "Please log in to view rankings.";
            return;
        }

        string token = AuthManager.Instance.Token;

        if (string.IsNullOrEmpty(token))
        {
            popUpUI.gameObject.SetActive(true);
            popUpUI.popUpText.text = "JWT token is missing.";
            return;
        }
            

        StartCoroutine(
            rankingApiClient.GetRanking(
                token,
                OnRankingReceived
            )
        );
    }

    private void OnRankingReceived(RankingResponse response)
    {
        if (response == null)
        {
            popUpUI.gameObject.SetActive(true);
            popUpUI.popUpText.text = "Failed to retrieve ranking data.";
            Debug.LogError("Failed to retrieve ranking data.");
            return;
        }

        if (!response.success)
        {
            popUpUI.gameObject.SetActive(true);
            popUpUI.popUpText.text = $"Ranking request failed: {response.message}";
            Debug.LogError(
                $"Ranking request failed: {response.message}"
            );
            return;
        }

        for (int i = 0; i < rankingRows.Length; i++)
        {
            if (i < response.rankings.Length)
            {
                RankingUserResponse user =
                    response.rankings[i];

                rankingRows[i].SetData(
                    user.rank,
                    user.nickname,
                    user.wins,
                    user.losses
                );
            }
            else
            {
                rankingRows[i].Clear();
            }
        }

        RankingUserResponse me = response.myRanking;

        myRankingRow.SetData(
            me.rank,
            me.nickname,
            me.wins,
            me.losses
        );

        rankingPanel.SetActive(true);
    }

    public void OnRankingOffButtonClicked()
    {
        rankingPanel.SetActive(false);
    }
}
