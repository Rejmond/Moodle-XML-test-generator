using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TestGenerator
{
    class Generator
    {
        public string InputText { get; set; }
        public string Category { get; set; }
        public List<Question> questions { get; set; }
        public XDocument Xml { get; set; }
        
        public bool Parse()
        {
            var lines = InputText.Split(Environment.NewLine.ToCharArray())
                .Select(l => l.Trim())
                .Where(l => l != string.Empty)
                .ToList();
            questions = new List<Question>();
            int questionIndex = 0;
            Question currentQuestion = null;
            Answer currentAnswer = null;
            Category = string.Empty;
            bool isQuestion = false;
            bool inAnswer = false;
            foreach (var line in lines)
            {
                if (StartWithNumber(line))
                {
                    string qName = string.Format("Вопрос №{0}", ++questionIndex);
                    currentQuestion = new Question(qName, RemoveNumber(line));
                    isQuestion = true;
                    inAnswer = false;
                    questions.Add(currentQuestion);
                }
                else if (StartWithLetter(line))
                {
                    currentAnswer = new Answer(RemoveLetter(line), EndWithPlus(line));
                    isQuestion = false;
                    inAnswer = true;
                    currentQuestion.Answers.Add(currentAnswer);
                }
                else
                {
                    if (isQuestion)
                    {
                        currentQuestion.Text.Add(line);
                    }
                    else if (inAnswer)
                    {
                        currentAnswer.Text.Add(line);
                        currentAnswer.Correct = EndWithPlus(line);
                    }
                    else
                    {
                        Category += line;
                    }
                }
            }
            foreach (var question in questions)
            {
                question.UpdateFinalText();
                foreach (var answer in question.Answers)
                {
                    answer.Text[answer.Text.Count - 1] = ClearStringEnd(
                        RemoveEndPlus(answer.Text[answer.Text.Count - 1])
                    );
                    answer.UpdateFinalText();
                }
                question.UpdateGrades();
            }
            return true;
        }
        
        public string RemoveEndPlus(string input)
        {
            input = input.TrimEnd();
            if (input.Last() == '+')
            {
                input = input.Substring(0, input.Length - 1);
            }
            return input;
        }

        public string ClearStringEnd(string input)
        {
            input = input.Trim();
            var chars = new char[]
            {
                '.', ',', ':', ';'
            };
            while (chars.Contains(input.Last()))
            {
                input = input.Substring(0, input.Length - 1);
            }
            return input;
        }

        private bool EndWithPlus(string input)
        {
            return input.Trim().Last() == '+';
        }

        private bool StartWithLetter(string input)
        {
            var chars = input.ToCharArray();
            if (chars.Length == 0) return false;
            if (!IsLetter(chars[0])) return false;
            int i = 1;
            while (i < chars.Length)
            {
                if (IsLetter(chars[i]))
                {
                    i++;
                    continue;
                }
                if (IsSeparator(chars[i]))
                {
                    return true;
                }
                return false;
            }
            return false;

        }
        private string RemoveLetter(string input)
        {
            var chars = input.ToCharArray();
            if (chars.Length == 0) return input;
            if (!IsLetter(chars[0])) return input;
            int i = 1;
            while (i < chars.Length)
            {
                if (IsLetter(chars[i]))
                {
                    i++;
                    continue;
                }
                if (IsSeparator(chars[i]))
                {
                    i++;
                    break;
                }
            }
            return input.Substring(i).Trim();
        }

        private bool StartWithNumber(string input)
        {
            var chars = input.ToCharArray();
            if (chars.Length == 0) return false;
            if (!IsNumber(chars[0])) return false;
            int i = 1;
            while (i < chars.Length)
            {
                if (IsNumber(chars[i]))
                {
                    i++;
                    continue;
                }
                if (IsSeparator(chars[i]))
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        private string RemoveNumber(string input)
        {
            var chars = input.ToCharArray();
            if (chars.Length == 0) return input;
            if (!IsNumber(chars[0])) return input;
            int i = 1;
            while (i < chars.Length)
            {
                if (IsNumber(chars[i]))
                {
                    i++;
                    continue;
                }
                if (IsSeparator(chars[i]))
                {
                    i++;
                    break;
                }
            }
            return input.Substring(i).Trim();
        }

        private bool IsLetter(char input)
        {
            int index = Convert.ToInt32(input);
            if (index >= Convert.ToInt32('a') && index <= Convert.ToInt32('z'))
            {
                return true;
            }
            if (index >= Convert.ToInt32('A') && index <= Convert.ToInt32('Z'))
            {
                return true;
            }
            if (index >= Convert.ToInt32('а') && index <= Convert.ToInt32('я'))
            {
                return true;
            }
            if (index >= Convert.ToInt32('А') && index <= Convert.ToInt32('Я'))
            {
                return true;
            }
            return false;
        }

        private bool IsSeparator(char input)
        {
            var separators = new char[]
            {
                '.', ')', '-', ':'
            };
            return separators.Contains(input);
        }

        private bool IsNumber(char input)
        {
            var numbers = new char[]
            {
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
            };
            return numbers.Contains(input);
        }

        public void GetXML()
        {
            Xml = new XDocument();
            XElement quiz = new XElement("quiz");
            Xml.Add(quiz);
            if (Category != string.Empty)
            {
                XElement categoryEl = new XElement("question",
                    new XAttribute("type", "category"),
                    new XElement("category",
                        new XElement("text",
                            string.Format("$course$/{0}", Category)
                        )
                    )
                );
                quiz.Add(categoryEl);
            }
            foreach (var question in questions)
            {
                XElement questionEl = new XElement("question",
                    new XAttribute("type", "multichoice"));
                // Назавние
                questionEl.Add(new XElement("name",
                    new XElement("text", question.Name)
                ));
                // Текст вопроса
                string questiontext = "";
                List<String> lines = question.FinalText.Split(Environment.NewLine.ToCharArray())
                    .Where(e => e.Trim() != string.Empty).ToList();
                foreach (var line in lines)
                {
                    questiontext += string.Format("<p>{0}</p>", line);
                }
                XCData questiontextCData = new XCData(questiontext);
                questionEl.Add(new XElement("questiontext",
                    new XAttribute("format", "html"),
                    new XElement("text", questiontextCData)
                ));
                // Отзыв к вопросу
                questionEl.Add(new XElement("generalfeedback",
                    new XAttribute("format", "html"),
                    new XElement("text")
                ));
                // Балл по умолчанию
                questionEl.Add(new XElement("defaultgrade", "1.0000000"));
                // Штраф
                questionEl.Add(new XElement("penalty", "0.3333333"));
                // Видимость
                questionEl.Add(new XElement("hidden", 0));
                // Один вариант ответа
                questionEl.Add(new XElement("single", 
                    question.Answers.Count(a => a.Grade > 0) == 1 ? "true" : "false"));
                // Перемешивать ответы
                questionEl.Add(new XElement("shuffleanswers", "true"));
                // Нумерация вопосов
                questionEl.Add(new XElement("answernumbering", "abc"));
                // Отзыв на правильный ответ
                questionEl.Add(new XElement("correctfeedback",
                    new XAttribute("format", "html"),
                    new XElement("text", "Ваш ответ верный.")
                ));
                // Отзыв на частично правильный ответ
                questionEl.Add(new XElement("partiallycorrectfeedback",
                    new XAttribute("format", "html"),
                    new XElement("text", "Ваш ответ частично правильный.")
                ));
                // Отзыв на неправильный ответ
                questionEl.Add(new XElement("incorrectfeedback",
                    new XAttribute("format", "html"),
                    new XElement("text", "Ваш ответ неправильный.")
                ));
                // Показывать номер правильного
                questionEl.Add(new XElement("shownumcorrect"));
                //Ответы
                foreach (var answer in question.Answers)
                {
                    string answerText = "";
                    List<String> answerTextLines = answer.FinalText.Split(Environment.NewLine.ToCharArray())
                        .Where(e => e.Trim() != string.Empty).ToList();
                    foreach (var line in answerTextLines)
                    {
                        answerText += string.Format("<p>{0}</p>", line);
                    }
                    XCData answertextCData = new XCData(answerText);
                    questionEl.Add(new XElement("answer",
                        new XAttribute("fraction", answer.Grade),
                        new XAttribute("format", "html"),
                        new XElement("text", answertextCData),
                        new XElement("feedback",
                            new XAttribute("format", "html"),
                            new XElement("text")
                        )
                    ));
                }

                quiz.Add(questionEl);
            }
        }

    }
}
