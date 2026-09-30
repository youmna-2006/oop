using System.Diagnostics;
using static oop.PracticalExam;

namespace oop
{
    internal class Program
    {
        static void Main(string[] args)
        {
           
            Console.WriteLine("                   E X A M   S T U D I O                ");
            Console.WriteLine("                    OOP Examination System              ");
            
            Console.WriteLine("Build an exam, then take it from the same console.\n");

            Console.Write("> Subject ID: ");
            int subId;
            int.TryParse(Console.ReadLine(), out subId);

            Console.Write("> Subject name: ");
            string subName = Console.ReadLine();

            Subject sub = new Subject(subId, subName);
            sub.CreateExam();

            Console.WriteLine("Press Enter to start the exam...");
            Console.ReadLine();

            sub.SubjectExam.ShowExam();
        }
    }
    public class Answer : ICloneable
    {
        public int AnswerId { get; set; }
        public string AnswerText { get; set; }

        public Answer(int id, string text)
        {
            AnswerId = id;
            AnswerText = text;
        }

        public object Clone()
        {
            return new Answer(this.AnswerId, this.AnswerText);
        }

        public override string ToString()
        {
            return $"{AnswerId}. {AnswerText}";
        }
    }
    public abstract class Question : IComparable<Question>, ICloneable
    {
        public string Header { get; set; }
        public string Body { get; set; }
        public int Mark { get; set; }
        public Answer[] AnswerList { get; set; }
        public Answer RightAnswer { get; set; }
        public Answer UserAnswer { get; set; }

        public Question()
        {
            Header = "Question";
        }

        public Question(string header, string body, int mark) : this()
        {
            Header = header;
            Body = body;
            Mark = mark;
        }

        public abstract void DisplayQuestion();

        public int CompareTo(Question other)
        {
            if (other == null) return 1;
            return this.Mark.CompareTo(other.Mark);
        }

        public abstract object Clone();

        public override string ToString()
        {
            return $"[{Header}] - {Body} (Mark: {Mark})";
        }
    }
    public class TrueFalseQuestion : Question
    {
        public TrueFalseQuestion(string header, string body, int mark, Answer rightAnswer)
            : base(header, body, mark)
        {
            AnswerList = new Answer[]
            {
                new Answer(1, "True"),
                new Answer(2, "False")
            };
            RightAnswer = rightAnswer;
        }

        public override void DisplayQuestion()
        {
            Console.WriteLine($"{Header} [True / False • {Mark} marks]");
            Console.WriteLine(Body);
            foreach (var ans in AnswerList)
            {
                Console.WriteLine($"> {ans}");
            }
        }

        public override object Clone()
        {
            return new TrueFalseQuestion(this.Header, this.Body, this.Mark, (Answer)this.RightAnswer.Clone());
        }
    }
    public class McqQuestion : Question
    {
        public McqQuestion(string header, string body, int mark, Answer[] answerList, Answer rightAnswer)
            : base(header, body, mark)
        {
            AnswerList = answerList;
            RightAnswer = rightAnswer;
        }

        public override void DisplayQuestion()
        {
            Console.WriteLine($"{Header} [MCQ • {Mark} marks]");
            Console.WriteLine(Body);
            foreach (var ans in AnswerList)
            {
                Console.WriteLine($"> {ans}");
            }
        }

        public override object Clone()
        {
            Answer[] clonedAnswers = new Answer[AnswerList.Length];
            for (int i = 0; i < AnswerList.Length; i++)
            {
                clonedAnswers[i] = (Answer)AnswerList[i].Clone();
            }
            return new McqQuestion(this.Header, this.Body, this.Mark, clonedAnswers, (Answer)this.RightAnswer.Clone());
        }
    }
    public abstract class Exam
    {
        public int Time { get; set; }
        public int NumberOfQuestions { get; set; }
        public Question[] Questions { get; set; }
        public string SubjectName { get; set; }

        public Exam(int time, int numberOfQuestions, string subjectName)
        {
            Time = time;
            NumberOfQuestions = numberOfQuestions;
            Questions = new Question[numberOfQuestions];
            SubjectName = subjectName;
        }

        public abstract void ShowExam();
    }
    public class PracticalExam : Exam
    {
        public PracticalExam(int time, int numberOfQuestions, string subjectName)
            : base(time, numberOfQuestions, subjectName) { }

