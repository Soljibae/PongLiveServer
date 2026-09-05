using TMPro;
using UnityEngine;

public class RankingRowUI : MonoBehaviour
{
    private TMP_Text rankText;
    private TMP_Text nicknameText;
    private TMP_Text winsText;
    private TMP_Text lossesText;

    private void Awake()
    {
        SetInstance();
    }

    public void SetData(
        int rank,
        string nickname,
        int wins,
        int losses)
    {
        SetInstance();

        rankText.text = rank.ToString();
        nicknameText.text = nickname;
        winsText.text = wins.ToString();
        lossesText.text = losses.ToString();
    }

    public void Clear()
    {
        SetInstance();

        rankText.text = string.Empty;
        nicknameText.text = string.Empty;
        winsText.text = string.Empty;
        lossesText.text = string.Empty;
    }

    private void SetInstance()
    {
        if(rankText == null)
            rankText = transform.Find("RankText").GetComponent<TMP_Text>();
        if(nicknameText == null)
            nicknameText = transform.Find("NicknameText").GetComponent<TMP_Text>();
        if(winsText == null)
            winsText = transform.Find("WinText").GetComponent<TMP_Text>();
        if(lossesText == null)
            lossesText = transform.Find("LossText").GetComponent<TMP_Text>();
    }
}
