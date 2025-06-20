using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("옵션 UI")]
    public Button DeleteButton;
    public Button EditButton;

    [Header("프로필 이미지 목록")]
    public Sprite[] ProfileSprites; // 0~4 인덱스

    [Header("좋아요 UI")]
    public Toggle LikeToggle;
    public Image CheckmarkImage;

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
        LikeToggle.onValueChanged.AddListener(OnLikeToggleChanged);
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
        await SetupLikeToggleAsync(post);

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

    private async Task SetupLikeToggleAsync(PostDTO post)
    {
        // 이벤트 제거
        LikeToggle.onValueChanged.RemoveListener(OnLikeToggleChanged);

        // 현재 사용자의 좋아요 여부 확인
        bool isLiked = await LikeManager.Instance.IsLiked(post.Id);

        // 토글 상태 설정
        LikeToggle.isOn = isLiked;

        // 이벤트 다시 등록
        LikeToggle.onValueChanged.AddListener(OnLikeToggleChanged);
    }

    //private async void OnLikeToggleChanged(bool isOn)
    //{
    //    // 유저가 눌렀을 때만 ToggleLike 호출
    //    bool result = await LikeManager.Instance.ToggleLike(_currentPost.Id);
    //    _currentPost = BoardManager.Instance.GetPostById(_currentPost.Id);
    //    LikeCountText.text = _currentPost.LikeCount.ToString();
    //}

    private async void OnLikeToggleChanged(bool isOn)
    {
        if (_currentPost == null) return;

        bool nowLiked = await LikeManager.Instance.ToggleLike(_currentPost.Id);

        _currentPost.LikeCount += nowLiked ? 1 : -1;
        LikeCountText.text = _currentPost.LikeCount.ToString();

        BoardManager.Instance.UpdateLocalPost(_currentPost);
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

    public void OnClickBack()
    {
        //BoardManager.Instance.SetSelectedPostId(null); // 선택된 게시글 ID 초기화
        UnityEngine.SceneManagement.SceneManager.LoadScene("Post"); // 게시판 씬으로 돌아가기
    }

    public async void OnClickDelete()
    {
        if (_currentPost == null) return;

        if (!AccountManager.Instance.IsMyPost(_currentPost.Email))
        {
            Debug.LogWarning("본인 게시글만 삭제할 수 있습니다.");
            return;
        }

        bool confirm = true;

        if (confirm)
        {
            await BoardManager.Instance.DeletePost(_currentPost.Id);
            SceneManager.LoadScene("Post"); // 삭제 후 목록으로
        }
    }

    public async void OnClickEdit()
    {
        if (_currentPost == null)
        {
            Debug.LogWarning("게시글 정보가 없습니다.");
            return;
        }

        if (!AccountManager.Instance.IsMyPost(_currentPost.Email))
        {
            Debug.LogWarning("본인 게시글만 수정할 수 있습니다.");
            return;
        }

        // 현재 게시글 ID를 BoardManager에 저장
        BoardManager.Instance.SetSelectedPostId(_currentPost.Id);

        // 선택된 게시글을 최신화해서 편집 시 최신 데이터 사용
        await BoardManager.Instance.RefreshPost(_currentPost.Id);

        // 수정 씬으로 전환
        SceneManager.LoadScene("Post Edit");
    }


}
