using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class QuizScoreRecord
{
    public string quizTitle;
    public int highScore;
    public int totalQuestions;
}

[Serializable]
public class QuizAttemptRecord
{
    public string quizTitle;
    public int score;
    public int totalQuestions;
    public string datePlayed; // format: "yyyy-MM-dd HH:mm"
}

[Serializable]
public class UserProfile
{
    public string username;
    public int streak;
    public string lastPlayedDate; // format: "yyyy-MM-dd"
    public int totalXp;
    public List<QuizScoreRecord> highScores = new List<QuizScoreRecord>();
    public List<QuizAttemptRecord> quizHistory = new List<QuizAttemptRecord>();
}

[Serializable]
public class LeaderboardEntry
{
    public string username;
    public int score;
    public int totalQuestions;
    public float timeTaken;
    public string dateAchieved;
}

[Serializable]
public class QuizLeaderboard
{
    public string quizTitle;
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
}

[Serializable]
public class LeaderboardContainer
{
    public List<QuizLeaderboard> leaderboards = new List<QuizLeaderboard>();
}

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance;

    private string profilePath;
    private string leaderboardPath;

    public UserProfile CurrentProfile { get; private set; }
    private LeaderboardContainer leaderboardData = new LeaderboardContainer();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            profilePath = Path.Combine(Application.persistentDataPath, "UserProfile.json");
            leaderboardPath = Path.Combine(Application.persistentDataPath, "Leaderboards.json");
            LoadLeaderboard();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- PROFILE MANAGEMENT ---

    public bool HasProfile()
    {
        return File.Exists(profilePath);
    }

    public void CreateProfile(string username)
    {
        CurrentProfile = new UserProfile
        {
            username = username,
            streak = 1,
            lastPlayedDate = DateTime.Now.ToString("yyyy-MM-dd"),
            totalXp = 0
        };
        SaveProfile();
    }

    public void LoadProfile()
    {
        if (File.Exists(profilePath))
        {
            try
            {
                string json = File.ReadAllText(profilePath);
                CurrentProfile = JsonUtility.FromJson<UserProfile>(json);
                UpdateStreak();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading user profile: {e.Message}");
                CreateProfile("Guest");
            }
        }
        else
        {
            CurrentProfile = null;
        }
    }

    public void SaveProfile()
    {
        if (CurrentProfile == null) return;
        try
        {
            string json = JsonUtility.ToJson(CurrentProfile, true);
            File.WriteAllText(profilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving user profile: {e.Message}");
        }
    }

    private void UpdateStreak()
    {
        if (CurrentProfile == null || string.IsNullOrEmpty(CurrentProfile.lastPlayedDate)) return;

        try
        {
            DateTime lastDate = DateTime.ParseExact(CurrentProfile.lastPlayedDate, "yyyy-MM-dd", null);
            DateTime today = DateTime.Today;
            double daysDiff = (today - lastDate).TotalDays;

            if (daysDiff >= 1.0f && daysDiff < 2.0f)
            {
                // Played yesterday, increment streak
                CurrentProfile.streak++;
                CurrentProfile.lastPlayedDate = today.ToString("yyyy-MM-dd");
                SaveProfile();
            }
            else if (daysDiff >= 2.0f)
            {
                // Missed a day, reset streak
                CurrentProfile.streak = 1;
                CurrentProfile.lastPlayedDate = today.ToString("yyyy-MM-dd");
                SaveProfile();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Streak computation error: {e.Message}");
        }
    }

    public void RecordQuizCompletion(string quizTitle, int score, int totalQuestions, int xpEarned)
    {
        if (CurrentProfile == null) return;

        // Add XP
        CurrentProfile.totalXp += xpEarned;

        // Update High Score
        QuizScoreRecord record = CurrentProfile.highScores.Find(r => r.quizTitle == quizTitle);
        if (record == null)
        {
            record = new QuizScoreRecord
            {
                quizTitle = quizTitle,
                highScore = score,
                totalQuestions = totalQuestions
            };
            CurrentProfile.highScores.Add(record);
        }
        else
        {
            if (score > record.highScore)
            {
                record.highScore = score;
                record.totalQuestions = totalQuestions;
            }
        }

        // Record Quiz Attempt in History
        if (CurrentProfile.quizHistory == null)
        {
            CurrentProfile.quizHistory = new List<QuizAttemptRecord>();
        }
        CurrentProfile.quizHistory.Add(new QuizAttemptRecord
        {
            quizTitle = quizTitle,
            score = score,
            totalQuestions = totalQuestions,
            datePlayed = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
        });

        // Update last played date to today
        CurrentProfile.lastPlayedDate = DateTime.Today.ToString("yyyy-MM-dd");
        SaveProfile();
    }

    // --- LEADERBOARD MANAGEMENT ---

    private void LoadLeaderboard()
    {
        if (File.Exists(leaderboardPath))
        {
            try
            {
                string json = File.ReadAllText(leaderboardPath);
                leaderboardData = JsonUtility.FromJson<LeaderboardContainer>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading leaderboards: {e.Message}");
                leaderboardData = new LeaderboardContainer();
            }
        }
    }

    public void SaveLeaderboard()
    {
        try
        {
            string json = JsonUtility.ToJson(leaderboardData, true);
            File.WriteAllText(leaderboardPath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving leaderboards: {e.Message}");
        }
    }

    public void AddLeaderboardEntry(string quizTitle, string username, int score, int totalQuestions, float timeTaken)
    {
        QuizLeaderboard qlb = leaderboardData.leaderboards.Find(l => l.quizTitle == quizTitle);
        if (qlb == null)
        {
            qlb = new QuizLeaderboard { quizTitle = quizTitle };
            leaderboardData.leaderboards.Add(qlb);
        }

        LeaderboardEntry entry = new LeaderboardEntry
        {
            username = username,
            score = score,
            totalQuestions = totalQuestions,
            timeTaken = timeTaken,
            dateAchieved = DateTime.Now.ToString("yyyy-MM-dd")
        };

        qlb.entries.Add(entry);

        // Sort: Descending by score, then Ascending by timeTaken
        qlb.entries.Sort((a, b) =>
        {
            if (b.score != a.score)
                return b.score.CompareTo(a.score);
            return a.timeTaken.CompareTo(b.timeTaken);
        });

        // Cap at top 10
        if (qlb.entries.Count > 10)
        {
            qlb.entries.RemoveRange(10, qlb.entries.Count - 10);
        }

        SaveLeaderboard();
    }

    public List<LeaderboardEntry> GetLeaderboard(string quizTitle)
    {
        QuizLeaderboard qlb = leaderboardData.leaderboards.Find(l => l.quizTitle == quizTitle);
        if (qlb != null)
        {
            return qlb.entries;
        }
        return new List<LeaderboardEntry>();
    }
}
