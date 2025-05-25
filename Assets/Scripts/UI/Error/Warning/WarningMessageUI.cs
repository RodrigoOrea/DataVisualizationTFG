using UnityEngine;
using TMPro;

public class WarningMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;

    public void SetMessage(string message)
    {
        messageText.text = message;
    }

    public void CloseMessage()
    {
        Destroy(gameObject);
    }
}
