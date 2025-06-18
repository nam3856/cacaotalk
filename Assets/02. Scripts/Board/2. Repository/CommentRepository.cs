// 게시글 저장!
// 정:
// 부:
using Firebase.Firestore;
public class CommentRepository
{
    private FirebaseFirestore _db;
    public CommentRepository()
    {
        _db = FirebaseFirestore.DefaultInstance;
    }
}