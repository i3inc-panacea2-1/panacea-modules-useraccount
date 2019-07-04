using Panacea.Controls;
using Panacea.Core;
using Panacea.Modularity.UiManager;
using Panacea.Modules.UserAccount.Views;
using Panacea.Multilinguality;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Panacea.Modules.UserAccount.ViewModels
{
    [View(typeof(UpdateCredentials))]
    public class UpdateCredentialsViewModel : ViewModelBase
    {
        private PanaceaServices _core;
        public IUser User { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public UpdateCredentialsViewModel(PanaceaServices core)
        {
            this._core = core;
            this.User = core.UserService.User;
            this.Email = User.Email;
            this.DateOfBirth = User.DateOfBirth;
            SaveCommand = new RelayCommand( async args =>
            {
                if (Email == User.Email && DateOfBirth == User.DateOfBirth)
                {
                    if (_core.TryGetUiManager(out IUiManager _uiManager))
                    {
                        _uiManager.GoBack();
                        return;
                    } else
                    {
                        _core.Logger.Error(this, "ui manager not loaded");
                    }
                }
                var errors = new List<TranslatableObject>();
                if (!string.IsNullOrEmpty(Email))
                {
                    if (
                        !Regex.IsMatch(Email,
                            @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                            RegexOptions.IgnoreCase))
                    {
                        errors.Add(new TranslatableObject("You must provide a valid email address."));
                    }
                }
                if (errors.Count > 0)
                {
                    OnError(errors);
                    return;
                }
                if (_core.TryGetUiManager(out IUiManager _ui))
                {
                    await _ui.DoWhileBusy(async () =>
                    {
                        try
                        {
                            string password = await _core.UserService.ChangePasswordAsync(Email, DateOfBirth);
                            if (password != null)
                            {
                                await _ui.ShowPopup<object>(new NewPasswordViewModel(password));
                                _ui.GoBack();
                                return;
                            }
                        }
                        catch(WebException)
                        {
                            _ui.Toast(new Translator("UserAccount").Translate("Unable to save new information due to network problems. Please try again later."));
                        }
                        catch(Exception ex)
                        {
                            _ui.Toast(ex.Message));
                        }
                    });
                }
                else
                {
                    _core.Logger.Error(this, "ui manager not loaded");
                }
            });
        }
        private void OnError(List<TranslatableObject> e)
        {
            var errorsPopup = new RegisterErrorsViewModel();
            errorsPopup.Errors = e;
            if(_core.TryGetUiManager(out IUiManager _ui))
            {
                _ui.ShowPopup(errorsPopup, new TranslatableObject("Change credentials failed", new Translator("UserAccount")).Text, PopupType.Warning);
            } else
            {
                _core.Logger.Error(this, "ui manager not loaded");
            }
            return;
        }
    }
}
