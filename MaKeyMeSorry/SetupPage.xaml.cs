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
using Windows.Media;

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
        string player1name;
        string player2name;
        string player3name;
        string player4name;
        Color player1_color;
        Color player2_color;
        Color player3_color;
        Color player4_color;

        TextBlock red_player1 = new TextBlock();
        TextBlock blue_player1 = new TextBlock();
        TextBlock green_player1 = new TextBlock();
        TextBlock yellow_player1 = new TextBlock();

        TextBlock red_player2 = new TextBlock();
        TextBlock blue_player2 = new TextBlock();
        TextBlock green_player2 = new TextBlock();
        TextBlock yellow_player2 = new TextBlock();

        TextBlock red_player3 = new TextBlock();
        TextBlock blue_player3 = new TextBlock();
        TextBlock green_player3 = new TextBlock();
        TextBlock yellow_player3 = new TextBlock();

        TextBlock red_player4 = new TextBlock();
        TextBlock blue_player4 = new TextBlock();
        TextBlock green_player4 = new TextBlock();
        TextBlock yellow_player4 = new TextBlock();

        TextBlock choose_color1 = new TextBlock();
        TextBlock choose_color2 = new TextBlock();
        TextBlock choose_color3 = new TextBlock();
        TextBlock choose_color4 = new TextBlock();

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        KeyEventHandler key_up_handler;
        KeyEventHandler key_down_handler;
        int focus_index = -1;

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

            player1_color = Color.WHITE;
            player2_color = Color.WHITE;
            player3_color = Color.WHITE;
            player4_color = Color.WHITE;

            numHumanPlayers = 0;

            key_up_handler = new KeyEventHandler(Page_KeyUp);
            Window.Current.Content.AddHandler(UIElement.KeyUpEvent, key_up_handler, true);

            key_down_handler = new KeyEventHandler(Page_KeyDown);
            Window.Current.Content.AddHandler(UIElement.KeyDownEvent, key_down_handler, true);

            this.InitializeComponent();

            Loaded += delegate { NumPlayersComboBox.Focus(FocusState.Keyboard); };
            focus_index = 0;


            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            NumPlayersComboBox.SelectedIndex = 0;


            red_player1.Text = "Red";
            blue_player1.Text = "Blue";
            green_player1.Text = "Green";
            yellow_player1.Text = "Yellow";

            red_player2.Text = "Red";
            blue_player2.Text = "Blue";
            green_player2.Text = "Green";
            yellow_player2.Text = "Yellow";

            red_player3.Text = "Red";
            blue_player3.Text = "Blue";
            green_player3.Text = "Green";
            yellow_player3.Text = "Yellow";

            red_player4.Text = "Red";
            blue_player4.Text = "Blue";
            green_player4.Text = "Green";
            yellow_player4.Text = "Yellow";

            choose_color1.Text = "Choose Color";
            choose_color1.Foreground = new SolidColorBrush(Windows.UI.Colors.Gray);
            color_combo_1.Items.Add(choose_color1);

            choose_color2.Text = "Choose Color";
            choose_color2.Foreground = new SolidColorBrush(Windows.UI.Colors.Gray);
            color_combo_2.Items.Add(choose_color2);

            choose_color3.Text = "Choose Color";
            choose_color3.Foreground = new SolidColorBrush(Windows.UI.Colors.Gray);
            color_combo_3.Items.Add(choose_color3);

            choose_color4.Text = "Choose Color";
            choose_color4.Foreground = new SolidColorBrush(Windows.UI.Colors.Gray);
            color_combo_4.Items.Add(choose_color4);

            reinsert_color_to_all_except("Red", 0);
            reinsert_color_to_all_except("Blue", 0);
            reinsert_color_to_all_except("Green", 0);
            reinsert_color_to_all_except("Yellow", 0);

            color_combo_1.SelectedIndex = 0;
            color_combo_2.SelectedIndex = 0;
            color_combo_3.SelectedIndex = 0;
            color_combo_4.SelectedIndex = 0;
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
            int player1_index = 0;


            if (red_selected)
            {
                Player player;
                if (player1_color == Color.RED)
                {
                    player = new Player(player1name, player1_color, true, true);
                    player1_index = 0;
                }
                else if (player2_color == Color.RED) player = new Player(player2name, player2_color, true, true);
                else if (player3_color == Color.RED) player = new Player(player3name, player3_color, true, true);
                else player = new Player(player4name, player4_color, true, true);
                players.Add(player);
            }
            else
            {
                // Add Red Computer
                string computerName = "Red Computer";
                Player computer = new Player(computerName, Color.RED, false, true);
                players.Add(computer);
            }

            if (blue_selected)
            {
                Player player;
                if (player1_color == Color.BLUE)
                {
                    player = new Player(player1name, player1_color, true, true);
                    player1_index = 1;
                }
                else if (player2_color == Color.BLUE) player = new Player(player2name, player2_color, true, true);
                else if (player3_color == Color.BLUE) player = new Player(player3name, player3_color, true, true);
                else player = new Player(player4name, player4_color, true, true);
                players.Add(player);
            }
            else
            {
                // Add Blue Computer
                string computerName = "Blue Computer";
                Player computer = new Player(computerName, Color.BLUE, false, true);
                players.Add(computer);
            }

            if (yellow_selected)
            {
                Player player;
                if (player1_color == Color.YELLOW)
                {
                    player = new Player(player1name, player1_color, true, true);
                    player1_index = 2;
                }
                else if (player2_color == Color.YELLOW) player = new Player(player2name, player2_color, true, true);
                else if (player3_color == Color.YELLOW) player = new Player(player3name, player3_color, true, true);
                else player = new Player(player4name, player4_color, true, true);
                players.Add(player);
            }
            else
            {
                // Add Yellow Computer
                string computerName = "Yellow Computer";
                Player computer = new Player(computerName, Color.YELLOW, false, true);
                players.Add(computer);
            }

            if (green_selected)
            {
                Player player;
                if (player1_color == Color.GREEN)
                {
                    player = new Player(player1name, player1_color, true, true);
                    player1_index = 3;
                }
                else if (player2_color == Color.GREEN) player = new Player(player2name, player2_color, true, true);
                else if (player3_color == Color.GREEN) player = new Player(player3name, player3_color, true, true);
                else player = new Player(player4name, player4_color, true, true);
                players.Add(player);
            }
            else
            {
                // Add Green Computer
                string computerName = "Green Computer";
                Player computer = new Player(computerName, Color.GREEN, false, true);
                players.Add(computer);
            }

            /*
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
            */

            Game game = new Game(numHumanPlayers, players, player1_index);

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
                if (player1_color == Color.WHITE)
                {
                    MessageDialog message = new MessageDialog("Please select a color for all human players");
                    message.ShowAsync();
                    return;
                }
            }
            if (numHumanPlayers > 1)
            {
                if (player2_color == Color.WHITE)
                {
                    MessageDialog message = new MessageDialog("Please select a color for all human players");
                    message.ShowAsync();
                    return;
                }
            }
            if (numHumanPlayers > 2)
            {
                if (player3_color == Color.WHITE)
                {
                    MessageDialog message = new MessageDialog("Please select a color for all human players");
                    message.ShowAsync();
                    return;
                }
            }
            if (numHumanPlayers > 3)
            {
                if (player4_color == Color.WHITE)
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


        private void reinsert_color_to_all_except(string color, int ignored_player)
        {
            // passing in 0 will insert color in all combo boxes
            Debug.WriteLine("Inserting " + color + " into all players except: " + ignored_player);
            if (ignored_player != 1)
            {
                if (color == "Red") color_combo_1.Items.Add(red_player1);
                if (color == "Blue") color_combo_1.Items.Add(blue_player1);
                if (color == "Green") color_combo_1.Items.Add(green_player1);
                if (color == "Yellow") color_combo_1.Items.Add(yellow_player1);
            }
            if (ignored_player != 2)
            {

                if (color == "Red") color_combo_2.Items.Add(red_player2);
                if (color == "Blue") color_combo_2.Items.Add(blue_player2);
                if (color == "Green") color_combo_2.Items.Add(green_player2);
                if (color == "Yellow") color_combo_2.Items.Add(yellow_player2);
            }
            if (ignored_player != 3)
            {
                if (color == "Red") color_combo_3.Items.Add(red_player3);
                if (color == "Blue") color_combo_3.Items.Add(blue_player3);
                if (color == "Green") color_combo_3.Items.Add(green_player3);
                if (color == "Yellow") color_combo_3.Items.Add(yellow_player3);
            }
            if (ignored_player != 4)
            {
                if (color == "Red") color_combo_4.Items.Add(red_player4);
                if (color == "Blue") color_combo_4.Items.Add(blue_player4);
                if (color == "Green") color_combo_4.Items.Add(green_player4);
                if (color == "Yellow") color_combo_4.Items.Add(yellow_player4);
            }
        }

        private void remove_color_from_all_except(string color, int ignored_player)
        {
            Debug.WriteLine("Removing " + color + " from all players except: " + ignored_player);
            if (ignored_player != 1)
            {
                if (color == "Red") color_combo_1.Items.Remove(red_player1);
                if (color == "Blue") color_combo_1.Items.Remove(blue_player1);
                if (color == "Green") color_combo_1.Items.Remove(green_player1);
                if (color == "Yellow") color_combo_1.Items.Remove(yellow_player1);
                if (player1_color == Color.RED) player1colorIndex = color_combo_1.Items.IndexOf(red_player1);
                if (player1_color == Color.BLUE) player1colorIndex = color_combo_1.Items.IndexOf(blue_player1);
                if (player1_color == Color.YELLOW) player1colorIndex = color_combo_1.Items.IndexOf(yellow_player1);
                if (player1_color == Color.GREEN) player1colorIndex = color_combo_1.Items.IndexOf(green_player1);
                if (player1_color == Color.WHITE) player1colorIndex = color_combo_1.Items.IndexOf(choose_color1);
            }
            if (ignored_player != 2)
            {
                if (color == "Red") color_combo_2.Items.Remove(red_player2);
                if (color == "Blue") color_combo_2.Items.Remove(blue_player2);
                if (color == "Green") color_combo_2.Items.Remove(green_player2);
                if (color == "Yellow") color_combo_2.Items.Remove(yellow_player2);
                if (player2_color == Color.RED) player2colorIndex = color_combo_2.Items.IndexOf(red_player2);
                if (player2_color == Color.BLUE) player2colorIndex = color_combo_2.Items.IndexOf(blue_player2);
                if (player2_color == Color.YELLOW) player2colorIndex = color_combo_2.Items.IndexOf(yellow_player2);
                if (player2_color == Color.GREEN) player2colorIndex = color_combo_2.Items.IndexOf(green_player2);
                if (player2_color == Color.WHITE) player2colorIndex = color_combo_2.Items.IndexOf(choose_color2);
            }
            if (ignored_player != 3)
            {
                if (color == "Red") color_combo_3.Items.Remove(red_player3);
                if (color == "Blue") color_combo_3.Items.Remove(blue_player3);
                if (color == "Green") color_combo_3.Items.Remove(green_player3);
                if (color == "Yellow") color_combo_3.Items.Remove(yellow_player3);
                if (player3_color == Color.RED) player3colorIndex = color_combo_3.Items.IndexOf(red_player3);
                if (player3_color == Color.BLUE) player3colorIndex = color_combo_3.Items.IndexOf(blue_player3);
                if (player3_color == Color.YELLOW) player3colorIndex = color_combo_3.Items.IndexOf(yellow_player3);
                if (player3_color == Color.GREEN) player3colorIndex = color_combo_3.Items.IndexOf(green_player3);
                if (player3_color == Color.WHITE) player3colorIndex = color_combo_3.Items.IndexOf(choose_color3);
            }
            if (ignored_player != 4)
            {
                if (color == "Red") color_combo_4.Items.Remove(red_player4);
                if (color == "Blue") color_combo_4.Items.Remove(blue_player4);
                if (color == "Green") color_combo_4.Items.Remove(green_player4);
                if (color == "Yellow") color_combo_4.Items.Remove(yellow_player4);
                if (player4_color == Color.RED) player4colorIndex = color_combo_4.Items.IndexOf(red_player4);
                if (player4_color == Color.BLUE) player4colorIndex = color_combo_4.Items.IndexOf(blue_player4);
                if (player4_color == Color.YELLOW) player4colorIndex = color_combo_4.Items.IndexOf(yellow_player4);
                if (player4_color == Color.GREEN) player4colorIndex = color_combo_4.Items.IndexOf(green_player4);
                if (player4_color == Color.WHITE) player4colorIndex = color_combo_4.Items.IndexOf(choose_color4);
            }
        }

        private void unselect_current_color_from_player(int player)
        {
            Color currently_selected = Color.WHITE;

            if (player == 1) currently_selected = player1_color;
            if (player == 2) currently_selected = player2_color;
            if (player == 3) currently_selected = player3_color;
            if (player == 4) currently_selected = player4_color;

            Debug.WriteLine("Unselecting " + currently_selected + " from player: " + player);

            if (currently_selected == Color.RED)
            {
                reinsert_color_to_all_except("Red", player);
                red_selected = false;
            }
            if (currently_selected == Color.BLUE)
            {
                reinsert_color_to_all_except("Blue", player);
                blue_selected = false;
            }
            if (currently_selected == Color.GREEN)
            {
                reinsert_color_to_all_except("Green", player);
                green_selected = false;
            }
            if (currently_selected == Color.YELLOW)
            {
                reinsert_color_to_all_except("Yellow", player);
                yellow_selected = false;
            }
        }


        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            // Check if already selected color and changing colors

            Debug.WriteLine("ComboBox_SelectionChanged_1");

            TextBlock selected_Color = color_combo_1.SelectedItem as TextBlock;

            if (player1_color != Color.WHITE)
            {
                unselect_current_color_from_player(1);
                player1_color = Color.WHITE;
            }

            if (selected_Color.Text == "Red")
            {
                remove_color_from_all_except("Red", 1);
                red_selected = true;
                player1_color = Color.RED;
            }
            if (selected_Color.Text == "Blue")
            {
                remove_color_from_all_except("Blue", 1);
                blue_selected = true;
                player1_color = Color.BLUE;
            }
            if (selected_Color.Text == "Green")
            {
                remove_color_from_all_except("Green", 1);
                green_selected = true;
                player1_color = Color.GREEN;
            }
            if (selected_Color.Text == "Yellow")
            {
                remove_color_from_all_except("Yellow", 1);
                yellow_selected = true;
                player1_color = Color.YELLOW;
            }
            if (selected_Color.Text == "Choose Color" && player1_color != Color.WHITE)
            {
                unselect_current_color_from_player(1);
                player1_color = Color.WHITE;
            }
        }

        private void ComboBox_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {
            // Check if already selected color and changing colors

            TextBlock selected_Color = color_combo_2.SelectedItem as TextBlock;

            if (player2_color != Color.WHITE)
            {
                unselect_current_color_from_player(2);
                player2_color = Color.WHITE;
            }

            if (selected_Color.Text == "Red")
            {
                remove_color_from_all_except("Red", 2);
                red_selected = true;
                player2_color = Color.RED;
            }
            if (selected_Color.Text == "Blue")
            {
                remove_color_from_all_except("Blue", 2);
                blue_selected = true;
                player2_color = Color.BLUE;
            }
            if (selected_Color.Text == "Green")
            {
                remove_color_from_all_except("Green", 2);
                green_selected = true;
                player2_color = Color.GREEN;
            }
            if (selected_Color.Text == "Yellow")
            {
                remove_color_from_all_except("Yellow", 2);
                yellow_selected = true;
                player2_color = Color.YELLOW;
            }
            if (selected_Color.Text == "Choose Color" && player2_color != Color.WHITE)
            {
                unselect_current_color_from_player(2);
                player2_color = Color.WHITE;
            }
        }

        private void ComboBox_SelectionChanged_3(object sender, SelectionChangedEventArgs e)
        {
            // Check if already selected color and changing colors

            TextBlock selected_Color = color_combo_3.SelectedItem as TextBlock;

            if (player3_color != Color.WHITE)
            {
                unselect_current_color_from_player(3);
                player3_color = Color.WHITE;
            }

            if (selected_Color.Text == "Red")
            {
                remove_color_from_all_except("Red", 3);
                red_selected = true;
                player3_color = Color.RED;
            }
            if (selected_Color.Text == "Blue")
            {
                remove_color_from_all_except("Blue", 3);
                blue_selected = true;
                player3_color = Color.BLUE;
            }
            if (selected_Color.Text == "Green")
            {
                remove_color_from_all_except("Green", 3);
                green_selected = true;
                player3_color = Color.GREEN;
            }
            if (selected_Color.Text == "Yellow")
            {
                remove_color_from_all_except("Yellow", 3);
                yellow_selected = true;
                player3_color = Color.YELLOW;
            }
            if (selected_Color.Text == "Choose Color" && player3_color != Color.WHITE)
            {
                unselect_current_color_from_player(3);
                player3_color = Color.WHITE;
            }
        }

        private void ComboBox_SelectionChanged_4(object sender, SelectionChangedEventArgs e)
        {
            // Check if already selected color and changing colors

            TextBlock selected_Color = color_combo_4.SelectedItem as TextBlock;

            if (player4_color != Color.WHITE)
            {
                unselect_current_color_from_player(4);
                player4_color = Color.WHITE;
            }

            if (selected_Color.Text == "Red")
            {
                remove_color_from_all_except("Red", 4);
                red_selected = true;
                player4_color = Color.RED;
            }
            if (selected_Color.Text == "Blue")
            {
                remove_color_from_all_except("Blue", 4);
                blue_selected = true;
                player4_color = Color.BLUE;
            }
            if (selected_Color.Text == "Green")
            {
                remove_color_from_all_except("Green", 4);
                green_selected = true;
                player4_color = Color.GREEN;
            }
            if (selected_Color.Text == "Yellow")
            {
                remove_color_from_all_except("Yellow", 4);
                yellow_selected = true;
                player4_color = Color.YELLOW;
            }
            if (selected_Color.Text == "Choose Color" && player4_color != Color.WHITE)
            {
                unselect_current_color_from_player(4);
                player4_color = Color.WHITE;
            }
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
            if (e.Key == Windows.System.VirtualKey.Space)
            {
                name_textbox_1.Text = "Grace";
            }
        }

        int comboIndex = 0;
        int player1colorIndex = 0;
        int player2colorIndex = 0;
        int player3colorIndex = 0;
        int player4colorIndex = 0;


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

            if ((e.Key == Windows.System.VirtualKey.Right) || (e.Key == Windows.System.VirtualKey.Left))
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
                    Debug.WriteLine("player1colorIndex: " + player1colorIndex);
                    Debug.WriteLine("color_combo_1.SelectedIndex: " + color_combo_1.SelectedIndex);

                    if (player1colorIndex != color_combo_1.SelectedIndex)
                    {
                        Debug.WriteLine("Player1 ComboBox changed");
                        Debug.WriteLine("Player1 Current Color " + player1_color);
                        int index = -1;

                        if (player1_color == Color.RED) index = color_combo_1.Items.IndexOf(red_player1);
                        if (player1_color == Color.BLUE) index = color_combo_1.Items.IndexOf(blue_player1);
                        if (player1_color == Color.YELLOW) index = color_combo_1.Items.IndexOf(yellow_player1);
                        if (player1_color == Color.GREEN) index = color_combo_1.Items.IndexOf(green_player1);
                        if (player1_color == Color.WHITE) index = color_combo_1.Items.IndexOf(choose_color1);

                        Debug.WriteLine("Index of Current Color " + index);

                        if (e.Key == Windows.System.VirtualKey.Right) index = index - 1;
                        else if (e.Key == Windows.System.VirtualKey.Left) index = index + 1;

                        if (index == -1) color_combo_1.SelectedIndex = 0;
                        else if (index > 4) color_combo_1.SelectedIndex = 4;
                        else color_combo_1.SelectedIndex = index;
                    }
                    Debug.WriteLine("Player1 ComboBox not changed");
                    Debug.WriteLine("Player1 Current Color " + player1_color);
                    player1colorIndex = color_combo_1.SelectedIndex;
                    e.Handled = true;
                    return;
                }
                else if (focus_index == 4)
                {
                    // player 2 color combo box player2colorIndex color_combo_2
                    if (player2colorIndex != color_combo_2.SelectedIndex)
                    {
                        int index = -1;

                        if (player2_color == Color.RED) index = color_combo_2.Items.IndexOf(red_player2);
                        if (player2_color == Color.BLUE) index = color_combo_2.Items.IndexOf(blue_player2);
                        if (player2_color == Color.YELLOW) index = color_combo_2.Items.IndexOf(yellow_player2);
                        if (player2_color == Color.GREEN) index = color_combo_2.Items.IndexOf(green_player2);
                        if (player2_color == Color.WHITE) index = color_combo_2.Items.IndexOf(choose_color2);

                        if (e.Key == Windows.System.VirtualKey.Right) index = index - 1;
                        else if (e.Key == Windows.System.VirtualKey.Left) index = index + 1;

                        if (index == -1) color_combo_2.SelectedIndex = 0;
                        else if (index > 4) color_combo_2.SelectedIndex = 4;
                        else color_combo_2.SelectedIndex = index;
                    }
                    player2colorIndex = color_combo_2.SelectedIndex;
                    e.Handled = true;
                    return;
                }
                else if (focus_index == 6)
                {
                    // player 3 color combo box player3colorIndex color_combo_3
                    if (player3colorIndex != color_combo_3.SelectedIndex)
                    {
                        int index = -1;

                        if (player3_color == Color.RED) index = color_combo_3.Items.IndexOf(red_player3);
                        if (player3_color == Color.BLUE) index = color_combo_3.Items.IndexOf(blue_player3);
                        if (player3_color == Color.YELLOW) index = color_combo_3.Items.IndexOf(yellow_player3);
                        if (player3_color == Color.GREEN) index = color_combo_3.Items.IndexOf(green_player3);
                        if (player3_color == Color.WHITE) index = color_combo_3.Items.IndexOf(choose_color3);


                        if (e.Key == Windows.System.VirtualKey.Right) index = index - 1;
                        else if (e.Key == Windows.System.VirtualKey.Left) index = index + 1;

                        if (index == -1) color_combo_3.SelectedIndex = 0;
                        else if (index > 4) color_combo_3.SelectedIndex = 4;
                        else color_combo_3.SelectedIndex = index;
                    }
                    player3colorIndex = color_combo_3.SelectedIndex;
                    e.Handled = true;
                }
                else if (focus_index == 8)
                {
                    // player 4 color combo box player4colorIndex color_combo_4
                    if (player4colorIndex != color_combo_4.SelectedIndex)
                    {
                        int index = -1;

                        if (player4_color == Color.RED) index = color_combo_4.Items.IndexOf(red_player4);
                        if (player4_color == Color.BLUE) index = color_combo_4.Items.IndexOf(blue_player4);
                        if (player4_color == Color.YELLOW) index = color_combo_4.Items.IndexOf(yellow_player4);
                        if (player4_color == Color.GREEN) index = color_combo_4.Items.IndexOf(green_player4);
                        if (player4_color == Color.WHITE) index = color_combo_4.Items.IndexOf(choose_color4);

                        if (e.Key == Windows.System.VirtualKey.Right) index = index - 1;
                        else if (e.Key == Windows.System.VirtualKey.Left) index = index + 1;

                        if (index == -1) color_combo_4.SelectedIndex = 0;
                        else if (index > 4) color_combo_4.SelectedIndex = 4;
                        else color_combo_4.SelectedIndex = index;
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
                        if (NumPlayersComboBox.SelectedIndex == 3)
                        {
                            color_combo_4.SelectedIndex = 0;
                            player4colorIndex = color_combo_4.SelectedIndex;
                            name_textbox_4.Text = "";
                        }
                        else if (NumPlayersComboBox.SelectedIndex == 2)
                        {
                            color_combo_3.SelectedIndex = 0;
                            player3colorIndex = color_combo_3.SelectedIndex;
                            name_textbox_3.Text = "";
                        }
                        else if (NumPlayersComboBox.SelectedIndex == 1)
                        {
                            color_combo_2.SelectedIndex = 0;
                            player2colorIndex = color_combo_2.SelectedIndex;
                            name_textbox_2.Text = "";
                        }
                        else if (NumPlayersComboBox.SelectedIndex == 0)
                        {
                            color_combo_1.SelectedIndex = 0;
                            player1colorIndex = color_combo_1.SelectedIndex;
                            name_textbox_1.Text = "";
                        }
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

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(StartPage));
        }
    }
}
