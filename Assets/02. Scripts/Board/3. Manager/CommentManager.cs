using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CommentManager
{
    private readonly CommentRepository _repository;
    private readonly PostRepository _postRepository = new PostRepository();
    public event Action<List<Comment>> OnCommentsLoaded;
    public event Action<Comment> OnCommentAdded;
    public event Action<string> OnCommentDeleted;
    public event Action<string> OnError;

    public CommentManager()
    {
        _repository = new CommentRepository();
    }

    public async Task LoadCommentsAsync(string postId)
    {
        try
        {
            var comments = await _repository.GetCommentsAsync(postId);
            OnCommentsLoaded?.Invoke(comments);
        }
        catch (Exception e)
        {
            Debug.LogError($"댓글 로드 실패: {e.Message}");
            OnError?.Invoke("댓글을 불러오는 데 실패했습니다.");
        }
    }

    public async Task AddCommentAsync(string postId, string authorId, string nickname, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            OnError?.Invoke("댓글 내용을 입력해주세요.");
            return;
        }

        var comment = new Comment(postId, authorId, nickname, content);

        try
        {
            await _repository.AddCommentAsync(comment);
            await _postRepository.IncrementCommentCountAsync(postId);
            await BoardManager.Instance.RefreshPost(postId);
            OnCommentAdded?.Invoke(comment);
        }
        catch (Exception e)
        {
            Debug.LogError($"댓글 등록 실패: {e.Message}");
            OnError?.Invoke("댓글 등록에 실패했습니다.");
        }
    }

    public async Task DeleteCommentAsync(string postId, string commentId, string requesterId)
    {
        try
        {
            await _repository.DeleteCommentAsync(postId, commentId, requesterId);

            await _postRepository.DecrementCommentCountAsync(postId);
            OnCommentDeleted?.Invoke(commentId);
        }
        catch (Exception e)
        {
            Debug.LogError($"댓글 삭제 실패: {e.Message}");
            OnError?.Invoke("댓글 삭제에 실패했습니다.");
        }
    }

    
}
