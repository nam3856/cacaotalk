// 좋아요~
// 정:
// 부:
using Firebase.Firestore;

[FirestoreData]
public class Like
{
    [FirestoreProperty]
    public string Email { get; set; }

    [FirestoreProperty]
    public PostId PostId { get; set; }

    [FirestoreProperty]
    public Timestamp LikedAt { get; set; }

    public Like() { } // Firestore 역직렬화용 기본 생성자

    public Like(string email, PostId postId)
    {
        Email = email;
        PostId = postId;
        LikedAt = Timestamp.GetCurrentTimestamp();
    }
}