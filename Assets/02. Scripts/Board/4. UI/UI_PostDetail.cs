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
    private CommentManager _commentManager;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _commentManager = new CommentManager();
        _commentManager.OnCommentsLoaded += OnCommentsLoaded;
        _commentManager.OnCommentAdded += OnCommentAdded;
        _commentManager.OnError += error => Debug.LogError(error);
        SubmitButton.onClick.AddListener(OnClickSubmit);
        _currentPost = BoardManager.Instance.GetSelectedPost();
        if (_currentPost != null)
        {
            ShowPostAsync(_currentPost).ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Debug.Log("게시글 상세 정보 표시 완료");
                }
                else
                {
                    Debug.LogError("게시글 상세 정보 표시 실패: " + task.Exception);
                }
            });
        }
        else
        {
            Debug.LogWarning("현재 선택된 게시글이 없습니다.");
        }


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


}
