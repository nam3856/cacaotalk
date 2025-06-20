using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Post : MonoBehaviour
{
    [Header("UI_References")]
    public Image ProfileImage;
    public Sprite[] ProfileSprites; // 0~4 인덱스
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
        ContentPreviewText.text = TrimContent(post.Content, 20);
        CommentLikeText.text = $"댓글 {post.CommentCount}  ·  좋아요 {post.LikeCount}";

        //프로필 이미지 처리 (예: Index → Sprite 매핑)
        if (post.ImageIndex >= 0 && post.ImageIndex < ProfileSprites.Length)
            ProfileImage.sprite = ProfileSprites[post.ImageIndex];
    }
    public void OnClick()
    {
        BoardManager.Instance.SetSelectedPostId(post.Id);
        SceneManager.LoadScene("Post Detail (Real)"); // 상세 보기 씬 이름
    }
    private string TrimContent(string content, int maxLineLength)
    {
        if (string.IsNullOrWhiteSpace(content)) return "";

        // 줄바꿈으로 분리
        string[] lines = content.Split('\n');
        string firstLine = lines[0];

        // 첫 줄 자르기
        string trimmed = firstLine.Length > maxLineLength
            ? firstLine.Substring(0, maxLineLength) + "..."
            : firstLine;

        // 만약 두 번째 줄 이상이 존재한다면 무조건 ... 붙이기 (이미 ... 있으면 중복 방지)
        if (lines.Length > 1 && !trimmed.EndsWith("..."))
        {
            trimmed += "...";
        }

        return trimmed;
    }

}
