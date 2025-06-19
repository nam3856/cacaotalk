// 게시글 저장!
// 정:
// 부:

using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PostRepository
{
    private readonly CollectionReference _posts;
    public PostRepository()
    {
        _posts = FirebaseFirestore.DefaultInstance.Collection("posts");
    }

    public async Task<PostId> AddPost(PostDTO post)
    {
        DocumentReference docRef = _posts.Document(post.Id.Value);
        await docRef.SetAsync(post);
        return post.Id;
    }

    public async Task<PostDTO> GetPost(PostId postid)
    {
        DocumentSnapshot snapshot = await _posts.Document(postid.Value).GetSnapshotAsync();
        if (!snapshot.Exists) return null;

        var post = snapshot.ConvertTo<PostDTO>();
        post.Id = postid; // PostId 설정
        return post;
    }

    public async Task<List<PostDTO>> GetPosts(int limit = 20)
    {
        var snapshot = await _posts.OrderByDescending("CreatedAt")
                                   .Limit(limit)
                                   .GetSnapshotAsync();
        List<PostDTO> posts = new List<PostDTO>();
        foreach (var doc in snapshot.Documents)
        {
            var post = doc.ConvertTo<PostDTO>();
            post.Id = new PostId(doc.Id); // PostId 설정
            posts.Add(post);
        }
        return posts;
    }

    public async Task UpdatePost(PostDTO post)
    {
        await _posts.Document(post.Id.Value).SetAsync(post);
    }

    public async Task DeletePost(PostId postid)
    {
        await _posts.Document(postid.Value).DeleteAsync();
    }

    public CollectionReference GetCollection()
    {
        return _posts;
    }
}