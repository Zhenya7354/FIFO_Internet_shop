using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_3.Inż_oprogramowania
{
    //Репрезентує одну партію товару
    class Batch
    {
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public Batch(int Q, decimal UP)
        {
            Quantity = Q;
            UnitPrice = UP;
        }
    }

    class Warehouse
    {
        private readonly List<Batch> _batches;
        public Warehouse()
        {
            _batches = new List<Batch>();
        }

        //Додавання нової партії PZ
        public void AddBatch(int quantity, decimal UnitPrice)
        {
            _batches.Add(new Batch(quantity, UnitPrice));
        }

        public decimal IssueGoods(int quantity)
        {
            decimal totalValue = 0m;
            int toIssue = quantity;

            while(toIssue > 0 && _batches.Count > 0)
            {
                var currentBatch = _batches[0];
                // Продаємо цілу партію
                if(currentBatch.Quantity <= toIssue)
                {
                    //Обраховуємо ціну
                    totalValue += currentBatch.Quantity * currentBatch.UnitPrice;
                    toIssue -= currentBatch.Quantity;
                    //Усуваємо продану партію
                    _batches.RemoveAt(0);
                }
                else
                {
                    totalValue += toIssue * currentBatch.UnitPrice;
                    currentBatch.Quantity -= toIssue;
                    toIssue = 0;
                }
            }
            if(toIssue > 0 )
            {
                throw new InvalidOperationException("Brak wystarczajacej ilosci towaru w magazynie!");
            }
            return totalValue;
        }

        public (List<(int Quantity,decimal UnitPrice)> Inventory,int TotalQuantity, decimal TotalValue ) GetInventory()
        {
            var inventory = _batches.Select(b => (b.Quantity, b.UnitPrice)).ToList();
            int totalQuantitu = _batches.Sum(b => b.Quantity);
            decimal totalValue = _batches.Sum(b => b.Quantity * b.UnitPrice);
            return (inventory, totalQuantitu, totalValue);
        }
    }

    class PZDok
    {
        public PZDok(Warehouse warehouse, int Quantity, decimal UnitP)
        {
            warehouse.AddBatch(Quantity, UnitP);
        }
    }

    class WZDok
    {
        public int Quantity { get; }
        public decimal Value { get; }

        public WZDok(Warehouse warehouse, int Q)
        {
            Quantity = Q;
            Value = warehouse.IssueGoods(Q);
        }
    }
        internal class Program
    {
        static void Main(string[] args)
        {
            var warehouse = new Warehouse();

            //Додавання партій товару
            new PZDok(warehouse, 100, 10m);
            new PZDok(warehouse, 100, 20m);


            Console.WriteLine("Stan magazynu: ");
            var inventory = warehouse.GetInventory();
            foreach (var item in inventory.Inventory)
            {
                Console.WriteLine($"Partia: Ilosc: {item.Quantity}, Cena jednostkowa: {item.UnitPrice}");
            }
                Console.WriteLine($"Laczna ilosc: {inventory.TotalQuantity}, Laczna cena {inventory.TotalValue}");


            try
            {
                var WzDok = new WZDok(warehouse, 150);
                Console.WriteLine($"Wydano: {WzDok.Quantity} sztuk za laczna wartosc: {WzDok.Value}");
            }
            catch(InvalidOperationException ex)
            {
                Console.WriteLine($"Blad: {ex.Message}");
            }


            Console.WriteLine("Stan magazynu po wydaniu:");
            inventory = warehouse.GetInventory();
            foreach (var item in inventory.Inventory)
            {
                Console.WriteLine($"Partia: Ilosc: {item.Quantity}, Cena jednostkowa: {item.UnitPrice}");
            }
            Console.WriteLine($"Laczna ilosc: {inventory.TotalQuantity}, Laczna cena {inventory.TotalValue}");
        }
    }
}
