using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UpdateScore : MonoBehaviour
{
    Text scoreText = null;

    private void Awake()
    {
        scoreText = GetComponent<Text>();
        Assert.IsNotNull(scoreText, "Text component could not be found.");
    }

    public void SetScoreText(int score)
    {
        scoreText.text = string.Format(score.ToString().PadLeft(4, '0'));
    }
}