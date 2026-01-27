using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string firstLevelSceneName = "SampleScene"; // change to your game scene name

    public void StartGameRegular()
    {
        GameState.nextDir = TrolleyDirection.Forward;
        SceneManager.LoadScene(firstLevelSceneName);

    }

    public void StartGameRandom()
    {
        GameState.nextDir = TrolleyDirection.Random;
        SceneManager.LoadScene(firstLevelSceneName);

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
