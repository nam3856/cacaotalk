using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_PostList : MonoBehaviour
{
    [Header("UI_Reference")]
    public List<UI_Post> postSlots;

    public TextMeshProUGUI pageText;
    public Button PrevButton;
    public Button NextButton;
    public Button WriteButton;
    public Button LogOutButton;

    private int currentPage = 1;
    private const int PostsPerPage = 5;

    private async void Start()
    {
        await LoadPage(1);

        PrevButton.onClick.AddListener(() => LoadPage(currentPage - 1));
        NextButton.onClick.AddListener(() => LoadPage(currentPage + 1));
        WriteButton.onClick.AddListener(OnClickWrite);
        LogOutButton.onClick.AddListener(OnClickLogOut);
    }

    private async Task LoadPage(int page)
    {
        if (page < 1) return;

        // 초기화
        foreach (var slot in postSlots)
        {
            slot.gameObject.SetActive(false);
        }

        // 게시글 로드
        bool isFirst = page == 1;
        var posts = await BoardManager.Instance.LoadPostsPaged(PostsPerPage, reset: isFirst);

        if (posts.Count == 0 && page > 1) return;

        currentPage = page;
        pageText.text = currentPage.ToString();

        for (int i = 0; i < posts.Count && i < postSlots.Count; i++)
        {
            postSlots[i].Setup(posts[i]);
            postSlots[i].gameObject.SetActive(true);
        }
    }

    public void OnClickWrite()
    {
        SceneManager.LoadScene("Post Write");
    }

    public void OnClickLogOut()
    {
        Firebase.Auth.FirebaseAuth.DefaultInstance.SignOut();
        SceneManager.LoadScene("SampleScene");
    }
}
