using CurlingScheduler.Model;
using CurlingScheduler.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace CurlingScheduler.Ui.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private RelayCommand _generateSchedule;
        private RelayCommand _openFile;
        private RelayCommand _saveFile;
        private RelayCommand _saveFileAs;
        private RelayCommand _exitProgram;

        private ObservableCollection<string> _availableDrawAlignment =
            new ObservableCollection<string>(new string[] { "Balanced", "Squished" });

        private string _drawAlignment = "Squished";
        private string _teamsText = string.Empty;
        private string _gameSchedule = string.Empty;
        private string _stoneSchedule = string.Empty;

        private string _currentFilename = string.Empty;

        private int _sheetCount = 5;
        private int _stoneCount = 4;
        private int _weekCount = 7;
        private int _drawCount = 1;
        private int _drawCountMinimum = 1;

        private bool _balanceStones = true;

        private IEnumerable<string> _teams = new string[] { "" };

        private ScheduleCreator _scheduleCreator;
        private ConfigurationManager _configManager;

        public MainViewModel(
            ScheduleCreator scheduleCreator,
            ConfigurationManager configManager)
        {
            _scheduleCreator = scheduleCreator;
            _configManager = configManager;
        }

        public RelayCommand GenerateSchedule => _generateSchedule ?? (_generateSchedule = new RelayCommand(() =>
        {
            var alignment = (DrawAlignment)Enum.Parse(typeof(DrawAlignment), DrawAlignment);

            (GameSchedule, StoneSchedule) = _scheduleCreator.CreateSchedule(_teams, SheetCount, StoneCount, DrawCount, WeekCount, alignment, BalanceStones);
        }));

        public RelayCommand OpenFile => _openFile ?? (_openFile = new RelayCommand(() =>
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "txt files (*.txt)|*.txt";
            var result = dialog.ShowDialog();

            if (result.Value == true)

            {
                _currentFilename = dialog.FileName;

                LoadData();
            }
        }));

        public RelayCommand SaveFile => _saveFile ?? (_saveFile = new RelayCommand(() =>
        {
            if (string.IsNullOrEmpty(_currentFilename))
            {
                SaveFileAs.Execute(null);
                return;
            }

            var confirmation =
                MessageBox.Show(
                    $"Save Configuration data to {_currentFilename}?",
                    "Confirm Save",
                    MessageBoxButton.YesNo);
            if (confirmation == MessageBoxResult.Yes)
            {
                SaveData();
            }
        }));

        public RelayCommand SaveFileAs => _saveFileAs ?? (_saveFileAs = new RelayCommand(() =>
        {
            var savedialog = new SaveFileDialog();
            savedialog.Filter = "txt files (*.txt)|*.txt";
            var result = savedialog.ShowDialog();

            if (result.Value == true)
            {
                _currentFilename = savedialog.FileName;

                SaveData();
            }
        }));

        public RelayCommand ExitProgram => _exitProgram ?? (_exitProgram = new RelayCommand(() =>
        {
            Application.Current.Shutdown();
        }));

        private void SaveData()
        {
            var config = new Configuration
            {
                Teams = _teams.ToArray(),
                WeekCount = _weekCount,
                DrawCount = _drawCount,
                DrawAlignment = _drawAlignment,
                SheetCount = _sheetCount,
                StoneCount = _stoneCount
            };

            _configManager.SaveConfiguration(_currentFilename, config);
        }

        private void LoadData()
        {
            var config = _configManager.LoadConfiguration(_currentFilename);

            TeamsText = config.Teams.Aggregate((a, b) => ($"{a}{Environment.NewLine}{b}"));
            WeekCount = config.WeekCount;
            StoneCount = config.StoneCount;
            DrawCount = config.DrawCount;
            DrawAlignment = config.DrawAlignment;
            SheetCount = config.SheetCount;
        }

        private void UpdateDrawCountMinimum()
        {
            var teamCount = _teams.Count();

            var notEven = teamCount % (2 * StoneCount) != 0;

            DrawCountMinimum = teamCount / (2 * StoneCount) + (notEven ? 1 : 0);

            if (DrawCount < DrawCountMinimum)
            {
                DrawCount = DrawCountMinimum;
            }
        }

        private void UpdateStoneCountMaximum()
        {
            if (StoneCount > SheetCount)
            {
                StoneCount = SheetCount;
            }
        }

        public ObservableCollection<string> AvailableDrawAlignment
        {
            get => _availableDrawAlignment;
            set => Set(() => AvailableDrawAlignment, ref _availableDrawAlignment, value);
        }

        public string DrawAlignment
        {
            get => _drawAlignment;
            set => Set(() => DrawAlignment, ref _drawAlignment, value);
        }

        public string TeamsText
        {
            get => _teamsText;
            set
            {
                Set(() => TeamsText, ref _teamsText, value);

                _teams = TeamsText.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                                  .ToHashSet();

                UpdateDrawCountMinimum();
            }
        }

        public string GameSchedule
        {
            get => _gameSchedule;
            set => Set(() => GameSchedule, ref _gameSchedule, value);
        }

        public string StoneSchedule
        {
            get => _stoneSchedule;
            set => Set(() => StoneSchedule, ref _stoneSchedule, value);
        }

        public int SheetCount
        {
            get => _sheetCount;
            set
            {
                Set(() => SheetCount, ref _sheetCount, value);
                UpdateStoneCountMaximum();
                UpdateDrawCountMinimum();
            }
        }

        public int StoneCount
        {
            get => _stoneCount;
            set
            {
                Set(() => StoneCount, ref _stoneCount, value);
                UpdateDrawCountMinimum();
            }
        }

        public int WeekCount
        {
            get => _weekCount;
            set => Set(() => WeekCount, ref _weekCount, value);
        }

        public int DrawCount
        {
            get => _drawCount;
            set => Set(() => DrawCount, ref _drawCount, value);
        }

        public int DrawCountMinimum
        {
            get => _drawCountMinimum;
            set => Set(() => DrawCountMinimum, ref _drawCountMinimum, value);
        }

        public bool BalanceStones
        {
            get => _balanceStones;
            set => Set(() => BalanceStones, ref _balanceStones, value);
        }
    }
}