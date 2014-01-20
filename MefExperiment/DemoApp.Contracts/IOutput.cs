namespace DemoApp.Contracts
{
    /// <summary>
    /// Spits out a string to somewhere
    /// </summary>
    public interface IOutput
    {
        /// <summary>
        /// Write a string. Where it goes is implementation specific.
        /// </summary>
        void Write(string s);
    }
}