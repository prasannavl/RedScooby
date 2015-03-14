// Author: Prasanna V. Loganathar
// Created: 9:52 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.Phone.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using RedScooby.Actions;
using RedScooby.Data.Tables;
using RedScooby.Infrastructure.Framework.Commands;
using RedScooby.Logging;
using RedScooby.Models;
using RedScooby.ViewModels.Circles;
using RedScooby.Views.Components;
using ContactEmail = RedScooby.Data.Tables.ContactEmail;

namespace RedScooby.Views.Circles
{
    public class CircleDetailViewBase : PageView<CircleDetailViewModel> { }

    public sealed partial class CircleDetailView
    {
        private readonly TaskCompletionSource<bool> initCompletionSource = new TaskCompletionSource<bool>();
        private readonly TaskCompletionSourceHolder addContactsTcsHolder = new TaskCompletionSourceHolder();
        private CircleDetailViewModel modelCache;
        private PivotItem lastSelectedPivotItem;
        private ContactStore contactStore;
        private Func<ContactModel, Task<Stream>> thumbnailProvider;

        public CircleDetailView()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        public override CircleDetailViewModel CreateViewModel()
        {
            return modelCache ?? (modelCache = base.CreateViewModel());
        }

        public AppBar GetDefaultAppBar()
        {
            var appBar = new CommandBar();
            appBar.PrimaryCommands.Add(new AppBarButton
            {
                Icon = new SymbolIcon(Symbol.Add),
                Label = "add",
                Command = CommandFactory.CreateAsync(PickContactsAsync),
            });

            appBar.PrimaryCommands.Add(new AppBarButton
            {
                Icon = new SymbolIcon(Symbol.Edit),
                Label = "edit",
                Command = CommandFactory.Create(SwitchListViewToEdit)
            });

            return appBar;
        }

        public AppBar GetEditModeAppBar()
        {
            var appBar = new CommandBar();

            appBar.PrimaryCommands.Add(new AppBarButton
            {
                Icon = new SymbolIcon(Symbol.Delete),
                Label = "delete",
                Command = CommandFactory.CreateAsync(DeleteSelectedAsync)
            });

            return appBar;
        }


        protected override async void OnInitialized()
        {
            try
            {
                contactStore = await ContactManager.RequestStoreAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            thumbnailProvider = async contact =>
            {
                if (contact == null)
                    return null;

                var id = contact.LocalStoreId;
                var store = contactStore;

                if (store == null)
                    return null;

                try
                {
                    // Make sure context is restored, required for OpenRead of thumbnail
                    var c = await store.GetContactAsync(id);
                    if (c != null)
                    {
                        var thumb = c.Thumbnail;
                        if (thumb != null)
                        {
                            var nativeStream = await thumb.OpenReadAsync().AsTask().ConfigureAwait(false);
                            if (nativeStream != null)
                            {
                                return nativeStream.AsStreamForRead();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warn(ex);
                }

                return null;
            };

            initCompletionSource.TrySetResult(true);

            base.OnInitialized();
        }

        protected override async void OnSetModelContext(object context)
        {
            base.OnSetModelContext(context);
            try
            {
                await initCompletionSource.Task;
                Model.SetThumbnailProvider(thumbnailProvider);
                await Model.SetContextAsync((int) context);
            }
            catch (TaskCanceledException) { }
        }

        protected override void OnUnloaded()
        {
            initCompletionSource.TrySetCanceled();
            var tcs = addContactsTcsHolder.Source;
            if (tcs != null)
            {
                tcs.TrySetCanceled();
            }
            base.OnUnloaded();
        }

        private async Task DeleteSelectedAsync()
        {
            var selectedItems = MembersListView.SelectedItems.ToArray();
            foreach (CircleDetailViewModel.UserContactWithThumbnail selectedItem in selectedItems)
            {
                await Model.RemoveContactFromCircle(selectedItem);
            }
            SwitchListViewToNormal();
        }

        private void SwitchListViewToEdit()
        {
            if (Model.Members.Count > 0)
            {
                MembersListView.SelectionMode = ListViewSelectionMode.Multiple;
                ShowAppBar(GetEditModeAppBar());
            }

            ShellManager.Current.BackPressed += BackPressHandlerForEditView;
        }

        private void BackPressHandlerForEditView(object sender, BackPressedEventArgs backPressedEventArgs)
        {
            SwitchListViewToNormal();
            backPressedEventArgs.Handled = true;
        }

        private void SwitchListViewToNormal()
        {
            MembersListView.SelectionMode = ListViewSelectionMode.None;
            ShowAppBar(GetDefaultAppBar());
            ShellManager.Current.BackPressed -= BackPressHandlerForEditView;
        }

        private async Task PickContactsAsync()
        {
            var picker = new ContactPicker();
            // TODO: Reimplement picker using the store instead, to pick only the name. 
            picker.DesiredFieldsWithContactFieldType.Add(ContactFieldType.Email | ContactFieldType.PhoneNumber);
            var contacts = await picker.PickContactsAsync();

            var tcs = addContactsTcsHolder.Source;
            if (tcs == null)
            {
                tcs = new TaskCompletionSource<bool>();
                addContactsTcsHolder.Source = tcs;
            }
            else
            {
                await tcs.Task;
            }

            await Task.Run(async () =>
            {
                var contactModels = contacts.Select(x => new ContactModel()
                {
                    LocalStoreId = x.Id,
                    Name = x.FirstName + " " + x.LastName,
                    Emails =
                        new ObservableCollection<ContactEmail>(
                            x.Emails.Select(e => new ContactEmail
                            {
                                Email = e.Address,
                            })),
                    PhoneNumbers =
                        new ObservableCollection<ContactPhoneNumber>(x.Phones.Select(p => new ContactPhoneNumber
                        {
                            Number = p.Number,
                        })),
                });

                foreach (var contact in contactModels)
                {
                    if (tcs.Task.IsCanceled)
                        break;

                    await Model.AddContactToCircle(contact);
                }
            });

            tcs.TrySetResult(true);
            addContactsTcsHolder.Source = null;
        }

        private void Pivot_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var pivot = (PivotItem) RootPivot.SelectedItem;
            if (pivot == MembersPivotItem)
            {
                ShowAppBar(GetDefaultAppBar());
            }
            else
            {
                HideAppBar();
            }

            lastSelectedPivotItem = pivot;
        }

        public class TaskCompletionSourceHolder
        {
            public TaskCompletionSource<bool> Source;
        }
    }
}
