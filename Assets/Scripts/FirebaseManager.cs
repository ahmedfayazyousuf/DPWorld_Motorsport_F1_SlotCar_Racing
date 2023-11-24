using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public FirebaseFirestore firestoreDb;
    private bool isFirebaseInitialized = false;

    public FirebaseFirestore FirestoreDb
    {
        get { return firestoreDb; }
    }

    void Start()
    {
        InitializeFirebase();
    }

    public bool IsFirebaseInitialized
    {
        get { return isFirebaseInitialized; }
    }

    void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;

            firestoreDb = FirebaseFirestore.DefaultInstance;

            if (firestoreDb != null)
            {
                Debug.Log("Firestore has been initialized.");
                isFirebaseInitialized = true; // Set the initialization status to true

            }
            else
            {
                Debug.LogError("Failed to initialize Firestore.");
            }
        });
    }
}
