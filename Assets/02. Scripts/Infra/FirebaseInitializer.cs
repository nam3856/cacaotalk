using Firebase.Extensions;
using UnityEngine;

public class FirebaseInitializer : MonoBehaviour
{
    private Firebase.FirebaseApp _app;
    [SerializeField] UserDataManager userDataManager;
    private void Start()
    {
        Init();
    }

    private void Init()
    {

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                _app = Firebase.FirebaseApp.DefaultInstance;
                Debug.Log("Firebase is ready to use.");
                
                AccountManager.Instance.Init(); // Initialize UserDataManager or any other Firebase-related managers here.


                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });


    }

    
}
