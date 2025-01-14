using UnityEngine;

public class HasPlayerLost : MonoBehaviour
{
    public void turnFalse()
    {
        if (PlayerInfo.Instance != null)
            PlayerInfo.Instance.hasPlayerLost = false;
    }
}
