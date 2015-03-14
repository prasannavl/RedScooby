// Author: Prasanna V. Loganathar
// Created: 9:24 PM 11-02-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using RedScooby.Helpers;
using RedScooby.Infrastructure.Framework;
using RedScooby.Models;
using RedScooby.ViewModels.Setup;
using RedScooby.Views.Components;

namespace RedScooby.Views.Setup
{
    public class UserSetupViewBase : PageView<UserSetupViewModel> { }

    public sealed partial class UserSetupView
    {
        private const string signingUpText = "Signing up..";
        private readonly bool invokeLogin;
        private readonly bool invokeRegistration;
        private readonly Regex pinRegex = new Regex(@"^\d{1,4}");
        private LoginViewModel loginViewModel;
        private RegistrationViewModel registrationViewModel;
        private bool isInitialized;
        private bool isSending;
        private LaunchActivatedEventArgs launchActivatedEventArgs;
        private bool step2Accessed;
        private bool step3Accessed;
        private bool step4Accessed;
        public UserSetupView() : this(true, false) { }

        public UserSetupView(bool invokeLogin = true, bool invokeRegistration = false)
        {
            InitializeComponent();

            VisualStateManager.GoToState(this, Empty.Name, false);
            VisualStateManager.GoToState(this, VerificationHidden.Name, false);

            DatePickerFlyout.MinYear = new DateTimeOffset(1800, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);
            DatePickerFlyout.MaxYear = DateTimeOffset.Now;

            this.invokeLogin = invokeLogin;
            this.invokeRegistration = invokeRegistration;
        }

        public override UserSetupViewModel Model
        {
            get { return base.Model; }
            set
            {
                base.Model = value;
                if (value != null)
                {
                    loginViewModel = value.LoginViewModel;
                    registrationViewModel = value.RegistrationViewModel;
                }
            }
        }

        public void CarryLaunchParameters(LaunchActivatedEventArgs e)
        {
            launchActivatedEventArgs = e;
        }

        public void InvokeLogin()
        {
            PagePivot.SelectedIndex = 1;
            VisualStateManager.GoToState(this, Step1.Name, true);
            VisualStateManager.GoToState(this, LoginNumberVerificationStep1.Name, true);
        }

        public void InvokeRegistration()
        {
            VisualStateManager.GoToState(this, Step1.Name, true);
            VisualStateManager.GoToState(this, LoginNumberVerificationStep1.Name, false);
        }

        public async Task InitializeAsync()
        {
            if (!isInitialized)
            {
                if (Model == null)
                    Model = CreateViewModel();

                // Important to set this variable before the thread could yield.

                isInitialized = true;
                await Model.InitializeAsync();
                Model.Load();

                // Could potentially be executed on a different thread. Ensure its always on the correct scheduler.

                DispatchHelper.Current.Run(() =>
                {
                    Unloaded += (sender, args) => { Model.Cleanup(); };

                    var dob = Model.RegistrationViewModel.User.DateOfBirth;
                    if (dob != default(DateTimeOffset))
                    {
                        DateOfBirthButton.Content = dob.ToString("D");
                    }

                    GenderComboBox.SelectedIndex = Model.RegistrationViewModel.User.Gender == Gender.Male ? 0 : 1;
                });
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            // Run it in a separate task, if its set to autoInitialize and wasn't preloaded.
            // Running as a separate task is important to load the page immediately, and keep it responsive
            // on lower end phones while post initialize is done separately.

            if (!isInitialized)
            {
                Task.Run(() => InitializeAsync());
            }

            if (invokeLogin)
                InvokeLogin();
            if (invokeRegistration)
                InvokeRegistration();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            if (!isInitialized)
            {
                Task.Run(() => InitializeAsync());
            }
        }

        protected override void OnUnloaded()
        {
            base.OnUnloaded();
            Model.Cleanup();
            isInitialized = false;
        }

        private void CompleteVerificationBackButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, LoginNumberVerificationStep1.Name, true);
        }

