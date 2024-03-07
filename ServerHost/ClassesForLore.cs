using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerHost
{
    public enum Actions
    {
        BuyShares,
        SellShares,
        DepositMoney,
        InvestMoney,
        Departure,
        Move
    }
    interface IItemProcessor
    {
        List<string> ProcessItems(string Name, List<Item> items, Actions action);
    }
    interface IFinancialOperations
    {
        List<string> BuyShares(List<Item> shares);
        List<string> SellShares(List<Item> shares);
        string DepositMoney(double money, Person.BankAccount account);
    }
    public abstract class IntelligenceСreature
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

    public class Person : IntelligenceСreature, IFinancialOperations, IItemProcessor
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
                case Actions.Move:
                    return "ездил";
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }
        public override List<string> PerformAction(List<Item> items, Actions action)
        {
            return ProcessItems(Name, items, action);
        }
        public string PerformActionToStoreItem(Item sourceItem, Actions action, Storage storage)
        {
            string actionDescription = GetActionDescription(action);
            return $"{Name} {actionDescription} {sourceItem.Name} в {storage.Name}";
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
        public string DepositMoney(double money, BankAccount account)
        {
            account.Balance += money;
            return ($"{Name} вкладывает {money}, на {account}");
        }
        public class BankAccount
        {
            public double Balance { get; set; }
            public Person Person { get; set; }

            public BankAccount(double balance, Person person)
            {
                Balance = balance;
                Person = person;
            }

            public string DisplayBalance()
            {
                return ($"{Person.Name}'s баланс: {Balance}");
            }
            public void AddMoney(double money)
            {
                Balance += money;
            }
            public void RemoveMoney(double money)
            {
                Balance -= money;
            }
        }
    }
    public class Crowd : IntelligenceСreature
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
    public record Item(string Name);

    public class Storage
    {
        public List<Item> items;
        public string Name { get; set; }

        public Storage(string name)
        {
            items = new List<Item>();
            Name = name;
        }

        public void StoreItems(List<Item> itemsToStore)
        {
            items.AddRange(itemsToStore);
        }

        public List<Item> RetrieveItems(int quantity)
        {
            if (quantity > items.Count)
            {
                quantity = items.Count;
            }

            List<Item> retrievedItems = items.GetRange(0, quantity);
            items.RemoveRange(0, quantity);

            return retrievedItems;
        }
    }

    public class Bank
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
        public string MoneyExchange(Person person)
        {
            return $"{person.Name} обменивал купюры";

        }
    }
}
