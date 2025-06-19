using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ContentSetter : MonoBehaviour
{
    [Header("자동 조정 대상 텍스트")]
    public TextMeshProUGUI targetText;

    private void OnEnable()
    {
        AdjustHeight();
    }

    public void SetText(string val)
    {
        if (targetText != null)
        {
            targetText.text = val;
            AdjustHeight();
        }
    }

    public void AdjustHeight()
    {
        if (targetText != null)
        {
            RectTransform thisRect = GetComponent<RectTransform>();
            float finalHeight = targetText.preferredHeight;
            finalHeight = finalHeight + 40;

            thisRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, finalHeight);
        }
    }

}
