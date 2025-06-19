using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Post : MonoBehaviour
{
    [Header("UI_References")]
    public Image ProfileImage;
    public TextMeshProUGUI NicknameText;
    public TextMeshProUGUI DateText;
    public TextMeshProUGUI ContentPreviewText;
    public TextMeshProUGUI CommentLikeText;

    private PostDTO post;

    public void Setup(PostDTO postData)
    {
        post = postData;

        string date = post.CreatedAt.ToDateTime().ToString("yyyy.MM.dd");

        NicknameText.text = post.Nickname;
        DateText.text = date;
        ContentPreviewText.text = TrimContent(post.Content, 50);
        CommentLikeText.text = $"댓글 {post.CommentCount}  ·  좋아요 {post.LikeCount}";

        //프로필 이미지 처리 (예: Index → Sprite 매핑)
        //ProfileImage.sprite = ProfileImageManager.Instance.GetProfileSprite(post.ImageIndex);
    }
    public void OnClick()
    {
        BoardManager.Instance.SetSelectedPostId(post.Id);
        SceneManager.LoadScene("Post Detail (Real)"); // 상세 보기 씬 이름
    }
    private string TrimContent(string content, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(content)) return "";
        return content.Length > maxLength ? content.Substring(0, maxLength) + "..." : content;
    }
}
