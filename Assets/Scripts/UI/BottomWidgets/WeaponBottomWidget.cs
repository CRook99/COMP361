using TMPro;
using UnityEngine;

namespace UI.BottomWidgets
{
    public class WeaponBottomWidget : MonoBehaviour
    {
        [TextArea(3, 5)]
        [SerializeField] private string bodyText;
        [SerializeField] private TextMeshProUGUI textElement;
    }
}