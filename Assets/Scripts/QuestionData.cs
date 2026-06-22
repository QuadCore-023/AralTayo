using System.Collections.Generic;

[System.Serializable]
public class LessonSlide
{
    public string title;
    public string content;
    public string[] bullets;
}

[System.Serializable]
public class QuestionItem
{
    public string question;
    public string correct_answer;
    public string[] distractors;
}

[System.Serializable]
public class QuizDataFile
{
    public string title;
    public string category;   // e.g. "Math", "Science", "History"
    public string difficulty; // e.g. "Easy", "Medium", "Hard"
    public List<LessonSlide> lessons; // Lesson slides for Study Mode
    public List<QuestionItem> questions;
}