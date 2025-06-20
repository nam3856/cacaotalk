using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CommentSetter : MonoBehaviour
{
    [SerializeField] private Image _profileImage;
    [SerializeField] private TextMeshProUGUI _nicknameText;
    [SerializeField] private TextMeshProUGUI _commentText;
    [SerializeField] private TextMeshProUGUI _timeText;

    public void SetComment(string nickname, string comment, Sprite profileImage, string time)
    {
        _nicknameText.text = nickname;
        _commentText.text = comment;
        _profileImage.sprite = profileImage;
        _timeText.text = time;
        AdjustHeight();
    }

    public void AdjustHeight()
    {
        if (_commentText != null)
        {
            RectTransform thisRect = GetComponent<RectTransform>();
            float finalHeight = _commentText.preferredHeight;
            finalHeight = finalHeight + 300;

            thisRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, finalHeight);
        }
    }
}
