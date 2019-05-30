using Panacea.Controls;
using Panacea.Core;
using Panacea.Modularity.UiManager;
using Panacea.Modularity.UserAccount;
using Panacea.Modules.UserAccount.Models;
using Panacea.Modules.UserAccount.Views;
using Panacea.Multilinguality;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Panacea.Modules.UserAccount.ViewModels
{
    [View(typeof(RegisterControl))]
    class RegisterViewModel : ViewModelBase
    {
        public RegisterViewModel(IUserAccountManager manager, PanaceaServices core, TaskCompletionSource<bool> source)
        {
            _core = core;
            _source = source;
            LoginCommand = new RelayCommand(async arg =>
            {
                _waitingForAnotherTask = true;
                var res = await manager.LoginAsync();
                _waitingForAnotherTask = false;
                source.TrySetResult(res);
            });
            RegisterCommand = new AsyncCommand(async arg =>
            {
                await RegisterAsync();
            });
        }
        public override void Deactivate()
        {
            if (!_waitingForAnotherTask)
                _source.TrySetResult(false);
        }

        bool _waitingForAnotherTask = false;
        public RelayCommand LoginCommand { get; }

        public AsyncCommand RegisterCommand { get; }

        Translator _translator = new Translator("UserAccount");
        string _firstName;
        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }

        string _lastName;
        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged();
            }
        }

        string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        string _answer;
        public string Answer
        {
            get => _answer;
            set
            {
                _answer = value;
                OnPropertyChanged();
            }
        }

        DateTime _dateOfBirth = new DateTime(1980, 1, 1);
        public DateTime DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                _dateOfBirth = value;
                OnPropertyChanged();
            }
        }

        string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();
            }
        }

        string _cardCode;
        public string CardCode
        {
            get => _cardCode;
            set
            {
                _cardCode = value;
                OnPropertyChanged();
            }
        }

        SecurityQuestion _question;
        public SecurityQuestion Question
        {
            get => _question;
            set
            {
                _question = value;
                OnPropertyChanged();
            }
        }

        List<SecurityQuestion> _questions;
        private readonly PanaceaServices _core;
        private readonly TaskCompletionSource<bool> _source;

        public List<SecurityQuestion> Questions
        {
            get => _questions;
            set
            {
                _questions = value;
                OnPropertyChanged();
            }
        }

        public override async void Activate()
        {
            await LoadAsync();
        }

        public async Task LoadAsync()
        {
            try
            {
                var response = await _core.HttpClient.GetObjectAsync<List<SecurityQuestion>>(
                    "get_security_questions/",
                    allowCache: false
                    );
                if (response.Success)
                {
                    var lst = response.Result;
                    Questions = lst;
                }
                else
                {
                    if (_core.TryGetUiManager(out IUiManager ui))
                    {
                        var errors = new List<TranslatableObject>
                            {
                                new TranslatableObject("An unexpected error occured. Please try again later.",
                                    _translator)
                            };
                        await ui.ShowPopup(new RegisterErrorsViewModel() { Errors = errors }, popupType: PopupType.Error);
                    }
                    return;
                }
            }
            catch
            {
                if (_core.TryGetUiManager(out IUiManager ui))
                {
                    var errors = new List<TranslatableObject>
                            {
                                new TranslatableObject("An unexpected error occured. Please try again later.",
                                    _translator)
                            };
                    await ui.ShowPopup(new RegisterErrorsViewModel() { Errors = errors }, popupType: PopupType.Error);
                }

            }

        }

        public async Task RegisterAsync()
        {
            var args = new List<KeyValuePair<string, string>>()
            {

                new KeyValuePair<string, string>("first_name", FirstName),
                new KeyValuePair<string, string>("last_name", LastName),
                new KeyValuePair<string, string>("date_of_birth", DateOfBirth.ToString("yyyy-MM-dd")),
                new KeyValuePair<string, string>("security_answer", Answer),
                new KeyValuePair<string, string>("e_mail",Email),
                new KeyValuePair<string, string>("phonenumber", PhoneNumber),
                new KeyValuePair<string, string>("card", CardCode)
            };
            var required = new Dictionary<String, TranslatableObject>() {
                { "first_name" , new TranslatableObject("First Name", _translator) },
                { "last_name" , new TranslatableObject("Last Name", _translator)},
                { "security_answer" , new TranslatableObject("Security answer", _translator)},
                { "date_of_birth" , new TranslatableObject("Date of birth", _translator)}
            };
            var errors = (from pair in args where required.ContainsKey(pair.Key) where string.IsNullOrEmpty(pair.Value) select new TranslatableObject("{0} cannot be empty.", _translator, required[pair.Key].Text)).ToList();
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
            if (Question == null)
            {
                errors.Add(new TranslatableObject("You must select a security question."));
            }
            else
            {
                args.Add(new KeyValuePair<string, string>("security_question", Question.Question));
            }

            if (args.Take(4).Any(k => k.Value != null && string.IsNullOrWhiteSpace(k.Value.ToString())))
            {
                errors.Add(new TranslatableObject("Required fields cannot be empty."));
            }
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                if (errors.Count > 0)
                {
                    await ui.ShowPopup(new RegisterErrorsViewModel() { Errors = errors }, popupType: PopupType.Warning);
                    return;
                }
                try
                {
                    var res = await ui.DoWhileBusy(async () =>
                    {

                        var response = await _core.HttpClient.GetObjectAsync<RegisterResponse>(
                            "register_user/",
                            allowCache: false,
                            postData: args
                            );
                        if (response.Success)
                        {
                            var details = response.Result;
                            if(!await _core.UserService.LoginAsync(DateOfBirth, details.Password))
                            {
                                throw new Exception("Login failed");
                            }
                        }
                        return response;

                    });
                    if (res.Success)
                    {
                        var passPopup = new NewPasswordViewModel(res.Result.Password);
                        if (_core.TryGetUiManager(out ui))
                        {
                            await ui.ShowPopup(passPopup, new TranslatableObject("Registration was successful!", _translator).Text, PopupType.Success);
                            _source.TrySetResult(true);
                        }
                    }
                    else
                    {
                        errors = new List<TranslatableObject>()
                        {
                            new TranslatableObject("Email already exists.", _translator)
                        };
                        await ui.ShowPopup(new RegisterErrorsViewModel() { Errors = errors }, popupType: PopupType.Error);
                    }
                }
                catch
                {
                    errors = new List<TranslatableObject>
                        {
                            new TranslatableObject("Internet connection error has occured. Please try again  later.", _translator)
                        };
                    await ui.ShowPopup(new RegisterErrorsViewModel() { Errors = errors }, popupType: PopupType.Error);
                }
            }
        }
    }
}
