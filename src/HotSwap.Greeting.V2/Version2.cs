namespace HotSwap.Greeting.V2
{
    using System;
    using System.ComponentModel.Composition;
    using Contracts;

    [Export(typeof(ISimpleTask))]
    public class Version2 : ISimpleTask
    {
        string _greet;

        public int Version()
        {
            return 2;
        }

        public object GetState()
        {
            return _greet;
        }

        public void SetState(object prevState)
        {
            // Copy state from old version to this version.
            // Normally, this would have a more complex object, and would
            // check its compatibility with the incoming versions.
            // New versions should be able to know about older versions.
            //
            // We never allow going from a higher version to a lower one.
            // If you want to roll-back, release a next higher version with
            // the code from the rollback target version.

            if (!(prevState is string)) throw new Exception("Can't upgrade from that");
            _greet = prevState.ToString();
        }

        public string Greeting(string who)
        {
            return _greet + ", " + who + "!";
        }

        public void SetGreets(string greet)
        {
            _greet = greet;
        }
    }
}
