using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] int mainMenuSceneIndex;
    public bool AI { get; set; }
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneIndex);
    }
}
