using Firebase.Auth;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BoardManager : MonoBehaviourSingleton<BoardManager>
{
    // 게시글 저장소
    private PostRepository postRepository;
    private List<PostDTO> cachedPosts = new();
    private DocumentSnapshot lastVisibleSnapshot = null;
    private PostId selectedPostId;

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
    //public async Task<List<PostDTO>> LoadPostsPaged(int limit = 5, bool reset = false)
    //{
    //    if (reset)
    //    {
    //        cachedPosts.Clear();
    //        lastVisibleSnapshot = null; // 초기화
    //    }

    //    Query query = postRepository.GetCollection().OrderByDescending("CreatedAt").Limit(limit);

    //    if (lastVisibleSnapshot != null)
    //    {
    //        query = query.StartAfter(lastVisibleSnapshot);
    //    }

    //    QuerySnapshot snapshot = await query.GetSnapshotAsync();

    //    // Document Null 체크
    //    if (snapshot == null || snapshot.Documents == null || !snapshot.Documents.Any())
    //        return new List<PostDTO>();

    //    foreach (var doc in snapshot.Documents)
    //    {
    //        PostDTO post = doc.ConvertTo<PostDTO>();
    //        post.Id = new PostId(doc.Id); // PostId 설정
    //        cachedPosts.Add(post);
    //    }

    //    if (snapshot.Documents.Count() > 0)
    //    {
    //        lastVisibleSnapshot = snapshot.Documents.Last(); // 마지막 문서 저장
    //    }

    //    return new List<PostDTO>(cachedPosts);
    //}

    // 게시글 수정
    public async Task UpdatePost(PostDTO post)
    {
        var currentUser = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser;
        if (currentUser == null || currentUser.Email != post.Email)
        {
            Debug.LogError("게시글 수정 권한이 없습니다.");
            return;
        }

        await postRepository.UpdatePost(post);
        int index = cachedPosts.FindIndex(p => p.Id == post.Id);
        if (index >= 0)
        {
            cachedPosts[index] = post; // 캐시 업데이트
            OnPostUpdated?.Invoke(post); // 이벤트 발생
        }
    }

    // 내부에서 PostDTO 업데이트 시도
    public void UpdateLocalPost(PostDTO updated)
    {
        int index = cachedPosts.FindIndex(p => p.Id == updated.Id);
        if (index >= 0)
        {
            cachedPosts[index] = updated;
            OnPostUpdated?.Invoke(updated);
        }
    }


    // 게시글 삭제
    public async Task DeletePost(PostId postId)
    {
        var currentUser = FirebaseAuth.DefaultInstance.CurrentUser;
        if (currentUser == null)
        {
            Debug.LogWarning("사용자가 로그인하지 않았습니다.");
            return;
        }
        var targetPost = cachedPosts.Find(p => p.Id == postId);
        if (targetPost == null)
        {
            Debug.LogWarning("삭제할 게시글을 찾을 수 없습니다.");
            return;
        }
        if (targetPost.Email != currentUser.Email)
        {
            Debug.LogError("게시글 삭제 권한이 없습니다.");
            return;
        }

        await postRepository.DeletePost(postId);
        cachedPosts.RemoveAll(p => p.Id == postId); // 캐시에서 삭제
        OnPostDeleted?.Invoke(postId); // 이벤트 발생
    }

    // 게시글 갱신 (UI에서 호출 가능)
    public async Task RefreshPost(PostId postId)
    {
        var doc = await postRepository.GetPost(postId);
        if (doc != null)
        {
            int index = cachedPosts.FindIndex(p => p.Id == postId);
            if (index >= 0)
            {
                cachedPosts[index] = doc;
                OnPostUpdated?.Invoke(doc);
            }
        }
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

    public void SetSelectedPostId(PostId id)
    {
        // null 체크만 허용하고, 빈 문자열만 막음
        if (id != null && string.IsNullOrWhiteSpace(id.Value))
            throw new ArgumentException("PostId는 비어 있을 수 없습니다.");

        selectedPostId = id;
    }


    public PostDTO GetSelectedPost()
    {
        if (selectedPostId == null || string.IsNullOrEmpty(selectedPostId.Value))
        {
            Debug.LogWarning("선택된 게시글 ID가 설정되지 않았습니다.");
            return null;
        }

        var post = GetPostById(selectedPostId);

        if (post == null)
        {
            Debug.LogWarning($"선택된 ID({selectedPostId.Value})에 해당하는 게시글이 캐시에 없습니다.");
        }

        return post;
    }
}