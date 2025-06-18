using UnityEngine;
using System.Collections.Generic;
public class BoardManager : MonoBehaviour
{
    // 게시글 저장소
    private PostRepository postRepository;
    // 게시글 목록
    private List<Post> posts;
    // 초기화
    private void Awake()
    {
        postRepository = new PostRepository();
        posts = new List<Post>();
    }
    // 게시글 추가
    public void AddPost(Post post)
    {
        posts.Add(post);
        postRepository.SavePost(post);
    }
    // 게시글 목록 가져오기
    public List<Post> GetPosts()
    {
        return posts;
    }
}
{
}