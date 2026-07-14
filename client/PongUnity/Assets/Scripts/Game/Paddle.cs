using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField] private float width;
    [SerializeField] private float height;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;

    public int CurrentSpeed { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
