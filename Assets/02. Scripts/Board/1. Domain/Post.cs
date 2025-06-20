// 글쓰기 기능
// 정:
// 부:
// Post에서는 프로필 이미지 / 닉네임 / 타임 스탬프 / 글 내용(텍스트) / 코멘트 수 / 좋아요 수


using Firebase.Firestore;
using System;

public class Post
{
    [FirestoreProperty]
    public PostId Id { get; set; }

    [FirestoreProperty]
    public string Email { get; private set; }

    [FirestoreProperty]
    public string Nickname { get; private set; }

    [FirestoreProperty]
    public int ImageIndex { get; private set; } // 프로필 이미지 인덱스

    [FirestoreProperty]
    public Timestamp CreatedAt { get; private set; }

    [FirestoreProperty]
    public string Content { get; private set; }

    [FirestoreProperty]
    public int CommentCount { get; set; }

    [FirestoreProperty]
    public int LikeCount { get; set; }

    public Post() { } // Firestore 역직렬화용 기본 생성자

    public const int MaxContentLength = 1000;

    public Post(PostId id, string email, string nickname, int imageIndex, Timestamp createdAt, string content, int commentcount, int likecount)
    {
        // 아이디 검증
        if (string.IsNullOrWhiteSpace(id.Value))
            throw new ArgumentException("게시글 ID는 비어 있을 수 없습니다.", nameof(id));

        // 이메일 검증
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            throw new ArgumentException("유효하지 않은 이메일입니다.", nameof(email));

        // 닉네임 검증
        if (string.IsNullOrWhiteSpace(nickname))
            throw new ArgumentException("닉네임은 비어 있을 수 없습니다.", nameof(nickname));

        // 프로필 이미지 인덱스 검증
        if (imageIndex < 0 || imageIndex > 4) // 예시로 0~4 범위의 인덱스만 허용
            throw new ArgumentOutOfRangeException(nameof(imageIndex), "프로필 이미지 인덱스는 0에서 4 사이여야 합니다.");

        // 타임스탬프 검증
        if (createdAt == null)
            throw new ArgumentNullException(nameof(createdAt), "생성일은 null일 수 없습니다.");

        // 내용 검증
        var contentSpec = new PostContentSpecificaion();
        if (!contentSpec.IsSatisfiedBy(content))
            throw new ArgumentException(contentSpec.ErrorMessage, nameof(content));


        // 코멘트 수와 좋아요 수는 음수일 수 없음
        if (commentcount < 0)
            throw new ArgumentOutOfRangeException(nameof(commentcount), "코멘트 수는 음수일 수 없습니다.");
        if (likecount < 0)
            throw new ArgumentOutOfRangeException(nameof(likecount), "좋아요 수는 음수일 수 없습니다.");


        Id = id;
        ImageIndex = imageIndex;
        Email = email;
        Nickname = nickname;
        CreatedAt = createdAt;
        Content = content;
        CommentCount = commentcount;
        LikeCount = likecount;
    }
}