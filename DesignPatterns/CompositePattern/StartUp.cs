﻿using System;

namespace CompositePattern
{
    public class StartUp
    {
        public static void Main()
        {
            var phone = new SingleGift("Phone", 256);
            phone.CalculateTotalPrice();

            Console.WriteLine();

            var rootBox = new CompositeGift("RootBox", 0);
            var truckToy = new SingleGift("TruckToy", 100);
            var plainToy = new SingleGift("PlainToy", 200);
            rootBox.Add(truckToy);
            rootBox.Add(plainToy);
            var childBox = new CompositeGift("ChildBox", 0);
            var soldierToy = new SingleGift("SoldierToy", 200);
            childBox.Add(soldierToy);
            rootBox.Add(childBox);

            Console.WriteLine($"Total price of this composite present is {rootBox.CalculateTotalPrice()}");
        }
    }
}
