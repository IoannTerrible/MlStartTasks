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
        DepositMoney
    }
    interface IItemProcessor
    {
        void ProcessItems(string Name, List<Item> items, Actions action);
    }
    interface IFinancialOperations
    {
        void BuyShares(List<Item> shares);
        void SellShares(List<Item> shares);
        void DepositMoney(double money, BankAccount account);
    }
    abstract class IntelligenceСreature
    {
        public string Description { get; set; }

        public IntelligenceСreature(string des)
        {
            Description = des;
        }

        public virtual void PerformAction(List<Item> items, Actions action) { }
    }

    internal class Person : IntelligenceСreature, IFinancialOperations, IItemProcessor
    {
        public string Name { get; set; }
        public Person(string name, string des) : base(des)
        {
            Name = name;
        }
        public void ProcessItems(string Name, List<Item> items, Actions action)
        {
            string actionDescription = GetActionDescription(action);
            foreach (var item in items)
            {
                Console.WriteLine($"{Name} {actionDescription} предмет {item.Name}");
            }
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
                    return "вкладывает деньги";
                // Добавьте другие действия по мере необходимости
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }
        public override void PerformAction(List<Item> items, Actions action)
        {
            ProcessItems(Name, items, action);
        }

        public void BuyShares(List<Item> shares)
        {
            ProcessItems(Name, shares, Actions.BuyShares);
        }

        public void SellShares(List<Item> shares)
        {
            ProcessItems(Name, shares, Actions.SellShares);
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

            public void DisplayBalance()
            {
                Console.WriteLine($"{Person.Name}'s баланс: {Balance}");
            }
        }
        public void DepositMoney(double money, BankAccount account)
        {
            Console.WriteLine($"{Name} вкладывает {money}, на {account}");
            account.Balance += money;
        }
    }
    class Crowd : IntelligenceСreature
    {
        public Crowd(string des) : base(des)
        {

        }
    }

    internal class Item
    {
        public string Name { get; set; }

        public Item(string name)
        {
            Name = name;
        }
    }
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
