using Unity.Netcode;
using UnityEngine;

public class NetworkGoalZone : NetworkBehaviour
{
    [SerializeField] private PlayerSide PlayerSide;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!IsServer) 
            return;

        NetworkBall ball = collision.gameObject.GetComponentInParent<NetworkBall>();

        if (!ball)
            return;

        switch(PlayerSide)
        {
            case PlayerSide.Left:
                NetworkGameManager.Instance.AddScoreServer(false);
                break;
            case PlayerSide.Right:
                NetworkGameManager.Instance.AddScoreServer(true);
                break;
        }
    }
}
