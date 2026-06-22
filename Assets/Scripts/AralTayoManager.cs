using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AralTayoManager : MonoBehaviour
{
    public static AralTayoManager Instance;

    [Header("Panels")]
    public GameObject loginPanel;
    public GameObject mainMenuPanel;
    public GameObject categorySelectPanel;
    public GameObject quizDetailsPanel;
    public GameObject studyPanel;
    public GameObject quizScreen;
    public GameObject feedbackOverlay;
    public GameObject resultsScreen;
    public GameObject leaderboardPanel;
    public GameObject historyPanel;

    [Header("Login Panel")]
    public TMP_InputField usernameInput;
    public Button loginBtn;
    public TMP_Text loginWarningText;

    [Header("Main Menu Panel")]
    public TMP_Text welcomeText;
    public TMP_Text streakText;
    public TMP_Text xpText;
    public Button playBtn;
    public Button mainMenuLeaderboardBtn;
    public Button mainMenuHistoryBtn;
    public Button logoutBtn;
    public Button exitBtn;

    [Header("Category Selection Panel")]
    public Transform categoryScrollContent;
    public GameObject categoryItemPrefab; // Prefab with a Button and TMP_Text
    public Button backFromCategoryBtn;

    [Header("Quiz Selection/Details Panel")]
    public TMP_Dropdown quizDropdown;
    public TMP_Text quizTitleText;
    public TMP_Text quizDescText;
    public TMP_Text quizMetaText;
    public Slider questionSlider;
    public TMP_Text sliderValueText;
    public Toggle studyModeToggle;
    public Button startQuizBtn;
    public Button backFromDetailsBtn;

    [Header("Study / Lesson Panel")]
    public TMP_Text studyTitleText;
    public TMP_Text studyContentText;
    public TMP_Text slideProgressText;
    public Button nextSlideBtn;
    public Button prevSlideBtn;
    public Button backFromStudyBtn;

    [Header("Quiz Screen")]
    public TMP_Text progressText;
    public TMP_Text questionText;
    public TMP_Text timerText;
    public Button[] answerBtns;

    [Header("Feedback Overlay")]
    public TMP_Text feedbackResultText;
    public TMP_Text feedbackAnswerText;
    public Button nextQuestionBtn;

    [Header("Results Screen")]
    public TMP_Text scoreText;
    public TMP_Text xpGainedText;
    public Transform resultsScrollContent;
    public GameObject resultItemPrefab;
    public Button resultsBackToMenuBtn;
    public Button viewLeaderboardBtn;

    [Header("Leaderboard Panel")]
    public TMP_Text leaderboardTitleText;
    public Transform leaderboardScrollContent;
    public GameObject leaderboardItemPrefab;
    public Button backFromLeaderboardBtn;

    [Header("History Panel")]
    public Transform historyScrollContent;
    public GameObject historyItemPrefab;
    public Button backFromHistoryBtn;

    // Gameplay and Navigation State
    private int questionCount = 5;
    private List<QuestionItem> activeQuestions = new List<QuestionItem>();
    private int currentIndex;
    private int score;
    private string currentCorrectAnswer;
    private bool isTrueFalse;
    private List<(string question, string correct, bool wasCorrect)> sessionLog = new List<(string, string, bool)>();

    private string selectedCategory;
    private string selectedQuizName;
    private QuizDataFile selectedQuizData;

    // Study state
    private List<LessonSlide> activeLessons = new List<LessonSlide>();
    private int currentSlideIndex;

    // Timer state
    private float quizTimer;
    private bool isTimerRunning;
    private float questionTimer;
    private bool isQuestionTimerRunning;
    private const float QUESTION_TIME_LIMIT = 20f;

    void Awake() => Instance = this;

    void Start()
    {
        // Setup initial UI list listeners and hooks
        SetupButtonListeners();
        SetupSlider();
        
        // Load player profile
        ProfileManager.Instance.LoadProfile();

        if (ProfileManager.Instance.CurrentProfile == null)
        {
            ShowOnly(loginPanel);
        }
        else
        {
            UpdateProfileUI();
            ShowOnly(mainMenuPanel);
        }
    }

    void SetupButtonListeners()
    {
        // Login
        loginBtn.onClick.AddListener(OnLoginClicked);

        // Main Menu
        playBtn.onClick.AddListener(OnPlayClicked);
        mainMenuLeaderboardBtn.onClick.AddListener(OnMainMenuLeaderboardClicked);
        if (mainMenuHistoryBtn != null) mainMenuHistoryBtn.onClick.AddListener(OnMainMenuHistoryClicked);
        logoutBtn.onClick.AddListener(OnLogoutClicked);
        exitBtn.onClick.AddListener(Application.Quit);

        if (backFromHistoryBtn != null) backFromHistoryBtn.onClick.AddListener(() => ShowOnly(mainMenuPanel));

        // Category Selection
        backFromCategoryBtn.onClick.AddListener(() => ShowOnly(mainMenuPanel));

        // Quiz Selection/Details
        startQuizBtn.onClick.AddListener(StartQuizFlow);
        backFromDetailsBtn.onClick.AddListener(() => ShowOnly(categorySelectPanel));

        // Study Panel
        nextSlideBtn.onClick.AddListener(OnNextSlideClicked);
        prevSlideBtn.onClick.AddListener(OnPrevSlideClicked);
        backFromStudyBtn.onClick.AddListener(() => ShowOnly(quizDetailsPanel));

        // Quiz Screen Answers (Set up dynamically or indices)
        for (int i = 0; i < answerBtns.Length; i++)
        {
            int idx = i;
            answerBtns[i].onClick.AddListener(() => OnAnswerSelected(idx));
        }

        // Feedback
        nextQuestionBtn.onClick.AddListener(NextQuestion);

        // Results
        resultsBackToMenuBtn.onClick.AddListener(() => ShowOnly(mainMenuPanel));
        viewLeaderboardBtn.onClick.AddListener(ShowLeaderboard);

        // Leaderboard
        backFromLeaderboardBtn.onClick.AddListener(() => {
            if (resultsScreen.activeSelf || selectedQuizData != null)
                ShowOnly(resultsScreen);
            else
                ShowOnly(mainMenuPanel);
        });
    }

    void SetupSlider()
    {
        questionSlider.onValueChanged.AddListener(val => {
            questionCount = (int)val;
            sliderValueText.text = questionCount.ToString();
        });
    }

    void ShowOnly(GameObject target)
    {
        loginPanel.SetActive(target == loginPanel);
        mainMenuPanel.SetActive(target == mainMenuPanel);
        categorySelectPanel.SetActive(target == categorySelectPanel);
        quizDetailsPanel.SetActive(target == quizDetailsPanel);
        studyPanel.SetActive(target == studyPanel);
        quizScreen.SetActive(target == quizScreen);
        feedbackOverlay.SetActive(target == feedbackOverlay);
        resultsScreen.SetActive(target == resultsScreen);
        leaderboardPanel.SetActive(target == leaderboardPanel);
        if (historyPanel != null) historyPanel.SetActive(target == historyPanel);
    }

    // --- LOGIN FLOW ---

    void OnLoginClicked()
    {
        string username = usernameInput.text.Trim();
        if (string.IsNullOrEmpty(username))
        {
            loginWarningText.text = "Username cannot be empty!";
            loginWarningText.gameObject.SetActive(true);
            return;
        }

        ProfileManager.Instance.CreateProfile(username);
        ProfileManager.Instance.LoadProfile();
        UpdateProfileUI();
        ShowOnly(mainMenuPanel);
    }

    void OnLogoutClicked()
    {
        // Delete profile and return to login
        string path = System.IO.Path.Combine(Application.persistentDataPath, "UserProfile.json");
        if (System.IO.File.Exists(path))
            System.IO.File.Delete(path);

        usernameInput.text = "";
        loginWarningText.gameObject.SetActive(false);
        ShowOnly(loginPanel);
    }

    void UpdateProfileUI()
    {
        var profile = ProfileManager.Instance.CurrentProfile;
        if (profile != null)
        {
            welcomeText.text = $"Kumusta, {profile.username}!";
            streakText.text = $"🔥 {profile.streak} Araw (Streak)";
            xpText.text = $"⭐ {profile.totalXp} XP";
        }
    }

    // --- CATEGORY SELECT FLOW ---

    void OnPlayClicked()
    {
        DataManager.Instance.LoadAllFiles();
        ShowOnly(categorySelectPanel);
        BuildCategoryList();
    }

    void BuildCategoryList()
    {
        foreach (Transform child in categoryScrollContent) Destroy(child.gameObject);

        var categories = DataManager.Instance.loadedFiles.Values
            .Select(q => string.IsNullOrEmpty(q.category) ? "General" : q.category)
            .Distinct()
            .ToList();

        if (categories.Count == 0)
        {
            GameObject emptyItem = Instantiate(categoryItemPrefab, categoryScrollContent);
            emptyItem.GetComponentInChildren<TMP_Text>().text = "No quizzes found. Please upload .json files.";
            emptyItem.GetComponent<Button>().interactable = false;
            return;
        }

        foreach (string category in categories)
        {
            GameObject item = Instantiate(categoryItemPrefab, categoryScrollContent);
            item.GetComponentInChildren<TMP_Text>().text = category;
            
            string catName = category; // local copy for closure
            item.GetComponent<Button>().onClick.AddListener(() => OnCategorySelected(catName));
        }
    }

    void OnCategorySelected(string category)
    {
        selectedCategory = category;
        ShowOnly(quizDetailsPanel);
        PopulateQuizDropdown();
    }

    // --- QUIZ DETAILS FLOW ---

    void PopulateQuizDropdown()
    {
        quizDropdown.ClearOptions();
        var quizNames = DataManager.Instance.loadedFiles
            .Where(kv => (string.IsNullOrEmpty(kv.Value.category) ? "General" : kv.Value.category) == selectedCategory)
            .Select(kv => kv.Key)
            .ToList();

        quizDropdown.AddOptions(quizNames);
        
        quizDropdown.onValueChanged.RemoveAllListeners();
        quizDropdown.onValueChanged.AddListener(index => {
            if (index >= 0 && index < quizNames.Count)
                DisplayQuizDetails(quizNames[index]);
        });

        if (quizNames.Count > 0)
        {
            DisplayQuizDetails(quizNames[0]);
        }
    }

    void DisplayQuizDetails(string quizName)
    {
        selectedQuizName = quizName;
        selectedQuizData = DataManager.Instance.loadedFiles[quizName];

        quizTitleText.text = selectedQuizData.title;
        quizDescText.text = $"Subukan ang iyong kaalaman sa {selectedQuizData.title}. Mag-aral gamit ang flashcards at sagutin ang maikling pagsusulit!";
        
        int questionCountPool = selectedQuizData.questions != null ? selectedQuizData.questions.Count : 0;
        int lessonCountPool = selectedQuizData.lessons != null ? selectedQuizData.lessons.Count : 0;
        string difficulty = string.IsNullOrEmpty(selectedQuizData.difficulty) ? "Katamtaman" : selectedQuizData.difficulty;

        quizMetaText.text = $"Category: {selectedCategory}\nHirap: {difficulty}\nMga Tanong: {questionCountPool}\nMga Slide: {lessonCountPool}";

        questionSlider.minValue = 1;
        questionSlider.maxValue = Mathf.Max(1, questionCountPool);
        questionSlider.wholeNumbers = true;
        questionSlider.value = Mathf.Min(5, questionCountPool);
        questionCount = (int)questionSlider.value;
        sliderValueText.text = questionCount.ToString();
    }

    // --- STUDY MODE FLOW ---

    void StartQuizFlow()
    {
        if (selectedQuizData == null) return;

        score = 0;
        currentIndex = 0;
        sessionLog.Clear();
        quizTimer = 0f;

        var pool = new List<QuestionItem>(selectedQuizData.questions);
        Shuffle(pool);
        int count = Mathf.Min(questionCount, pool.Count);
        activeQuestions = pool.Take(count).ToList();

        if (studyModeToggle.isOn && selectedQuizData.lessons != null && selectedQuizData.lessons.Count > 0)
        {
            activeLessons = new List<LessonSlide>(selectedQuizData.lessons);
            currentSlideIndex = 0;
            ShowOnly(studyPanel);
            DisplayLessonSlide();
        }
        else
        {
            StartQuizGameplay();
        }
    }

    void DisplayLessonSlide()
    {
        if (activeLessons == null || activeLessons.Count == 0)
        {
            StartQuizGameplay();
            return;
        }

        LessonSlide slide = activeLessons[currentSlideIndex];
        studyTitleText.text = slide.title;

        string content = slide.content;
        if (slide.bullets != null && slide.bullets.Length > 0)
        {
            content += "\n\n";
            foreach (var bullet in slide.bullets)
                content += $"• {bullet}\n";
        }
        studyContentText.text = content;
        slideProgressText.text = $"Slide {currentSlideIndex + 1} / {activeLessons.Count}";

        if (currentSlideIndex == activeLessons.Count - 1)
            nextSlideBtn.GetComponentInChildren<TMP_Text>().text = "Simulan ang Pagsusulit";
        else
            nextSlideBtn.GetComponentInChildren<TMP_Text>().text = "Susunod";

        prevSlideBtn.interactable = currentSlideIndex > 0;
    }

    void OnNextSlideClicked()
    {
        if (currentSlideIndex < activeLessons.Count - 1)
        {
            currentSlideIndex++;
            DisplayLessonSlide();
        }
        else
        {
            StartQuizGameplay();
        }
    }

    void OnPrevSlideClicked()
    {
        if (currentSlideIndex > 0)
        {
            currentSlideIndex--;
            DisplayLessonSlide();
        }
    }

    // --- QUIZ GAMEPLAY FLOW ---

    void StartQuizGameplay()
    {
        ShowOnly(quizScreen);
        isTimerRunning = true;
        DisplayQuestion();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            quizTimer += Time.deltaTime;
        }

        if (isQuestionTimerRunning)
        {
            questionTimer -= Time.deltaTime;
            timerText.text = $"⏱️ {Mathf.Max(0, Mathf.CeilToInt(questionTimer))}s";
            if (questionTimer <= 0)
            {
                isQuestionTimerRunning = false;
                OnQuestionTimeOut();
            }
        }
    }

    void DisplayQuestion()
    {
        if (currentIndex >= activeQuestions.Count)
        {
            ShowResults();
            return;
        }

        QuestionItem q = activeQuestions[currentIndex];
        progressText.text = $"{currentIndex + 1} / {activeQuestions.Count}";

        // Alternate true/false and MCQ if distractors exist, otherwise force MCQ
        isTrueFalse = (q.distractors != null && q.distractors.Length > 0) && (Random.value >= 0.5f);

        if (!isTrueFalse)
            ShowAsMCQ(q);
        else
            ShowAsTrueFalse(q);

        questionTimer = QUESTION_TIME_LIMIT;
        isQuestionTimerRunning = true;
    }

    void ShowAsMCQ(QuestionItem q)
    {
        questionText.text = q.question;
        currentCorrectAnswer = q.correct_answer;

        var options = new List<string> { q.correct_answer };
        var distractors = q.distractors != null ? new List<string>(q.distractors) : new List<string>();
        Shuffle(distractors);
        options.AddRange(distractors.Take(3));
        Shuffle(options);

        for (int i = 0; i < answerBtns.Length; i++)
        {
            if (i < options.Count)
            {
                answerBtns[i].gameObject.SetActive(true);
                answerBtns[i].GetComponentInChildren<TMP_Text>().text = options[i];
            }
            else
            {
                answerBtns[i].gameObject.SetActive(false);
            }
        }
    }

    void ShowAsTrueFalse(QuestionItem q)
    {
        bool showCorrect = Random.value >= 0.5f;
        string shown = showCorrect ? q.correct_answer : q.distractors[Random.Range(0, q.distractors.Length)];

        questionText.text = $"{q.question}\n\nTama ba ang sagot na ito?\n-> **{shown}**";
        currentCorrectAnswer = showCorrect ? "Tama" : "Mali";

        answerBtns[0].gameObject.SetActive(true);
        answerBtns[0].GetComponentInChildren<TMP_Text>().text = "Tama";
        answerBtns[1].gameObject.SetActive(true);
        answerBtns[1].GetComponentInChildren<TMP_Text>().text = "Mali";
        answerBtns[2].gameObject.SetActive(false);
        answerBtns[3].gameObject.SetActive(false);
    }

    void OnAnswerSelected(int btnIndex)
    {
        isQuestionTimerRunning = false;
        string selected = answerBtns[btnIndex].GetComponentInChildren<TMP_Text>().text;
        
        // Localise evaluation (since T/F answers are in Tagalog "Tama"/"Mali" but correct_answer is English or depends on dynamic evaluation)
        bool correct;
        if (isTrueFalse)
        {
            correct = selected == currentCorrectAnswer;
        }
        else
        {
            correct = selected == currentCorrectAnswer;
        }

        if (correct) score++;
        
        sessionLog.Add((activeQuestions[currentIndex].question,
                        activeQuestions[currentIndex].correct_answer,
                        correct));

        feedbackResultText.text = correct ? "Tama!" : "Mali!";
        feedbackResultText.color = correct ? new Color(0.2f, 0.8f, 0.4f) : new Color(0.9f, 0.3f, 0.3f);
        feedbackAnswerText.text = correct ? "Magaling!" : $"Tamang Sagot: {activeQuestions[currentIndex].correct_answer}";

        ShowOnly(feedbackOverlay);
    }

    void OnQuestionTimeOut()
    {
        sessionLog.Add((activeQuestions[currentIndex].question, activeQuestions[currentIndex].correct_answer, false));
        feedbackResultText.text = "Ubos na ang oras!";
        feedbackResultText.color = new Color(0.9f, 0.5f, 0.1f);
        feedbackAnswerText.text = $"Tamang Sagot: {activeQuestions[currentIndex].correct_answer}";
        ShowOnly(feedbackOverlay);
    }

    void NextQuestion()
    {
        currentIndex++;
        ShowOnly(quizScreen);
        DisplayQuestion();
    }

    // --- RESULTS FLOW ---

    void ShowResults()
    {
        isTimerRunning = false;
        isQuestionTimerRunning = false;
        ShowOnly(resultsScreen);

        scoreText.text = $"{score} / {activeQuestions.Count}";
        
        int xpEarned = score * 10;
        if (score == activeQuestions.Count) xpEarned += 50; // Perfect score bonus
        xpGainedText.text = $"+{xpEarned} XP!";

        // Save progress locally
        ProfileManager.Instance.RecordQuizCompletion(selectedQuizData.title, score, activeQuestions.Count, xpEarned);
        ProfileManager.Instance.AddLeaderboardEntry(selectedQuizData.title, ProfileManager.Instance.CurrentProfile.username, score, activeQuestions.Count, quizTimer);
        
        UpdateProfileUI();

        foreach (Transform child in resultsScrollContent) Destroy(child.gameObject);

        foreach (var entry in sessionLog)
        {
            GameObject go = Instantiate(resultItemPrefab, resultsScrollContent);
            string icon = entry.wasCorrect ? "✔" : "✘";
            Color color = entry.wasCorrect ? new Color(0.1f, 0.6f, 0.2f) : new Color(0.8f, 0.1f, 0.1f);
            
            TMP_Text text = go.GetComponentInChildren<TMP_Text>();
            text.text = $"[{icon}] {entry.question}\n      Sagot: {entry.correct}";
            text.color = color;
        }
    }

    // --- LEADERBOARD FLOW ---

    void OnMainMenuLeaderboardClicked()
    {
        // View leaderboard for first available quiz or general
        if (DataManager.Instance.loadedFileNames.Count > 0)
        {
            selectedQuizName = DataManager.Instance.loadedFileNames[0];
            selectedQuizData = DataManager.Instance.loadedFiles[selectedQuizName];
            ShowLeaderboard();
        }
        else
        {
            ShowOnly(leaderboardPanel);
            leaderboardTitleText.text = "Leaderboard";
            foreach (Transform child in leaderboardScrollContent) Destroy(child.gameObject);
            GameObject go = Instantiate(leaderboardItemPrefab, leaderboardScrollContent);
            go.GetComponentInChildren<TMP_Text>().text = "Upload quizzes first to view leaderboards!";
        }
    }

    void ShowLeaderboard()
    {
        ShowOnly(leaderboardPanel);
        leaderboardTitleText.text = $"{selectedQuizData.title} - Leaderboard";

        foreach (Transform child in leaderboardScrollContent) Destroy(child.gameObject);

        var entries = ProfileManager.Instance.GetLeaderboard(selectedQuizData.title);
        
        if (entries.Count == 0)
        {
            GameObject go = Instantiate(leaderboardItemPrefab, leaderboardScrollContent);
            go.GetComponentInChildren<TMP_Text>().text = "No records yet. Be the first to play!";
            return;
        }

        int rank = 1;
        foreach (var entry in entries)
        {
            GameObject go = Instantiate(leaderboardItemPrefab, leaderboardScrollContent);
            int min = Mathf.FloorToInt(entry.timeTaken / 60);
            int sec = Mathf.FloorToInt(entry.timeTaken % 60);
            string timeStr = $"{min:00}:{sec:00}";

            go.GetComponentInChildren<TMP_Text>().text = 
                $"{rank}. {entry.username} — Puntos: {entry.score}/{entry.totalQuestions} ({timeStr})";
            rank++;
        }
    }

    // --- HISTORY FLOW ---

    void OnMainMenuHistoryClicked()
    {
        ShowHistory();
    }

    void ShowHistory()
    {
        if (historyPanel == null) return;
        ShowOnly(historyPanel);

        foreach (Transform child in historyScrollContent) Destroy(child.gameObject);

        var profile = ProfileManager.Instance.CurrentProfile;
        if (profile == null || profile.quizHistory == null || profile.quizHistory.Count == 0)
        {
            GameObject go = Instantiate(historyItemPrefab, historyScrollContent);
            if (go != null)
            {
                TMP_Text txt = go.GetComponentInChildren<TMP_Text>();
                if (txt != null) txt.text = "Walang nakaraang tala ng pagsusulit.";
            }
            return;
        }

        // Show history in reverse chronological order (newest first)
        for (int i = profile.quizHistory.Count - 1; i >= 0; i--)
        {
            var attempt = profile.quizHistory[i];
            GameObject go = Instantiate(historyItemPrefab, historyScrollContent);
            if (go != null)
            {
                TMP_Text txt = go.GetComponentInChildren<TMP_Text>();
                if (txt != null)
                {
                    txt.text = $"{attempt.datePlayed} — {attempt.quizTitle}\n      Puntos: {attempt.score}/{attempt.totalQuestions}";
                }
            }
        }
    }

    // --- HELPERS ---

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}