using static Serilog.Events.LogEventLevel;
using ClassLibrary;

namespace ServerHost
{
    internal class MathClass
    {
        public static double[,] Solve(double[,] emptyDoubleArray, int[] arrayWithNumbersForСondition, int[] firstBaseArray, double[] secondBaseArray)
        {
            for (int i = 0; i < firstBaseArray.Length; i++)
            {
                if (firstBaseArray[i] == 9)
                {
                    for (int j = 0; j < emptyDoubleArray.GetLength(1); j++)
                    {
                        emptyDoubleArray[i, j] = Math.Sin(Math.Sin(Math.Pow((secondBaseArray[j] / (secondBaseArray[j] + 0.5)), secondBaseArray[j])));
                    }
                }
                else if (arrayWithNumbersForСondition.Contains(firstBaseArray[i]))
                {
                    for (int j = 0; j < emptyDoubleArray.GetLength(1); j++)
                    {
                        double exp = 0.5 / (Math.Tan(2 * secondBaseArray[j]) + (2.0 / 3.0));
                        emptyDoubleArray[i, j] = Math.Pow(exp, Math.Pow(Math.Pow(secondBaseArray[j], 1.0 / 3.0), 1.0 / 3.0));
                    }
                }
                else
                {
                    for (int j = 0; j < emptyDoubleArray.GetLength(1); j++)
                    {
                        emptyDoubleArray[i, j] = Math.Tan(Math.Pow(((Math.Pow(Math.E, 1 - secondBaseArray[j] / Math.PI) / 3.0) / 4.0), 3));
                    }
                }
            }
            return emptyDoubleArray;
        }
        public static double GetAnswer()
        {
            int[] k = Enumerable.Range(5, 15).Where(x2 => x2 % 2 != 0).ToArray();
            double[] x = new double[13];
            Random random = new Random();
            for (int i = 0; i < x.Length; i++)
            {
                x[i] = random.NextDouble(-12, 16);
                Logger.LogByTemplate(Debug, note: $"X array index {i} = {x[i]} ");
            }

            double[,] tempArr2 = new double[8, 13];
            int[] numbers = { 5, 7, 11, 15 };
            tempArr2 = MathClass.Solve(
                emptyDoubleArray: tempArr2,
                arrayWithNumbersForСondition: numbers,
                firstBaseArray: k,
                secondBaseArray: x);
            double[] FirstElement = Enumerable.Range(0, tempArr2.GetLength(1))
                .Select(col => tempArr2[MainFunProgram.Num1 % 8, col])
                .ToArray();

            double[] SecondElemet = Enumerable.Range(0, tempArr2.GetLength(0))
                .Select(row => tempArr2[row, MainFunProgram.Num2 % 13])
                .ToArray();

            double answer = Math.Round((FirstElement.Min() + SecondElemet.Average()), 4);
            Logger.LogByTemplate(Debug, note: $"answer = {answer}");

            if (double.IsNaN(answer))
            {
                GetAnswer();
                Logger.LogByTemplate(Error, note: "Answer is NaN, recursively calling GetAnswer.");
            }
            else
            {
                Logger.LogByTemplate(Information, note: "Answer is valid, returning.");
                return answer;
            }

            Logger.LogByTemplate(Warning, note: "Unexpected flow, should not reach here.");
            Logger.LogByTemplate(Debug, note: "Returning default value due to unexpected flow.");
            return 0;
        }
    }
}
