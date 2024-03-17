using ClassLibrary;
using static Serilog.Events.LogEventLevel;

namespace ServerHost
{
    internal static class RandomExtension
    {
        public static double NextDouble(this Random random, int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue) + random.NextDouble();
        }
    }
    public class MainFunProgram
    {
        #region Properties and Fields
        private static double[,]? arr2;

        public static List<string>? Lines { get; set; }
        public static float DelayInSeconds { get; set; }
        public static int Num1 { get; set; }
        public static int Num2 { get; set; }
        #endregion
        #region CoreMethods
        public static void CoreMain()
        {
            Lines = new List<string>();
            ProcessActions();
        }
        public static void GetNumbersFromSendedArrayOfStrings(string[] arrayOfStrings)
        {
            if (arrayOfStrings.Length < 3)
            {
                Logger.LogByTemplate(Error, note: "Insufficient number of parameters in the array");
                return;
            }

            if (!int.TryParse(arrayOfStrings.ElementAtOrDefault(0), out int num1) ||
                !int.TryParse(arrayOfStrings.ElementAtOrDefault(1), out int num2) ||
                !float.TryParse(arrayOfStrings.ElementAtOrDefault(2), out float delayInSeconds))
            {
                Logger.LogByTemplate(Error, note: "Error while parsing parameters from config file");
                return;
            }
            MainFunProgram.Num1 = num1;
            MainFunProgram.Num2 = num2;
            MainFunProgram.DelayInSeconds = delayInSeconds;
        }
        public static void ProcessActions()
        {
            Person neshnaika = new("Незнайка", "Житель солнечного города, Главный герой");
            Person kozlik = new("Козлик", "Досыта хлебнувший жизни лунатик");
            Person korotishka = new("Коротышка", "Неназванный житель");
            Person miga = new("Мига", "Житель лунного города");

            Bank bank1 = new("Банк1", 1000000, 10);
            Person.BankAccount migaBankAccount1 = new(100, miga);

            Storage unburnedCloset = new("НезгораемыйШкаф");
            Storage unburnedChest0 = new("НезгораемыйСундук1");
            Storage unburnedChest1 = new("НезгораемыйСундук2");

            unburnedChest0.StoreItems(new List<Item> { new("Акция1"), new("Акция2"), new("Акция3") });
            unburnedChest1.StoreItems(new List<Item> { new("Акция4"), new("Акция5"), new("Акция6") });

            Crowd thoseWhoWishingToPurchaseShares = new("Те кто хочет купить акции компании больших растений", "Желающие Приобрести Акции");
            Crowd passersby = new("Коротышки на улице", "Прохожие");
            Crowd population = new("Население города", "Население города");

            Lines.AddRange(korotishka.PerformAction(new List<Item> { new("Деньги") }, Actions.InvestMoney));
            Lines.AddRange(korotishka.BuyShares(new List<Item> { new("Акция1"), new("Акция2") }));
            Lines.Add(korotishka.PerfomSimplyAction(Actions.Departure));
            Lines.Add(thoseWhoWishingToPurchaseShares.GetState("Cтановилось всё больше и больше"));
            Lines.AddRange(neshnaika.SellShares(new List<Item> { new("Акция1"), new("Акция2") }));
            Lines.AddRange(kozlik.SellShares(new List<Item> { new("Акция3"), new("Акция4") }));

            Lines.Add(miga.PerfomSimplyAction(Actions.Move));
            Lines.Add(migaBankAccount1.DisplayBalance());
            Lines.Add(bank1.MoneyExchange(miga));
            Lines.Add(miga.PerformActionToStoreItem(
                sourceItem: new Item("Деньги"),
                action: Actions.InvestMoney,
                storage: unburnedCloset));
            migaBankAccount1.RemoveMoney(migaBankAccount1.Balance);
            Lines.Add(migaBankAccount1.DisplayBalance());

            unburnedCloset.StoreItems(itemsToStore: new List<Item> { new("Деньги"), new("Деньги") });
            Lines.Add(thoseWhoWishingToPurchaseShares.GetState("Толклись на улице, дожидаясь открытия конторы"));
            Lines.Add(passersby.GetState("Заинтересовались происходящим"));
            Lines.Add(population.GetState("Узнало об акциях Общества Гигантских растений"));
            Lines.Add(population.GetState("Спешило накупить акций Общества Гигантских растений для выгодной перепродажи"));
            unburnedChest0.RetrieveItems(unburnedChest0.items.Count);
            unburnedChest1.RetrieveItems(unburnedChest1.items.Count);

            int[] k = Enumerable.Range(5, 15).Where(x2 => x2 % 2 != 0).ToArray();
            double[] x = new double[13];
            Random random = new Random();
            for (int i = 0; i < x.Length; i++)
            {
                x[i] = random.NextDouble(-12, 16);
                Logger.LogByTemplate(Debug, note: $"X array index {i} = {x[i]} ");
            }

            arr2 = new double[8, 13];
            int[] numbers = { 5, 7, 11, 15 };
            arr2 = MathClass.Solve(
                emptyDoubleArray: arr2,
                arrayWithNumbersForСondition: numbers,
                firstBaseArray: k,
                secondBaseArray: x);

            try
            {
                Logger.LogByTemplate(Information, note: "Parsing successful.");
                double[] FirstElement = Enumerable.Range(0, arr2.GetLength(1))
                    .Select(col => arr2[Num1 % 8, col])
                    .ToArray();

                double[] SecondElemet = Enumerable.Range(0, arr2.GetLength(0))
                    .Select(row => arr2[row, Num2 % 13])
                    .ToArray();

                double answer = Math.Round((FirstElement.Min() + SecondElemet.Average()), 4);

                Logger.LogByTemplate(Debug, note: $"answer = {answer}");

                if (double.IsNaN(answer))
                {
                    Logger.LogByTemplate(Warning, note: $"The calculated result is not a valid number. answer = {answer}. Please check your input data.");
                    throw new Exception("Failed to generate number. try again");
                }

                AddNarrativeline(answer);
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(Error, ex, $"Parsing failed. Invalid format in config file.");
            }
        }
        public static void AddNarrativeline(double number)
        {
            Lines.Add($"Все акции общества были распроданы со средней стоимостью {Math.Abs(number)}");
        }
        #endregion
    }
}
