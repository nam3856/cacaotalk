using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_PostList : MonoBehaviour
{
    [Header("UI_Reference")]
    public Transform contentParent;             // ScrollView 안의 Content
    public GameObject postItemPrefab;           // UI_Post 프리팹

    public Button WriteButton;
    public Button LogOutButton;
    public Button PrevButton;
    public Button NextButton;
    public TMP_Text PageNumText;

    private const int PostsPerPage = 5;
    private readonly List<PostDTO> _posts = new();
    private int _currentPage = 1;

    private async void Start()
    {
        WriteButton.onClick.AddListener(OnClickWrite);
        LogOutButton.onClick.AddListener(OnClickLogOut);
        if (PrevButton != null)
            PrevButton.onClick.AddListener(OnPrevPage);
        if (NextButton != null)
            NextButton.onClick.AddListener(OnNextPage);

        await LoadAllPosts();
        UpdatePage();
    }

    private async Task LoadAllPosts()
    {
        // 기존 UI 제거
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        _posts.Clear();

        // 게시글 전체 불러오기 (단 한 번!)
        var posts = await BoardManager.Instance.LoadPost(1000);
        _posts.AddRange(posts);
    }

    private void OnClickWrite()
    {
        SceneManager.LoadScene("Post Write");
    }

    private void OnClickLogOut()
    {
        Firebase.Auth.FirebaseAuth.DefaultInstance.SignOut();
        SceneManager.LoadScene("Login");
    }

    private void OnPrevPage()
    {
        if (_currentPage <= 1) return;
        _currentPage--;
        UpdatePage();
    }

    private void OnNextPage()
    {
        if (_currentPage >= TotalPages) return;
        _currentPage++;
        UpdatePage();
    }

    private int TotalPages => Mathf.CeilToInt((float)_posts.Count / PostsPerPage);

    private void UpdatePage()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        int start = (_currentPage - 1) * PostsPerPage;
        var pagePosts = _posts.Skip(start).Take(PostsPerPage);
        foreach (var post in pagePosts)
        {
            var go = Instantiate(postItemPrefab, contentParent);
            go.GetComponent<UI_Post>().Setup(post);
        }

        if (PrevButton != null)
            PrevButton.interactable = _currentPage > 1;
        if (NextButton != null)
            NextButton.interactable = _currentPage < TotalPages;
        if (PageNumText != null)
            PageNumText.text = BuildPageLabel();
    }

    private string BuildPageLabel()
    {
        int total = TotalPages;
        int current = _currentPage;

        if (total <= 1)
            return "<b>1</b>";

        if (total <= 5)
        {
            return string.Join(" ", Enumerable.Range(1, total)
                .Select(p => p == current ? $"<b>{p}</b>" : p.ToString()));
        }

        List<string> display = new();

        // Always show first page
        display.Add(current == 1 ? "<b>1</b>" : "1");

        // 앞쪽 ...
        if (current > 3)
            display.Add("...");

        // 중간 현재 페이지
        if (current > 1 && current < total)
            display.Add($"<b>{current}</b>");

        // 뒤쪽 ...
        if (current < total - 2)
            display.Add("...");

        // Always show last page
        display.Add(current == total ? $"<b>{total}</b>" : total.ToString());

        return string.Join(" ", display);
    }


}