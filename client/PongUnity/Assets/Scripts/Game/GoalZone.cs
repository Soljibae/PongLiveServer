using UnityEngine;

public class GoalZone : MonoBehaviour
{
    [SerializeField] bool isLeft;
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

        if(ball)
        {
            gameManager.AddScore(isLeft);
        }
    }
}
