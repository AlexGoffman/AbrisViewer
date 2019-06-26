using System.Collections.ObjectModel;

using WpfApp1.Model;
using WpfApp1.Behavior;
using System.Windows;

namespace WpfApp1.ViewModel
{
    class AbrisViewModel : ViewModelBase
    {
        #region Закрытые поля
        AbrisDataContext _abrisDataContext;
        //Коллекция записей таблицы
        private ObservableCollection<AbrisRecord> _abrisRecords;
        private string _userName = string.Empty;
        private string _password = string.Empty;
        private string _phpsessid = string.Empty;
        private string _tableMetadata = string.Empty;
        private string _tableName = string.Empty;
        //Команды
        private RelayCommand _logInCommand;
        private RelayCommand _requestTableMetaDataCommand;
        #endregion Закрытые поля

        #region Свойства
        /// <summary>
        ///  Свойство, отвечающее за коллекцию записей из таблицы
        /// </summary>
        public ObservableCollection<AbrisRecord> AbrisRecords
        {
            get
            {
                return _abrisRecords;
            }
            set
            {
                _abrisRecords = value;
                OnPropertyChanged(nameof(AbrisRecords));
            }
        }

        /// <summary>
        ///  Свойство, отвечающее за логин для входа
        /// </summary>
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }

        /// <summary>
        ///  Свойство, отвечающее за пароль для входа
        /// </summary>
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        /// <summary>
        ///  Метаданные с сервера
        /// </summary>
        public string TableMetadata
        {
            get
            {
                return _tableMetadata;
            }
            set
            {
                _tableMetadata = value;
                //_tableMetadata = MetaDataConverter.Convert(_tableMetadata);
                OnPropertyChanged(nameof(TableMetadata));
            }
        }

        /// <summary>
        ///  Название таблицы для вывода
        /// </summary>
        public string TableName
        {
            get
            {
                return _tableName;
            }
            set
            {
                _tableName = value;
                OnPropertyChanged(nameof(TableName));
            }
        }

        #endregion Свойства

        #region Команды 

        public RelayCommand LogInCommand
        {
            get
            {
                return _logInCommand ?? (_logInCommand = new RelayCommand(obj =>
                {
                    if(_abrisDataContext.RequestWithLogin(UserName, Password))
                    {
                        MessageBox.Show("Авторизация успешна!!");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка авторизации!!\n Неверный логин или пароль, повторите попытку снова.");
                    }
                }));
            }
        }

        public RelayCommand RequestTableMetaDataCommand
        {
            get
            {
                return _requestTableMetaDataCommand ?? (_requestTableMetaDataCommand = new RelayCommand(obj =>
                {
                    TableMetadata = _abrisDataContext.RequestForMetaData(TableName);
                }));
            }
        }

        #endregion Команды

        public AbrisViewModel()
        {
            _abrisDataContext = new AbrisDataContext();
        }
    }
}
