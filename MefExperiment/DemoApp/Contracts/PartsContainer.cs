using System.ComponentModel.Composition;

namespace DemoApp.Contracts
{
    public class PartsContainer
    {
        [Import]
        public IDemoDataSource DataSource { get; set; }
    }
}