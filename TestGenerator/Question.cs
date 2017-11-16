using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGenerator
{

    public class Question
    {
        public string Name { get; set; }
        public List<string> Text { get; set; }
        public List<Answer> Answers { get; set; }
        public string FinalText { get; set; }
        
        public Question(string name, string text)
        {
            Name = name;
            Text = new List<string>();
            Text.Add(text);
            Answers = new List<Answer>();
        }

        public void UpdateFinalText()
        {
            FinalText = string.Empty;
            for (int i = 0; i < Text.Count; i++)
            {
                FinalText += Text[i];
                if (i != Text.Count - 1)
                {
                    FinalText += Environment.NewLine;
                }
            }
        }

        public void UpdateGrades()
        {
            int correctCount = Answers.Count(a => a.Correct);
            if (correctCount == 1)
            {
                foreach (var answer in Answers)
                {
                    answer.Grade = answer.Correct ? 100 : 0;
                }
            }
            else
            {
                double grade = 100;
                switch (correctCount)
                {
                    case 2:
                        grade = 50;
                        break;
                    case 3:
                        grade = 33.33333;
                        break;
                    case 4:
                        grade = 25;
                        break;
                    case 5:
                        grade = 20;
                        break;
                }
                foreach (var answer in Answers)
                {
                    answer.Grade = answer.Correct ? grade : -100;
                }
            }
        }
    }

    public class Answer
    {
        public List<string> Text { get; set; }
        public bool Correct { get; set; }
        public string FinalText { get; set; }
        public double Grade { get; set; }

        public Answer(string text, bool correct)
        {
            Text = new List<string>();
            Text.Add(text);
            Correct = correct; ;
        }

        public void UpdateFinalText()
        {
            FinalText = string.Empty;
            for (int i = 0; i < Text.Count; i++)
            {
                FinalText += Text[i];
                if (i != Text.Count - 1)
                {
                    FinalText += Environment.NewLine;
                }
            }
        }
                
    }
}
