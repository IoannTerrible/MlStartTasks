using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static MlStartTask2.Person;
//Коротышка.Выложил деньги
//Коротышка.Получил Акции
//Коротышка.Удалился
//Незнайка.Продавал акции
//Козлик.Продавал акции
//Мига.Ездил в банк
//Мига.Обменивал купюры
//Мига.Клал купюры в шкаф
//Покупатели.Явились
//Покупатели.Толкались
//Прохожие.Обратили внимание
//Население города.Узнало
//Население.Спешило покупать акции.
//Акции.Распродались.
namespace MlStartTask2
{
    public enum Actions
    {
        BuyShares,
        SellShares,
        DepositMoney,
        InvestMoney,
        Departure
    }
    interface IItemProcessor
    {
        List<string> ProcessItems(string Name, List<Item> items, Actions action);
    }
    interface IFinancialOperations
    {
        List<string> BuyShares(List<Item> shares);
        List<string> SellShares(List<Item> shares);
        string DepositMoney(double money, BankAccount account);
    }
    abstract class IntelligenceСreature
    {
        public string Description { get; set; }

        public IntelligenceСreature(string des)
        {
            Description = des;
        }
        public virtual List<string> PerformAction(List<Item> items, Actions action)
        {
            return new List<string>();
        }
        public virtual string GetState(string state)
        {
            return state;
        }
    }

    internal class Person : IntelligenceСreature, IFinancialOperations, IItemProcessor
    {
        public string Name { get; set; }
        public Person(string name, string des) : base(des)
        {
            Name = name;
        }
        public List<string> ProcessItems(string Name, List<Item> items, Actions action)
        {
            List<string> result = new List<string>();
            string actionDescription = GetActionDescription(action);

            foreach (var item in items)
            {
                result.Add($"{Name} {actionDescription} предмет {item.Name}");
            }

            return result;
        }
        private string GetActionDescription(Actions action)
        {
            switch (action)
            {
                case Actions.BuyShares:
                    return "покупает";
                case Actions.SellShares:
                    return "продает";
                case Actions.DepositMoney:
                    return "вкладывает";
                case Actions.InvestMoney:
                    return "инвестирует";
                case Actions.Departure:
                    return "удалился";
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }
        public override List<string> PerformAction(List<Item> items, Actions action)
        {
            return ProcessItems(Name, items, action);
        }
        public string PerfomSimplyAction(Actions action)
        {
            return $"{Name} {GetActionDescription(action)}";
        }
        public List<string> BuyShares(List<Item> shares)
        {
            return ProcessItems(Name, shares, Actions.BuyShares);
        }

        public List<string> SellShares(List<Item> shares)
        {
            return ProcessItems(Name, shares, Actions.SellShares);
        }
        public class BankAccount
        {
            public double Balance { get; set; }
            public int InterestRate { get; set; }
            public Person Person { get; set; }

            public BankAccount(double balance, int interestRate, Person person)
            {
                Balance = balance;
                InterestRate = interestRate;
                Person = person;
            }

            public string DisplayBalance()
            {
                return ($"{Person.Name}'s баланс: {Balance}");
            }
        }
        public string DepositMoney(double money, BankAccount account)
        {
            account.Balance += money;
            return ($"{Name} вкладывает {money}, на {account}");
        }
    }
    class Crowd : IntelligenceСreature
    {
        public string DescriptionOfCrowd { get; set; }
        public string NameOfCrowd { get; set; }
        public Crowd(string des, string name) : base(des)
        {
            DescriptionOfCrowd = des;
            NameOfCrowd = name;
        }
        public override string GetState(string state)
        {
            return $"{NameOfCrowd} {state}";
        }
    }
    internal record Item(string Name);
    class Bank
    {
        public string Name { get; set; }
        public double Money { get; set; }
        public int PercentageRate { get; set; }

        public Bank(string name, double money, int pros)
        {
            Name = name;
            Money = money;
            PercentageRate = pros;
        }

    }
}
