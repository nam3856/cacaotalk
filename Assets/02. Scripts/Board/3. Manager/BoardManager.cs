using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class BoardManager : MonoBehaviourSingleton<BoardManager>
{
    // 게시글 저장소
    private PostRepository postRepository;
    private List<PostDTO> cachedPosts = new();
    private DocumentSnapshot lastVisibleSnapshot = null;

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

    // 게시글 페이징 로드
    public async Task<List<PostDTO>> LoadPostsPaged(int limit = 5, bool reset = false)
    {
        if (reset)
        {
            cachedPosts.Clear();
            lastVisibleSnapshot = null; // 초기화
        }

        Query query = postRepository.GetCollection().OrderByDescending("CreatedAt").Limit(limit);

        if (lastVisibleSnapshot != null)
        {
            query = query.StartAfter(lastVisibleSnapshot);
        }

        QuerySnapshot snapshot = await query.GetSnapshotAsync();

        // Document Null 체크
        if (snapshot == null || snapshot.Documents == null || !snapshot.Documents.Any())
            return new List<PostDTO>();

        foreach (var doc in snapshot.Documents)
        {
            PostDTO post = doc.ConvertTo<PostDTO>();
            post.Id = new PostId(doc.Id); // PostId 설정
            cachedPosts.Add(post);
        }

        if (snapshot.Documents.Count() > 0)
        {
            lastVisibleSnapshot = snapshot.Documents.Last(); // 마지막 문서 저장
        }

        return new List<PostDTO>(cachedPosts);
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