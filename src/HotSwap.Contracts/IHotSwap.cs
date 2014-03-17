namespace HotSwap.Contracts
{
    /// <summary>
    /// Basic contract for a hot swap task
    /// </summary>
    public interface IHotSwap
    {
        /// <summary>
        /// Version; highest version available is run. All others are upgraded
        /// </summary>
        int Version();

        /// <summary>
        /// Get current state of task. This is requested by OLD versions before an upgrade
        /// </summary>
        object GetState();

        /// <summary>
        /// Set state of task. This is passed from OLD versions to the latest version.
        /// </summary>
        void SetState(object prevState);
    }
}