using Panacea.Controls;
using Panacea.Core;
using Panacea.Modularity.UiManager;
using Panacea.Modules.UserAccount.Views;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Panacea.Modules.UserAccount.ViewModels
{
    [View(typeof(LoginControl))]
    class LoginViewModel : ViewModelBase
    {
        private bool _signingIn;

        public LoginViewModel(PanaceaServices core, TaskCompletionSource<bool> source)
        {
            _core = core;
            _source = source;
            if (Debugger.IsAttached)
            {
                Email = "v.a@dotbydot.gr";
            }
            LoginWithDateCommand = new RelayCommand(async arg =>
            {
                try
                {
                    _signingIn = true;
                    var password = (arg as PasswordBox).Password;
                    if (string.IsNullOrEmpty(Date))
                    {
                        ShowWarning("Please provide a date of birth");
                        return;
                    }
                    if (string.IsNullOrEmpty(password))
                    {
                        ShowWarning("Please provide a password");
                        return;
                    }
                    if (await DoWhileBusy(() => core.UserService.LoginAsync(DateTime.Parse(Date), password)))
                    {
                        source?.SetResult(true);
                    }
                    else
                    {
                        source?.SetResult(false);
                    }
                }
                catch
                {

                }
                finally
                {
                    _signingIn = false;
                }

            });
            LoginWithEmailCommand = new RelayCommand(async arg =>
            {
                try
                {

                    var password = (arg as PasswordBox).Password;
                    _signingIn = true;

                    if (string.IsNullOrEmpty(Email))
                    {
                        ShowWarning("Please provide an email");
                        return;
                    }

                    if (!Regex.IsMatch(Email,
                            @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                            RegexOptions.IgnoreCase))
                    {
                        ShowWarning("Please provide a valid email");
                        return;
                    }

                    if (string.IsNullOrEmpty(password))
                    {
                        ShowWarning("Please provide a password");
                        return;
                    }

                    if (await DoWhileBusy(() => core.UserService.LoginAsync(Email, password)))
                    {
                        source?.SetResult(true);
                    }
                    else
                    {
                        source?.SetResult(false);
                    }
                }
                catch
                {

                }
                finally
                {
                    _signingIn = false;
                }
            });
        }

        public override void Deactivate()
        {
            _source.TrySetResult(false);
        }

        private Task<T> DoWhileBusy<T>(Func<Task<T>> act)
        {
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                return ui.DoWhileBusy(act);
            }
            else
            {
                return act();
            }
        }

        async void ShowWarning(string s)
        {
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                await ui.ShowPopup(new WarningViewModel(s), "Login failed", PopupType.Warning);
            }
        }

        public string Email { get; set; }

        public string Date { get; set; }

        private readonly PanaceaServices _core;
        private readonly TaskCompletionSource<bool> _source;

        public RelayCommand LoginWithEmailCommand { get; }

        public RelayCommand LoginWithDateCommand { get; }
    }
}
