using Firebase.Firestore;

[FirestoreData]
public class Like
{
    [FirestoreProperty] public string Email { get; set; }
    [FirestoreProperty] public string PostId { get; set; }
    [FirestoreProperty] public Timestamp LikedAt { get; set; }

    public Like() { }

    public Like(string email, string postId)
    {
        Email = email;
        PostId = postId;
        LikedAt = Timestamp.GetCurrentTimestamp();
    }
}
