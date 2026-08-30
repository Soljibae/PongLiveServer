using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Bootstrap Start");

        SceneManager.LoadScene("MainMenuScene");
    }
}
