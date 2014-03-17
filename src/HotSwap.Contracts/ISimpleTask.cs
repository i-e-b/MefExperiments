namespace HotSwap.Contracts
{
    /// <summary>
    /// Really dumb sample task contract
    /// </summary>
    public interface ISimpleTask : IHotSwap
    {
        /// <summary>
        /// Greet someone, with stateful greeting
        /// </summary>
        string Greeting(string who);

        /// <summary>
        /// How to greet.
        /// </summary>
        void SetGreets(string greet);
    }
}