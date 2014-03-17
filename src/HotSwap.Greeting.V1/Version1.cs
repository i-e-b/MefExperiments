namespace HotSwap.Greeting.V1
{
    using System;
    using System.ComponentModel.Composition;
    using HotSwap.Contracts;

    [Export(typeof(ISimpleTask))]
    public class Version1 : ISimpleTask
    {
        string _greet;

        public int Version()
        {
            return 1;
        }

        public object GetState()
        {
            return _greet;
        }

        public void SetState(object prevState)
        {
            throw new Exception("This is Version 1. You can't upgrade from version 1!");
        }

        public string Greeting(string who)
        {
            return _greet + " " + who;
        }

        public void SetGreets(string greet)
        {
            _greet = greet;
        }
    }
}
