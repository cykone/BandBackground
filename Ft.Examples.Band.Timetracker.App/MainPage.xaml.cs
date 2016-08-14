using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Ft.Examples.Band.Timetracker.App.Tiles;
using Ft.Examples.Band.Timetracker.Background;
using Microsoft.Band;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Ft.Examples.Band.Timetracker.App
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static Guid TileId = new Guid(TimetrackerTileConstants.TileId);

        private static Guid PageId = new Guid(TimetrackerTileConstants.PageId);

        private IBandClient _bandClient;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await this.ConnectToBandAsync();
        }

        private async Task ConnectToBandAsync()
        {
            var pairedBands = await BandClientManager.Instance.GetBandsAsync();
            var firstConnectedBand = pairedBands.FirstOrDefault();
            if (firstConnectedBand == null)
            {
                this.CreateButton.IsEnabled = false;
                this.ConnectToBandButton.IsEnabled = true;
                return;
            }

            this._bandClient = await BandClientManager.Instance.ConnectAsync(firstConnectedBand);
            if (this._bandClient != null)
            {
                this.CreateButton.IsEnabled = true;
                this.ConnectToBandButton.IsEnabled = false;

                if (this._bandClient.TileManager.TileInstalledAndOwned(TileId, CancellationToken.None))
                {
                    // Forground events
                    this._bandClient.TileManager.TileOpened += this.OnOpened;
                    this._bandClient.TileManager.TileButtonPressed += this.OnTileButtonPressed;
                    this._bandClient.TileManager.TileClosed += this.OnClosed;

                    // Start listening for foreground
                    await this._bandClient.TileManager.StartReadingsAsync();

                    // Register for the background events.
                    await this._bandClient.SubscribeToBackgroundTileEventsAsync(TileId);
                }
            }
        }

        private void OnTileButtonPressed(object sender, BandTileEventArgs<IBandTileButtonPressedEvent> e)
        {

        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            await this._bandClient.TileManager.StopReadingsAsync();
            this._bandClient?.Dispose();
        }

        private void OnClosed(object sender, BandTileEventArgs<IBandTileClosedEvent> e)
        { // Currently not used
        }

        private void OnOpened(object sender, BandTileEventArgs<IBandTileOpenedEvent> e)
        { // Currently not used
        }

        public async void OnCreateTileButtonTapped(object sender, TappedRoutedEventArgs tappedRoutedEventArgs)
        {
            // Check for existing tile and remove it. Just for demo purpuse
            if (this._bandClient.TileManager.TileInstalledAndOwned(TileId, CancellationToken.None))
            {
                await this._bandClient.TileManager.RemoveTileAsync(TileId);
                // await this._bandClient.TileManager.StopReadingsAsync();
            }

            var existingTiles = await this._bandClient.TileManager.GetTilesAsync();
            var timetrackerTile = existingTiles.FirstOrDefault(x => x.TileId == TileId);
            if (timetrackerTile == null && await this._bandClient.TileManager.GetRemainingTileCapacityAsync() > 0)
            {
                var timeTrackerTileDesign = new TimerTrackerTile();
                timetrackerTile = new BandTile(TileId)
                {
                    Name = TimetrackerTileConstants.TileTitle,
                    TileIcon = await this.LoadIconAsync("ms-appx:///Assets/LargeIcon.png"),
                    SmallIcon = await this.LoadIconAsync("ms-appx:///Assets/SmallIcon.png")
                };

                timetrackerTile.PageLayouts.Add(timeTrackerTileDesign.Layout);

                await timeTrackerTileDesign.LoadIconsAsync(timetrackerTile);
                await this._bandClient.TileManager.AddTileAsync(timetrackerTile);
                await this._bandClient.TileManager.SetPagesAsync(TileId, new PageData(PageId, 0, timeTrackerTileDesign.Data.All));
            }

            if (this._bandClient.TileManager.TileInstalledAndOwned(TileId, CancellationToken.None))
            {
                // Forground events
                this._bandClient.TileManager.TileOpened += this.OnOpened;
                this._bandClient.TileManager.TileButtonPressed += this.OnTileButtonPressed;
                this._bandClient.TileManager.TileClosed += this.OnClosed;

                // Start listening for foreground
                await this._bandClient.TileManager.StartReadingsAsync();

                // Register for the background events.
                await this._bandClient.SubscribeToBackgroundTileEventsAsync(TileId);
            }
        }

        private async Task<BandIcon> LoadIconAsync(string uri)
        {
            var imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri));
            using (var fileStream = await imageFile.OpenAsync(FileAccessMode.Read))
            {
                var bitmap = new WriteableBitmap(1, 1);
                await bitmap.SetSourceAsync(fileStream);
                return bitmap.ToBandIcon();
            }
        }

        private async void OnConnectBandTapped(object sender, TappedRoutedEventArgs e)
        {
            await this.ConnectToBandAsync();
        }
    }
}