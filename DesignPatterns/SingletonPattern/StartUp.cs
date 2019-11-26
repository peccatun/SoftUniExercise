using SingletonPattern;
using System;

namespace DesignPatterns
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var db = SingletonDataContainer.Instance;
            var db2 = SingletonDataContainer.Instance;
            var db3 = SingletonDataContainer.Instance;
            var db4 = SingletonDataContainer.Instance;

            Console.WriteLine(db.GetPopulation("Sofiq"));

        }
    }
}
