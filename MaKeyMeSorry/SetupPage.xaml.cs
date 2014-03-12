using MaKeyMeSorry.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Windows.Input;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace MaKeyMeSorry
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SetupPage : Page
    {

        private int currentComboBoxIndex;
        bool red_selected;
        bool blue_selected;
        bool green_selected;
        bool yellow_selected;
        static int red_index = 0;
        static int blue_index = 1;
        static int green_index = 3;
        static int yellow_index = 2;
        int player1_color_selected;
        int player2_color_selected;
        int player3_color_selected;
        int player4_color_selected;

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public SetupPage()
        {
            currentComboBoxIndex = -1;
            red_selected = false;
            blue_selected = false;
            yellow_selected = false;
            green_selected = false;
            player1_color_selected = -1;
            player2_color_selected = -1;
            player3_color_selected = -1;
            player4_color_selected = -1;

            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
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
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (currentComboBoxIndex > NumPlayersComboBox.SelectedIndex)
            {
                PlayerOptions1.Visibility = Visibility.Collapsed;
                PlayerOptions2.Visibility = Visibility.Collapsed;
                PlayerOptions3.Visibility = Visibility.Collapsed;
                PlayerOptions4.Visibility = Visibility.Collapsed;
                ComputerPlayerMessage.Text = "Four computer players will be added to this game";
            }

            if (NumPlayersComboBox.SelectedIndex >= 1)
            {
                PlayerOptions1.Visibility = Visibility.Visible;
                ComputerPlayerMessage.Text = "Three computer players will be added to this game";
            }

            if (NumPlayersComboBox.SelectedIndex >= 2)
            {
                PlayerOptions2.Visibility = Visibility.Visible;
                ComputerPlayerMessage.Text = "Two computer players will be added to this game";
            }

            if (NumPlayersComboBox.SelectedIndex >= 3)
            {
                PlayerOptions3.Visibility = Visibility.Visible;
                ComputerPlayerMessage.Text = "One computer player will be added to this game";
            }

            if (NumPlayersComboBox.SelectedIndex >= 4)
            {
                PlayerOptions4.Visibility = Visibility.Visible;
                ComputerPlayerMessage.Text = "No computer players will be added to this game";
            }

            currentComboBoxIndex = NumPlayersComboBox.SelectedIndex;
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            // Check if already selected color and changing colors
            if (player1_color_selected != -1)
            {
                if (player1_color_selected == red_index)
                {
                    red2.Visibility = Visibility.Visible;
                    red3.Visibility = Visibility.Visible;
                    red4.Visibility = Visibility.Visible;
                    red_selected = false;
                }
                if (player1_color_selected == blue_index)
                {
                    blue2.Visibility = Visibility.Visible;
                    blue3.Visibility = Visibility.Visible;
                    blue4.Visibility = Visibility.Visible;
                    blue_selected = false;
                }
                if (player1_color_selected == green_index)
                {
                    green2.Visibility = Visibility.Visible;
                    green3.Visibility = Visibility.Visible;
                    green4.Visibility = Visibility.Visible;
                    green_selected = false;
                }
                if (player1_color_selected == yellow_index)
                {
                    yellow2.Visibility = Visibility.Visible;
                    yellow3.Visibility = Visibility.Visible;
                    yellow4.Visibility = Visibility.Visible;
                    yellow_selected = false;
                }
            }

            if (color_combo_1.SelectedIndex == red_index)
            {
                red2.Visibility = Visibility.Collapsed;
                red3.Visibility = Visibility.Collapsed;
                red4.Visibility = Visibility.Collapsed;
                red_selected = true;
            }
            if (color_combo_1.SelectedIndex == blue_index)
            {
                blue2.Visibility = Visibility.Collapsed;
                blue3.Visibility = Visibility.Collapsed;
                blue4.Visibility = Visibility.Collapsed;
                blue_selected = true;
            }
            if (color_combo_1.SelectedIndex == green_index)
            {
                green2.Visibility = Visibility.Collapsed;
                green3.Visibility = Visibility.Collapsed;
                green4.Visibility = Visibility.Collapsed;
                green_selected = true;
            }
            if (color_combo_1.SelectedIndex == yellow_index)
            {
                yellow2.Visibility = Visibility.Collapsed;
                yellow3.Visibility = Visibility.Collapsed;
                yellow4.Visibility = Visibility.Collapsed;
                blue_selected = true;
            }

            player1_color_selected = color_combo_1.SelectedIndex;
        }

        private void ComboBox_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {
            // Check if already selected color and changing colors
            if (player2_color_selected != -1)
            {
                if (player2_color_selected == red_index)
                {
                    red1.Visibility = Visibility.Visible;
                    red3.Visibility = Visibility.Visible;
                    red4.Visibility = Visibility.Visible;
                    red_selected = false;
                }
                if (player2_color_selected == blue_index)
                {
                    blue1.Visibility = Visibility.Visible;
                    blue3.Visibility = Visibility.Visible;
                    blue4.Visibility = Visibility.Visible;
                    blue_selected = false;
                }
                if (player2_color_selected == green_index)
                {
                    green1.Visibility = Visibility.Visible;
                    green3.Visibility = Visibility.Visible;
                    green4.Visibility = Visibility.Visible;
                    green_selected = false;
                }
                if (player2_color_selected == yellow_index)
                {
                    yellow1.Visibility = Visibility.Visible;
                    yellow3.Visibility = Visibility.Visible;
                    yellow4.Visibility = Visibility.Visible;
                    yellow_selected = false;
                }
            }

            if (color_combo_2.SelectedIndex == red_index)
            {
                red1.Visibility = Visibility.Collapsed;
                red3.Visibility = Visibility.Collapsed;
                red4.Visibility = Visibility.Collapsed;
                red_selected = true;
            }
            if (color_combo_2.SelectedIndex == blue_index)
            {
                blue1.Visibility = Visibility.Collapsed;
                blue3.Visibility = Visibility.Collapsed;
                blue4.Visibility = Visibility.Collapsed;
                blue_selected = true;
            }
            if (color_combo_2.SelectedIndex == green_index)
            {
                green1.Visibility = Visibility.Collapsed;
                green3.Visibility = Visibility.Collapsed;
                green4.Visibility = Visibility.Collapsed;
                green_selected = true;
            }
            if (color_combo_2.SelectedIndex == yellow_index)
            {
                yellow1.Visibility = Visibility.Collapsed;
                yellow3.Visibility = Visibility.Collapsed;
                yellow4.Visibility = Visibility.Collapsed;
                blue_selected = true;
            }

            player2_color_selected = color_combo_2.SelectedIndex;
        }

        private void ComboBox_SelectionChanged_3(object sender, SelectionChangedEventArgs e)
        {
            // Check if already selected color and changing colors
            if (player3_color_selected != -1)
            {
                if (player3_color_selected == red_index)
                {
                    red2.Visibility = Visibility.Visible;
                    red1.Visibility = Visibility.Visible;
                    red4.Visibility = Visibility.Visible;
                    red_selected = false;
                }
                if (player3_color_selected == blue_index)
                {
                    blue2.Visibility = Visibility.Visible;
                    blue1.Visibility = Visibility.Visible;
                    blue4.Visibility = Visibility.Visible;
                    blue_selected = false;
                }
                if (player3_color_selected == green_index)
                {
                    green2.Visibility = Visibility.Visible;
                    green1.Visibility = Visibility.Visible;
                    green4.Visibility = Visibility.Visible;
                    green_selected = false;
                }
                if (player3_color_selected == yellow_index)
                {
                    yellow2.Visibility = Visibility.Visible;
                    yellow1.Visibility = Visibility.Visible;
                    yellow4.Visibility = Visibility.Visible;
                    yellow_selected = false;
                }
            }

            if (color_combo_3.SelectedIndex == red_index)
            {
                red2.Visibility = Visibility.Collapsed;
                red1.Visibility = Visibility.Collapsed;
                red4.Visibility = Visibility.Collapsed;
                red_selected = true;
            }
            if (color_combo_3.SelectedIndex == blue_index)
            {
                blue2.Visibility = Visibility.Collapsed;
                blue1.Visibility = Visibility.Collapsed;
                blue4.Visibility = Visibility.Collapsed;
                blue_selected = true;
            }
            if (color_combo_3.SelectedIndex == green_index)
            {
                green2.Visibility = Visibility.Collapsed;
                green1.Visibility = Visibility.Collapsed;
                green4.Visibility = Visibility.Collapsed;
                green_selected = true;
            }
            if (color_combo_3.SelectedIndex == yellow_index)
            {
                yellow2.Visibility = Visibility.Collapsed;
                yellow1.Visibility = Visibility.Collapsed;
                yellow4.Visibility = Visibility.Collapsed;
                blue_selected = true;
            }

            player3_color_selected = color_combo_3.SelectedIndex;
        }

        private void ComboBox_SelectionChanged_4(object sender, SelectionChangedEventArgs e)
        {
            // Check if already selected color and changing colors
            if (player4_color_selected != -1)
            {
                if (player4_color_selected == red_index)
                {
                    red2.Visibility = Visibility.Visible;
                    red1.Visibility = Visibility.Visible;
                    red3.Visibility = Visibility.Visible;
                    red_selected = false;
                }
                if (player4_color_selected == blue_index)
                {
                    blue2.Visibility = Visibility.Visible;
                    blue1.Visibility = Visibility.Visible;
                    blue3.Visibility = Visibility.Visible;
                    blue_selected = false;
                }
                if (player4_color_selected == green_index)
                {
                    green2.Visibility = Visibility.Visible;
                    green1.Visibility = Visibility.Visible;
                    green3.Visibility = Visibility.Visible;
                    green_selected = false;
                }
                if (player4_color_selected == yellow_index)
                {
                    yellow2.Visibility = Visibility.Visible;
                    yellow1.Visibility = Visibility.Visible;
                    yellow3.Visibility = Visibility.Visible;
                    yellow_selected = false;
                }
            }

            if (color_combo_4.SelectedIndex == red_index)
            {
                red2.Visibility = Visibility.Collapsed;
                red1.Visibility = Visibility.Collapsed;
                red3.Visibility = Visibility.Collapsed;
                red_selected = true;
            }
            if (color_combo_4.SelectedIndex == blue_index)
            {
                blue2.Visibility = Visibility.Collapsed;
                blue1.Visibility = Visibility.Collapsed;
                blue3.Visibility = Visibility.Collapsed;
                blue_selected = true;
            }
            if (color_combo_4.SelectedIndex == green_index)
            {
                green2.Visibility = Visibility.Collapsed;
                green1.Visibility = Visibility.Collapsed;
                green3.Visibility = Visibility.Collapsed;
                green_selected = true;
            }
            if (color_combo_4.SelectedIndex == yellow_index)
            {
                yellow2.Visibility = Visibility.Collapsed;
                yellow1.Visibility = Visibility.Collapsed;
                yellow3.Visibility = Visibility.Collapsed;
                blue_selected = true;
            }

            player4_color_selected = color_combo_4.SelectedIndex;
        }

    }
}