        private void CompleteVerificationNextButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FinishLogin();
        }

        private void DatePickerFlyout_OnDatePicked(DatePickerFlyout sender, DatePickedEventArgs args)
        {
            registrationViewModel.User.DateOfBirth = args.NewDate;
            DateOfBirthButton.Content = args.NewDate.ToString("D");
        }

        private async void FinishLogin()
        {
            var systemProgress = StatusBar.GetForCurrentView().ProgressIndicator;
            systemProgress.Text = "logging in";
            var _ = systemProgress.ShowAsync();

            //DESIGN_TEMP: Delay
            await Task.Delay(1000);

            var res = await loginViewModel.LoginAsync();
            if (res.Error == null)
            {
                var page = new PreStartNotesView();
                page.CarryForwardLaunchEventArgs(launchActivatedEventArgs);
                Window.Current.Content = page;
                OnUnloaded();
            }
            else
            {
                var dialog = new MessageDialog(res.Error, "Something went wrong.");
                var ignore = dialog.ShowAsync();
            }

            var ix2 = systemProgress.HideAsync();
        }

        private void GenderComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (registrationViewModel != null)
            {
                var combo = (ComboBox) sender;
                registrationViewModel.User.Gender = combo.SelectedIndex == 0 ? Gender.Male : Gender.Female;
            }
        }

        private void NumberUpdateTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            // Windows Phone Data Binding doesn't support modifying the updating element directly. 
            // So workaround with propogating the new changed value from the model to UI
            var txtBox = (TextBox) sender;

            // Tag set for Login only. Differentiate the two phone number inputs.
            if (txtBox.Tag == null)
            {
                if (txtBox.Text != registrationViewModel.PhoneNumber)
                {
                    registrationViewModel.PhoneNumber = txtBox.Text;
                    txtBox.Text = registrationViewModel.PhoneNumber;
                }
                else
                {
                    txtBox.Select(txtBox.Text.Length, 0);
                }
            }
            else
            {
                if (txtBox.Text != loginViewModel.PhoneNumber)
                {
                    loginViewModel.PhoneNumber = txtBox.Text;
                    txtBox.Text = loginViewModel.PhoneNumber;
                }
                else
                {
                    txtBox.Select(txtBox.Text.Length, 0);
                }
            }
        }

        private async void PhoneLoginNextButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var res = loginViewModel.ValidatePhoneNumber();
            if (res.IsValid)
            {
                if (!isSending)
                {
                    isSending = true;
                    VisualStateManager.GoToState(this, VerificationShown.Name, true);

                    var phRes = await loginViewModel.RequestPhoneVerificationAsync();

                    isSending = false;
                    VisualStateManager.GoToState(this, VerificationHidden.Name, true);

                    if (!phRes.Success)
                    {
                        var dialog = new MessageDialog(phRes.Error, "Something went wrong.");
                        var ignore = dialog.ShowAsync();
                    }
                    else
                    {
                        VisualStateManager.GoToState(this, LoginNumberVerificationStep2.Name, true);
                    }
                }
            }
            else
            {
                var dialog = new MessageDialog(res.Errors.First().ErrorMessage, "Invalid information");
                await dialog.ShowAsync();
            }
        }

        private async void Step1NextButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var res = registrationViewModel.ValidateBasicInfo();
            if (res.IsValid)
            {
                await Step1To2Story.RunOnceOrSkipToEndAsync(ref step2Accessed);
                VisualStateManager.GoToState(this, Step2.Name, false);
            }
            else
            {
                var dialog = new MessageDialog(res.Errors.First().ErrorMessage, "Invalid information");
                await dialog.ShowAsync();
            }
        }

        private void Step2BackButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Step1To2Story.Stop();
            VisualStateManager.GoToState(this, Step1.Name, false);
        }

        private void Step2NextButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SwitchToState(Step3.Name, ref step3Accessed);
        }

        private void Step3BackButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, Step2.Name, false);
        }

        private async void Step3NextButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var res = registrationViewModel.ValidatePinCodes();
            if (res.IsValid)
            {
                SwitchToState(Step4.Name, ref step4Accessed);
            }
            else
            {
                var dialog = new MessageDialog(res.Errors.First().ErrorMessage, "Invalid information");
                await dialog.ShowAsync();
            }
        }

        private void Step4BackButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, Step3.Name, false);
        }

        private async void Step4NextButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var res = registrationViewModel.ValidatePhoneNumber();
            if (res.IsValid)
            {
                if (!isSending)
                {
                    isSending = true;
                    SignupProgressTextBlock.Text = signingUpText;

                    VisualStateManager.GoToState(this, VerificationShown.Name, true);

                    var regResult = await registrationViewModel.RegisterAsync();
                    if (!regResult.Success)
                    {
                        VisualStateManager.GoToState(this, VerificationHidden.Name, true);
                        isSending = false;

                        var dialog = new MessageDialog(regResult.Error, "Something went wrong.");
                        var ignore = dialog.ShowAsync();
                    }
                    else
                    {
                        loginViewModel.LoadFromUserModel(regResult.RegisteredUser);

                        VisualStateManager.GoToState(this, LoginNumberVerificationStep1.Name, false);

                        PagePivot.SelectedIndex = 1;

                        var phRes = await loginViewModel.RequestPhoneVerificationAsync();

                        isSending = false;
                        VisualStateManager.GoToState(this, VerificationHidden.Name, true);

                        if (!phRes.Success)
                        {
                            var dialog = new MessageDialog(phRes.Error, "Something went wrong.");
                            var ignore = dialog.ShowAsync();
                        }
                        else
                        {
                            VisualStateManager.GoToState(this, LoginNumberVerificationStep2.Name, true);
                        }
                    }
                }
            }
            else
            {
                var dialog = new MessageDialog(res.Errors.First().ErrorMessage, "Invalid information");
                var _ = dialog.ShowAsync();
            }
        }

        private void SwitchToState(string state, ref bool stateTransitionsCheckFlag)
        {
            // Animations only on the first view. Click back, or next from then on shouldn't display 
            // animations.

            if (!stateTransitionsCheckFlag)
            {
                stateTransitionsCheckFlag = true;
                VisualStateManager.GoToState(this, state, true);
            }
            else
            {
                VisualStateManager.GoToState(this, state, false);
            }
        }

        private void ValidateTextBoxForPin(object sender, TextChangedEventArgs e)
        {
            // Windows Phone data binding doesn't support modifying the updating element directly. 
            // Handle pre-restriction directly in the UI instead of waiting for validation.

            var tb = (TextBox) sender;

            var text = tb.Text;
            var selectionStart = tb.SelectionStart;

            if (!pinRegex.IsMatch(text))
            {
                tb.Text = string.Empty;
            }
            if (text.Length > 4)
            {
                tb.Text = text.Remove(4);
            }

            if (sender == DisasterPinTextBox)
            {
                Model.RegistrationViewModel.User.Settings.DisasterPinCode = tb.Text;
            }
            else if (sender == PinTextBox)
            {
                Model.RegistrationViewModel.User.Settings.PinCode = tb.Text;
            }

            tb.SelectionStart = selectionStart;
        }

        private void NameTextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox) sender;
            var selectionStart = tb.SelectionStart;

            var text = tb.Text.ToConservativeTitleCase();
            Model.RegistrationViewModel.User.Name = text;
            tb.SelectionStart = selectionStart;
        }
    }
}
