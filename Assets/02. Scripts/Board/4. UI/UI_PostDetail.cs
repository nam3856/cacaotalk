using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Firestore;

public class UI_PostDetail : MonoBehaviour
{
    [Header("게시글 UI")]
    public TextMeshProUGUI NicknameText;
    public Image ProfileImage;
    public TextMeshProUGUI TimeText;
    public ContentSetter ContentText;
    public TextMeshProUGUI LikeCountText;

    [Header("댓글 UI")]
    public Transform CommentContainer;
    public GameObject CommentPrefab;
    public TMP_InputField CommentInputField;
    public Button SubmitButton;

    [Header("프로필 이미지 목록")]
    public Sprite[] ProfileSprites; // 0~4 인덱스

    private PostDTO _currentPost;
    private CommentRepository _commentRepository;

    private void Awake()
    {
        SubmitButton.onClick.AddListener(OnClickSubmit);
    }

    private void Initialize()
    {
        _commentRepository = new CommentRepository();
    }
    public async Task ShowPostAsync(PostDTO post)
    {
        _currentPost = post;

        NicknameText.text = post.Nickname;
        ContentText.SetText(post.Content);
        LikeCountText.text = post.LikeCount.ToString();
        TimeText.text = FormatTime(post.CreatedAt.ToDateTime());

        if (post.ImageIndex >= 0 && post.ImageIndex < ProfileSprites.Length)
            ProfileImage.sprite = ProfileSprites[post.ImageIndex];

        await LoadCommentsAsync();

        BoardManager.Instance.OnPostUpdated -= HandlePostUpdated;
        BoardManager.Instance.OnPostUpdated += HandlePostUpdated;
    }

    private void HandlePostUpdated(PostDTO updatedPost)
    {
        if (_currentPost == null) return;
        if (updatedPost.Id != _currentPost.Id) return;

        Debug.Log("현재 보고 있는 게시글이 업데이트됨");

        _currentPost = updatedPost;
        LikeCountText.text = updatedPost.LikeCount.ToString();
    }

    private async Task LoadCommentsAsync()
    {
        foreach (Transform child in CommentContainer)
            Destroy(child.gameObject);

        var comments = await _commentRepository.GetCommentsAsync(_currentPost.Id.Value);
        foreach (var comment in comments)
        {
            var obj = Instantiate(CommentPrefab, CommentContainer);
            var content = obj.GetComponent<ContentSetter>();
            content.SetText($"{comment.AuthorNickname}: {comment.Content}");
        }
    }

    private async void OnClickSubmit()
    {
        string content = CommentInputField.text.Trim();
        if (string.IsNullOrWhiteSpace(content)) return;

        string authorId = "test-user-001";
        string nickname = "경민남";

        var comment = new Comment(_currentPost.Id.Value, authorId, nickname, content);
        await _commentRepository.AddCommentAsync(comment);

        CommentInputField.text = "";
        await LoadCommentsAsync();
    }

    private string FormatTime(System.DateTime dt)
    {
        return dt.ToString("yyyy.MM.dd HH:mm");
    }


    private void OnGUI()
    {
        if(GUI.Button(new Rect(10, 10, 100, 30), "Init"))
        {
            Initialize();
            Debug.Log("CommentRepository 초기화 완료");
        }
        if(GUI.Button(new Rect(10, 50, 100, 30), "Load Comments"))
        {
            if (_currentPost != null)
            {

                LoadCommentsAsync().ContinueWith(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        Debug.Log("댓글 로드 완료");
                    }
                    else
                    {
                        Debug.LogError("댓글 로드 실패: " + task.Exception);
                    }
                });
            }
            else
            {
                Debug.LogWarning("현재 게시글이 설정되지 않았습니다.");
            }
        }
        if (GUI.Button(new Rect(10, 90, 100, 30), "Show Post"))
        {
            ShowPostTest();
        }
    }

    private async Task ShowPostTest()
    {
        PostDTO testPost = new PostDTO
        {
            Id = new PostId("testpost"),
            Email = "test@test.com",
            Nickname = "테스트!!",
            ImageIndex = 0,
            CreatedAt = Timestamp.GetCurrentTimestamp(),
            Content = "안녕하세요 테스트입니다안녕하세요 테스트입니다안녕하세요 테스트입니다안녕하세요 테스트입니다안녕하세요 테스트입니다안녕하세요 테스트입니다안녕하세요 테스트입니다안녕하세요 테스트입니다안녕하세요 테스트입니다안녕하세요 테스트입니다안녕하세요 테스트입니다안녕하세요 테스트입니다안녕하세요 테스트입니다안녕하세요 테스트입니다안녕하세요 테스트입니다안녕하세요 테스트입니다안녕하세요 테스트입니다안녕하세요 테스트입니다안녕하세요 테스트입니다안녕하세요 테스트입니다안녕하세요 테스트입니다안녕하세요 테스트입니다안녕하세요 테스트입니다안녕하세요 테스트입니다안녕하세요 테스트입니다안녕하세요 테스트입니다안녕하세요 테스트입니다",
            CommentCount = 1,
            LikeCount = 99
        };

        await ShowPostAsync(testPost);
        Debug.Log("테스트 게시글 표시 완료");
    }
}
