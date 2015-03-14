// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.IO;
using System.Threading.Tasks;

namespace RedScooby.Infrastructure.Scaffold
{
    public interface IStreamProvider
    {
        Task<Stream> GetStream();
    }

    public interface IKeyedStreamProvider
    {
        Task<Stream> GetStream(string key);
    }
}
