using Serilog;
using System.Reflection;
using static Serilog.Events.LogEventLevel;

namespace MlStartTask2
{
    class Program
    {
        static async Task Main()
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
                x[i] = random.NextDouble(-12, 16);
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
                string content = "7 9 5";
                File.WriteAllText(filePath, content);
            }

            string realContent = File.ReadAllText(filePath);
            int num1, num2, delayInSeconds;
            if (!int.TryParse(realContent.Split()[0], out num1))
            {
                Logger.LogByTemplate(Error, note: $"Error while Parsing first param from file");
            }
            if (!int.TryParse(realContent.Split()[1], out num2))
            {
                Logger.LogByTemplate(Error, note: $"Error while Parsing second param from file");
            }
            if (!int.TryParse(realContent.Split()[2], out delayInSeconds))
            {
                Logger.LogByTemplate(Error, note: $"Error while Parsing third param from file");
            }

            try
            {


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
                    , $"Parsing failed. Invalid format in config file.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
            static async Task PrintLinesWithDelay(List<string> lines, int delayMilliseconds)
            {
                foreach (var line in lines)
                {
                    Console.WriteLine(line);
                    await Task.Delay(delayMilliseconds);
                }
            }
            List<string> lines = new List<string>();
            Person Neshnaika = new Person("Незнайка", "Житель солнечного города, Главный герой");
            Person Kozlik = new Person("Козлик", "Досыта хлебнувший жизни лунатик");
            Person Korotishka = new Person("Коротышка", "Житель солнечного города");
            Crowd ThoseWhoWishingToPurchaseShares = new Crowd("Те кто хочет купить акции компании больших растений", "Желающие Приобрести Акции");

            lines.AddRange(Korotishka.PerformAction(new List<Item> { new Item("Деньги") }, Actions.InvestMoney));
            lines.AddRange(Korotishka.BuyShares(new List<Item> { new Item("Акция1"), new Item("Акция2") }));
            lines.Add(Korotishka.PerfomSimplyAction(Actions.Departure));
            lines.Add(ThoseWhoWishingToPurchaseShares.GetState("Cтановилось всё больше и больше"));
            lines.AddRange(Neshnaika.SellShares(new List<Item> { new Item("Акция1"), new Item("Акция2") }));
            lines.AddRange(Kozlik.SellShares(new List<Item> { new Item("Акция1"), new Item("Акция2") }));
            await PrintLinesWithDelay(lines, delayInSeconds);

        }
    }
}
