using Firebase.Firestore;

[FirestoreData]
public class PostDTO
{
    [FirestoreProperty]
    public string Email { get; set; }

    private string _id;

    public PostId Id
    {
        get => new PostId(_id);
        set => _id = value.Value;
    }


    [FirestoreProperty]
    public string Nickname { get; set; }

    [FirestoreProperty]
    public int ImageIndex { get; set; }

    [FirestoreProperty]
    public Timestamp CreatedAt { get; set; }

    [FirestoreProperty]
    public string Content { get; set; }

    [FirestoreProperty]
    public int CommentCount { get; set; }

    [FirestoreProperty]
    public int LikeCount { get; set; }
}
