using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiceRoll : MonoBehaviour
{
    [SerializeField] private TMP_Text resultText;
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
