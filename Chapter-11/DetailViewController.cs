﻿using System;
using System.Collections.Generic;

using UIKit;
using Foundation;

namespace Presidents
{
	public partial class DetailViewController : UIViewController, IUIPopoverControllerDelegate
	{
		UIBarButtonItem languageButton = null;
		UIPopoverController languagePopoverController = null;
		string languageString = "";

		public object DetailItem { get; set; }    // TODO: change the type from object to what it's supposed to be

		public string LanguageString 
		{ 
			get{ return languageString; }
			set
			{ 
				languageString = value; 
				ConfigureView ();
				if (languagePopoverController != null) 
				{
					languagePopoverController.Dismiss (true);
					languagePopoverController = null;
				}
			}
		}

		public DetailViewController (IntPtr handle) : base (handle)
		{
		}

		public void SetDetailItem (object newDetailItem)
		{
			if (DetailItem != newDetailItem) {
				DetailItem = newDetailItem;
				
				// Update the view
				ConfigureView ();
			}
		}
			

		void ConfigureView ()
		{
			// Update the user interface for the detail item
			if (IsViewLoaded && DetailItem != null) {
				var presDict = DetailItem as Dictionary<string, string>;

				// Display the president's name
				Title = presDict ["name"]; 

				// Modify the URL for the selected language
				var urlString = ModifyUrlForlanguage(presDict["url"], LanguageString);

				// Display the Wkikpedia URL for the selected president
				detailDescriptionLabel.Text = urlString;

				// Display the Wikipedia article
				var url = new NSUrl(urlString);
				var request = new NSUrlRequest (url);
				detailWebView.LoadRequest (request);
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			if(UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
			{
				LanguageListController languageListController = null;

				// This will launch the popover when the "Choose Language" button is pressed
				EventHandler ToggleLanguagePopover = (s, e) => {
					if (languagePopoverController == null) {    // Create a TableViewController for the language list
						languageListController = new LanguageListController ();
						languageListController.DetailViewControllerRef = this;

						languagePopoverController = new UIPopoverController (languageListController);   // Put the language list in a popover
						languagePopoverController.PresentFromBarButtonItem (languageButton,
							UIPopoverArrowDirection.Any, true);
					} else {                                     // if we already have a conroller, kill it
						languagePopoverController.Dismiss (true);
						languagePopoverController = null;
					}
				};

				languageButton = new UIBarButtonItem ("Choose Language", UIBarButtonItemStyle.Plain, ToggleLanguagePopover);
				NavigationItem.RightBarButtonItem = languageButton;
			}

			ConfigureView ();
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
			
		[Export ("popoverControllerDidDismissPopover:")]
		public void DidDismiss (UIKit.UIPopoverController popoverController)
		{
			if(popoverController == languagePopoverController)
			{
				languagePopoverController = null;
			}
		}

		private string ModifyUrlForlanguage(string url, string languageCode)
		{
			if (languageCode != "")
				return url.Remove (7, 2).Insert (7, languageCode);
			else
				return url;
		}
	}
}


