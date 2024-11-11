using UnityEngine;
using UnityEngine.UI;

public class DiceRoll : MonoBehaviour
{
    public Text resultText;
    private TileSelector tileSelector;

    private void Start()
    {
        tileSelector = Object.FindAnyObjectByType<TileSelector>();
    }

    public void RollDice()
    {
        int diceRoll = Random.Range(1, 7);
        resultText.text = "You rolled: " + diceRoll;

        if (tileSelector != null)
        {
            tileSelector.SetMaxSteps(diceRoll);
            tileSelector.HighlightPotentialMoves();
        }
    }
}
