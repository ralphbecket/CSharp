using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace MathsPractice
{
    public static class MathsPractice
    {
        public static void Main(string[] args)
        {
            var questionSchemata = new[] {
                new QuestionSchema(
                    "Simple single-digit addition",
                    "Adding small numbers.",
                    GenerateSmallSumQuestion
                ),
                new QuestionSchema(
                    "Simple two-digit addition",
                    "Adding medium sized numbers.",
                    GenerateMedSumQuestion
                ),
                new QuestionSchema(
                    "Long addition",
                    "Adding larger numbers.",
                    GenerateBigSumQuestion
                ),
                new QuestionSchema(
                    "Simple single-digit subtraction",
                    "Subtracting small numbers.",
                    GenerateSmallDiffQuestion
                ),
                new QuestionSchema(
                    "Simple two-digit subtraction",
                    "Subtracting medium sized numbers.",
                    GenerateMedDiffQuestion
                ),
                new QuestionSchema(
                    "Long subtraction",
                    "Subtracting larger numbers.",
                    GenerateBigDiffQuestion
                ),
                new QuestionSchema(
                    "Simple single-digit multiplication",
                    "Multiplying single-digit numbers.",
                    GenerateSmallMulQuestion
                ),
                new QuestionSchema(
                    "Simple two-digit multiplication",
                    "Multiplying medium sized numbers.",
                    GenerateBigMulQuestion
                ),
                new QuestionSchema(
                    "Long multiplication",
                    "Multiplying larger numbers.",
                    GenerateBigMulQuestion
                ),
                new QuestionSchema(
                    "Simple small-number division",
                    "Dividing small numbers.",
                    GenerateDivQuestion
                ),
                new QuestionSchema(
                    "Long division",
                    "Dividing larger numbers.",
                    GenerateLongDivQuestion
                ),
            };
            var numSchemata = questionSchemata.Length;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Choose a problem set:");
                for (var i = 0; i < numSchemata; i++)
                {
                    Console.WriteLine($"{i + 1} - {questionSchemata[i].Description}");
                }
                while (true)
                {
                    Console.Write(">  ");
                    var response = Console.ReadLine().Trim();
                    var schemaNumber = -1;
                    if (!int.TryParse(response, out schemaNumber) || schemaNumber < 1 || numSchemata < schemaNumber)
                    {
                        Console.WriteLine($"*  Please type in a number from 1 to {numSchemata}.");
                        continue;
                    }
                    var questionSchema = questionSchemata[schemaNumber - 1];
                    var tag = questionSchema.Tag;
                    var description = questionSchema.Description;
                    var questionGenerator = questionSchema.QuestionGenerator;
                    ConductTestViaConsole(tag, description, questionGenerator);
                    break;
                }
            }
        }

        internal static Exception Error(string msg) => new ApplicationException(msg);

        internal enum Op { Add, Sub, Mul, Div, LongDiv }

        internal class Question
        {
            internal Question(int x, Op op, int y)
            {
                X = x;
                Op = op;
                Y = y;
            }
            internal int X;
            internal Op Op;
            internal int Y;
            internal int Result =>
                Op switch
                {
                    Op.Add => X + Y,
                    Op.Sub => X - Y,
                    Op.Mul => X * Y,
                    Op.Div => X / Y,
                    Op.LongDiv => Y / X,
                    _ => throw Error($"Unimplemented operator: {Op}")
                };
            internal int Remainder =>
                Op switch
                {
                    Op.Div => X % Y,
                    Op.LongDiv => Y % X,
                    _ => 0
                };
        }

        internal class Answer
        {
            internal Answer(Question question)
            {
                Question = question;
            }
            internal Question Question;
            internal bool HasAnswer;
            internal int _Result;
            internal int _Remainder;
            internal int Result
            {
                get => _Result;
                set { _Result = value; HasAnswer = true; UpdateTimeTaken(); }
            }
            internal int Remainder
            {
                get => _Remainder;
                set { _Remainder = value; HasAnswer = true; UpdateTimeTaken(); }
            }
            internal bool IsCorrect =>
                HasAnswer &&
                Result == Question.Result &&
                Remainder == Question.Remainder;
            internal DateTime StartTime;
            internal TimeSpan TimeTaken;
            internal void StartAnswering()
            {
                StartTime = DateTime.Now;
            }
            internal void UpdateTimeTaken()
            {
                TimeTaken += DateTime.Now - StartTime;
                StartTime = DateTime.Now;
            }
            internal void StopAnswering()
            {
                TimeTaken += DateTime.Now - StartTime;
            }
            internal void StopAnsweringWithoutExtendingTimeTaken()
            {
            }
        }

        internal class QuestionSet
        {
            internal QuestionSet(string tag, string description, IEnumerable<Question>? questions = null)
            {
                Tag = tag;
                Description = description;
                if (questions != null) Questions.AddRange(questions);
            }
            internal QuestionSet(string tag, string description, int numQuestions, Func<Random, Question> questionGenerator, Random? rnd = null)
            {
                Tag = tag;
                Description = description;
                rnd = rnd ?? new Random();
                Questions.AddRange(Enumerable.Range(1, numQuestions).Select(_ => questionGenerator(rnd)));
            }
            internal string Tag;
            internal string Description;
            internal List<Question> Questions = new List<Question> { };
        }

        internal class AnswerSet
        {
            internal AnswerSet(QuestionSet questionSet)
            {
                Answers.AddRange(questionSet.Questions.Select(x => new Answer(x)));
            }
            internal List<Answer> Answers = new List<Answer> { };
        }

        internal class QuestionSchema
        {
            internal QuestionSchema(string tag, string description, Func<Random, Question> questionGenerator)
            {
                Tag = tag;
                Description = description;
                QuestionGenerator = questionGenerator;
            }
            internal string Tag;
            internal string Description;
            internal Func<Random, Question> QuestionGenerator;
        }

        internal static int RndNat(Random rnd, int ub, int lb = 1, Func<int, bool>? isOk = null) =>
            Enumerable
            .Range(0, int.MaxValue)
            .Select(_ => rnd.Next(lb, ub + 1))
            .Where(x => isOk == null || isOk(x))
            .First();

        internal static Question GenerateSmallSumQuestion(Random rnd) =>
            new Question(RndNat(rnd, lb: 2, ub: 9), Op.Add, RndNat(rnd, lb: 2, ub: 9));

        internal static Question GenerateMedSumQuestion(Random rnd) =>
            new Question(RndNat(rnd, lb: 2, ub: 9), Op.Add, RndNat(rnd, lb: 11, ub: 19));

        internal static Question GenerateBigSumQuestion(Random rnd) =>
            new Question(RndNat(rnd, lb: 10, ub: 999), Op.Add, RndNat(rnd, lb: 10, ub: 999));

        internal static Question GenerateSmallDiffQuestion(Random rnd)
        {
            var y = RndNat(rnd, lb: 1, ub: 9);
            var x = RndNat(rnd, lb: y, ub: 10);
            return new Question(x, Op.Sub, y);
        }

        internal static Question GenerateMedDiffQuestion(Random rnd)
        {
            var y = RndNat(rnd, lb: 1, ub: 19);
            var x = RndNat(rnd, lb: y + 1, ub: 100);
            return new Question(x, Op.Sub, y);
        }

        internal static Question GenerateBigDiffQuestion(Random rnd)
        {
            var y = RndNat(rnd, lb: 21, ub: 100);
            var x = RndNat(rnd, lb: y + 1, ub: 1000);
            return new Question(x, Op.Sub, y);
        }

        internal static Question GenerateSmallMulQuestion(Random rnd) =>
            new Question(RndNat(rnd, lb: 2, ub: 12), Op.Mul, RndNat(rnd, lb: 2, ub: 12));

        internal static Question GenerateMedMulQuestion(Random rnd) =>
            new Question(RndNat(rnd, lb: 2, ub: 9), Op.Mul, RndNat(rnd, lb: 11, ub: 19));

        internal static Question GenerateBigMulQuestion(Random rnd) =>
            new Question(RndNat(rnd, lb: 10, ub: 999), Op.Mul, RndNat(rnd, lb: 10, ub: 999));

        internal static Question GenerateDivQuestion(Random rnd)
        {
            var y = RndNat(rnd, lb: 2, ub: 10);
            var x = RndNat(rnd, lb: 2, ub: 12);
            return new Question(x * y, Op.Div, y);
        }

        internal static Question GenerateLongDivQuestion(Random rnd)
        {
            var x = RndNat(rnd, lb: 3, ub: 10);
            var y = RndNat(rnd, lb: 100, ub: 1000);
            return new Question(x, Op.LongDiv, y);
        }

        internal static void ConductTestViaConsole(string tag, string description, Func<Random, Question> questionGenerator, int numQuestions = 20, Random? rnd = null)
        {
            var questionSet = new QuestionSet(tag, description, numQuestions, questionGenerator, rnd);
            var answerSet = new AnswerSet(questionSet);
            var answers = answerSet.Answers;
            var questionNum = 1;
            void WriteQuestionAndAnswer(Answer answer)
            {
                var question = answer.Question;
                var op = question.Op;
                var x = question.X.ToString();
                var y = question.Y.ToString();
                var answerStr = (!answer.HasAnswer ? "" : answer.Remainder == 0 ? $"{answer.Result}" : $"{answer.Result}r{answer.Remainder}");
                switch (op)
                {
                    case Op.Add:
                        Console.WriteLine($"{x,16}");
                        Console.WriteLine($"{("+  " + y),16}");
                        Console.WriteLine($"{"--------",16}");
                        Console.WriteLine($"{answerStr,16}");
                        break;
                    case Op.Sub:
                        Console.WriteLine($"{x,16}");
                        Console.WriteLine($"{("-  " + y),16}");
                        Console.WriteLine($"{"--------",16}");
                        Console.WriteLine($"{answerStr,16}");
                        break;
                    case Op.Mul:
                        Console.WriteLine($"{x,16}");
                        Console.WriteLine($"{("x  " + y),16}");
                        Console.WriteLine($"{"--------",16}");
                        Console.WriteLine($"{answerStr,16}");
                        break;
                    case Op.Div:
                        var divStr = $"{x} / {y}";
                        Console.WriteLine($"{divStr,16}  =  {answerStr}");
                        break;
                    case Op.LongDiv:
                        var ir = answerStr.IndexOf('r');
                        var adj = (ir == -1 ? 0 : 2);
                        while (answerStr.Length - adj < y.Length) answerStr = " " + answerStr;
                        Console.WriteLine($"{"",8}   {answerStr}");
                        Console.WriteLine($"{"",8} _______");
                        Console.WriteLine($"{x,8} ) {y}");
                        break;
                    default:
                        throw Error($"Unimplemented op: {op}");
                }
            }
            var startTime = DateTime.Now;
            var timeSoFar = DateTime.Now - startTime;
            var numAnswered = 0;
            while (true)
            {
                numAnswered = answers.Where(x => x.HasAnswer).Count();
                timeSoFar = DateTime.Now - startTime;
                var answer = answers[questionNum - 1];
                Console.Clear();
                Console.WriteLine($"You have answered {numAnswered} so far and taken {timeSoFar.TotalSeconds:#} seconds.");
                if (numAnswered == numQuestions)
                {
                    Console.WriteLine($"- Type 'q' to finish.");
                    Console.WriteLine($"- Type 'n' to see the next answer.");
                    Console.WriteLine($"- Type 'p' to see the previous answer.)");
                }
                if (answer.Question.Op == Op.Div || answer.Question.Op == Op.LongDiv)
                {
                    Console.WriteLine("Answer a division question with a remainder as, e.g., 7r2.");
                }
                Console.WriteLine($"\nQuestion {questionNum} of {numQuestions}");
                Console.WriteLine();
                WriteQuestionAndAnswer(answer);
                Console.WriteLine();
                answer.StartAnswering();
                Console.Write("\n>  ");
                var response = Console.ReadLine().Trim().ToLower();
                var quit = false;
                switch (response)
                {
                    case "q":
                        quit = true;
                       break;
                    case "n":
                        if (answer.HasAnswer) answer.StopAnsweringWithoutExtendingTimeTaken(); else answer.StopAnswering();
                        if (numQuestions < ++questionNum) questionNum = 1;
                        continue;
                    case "p":
                        if (answer.HasAnswer) answer.StopAnsweringWithoutExtendingTimeTaken(); else answer.StopAnswering();
                        if (--questionNum < 1) questionNum = numQuestions;
                        continue;
                    default:
                        var ir = response.IndexOf('r');
                        var result = 0;
                        var remainder = 0;
                        var isOk = true;
                        if (ir == -1)
                        {
                            isOk &= int.TryParse(response, out result);
                        }
                        else
                        {
                            isOk &= int.TryParse(response.Substring(0, ir).Trim(), out result);
                            isOk &= int.TryParse(response.Substring(ir + 1).Trim(), out remainder);
                        }
                        if (isOk)
                        {
                            answer.Result = result;
                            answer.Remainder = remainder;
                            if (numQuestions < ++questionNum) questionNum = 1;
                            continue;
                        }
                        Console.WriteLine("*  I'm sorry, I couldn't understand that.");
                        break;
                }
                if (quit) break;
            }
            Console.Clear();
            Console.WriteLine($"You have answered {numAnswered} of {numQuestions} in {timeSoFar.TotalSeconds:#} seconds.");
            var incorrectAnswers = answers.Where(x => !x.IsCorrect).ToArray();
            var numIncorrect = incorrectAnswers.Length;
            if (numIncorrect == 0)
            {
                Console.WriteLine($"\nYou got all of them correct -- well done!");
                Console.WriteLine($"\n[Hit Enter to continue.]");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine($"\nUnfortunately you got {numIncorrect} of {numQuestions} wrong.");
                Console.WriteLine($"Here are the ones you need to look at again:");
                foreach (var answer in incorrectAnswers)
                {
                    Console.WriteLine();
                    WriteQuestionAndAnswer(answer);
                    Console.WriteLine($"\n[Hit Enter to continue.]");
                    Console.ReadLine();
                }
            }
        }
    }
}
