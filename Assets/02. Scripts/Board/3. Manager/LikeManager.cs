using Firebase.Auth;
using Firebase.Firestore;
using System.Threading.Tasks;

public class LikeManager
{
    private static LikeManager _instance;

    public static LikeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LikeManager();
            }
            return _instance;
        }
    }

    private readonly LikeRepository repo = new LikeRepository();
    private readonly FirebaseFirestore _db = FirebaseFirestore.DefaultInstance;

    public async Task<bool> ToggleLike(PostId postId)
    {
        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null || string.IsNullOrEmpty(user.Email)) return false;

        string email = user.Email;
        var postRef = _db.Collection("posts").Document(postId.Value);

        if (await repo.Exists(postId, email))
        {
            await repo.RemoveLike(postId, email);
            await postRef.UpdateAsync("LikeCount", FieldValue.Increment(-1));
            return false;
        }
        else
        {
            await repo.AddLike(new Like(email, postId));
            await postRef.UpdateAsync("LikeCount", FieldValue.Increment(1));
            return true;
        }
    }

    public async Task<bool> IsLiked(PostId postId)
    {
        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null) return false;

        return await repo.Exists(postId, user.UserId);
    }
}
