// 댓글달기
// 정:
// 부:
using Firebase.Firestore;
using System;

[FirestoreData]
public class Comment
{
    [FirestoreProperty]
    public string CommentId { get; private set; }

    [FirestoreProperty]
    public string PostId { get; private set; }

    [FirestoreProperty]
    public string AuthorId { get; private set; }

    [FirestoreProperty]
    public string AuthorNickname { get; private set; }

    [FirestoreProperty]
    public string Content { get; private set; }

    [FirestoreProperty]
    public Timestamp CreatedAt { get; private set; }

    [FirestoreProperty]
    public int ImageIndex { get; private set; }

    public Comment() { }

    public Comment(string postId, string authorId, string authorNickname, string content, int imageIndex)
    {
        CommentId = Guid.NewGuid().ToString();
        PostId = postId;
        AuthorId = authorId;
        AuthorNickname = authorNickname;
        Content = content;
        CreatedAt = Timestamp.GetCurrentTimestamp();
        ImageIndex = imageIndex;
    }
}
