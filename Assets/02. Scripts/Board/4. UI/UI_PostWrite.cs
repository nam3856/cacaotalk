using Firebase.Auth;
using Firebase.Firestore;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_PostWrite : MonoBehaviour
{
    [Header("UI Reference")]
    public TMP_InputField contentInput;
    public Button cancelButton;
    public Button submitButton;

    private void Start()
    {
        cancelButton.onClick.AddListener(OnCancel);
        submitButton.onClick.AddListener(OnSubmit);
    }

    private void OnCancel()
    {
        SceneManager.LoadScene("Post"); // 또는 이전 씬
    }

    private async void OnSubmit()
    {
        string content = contentInput.text.Trim();

        if (string.IsNullOrEmpty(content))
        {
            Debug.LogWarning("내용을 입력해주세요.");
            return;
        }

        if (content.Length > Post.MaxContentLength)
        {
            Debug.LogWarning($"내용은 {Post.MaxContentLength}자를 넘을 수 없습니다.");
            return;
        }

        var user = AccountManager.Instance.CurrentAccount;
        if (user == null)
        {
            Debug.LogError("로그인된 사용자가 없습니다.");
            return;
        }

        PostDTO post = new PostDTO
        {
            Email = user.Email,
            Nickname = user.Nickname ?? "익명",
            ImageIndex = user.ImageIndex,
            CreatedAt = Timestamp.GetCurrentTimestamp(),
            Content = content,
            CommentCount = 0,
            LikeCount = 0
        };

        await BoardManager.Instance.AddPost(post);
        SceneManager.LoadScene("Post"); // 글 작성 후 목록으로 이동
    }
}
