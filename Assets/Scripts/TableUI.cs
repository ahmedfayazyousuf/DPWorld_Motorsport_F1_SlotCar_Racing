using UnityEngine;
using TMPro;
using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine.Video;

using System.Collections;
using Firebase.Database;
using System;
using UnityEngine.Video;
using System.IO.Ports;
using Unity.VisualScripting;

public class TableUI : MonoBehaviour
{

    private float currentLapTimePlayer1;
    private float currentLapTimePlayer2;

    private string Player1FullName = "";
    private string Player2FullName = "";

    public VideoPlayer DPWorldPort;
    public VideoPlayer AINDubai;
    public VideoPlayer BurjKhalifa;
    public VideoPlayer MoFuture;

    public VideoClip DPWorldPortClip;
    public VideoClip AINDubaiClip;
    public VideoClip BurjKhalifaClip;
    public VideoClip MoFutureClip;

    public VideoPlayer DiscsAnimation;
    public VideoClip DiscsAnimationClip;
    public VideoClip IdleDiscsAnimationClip;

    public VideoPlayer TrackTriggerAnimation;
    public VideoClip TrackTriggerAnimationClip;

    public VideoPlayer TrackTriggerAnimation2;
    public VideoClip TrackTriggerAnimationClip2;

    public TextMeshProUGUI Player1Name;
    public TextMeshProUGUI Player2Name;

    public FirebaseManager firebaseManager;

    public GameObject LeaderboardPanel;
    public GameObject LapsPanel;
    public GameObject MapTop;

    public TextMeshProUGUI Player1Lap1TimeText;
    public TextMeshProUGUI Player1Lap2TimeText;
    public TextMeshProUGUI Player1Lap3TimeText;
    public TextMeshProUGUI Player1Lap4TimeText;
    public TextMeshProUGUI Player1Lap5TimeText;
    public TextMeshProUGUI Player1TotalRaceTimeText;

    public TextMeshProUGUI Player2Lap1TimeText;
    public TextMeshProUGUI Player2Lap2TimeText;
    public TextMeshProUGUI Player2Lap3TimeText;
    public TextMeshProUGUI Player2Lap4TimeText;
    public TextMeshProUGUI Player2Lap5TimeText;
    public TextMeshProUGUI Player2TotalRaceTimeText;
    public TextMeshProUGUI TextError;

    private bool raceStarted = false;

    private float Player1LapStartTime;
    private int Player1LapCount = 0; // Player 1 Lap count (max 5 laps)
    private float[] Player1LapTimes = new float[5]; // Array to store lap times for player 1
    private float Player1TotalRaceTime = 0; // Total race time player 1
    private bool Player1LapInProgress = false; // Variable to track if a lap is in progress

    private float Player2LapStartTime;
    private int Player2LapCount = 0; // Player 2 Lap count (max 5 laps)
    private float[] Player2LapTimes = new float[5]; // Array to store lap times for player 2
    private float Player2TotalRaceTime = 0; // Total race time player 2
    private bool Player2LapInProgress = false; // Variable to track if a lap is in progress

    private string Player1Email = "";
    private string Player2Email = "";

    private string Player1CCode = "";
    private string Player2CCode = "";

    private string Player1Number = "";
    private string Player2Number = "";

    private bool restartRequested = false;

    private Queue<int> signalQueue = new Queue<int>();

    public TextMeshProUGUI playerName1;
    public TextMeshProUGUI playerName2;
    public TextMeshProUGUI playerName3;
    public TextMeshProUGUI playerName4;
    public TextMeshProUGUI playerName5;
    public TextMeshProUGUI playerName6;
    public TextMeshProUGUI playerName7;
    public TextMeshProUGUI playerName8;
    public TextMeshProUGUI playerName9;
    public TextMeshProUGUI playerName10;

    public TextMeshProUGUI playerScore1;
    public TextMeshProUGUI playerScore2;
    public TextMeshProUGUI playerScore3;
    public TextMeshProUGUI playerScore4;
    public TextMeshProUGUI playerScore5;
    public TextMeshProUGUI playerScore6;
    public TextMeshProUGUI playerScore7;
    public TextMeshProUGUI playerScore8;
    public TextMeshProUGUI playerScore9;
    public TextMeshProUGUI playerScore10;

