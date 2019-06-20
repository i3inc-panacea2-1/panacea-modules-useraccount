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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Panacea.Modules.UserAccount.ViewModels
{
    [View(typeof(UpdateAccount))]
    public class UpdateAccountViewModel : ViewModelBase
    {
        private PanaceaServices _core;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Telephone { get; set; }
        private string oldFirstName;
        private string oldLastName;
        private string oldTelephone;
        public IUser User { get; set; }
        public override void Activate()
        {
            oldFirstName = this.User.FirstName;
            oldLastName = this.User.LastName;
            oldTelephone = this.User.Telephone;
            base.Activate();
        }
        public UpdateAccountViewModel(PanaceaServices core)
        {
            this._core = core;
            this.User = _core.UserService.User;
            FirstName = this.User.FirstName;
            LastName = this.User.LastName;
            Telephone = this.User.Telephone;
            ChangeCredentialsCommand = new RelayCommand(arg =>
            {

                if (_core.TryGetUiManager(out IUiManager ui))
                {
                    ui.Navigate(new UpdateCredentialsViewModel(_core));
                }
                else
                {
                    _core.Logger.Error(this, "ui manager not loaded");
                }
            });
            ResetPasswordCommand = new RelayCommand(async arg =>
            {

                if (_core.TryGetUiManager(out IUiManager ui))
                {
                    await ui.ShowPopup<object>(new PasswordResetViewModel(_core));
                }
                else
                {
                    _core.Logger.Error(this, "ui manager not loaded");
                }
            });
            SaveCommand = new RelayCommand(async arg =>
            {
                if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName))
                {

                    if (_core.TryGetUiManager(out IUiManager _ui))
                    {
                        _ = _ui.ShowPopup(
                            new SimplePopupViewModel(
                                new Translator("core").Translate("Last and first names are required.")),
                            null,
                            PopupType.Error);
                        return;
                    }
                    else
                    {
                        _core.Logger.Error(this, "ui manager not loaded");
                    }
                }

                if (_core.TryGetUiManager(out IUiManager ui))
                {
                    await ui.DoWhileBusy(async () =>
                    {
                        try
                        {
                            if (await _core.UserService.UpdateUserInfoAsync(FirstName,LastName,Telephone))
                            {
                                _ = ui.ShowPopup(
                                    new SimplePopupViewModel(
                                        new Translator("core").Translate("Your account has been updated!")),
                                    null,
                                    PopupType.Information);
                                oldFirstName = FirstName;
                                oldLastName = LastName;
                                oldTelephone = Telephone;
                            }
                            else
                            {
                                _ = ui.ShowPopup(
                                    new SimplePopupViewModel(
                                        new Translator("core").Translate("Your account was not updated. Please try again later.")),
                                    null,
                                    PopupType.Error);
                                FirstName = oldFirstName;
                                LastName = oldLastName;
                                Telephone = oldTelephone;
                            }
                            ui.GoBack();
                        }
                        catch
                        {
                            _ = ui.ShowPopup(
                                new SimplePopupViewModel(
                                    new Translator("core").Translate("Your account was not updated. Please try again later.")),
                                null,
                                PopupType.Error);
                            FirstName = oldFirstName;
                            LastName = oldLastName;
                            Telephone = oldTelephone;
                        }
                    });
                }
                else
                {
                    _core.Logger.Error(this, "ui manager not loaded");
                }
            });
        }
        public RelayCommand ChangeCredentialsCommand { get; }
        public RelayCommand ResetPasswordCommand { get; }
        public RelayCommand SaveCommand { get; }
    }
}
