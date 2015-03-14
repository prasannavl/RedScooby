// Author: Prasanna V. Loganathar
// Created: 12:32 AM 17-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using RedScooby.Helpers;
using RedScooby.Infrastructure.Composition;
using RedScooby.Infrastructure.Framework.Commands;
using RedScooby.Logging;
using RedScooby.Models;
using RedScooby.Views.Circles;

namespace RedScooby.ViewModels.Circles
{
    public class CirclesViewModel : ViewModelBase
    {
        private readonly CirclesModel model;

        public CirclesViewModel(CirclesModel model)
        {
            this.model = model;

            NavigateToCircleDetailViewAsyncCommand = CommandFactory.CreateAsyncWithParameter<string>(async s =>
            {
                int id;
                if (int.TryParse(s, out id))
                {
                    var page =
                        await WindowHelpers.NavigateToWithModelContext<CircleDetailView>(id).ConfigureAwait(false);
                }
                else
                {
                    Log.Error("Invalid CircleId provided : " + s);
                }
            });
        }

        public AsyncRelayCommand<string> NavigateToCircleDetailViewAsyncCommand { get; private set; }
    }
}
