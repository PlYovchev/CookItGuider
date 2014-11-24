using CookItUniversal.Common;
using CookItUniversal.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace CookItUniversal.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RecipeStepPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private bool isDeviceShaken;

        public RecipeStepPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            //this.Accelerometer.Shaken += (snd, args) =>
            //{
            //    Debug.WriteLine("shaken");
            //    this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //    {
            //        isDeviceShaken = true;
            //    });
                
            //};

            this.Accelerometer = Accelerometer.GetDefault();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += (obj, args) =>
            {
                isDeviceShaken = true;
            };
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Start();

            this.Accelerometer.ReadingChanged += (snd, args) =>
            {
                if (isDeviceShaken)
                {
                    var x = args.Reading.AccelerationX;
                    var y = args.Reading.AccelerationY;
                    if ((y > -0.2 && y < 0.2) && (x > 0.8))
                    {
                        this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            ((RecipeStepViewModel)this.DataContext).LoadNextStep();
                            isDeviceShaken = false;
                        });
                    }
                    else if ((y > -0.2 && y < 0.2) && (x < -0.8))
                    {
                        this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            ((RecipeStepViewModel)this.DataContext).LoadPreviousStep();
                            isDeviceShaken = false;
                        });
                    }
                }
            };
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            IList<RecipeStepViewModel> recipeSteps = e.Parameter as IList<RecipeStepViewModel>;
            RecipeStepViewModel recipeStepsViewModel = new RecipeStepViewModel(recipeSteps);
            this.DataContext = recipeStepsViewModel;

            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void MediaElement_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            MediaElement mediaElement = sender as MediaElement;
            MediaElementState state = mediaElement.CurrentState;
            if (mediaElement.CurrentState == MediaElementState.Paused)
            {
                mediaElement.Play();
            }
            else if (mediaElement.CurrentState == MediaElementState.Playing)
            {
                mediaElement.Pause();
            }
            
        }

        private void OnHoldEvent(object sender, HoldingRoutedEventArgs e)
        {
            MediaElement mediaElement = sender as MediaElement;
            mediaElement.Stop();
            mediaElement.Play();
        }

        private void OnSwipeListener(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (!e.IsInertial)
            {
                MediaElement mediaElement = sender as MediaElement;

                var delta = e.Delta.Translation;
                if (delta.X > 0)
                {
                    mediaElement.Position += TimeSpan.FromMilliseconds(300);
                }
                else if (delta.X < 0)
                {
                    mediaElement.Position -= TimeSpan.FromMilliseconds(300);
                }

                Debug.WriteLine(((int)delta.X).ToString());
            }
        }

        private void OnDismissClick(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        public Accelerometer Accelerometer { get; set; }
    }
}
