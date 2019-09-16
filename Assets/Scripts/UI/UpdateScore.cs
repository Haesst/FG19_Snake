using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UpdateScore : MonoBehaviour
{
    Text scoreText = null;

    /// <summary>
    /// Make sure we have a text component on the gameobject.
    /// </summary>
    private void Awake()
    {
        scoreText = GetComponent<Text>();
        Assert.IsNotNull(scoreText, "Text component could not be found!");
        SetScoreText(0);
    }

    public void SetScoreText(int score)
    {
        // Update the score
        scoreText.text = score.ToString().PadLeft(4, '0');
    }
}