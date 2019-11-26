using System;

namespace FacadePattern
{
    public class StartUp
    {
        static void Main()
        {
            var car = new CarBuilderFacade()
                .Info
                    .WithType("Honda")
                    .WithColor("Red")
                    .WithNumberOfDoors(5)
                .Built
                    .InCity("Karlovo")
                    .AtAddress("Yumruk-Chal 1")
                .Build();

            Console.WriteLine(car);

        }
    }
}
