using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneQuiz
{
    private string quizType;
    private int quizId;
    private int waktu;
    private int waktu_salah;
    [TextArea(3,5)]
    private string question;
    private string questionImageLink;
    private List<string> pilihanPilgan;
    [TextArea(3,5)]
    private string hint;
    private string answer;

    public string QuizType { get => quizType; set => quizType = value; }
    public int QuizId { get => quizId; set => quizId = value; }
    public int Waktu { get => waktu; set => waktu = value; }
    public int Waktu_Salah { get => waktu_salah; set => waktu_salah = value; }
    public string Question { get => question; set => question = value; }
    public string QuestionImageLink { get => questionImageLink; set => questionImageLink = value; }
    public List<string> PilihanPilgan { get => pilihanPilgan; set => pilihanPilgan = value; }
    public string Hint { get => hint; set => hint = value; }
    public string Answer { get => answer; set => answer = value; }
}
