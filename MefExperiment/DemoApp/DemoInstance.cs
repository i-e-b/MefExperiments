using System;
using DemoApp.Contracts;

namespace DemoApp
{
    public class DemoInstance
    {
        private readonly PartsContainer _parts;

        public DemoInstance(PartsContainer parts)
        {
            _parts = parts;
        }

        public void GoDoStuff()
        {
            Console.WriteLine(string.Join(", ", _parts.DataSource.GetData(1)));
        }
    }
}