using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private int playSceneIndex;
    public void PlayGame(bool ai)
    {
        GameManager.Instance.AI = ai;
        SceneManager.LoadScene(playSceneIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Quit Application");
#else
        Application.Quit();
#endif
    }
}
