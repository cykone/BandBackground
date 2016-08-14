using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Storage;
using Ft.Examples.Band.Timetracker.Background.Model;
using Microsoft.Band;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;

namespace Ft.Examples.Band.Timetracker.Background
{
    public sealed class BackgroundService : IBackgroundTask
    {
        // private const string CurrentTrackingKey = "CurrentTrackingKey";

        private BackgroundTaskDeferral _deferral;

        private AppServiceConnection _appServiceConnection;

        private IBandClient _bandClient;

        private TimeTrackerModel _current;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            this._deferral = taskInstance.GetDeferral();

            taskInstance.Canceled += this.OnBackgroundServiceCanceled;

            BackgroundTileEventHandler.Instance.TileOpened += this.OnTimetrackerTileOpened;
            BackgroundTileEventHandler.Instance.TileClosed += this.OnTimetrackerTileClosed;
            BackgroundTileEventHandler.Instance.TileButtonPressed += this.OnTimetracketTileButtonPressed;

            var details = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            if (details != null)
            {
                this._appServiceConnection = details.AppServiceConnection;
                this._appServiceConnection.RequestReceived += this.OnRequestReceived;
            }
        }

        private async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var requestDeferral = args.GetDeferral();
            BackgroundTileEventHandler.Instance.HandleTileEvent(args.Request.Message);
            await args.Request.SendResponseAsync(new ValueSet());
            requestDeferral.Complete();
        }

        private async void OnTimetracketTileButtonPressed(object sender, BandTileEventArgs<IBandTileButtonPressedEvent> e)
        {
            try
            {
                this.ConnectToBand();

                if (e.TileEvent.ElementId == 4)
                {
                    if (this._current == null)
                    {
                        this._current = this.GetCurrent();
                    }

                    if (!this._current.IsStarted)
                    {
                        this._current.Start();
                        await this.UpdateText("Pause", "Started");
                    }
                    else if (this._current.IsPaused)
                    {
                        this._current.Resume();
                        await this.UpdateText("Pause", "Resumed");
                    }
                    else
                    {
                        this._current.Pause();
                        await this.UpdateText("Continue", "Paused");
                    }
                }

                if (e.TileEvent.ElementId == 5)
                {
                    this._current.FinishTracking();
                    await this.UpdateText("Start", "Worked hours: " + this._current.CalculateWorkedHours());
                    this.ClearCurrent();

                    this._current = null;
                }
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                this.DisconnectFromBand();
            }
        }

        private async Task UpdateText(string startPauseButtonText, string textField)
        {
            await this._bandClient.TileManager.SetPagesAsync(new Guid(TimetrackerTileConstants.TileId), new PageData(new Guid(TimetrackerTileConstants.PageId), 0,
                          new TextButtonData(4, startPauseButtonText),
                          new TextButtonData(5, "Track"),
                          new TextBlockData(2, textField)));
        }

        private void OnTimetrackerTileClosed(object sender, BandTileEventArgs<IBandTileClosedEvent> e)
        {
            this.UpdateCurrent(this._current);
        }

        private void OnTimetrackerTileOpened(object sender, BandTileEventArgs<IBandTileOpenedEvent> e)
        {
            this._current = this.GetCurrent();
        }

        private void OnBackgroundServiceCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            this.DisconnectFromBand();
            this._deferral?.Complete();
        }

        private void ConnectToBand()
        {
            var pairedBands = BandClientManager.Instance.GetBandsAsync(isBackground: true).Result;
            this._bandClient = BandClientManager.Instance.ConnectAsync(pairedBands[0]).Result;
        }

        private void DisconnectFromBand()
        {
            if (this._bandClient != null)
            {
                this._bandClient.Dispose();
                this._bandClient = null;
            }
        }

        private bool HasCurrent()
        {
            return ApplicationData.Current.LocalSettings.Values.ContainsKey("CurrentTrackingKey");
        }

        private void ClearCurrent()
        {
            ApplicationData.Current.LocalSettings.Values.Remove("CurrentTrackingKey");
        }

        private TimeTrackerModel GetCurrent()
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("CurrentTrackingKey"))
            {
                return TimeTrackerModel.FromJson((string)ApplicationData.Current.LocalSettings.Values["CurrentTrackingKey"]);
            }

            return new TimeTrackerModel();
        }

        private void UpdateCurrent(TimeTrackerModel current)
        {
            if (current == null)
            {
                return;
            }

            if (this.HasCurrent())
            {
                this.ClearCurrent();
            }

            var jsonData = TimeTrackerModel.ToJson(current);
            ApplicationData.Current.LocalSettings.Values.Add("CurrentTrackingKey", jsonData);
        }
    }
}