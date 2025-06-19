using System;
using System.Collections.Generic;
using System.Threading.Tasks;
public class BoardManager : MonoBehaviourSingleton<BoardManager>
{
    // 게시글 저장소
    private PostRepository postRepository;
    private List<PostDTO> cachedPosts = new();

    // 이벤트 정의
    public event Action<PostDTO> OnPostAdded;
    public event Action<PostDTO> OnPostUpdated;
    public event Action<PostId> OnPostDeleted;

    protected override void Awake()
    {
        base.Awake();
        postRepository = new PostRepository();
    }

    // 게시글 작성
    public async Task<PostId> AddPost(PostDTO post)
    {
        PostId postId = await postRepository.AddPost(post);
        post.Id = postId; // PostId 설정
        cachedPosts.Insert(0, post);
        OnPostAdded?.Invoke(post); // 이벤트 발생
        return postId;
    }

    // 게시글 전체 불러오기
    public async Task<List<PostDTO>> LoadPost(int limit = 20)
    {
        cachedPosts = await postRepository.GetPosts(limit);
        return cachedPosts;
    }

    // 게시글 수정
    public async Task UpdatePost(PostDTO post)
    {
        await postRepository.UpdatePost(post);
        int index = cachedPosts.FindIndex(p => p.Id == post.Id);
        if (index >= 0)
        {
            cachedPosts[index] = post; // 캐시 업데이트
            OnPostUpdated?.Invoke(post); // 이벤트 발생
        }
    }

    // 게시글 삭제
    public async Task DeletePost(PostId postId)
    {
        await postRepository.DeletePost(postId);
        cachedPosts.RemoveAll(p => p.Id == postId); // 캐시에서 삭제
        OnPostDeleted?.Invoke(postId); // 이벤트 발생
    }

    // 캐시된 게시글 목록 반환
    public List<PostDTO> GetCachedPosts()
    {
        return new List<PostDTO>(cachedPosts);
    }


    // 특정 게시글 ID 조회
    public PostDTO GetPostById(PostId postId)
    {
        return cachedPosts.Find(p => p.Id == postId);
    }
}