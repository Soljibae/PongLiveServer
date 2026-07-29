using UnityEngine;

public class GoalZone : MonoBehaviour
{
    [SerializeField] private PlayerSide localPlayerSide;
    [SerializeField] private GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Ball ball = collision.gameObject.GetComponentInParent<Ball>();

        if (!ball)
            return;

        switch(localPlayerSide)
        {
            case PlayerSide.Left:
                gameManager.AddScore(false);
                break;
            case PlayerSide.Right:
                gameManager.AddScore(true);
                break;
        }
    }
}
