using UnityEngine;
using System;
public class UI_Comment : MonoBehaviour
{
    private CommentManager _commentManager;

    public void Initialize()
    {
        _commentManager = new();
    }
    void OnGUI()
    {
        if (GUI.Button(new Rect(10,120,150,100), "Init"))
        {
            Initialize();
        }
        if (GUI.Button(new Rect(10, 10, 150, 100), "Comment"))
        {
            TestComment();
        }
    }

    private async void TestComment()
    {
        string testPostId = "a";

        await _commentManager.AddCommentAsync(
            testPostId,
            "user-123",
            "경민남",
            "이거 댓글 기능 테스트 중입니다."
        );

        await _commentManager.LoadCommentsAsync(testPostId);
    }
}