using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    abstract class IntelligenceСreature
    {
        public string Description { get; set; }

        public IntelligenceСreature(string des)
        {
            Description = des;
        }

        public virtual void PerformAction(List<Item> items) { }
    }

    class Person : IntelligenceСreature
    {
        public string Name { get; set; }
        public Person(string name, string des) : base(des)
        {
            Name = name;
        }

        public override void PerformAction(List<Item> items)
        {
            foreach (var item in items)
            {
                Console.WriteLine($"{Name} продал предмет {item.Name}");
            }
        }
    }
    class Crowd : IntelligenceСreature
    {
        public Crowd(string des) : base(des)
        {

        }
    }

    class Item
    {
        public string Name { get; set; }

        public Item(string name)
        {
            Name = name;
        }
    }
}
