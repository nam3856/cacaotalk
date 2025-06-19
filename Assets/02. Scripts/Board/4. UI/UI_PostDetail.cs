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
    public TextMeshProUGUI ContentText;
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
    private CommentManager _commentManager;

    private void Awake()
    {
        SubmitButton.onClick.AddListener(OnClickSubmit);
        Initialize();
    }

    private void Initialize()
    {
        _commentManager = new CommentManager();
        _commentManager.OnCommentsLoaded += OnCommentsLoaded;
        _commentManager.OnCommentAdded += OnCommentAdded;
        _commentManager.OnError += error => Debug.LogError(error);
    }
    public async Task ShowPostAsync(PostDTO post)
    {
        _currentPost = post;

        NicknameText.text = post.Nickname;
        ContentText.text = post.Content;
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
        {
            if (child.GetComponent<CommentSetter>() == null) continue;
            Destroy(child.gameObject);
        }

        await _commentManager.LoadCommentsAsync(_currentPost.Id.Value);
    }

    private void OnCommentsLoaded(List<Comment> comments)
    {
        foreach (var comment in comments)
        {
            var obj = Instantiate(CommentPrefab, CommentContainer);
            var content = obj.GetComponentInChildren<CommentSetter>();

            var sprite = (comment.ImageIndex >= 0 && comment.ImageIndex < ProfileSprites.Length)
                ? ProfileSprites[comment.ImageIndex]
                : ProfileSprites[0];

            content.SetComment(comment.AuthorNickname, comment.Content, sprite, FormatTime(comment.CreatedAt.ToDateTime()));
        }
    }

    private async void OnClickSubmit()
    {
        string content = CommentInputField.text.Trim();
        if (string.IsNullOrWhiteSpace(content)) return;

        string authorId = AccountManager.Instance.GetMyEmail();   // 유저 이메일
        string nickname = AccountManager.Instance.GetMyNickname();
        int imageIndex = AccountManager.Instance.CurrentAccount?.ImageIndex ?? 0;

        await _commentManager.AddCommentAsync(_currentPost.Id.Value, authorId, nickname, content, imageIndex);
        CommentInputField.text = "";
    }
    private void OnCommentAdded(Comment comment)
    {
        LoadCommentsAsync();
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
