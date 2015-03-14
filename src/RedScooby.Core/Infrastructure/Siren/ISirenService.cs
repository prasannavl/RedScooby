// Author: Prasanna V. Loganathar
// Created: 6:48 PM 15-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Threading.Tasks;

namespace RedScooby.Infrastructure.Siren
{
    public interface ISirenService : IDisposable
    {
        bool IsOn { get; }
        Task TurnOffAsync();
        Task TurnOnAsync();
    }
}
