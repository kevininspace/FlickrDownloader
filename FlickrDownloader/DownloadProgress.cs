////-----------------------------------------------------------------------
//// <copyright file="DownloadProgress.cs" company="Sondre Bjellås">
//// This software is licensed as Microsoft Public License (Ms-PL).
//// </copyright>
////-----------------------------------------------------------------------

//using System;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.IO;
//using System.Net;
//using FlickrNet;
//using System.Threading;

//namespace FlickrDownloader
//{
//    public partial class DownloadProgress : UserControl, IWizardPage
//    {
//        public DownloadProgress()
//        {
//            //InitializeComponent();

//            _mtx = new Mutex(true, "DownloadMutex");

//        }

//        private static Mutex _mtx;
//        public static ManualResetEvent DownloadHandler = new ManualResetEvent(false);

//        public Bitmap Icon
//        {
//            get { return Properties.Resources.Hard_Drive; }
//        }

//        public event NextDelegate Next;

//        public void OnNext()
//        {
//            if (Next != null)
//                Next(this, false);
//        }

//        public void ReloadPage()
//        {

//            MessageBox.Show("Reload page");

//        }

//        public void StartDownload()
//        {
//            ResetDownloadUI();

//            DirectoryInfo dir = new DirectoryInfo(PhotoService.Instance.SearchOptions.Path);
//            if (!dir.Exists)
//                dir.Create();

//            backgroundWorker.RunWorkerAsync();
//        }

//        private void ResetDownloadUI()
//        {
//            progressBar1.Value = 0;
//            progressBar1.Enabled = true;

//            buttonPause.Visible = true;
//            buttonCancel.Visible = true;

//            UpdateStatus("Downloading...");
//            labelCompleted.Visible = false;
//            linkLabel.Visible = false;
//        }

//        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
//        {
//            progressBar1.Enabled = false;

//            buttonPause.Visible = false;
//            buttonCancel.Visible = false;

//            UpdateStatus(Properties.Resources.DownloadsCompleted);
//            labelCompleted.Visible = true;

//            linkLabel.Text = PhotoService.Instance.SearchOptions.Path;
//            linkLabel.Visible = true;
//        }

//        private bool _cancelDownload = false;
//        private bool _pauseDownload = false;

//        public void CancelDownload()
//        {
//            backgroundWorker.CancelAsync();
//            UpdateStatus(Properties.Resources.DownloadsCanceled);
//            _cancelDownload = true;
//        }

//        public void PauseDownload()
//        {
//            if (!_pauseDownload)
//            {
//                // This is additionally done in the download thread, this is done
//                // twice to give the user more direct feedback and both places because
//                // an existing download will complete (and update progress will be reported)
//                // even when the user pauses or cancels.
//                UpdateStatus(Properties.Resources.DownloadsPaused);
//                buttonPause.Text = "&Resume";
//                _pauseDownload = true;
//            }
//            else
//            {
//                buttonPause.Text = "&Pause";
//                _pauseDownload = false;
//                _mtx.ReleaseMutex();
//            }
//        }

//        public string Title
//        {
//            get { return Properties.Resources.PageProgressTitle; }
//        }
//        public string Description
//        {
//            get { return Properties.Resources.PageProgressDescription; }
//        }

//        private void buttonPause_Click(object sender, EventArgs e)
//        {
//            PauseDownload();
//        }

//        private void buttonCancel_Click(object sender, EventArgs e)
//        {
//            CancelDownload();
//        }

//        long _totalPhotos;

//        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
//        {
//            if (e.Cancel)
//                return;

//            WebClient client = new WebClient();
//            string path = PhotoService.Instance.SearchOptions.Path;

//            if (PhotoService.Instance.SearchOptions.Type == SearchType.Selection)
//            {
//                long index = 0;
//                _totalPhotos = (long)PhotoService.Instance.SelectedPhotos.Length;
//                ProcessPhotos(PhotoService.Instance.SelectedPhotos, ref index, client, path);
//            }
//            else
//            {
//                bool creativeCommonsOnly = true;

