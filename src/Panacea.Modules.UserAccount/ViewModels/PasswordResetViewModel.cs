using Panacea.Controls;
using Panacea.Core;
using Panacea.Modularity.UiManager;
using Panacea.Modules.UserAccount.Views;
using Panacea.Multilinguality;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.UserAccount.ViewModels
{
    [View(typeof(PasswordReset))]
    public class PasswordResetViewModel : PopupViewModelBase<object>
    {
        private PanaceaServices _core;
        private IUser user;

        public RelayCommand ChangePasswordCommand { get; set; }
        public RelayCommand CloseCommand { get; set; }

        public PasswordResetViewModel(PanaceaServices core)
        {
            this._core = core;
            this.user = core.UserService.User;
            ChangePasswordCommand = new RelayCommand(async args =>
            {
                if (_core.TryGetUiManager(out IUiManager _ui))
                {
                    await _ui.DoWhileBusy(async () =>
                    {
                        try
                        {
                            string password = await _core.UserService.ChangePasswordAsync(user.Email, user.DateOfBirth);
                            if (password != null)
                            {
                                await _ui.ShowPopup<object>(new NewPasswordViewModel(password));
                                SetResult(null);
                                return;
                            }
                        }
                        catch
                        {
                            _ui.Toast(new Translator("core").Translate("Unable to save new information due to network problems. Please try again later."));
                        }
                    });
                }
                else
                {
                    _core.Logger.Error(this, "ui manager not loaded");
                }
            });
            CloseCommand = new RelayCommand(args =>
            {
                SetResult(null);
            });
        }
    }
}
