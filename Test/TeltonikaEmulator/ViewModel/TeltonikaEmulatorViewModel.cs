using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using TeltonikaEmulator.Annotations;
using TeltonikaEmulator.Encoding;
using TeltonikaEmulator.Helpers;
using TeltonikaEmulator.Models;
using TeltonikaEmulator.Parsers;

namespace TeltonikaEmulator.ViewModel
{
    public class TeltonikaEmulatorViewModel : INotifyPropertyChanged
    {
        #region fields
        private string _fileName;
        private int _boxNumber;
        private string _imei;
        private SourceOfIMEIs _sourceOfImeIs;
        private decimal _sleepPeriod;
        private string _ipAddress;
        private int _port;
        private DeepObservableCollection<LogVm> _logs;
        CancellationTokenSource _cts;
        private readonly string[] _file;
        #endregion

        #region ctor

        public TeltonikaEmulatorViewModel()
        {
            LoadDialogButton = new RelayCommand(LoadDialogFile);
            EmulateButton = new RelayCommand(Emulate);
            CancelButton = new RelayCommand(CancelTasks);
            _boxNumber = 1;
            _sleepPeriod = 1;
            _ipAddress = "127.0.0.1";
            _port = 34400;
            Logs = new DeepObservableCollection<LogVm>();
             _file = ResourFiles.Resource._19_05_07_352093082435032?.Split('\r');
        }

        private void CancelTasks()
        {
                _cts?.Cancel() ;
        }

        #endregion

        #region methods

        private void Emulate()
        {
            _cts = new CancellationTokenSource();
            if (string.IsNullOrEmpty(_fileName) &&  _file.Length ==0)
            {
                MessageBox.Show("Le chemin vers le fichier log est vide", "Attention", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            Logs.Clear();
            // ReSharper disable once ComplexConditionExpression
            var t = new Task(() =>
            {
                var config = new EmulationConfig
                {
                    BoxNumber = _boxNumber,
                    IpAddress = _ipAddress,
                    Port = _port,
                    SleepPeriod = _sleepPeriod * 1000,
                    SourceOfImeIs = _sourceOfImeIs
                };
                if (_sourceOfImeIs == SourceOfIMEIs.OneIMEI)
                {
                    if (string.IsNullOrEmpty(_imei))
                    {
                        MessageBox.Show("Le champ IMEI est requis", "Attention", MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }

                    config.IMEIs = new[] {_imei};
                }
                TeltonikaPacketEncoder.UpdateLogDataGird += log =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (!Logs.Any(x => x.Equals(log)))
                            Logs.Add(log);
                    });
                };
                // extracting and parsing and encoding  avl data. 
                var encodedData = (!string.IsNullOrEmpty(_fileName))?
                    TeltonikaPacketEncoder.Encoding(AvlDataParser.ParseAvlData(_fileName)):
                    TeltonikaPacketEncoder.Encoding(AvlDataParser.ParseAvlData(_file));
                // launch the emulation
                var emulator = new Emulator();
                emulator.UpdateLogDataGird += log =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (!Logs.Any(x => x.Equals(log)))
                            Logs.Add(log);
                    });
                };

                emulator.Emulate(config, encodedData, _cts.Token);

            });
            t.Start();

        }

        private void LoadDialogFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() != true) return;
            FileName = openFileDialog.FileName;
        }


        #endregion
        #region    Utilities


        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                OnPropertyChanged(nameof(FileName));
            }
        }


        public int BoxNumber
        {
            get { return _boxNumber; }
            set
            {
                _boxNumber = value;
                OnPropertyChanged(nameof(BoxNumber));
            }
        }

        public string Imei
        {
            get { return _imei; }
            set
            {
                _imei = value;
                OnPropertyChanged(nameof(Imei));
            }
        }

        public SourceOfIMEIs SourceOfImeIs
        {
            get { return _sourceOfImeIs; }
            set
            {
                _sourceOfImeIs = value;
                OnPropertyChanged(nameof(SourceOfImeIs));
            }
        }

        public decimal SleepPeriod
        {
            get { return _sleepPeriod; }
            set
            {
                _sleepPeriod = value;
                OnPropertyChanged(nameof(SleepPeriod));
            }
        }

        public string IpAddress
        {
            get { return _ipAddress; }
            set
            {
                _ipAddress = value;
                OnPropertyChanged(IpAddress);
            }
        }

        public int Port
        {
            get { return _port; }
            set
            {
                _port = value;
                OnPropertyChanged(nameof(Port));
            }
        }
        public ICommand EmulateButton { get; private set; }

        public ICommand LoadDialogButton { get; private set; }
        public ICommand CancelButton { get; private set; }
        

        public DeepObservableCollection<LogVm> Logs
        {
            get { return _logs; }
            set
            {
                _logs = value;
                OnPropertyChanged(nameof(Logs));
            }
        }




        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
