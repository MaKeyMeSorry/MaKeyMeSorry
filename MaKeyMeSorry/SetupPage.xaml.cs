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
using Windows.UI.Popups;
using System.Diagnostics;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace MaKeyMeSorry
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SetupPage : Page
    {

        private int currentComboBoxIndex;
        int numHumanPlayers;
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
        string player1name;
        string player2name;
        string player3name;
        string player4name;
        Color player1_color;
        Color player2_color;
        Color player3_color;
        Color player4_color;

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

        KeyEventHandler key_up_handler;
        KeyEventHandler key_down_handler;
        int focus_index = -1;

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

            numHumanPlayers = 0;

            key_up_handler = new KeyEventHandler(Page_KeyUp);
            Window.Current.Content.AddHandler(UIElement.KeyUpEvent, key_up_handler, true);

            key_down_handler = new KeyEventHandler(Page_KeyDown);
            Window.Current.Content.AddHandler(UIElement.KeyDownEvent, key_down_handler, true);

            this.InitializeComponent();

            Loaded += delegate { NumPlayersComboBox.Focus(FocusState.Keyboard); };
            focus_index = 0;

            // NumPlayersComboBox.Focus(FocusState.Keyboard);

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

            List<Player> players = new List<Player>();

            if (numHumanPlayers >= 1)
            {
                Player player1 = new Player(player1name, player1_color, true, true);
                players.Add(player1);
            }
            if (numHumanPlayers >= 2)
            {
                Player player2 = new Player(player2name, player2_color, true, true);
                players.Add(player2);
            }
            if (numHumanPlayers >= 3)
            {
                Player player3 = new Player(player3name, player3_color, true, true);
                players.Add(player3);
            }
            if (numHumanPlayers >= 4)
            {
                Player player4 = new Player(player4name, player4_color, true, true);
                players.Add(player4);
            }

            int numComputers = 4 - numHumanPlayers;

            if (!red_selected && numComputers > 0)
            {
                string computerName = "Red Computer";
                Player redComputer = new Player(computerName, Color.RED, false, true);
                players.Add(redComputer);
                numComputers--;
            }
            if (!blue_selected && numComputers > 0)
            {
                string computerName = "Blue Computer";
                Player blueComputer = new Player(computerName, Color.BLUE, false, true);
                players.Add(blueComputer);
                numComputers--;
            }
            if (!yellow_selected && numComputers > 0)
            {
                string computerName = "Yellow Computer";
                Player yellowComputer = new Player(computerName, Color.YELLOW, false, true);
                players.Add(yellowComputer);
                numComputers--;
            }
            if (!green_selected && numComputers > 0)
            {
                string computerName = "Green Computer";
                Player greenComputer = new Player(computerName, Color.GREEN, false, true);
                players.Add(greenComputer);
                numComputers--;
            }


            Game game = new Game(numHumanPlayers, players);

            // TODO: save and load game

            MaKeyMeSorry.App.currentGame = game;

            // e.PageState["Game"] = game;

            //e.PageState.Add("Game", game);

            // MaKeyMeSorry.Current.State["param"] = p;
            // NavigationService.Navigate(new Uri("/PhonePageOne.xaml", UriKind.Relative));

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

            // If we want to force them to choose colors uncomment this
            if (numHumanPlayers > 0)
            {
                if (player1_color_selected == -1)
                {
                    MessageDialog message = new MessageDialog("Please select a color for all human players");
                    message.ShowAsync();
                    return;
                }
            }
            if (numHumanPlayers > 1)
            {
                if (player2_color_selected == -1)
                {
                    MessageDialog message = new MessageDialog("Please select a color for all human players");
                    message.ShowAsync();
                    return;
                }
            }
            if (numHumanPlayers > 2)
            {
                if (player3_color_selected == -1)
                {
                    MessageDialog message = new MessageDialog("Please select a color for all human players");
                    message.ShowAsync();
                    return;
                }
            }
            if (numHumanPlayers > 3)
            {
                if (player4_color_selected == -1)
                {
                    MessageDialog message = new MessageDialog("Please select a color for all human players");
                    message.ShowAsync();
                    return;
                }
            }

            Window.Current.Content.RemoveHandler(UIElement.KeyUpEvent, key_up_handler);
            Window.Current.Content.RemoveHandler(UIElement.KeyDownEvent, key_down_handler);
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
                numHumanPlayers = 0;
            }

            if (NumPlayersComboBox.SelectedIndex >= 1)
            {
                PlayerOptions1.Visibility = Visibility.Visible;
                ComputerPlayerMessage.Text = "Three computer players will be added to this game";
                numHumanPlayers = 1;
            }

            if (NumPlayersComboBox.SelectedIndex >= 2)
            {
                PlayerOptions2.Visibility = Visibility.Visible;
                ComputerPlayerMessage.Text = "Two computer players will be added to this game";
                numHumanPlayers = 2;
            }

            if (NumPlayersComboBox.SelectedIndex >= 3)
            {
                PlayerOptions3.Visibility = Visibility.Visible;
                ComputerPlayerMessage.Text = "One computer player will be added to this game";
                numHumanPlayers = 3;
            }

            if (NumPlayersComboBox.SelectedIndex >= 4)
            {
                PlayerOptions4.Visibility = Visibility.Visible;
                ComputerPlayerMessage.Text = "No computer players will be added to this game";
                numHumanPlayers = 4;
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
                    player1_color = Color.WHITE;
                }
                if (player1_color_selected == blue_index)
                {
                    blue2.Visibility = Visibility.Visible;
                    blue3.Visibility = Visibility.Visible;
                    blue4.Visibility = Visibility.Visible;
                    blue_selected = false;
                    player1_color = Color.WHITE;
                }
                if (player1_color_selected == green_index)
                {
                    green2.Visibility = Visibility.Visible;
                    green3.Visibility = Visibility.Visible;
                    green4.Visibility = Visibility.Visible;
                    green_selected = false;
                    player1_color = Color.WHITE;
                }
                if (player1_color_selected == yellow_index)
                {
                    yellow2.Visibility = Visibility.Visible;
                    yellow3.Visibility = Visibility.Visible;
                    yellow4.Visibility = Visibility.Visible;
                    yellow_selected = false;
                    player1_color = Color.WHITE;
                }
            }

            if (color_combo_1.SelectedIndex == red_index)
            {
                red2.Visibility = Visibility.Collapsed;
                red3.Visibility = Visibility.Collapsed;
                red4.Visibility = Visibility.Collapsed;
                red_selected = true;
                player1_color = Color.RED;
            }
            if (color_combo_1.SelectedIndex == blue_index)
            {
                blue2.Visibility = Visibility.Collapsed;
                blue3.Visibility = Visibility.Collapsed;
                blue4.Visibility = Visibility.Collapsed;
                blue_selected = true;
                player1_color = Color.BLUE;
            }
            if (color_combo_1.SelectedIndex == green_index)
            {
                green2.Visibility = Visibility.Collapsed;
                green3.Visibility = Visibility.Collapsed;
                green4.Visibility = Visibility.Collapsed;
                green_selected = true;
                player1_color = Color.GREEN;
            }
            if (color_combo_1.SelectedIndex == yellow_index)
            {
                yellow2.Visibility = Visibility.Collapsed;
                yellow3.Visibility = Visibility.Collapsed;
                yellow4.Visibility = Visibility.Collapsed;
                yellow_selected = true;
                player1_color = Color.YELLOW;
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
                    player2_color = Color.WHITE;
                }
                if (player2_color_selected == blue_index)
                {
                    blue1.Visibility = Visibility.Visible;
                    blue3.Visibility = Visibility.Visible;
                    blue4.Visibility = Visibility.Visible;
                    blue_selected = false;
                    player2_color = Color.WHITE;
                }
                if (player2_color_selected == green_index)
                {
                    green1.Visibility = Visibility.Visible;
                    green3.Visibility = Visibility.Visible;
                    green4.Visibility = Visibility.Visible;
                    green_selected = false;
                    player2_color = Color.WHITE;
                }
                if (player2_color_selected == yellow_index)
                {
                    yellow1.Visibility = Visibility.Visible;
                    yellow3.Visibility = Visibility.Visible;
                    yellow4.Visibility = Visibility.Visible;
                    yellow_selected = false;
                    player2_color = Color.WHITE;
                }
            }

            if (color_combo_2.SelectedIndex == red_index)
            {
                red1.Visibility = Visibility.Collapsed;
                red3.Visibility = Visibility.Collapsed;
                red4.Visibility = Visibility.Collapsed;
                red_selected = true;
                player2_color = Color.RED;
            }
            if (color_combo_2.SelectedIndex == blue_index)
            {
                blue1.Visibility = Visibility.Collapsed;
                blue3.Visibility = Visibility.Collapsed;
                blue4.Visibility = Visibility.Collapsed;
                blue_selected = true;
                player2_color = Color.BLUE;
            }
            if (color_combo_2.SelectedIndex == green_index)
            {
                green1.Visibility = Visibility.Collapsed;
                green3.Visibility = Visibility.Collapsed;
                green4.Visibility = Visibility.Collapsed;
                green_selected = true;
                player2_color = Color.GREEN;
            }
            if (color_combo_2.SelectedIndex == yellow_index)
            {
                yellow1.Visibility = Visibility.Collapsed;
                yellow3.Visibility = Visibility.Collapsed;
                yellow4.Visibility = Visibility.Collapsed;
                yellow_selected = true;
                player2_color = Color.YELLOW;
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
                    player3_color = Color.WHITE;
                }
                if (player3_color_selected == blue_index)
                {
                    blue2.Visibility = Visibility.Visible;
                    blue1.Visibility = Visibility.Visible;
                    blue4.Visibility = Visibility.Visible;
                    blue_selected = false;
                    player3_color = Color.WHITE;
                }
                if (player3_color_selected == green_index)
                {
                    green2.Visibility = Visibility.Visible;
                    green1.Visibility = Visibility.Visible;
                    green4.Visibility = Visibility.Visible;
                    green_selected = false;
                    player3_color = Color.WHITE;
                }
                if (player3_color_selected == yellow_index)
                {
                    yellow2.Visibility = Visibility.Visible;
                    yellow1.Visibility = Visibility.Visible;
                    yellow4.Visibility = Visibility.Visible;
                    yellow_selected = false;
                    player3_color = Color.WHITE;
                }
            }

            if (color_combo_3.SelectedIndex == red_index)
            {
                red2.Visibility = Visibility.Collapsed;
                red1.Visibility = Visibility.Collapsed;
                red4.Visibility = Visibility.Collapsed;
                red_selected = true;
                player3_color = Color.RED;
            }
            if (color_combo_3.SelectedIndex == blue_index)
            {
                blue2.Visibility = Visibility.Collapsed;
                blue1.Visibility = Visibility.Collapsed;
                blue4.Visibility = Visibility.Collapsed;
                blue_selected = true;
                player3_color = Color.BLUE;
            }
            if (color_combo_3.SelectedIndex == green_index)
            {
                green2.Visibility = Visibility.Collapsed;
                green1.Visibility = Visibility.Collapsed;
                green4.Visibility = Visibility.Collapsed;
                green_selected = true;
                player3_color = Color.GREEN;
            }
            if (color_combo_3.SelectedIndex == yellow_index)
            {
                yellow2.Visibility = Visibility.Collapsed;
                yellow1.Visibility = Visibility.Collapsed;
                yellow4.Visibility = Visibility.Collapsed;
                yellow_selected = true;
                player3_color = Color.YELLOW;
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
                    player4_color = Color.WHITE;
                }
                if (player4_color_selected == blue_index)
                {
                    blue2.Visibility = Visibility.Visible;
                    blue1.Visibility = Visibility.Visible;
                    blue3.Visibility = Visibility.Visible;
                    blue_selected = false;
                    player4_color = Color.WHITE;
                }
                if (player4_color_selected == green_index)
                {
                    green2.Visibility = Visibility.Visible;
                    green1.Visibility = Visibility.Visible;
                    green3.Visibility = Visibility.Visible;
                    green_selected = false;
                    player4_color = Color.WHITE;
                }
                if (player4_color_selected == yellow_index)
                {
                    yellow2.Visibility = Visibility.Visible;
                    yellow1.Visibility = Visibility.Visible;
                    yellow3.Visibility = Visibility.Visible;
                    yellow_selected = false;
                    player4_color = Color.WHITE;
                }
            }

            if (color_combo_4.SelectedIndex == red_index)
            {
                red2.Visibility = Visibility.Collapsed;
                red1.Visibility = Visibility.Collapsed;
                red3.Visibility = Visibility.Collapsed;
                player4_color = Color.RED;
                red_selected = true;
            }
            if (color_combo_4.SelectedIndex == blue_index)
            {
                blue2.Visibility = Visibility.Collapsed;
                blue1.Visibility = Visibility.Collapsed;
                blue3.Visibility = Visibility.Collapsed;
                player4_color = Color.BLUE;
                blue_selected = true;
            }
            if (color_combo_4.SelectedIndex == green_index)
            {
                green2.Visibility = Visibility.Collapsed;
                green1.Visibility = Visibility.Collapsed;
                green3.Visibility = Visibility.Collapsed;
                green_selected = true;
                player4_color = Color.GREEN;
            }
            if (color_combo_4.SelectedIndex == yellow_index)
            {
                yellow2.Visibility = Visibility.Collapsed;
                yellow1.Visibility = Visibility.Collapsed;
                yellow3.Visibility = Visibility.Collapsed;
                yellow_selected = true;
                player4_color = Color.YELLOW;
            }

            player4_color_selected = color_combo_4.SelectedIndex;
        }

        private void TextBox_TextChanged1(object sender, TextChangedEventArgs e)
        {
            player1name = name_textbox_1.Text;
        }

        private void TextBox_TextChanged2(object sender, TextChangedEventArgs e)
        {
            player2name = name_textbox_2.Text;
        }

        private void TextBox_TextChanged3(object sender, TextChangedEventArgs e)
        {
            player3name = name_textbox_3.Text;
        }

        private void TextBox_TextChanged4(object sender, TextChangedEventArgs e)
        {
            player4name = name_textbox_4.Text;
        }

        private void name_textbox_1_GotFocus(object sender, RoutedEventArgs e)
        {
            Grace_Message_1.Visibility = Visibility.Visible;
        }

        private void name_textbox_1_LostFocus(object sender, RoutedEventArgs e)
        {
            Grace_Message_1.Visibility = Visibility.Collapsed;
        }

        private void name_textbox_1_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                name_textbox_1.Text = "Grace";
            }
        }

        int comboIndex = -1;
        int player1colorIndex = -1;
        int player2colorIndex = -1;
        int player3colorIndex = -1;
        int player4colorIndex = -1;

        //private void NumPlayersComboBox_KeyUp(object sender, KeyRoutedEventArgs e)
        //{
        //    Debug.WriteLine("Key Up Pressed");

        //    if (comboIndex != NumPlayersComboBox.SelectedIndex)
        //    {
        //        if ((e.Key == Windows.System.VirtualKey.Right))
        //        {
        //            NumPlayersComboBox.SelectedIndex = comboIndex;
        //            return;
        //        }
        //    }
        //    comboIndex = NumPlayersComboBox.SelectedIndex;
        //    e.Handled = true;
        //}

        //private void NumPlayersComboBox_KeyDown(object sender, KeyRoutedEventArgs e)
        //{
        //    //if (e.Key == Windows.System.VirtualKey.Right)
        //    //{
        //    //    e.Handled = true;
        //    //    return;
        //    //}
        //    //if (e.Key == Windows.System.VirtualKey.Left)
        //    //{
        //    //    e.Handled = true;
        //    //    return;
        //    //}

        //    Debug.WriteLine("Key Down Pressed");

        //    if (comboIndex != NumPlayersComboBox.SelectedIndex)
        //    {
        //        if ((e.Key == Windows.System.VirtualKey.Right))
        //        {
        //            NumPlayersComboBox.SelectedIndex = comboIndex;
        //            return;
        //        }
        //    }
        //    comboIndex = NumPlayersComboBox.SelectedIndex;
        //    e.Handled = true;
        //}


        private void Page_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            Debug.WriteLine("Page Key Up Pressed");

            if ((e.Key == Windows.System.VirtualKey.Right))
            {
                Debug.WriteLine("Right pressed, Current Focus: " + focus_index);
                e.Handled = true;
                switch (focus_index)
                {
                    case(-1):
                        // focus currently on back button
                        focus_index++;
                        // go to num players combo box
                        NumPlayersComboBox.Focus(FocusState.Keyboard);
                        break;
                    case (0):
                        // focus currently on num players combo box
                        if (numHumanPlayers >= 1)
                        {
                            // go to player 1 name
                            focus_index++;
                            name_textbox_1.Focus(FocusState.Keyboard);
                        }
                        else
                        {
                            // go to start game
                            focus_index = 9;
                            start_button.Focus(FocusState.Keyboard);
                        }
                        break;
                    case (1):
                        // focus currently on player 1 name
                        focus_index++;
                        // go to player 1 color
                        color_combo_1.Focus(FocusState.Keyboard);
                        break;
                    case (2):
                        // focus currently on player 1 color
                        if (numHumanPlayers >= 2)
                        {
                            // go to player 2 name
                            focus_index++;
                            name_textbox_2.Focus(FocusState.Keyboard);
                        }
                        else
                        {
                            // go to start game
                            focus_index = 9;
                            start_button.Focus(FocusState.Keyboard);
                        }
                        break;
                    case (3):
                        // focus currently on player 2 name
                        focus_index++;
                        // go to player 2 color
                        color_combo_2.Focus(FocusState.Keyboard);
                        break;
                    case (4):
                        // focus currently on player 2 color
                        if (numHumanPlayers >= 3)
                        {
                            // go to player 3 name
                            focus_index++;
                           name_textbox_3.Focus(FocusState.Keyboard);
                        }
                        else
                        {
                            // go to start game
                            focus_index = 9;
                            start_button.Focus(FocusState.Keyboard);
                        }
                        break;
                    case (5):
                        // focus currently on player 3 name
                        focus_index++;
                        // go to player 3 color
                        color_combo_3.Focus(FocusState.Keyboard);
                        break;
                    case (6):
                        // focus currently on player 3 color
                        if (numHumanPlayers >= 4)
                        {
                            // go to player 4 name
                            focus_index++;
                            name_textbox_4.Focus(FocusState.Keyboard);
                        }
                        else
                        {
                            // go to start game
                            focus_index = 9;
                            start_button.Focus(FocusState.Keyboard);
                        }
                        break;
                    case (7):
                        // focus currently on player 4 name
                        focus_index++;
                        // go to player 4 color
                        color_combo_4.Focus(FocusState.Keyboard);
                        break;
                    case (8):
                        // focus currently on player 4 color
                        focus_index++;
                        // go to start game
                        start_button.Focus(FocusState.Keyboard);
                        break;
                    case (9):
                        // focus currently on start game
                        // go to back button
                        focus_index = -1;
                        backButton.Focus(FocusState.Keyboard);
                        break;
                    default:
                        break;
                }

            }

            if ((e.Key == Windows.System.VirtualKey.Left))
            {
                Debug.WriteLine("Left pressed, Current Focus: " + focus_index);
                e.Handled = true;
                switch (focus_index)
                {
                    case (-1):
                        // focus currently on back button
                        focus_index = 9;
                        // go to start game
                        start_button.Focus(FocusState.Keyboard);
                        break;
                    case (0):
                        // focus currently on num players combo box
                        // go to back button
                        focus_index--;
                        backButton.Focus(FocusState.Keyboard);
                        break;
                    case (1):
                        // focus currently on player 1 name
                        focus_index--;
                        // go to num players combo box
                        NumPlayersComboBox.Focus(FocusState.Keyboard);
                        break;
                    case (2):
                        // focus currently on player 1 color
                        // go to player 1 name
                        focus_index--;
                        name_textbox_1.Focus(FocusState.Keyboard);
                        break;
                    case (3):
                        // focus currently on player 2 name
                        focus_index--;
                        // go to player 1 color
                        color_combo_1.Focus(FocusState.Keyboard);
                        break;
                    case (4):
                        // focus currently on player 2 color
                        // go to player 2 name
                        focus_index--;
                        name_textbox_2.Focus(FocusState.Keyboard);
                        break;
                    case (5):
                        // focus currently on player 3 name
                        focus_index--;
                        // go to player 2 color
                        color_combo_2.Focus(FocusState.Keyboard);
                        break;
                    case (6):
                        // focus currently on player 3 color
                        // go to player 3 name
                        focus_index--;
                        name_textbox_3.Focus(FocusState.Keyboard);
                        break;
                    case (7):
                        // focus currently on player 4 name
                        focus_index--;
                        // go to player 3 color
                        color_combo_3.Focus(FocusState.Keyboard);
                        break;
                    case (8):
                        // focus currently on player 4 color
                        focus_index--;
                        // go to player 4 name
                        name_textbox_4.Focus(FocusState.Keyboard);
                        break;
                    case (9):
                        // focus currently on start game
                        if (numHumanPlayers == 4)
                        {
                            // go to player 4's combo box
                            focus_index = 8;
                            color_combo_4.Focus(FocusState.Keyboard);
                        }
                        else if (numHumanPlayers == 3)
                        {
                            // go to player 3's combo box
                            focus_index = 6;
                            color_combo_3.Focus(FocusState.Keyboard);
                        }
                        else if (numHumanPlayers == 2)
                        {
                            // go to player 2's combo box
                            focus_index = 4;
                            color_combo_2.Focus(FocusState.Keyboard);
                        }
                        else if (numHumanPlayers == 1)
                        {
                            // go to player 1's combo box
                            focus_index = 2;
                            color_combo_1.Focus(FocusState.Keyboard);
                        }
                        else
                        {
                            // go to num players combo box
                            focus_index = 0;
                            NumPlayersComboBox.Focus(FocusState.Keyboard);
                        }
                        break;
                    default:
                        break;
                }

            }

        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {

            Debug.WriteLine("Page Key Down Pressed");

            if ((e.Key == Windows.System.VirtualKey.Right))
            {
                if (focus_index == 0)
                {
                    // number of players combo box
                    if (comboIndex != NumPlayersComboBox.SelectedIndex)
                    {
                        NumPlayersComboBox.SelectedIndex = comboIndex;
                        e.Handled = true;
                        return;
                    }
                    comboIndex = NumPlayersComboBox.SelectedIndex;
                    e.Handled = true;
                }
                else if (focus_index == 2)
                {
                    // player 1 color combo box player1colorIndex color_combo_1
                    if (player1colorIndex != color_combo_1.SelectedIndex)
                    {
                        color_combo_1.SelectedIndex = player1colorIndex;
                        e.Handled = true;
                        return;
                    }
                    player1colorIndex = color_combo_1.SelectedIndex;
                    e.Handled = true;
                }
                else if (focus_index == 4)
                {
                    // player 2 color combo box player2colorIndex color_combo_2
                    if (player2colorIndex != color_combo_2.SelectedIndex)
                    {
                        color_combo_2.SelectedIndex = player2colorIndex;
                        e.Handled = true;
                        return;
                    }
                    player2colorIndex = color_combo_2.SelectedIndex;
                    e.Handled = true;
                }
                else if (focus_index == 6)
                {
                    // player 3 color combo box player3colorIndex color_combo_3
                    if (player3colorIndex != color_combo_3.SelectedIndex)
                    {
                        color_combo_3.SelectedIndex = player3colorIndex;
                        e.Handled = true;
                        return;
                    }
                    player3colorIndex = color_combo_3.SelectedIndex;
                    e.Handled = true;
                }
                else if (focus_index == 8)
                {
                    // player 4 color combo box player4colorIndex color_combo_4
                    if (player4colorIndex != color_combo_4.SelectedIndex)
                    {
                        color_combo_4.SelectedIndex = player4colorIndex;
                        e.Handled = true;
                        return;
                    }
                    player4colorIndex = color_combo_4.SelectedIndex;
                    e.Handled = true;
                }
            }
            else if ((e.Key == Windows.System.VirtualKey.Left))
            {
                if (focus_index == 0)
                {
                    // number of players combo box
                    if (comboIndex != NumPlayersComboBox.SelectedIndex)
                    {
                        NumPlayersComboBox.SelectedIndex = comboIndex;
                        e.Handled = true;
                        return;
                    }
                    comboIndex = NumPlayersComboBox.SelectedIndex;
                    e.Handled = true;
                }
                else if (focus_index == 2)
                {
                    // player 1 color combo box player1colorIndex color_combo_1
                    if (player1colorIndex != color_combo_1.SelectedIndex)
                    {
                        color_combo_1.SelectedIndex = player1colorIndex;
                        e.Handled = true;
                        return;
                    }
                    player1colorIndex = color_combo_1.SelectedIndex;
                    e.Handled = true;
                }
                else if (focus_index == 4)
                {
                    // player 2 color combo box player2colorIndex color_combo_2
                    if (player2colorIndex != color_combo_2.SelectedIndex)
                    {
                        color_combo_2.SelectedIndex = player2colorIndex;
                        e.Handled = true;
                        return;
                    }
                    player2colorIndex = color_combo_2.SelectedIndex;
                    e.Handled = true;
                }
                else if (focus_index == 6)
                {
                    // player 3 color combo box player3colorIndex color_combo_3
                    if (player3colorIndex != color_combo_3.SelectedIndex)
                    {
                        color_combo_3.SelectedIndex = player3colorIndex;
                        e.Handled = true;
                        return;
                    }
                    player3colorIndex = color_combo_3.SelectedIndex;
                    e.Handled = true;
                }
                else if (focus_index == 8)
                {
                    // player 4 color combo box player4colorIndex color_combo_4
                    if (player4colorIndex != color_combo_4.SelectedIndex)
                    {
                        color_combo_4.SelectedIndex = player4colorIndex;
                        e.Handled = true;
                        return;
                    }
                    player4colorIndex = color_combo_4.SelectedIndex;
                    e.Handled = true;
                }
            }
            else
            {
                if (focus_index == 0)
                {
                    // number of players combo box
                    if (comboIndex != NumPlayersComboBox.SelectedIndex)
                    {
                        comboIndex = NumPlayersComboBox.SelectedIndex;
                    }
                }
                else if (focus_index == 2)
                {
                    // player 1 color combo box player1colorIndex color_combo_1
                    if (player1colorIndex != color_combo_1.SelectedIndex)
                    {
                        player1colorIndex = color_combo_1.SelectedIndex;
                    }
                }
                else if (focus_index == 4)
                {
                    // player 2 color combo box player2colorIndex color_combo_2
                    if (player2colorIndex != color_combo_2.SelectedIndex)
                    {
                        player2colorIndex = color_combo_2.SelectedIndex;
                    }
                }
                else if (focus_index == 6)
                {
                    // player 3 color combo box player3colorIndex color_combo_3
                    if (player3colorIndex != color_combo_3.SelectedIndex)
                    {
                        player3colorIndex = color_combo_3.SelectedIndex;
                    }
                }
                else if (focus_index == 8)
                {
                    // player 4 color combo box player4colorIndex color_combo_4
                    if (player4colorIndex != color_combo_4.SelectedIndex)
                    {
                        player4colorIndex = color_combo_4.SelectedIndex;
                    }
                }
            }
        }
    }
}
