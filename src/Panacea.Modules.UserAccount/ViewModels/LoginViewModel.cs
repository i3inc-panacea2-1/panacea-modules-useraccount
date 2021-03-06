﻿using Panacea.Controls;
using Panacea.Core;
using Panacea.Modularity.UiManager;
using Panacea.Modularity.UserAccount;
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
        private bool _watingForAnotherTask;

        public LoginViewModel(IUserAccountManager manager, PanaceaServices core, TaskCompletionSource<bool> source)
        {
            _core = core;
            _source = source;
            if (Debugger.IsAttached)
            {
                Email = "v.a@dotbydot.gr";
            }
            ForgotPasswordCommand = new RelayCommand(async arg =>
            {
                if (_core.TryGetUiManager(out IUiManager ui))
                {
                    var src = new TaskCompletionSource<bool>();
                    var forgotPassViewModel = new ForgotPasswordViewModel(_core, src);
                    _watingForAnotherTask = true;
                    ui.Navigate(forgotPassViewModel);
                    var res = await src.Task;
                    if (_core.TryGetUiManager(out IUiManager ui2))
                    {
                        ui2.GoHome();
                    }
                    source.SetResult(res);
                }
                else
                {
                    _core.Logger.Error(this, "ui manager not loaded");
                }
            });
            RegisterCommand = new RelayCommand(async arg =>
            {
                _watingForAnotherTask = true;
                source.SetResult(await manager.RegisterAsync());
            });
            LoginWithDateCommand = new AsyncCommand(async arg =>
            {
                try
                {
                    var password = (arg as PasswordBox).Password;
                    if (string.IsNullOrEmpty(Date.ToString()))
                    {
                        ShowWarning("Please provide a date of birth");
                        return;
                    }
                    if (string.IsNullOrEmpty(password))
                    {
                        ShowWarning("Please provide a password");
                        return;
                    }
                    if (await DoWhileBusy(() => core.UserService.LoginAsync(Date.Value, password)))
                    {
                        if (_core.TryGetUiManager(out IUiManager ui))
                        {
                            ui.GoHome();
                        }
                        source?.SetResult(true);
                        return;
                    }
                    else
                    {
                        ShowWarning("Incorrect credentials");
                        return;
                    }
                }
                catch
                {

                }
                finally
                {

                }

            });
            LoginWithEmailCommand = new AsyncCommand(async arg =>
            {
                var password = (arg as PasswordBox).Password;

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
                    if (_core.TryGetUiManager(out IUiManager ui))
                    {
                        ui.GoHome();
                    }
                    source?.SetResult(true);
                    return;

                }
                else
                {
                    ShowWarning("Incorrect credentials", PopupType.Error);
                    return;
                }
            });
        }

        public override void Activate()
        {
            _watingForAnotherTask = false;
        }

        public override void Deactivate()
        {
            if (!_watingForAnotherTask)
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

        async void ShowWarning(string s, PopupType type = PopupType.Warning)
        {
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                await ui.ShowPopup(new WarningViewModel(s), "Login failed", type);
            }
        }

        public string Email { get; set; }

        public DateTime? Date { get; set; } = null;

        private readonly PanaceaServices _core;
        private readonly TaskCompletionSource<bool> _source;

        public RelayCommand ForgotPasswordCommand { get; }
        public AsyncCommand LoginWithEmailCommand { get; }

        public AsyncCommand LoginWithDateCommand { get; }

        public RelayCommand RegisterCommand { get; }
    }
}
