using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class QuizGame : MonoBehaviour {

	private string[] Question = 
	{
		"วันที่ 1 มกราคม บอกเวลาแบบใด", "Question2", "Question3", "Question4",
	};

    public CountDown Timer; //อ้างอิงตัวจับเวลา
	public Text _TxtQuiz; //อ้างอิงGameObj 'QuestionQuiz' แสดงคำถาม
	public Text[] _TxtAnswers; //ใช้อ้างอิงGameObj Choice แต่ละปุ่มคำตอบ
	private int _CorrectPosition; //เก็บค่าindaxปุ่มที่แรนดอมว่าจะให้คำตอบที่ถูกอยู่ปุ่มไหน
	private int _IndexQuiz; //ใช้เก็บค่า index ไปหาค่าคำตอบของคำถามนั้นๆ
	private int[] _IndexAnswers; //เก็บคำตอบของแต่ละปุ่ม เช็คคำตอบที่ผิดซ้ำกัน
	//public static GUIStyle radioButton;

	//ShowScore
	private int _ScoreCorrect = 0;
	private int _ScoreWrong = 0;
	public Text _TxtCorrect;
	public Text _TxtWrong;
    public float Time = 60;

	private void RandomAnswers()
	{
		_CorrectPosition = Random.Range (0, _TxtAnswers.Length);
		_TxtAnswers [_CorrectPosition].text = Answers [_IndexQuiz];

		//RandomAnswers
		for(int i = 0; i< _TxtAnswers.Length; i++) //สั่งวนลูป 2 รอบ เพราะ TxtAnswers.Length มีขนาดเท่ากับ 2 i จึงเท่ากับ 2
		{
			if (i != _CorrectPosition) 
			{
				int rand = Random.Range (0, Answers.Length);

				while (rand == _IndexQuiz || CheckDuplicateAnswer(i, rand)) //เช็คกรณีปุ่มแรนดอมคำตอบไปซ้ำกับอินเด็กที่มีคำตอบที่ถูก ให้แรนดอมใหม่
				{
					rand = Random.Range (0, Answers.Length);
				}

				_TxtAnswers [i].text = Answers [rand];
				_IndexAnswers [i] = rand; //randคือคำตอบที่แรนดอมมาได้ เอามาเทียบกับตัวแปร _IndexAnswers

			}
		}
	}

	private bool CheckDuplicateAnswer(int index, int rand)
	{
		for (int i = 0; i < index; i++) 
		{
			if(_IndexAnswers[i] == rand)
			{
				return true;
			}
		}
		return false;
	}

	private void RandomQuiz()
	{
		_IndexAnswers = new int[2];
		_IndexQuiz = Random.Range (0, Question.Length);
		_TxtQuiz.text = Question [_IndexQuiz];
	}

	private string[] Answers = 
	{
		"ทศวรรษ", "คริสต์ศักราช", "ศตวรรษ", "สหัสวรรษ",
	};

	private void Start () {
		_IndexAnswers = new int[2];
		_IndexQuiz = Random.Range (0, Question.Length);
		_TxtQuiz.text = Question[_IndexQuiz];

        Timer.SetTime(Time);
	    Timer.OnTimesUpE += OnTimesUp;
    }

    public void StartQuiz()
    {
        RandomQuiz();
        RandomAnswers();

        Timer.StartTimer();
    }

    private void OnTimesUp()
    {
        //TODO: end quiz
    }

    public void OnClickAnswerButton(int index) //ฟังก์ชันสำหรับเขียนโค้ดทำอะไรต่อเมื่อคลิกปุ่ม
	{
		if (index == _CorrectPosition) 
		{
			_ScoreCorrect += 1;
			_TxtCorrect.text = ": " + _ScoreCorrect;
		} else 
		{
			_ScoreWrong += 1;
			_TxtWrong.text = ": " + _ScoreWrong;
		}

		RandomQuiz ();
		RandomAnswers ();
	}

}
