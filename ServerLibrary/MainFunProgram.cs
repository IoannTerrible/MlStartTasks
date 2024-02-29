using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Serilog.Events.LogEventLevel;

namespace ServerLibrary
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
        public static List<string> lines { get; set; }
        public static float delayInSeconds { get; set; }
        public static int num1 { get; set; }
        public static int num2 { get; set; }

        public static void Main(string[] args)
        {
            lines = new List<string>();
            Logger.CreateLogDirectory(
                Debug,
                Information,
                Warning,
                Error
            );
            Random random = new Random();
            int[] k = Enumerable.Range(5, 15).Where(x2 => x2 % 2 != 0).ToArray();
            double[] x = new double[13];

            for (int i = 0; i < x.Length; i++)
            {
                //Or forever
                x[i] = random.NextDouble(12, 16); //NUMBERS ARE MADE POSITIVE TEMPORARILY TO AVOID COMPUTATION ERRORS.
                Logger.LogByTemplate(Debug, note: $"X array index {i} = {x[i]} ");
            }

            Logger.LogByTemplate(Information, note: "Application started ");
            double[,] arr2 = new double[8, 13];
            int[] numbers = { 5, 7, 11, 15 };
            arr2 = MathClass.Solve(
                emptyDoubleArray: arr2,
                arrayWithNumbersForСondition: numbers,
                firstBaseArray: k,
                secondBaseArray: x);



            Person neshnaika = new Person("Незнайка", "Житель солнечного города, Главный герой");
            Person kozlik = new Person("Козлик", "Досыта хлебнувший жизни лунатик");
            Person korotishka = new Person("Коротышка", "Неназванный житель");
            Person miga = new Person("Мига", "Житель лунного города");
            Bank bank1 = new Bank("Банк1", 1000000, 10);
            Person.BankAccount migaBankAccount1 = new Person.BankAccount(100, miga);
            Storage unburnedCloset = new Storage("НезгораемыйШкаф");
            Storage unburnedChest0 = new Storage("НезгораемыйСундук1");
            Storage unburnedChest1 = new Storage("НезгораемыйСундук2");
            unburnedChest0.StoreItems(new List<Item> { new Item("Акция1"), new Item("Акция2"), new Item("Акция3") });
            unburnedChest1.StoreItems(new List<Item> { new Item("Акция4"), new Item("Акция5"), new Item("Акция6") });

            Crowd thoseWhoWishingToPurchaseShares = new Crowd("Те кто хочет купить акции компании больших растений", "Желающие Приобрести Акции");
            Crowd passersby = new Crowd("Коротышки на улице", "Прохожие");
            Crowd population = new Crowd("Население города", "Население города");

            lines.AddRange(korotishka.PerformAction(new List<Item> { new Item("Деньги") }, Actions.InvestMoney));
            lines.AddRange(korotishka.BuyShares(new List<Item> { new Item("Акция1"), new Item("Акция2") }));
            lines.Add(korotishka.PerfomSimplyAction(Actions.Departure));
            lines.Add(thoseWhoWishingToPurchaseShares.GetState("Cтановилось всё больше и больше"));
            lines.AddRange(neshnaika.SellShares(new List<Item> { new Item("Акция1"), new Item("Акция2") }));
            lines.AddRange(kozlik.SellShares(new List<Item> { new Item("Акция3"), new Item("Акция4") }));
            lines.Add(miga.PerfomSimplyAction(Actions.Move));
            lines.Add(migaBankAccount1.DisplayBalance());
            lines.Add(bank1.MoneyExchange(miga));
            lines.Add(miga.PerformActionToStoreItem(
                sourceItem: new Item("Деньги"),
                action: Actions.InvestMoney,
                storage: unburnedCloset));
            migaBankAccount1.RemoveMoney(migaBankAccount1.Balance);
            lines.Add(migaBankAccount1.DisplayBalance());
            unburnedCloset.StoreItems(itemsToStore: new List<Item> { new Item("Деньги"), new Item("Деньги") });
            lines.Add(thoseWhoWishingToPurchaseShares.GetState("Толклись на улице, дожидаясь открытия конторы"));
            lines.Add(passersby.GetState("Заинтересовались происходящим"));
            lines.Add(population.GetState("Узнало об акциях Общества Гигантских растений"));
            lines.Add(population.GetState("Спешило накупить акций Общества Гигантских растений для выгодной перепродажи"));
            unburnedChest0.RetrieveItems(unburnedChest0.items.Count);
            unburnedChest1.RetrieveItems(unburnedChest1.items.Count);
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
                    Logger.LogByTemplate(Error, note: "Failed to generate number. try again");
                    throw new Exception("Failed to generate number. try again");
                }
                AddNarrativeline(answer);
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(Error,
                    ex
                    , $"Parsing failed. Invalid format in config file.");
            }
            finally
            {
                //Log.CloseAndFlush();
            }


            void AddNarrativeline(double number)
            {
                lines.Add($"Все акции общества были распроданы со средней стоимостью {Math.Abs(number)}");
            }

        }
        //Very bad code 
        public static void GetNumbersFromSendedArrayOfStrings(string[] arrrayOfStrings)
        {
            if (arrrayOfStrings.Length < 3)
            {
                Logger.LogByTemplate(Error, note: "Insufficient number of parameters in the array");
                return;
            }

            int.TryParse(arrrayOfStrings[0], out int tempNum1);
            int.TryParse(arrrayOfStrings[1], out int tempNum2);
            float.TryParse(arrrayOfStrings[2], out float tempDelayInSeconds);

            if (tempNum1 == 0)
            {
                Logger.LogByTemplate(Error, note: $"Error while Parsing first param from file");
            }
            else
            {
                num1 = tempNum1;
            }

            if (tempNum2 == 0)
            {
                Logger.LogByTemplate(Error, note: $"Error while Parsing second param from file");
            }
            else
            {
                num2 = tempNum2;
            }

            if (tempDelayInSeconds == 0)
            {
                Logger.LogByTemplate(Error, note: $"Error while Parsing third param from file");
            }
            else
            {
                delayInSeconds = tempDelayInSeconds;
            }
        }
    }
}
