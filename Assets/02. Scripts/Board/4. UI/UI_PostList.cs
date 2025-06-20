using System.Threading.Tasks;
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

    private int currentPage = 1;
    private const int PostsPerPage = 5;

    private async void Start()
    {
        WriteButton.onClick.AddListener(OnClickWrite);
        LogOutButton.onClick.AddListener(OnClickLogOut);

        await LoadAllPosts();
    }

    private async Task LoadAllPosts()
    {
        // 기존 UI 제거
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 게시글 전체 불러오기 (예: 1000개까지)
        var posts = await BoardManager.Instance.LoadPost(1000); // 또는 limit 없는 메서드 사용

        foreach (var post in posts)
        {
            GameObject go = Instantiate(postItemPrefab, contentParent);
            go.GetComponent<UI_Post>().Setup(post);
        }
    }

    private void OnClickWrite()
    {
        SceneManager.LoadScene("Post Write");
    }

    private void OnClickLogOut()
    {
        Firebase.Auth.FirebaseAuth.DefaultInstance.SignOut();
        SceneManager.LoadScene("SampleScene");
    }
}