        public override void ShowExam()
        {
            Console.Clear();
            int totalMarks = 0;
            foreach (var q in Questions) totalMarks += q.Mark;

            Console.WriteLine($"─ PRACTICAL EXAM • {SubjectName}───────────────────────────────────");
            Console.WriteLine($" {NumberOfQuestions} questions | {totalMarks} marks | {Time} minutes\n");

            Stopwatch timer = new Stopwatch();
            timer.Start();

            for (int i = 0; i < Questions.Length; i++)
            {
                Console.WriteLine($"[====================] Question {i + 1}/{Questions.Length}");
                Questions[i].DisplayQuestion();

                int userChoice;
                do
                {
                    Console.Write("> Your answer: ");
                } while (!int.TryParse(Console.ReadLine(), out userChoice) || userChoice < 1 || userChoice > Questions[i].AnswerList.Length);

                Questions[i].UserAnswer = Questions[i].AnswerList[userChoice - 1];
                Console.WriteLine();
            }

            timer.Stop();

            int grade = 0;
            Console.WriteLine("─ PRACTICAL EXAM • CORRECT ANSWERS────────────────────────────────");
            for (int i = 0; i < Questions.Length; i++)
            {
                bool isCorrect = Questions[i].UserAnswer.AnswerId == Questions[i].RightAnswer.AnswerId;
                string markSymbol = isCorrect ? "✓" : "X";
                Console.ForegroundColor = isCorrect ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine($"{markSymbol} {Questions[i].Header}: {Questions[i].Body}");
                Console.ResetColor();

                Console.WriteLine($"   Your answer: {Questions[i].UserAnswer.AnswerText}");
                Console.WriteLine($"   Correct answer: {Questions[i].RightAnswer.AnswerText}\n");

                if (isCorrect) grade += Questions[i].Mark;
            }

            double percentage = ((double)grade / totalMarks) * 100;
            string status = percentage >= 50 ? "PASS" : "KEEP PRACTISING";

            Console.ForegroundColor = ConsoleColor.Yellow;
            
            Console.WriteLine($"│ SCORE  {grade}/{totalMarks}  •  {percentage:F1}%  •  {status}".PadRight(58) + "│");
            
            Console.ResetColor();

            Console.WriteLine($"Completed in {timer.Elapsed:mm\\:ss}.\n");
        }

        public class FinalExam : Exam
        {
            public FinalExam(int time, int numberOfQuestions, string subjectName)
                : base(time, numberOfQuestions, subjectName) { }

            public override void ShowExam()
            {
                Console.Clear();
                int totalMarks = 0;
                foreach (var q in Questions) totalMarks += q.Mark;

                Console.WriteLine($"─ FINAL EXAM • {SubjectName}───────────────────────────────────────");
                Console.WriteLine($" {NumberOfQuestions} questions | {totalMarks} marks | {Time} minutes\n");

                Stopwatch timer = new Stopwatch();
                timer.Start();

                for (int i = 0; i < Questions.Length; i++)
                {
                    Console.WriteLine($"[====================] Question {i + 1}/{Questions.Length}");
                    Questions[i].DisplayQuestion();

                    int userChoice;
                    do
                    {
                        Console.Write("> Your answer: ");
                    } while (!int.TryParse(Console.ReadLine(), out userChoice) || userChoice < 1 || userChoice > Questions[i].AnswerList.Length);

                    Questions[i].UserAnswer = Questions[i].AnswerList[userChoice - 1];
                    Console.WriteLine();
                }

                timer.Stop();

                int grade = 0;
                Console.WriteLine("┌─ FINAL EXAM RESULT───────────────────────────────────────────────┐");
                for (int i = 0; i < Questions.Length; i++)
                {
                    bool isCorrect = Questions[i].UserAnswer.AnswerId == Questions[i].RightAnswer.AnswerId;
                    Console.ForegroundColor = isCorrect ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.WriteLine($"Q{i + 1}: {Questions[i].Body}");
                    Console.ResetColor();

                    Console.WriteLine($"Your answer: {Questions[i].UserAnswer.AnswerText}");
                    Console.WriteLine($"Mark: {(isCorrect ? Questions[i].Mark : 0)}/{Questions[i].Mark}\n");

                    if (isCorrect) grade += Questions[i].Mark;
                }

                double percentage = ((double)grade / totalMarks) * 100;
                string status = percentage >= 50 ? "PASS" : "FAIL";

                Console.ForegroundColor = ConsoleColor.Green;
                
                Console.WriteLine($"│ SCORE  {grade}/{totalMarks}  •  {percentage:F1}%  •  {status}".PadRight(58) + "│");
                
                Console.ResetColor();

                Console.WriteLine($"Completed in {timer.Elapsed:mm\\:ss}.\n");
            }
        }

