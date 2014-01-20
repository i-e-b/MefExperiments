using System.Collections.Generic;

namespace DemoApp.Contracts
{
    public interface IDemoDataSource
    {
        IEnumerable<string> GetData(int id);
    }
}