using Serilog;
using static Serilog.Events.LogEventLevel;

namespace MlStartTask2
{
    class Program
    {
        static void Main()
        {
            Logger.CreateLogDirectory(
                Debug,
                Information,
                Warning,
                Error
                );

            Random random = new Random();
            int[] k = Enumerable.Range(5, 15).Where(x => x % 2 != 0).ToArray();
            double[] x = new double[13];

            for (int i = 0; i < x.Length; i++)
            {
                x[i] = random.NextDouble(-12,   16);
                Logger.LogByTemplate(Debug, note: $"X array index {i} = {x[i]} ");
            }

            Logger.LogByTemplate(Information, note: "Application started ");
            double[,] arr2 = new double[8, 13];
            int[] numbers = { 5, 7, 11, 15 };
            for (int i = 0; i < k.Length; i++)
            {
                if (k[i] == 9)
                {
                    for (int j = 0; j < arr2.GetLength(1); j++)
                    {
                        arr2[i, j] = Math.Sin(Math.Sin(Math.Pow((x[j] / (x[j] + 0.5)), x[j])));
                    }
                }
                else if (numbers.Contains(k[i]))
                {
                    for (int j = 0; j < arr2.GetLength(1); j++)
                    {
                        double exp = 0.5 / (Math.Tan(2 * x[j]) + (2.0 / 3.0));
                        arr2[i, j] = Math.Pow(exp, Math.Pow(Math.Pow(x[j], 1.0 / 3.0), 1.0 / 3.0));
                    }
                }
                else
                {
                    for (int j = 0; j < arr2.GetLength(1); j++)
                    {
                        arr2[i, j] = Math.Tan(Math.Pow(((Math.Pow(Math.E, 1 - x[j] / Math.PI) / 3.0) / 4.0), 3));
                    }
                }
            }

            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(currentDirectory,
                "config.txt");
            Logger.LogByTemplate(Debug,
                note: "Checking and configuring file ");
            Logger.LogByTemplate(Information,
                note: $"Config file path: {filePath}");

            if (!File.Exists(filePath))
            {
                Logger.LogByTemplate(Debug,
                    note: "Config file not found, creating with default content ");
                string content = "7 10";
                File.WriteAllText(filePath, content);
            }

            string realContent = File.ReadAllText(filePath);
            int num1, num2;
            try
            {
                num1 = int.Parse(realContent.Split()[0]);
                num2 = int.Parse(realContent.Split()[1]);
                Logger.LogByTemplate(Information,
                    note: $"Parsing successful. num1: {num1}, num2: {num2}");

                double[] FirstElement = Enumerable.Range(0, arr2.GetLength(1))
                                        .Select(col => arr2[num1 % 8, col])
                                        .ToArray();

                double[] SecondElemet = Enumerable.Range(0, arr2.GetLength(0))
                                        .Select(row => arr2[row, num2 % 13])
                                        .ToArray();

                var answer = (Math.Round((FirstElement.Min() + SecondElemet.Average()), 4));
                Logger.LogByTemplate(Debug,
                    note: $"answer = {answer}");

                if (double.IsNaN(answer))
                {
                    Logger.LogByTemplate(Warning,
                        note: $"The calculated result is not a valid number. answer = {answer} Please check your input data.");
                }
                Console.WriteLine(answer);
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(Error,
                    ex
                    ,$"Parsing failed. Invalid format in config file.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