//                if (Program.Debug)
//                {
//                    creativeCommonsOnly = false;
//                }

//                var photos = PhotoService.Instance.Search(1, 20, creativeCommonsOnly);

//                long totalPages = photos.Pages;

//                long totalPhotos = photos.TotalPhotos;
//                _totalPhotos = totalPhotos;

//                long index = 0;

//                for (int i = 0; i < totalPages; i++)
//                {
//                    if (_cancelDownload)
//                    {
//                        return;
//                    }

//                    if (i > 0)
//                    {
//                        photos = PhotoService.Instance.Search(i + 1, 20, creativeCommonsOnly);
//                    }

//                    ProcessPhotos(photos.PhotoCollection, ref index, client, path);
//                }
//            }

//            backgroundWorker.ReportProgress(100);

//        }

//        private void ProcessPhotos(Photo[] photos, ref long index, WebClient client, string path)
//        {
//            foreach (var photo in photos)
//            {
//                if (_cancelDownload)
//                {
//                    UpdateStatus(Properties.Resources.DownloadsCanceled);
//                    UpdateProgress(0);
//                    UpdateProgressState(TaskbarProgressBarState.NoProgress);
//                    return;
//                }

//                if (_pauseDownload)
//                {
//                    UpdateStatus(Properties.Resources.DownloadsPaused);
//                    UpdateProgressState(TaskbarProgressBarState.Paused);
//                    _mtx.WaitOne();
//                    UpdateStatus(Properties.Resources.DownloadsResume);
//                    UpdateProgressState(TaskbarProgressBarState.Normal);
//                }

//                index++;
//                int percentage = (int)(100 * index / _totalPhotos);

//                FlickrNet.Sizes sizes = FlickrFactory.GetInstance().PhotosGetSizes(photo.PhotoId);
//                FlickrNet.Size size = sizes.SizeCollection[sizes.SizeCollection.Length - 1];

//                string fileName = size.Source.Substring(size.Source.LastIndexOf("/") + 1);

//                if (photo.Media == "photo")
//                {
//                    string localPath = Path.Combine(path, fileName);

//                    if (File.Exists(localPath))
//                    {

//                    }

//                    client.DownloadFile(size.Source, localPath);


//                    // We want to avoid any calculations problems and report 100% progress before we are truly finished.
//                    if (percentage < 100)
//                    {
//                        backgroundWorker.ReportProgress(percentage, index);
//                    }
//                }
//            }
//        }

//        public delegate void UpdateStatusTextHandler(string text);
//        public delegate void UpdateProgressPercentageHandler(int progressPercentage);
//        public delegate void UpdateProgressStateHandler(TaskbarProgressBarState state);

//        private void UpdateStatus(string text)
//        {
//            if (this.InvokeRequired)
//            {
//                this.Invoke(new UpdateStatusTextHandler(UpdateStatus), text);
//            }

//            labelStatus.Text = text;
//        }

//        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
//        {
//            UpdateStatus(string.Format(Properties.Resources.DownloadStatus, e.UserState, _totalPhotos));
//            UpdateProgress(e.ProgressPercentage);
//        }

//        private void UpdateProgressState(TaskbarProgressBarState progressState)
//        {
//            if (this.InvokeRequired)
//            {
//                this.Invoke(new UpdateProgressStateHandler(UpdateProgressState), progressState);
//            }

//            if (Program.IsWindows7())
//            {
//                TaskbarManager.Instance.SetProgressState(progressState);
//            }
//        }

//        private void UpdateProgress(int progressPercentage)
//        {
//            if (this.InvokeRequired)
//            {
//                this.Invoke(new UpdateProgressPercentageHandler(UpdateProgress), progressPercentage);
//            }

//            progressBar1.Value = progressPercentage;

//            if (Program.IsWindows7())
//            {
//                // Windows 7: Update the progress in the taskbar.
//                TaskbarManager.Instance.SetProgressValue(progressPercentage, 100);
//            }
//        }



//        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
//        {
//            Process.Start(linkLabel.Text);
//        }
//    }
//}