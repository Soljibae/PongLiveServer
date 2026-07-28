using UnityEngine;

public class LeaveUI : MonoBehaviour
{
    public void OnClickCancelButton()
    {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
    }

    public void OnClickLeaveButton()
    {
        NetworkGameManager.Instance.LeaveMatch();
    }
}