        public class Subject
        {
            public int SubjectId { get; set; }
            public string SubjectName { get; set; }
            public Exam SubjectExam { get; set; }

            public Subject(int id, string name)
            {
                SubjectId = id;
                SubjectName = name;
            }

            public void CreateExam()
            {
                int examType, time, numQuestions;

                Console.WriteLine("─ EXAM SETUP─────────────────────────────────────────────────────");
                Console.WriteLine(" 1. Final exam     (MCQ + True/False)");
                Console.WriteLine(" 2. Practical exam (MCQ only)\n");

                do
                {
                    Console.Write("> Exam type: ");
                } while (!int.TryParse(Console.ReadLine(), out examType) || (examType != 1 && examType != 2));

                do
                {
                    Console.Write("> Duration in minutes: ");
                } while (!int.TryParse(Console.ReadLine(), out time) || time <= 0);

                do
                {
                    Console.Write("> Number of questions: ");
                } while (!int.TryParse(Console.ReadLine(), out numQuestions) || numQuestions <= 0);

                if (examType == 1)
                    SubjectExam = new FinalExam(time, numQuestions, SubjectName);
                else
                    SubjectExam = new PracticalExam(time, numQuestions, SubjectName);

                Console.WriteLine();

                for (int i = 0; i < numQuestions; i++)
                {
                    if (examType == 1) 
                    {
                        Console.WriteLine($"─ CREATE QUESTION {i + 1} OF {numQuestions}────────────────────────────────────");
                        Console.WriteLine(" 1. MCQ");
                        Console.WriteLine(" 2. True / False\n");

                        int qType;
                        do
                        {
                            Console.Write("> Question type: ");
                        } while (!int.TryParse(Console.ReadLine(), out qType) || (qType != 1 && qType != 2));

                        if (qType == 1)
                            SubjectExam.Questions[i] = CreateMcqQuestion(i + 1);
                        else
                            SubjectExam.Questions[i] = CreateTrueFalseQuestion(i + 1);
                    }
                    else 
                    {
                        Console.WriteLine($"┌─ CREATE QUESTION {i + 1} OF {numQuestions}────────────────────────────────────┐");
                        SubjectExam.Questions[i] = CreateMcqQuestion(i + 1);
                    }
                    Console.WriteLine();
                }

                Console.ForegroundColor = ConsoleColor.Green;
                string examTypeName = examType == 1 ? "Final exam" : "Practical exam";
                Console.WriteLine($"✓ {examTypeName} created for {SubjectName}.");
                Console.ResetColor();
            }

            private TrueFalseQuestion CreateTrueFalseQuestion(int qNum)
            {
                Console.Write("> Header: ");
                string header = Console.ReadLine();

                Console.Write("> Question: ");
                string body = Console.ReadLine();

                int mark = ReadValidInt("> Mark: ", "× Enter a whole number from 1 to 1000.", 1, 1000);

                Console.WriteLine(" 1. True");
                Console.WriteLine(" 2. False");

                int rightId = ReadValidInt("> Correct answer: ", "× Enter 1 or 2.", 1, 2);

                string rightText = rightId == 1 ? "True" : "False";
                return new TrueFalseQuestion(header, body, mark, new Answer(rightId, rightText));
            }

            private McqQuestion CreateMcqQuestion(int qNum)
            {
                Console.Write("> Header: ");
                string header = Console.ReadLine();

                Console.Write("> Question: ");
                string body = Console.ReadLine();

                int mark = ReadValidInt("> Mark: ", "× Enter a whole number from 1 to 1000.", 1, 1000);

                int numChoices = ReadValidInt("> Number of choices: ", "× Enter a whole number from 2 to 10.", 2, 10);

                Answer[] choices = new Answer[numChoices];
                for (int i = 0; i < numChoices; i++)
                {
                    Console.Write($"> Choice {i + 1}: ");
                    string text = Console.ReadLine();
                    choices[i] = new Answer(i + 1, text);
                }

                int rightId = ReadValidInt("> Correct choice number: ", $"× Enter a choice number from 1 to {numChoices}.", 1, numChoices);

                return new McqQuestion(header, body, mark, choices, choices[rightId - 1]);
            }

            
            private int ReadValidInt(string prompt, string errorMessage, int min, int max)
            {
                int value;
                while (true)
                {
                    Console.Write(prompt);
                    string input = Console.ReadLine();
                    if (int.TryParse(input, out value) && value >= min && value <= max)
                    {
                        return value;
                    }
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(errorMessage);
                    Console.ResetColor();
                }
            }
        }

    }
}
