using UnityEngine;
using UnityEngine.UI;

public class MultiGraphicButton : MonoBehaviour
{
    public Button button;
    public Text text;
    public Image image;

    private void Start()
    {
        if (button != null)
        {
            var buttonColors = button.colors;

            if (text != null)
                text.color = buttonColors.normalColor;

            if (image != null)
                image.color = buttonColors.normalColor;

            button.onClick.AddListener(() =>
            {
                if (text != null)
                    text.color = buttonColors.pressedColor;

                if (image != null)
                    image.color = buttonColors.pressedColor;
            });
        }
    }
}