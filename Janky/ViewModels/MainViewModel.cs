using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using Janky.Properties;
using Janky.Service;
using System.Windows.Shell;

namespace Janky.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private TaskbarItemInfo _taskbar;

        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { Set(() => IsBusy, ref _isBusy, value); }
        }

        private bool _experiencingErrors;
        public bool ExperiencingErrors
        {
            get { return _experiencingErrors; }
            set { Set(() => ExperiencingErrors, ref _experiencingErrors, value); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { Set(() => ErrorMessage, ref _errorMessage, value); }
        }


        private DateTime _lastUpdated;
        public DateTime LastUpdated
        {
            get { return _lastUpdated; }
            set { Set(() => LastUpdated, ref _lastUpdated, value); }
        }

        private JenkinsStatusService _service;
        private DispatcherTimer _timer = new DispatcherTimer();

        private ThreadSafeObservableCollection<ShortJobStatus> _jobs = new ThreadSafeObservableCollection<ShortJobStatus>();
        public ThreadSafeObservableCollection<ShortJobStatus> Jobs
        {
            get { return _jobs; }
            set { Set(() => Jobs, ref _jobs, value); }
        }

        public MainViewModel(TaskbarItemInfo taskbar)
        {
            _taskbar = taskbar;
            _service = new JenkinsStatusService(Settings.Default.ServiceBaseLocation);

            Refresh();

            int delay = Properties.Settings.Default.UpdateDelayInMinutes;
            if (delay <= 0)
                delay = 5;

            
            _timer.Interval = TimeSpan.FromMinutes(delay);
            _timer.Tick += (sender, args) => Refresh();
            _timer.Start();
        }


        private async void Refresh()
        {
            IsBusy = true;
            SetTaskbarWorking();

            await Task.Run(() => UpdateTaskList());
            
            IsBusy = false;
            UpdateBuildHealth();
            LastUpdated = DateTime.Now;
        }

        private void UpdateTaskList()
        {
            try
            {
                // For effect..
                System.Threading.Thread.Sleep(1500);

                var stats = _service.GetServerStats();

                if(Jobs.Count > 0)
                    Jobs.Clear();

                foreach (var steve in stats.Jobs.Where(x => x.Color != "disabled"))
                {
                    var item = _service.GetJobStatus(steve.Name);
                    item.Job = steve;

                    Jobs.Add(item);
                    System.Threading.Thread.Sleep(200);
                }

                ExperiencingErrors = false;
            }
            catch (Exception ex)
            {
                SetError(ex);
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void SetError(Exception ex)
        {
            ErrorMessage = ex.Message;

            if (ex is ArgumentException)
                ErrorMessage += " Check your configuration to make sure the service location is set correctly.";

            ExperiencingErrors = true;
            SetTaskbarOverlay(OverlayState.Error);
        }

        private void UpdateBuildHealth()
        {
            bool ok = Jobs.All(j => j.DidSucceed);
            SetTaskbarCondition(ok);

            //bool ood = Jobs.All(j => j.)
        }

        private void SetTaskbarWorking()
        {
            try
            {
                SetTaskbarOverlay(OverlayState.None);
                _taskbar.ProgressState = TaskbarItemProgressState.Indeterminate;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Taskbar down " + ex.Message);
            }
        }

        private void SetTaskbarCondition(bool isNormal)
        {
            try
            {
                _taskbar.ProgressValue = 1;
                _taskbar.ProgressState = isNormal ? NormalState : TaskbarItemProgressState.Error;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Taskbar down " + ex.Message);
            }

        }

        private void SetTaskbarOverlay(OverlayState state)
        {
            try
            {
                switch (state)
                {
                    case OverlayState.None:
                        _taskbar.Overlay = null;
                        break;
                    case OverlayState.Error:
                        _taskbar.Overlay = new BitmapImage(new Uri("pack://application:,,,/Janky;component/Assets/skull.png"));
                        break;
                    case OverlayState.OutOfDate:
                        _taskbar.Overlay = new BitmapImage(new Uri("pack://application:,,,/Janky;component/Assets/exclaimation_mark.png"));
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Taskbar down " + ex.Message);
            }
        }



        private TaskbarItemProgressState NormalState
        {
            get
            {
                return Settings.Default.OnlyShowFailNotification ? TaskbarItemProgressState.None : TaskbarItemProgressState.Normal;
            }
        }

        private enum OverlayState
        {
            None,
            Error,
            OutOfDate
        }
    }
}
