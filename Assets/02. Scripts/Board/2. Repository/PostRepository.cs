// 게시글 저장!
// 정:
// 부:
using Firebase.Firestore;
using System;
public class PostRepository
{
    private FirebaseFirestore _db;
    public PostRepository()
    {
        _db = FirebaseFirestore.DefaultInstance;
    }

    internal void SavePost(Post post)
    {
        throw new NotImplementedException();
    }
}