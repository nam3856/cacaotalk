// 게시글 저장!
// 정:
// 부:
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CommentRepository
{
    private readonly FirebaseFirestore _firestore;

    public CommentRepository()
    {
        _firestore = FirebaseFirestore.DefaultInstance;
    }

    private CollectionReference GetCommentsCollection(string postId)
    {
        return _firestore.Collection("posts").Document(postId).Collection("comments");
    }

    public async Task AddCommentAsync(Comment comment)
    {
        var collection = GetCommentsCollection(comment.PostId);
        await collection.Document(comment.CommentId).SetAsync(comment);
    }

    public async Task DeleteCommentAsync(string postId, string commentId, string requesterId)
    {
        var docRef = GetCommentsCollection(postId).Document(commentId);
        var snapshot = await docRef.GetSnapshotAsync();

        if (!snapshot.Exists)
        {
            Debug.LogWarning("댓글이 존재하지 않습니다.");
            return;
        }

        var comment = snapshot.ConvertTo<Comment>();

        if (comment.AuthorId != requesterId)
        {
            Debug.LogWarning("본인이 작성한 댓글만 삭제할 수 있습니다.");
            return;
        }

        await docRef.DeleteAsync();
    }

    public async Task<List<Comment>> GetCommentsAsync(string postId)
    {
        var snapshot = await GetCommentsCollection(postId)
            .OrderBy("CreatedAt")
            .GetSnapshotAsync();

        List<Comment> comments = new List<Comment>();
        foreach (var doc in snapshot.Documents)
        {
            var tempComment = doc.ConvertTo<Comment>();
            comments.Add(tempComment);
            //Debug.Log(tempComment.CommentId + " " + tempComment.Content + " " + tempComment.CreatedAt.ToDateTime() + " " + tempComment.ImageIndex);
        }
        return comments;
    }
}