    FirebaseFirestore firestoreDb;



    void Start()
    {
        Debug.Log("displays connected: " + Display.displays.Length);

        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }

        LapsPanel.SetActive(false);
        MapTop.SetActive(false);
        Invoke("MakeTrue", 3f);


        // Initialize Firebase Firestore
        firestoreDb = FirebaseFirestore.DefaultInstance;

        // Fetch leaderboard data initially
        FetchLeaderboard();

        // Subscribe to leaderboard updates using OnSnapshot
        SubscribeToLeaderboardUpdates();
        StartCoroutine(ContinuousSignalHandling());
    }

    IEnumerator ContinuousSignalHandling()
    {

        yield return new WaitForSeconds(3f);
        FetchPlayerNames();


        while (true)
        {
            yield return new WaitUntil(() => ArduinoManager.instance.LastSerialMessage != null);

            if (int.TryParse(ArduinoManager.instance.LastSerialMessage, out int parsedValue))
            {
                signalQueue.Enqueue(parsedValue);
            }
        }
    }

    void Update()
    {
        while (signalQueue.Count > 0)
        {
            int signal = signalQueue.Dequeue();
            HandleSignal(signal);
        }

        if (Player1LapInProgress)
        {
            currentLapTimePlayer1 = Time.time - Player1LapStartTime;
            UpdateLapTimeUI(Player1LapCount, currentLapTimePlayer1, 1);
        }

        if (Player2LapInProgress)
        {
            currentLapTimePlayer2 = Time.time - Player2LapStartTime;
            UpdateLapTimeUI(Player2LapCount, currentLapTimePlayer2, 2);
        }

        //if (!raceStarted && Input.GetKeyDown(KeyCode.X))

        if (Input.GetKeyDown(KeyCode.X))
        {
            MapTop.SetActive(false);
            ArduinoManager.instance.SendSerialMessage("8");
            Debug.Log("Sent Signal 8 to Arduino");
            LeaderboardPanel.SetActive(false);
            LapsPanel.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {

            LapsPanel.SetActive(false);
            LeaderboardPanel.SetActive(true);
            int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneIndex);

            int currentSceneIndex2 = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneIndex2);
        }
    }

    void StopAndDisableVideo(VideoPlayer videoPlayer)
    {
        // Stop the video playback
        videoPlayer.Stop();

        // Disable the VideoPlayer component
        videoPlayer.enabled = false;
    }


    void UpdateLapTimeUI(int lapCount, float currentLapTime, int PlayerNumber)
    {
        string formattedLapTime = currentLapTime.ToString("F2");

        // Update the corresponding TextMeshPro Pro Text object with the formatted lap time
        if (PlayerNumber == 1)
        {
            switch (lapCount)
            {
                case 0:
                    Player1Lap1TimeText.text = formattedLapTime;
                    break;
                case 1:
                    Player1Lap2TimeText.text = formattedLapTime;
                    break;
                case 2:
                    Player1Lap3TimeText.text = formattedLapTime;
                    break;
                case 3:
                    Player1Lap4TimeText.text = formattedLapTime;
                    break;
                case 4:
                    Player1Lap5TimeText.text = formattedLapTime;
                    break;
            }
        }

        if (PlayerNumber == 2)
        {
            switch (lapCount)
            {
                case 0:
                    Player2Lap1TimeText.text = formattedLapTime;
                    break;
                case 1:
                    Player2Lap2TimeText.text = formattedLapTime;
                    break;
                case 2:
                    Player2Lap3TimeText.text = formattedLapTime;
                    break;
                case 3:
                    Player2Lap4TimeText.text = formattedLapTime;
                    break;
                case 4:
                    Player2Lap5TimeText.text = formattedLapTime;
                    break;
            }
        }

    }


    void PlayVideo(VideoPlayer videoPlayer, VideoClip clip)
    {
        // Stop any currently playing videos
        videoPlayer.Stop();

        // Disable all VideoPlayer components except the one to be played
        videoPlayer.enabled = false;

        // Play the specified video
        videoPlayer.enabled = true;
        videoPlayer.clip = clip;
        videoPlayer.Play();
    }

    void MakeTrue()
    {
        Debug.Log("You Entered MakeTrue");
        MapTop.SetActive(true);
    }

    void HandleSignal(int signal)
    {
        switch (signal)
        {
            case 1:
                Debug.Log("Signal 1 - BurjKhalifa");
                PlayVideo(BurjKhalifa, BurjKhalifaClip);
                break;

            case 2:
                Debug.Log("Signal 2 - AINDubai");
                PlayVideo(AINDubai, AINDubaiClip);
                break;

            case 3:
                Debug.Log("Signal 3 - DPWorldPort");
                PlayVideo(DPWorldPort, DPWorldPortClip);
                break;

            case 4:
                Debug.Log("Signal 4 - MoFuture");
                PlayVideo(MoFuture, MoFutureClip);
                break;


            //PLAYER 1 CASE SIGNAL 5
            case 5:
                if (raceStarted && Player1LapInProgress)
                {
                    float Player1LapTime = Time.time - Player1LapStartTime;
                    Player1LapTimes[Player1LapCount] = Player1LapTime;
                    Player1TotalRaceTime += Player1LapTime;

                    // Format lap time with two decimal points
                    string formattedLapTime = Player1LapTime.ToString("F2");

                    // Update the corresponding TextMesh Pro Text object with the formatted lap time
                    switch (Player1LapCount)
                    {
                        case 0:
                            Player1Lap1TimeText.text = formattedLapTime;
                            break;
                        case 1:
                            Player1Lap2TimeText.text = formattedLapTime;
                            break;
                        case 2:
                            Player1Lap3TimeText.text = formattedLapTime;
                            break;
                        case 3:
                            Player1Lap4TimeText.text = formattedLapTime;
                            break;
                        case 4:
                            Player1Lap5TimeText.text = formattedLapTime;
                            break;
                    }

                    Player1LapCount++;
                    Player1LapStartTime = Time.time;

                    if (Player1LapCount == 5)
                    {
                        Player1LapInProgress = false; // Mark the end of the last lap

                        // Format total race time with two decimal points
                        string formattedTotalRaceTime = Player1TotalRaceTime.ToString("F2");

                        // Update the TextMesh Pro Text object for total time
                        Player1TotalRaceTimeText.text = formattedTotalRaceTime;

                        //Debug.Log("Lap 5 Time: " + formattedLapTime);
                        Debug.Log("Total Race Time P1: " + formattedTotalRaceTime);

                        // You can perform actions for race completion here
                        // Fetch player 1's name from Firestore

                        updatePlayer(Player1TotalRaceTime, 1);

                    }
                }
                else
                {
                    if (raceStarted)
                    {
                        Player1LapInProgress = true; // Mark the start of a new lap
                    }
                    else
                    {
                        raceStarted = true;
                        Player1LapStartTime = Time.time;
                        Player1LapCount = 0;
                        Player1TotalRaceTime = 0;
                        Player1LapInProgress = true; // Start the first lap
                        Debug.Log("Race started by receiving signal 6");
                    }
                }
                break;




            //PLAYER 2 CASE SIGNAL 6
            case 6:
                if (raceStarted && Player2LapInProgress)
                {
                    float Player2LapTime = Time.time - Player2LapStartTime;
                    Player2LapTimes[Player2LapCount] = Player2LapTime;
                    Player2TotalRaceTime += Player2LapTime;

                    // Format lap time with two decimal points
                    string formattedLapTime = Player2LapTime.ToString("F2");

                    // Update the corresponding TextMesh Pro Text object with the formatted lap time
                    switch (Player2LapCount)
                    {
                        case 0:
                            Player2Lap1TimeText.text = formattedLapTime;
                            break;
                        case 1:
                            Player2Lap2TimeText.text = formattedLapTime;
                            break;
                        case 2:
                            Player2Lap3TimeText.text = formattedLapTime;
                            break;
                        case 3:
                            Player2Lap4TimeText.text = formattedLapTime;
                            break;
                        case 4:
                            Player2Lap5TimeText.text = formattedLapTime;
                            break;
                    }

                    Player2LapCount++;
                    Player2LapStartTime = Time.time;

                    if (Player2LapCount == 5)
                    {
                        Player2LapInProgress = false; // Mark the end of the last lap

                        // Format total race time with two decimal points
                        string formattedTotalRaceTime = Player2TotalRaceTime.ToString("F2");

                        // Update the TextMesh Pro Text object for total time
                        Player2TotalRaceTimeText.text = formattedTotalRaceTime;

                        Debug.Log("Total Race Time P2: " + formattedTotalRaceTime);

                        // You can perform actions for race completion here
                        updatePlayer(Player2TotalRaceTime, 2);
                    }
                }
                else
                {
                    if (raceStarted)
                    {
                        Player2LapInProgress = true; // Mark the start of a new lap
                    }
                    else
                    {
                        raceStarted = true;
                        Player2LapStartTime = Time.time;
                        Player2LapCount = 0;
                        Player2TotalRaceTime = 0;
                        Player2LapInProgress = true; // Start the first lap
                        Debug.Log("Race started by receiving signal 7");
                    }
                }
                break;

            case 8:
                // Stop and disable the existing video clip
                StopAndDisableVideo(DiscsAnimation);

                // Play the IdleDiscsAnimationClip
                PlayVideo(DiscsAnimation, DiscsAnimationClip);

                PlayVideo(TrackTriggerAnimation, TrackTriggerAnimationClip);
                PlayVideo(TrackTriggerAnimation2, TrackTriggerAnimationClip2);

                Invoke("MakeTrue", 3f);

                raceStarted = true;

                Player1LapStartTime = Time.time;
                Player2LapStartTime = Time.time;

                Player1LapInProgress = true; // Start the first lap For Player 1
                Player2LapInProgress = true; // Start the first lap For Player 2

                Debug.Log("Received signal 8, timer started");

                break;

            default:
                Debug.Log("Received an unrecognized signal: " + signal);
                break;
        }
    }



    private void updatePlayer(float score, int playerNo)
    {

        if (firebaseManager == null)
        {
            Debug.LogError("FirebaseManager is not assigned. Make sure to assign it in the Inspector.");
            return;
        }
        Debug.LogError(Player1Name.text);



        if (playerNo == 1)
        {
            var data = new
            {
                Score = score,
                Name = Player1Name.text,
                Email = Player1Email,
                CCode = Player1CCode,
                Number = Player1Number,
            };
            FirebaseFirestore firestoreDb = firebaseManager.FirestoreDb;
            firestoreDb.Collection("Leaderboard").AddAsync(data);
        }

        if (playerNo == 2)
        {
            var data = new
            {
                Score = score,
                Name = Player2Name.text,
                Email = Player2Email,
                CCode = Player2CCode,
                Number = Player2Number,
            };
            FirebaseFirestore firestoreDb = firebaseManager.FirestoreDb;
            firestoreDb.Collection("Leaderboard").AddAsync(data);
        }


    }
    private void FetchPlayerNames()
    {
        // ... (existing code)

        // Fetch player 1's name from Firestore
        ListenerRegistration listenerRegistrationPlayer1 = firestoreDb.Collection("CurrentUsers")
            .Document("NAkbomUuiHLMXgFzshiv")
            .Listen(snapshot =>
            {
                MainThread.Request(() =>
                {
                    if (!snapshot.Exists)
                    {
                        Debug.LogError("Failed to fetch Player 1 data: ");
                    }
                    else
                    {
                        Dictionary<string, object> data = snapshot.ToDictionary();
                        if (data.TryGetValue("Name", out var name))
                        {
                            Player1FullName = name.ToString();
                            string[] nameParts = Player1FullName.Split(' ');
                            if (nameParts.Length > 0)
                            {
                                Player1Name.text = nameParts[0];
                                Player1Email = data["Email"].ToString();
                                Player1CCode = data["CountryCode"].ToString();
                                Player1Number = data["Number"].ToString();
                            }
                        }
                    }
                });
            });

        // Fetch player 2's name from Firestore
        ListenerRegistration listenerRegistrationPlayer2 = firestoreDb.Collection("CurrentUsers")
            .Document("kxRFF0co7QdYrHyjFFJg")
            .Listen(snapshot =>
            {
                MainThread.Request(() =>
                {
                    if (!snapshot.Exists)
                    {
                        Debug.LogError("Failed to fetch Player 2 data: ");
                    }
                    else
                    {
                        Dictionary<string, object> data = snapshot.ToDictionary();
                        if (data.TryGetValue("Name", out var name))
                        {
                            Player2FullName = name.ToString();
                            string[] nameParts = Player2FullName.Split(' ');
                            if (nameParts.Length > 0)
                            {
                                Player2Name.text = nameParts[0];
                                Player2Email = data["Email"].ToString();
                                Player2CCode = data["CountryCode"].ToString();
                                Player2Number = data["Number"].ToString();
                            }
                        }
                    }
                });
            });
    }

    async void FetchLeaderboard()
    {
        // Query to fetch the top 10 players based on the 'Score' field in descending order
        CollectionReference leaderboardRef = firestoreDb.Collection("Leaderboard");
        Firebase.Firestore.Query query = leaderboardRef.OrderBy("Score").Limit(10);

        // Fetch the data and update the UI
        try
        {
            QuerySnapshot snapshot = await query.GetSnapshotAsync();
            UpdateLeaderboardUI(snapshot.Documents);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to fetch leaderboard data: " + e.Message);
            TextError.text = e.Message;
        }
    }


    void SubscribeToLeaderboardUpdates()
    {
        // Subscribe to leaderboard updates using OnSnapshot
        CollectionReference leaderboardRef = firestoreDb.Collection("Leaderboard");
        if (leaderboardRef == null)
        {
            Debug.LogError("Error listening to leaderboard updates: ");
            return;
        }

        leaderboardRef.Listen(snapshot =>
        {
            if (leaderboardRef == null)
            {
                Debug.LogError("Error listening to leaderboard updates: ");
                return;
            }

            // Update the UI with the latest data
            UpdateLeaderboardUI(snapshot.Documents);
        });
    }

    void UpdateLeaderboardUI(IEnumerable<DocumentSnapshot> documents)
    {
        // Update the UI with the latest leaderboard data
        int index = 1; // Start index from 1 to match variable names
        foreach (DocumentSnapshot document in documents)
        {
            string playerName = document.GetValue<string>("Name");
            double playerScore = document.GetValue<double>("Score");

            // Update the TextMeshPro objects for player names and scores
            UpdatePlayerUI(index, playerName, playerScore);

            index++;
        }
    }

    void UpdatePlayerUI(int index, string playerName, double playerScore)
    {
        switch (index)
        {

            case 1:
                playerName1.text = playerName;
                playerScore1.text = playerScore.ToString("F2");
                break;
            case 2:
                playerName2.text = playerName;
                playerScore2.text = playerScore.ToString("F2");
                break;
            case 3:
                playerName3.text = playerName;
                playerScore3.text = playerScore.ToString("F2");
                break;
            case 4:
                playerName4.text = playerName;
                playerScore4.text = playerScore.ToString("F2");
                break;
            case 5:
                playerName5.text = playerName;
                playerScore5.text = playerScore.ToString("F2");
                break;
            case 6:
                playerName6.text = playerName;
                playerScore6.text = playerScore.ToString("F2");
                break;
            case 7:
                playerName7.text = playerName;
                playerScore7.text = playerScore.ToString("F2");
                break;
            case 8:
                playerName8.text = playerName;
                playerScore8.text = playerScore.ToString("F2");
                break;
            case 9:
                playerName9.text = playerName;
                playerScore9.text = playerScore.ToString("F2");
                break;
            case 10:
                playerName10.text = playerName;
                playerScore10.text = playerScore.ToString("F2");
                break;
        }
    }

}
