using Firebase.Firestore;
using System.Threading.Tasks;

public class LikeRepository
{
    private readonly FirebaseFirestore _db = FirebaseFirestore.DefaultInstance;

    public DocumentReference GetLikeRef(PostId postId, string email)
    {
        // 좋아요 문서의 경로를 생성합니다.
        return _db.Collection("likes").Document($"{postId}_{email}");
    }

    public async Task<bool> Exists(PostId postId, string email)
    {
        var snapshot = await GetLikeRef(postId, email).GetSnapshotAsync();
        return snapshot.Exists;
    }

    public async Task AddLike(Like like)
    {
        // 좋아요를 추가합니다.
        await GetLikeRef(like.PostId, like.Email).SetAsync(like);
    }

    public async Task RemoveLike(PostId postId, string email)
    {
        // 좋아요를 제거합니다.
        await GetLikeRef(postId, email).DeleteAsync();
    }
}