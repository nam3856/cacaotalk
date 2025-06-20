using Firebase.Firestore;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_PostEdit : MonoBehaviour
{
    [Header("UI Reference")]
    public TMP_InputField contentInput;
    public Button cancelButton;
    public Button submitButton;

    private PostDTO editingPost;

    private void Start()
    {
        cancelButton.onClick.AddListener(OnCancel);
        submitButton.onClick.AddListener(OnSubmit);

        // 선택된 게시글 ID를 통해 데이터 로드
        editingPost = BoardManager.Instance.GetSelectedPost();
        if (editingPost == null)
        {
            Debug.LogError("수정할 게시글을 찾을 수 없습니다.");
            SceneManager.LoadScene("Post"); // fallback
            return;
        }

        contentInput.text = editingPost.Content;
    }

    private void OnCancel()
    {
        SceneManager.LoadScene("Post Detail (Real)");
    }

    private async void OnSubmit()
    {
        string newContent = contentInput.text.Trim();

        var contentSpec = new PostContentSpecificaion();
        if (!contentSpec.IsSatisfiedBy(newContent))
        {
            Debug.LogWarning(contentSpec.ErrorMessage);
            return;
        }

        editingPost.Content = newContent;
        editingPost.CreatedAt = Timestamp.GetCurrentTimestamp(); // 시간도 갱신할 경우

        await BoardManager.Instance.UpdatePost(editingPost);
        SceneManager.LoadScene("Post Detail (Real)");
    }
}
