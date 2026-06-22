# AralTayo! App Requirements & Development Status

This document preserves the instructions, features, and status of the project. If you are a new AI assistant starting a new session, read this document to understand the codebase context, requirements, and next steps immediately.

---

## 🎯 Project Objective
Rename the prototype app "StudiKith" to **AralTayo!** and transition from prototype to actual production development.

---

## 🛠️ Requirements & Features to Implement
The user selected the following core features for their thesis:
1.  **Study Mode / Lessons**: Read notes and view flashcards before taking quizzes.
2.  **Timer & Streak Systems**: Gamification elements to motivate daily studying.
3.  **Subject / Category Selection**: Grouping quizzes by topic/subject (e.g. Science, Math, History) instead of a simple flat file list.
4.  **User Login / Profiles**: Track individual progress, XP, and history.
5.  **Local Leaderboards**: Compete with classmates by recording top scores (sorted by score and time taken).

---

## ⚙️ Completed Work & Architecture
We have already refactored the codebase to support these features:
*   **[QuestionData.cs](file:///C:/Users/domen/Unity/Thesis/Assets/Scripts/QuestionData.cs)**: Added `LessonSlide` structure and category/difficulty metadata.
*   **[ProfileManager.cs](file:///C:/Users/domen/Unity/Thesis/Assets/Scripts/ProfileManager.cs)**: Handles persistent user profiles (JSON), daily streak checks, XP increments, top 10 local scores, and quiz attempt history (`QuizAttemptRecord`).
*   **[AralTayoManager.cs](file:///C:/Users/domen/Unity/Thesis/Assets/Scripts/AralTayoManager.cs)**: Central controller containing the full UI workflow with support for 10 panels (Login, Menu, Category, Details, Study, Quiz, Feedback, Results, Leaderboard, History).
*   **[DataManager.cs](file:///C:/Users/domen/Unity/Thesis/Assets/Scripts/DataManager.cs)**: Updated to load lists instead of arrays.
*   **[sample.json](file:///C:/Users/domen/Unity/Thesis/Assets/StreamingAssets/sample.json)**: Updated default quiz file to include category (`Agham`) and two study slides.

---

## 🚀 Next Steps
1.  **Unity Scene Setup for History UI**:
    *   Duplicate the **Leaderboard Panel** in the hierarchy and rename it to **History Panel**.
    *   Inside **History Panel**, rename the header text to "Kasaysayan / History".
    *   Change the back button reference from `backFromLeaderboardBtn` to `backFromHistoryBtn` in the inspector.
    *   Rename the scroll view content holder to `historyScrollContent` and wire it to the manager.
    *   Assign the `ResultItem` or `LeaderboardItem` prefab to `historyItemPrefab`.
    *   On the **Main Menu Panel**, add a new **History Button** (with a clock or list icon) and assign it to `mainMenuHistoryBtn` in the `AralTayoManager` inspector.
2.  **Reusable Navigation Bar Setup (Prefab)**:
    *   **Step 1**: In the **Thesis App** scene, select your navigation buttons: `NavBedroomButton`, `NavDiningButton`, `NavHallway`, and `NavTabletButton`.
    *   **Step 2**: Create a new empty GameObject on your Canvas named `GlobalNavBar` (e.g., as a Panel or standard RectTransform). Add a layout group component (like `HorizontalLayoutGroup`) to arrange the buttons if desired.
    *   **Step 3**: Make the 4 navigation buttons children of this new `GlobalNavBar` object.
    *   **Step 4**: Drag the `GlobalNavBar` object from the hierarchy window into your project folder (e.g., `Assets/Prefabs`) to create a **Prefab**.
    *   **Step 5**: Attach the [GlobalNavigation.cs](file:///C:/Users/domen/Unity/Thesis/Assets/Scripts/GlobalNavigation.cs) script to this prefab.
    *   **Step 6**: In the Inspector for the prefab, assign the four button references:
        *   `Nav Bedroom Button` -> `NavBedroomButton`
        *   `Nav Dining Button` -> `NavDiningButton`
        *   `Nav Hallway Button` -> `NavHallway`
        *   `Nav Tablet Button` -> `NavTabletButton`
    *   **Step 7**: Toggle `Use Scene Navigation` in the Inspector:
        *   **Checked (Scene Mode)**: Loads the corresponding scene name when clicked (make sure the scene names match your project files and are added to the Build Settings!).
        *   **Unchecked (Panel Mode)**: Toggles UI panels directly within the same scene (simply assign the panel GameObjects in the inspector).
    *   **Step 8**: You can now drag the `GlobalNavBar` prefab into any scene or screen Canvas in your project to reuse it!
3.  **Figma Mockups Integration**:
    *   If you have Figma screenshots or layouts, place them in `C:\Users\domen\Unity\Thesis\UI_Designs`.
    *   We will then customize RectTransforms and layout groups to match the exact spacing and aesthetics of your Figma layout.
