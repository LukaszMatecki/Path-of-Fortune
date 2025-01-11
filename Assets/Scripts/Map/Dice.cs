using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dice : MonoBehaviour
{
    public Sprite[] diceSides;
    private Image img;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Button rollButton;
    [SerializeField] private Button moveButton;
    private TileSelector tileSelector;

    public bool hasRolled = false;


    private void Start()
    {
        img = GetComponent<Image>();

        if (img == null)
        {
            Debug.LogError("No Image component attached to the Dice game object.");
            return;
        }
        if (diceSides == null || diceSides.Length == 0)
        {
            Debug.LogError("Dice sides not assigned!");
        }
        if (rollButton == null || resultText == null)
        {
            Debug.LogError("Button or Text not assigned!");
            return;
        }

        rollButton.onClick.AddListener(OnRollButtonClicked);

        if (moveButton != null)
        {
            moveButton.onClick.AddListener(OnMoveButtonClicked); 
        }
        else
        {
            Debug.LogError("Move button not assigned!");
        }
    }

    private void OnRollButtonClicked()
    {
        
        if (hasRolled == false)
        {
            ResetAfterMove();
            StartCoroutine(RollTheDice());
        }
        else
        {
            Debug.Log("w tej turze wykonano ju¿ ruch");
        }

    }

    private IEnumerator RollTheDice()
    {
        hasRolled = true;
        rollButton.interactable = false;
        int randomDiceSide = 0;
        for (int i = 0; i <= 20; i++)
        {
            randomDiceSide = Random.Range(0, diceSides.Length);
            img.sprite = diceSides[randomDiceSide];
            yield return new WaitForSeconds(0.08f);
        }

        int finalSide = randomDiceSide + 1;
        resultText.text = "You rolled: " + finalSide;

        tileSelector = Object.FindAnyObjectByType<TileSelector>();
        if (tileSelector != null)
        {
            tileSelector.SetMaxSteps(finalSide);
            tileSelector.HighlightPotentialMoves();
        }
       

        rollButton.interactable = true;
    }

    private void OnMoveButtonClicked()
    {
        tileSelector.OnReadyButtonClicked();
        //Debug.Log("Move button clicked. Executing movement.");
        DisableTileSelector();
    }

    private void DisableTileSelector()
    {
        if (tileSelector != null)
        {
            tileSelector.isActive = false;
            //Debug.Log("TileSelector disabled after move.");
        }
    }

    public void ResetAfterMove()
    {
        hasRolled = false;
        rollButton.interactable = true;
        if (tileSelector != null)
        {
            tileSelector.isActive = true;
            //Debug.Log("TileSelector re-enabled after move.");
        }
    }
}