using Panacea.Controls;
using Panacea.Core;
using Panacea.Modularity.UiManager;
using Panacea.Modules.UserAccount.Models;
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
    [View(typeof(ForgotPassword))]
    public class ForgotPasswordViewModel : ViewModelBase
    {
        readonly Translator translator = new Translator("core");
        PanaceaServices _core;
        private readonly TaskCompletionSource<bool> _src;
        private async Task<bool> GetSecurityQuestionsAsync()
        {
            if (_core.TryGetUiManager(out IUiManager _ui))
            {
                return await _ui.DoWhileBusy(async () =>
                {
                    try
                    {
                        var response =
                            await _core.HttpClient.GetObjectAsync<List<SecurityQuestion>>("get_security_questions/", allowCache: false);
                        if (response.Success)
                        {
                            var lst = response.Result;
                            SecurityQuestions = lst;
                            OnPropertyChanged(nameof(SecurityQuestions));
                            return true;
                        }
                        return false;
                    }
                    catch
                    {
                        var errorsPopup = new RegisterErrorsViewModel();
                        var errors = new List<TranslatableObject>
                            {
                               new TranslatableObject("An unexpected error occured. Please try again later.", translator)
                            };
                        errorsPopup.Errors = errors;
                        await _ui.ShowPopup(errorsPopup);
                        return false;
                    }
                });
            }
            else
            {
                _core.Logger.Error(this, "ui manager not loaded");
            }
            return false;
        }
        public override async void Activate()
        {
            if (!await GetSecurityQuestionsAsync())
            {
                _src.TrySetResult(false);
            }
        }

        public ForgotPasswordViewModel(PanaceaServices core, TaskCompletionSource<bool> src)
        {
            _core = core;
            _src = src;

            RecoverPasswordCommand = new AsyncCommand(async arg =>
            {
                try
                {
                    var fname = FirstName;
                    var lname = LastName;
                    var dob = ((DateTime)DateOfBirth).ToString("yyyy-MM-dd");
                    var e_mail = Email;
                    var question = Question.Question;
                    var answer = Answer;
                    if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName)
                        || Question == null || string.IsNullOrEmpty(Answer))
                    {
                        throw new Exception();
                    }

                    if (_core.TryGetUiManager(out IUiManager _ui))
                    {
                        await _ui.DoWhileBusy(async () =>
                        {
                            try
                            {
                                var response = await _core.HttpClient.GetObjectAsync<string>(
                                    "user_forgot_password/",
                                    postData: new List<KeyValuePair<string, string>>()
                                {
                        new KeyValuePair<string, string>("email", e_mail),
                        new KeyValuePair<string, string>("security_question", question),
                        new KeyValuePair<string, string>("security_answer", answer),
                        new KeyValuePair<string, string>("date_of_birth", dob),
                        new KeyValuePair<string, string>("first_name", fname),
                        new KeyValuePair<string, string>("last_name", lname)
                                });

                                if (response.Success)
                                {
                                    var res = await _core.UserService.LoginAsync(DateOfBirth, response.Result);
                                    var passPopup = new NewPasswordViewModel(response.Result);
                                    await _ui.ShowPopup(passPopup, null, PopupType.Success);
                                    src.SetResult(true);
                                    return;

                                }
                                else
                                {
                                    var pop = new SimplePopupViewModel(response.Error);
                                    await _ui.ShowPopup(pop, null, PopupType.Error);
                                }
                            }
                            catch
                            {
                                var pop = new SimplePopupViewModel("Please fill all required data");
                                _ui.ShowPopup(pop, null, PopupType.Error);
                            }
                        });
                    }
                    else
                    {
                        _core.Logger.Error(this, "ui manager not loaded");
                    }
                }
                catch
                {

                    if (_core.TryGetUiManager(out IUiManager ui))
                    {
                        var pop = new SimplePopupViewModel(translator.Translate("Please fill all required data"));
                        await ui.ShowPopup(pop, null, PopupType.Error);
                    }
                    else
                    {
                        _core.Logger.Error(this, "ui manager not loaded");
                    }

                }
            });
            CancelCommand = new RelayCommand(arg =>
            {

            });
        }

        public List<SecurityQuestion> SecurityQuestions { get; set; }
        public AsyncCommand RecoverPasswordCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string DOBString { get; set; }
        public string Email { get; set; }
        public SecurityQuestion Question { get; set; }
        public string Answer { get; set; }
    }
}
