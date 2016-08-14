using Microsoft.Band;
using Microsoft.Band.Tiles;
using Microsoft.Band.Tiles.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Ft.Examples.Band.Timetracker.App.Tiles
{
	internal class TimerTrackerTile
	{
		private readonly PageLayout pageLayout;
		private readonly PageLayoutData pageLayoutData;
		
		private readonly FlowPanel panel = new FlowPanel();
		private readonly TextBlock textBlock = new TextBlock();
		private readonly FlowPanel panel2 = new FlowPanel();
		private readonly TextButton button = new TextButton();
		private readonly TextButton button2 = new TextButton();
		
		private readonly TextBlockData textBlockData = new TextBlockData(2, "Time");
		private readonly TextButtonData buttonData = new TextButtonData(4, "Start");
		private readonly TextButtonData button2Data = new TextButtonData(5, "Track");
		
		public TimerTrackerTile()
		{
			LoadIconMethod = LoadIcon;
			AdjustUriMethod = (uri) => uri;
			
			panel = new FlowPanel();
			panel.Orientation = FlowPanelOrientation.Vertical;
			panel.Rect = new PageRect(0, 0, 258, 128);
			panel.ElementId = 1;
			panel.Margins = new Margins(0, 0, 0, 0);
			panel.HorizontalAlignment = HorizontalAlignment.Left;
			panel.VerticalAlignment = VerticalAlignment.Top;
			
			textBlock = new TextBlock();
			textBlock.Font = TextBlockFont.Small;
			textBlock.Baseline = 0;
			textBlock.BaselineAlignment = TextBlockBaselineAlignment.Automatic;
			textBlock.AutoWidth = false;
			textBlock.ColorSource = ElementColorSource.Custom;
			textBlock.Color = new BandColor(255, 255, 255);
			textBlock.Rect = new PageRect(0, 0, 261, 32);
			textBlock.ElementId = 2;
			textBlock.Margins = new Margins(0, 0, 0, 0);
			textBlock.HorizontalAlignment = HorizontalAlignment.Left;
			textBlock.VerticalAlignment = VerticalAlignment.Top;
			
			panel.Elements.Add(textBlock);
			
			panel2 = new FlowPanel();
			panel2.Orientation = FlowPanelOrientation.Horizontal;
			panel2.Rect = new PageRect(0, 0, 261, 98);
			panel2.ElementId = 3;
			panel2.Margins = new Margins(0, 0, 0, 0);
			panel2.HorizontalAlignment = HorizontalAlignment.Left;
			panel2.VerticalAlignment = VerticalAlignment.Top;
			
			button = new TextButton();
			button.PressedColor = new BandColor(32, 32, 32);
			button.Rect = new PageRect(0, 0, 119, 55);
			button.ElementId = 4;
			button.Margins = new Margins(0, 8, 0, 0);
			button.HorizontalAlignment = HorizontalAlignment.Center;
			button.VerticalAlignment = VerticalAlignment.Top;
			
			panel2.Elements.Add(button);
			
			button2 = new TextButton();
			button2.PressedColor = new BandColor(32, 32, 32);
			button2.Rect = new PageRect(0, 0, 119, 55);
			button2.ElementId = 5;
			button2.Margins = new Margins(8, 8, 0, 0);
			button2.HorizontalAlignment = HorizontalAlignment.Center;
			button2.VerticalAlignment = VerticalAlignment.Top;
			
			panel2.Elements.Add(button2);
			
			panel.Elements.Add(panel2);
			pageLayout = new PageLayout(panel);
			
			PageElementData[] pageElementDataArray = new PageElementData[3];
			pageElementDataArray[0] = textBlockData;
			pageElementDataArray[1] = buttonData;
			pageElementDataArray[2] = button2Data;
			
			pageLayoutData = new PageLayoutData(pageElementDataArray);
		}
		
		public PageLayout Layout
		{
			get
			{
				return pageLayout;
			}
		}
		
		public PageLayoutData Data
		{
			get
			{
				return pageLayoutData;
			}
		}
		
		public Func<string, Task<BandIcon>> LoadIconMethod
		{
			get;
			set;
		}
		
		public Func<string, string> AdjustUriMethod
		{
			get;
			set;
		}
		
		private static async Task<BandIcon> LoadIcon(string uri)
		{
			StorageFile imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri));
			
			using (IRandomAccessStream fileStream = await imageFile.OpenAsync(FileAccessMode.Read))
			{
				WriteableBitmap bitmap = new WriteableBitmap(1, 1);
				await bitmap.SetSourceAsync(fileStream);
				return bitmap.ToBandIcon();
			}
		}
		
		public async Task LoadIconsAsync(BandTile tile)
		{
			await Task.Run(() => { }); // Dealing with CS1998
		}
		
		public static BandTheme GetBandTheme()
		{
			var theme = new BandTheme();
			theme.Base = new BandColor(51, 102, 204);
			theme.HighContrast = new BandColor(58, 120, 221);
			theme.Highlight = new BandColor(58, 120, 221);
			theme.Lowlight = new BandColor(49, 101, 186);
			theme.Muted = new BandColor(43, 90, 165);
			theme.SecondaryText = new BandColor(137, 151, 171);
			return theme;
		}
		
		public static BandTheme GetTileTheme()
		{
			var theme = new BandTheme();
			theme.Base = new BandColor(51, 102, 204);
			theme.HighContrast = new BandColor(58, 120, 221);
			theme.Highlight = new BandColor(58, 120, 221);
			theme.Lowlight = new BandColor(49, 101, 186);
			theme.Muted = new BandColor(43, 90, 165);
			theme.SecondaryText = new BandColor(137, 151, 171);
			return theme;
		}
		
		public class PageLayoutData
		{
			private readonly PageElementData[] array;
			
			public PageLayoutData(PageElementData[] pageElementDataArray)
			{
				array = pageElementDataArray;
			}
			
			public int Count
			{
				get
				{
					return array.Length;
				}
			}
			
			public T Get<T>(int i) where T : PageElementData
			{
				return (T)array[i];
			}
			
			public T ById<T>(short id) where T:PageElementData
			{
				return (T)array.FirstOrDefault(elm => elm.ElementId == id);
			}
			
			public PageElementData[] All
			{
				get
				{
					return array;
				}
			}
		}
		
	}
}
