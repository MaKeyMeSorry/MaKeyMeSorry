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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Windows.UI.Input;
using Windows.UI.Popups;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MaKeyMeSorry
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private NavigationHelper navigationHelper;

        private bool AI_on = false;

        private Game game;
        private int card_count;
        private Brush cur_selected_img;
        private int cur_selected_square;
        private List<Canvas> cur_selected_list;
        private int cur_pawn_selection;
        private List<bool> pawns_available;
        private int color_adjustment;
        private bool how_to_highlighted;
        private bool new_game_higlighted;
        private bool pass_highlighted;
        private List<Tuple<Pawn, List<Tuple<Square,ComboData.move>>>> myOptions;
        private int current_spot;
        private bool forefeit_disabled;
        private bool first;
        private bool from_safe_zone;
        private ComboData.move current_move_type;

        //variables for keeping state of a turn
        private Color color_of_current_turn;
        private int index_of_current_player;

        private bool card_drawn;
        private Card my_card;

        //timer stuff for 
        private double interval;
        private  DispatcherTimer timer; 
        private int times_ticked;
        private double x_diff;
        private double iterations;
        private double y_diff;
        private double m;
        private double x1;
        private double x2;
        private double y1;
        private double y2;
        private Canvas move_canvas;
        private ImageBrush move_ib;

        private List<Canvas> pawn_square_list;
        private List<Canvas> preview_square_list;

        private List<List<Canvas>> start_lists;
        private List<List<Canvas>> safe_zone_lists;
        private List<List<Canvas>> preview_safe_zone_lists;

        private List<Canvas> blue_start_list;
        private List<Canvas> yellow_start_list;
        private List<Canvas> green_start_list;
        private List<Canvas> red_start_list;

        private List<Canvas> blue_safe_zone_list;
        private List<Canvas> yellow_safe_zone_list;
        private List<Canvas> green_safe_zone_list;
        private List<Canvas> red_safe_zone_list;

        //This will keep count of home many pawns are in their home squares (Need to add an implementation of this for non AI)
        private int[] num_pawns_home;

        //Handles keyboard presses
        KeyEventHandler key_up_handler;


        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public MainPage()
        {
            first = true;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            this.InitializeComponent();
            this.IsTabStop = true;
            //index_of_current_player = -1;
            //color_of_current_turn = Color.RED;
            card_drawn = false;
            cur_selected_square = -1;
            cur_selected_img = null;
            cur_pawn_selection = -1;
            pawns_available = new List<bool>();
            pawns_available.Add(false);
            pawns_available.Add(false);
            pawns_available.Add(false);
            pawns_available.Add(false);
            color_adjustment = 60 + 6 * ((int)color_of_current_turn);
            how_to_highlighted = false;
            myOptions = new List<Tuple<Pawn, List<Tuple<Square,ComboData.move>>>>();
            current_spot = -1;
            forefeit_disabled = false;

            times_ticked = 0;
            move_canvas = new Canvas();
            move_canvas.Height = 100;
            move_canvas.Width = 100;
            game_grid.Children.Add(move_canvas);

            key_up_handler = new KeyEventHandler(App_KeyUp);
            Window.Current.Content.AddHandler(UIElement.KeyUpEvent, key_up_handler, true);

            Loaded += delegate { this.Focus(Windows.UI.Xaml.FocusState.Programmatic); };

            //Initialize count of home, if respective players number reaches 4 he won.
            num_pawns_home = new int[4];
            for (int z = 0; z < 4; z++)
            {

                num_pawns_home[z] = 0;

            }
            
            // TODO: Delete once we can pass the game from the setup screen
            /*
            List<Player> players = new List<Player>();
            Player redPlayer = new Player("Nick", Color.RED, true, true);
            Player bluePlayer = new Player("Nicole" , Color.BLUE, true, true);
            Player yellowPlayer = new Player("Max", Color.YELLOW, true, true);
            Player greenPlayer = new Player("Stephen" , Color.GREEN, true, true);

            players.Add(redPlayer);
            players.Add(bluePlayer);
            players.Add(yellowPlayer);
            players.Add(greenPlayer);

            game = new Game(4, players);
            */
            


            //game = new Game(4);
            card_count = 0;

            pawn_square_list = new List<Canvas>();
            preview_square_list = new List<Canvas>();
           
            safe_zone_lists = new List<List<Canvas>>();
            start_lists = new List<List<Canvas>>();
            preview_safe_zone_lists = new List<List<Canvas>>();

            blue_start_list = new List<Canvas>();
            yellow_start_list = new List<Canvas>();
            green_start_list = new List<Canvas>();
            red_start_list = new List<Canvas>();

            blue_safe_zone_list = new List<Canvas>();
            yellow_safe_zone_list = new List<Canvas>();
            green_safe_zone_list = new List<Canvas>();
            red_safe_zone_list = new List<Canvas>();

            //creating and placing the layers of canvases
            init_pawn_square_list(pawn_square_list);
            init_pawn_square_list(preview_square_list);
            init_start_zones();

            //public enum Color { RED, BLUE, YELLOW, GREEN, WHITE }
            //order here important, can use current turn color as index!
            safe_zone_lists.Add(red_safe_zone_list);
            safe_zone_lists.Add(blue_safe_zone_list);
            safe_zone_lists.Add(yellow_safe_zone_list);
            safe_zone_lists.Add(green_safe_zone_list);

            init_safe_zones(safe_zone_lists);

            preview_safe_zone_lists.Add(new List<Canvas>());
            preview_safe_zone_lists.Add(new List<Canvas>());
            preview_safe_zone_lists.Add(new List<Canvas>());
            preview_safe_zone_lists.Add(new List<Canvas>());

            init_safe_zones(preview_safe_zone_lists);

            start_lists.Add(red_start_list);
            start_lists.Add(blue_start_list);
            start_lists.Add(yellow_start_list);
            start_lists.Add(green_start_list);

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    switch (i)
                    {
                        case 0:
                            update_pawn_square(j, (Color)i, red_start_list, j+1);
                            break;
                        case 1:
                            update_pawn_square(j, (Color)i, blue_start_list, j+1);
                            break;
                        case 2:
                            update_pawn_square(j, (Color)i, yellow_start_list, j+1);
                            break;
                        case 3:
                            update_pawn_square(j, (Color)i, green_start_list, j+1);
                            break;
                        default:
                            break;
                    }
                                
            pass_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
            pass_button.BorderThickness = new Thickness(3, 3, 3, 3);
                    pass_button.IsEnabled = false;
                    //this.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                }
            
            }

            app_bar_open = false;
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
            game = MaKeyMeSorry.App.currentGame;
            //color_of_current_turn = game.players[index_of_current_player].get_pawn_color();
            index_of_current_player = game.get_start_index() - 1;
            change_turn(true);
            /*Square mySquare = game.board.get_square_at(6);
            ComboData comboData = new ComboData(ComboData.move.SWAP, mySquare);
            options_1.Visibility = Visibility.Visible;
            pawn_1.Text = "Pawn 1 Options:";
            options_1.Items.Add(comboData);
            options_1.SelectedIndex = 0;
            options_1.Focus(FocusState.Keyboard);
            cur_pawn_selection = 0;
            Debug.WriteLine(((ComboData)options_1.SelectedItem).move_choice + " space: " + ((ComboData)options_1.SelectedItem).square_location.get_index());*/
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
            MaKeyMeSorry.App.currentGame = game;
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


        void play_game()
        {
            /*if(first)
            {
                
                for (int i = 0; i < 4; i++)
                {
                    int color_test = 60 + 6 * (i);
                    for (int j = 0; j < 1; j++)
                    {
                        //testing card 11
                        //Debug.WriteLine(game.board.get_start_square((Color)i) - 2);
                        update_pawn_square(j, (Color)i, start_lists[i], 0);
                        update_pawn_square(game.board.get_start_square((Color)i) - 2, (Color)i, pawn_square_list, j+1);
                        game.players[i].pawns[j].move_to(game.board.get_square_at(game.board.get_start_square((Color)i) - 2));
                        //update_pawn_square(j, (Color)i, safe_zone_lists[i], j + 1);
                        //game.players[i].pawns[j].set_in_safe_zone(true);
                        //game.players[i].pawns[j].move_to(game.board.get_square_at(color_test + j));
                    }
                }
                first = false;
            }*/
            if (!card_drawn && (FocusManager.GetFocusedElement() != pass_button))
            {
                color_adjustment = 60 + 6 * ((int)color_of_current_turn);
                Debug.WriteLine("Return button pressed");
                card_drawn = true;
                my_card = draw_card();
                pass_highlighted = false;
                how_to_highlighted = false;
                new_game_higlighted = false;
                this.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                Debug.WriteLine("card value: " + my_card.get_value());
                myOptions = new List<Tuple<Pawn, List<Tuple<Square,ComboData.move>>>>();
                myOptions = game.get_move_options(color_of_current_turn, my_card);
                
                if(game.forfeit_enabled)
                {
                    pass_button.IsEnabled = true;
                }
                cover.Opacity = 0;
                player_turn.Text = game.players[index_of_current_player].get_player_name() + "'s Turn, Choose a Move!";
                deselect_all_buttons();
                if (game.get_player(color_of_current_turn).get_is_human())
                {
                    display_options(myOptions);
                }
                else{
                    display_options(myOptions);
                    /*
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    while (true)
                    {

                        if (stopwatch.ElapsedMilliseconds >= 100)
                        {

                            break;

                        }


                    }
                    play_game();*/
                }

            }
            else if (card_drawn && (FocusManager.GetFocusedElement() != pass_button))
            {
                if(myOptions.Count == 0 || cur_selected_square != -1)
                {
                    pass_highlighted = false;
                    how_to_highlighted = false;
                    new_game_higlighted = false;
                    pass_button.IsEnabled = false;
                    current_spot = cur_selected_square;
                    hide_selected_move(cur_selected_square);//, pawn_square_list);
                    from_safe_zone = false;
                    apply_card_real(my_card);
                    //Check to see if won
                    check_if_win();

                    change_turn();
                    pawn_1.Text = "";
                    pawn_2.Text = "";
                    pawn_3.Text = "";
                    pawn_4.Text = "";
                    options_1.Visibility = Visibility.Collapsed;
                    options_1.Items.Clear();
                    options_2.Visibility = Visibility.Collapsed;
                    options_2.Items.Clear();
                    options_3.Visibility = Visibility.Collapsed;
                    options_3.Items.Clear();
                    options_4.Visibility = Visibility.Collapsed;
                    options_4.Items.Clear();
                    no_options.Visibility = Visibility.Collapsed;
                    this.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                    deselect_all_buttons();
                    pawns_available.Clear();
                    pawns_available.Add(false);
                    pawns_available.Add(false);
                    pawns_available.Add(false);
                    pawns_available.Add(false);
                    if (!game.get_player(color_of_current_turn).get_is_human())
                    {
                        //play_game();
                    }
                }
                
            }
        }

        private void check_if_win()
        {

            if (num_pawns_home[(int)color_of_current_turn] == 4)
            {

                end_game_options_Click();

            }

        }

        bool app_bar_open;
        int current_menu_item_index;

        private void App_KeyUp(object sender, KeyRoutedEventArgs e)
        {

            Debug.WriteLine("Keyboard button pressed");
            /*if (first)
            {
                new_game_button.Focus(FocusState.Keyboard);
            }*/

            if (e.Key == Windows.System.VirtualKey.Space)
            {
                Debug.WriteLine("Space button pressed");
                if ((FocusManager.GetFocusedElement() == pass_button))
                {
                    pass_button_Click();
                }
                else
                {
                    play_game();
                }
            }

            if (e.Key == Windows.System.VirtualKey.Right)
            {
                if (app_bar_open)
                {
                    change_selected_menu_button_right();
                }
                else
                {
                    change_selected_pawn_box_right();
                }
                e.Handled = true;
            }

            if (e.Key == Windows.System.VirtualKey.Left)
            {
                if (app_bar_open)
                {
                    change_selected_menu_button_left();
                }
                else
                {
                    change_selected_pawn_box_left();
                }
                e.Handled = true;
            }

            if (e.Key == Windows.System.VirtualKey.W)
            {
                if (app_bar_open)
                {
                    app_bar.IsOpen = false;
                    app_bar_open = false;
                    get_selected_UI();
                    if(pawns_available[0])
                    {
                        options_1.SelectedIndex = 0;
                        options_1.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 0;
                    }
                    else if (pawns_available[1])
                    {
                        options_2.SelectedIndex = 0;
                        options_2.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 1;
                    }
                    else if (pawns_available[2])
                    {
                        options_3.SelectedIndex = 0;
                        options_3.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 2;
                    }
                    else if (pawns_available[3])
                    {
                        options_4.SelectedIndex = 0;
                        options_4.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 3;
                    }
                    else
                    {
                        this.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                    }
                    how_to_play_menu_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
                    new_game_menu_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
                    quit_menu_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
                }
                else
                {
                    how_to_play_menu_button.Focus(FocusState.Keyboard);
                    how_to_play_menu_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.White);
                    app_bar.IsOpen = true;
                    app_bar_open = true;
                    current_menu_item_index = 0;
                }

                e.Handled = true;
            }

        }

        private void change_selected_menu_button_left()
        {
            switch (current_menu_item_index)
            {
                case 0:
                    // Currently on the How To Play Button
                    how_to_play_menu_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
                    quit_menu_button.Focus(FocusState.Keyboard);
                    quit_menu_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.White);
                    current_menu_item_index = 2;
                    break;
                case 1:
                    // Currently on the New Game Button
                    new_game_menu_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
                    how_to_play_menu_button.Focus(FocusState.Keyboard);
                    how_to_play_menu_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.White);
                    current_menu_item_index--;
                    break;
                case 2:
                    // Currently on the Quit Button
                    quit_menu_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
                    new_game_menu_button.Focus(FocusState.Keyboard);
                    new_game_menu_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.White);
                    current_menu_item_index--;
                    break;
            }
        }

        private void change_selected_menu_button_right()
        {
            switch (current_menu_item_index)
            {
                case 0:
                    // Currently on the How To Play Button
                    how_to_play_menu_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
                    new_game_menu_button.Focus(FocusState.Keyboard);
                    new_game_menu_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.White);
                    current_menu_item_index++;
                    break;
                case 1:
                    // Currently on the New Game Button
                    new_game_menu_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
                    quit_menu_button.Focus(FocusState.Keyboard);
                    quit_menu_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.White);
                    current_menu_item_index++;
                    break;
                case 2:
                    // Currently on the Quit Button
                    quit_menu_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
                    how_to_play_menu_button.Focus(FocusState.Keyboard);
                    how_to_play_menu_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.White);
                    current_menu_item_index = 0;
                    break;
            }
        }


        private void change_selected_pawn_box_left()
            {
            //if any buttons are highlighted...change them back to normal
            deselect_all_buttons();
            int box_selected = get_selected_UI();

            Debug.WriteLine("box selected: " + box_selected);

            switch (box_selected)
            {
                case (0):
                    if (pass_button.IsEnabled)
                    {
                        highlight_forfeit_button();
                        cur_pawn_selection = -1;
                    }
                    else if (pawns_available[3])
                    {
                        options_4.SelectedIndex = 0;
                        options_4.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 3;
                    }
                    else if (pawns_available[2])
                    {
                        options_3.SelectedIndex = 0;
                        options_3.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 2;
                    }
                    else if (pawns_available[1])
                    {
                        options_2.SelectedIndex = 0;
                        options_2.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 1;
                    }            
                    else
                    {
                        options_1.SelectedIndex = 0;
                        options_1.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 0;
                    }
                    break;
                case (1):
                    if (pawns_available[0])
                    {
                        options_1.SelectedIndex = 0;
                        options_1.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 0;
                    }
                    else if (pass_button.IsEnabled)
                    {
                        highlight_forfeit_button();
                        cur_pawn_selection = -1;
                    }
                    else if (pawns_available[3])
                    {
                        options_4.SelectedIndex = 0;
                        options_4.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 3;
                    }
                    else if (pawns_available[2])
                    {
                        options_3.SelectedIndex = 0;
                        options_3.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 2;
                    }
                    else
                    {
                        options_2.SelectedIndex = 0;
                        options_2.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 1;
                    }
                    break;
                case (2):
                    if (pawns_available[1])
                    {
                        options_2.SelectedIndex = 0;
                        options_2.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 1;
                    }
                    else if (pawns_available[0])
                    {
                        options_1.SelectedIndex = 0;
                        options_1.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 0;
                    }
                    else if (pass_button.IsEnabled)
                    {
                        highlight_forfeit_button();
                        cur_pawn_selection = -1;
                    }
                    else if (pawns_available[3])
                    {
                        options_4.SelectedIndex = 0;
                        options_4.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 3;
                    }
                    else
                    {
                        options_3.SelectedIndex = 0;
                        options_3.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 2;
                    }
                    break;
                case (3):
                    if (pawns_available[2])
                    {
                        options_3.SelectedIndex = 0;
                        options_3.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 2;
                    }
                    else if (pawns_available[1])
                    {
                        options_2.SelectedIndex = 0;
                        options_2.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 1;
                    }
                    else if (pawns_available[0])
                    {
                        options_1.SelectedIndex = 0;
                        options_1.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 0;
                    }
                    else if (pass_button.IsEnabled)
                    {
                        highlight_forfeit_button();
                        cur_pawn_selection = -1;
                    }
                    else
                    {
                        options_4.SelectedIndex = 0;
                        options_4.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 3;
                    }
                    break;
                case (6):
                    Debug.WriteLine("inside 6");
                    if (pawns_available[3])
                    {
                        options_4.SelectedIndex = 0;
                        options_4.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 3;
                        pass_highlighted = false;
                    }
                    else if (pawns_available[2])
                    {
                        options_3.SelectedIndex = 0;
                        options_3.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 2;
                        pass_highlighted = false;
                    }
                    else if (pawns_available[1])
                    {
                        options_2.SelectedIndex = 0;
                        options_2.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 1;
                        pass_highlighted = false;
                    }
                    else if (pawns_available[0])
                    {
                        pass_highlighted = false;
                        options_1.SelectedIndex = 0;
                        options_1.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 0;
                    }
                    else
                    {
                        this.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                        pass_highlighted = false;
                    }

                    break;
                default:
                    if (pass_button.IsEnabled)
                    {
                        highlight_forfeit_button();
                        cur_pawn_selection = -1;
                    }
                    else
                    {
                        Debug.WriteLine("shouldn't happen 2");
                        this.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                        pass_highlighted = false;
                    }
                    break;
            }

            /*if (pawns_available[3] && (box_selected == 6))
            {
                options_4.SelectedIndex = 0;
                options_4.Focus(FocusState.Keyboard);
                cur_pawn_selection = 3;
                how_to_highlighted = false;
                pass_highlighted = false;
            }
            else if (pawns_available[2] && (box_selected == 3 || box_selected == 6))
            {
                options_3.SelectedIndex = 0;
                options_3.Focus(FocusState.Keyboard);
                cur_pawn_selection = 2;
                how_to_highlighted = false;
                pass_highlighted = false;
            }
            else if (pawns_available[1] && (box_selected == 2 || box_selected == 3 || box_selected == 6))
            {
                options_2.SelectedIndex = 0;
                options_2.Focus(FocusState.Keyboard);
                cur_pawn_selection = 1;
                how_to_highlighted = false;
                pass_highlighted = false;
            }
            else if (pawns_available[0] && (box_selected == 1 || box_selected == 2 || box_selected == 3 || box_selected == 6))
            {
                options_1.SelectedIndex = 0;
                options_1.Focus(FocusState.Keyboard);
                cur_pawn_selection = 0;
                how_to_highlighted = false;
                pass_highlighted = false;
            }/*
            else if ((box_selected == 0 || box_selected == 1 || box_selected == 2 || box_selected == 3))
            {
                if(pass_button.IsEnabled)
                {
                    highlight_forfeit_button();
            }
            else
            {
                    highlight_new_game_button();
            }
                hide_selected_move(cur_selected_square);
            }
            else if (box_selected == 4)
            {
                this.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                how_to_highlighted = false;
            }
            else if (box_selected == 5)
            {
                highlight_how_to_button();
                new_game_higlighted = false;
            }
            else if (box_selected == 6)
            {
                highlight_new_game_button();
                pass_highlighted = false;

            }
            else
            {
                if(pass_button.IsEnabled)
                {
                    highlight_forfeit_button();
                }
                else
                {
                    highlight_new_game_button();
                }
            } 
            else
            {
                if (pass_button.IsEnabled)
                {
                    highlight_forfeit_button();
                }
                else
                {
                    this.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                    pass_highlighted = false;
                }
            }*/
        }

        private void change_selected_pawn_box_right()
        {
            //if any buttons are highlighted...change them back to normal
            deselect_all_buttons();
            int box_selected = get_selected_UI();
            

            Debug.WriteLine("box selected: " + box_selected);
            switch(box_selected)
            {
                case(0):
                    if(pawns_available[1])
                    {
                        options_2.SelectedIndex = 0;
                        options_2.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 1;
                    } else if(pawns_available[2])
                    {
                        options_3.SelectedIndex = 0;
                        options_3.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 2;
                    } else if(pawns_available[3])
                    {
                        options_4.SelectedIndex = 0;
                        options_4.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 3;
                    } else if (pass_button.IsEnabled)
                    {
                        highlight_forfeit_button();
                        cur_pawn_selection = -1;
                    }
                    else
                    {
                        options_1.SelectedIndex = 0;
                        options_1.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 0;
                    }
                    break;
                case(1):
                    if (pawns_available[2])
                    {
                        options_3.SelectedIndex = 0;
                        options_3.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 2;
                    }
                    else if (pawns_available[3])
                    {
                        options_4.SelectedIndex = 0;
                        options_4.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 3;
                    }
                    else if (pass_button.IsEnabled)
                    {
                        highlight_forfeit_button();
                        cur_pawn_selection = -1;
                    }
                    else if (pawns_available[0])
                    {
                        options_1.SelectedIndex = 0;
                        options_1.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 0;
                    }
                    else 
                    {
                        options_2.SelectedIndex = 0;
                        options_2.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 1;
                    }
                    break;
                case(2):
                    if (pawns_available[3])
                    {
                        options_4.SelectedIndex = 0;
                        options_4.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 3;
                    }
                    else if (pass_button.IsEnabled)
                    {
                        highlight_forfeit_button();
                        cur_pawn_selection = -1;
                    }
                    else if (pawns_available[0])
                    {
                        options_1.SelectedIndex = 0;
                        options_1.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 0;
                    }
                    else if (pawns_available[1])
                    {
                        options_2.SelectedIndex = 0;
                        options_2.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 1;
                    }
                    else
                    {
                        options_3.SelectedIndex = 0;
                        options_3.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 2;
                    }
                    break;
                case(3):
                    if (pass_button.IsEnabled)
                    {
                        highlight_forfeit_button();
                        cur_pawn_selection = -1;
                    }
                    else if (pawns_available[0])
                    {
                        options_1.SelectedIndex = 0;
                        options_1.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 0;
                    }
                    else if (pawns_available[1])
                    {
                        options_2.SelectedIndex = 0;
                        options_2.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 1;
                    }
                    else if (pawns_available[2])
                    {
                        options_3.SelectedIndex = 0;
                        options_3.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 2;
                    }
                    else
                    {
                        options_4.SelectedIndex = 0;
                        options_4.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 3;
                    }
                    break;
                case(6):
                    Debug.WriteLine("inside 6");
                    if(pawns_available[0])
                    {
                        pass_highlighted = false;
                        Debug.WriteLine("1 is avaliable");
                        options_1.SelectedIndex = 0;
                        options_1.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 0;
                    }
                    else if (pawns_available[1])
                    {
                        options_2.SelectedIndex = 0;
                        options_2.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 1;
                        pass_highlighted = false;
                    }
                    else if (pawns_available[2])
                    {
                        options_3.SelectedIndex = 0;
                        options_3.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 2;
                        pass_highlighted = false;
                    }
                    else if (pawns_available[3])
                    {
                        options_4.SelectedIndex = 0;
                        options_4.Focus(FocusState.Keyboard);
                        cur_pawn_selection = 3;
                        pass_highlighted = false;
                    }
                    else 
                    {
                        Debug.WriteLine("shouldn't happen 1");
                        this.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                        pass_highlighted = false;
                    }

                    break;
                default:
                    if (pass_button.IsEnabled)
                    {
                        highlight_forfeit_button();
                        cur_pawn_selection = -1;
                    }
                    else
                    {
                        Debug.WriteLine("shouldn't happen 2");
                        this.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                        pass_highlighted = false;
                    }
                    break;
            }
            /*
            if (pawns_available[0] && box_selected == 6)
            {
                options_1.SelectedIndex = 0;
                options_1.Focus(FocusState.Keyboard);
                cur_pawn_selection = 0;
                how_to_highlighted = false;
                pass_highlighted = false;
            }
            else if (pawns_available[1] && (box_selected == 0 || box_selected == 6))
            {
                options_2.SelectedIndex = 0;
                options_2.Focus(FocusState.Keyboard);
                cur_pawn_selection = 1;
                how_to_highlighted = false;
                pass_highlighted = false;
            }
            else if (pawns_available[2] && (box_selected == 0 || box_selected == 1 || box_selected == 6))
            {
                options_3.SelectedIndex = 0;
                options_3.Focus(FocusState.Keyboard);
                cur_pawn_selection = 2;
                how_to_highlighted = false;
                pass_highlighted = false;
            }
            else if (pawns_available[3] && (box_selected == 0 || box_selected == 1 || box_selected == 2 || box_selected == 6))
            {
                options_4.SelectedIndex = 0;
                options_4.Focus(FocusState.Keyboard);
                cur_pawn_selection = 3;
                how_to_highlighted = false;
                pass_highlighted = false;
            }
            /*else if ((box_selected == 0 || box_selected == 1 || box_selected == 2 || box_selected == 3))
            {
                highlight_how_to_button();
                hide_selected_move(cur_selected_square);
            }
            else if (box_selected == 4)
            {
                highlight_new_game_button();
                how_to_highlighted = false;
            }
            else if (box_selected == 5)
            {
                if(pass_button.IsEnabled)
            {
                    highlight_forfeit_button();
                new_game_higlighted = false;
                }
                else
                {
                pass_highlighted = true;
                    new_game_higlighted = false;
                    change_selected_pawn_box_right();
                }
            }
            else if (box_selected == 6)
            {
                this.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                pass_highlighted = false;
            }
            else
            {
                highlight_how_to_button();
            }
            else
            {
                if (pass_button.IsEnabled)
                {
                    highlight_forfeit_button();
                }
                else
                {
                    this.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                    pass_highlighted = false;
                }
            }*/
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        
        private void ComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            color_adjustment = 60 + 6 * ((int)color_of_current_turn);
            ComboBox comboBox = (ComboBox)sender;
            int pawnSelected = -1;
            if (options_1.SelectedIndex != -1)
            {
                pawnSelected = 1;
            }
            else if (options_2.SelectedIndex != -1)
            {
                pawnSelected = 2;
            }
            else if (options_3.SelectedIndex != -1)
            {
                pawnSelected = 3;
            }
            else if (options_4.SelectedIndex != -1)
            {
                pawnSelected = 4;
            }

            //maybe this should be cur_selected_image ***nick****????
            if(comboBox.SelectedIndex != -1)
            {
                if(cur_selected_square >= 60)
                {
                    hide_selected_move(cur_selected_square);// - color_adjustment, safe_zone_lists[(int)color_of_current_turn]);
                }
                else
                {
                    //check if swap and if either location is a slide;

                    hide_selected_move(cur_selected_square);//, pawn_square_list);
                }

                //if (Convert.ToInt32(comboBox.SelectedValue) >= 60)
                //Debug.WriteLine("something " + ((ComboData)options_1.SelectedItem).square_location.get_index());

                if (Convert.ToInt32(((ComboData)comboBox.SelectedItem).square_location.get_index()) >= 60)

                {
                    Debug.WriteLine(Convert.ToInt32(((ComboData)comboBox.SelectedItem).square_location.get_index()) - color_adjustment);
                    show_selected_move(Convert.ToInt32(((ComboData)comboBox.SelectedItem).square_location.get_index()), pawnSelected);// - color_adjustment, safe_zone_lists[(int)color_of_current_turn]);
                    //Debug.WriteLine(Convert.ToInt32(comboBox.SelectedValue)- color_adjustment);
                    //show_selected_move(Convert.ToInt32(comboBox.SelectedValue), pawnSelected);
                    current_move_type = ((ComboData)comboBox.SelectedItem).move_choice;
                }
                else
                {
                    if(((ComboData)comboBox.SelectedItem).move_choice == ComboData.move.SWAP)
                    {
                       int swap_to_index = Convert.ToInt32(((ComboData)comboBox.SelectedItem).square_location.get_index());
                        Color temp_color = game.board.get_square_at(swap_to_index).get_pawn_in_square().get_color();
                        int swap_pawn_index = game.board.get_square_at(swap_to_index).get_pawn_in_square().get_id() + 1;
                       Player current_player = game.get_player(color_of_current_turn);
                       int my_index = current_player.pawns[pawnSelected-1].get_current_location().get_index();

                        //they are swapping to my location
                        show_selected_move(my_index, swap_pawn_index, temp_color, false, true);

                        //I'm swapping to this location, send my color
                        show_selected_move(Convert.ToInt32(((ComboData)comboBox.SelectedItem).square_location.get_index()), pawnSelected, color_of_current_turn, true, false);//, pawn_square_list);
                        
                        
;
                    }
                    else
                    {
                        show_selected_move(Convert.ToInt32(((ComboData)comboBox.SelectedItem).square_location.get_index()), pawnSelected, color_of_current_turn);//, pawn_square_list);
                    }
                    //show_selected_move(Convert.ToInt32(comboBox.SelectedValue), pawnSelected);//, pawn_square_list);
                    current_move_type = ((ComboData)comboBox.SelectedItem).move_choice;


                }
               // Debug.WriteLine("Current Sel val: " + Convert.ToInt32(comboBox.SelectedValue));
        }
            Debug.WriteLine("Current index: " + comboBox.SelectedIndex);

        }

        private string color_to_string(Color color)
        {
            switch (color)
            {
                case(Color.BLUE):
                    return ("Blue");           
                case(Color.YELLOW):
                    return ("Yellow");              
                case (Color.RED):
                    return ("Red");                   
                case (Color.GREEN):
                    return ("Green");                    
                case (Color.WHITE):
                    return ("White");                   
                default:
                    return ("");
            }
        }

        private void draw_card(object sender, TappedRoutedEventArgs e)
        {
            play_game();
        }

        private Card draw_card()
        {
            Card card = game.deck.draw_card();
            string card_val = card.get_value().ToString();
            string flipped_extenstion;

            int temp = (int)color_of_current_turn;
            switch (temp)
            {
                //public enum Color { RED, BLUE, YELLOW, GREEN, WHITE }
           
                case (0):
                    flipped_extenstion = "Red.png";
                    card_count++;
                    break;
                case (1):
                    flipped_extenstion = "Blue.png";
                    card_count++;
                    break;
                case (2):
                    flipped_extenstion = "Yellow.png";
                    card_count++;
                    break;
                case (3):
                    flipped_extenstion = "Green.png";
                    card_count++;
                    break;
                default:
                    flipped_extenstion = "";
                    break;
            }
            if (card_val == "13")
            {
                card_val = "Sorry";
            }

            string uri_string = "ms-appx:///Assets/Card Images/";
            uri_string += card_val;
            uri_string += " Card ";
            uri_string += flipped_extenstion;

            ImageBrush ib = new ImageBrush();
            Uri uri = new Uri(uri_string, UriKind.Absolute);
            ib.ImageSource = new BitmapImage(uri);
            used_deck.Background = ib;

            Debug.WriteLine("URI STRING: " + uri_string);
            Debug.WriteLine("cardvale: " + card_count + "  temp: " + temp + " Color of current turn:" + color_of_current_turn.ToString());

            uri_string = "ms-appx:///Assets/Card Back/Card Back ";

            if (card_count == 43)
            {
                uri_string += "3 Left.png";
            }
            else if (card_count == 44)
            {
                uri_string += "2 Left.png";
            }
            else if (card_count == 45)
            {
                uri_string += "1 Left.png";
                card_count = 1;
            }
            else
            {
                uri_string += "4 Left.png";
            }

            Debug.WriteLine("URI STRING: " + uri_string);

            //uri_string += back_extenstion;
            Uri uri2 = new Uri(uri_string, UriKind.Absolute);
            ImageBrush ib2 = new ImageBrush();
            ib2.ImageSource = new BitmapImage(uri2);
            full_deck.Background = ib2;

            return card;
        }

        void display_options(List<Tuple<Pawn, List<Tuple<Square,ComboData.move>>>> options)
        {
            // Display options
            /***********NICK*****************/

            pawns_available.Clear();
            pawns_available.Add(false);
            pawns_available.Add(false);
            pawns_available.Add(false);
            pawns_available.Add(false);

            if (options.Count == 0)
            {
                no_options.Visibility = Visibility.Visible;
            }

            foreach(Tuple<Pawn, List<Tuple<Square,ComboData.move>>> option in options)
            {
                int index = 1;
                switch(option.Item1.get_id())
                {
                    case 0:
                        options_1.Visibility = Visibility.Visible;
                        pawn_1.Text = "Pawn 1 Options:";
                        foreach (Tuple<Square, ComboData.move> mySquare in option.Item2)
                        {
                            ComboData comboData = new ComboData(mySquare.Item2, mySquare.Item1,index, option.Item2.Count);
                            options_1.Items.Add(comboData);
                            index++;
                        }
                        pawns_available[0] = true;
                        break;
                    case 1:
                        pawn_2.Text = "Pawn 2 Options:";
                        options_2.Visibility = Visibility.Visible;
                        foreach (Tuple<Square, ComboData.move> mySquare in option.Item2)
                        {
                            ComboData comboData = new ComboData(mySquare.Item2, mySquare.Item1,index,option.Item2.Count);
                            options_2.Items.Add(comboData);
                            index++;
                        }
                        pawns_available[1] = true;
                        break;
                    case 2:
                        pawn_3.Text = "Pawn 3 Options:";
                        options_3.Visibility = Visibility.Visible;
                        foreach (Tuple<Square, ComboData.move> mySquare in option.Item2)
                        {
                            ComboData comboData = new ComboData(mySquare.Item2, mySquare.Item1,index, option.Item2.Count);
                            options_3.Items.Add(comboData);
                            index++;
                        }
                        pawns_available[2] = true;
                        break;
                    case 3:
                        pawn_4.Text = "Pawn 4 Options:";
                        options_4.Visibility = Visibility.Visible;
                        foreach (Tuple<Square, ComboData.move> mySquare in option.Item2)
                        {
                            ComboData comboData = new ComboData(mySquare.Item2, mySquare.Item1,index, option.Item2.Count);
                            options_4.Items.Add(comboData);
                            index++;
                        }
                        pawns_available[3] = true;
                        break;
                    default:
                        break;
                }
            }

            for(int i = 0; i < 4; i++)
            {
                if(pawns_available[i])
                {
                    switch(i)
                    {
                        case 0:
                            options_1.Focus(FocusState.Keyboard);
                            options_1.SelectedIndex = 0;
                            cur_pawn_selection = 0;
                            break;
                        case 1:
                            options_2.Focus(FocusState.Keyboard);
                            options_2.SelectedIndex = 0;
                            cur_pawn_selection = 1;
                            break;
                        case 2:
                            options_3.Focus(FocusState.Keyboard);
                            options_3.SelectedIndex = 0;
                            cur_pawn_selection = 2;
                            break;
                        case 3:
                            options_4.Focus(FocusState.Keyboard);
                            options_4.SelectedIndex = 0;
                            cur_pawn_selection = 3;
                            break;
                        default:
                            break;
                    }
                    pass_highlighted = false;
                    pass_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
                    pass_button.BorderThickness = new Thickness(3, 3, 3, 3);
                    break;
                }
            }
        }

            /********************************/
         /*   if (options.Count >= 1)
            {
                pawn_1.Text = "Pawn 1 Options:";
                Tuple<Pawn, List<Square>> pawnOptions = options.ElementAt(0);
                options_1.Visibility = Visibility.Visible;
                foreach (Square mySquare in pawnOptions.Item2)
                {
                    String option = "" + mySquare.get_index();//"Move to square: " + mySquare.get_index();
                    
                    options_1.Items.Add(new Tuple<String, Pawn>(mySquare.get_index().ToString(), options.ElementAt(0).Item1));
                    //options_1.Items.
                }
                options_1.Focus(FocusState.Keyboard);
                options_1.SelectedIndex = 0;
                
                Debug.WriteLine("SELECTED SQUARE " + Convert.ToInt32(options_1.SelectedValue));
                //show_selected_move(Convert.ToInt32(options_1.SelectedValue),pawn_square_list);
                //cur_selected_square = (int)options_1.SelectedItem;
                Debug.WriteLine("CURRENT SELECTED SQUARE" + cur_selected_square);
                //options_1.SelectedIndexChanged += new System.EventHandler(ComboBox_SelectionChanged);

            }
            if (options.Count >= 2)
            {
                pawn_2.Text = "Pawn 2 Options:";
                Tuple<Pawn, List<Square>> pawnOptions = options.ElementAt(1);
                options_2.Visibility = Visibility.Visible;
                foreach (Square mySquare in pawnOptions.Item2)
                {
                    String option = "Move to square: " + mySquare.get_index();
                    options_2.Items.Add(option);
                }
            }
            if (options.Count >= 3)
            {
                pawn_3.Text = "Pawn 3 Options:";
                Tuple<Pawn, List<Square>> pawnOptions = options.ElementAt(2);
                options_3.Visibility = Visibility.Visible;
                foreach (Square mySquare in pawnOptions.Item2)
                {
                    String option = "Move to square: " + mySquare.get_index();
                    options_3.Items.Add(option);
                }
            }
            if (options.Count >= 5)
            {
                pawn_4.Text = "Pawn 4 Options:";
                Tuple<Pawn, List<Square>> pawnOptions = options.ElementAt(3);
                options_4.Visibility = Visibility.Visible;
                foreach (Square mySquare in pawnOptions.Item2)
                {
                    String option = "Move to square: " + mySquare.get_index();
                    options_4.Items.Add(option);
                }
            }
        }*/

        void apply_card(Card card){
            
            //game.players.ElementAt(1).get_pawn_from_start()

            //update_pawn_square(game.players.ElementAt(1).get_pawn_from_start().get_id()+5, Color.BLUE, blue_safe_zone_list);
            //game.players.ElementAt(1).get_pawn_from_start().move_to(game.board.get_square_at(game.players.ElementAt(1).get_pawn_from_start().get_id()));
            /*************************** NICK'S SECITION *******************************/
            Square currentSquare;
            Square moveToSquare;

            Debug.WriteLine("card value: " + card.get_value());
            List<Tuple<Pawn, List<Tuple<Square,ComboData.move>>>> options = new List<Tuple<Pawn, List<Tuple<Square,ComboData.move>>>>();
            options = game.get_move_options(color_of_current_turn, card);

            //display_options(options);

/*
            if (card.get_value() != 13)
            {

                int pawnIndex = 0;
                int color_adjustment = 60 + 6 * ((int)color_of_current_turn);

                foreach (Tuple<Pawn, List<Square>> pawnChoice in options)
                {
                    Debug.WriteLine("Pawn " + pawnIndex + "'s choices");
                    foreach (Square mySquare in pawnChoice.Item2)
                    {
                        Debug.WriteLine("Choice: Move to square: " + mySquare.get_index());

                    }
                    pawnIndex++;
                    Debug.WriteLine("first choice of first pawn made");
                }
                if (options.Count != 0)
                {
                    int pawnChoice = 0;

                    for(int i = 0; i < options.Count; i++)
                    {
                        if(options.ElementAt(i).Item2.Count > 0)
                        {
                            pawnChoice = i;
                            break;
                        }
                    }

                    if (options.ElementAt(pawnChoice).Item2.Count != 0)
                    {
                    

                        currentSquare = options.ElementAt(pawnChoice).Item1.get_current_location();
                        moveToSquare = options.ElementAt(pawnChoice).Item2.ElementAt(0);

                        if (!options.ElementAt(pawnChoice).Item1.is_start())
                        {
                            Debug.WriteLine("PAWN NOT AT START!");
                            if (currentSquare.get_Type() == SquareKind.SAFE)
                            {
                                //update_pawn_square(currentSquare.get_index() - 66,color_of_current_turn, blue_safe_zone_list);
                                //pawn num doesn't matter
                                update_pawn_square(currentSquare.get_index() - color_adjustment, color_of_current_turn, safe_zone_lists[(int)color_of_current_turn],0);
                            }
                            else
                            {
                                //pawn num doesn't matter
                                update_pawn_square(currentSquare.get_index(), color_of_current_turn, pawn_square_list,0);
                            }

                        }
                        else
                        {
                            //update_pawn_square(options.ElementAt(pawnChoice).Item1.get_id(), color_of_current_turn, blue_start_list);
                            //pawn num doesn't matter
                            update_pawn_square(options.ElementAt(pawnChoice).Item1.get_id(), color_of_current_turn, start_lists[(int)color_of_current_turn],0);

                        }
                        Debug.WriteLine("PAWN 0's location: " + options.ElementAt(pawnChoice).Item1.get_current_location());
                        //if (options.ElementAt(0).Item2.Count != 0)
                        //{
                        //moved up into else
                        //    if (options.ElementAt(0).Item1.is_start())
                        //    {
                        //    }
                        Debug.WriteLine("PAWN 0's move to location: " + options.ElementAt(pawnChoice).Item2.ElementAt(0).get_index());

                        if (moveToSquare.get_Type() == SquareKind.SAFE)//|| moveToSquare.get_Type() == SquareKind.HOMESQ)
                        {
                            update_pawn_square(moveToSquare.get_index() - color_adjustment, color_of_current_turn, safe_zone_lists[(int)color_of_current_turn], options.ElementAt(pawnChoice).Item1.get_id()+1);
                            options.ElementAt(pawnChoice).Item1.set_in_safe_zone(true);

                        }
                        else if (moveToSquare.get_Type() == SquareKind.HOMESQ)
                        {
                            //update_pawn_square(moveToSquare.get_index() + options.ElementAt(pawnChoice).Item1.get_id() - 66, color_of_current_turn, blue_safe_zone_list);
                            update_pawn_square(moveToSquare.get_index() + options.ElementAt(pawnChoice).Item1.get_id() - color_adjustment, color_of_current_turn,
                                safe_zone_lists[(int)color_of_current_turn], options.ElementAt(pawnChoice).Item1.get_id()+1);
                        }
                        else
                        {
                            //send someone home here if they are in the square you want!
                            if (moveToSquare.get_has_pawn())
                            {
                                //send the visual pawn to start square
                                update_pawn_square(moveToSquare.get_pawn_in_square().get_id(), moveToSquare.get_pawn_in_square().get_color(), start_lists[(int)moveToSquare.get_pawn_in_square().get_color()], moveToSquare.get_pawn_in_square().get_id() + 1);
                                //send the data pawn to start state
                                moveToSquare.get_pawn_in_square().sorry();

                                //to keep update_pawn_the same i call it twice, we could just change the update pawn square function though
                                //first call sets square image brush to nill;
                                //pawn num doesn't matter here
                                update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, 0);
                            }
                            //second call sets square image brush to pawn we want
                            //or first if no one was there in the first place
                            update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, options.ElementAt(pawnChoice).Item1.get_id()+1);
                        }
                        //update_pawn_square(moveToSquare.get_index(), Color.BLUE);
                        options.ElementAt(pawnChoice).Item1.move_to(options.ElementAt(pawnChoice).Item2.ElementAt(0));
                    }


                    
                }


            }
            else if (card.get_value() == 13)
            {
                int pawnIndex = 0;
                color_adjustment = 60 + 6 * ((int)color_of_current_turn);


                if (options.Count != 0)
                {
                    int pawnChoice = -1;

                    for (int i = 0; i < options.Count; i++)
                    {
                        if (options.ElementAt(i).Item2.Count > 0)
                        {
                            if (!options.ElementAt(i).Item1.is_in_safe_zone())
                            {
                                pawnChoice = i;
                                break;
                            }
                        }
                    }



                    if(pawnChoice != -1)
                    {
                        currentSquare = options.ElementAt(pawnChoice).Item1.get_current_location();
                        moveToSquare = options.ElementAt(pawnChoice).Item2.ElementAt(0);

                        //pawn num doesn't matter here
                        update_pawn_square(currentSquare.get_index(), color_of_current_turn, pawn_square_list,0);

                        //send the visual pawn to start square
                        update_pawn_square(moveToSquare.get_pawn_in_square().get_id(), moveToSquare.get_pawn_in_square().get_color(), start_lists[(int)moveToSquare.get_pawn_in_square().get_color()], moveToSquare.get_pawn_in_square().get_id()+1);
                        //send the data pawn to start state
                        moveToSquare.get_pawn_in_square().sorry();

                        //to keep update_pawn_the same i call it twice, we could just change the update pawn square function though
                        //first call sets square image brush to nill;
                        //pawn num doesn't matter here
                        update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list,0);

                        //second call sets square image brush to pawn we want
                        //or first if no one was there in the first place
                        update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, options.ElementAt(pawnChoice).Item1.get_id()+1);

                        //update_pawn_square(moveToSquare.get_index(), Color.BLUE);
                        options.ElementAt(pawnChoice).Item1.move_to(options.ElementAt(pawnChoice).Item2.ElementAt(0));

                        //sorry someone, get pawn options to sorry someone here 
                    }


                    
                }

            }*/
            //change_turn();

            /*********************** NICK'S SECITION ***********************************/

        }
        private int get_AI_sorry_choice(int curPawn, int curHome, int enemyPawn, int enemyHome)
        {

            int result = 0;
            result = ((enemyHome - curPawn) - (enemyHome - enemyPawn)) + ((curHome - enemyPawn) - (curHome - curPawn));
            return result;

        }

        private Tuple<int, int> update_tuple(int levelOrPawn, int countOrMove)
        {

            return new Tuple<int, int>(levelOrPawn, countOrMove);

        }

        private void execute_update(List<Tuple<Pawn, List<Tuple<Square, ComboData.move>>>> options, int Pawn, Square currentSquare, Square moveToSquare, int levelMove, Player currentPlayer)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (true)
            {

                if (stopwatch.ElapsedMilliseconds >= 100)
                {

                    break;

                }


            }
            show_selected_move(moveToSquare.get_index(), Pawn);
            stopwatch = Stopwatch.StartNew();
            while (true)
            {

                if (stopwatch.ElapsedMilliseconds >= 100)
                {

                    break;

                }


            }
            actual_execute_update(options, Pawn, currentSquare, moveToSquare, levelMove, currentPlayer);
        }
        private void actual_execute_update(List<Tuple<Pawn, List<Tuple<Square,ComboData.move>>>> options, int Pawn, Square currentSquare, Square moveToSquare, int levelMove, Player currentPlayer)
        {
            int pawnInSquare = options.ElementAt(Pawn).Item1.get_id();
            switch (levelMove)
            {

                case 0:
                    // Initiate a pawn to a location without any other pawns in it

                    update_pawn_square(currentPlayer.pawns[pawnInSquare].get_id(), color_of_current_turn, start_lists[(int)color_of_current_turn], 0);

                    update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, currentPlayer.pawns[pawnInSquare].get_id() + 1);

                    currentPlayer.pawns[pawnInSquare].move_to(moveToSquare);

                    break;
                case 1:
                    // Initiate a pawn to a location with a pawn in it

                    update_pawn_square(currentPlayer.pawns[pawnInSquare].get_id(), color_of_current_turn, start_lists[(int)color_of_current_turn], 0);

                    update_pawn_square(moveToSquare.get_pawn_in_square().get_id(), moveToSquare.get_pawn_in_square().get_color(), start_lists[(int)moveToSquare.get_pawn_in_square().get_color()], moveToSquare.get_pawn_in_square().get_id() + 1);

                    moveToSquare.get_pawn_in_square().sorry();

                    update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, 0);

                    update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, currentPlayer.pawns[pawnInSquare].get_id() + 1);

                    currentPlayer.pawns[pawnInSquare].move_to(moveToSquare);

                    break;
                case 2:
                    // Move a pawn to a safe location from a safe location

                    update_pawn_square(currentSquare.get_index() - color_adjustment, color_of_current_turn, safe_zone_lists[(int)color_of_current_turn], 0);

                    update_pawn_square(moveToSquare.get_index() - color_adjustment, color_of_current_turn, safe_zone_lists[(int)color_of_current_turn], currentPlayer.pawns[pawnInSquare].get_id() + 1);

                    currentPlayer.pawns[pawnInSquare].set_in_safe_zone(true);

                    currentPlayer.pawns[pawnInSquare].move_to(moveToSquare);

                    break;
                case 3:
                    // Move a pawn to Safe Location from not safe

                    update_pawn_square(currentSquare.get_index(), color_of_current_turn, pawn_square_list, 0);

                    update_pawn_square(moveToSquare.get_index() - color_adjustment, color_of_current_turn, safe_zone_lists[(int)color_of_current_turn], currentPlayer.pawns[pawnInSquare].get_id() + 1);

                    currentPlayer.pawns[pawnInSquare].set_in_safe_zone(true);

                    currentPlayer.pawns[pawnInSquare].move_to(moveToSquare);

                    break;
                case 4:
                    // Move a pawn to home location from safe 

                    update_pawn_square(currentSquare.get_index() - color_adjustment, color_of_current_turn, safe_zone_lists[(int)color_of_current_turn], 0);

                    update_pawn_square(moveToSquare.get_index() + currentPlayer.pawns[pawnInSquare].get_id() - color_adjustment, color_of_current_turn,
                    safe_zone_lists[(int)color_of_current_turn], currentPlayer.pawns[pawnInSquare].get_id() + 1);

                    num_pawns_home[(int)color_of_current_turn] = num_pawns_home[(int)color_of_current_turn] + 1;

                    currentPlayer.pawns[pawnInSquare].move_to(moveToSquare);

                    break;
                case 5:
                    // Move a pawn to home from not safe

                    update_pawn_square(currentSquare.get_index(), color_of_current_turn, pawn_square_list, 0);

                    update_pawn_square(moveToSquare.get_index() + currentPlayer.pawns[pawnInSquare].get_id() - color_adjustment, color_of_current_turn,
                    safe_zone_lists[(int)color_of_current_turn], currentPlayer.pawns[pawnInSquare].get_id() + 1);

                    num_pawns_home[(int)color_of_current_turn] = num_pawns_home[(int)color_of_current_turn] + 1;

                    currentPlayer.pawns[pawnInSquare].move_to(moveToSquare);

                    break;
                case 6:
                    // Move a pawn to square with no other pawns

                    update_pawn_square(currentSquare.get_index(), color_of_current_turn, pawn_square_list, 0);

                    update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, currentPlayer.pawns[pawnInSquare].get_id() + 1);

                    currentPlayer.pawns[pawnInSquare].move_to(moveToSquare);

                    break;

                case 7:
                    // Move a pawn to a square with a pawn in it

                    update_pawn_square(currentSquare.get_index(), color_of_current_turn, pawn_square_list, 0);

                    update_pawn_square(moveToSquare.get_pawn_in_square().get_id(), moveToSquare.get_pawn_in_square().get_color(), start_lists[(int)moveToSquare.get_pawn_in_square().get_color()], moveToSquare.get_pawn_in_square().get_id() + 1);

                    moveToSquare.get_pawn_in_square().sorry();

                    update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, 0);

                    update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, currentPlayer.pawns[pawnInSquare].get_id() + 1);

                    currentPlayer.pawns[pawnInSquare].move_to(moveToSquare);

                    break;
                case 8:

                    //Move a pawn to a not safe square from safe where there is no pawn

                    update_pawn_square(currentSquare.get_index() - color_adjustment, color_of_current_turn, safe_zone_lists[(int)color_of_current_turn], 0);

                    update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, currentPlayer.pawns[pawnInSquare].get_id() + 1);

                    currentPlayer.pawns[pawnInSquare].set_in_safe_zone(false);

                    currentPlayer.pawns[pawnInSquare].move_to(moveToSquare);

                    break;

                case 9:

                    //Move a pawn to not safe square from safe where there is a pawn

                    update_pawn_square(currentSquare.get_index() - color_adjustment, color_of_current_turn, safe_zone_lists[(int)color_of_current_turn], 0);

                    update_pawn_square(moveToSquare.get_pawn_in_square().get_id(), moveToSquare.get_pawn_in_square().get_color(), start_lists[(int)moveToSquare.get_pawn_in_square().get_color()], moveToSquare.get_pawn_in_square().get_id() + 1);

                    moveToSquare.get_pawn_in_square().sorry();

                    update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, 0);

                    update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, currentPlayer.pawns[pawnInSquare].get_id() + 1);

                    currentPlayer.pawns[pawnInSquare].set_in_safe_zone(false);

                    currentPlayer.pawns[pawnInSquare].move_to(moveToSquare);

                    break;

                case 10:
			
			        //Execute a swap when current Pawn is not in safe
			
			        update_pawn_square(currentSquare.get_index(), color_of_current_turn, pawn_square_list, 0);
			
			        update_pawn_square(currentSquare.get_index(), moveToSquare.get_pawn_in_square().get_color(), pawn_square_list, moveToSquare.get_pawn_in_square().get_id() + 1);
			
			        game.players[(int)moveToSquare.get_pawn_in_square().get_color()].pawns[moveToSquare.get_pawn_in_square().get_id()].move_to(currentSquare);
			
			        update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, 0);

                    update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, currentPlayer.pawns[pawnInSquare].get_id() + 1);

                    currentPlayer.pawns[pawnInSquare].move_to(moveToSquare);

                    break;

                case 11:

                    //Execute Sorry if pawn is at start
                    update_pawn_square(currentPlayer.pawns[pawnInSquare].get_id(), color_of_current_turn, start_lists[(int)color_of_current_turn], 0);
           
                    update_pawn_square(moveToSquare.get_pawn_in_square().get_id(), moveToSquare.get_pawn_in_square().get_color(), start_lists[(int)moveToSquare.get_pawn_in_square().get_color()], moveToSquare.get_pawn_in_square().get_id() + 1);
           
                    moveToSquare.get_pawn_in_square().sorry();

                    update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, 0);

                    update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, currentPlayer.pawns[pawnInSquare].get_id() + 1);

                    currentPlayer.pawns[pawnInSquare].move_to(moveToSquare);


                    break;
                case 12:

                    //Execute Sorry if pawn is on safe
                    update_pawn_square(currentSquare.get_index() - color_adjustment, color_of_current_turn, safe_zone_lists[(int)color_of_current_turn], 0);
                    currentPlayer.pawns[pawnInSquare].set_in_safe_zone(false);

                    update_pawn_square(moveToSquare.get_pawn_in_square().get_id(), moveToSquare.get_pawn_in_square().get_color(), start_lists[(int)moveToSquare.get_pawn_in_square().get_color()], moveToSquare.get_pawn_in_square().get_id() + 1);
           
                    moveToSquare.get_pawn_in_square().sorry();

                    update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, 0);

                    update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, currentPlayer.pawns[pawnInSquare].get_id() + 1);

                    currentPlayer.pawns[pawnInSquare].move_to(moveToSquare);


                    break;

                case 13:

                    //Execute Sorry if Pawn is neither
                    update_pawn_square(currentSquare.get_index(), color_of_current_turn, pawn_square_list, 0);

                    update_pawn_square(moveToSquare.get_pawn_in_square().get_id(), moveToSquare.get_pawn_in_square().get_color(), start_lists[(int)moveToSquare.get_pawn_in_square().get_color()], moveToSquare.get_pawn_in_square().get_id() + 1);
           
                    moveToSquare.get_pawn_in_square().sorry();

                    update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, 0);

                    update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, currentPlayer.pawns[pawnInSquare].get_id() + 1);

                    currentPlayer.pawns[pawnInSquare].move_to(moveToSquare);


                    break;

            }

        } //NEED TO ADD FOR OPTIMIZATION
        
        void apply_card_real(Card card)
        {
            /*************************** NICK'S SECITION *******************************/
            Square currentSquare;
            Square moveToSquare;
            Square moveToSquareTwo;

            Debug.WriteLine("card value: " + card.get_value());
            List<Tuple<Pawn, List<Tuple<Square,ComboData.move>>>> options = new List<Tuple<Pawn, List<Tuple<Square,ComboData.move>>>>();
            options = game.get_move_options(color_of_current_turn, card);

            /************************* Zoltan's Section *******************************/
            /**************************************************************************/

           
            //AI_on = true;

            if ((!game.get_player(color_of_current_turn).get_is_human()))// && AI_on)
            {
                var moveLevel = new Tuple<int, int>(0, 0);
                var bestMove = new Tuple<int, int>(0, 0);

                int levelOfMove = 11;
                int countOfMove = 0;
                int pawnOfBest = 5;
                int moveOfBest = 3;

                Player currentPlayer = game.get_player(color_of_current_turn);

                if ((card.get_value() == 1 || card.get_value() == 2) && options.Count == 1 && options.ElementAt(0).Item1.is_start())
                {
                    // This will happen if there isnt any other pawn in the board (for the computer whose turn it is)

                    currentSquare = options.ElementAt(0).Item1.get_current_location();
                    moveToSquare = options.ElementAt(0).Item2.ElementAt(0).Item1;

                    int color_adjustment = 60 + 6 * ((int)color_of_current_turn);

                    if (moveToSquare.get_has_pawn())
                    {

                        execute_update(options, 0, currentSquare, moveToSquare, 1, currentPlayer);

                    }
                    else
                    {

                        execute_update(options, 0, currentSquare, moveToSquare, 0, currentPlayer);

                    }


                }
                else if (options.Count == 1 )//|| (options.Count > 1 && (card.get_value() == 11 || card.get_value() == 13)))
                {

                    //This will yield the adequate move if there is already a single pawn on the board for the computer who's current turn it is

                    currentSquare = options.ElementAt(0).Item1.get_current_location();

                    int color_adjustment = 60 + 6 * ((int)color_of_current_turn);

                    if ((card.get_value() < 10) || card.get_value() == 12)
                    {
                        //This executes a normal movement for the pawn ( cards: 3, 4, 5, 8, 12 ) Also 7 since it will only be moving forward
                        Debug.WriteLine("gets here");

                        moveToSquare = options.ElementAt(0).Item2.ElementAt(0).Item1;

                        if (moveToSquare.get_Type() == SquareKind.SAFE)
                        {

                            if (currentSquare.get_Type() == SquareKind.SAFE)
                            {

                                execute_update(options, 0, currentSquare, moveToSquare, 2, currentPlayer);

                            }
                            else
                            {

                                execute_update(options, 0, currentSquare, moveToSquare, 3, currentPlayer);

                            }

                        }
                        else if (moveToSquare.get_Type() == SquareKind.HOMESQ)
                        {

                            if (currentSquare.get_Type() == SquareKind.SAFE)
                            {

                                execute_update(options, 0, currentSquare, moveToSquare, 4, currentPlayer);
                                num_pawns_home[index_of_current_player] = num_pawns_home[index_of_current_player] + 1;

                            }
                            else
                            {

                                execute_update(options, 0, currentSquare, moveToSquare, 5, currentPlayer);
                                num_pawns_home[index_of_current_player] = num_pawns_home[index_of_current_player] + 1;

                            }

                        }
                        else if (moveToSquare.get_has_pawn())
                        {
                            if(currentSquare.get_Type() == SquareKind.SAFE)
                            {

                                execute_update(options, 0, currentSquare, moveToSquare, 9, currentPlayer);

                            }
                            else
                            {

                                execute_update(options, 0, currentSquare, moveToSquare, 7, currentPlayer);

                            }

                        }
                        else
                        {
                            Debug.WriteLine("FAIL HERE");
                            if(currentSquare.get_Type() == SquareKind.SAFE)
                            {

                                execute_update(options, 0, currentSquare, moveToSquare, 8, currentPlayer);

                            }
                            else
                            {

                                execute_update(options, 0, currentSquare, moveToSquare, 6, currentPlayer);

                            }
                            
                        }

                    }
                    else if (card.get_value() == 10)
                    {

                        //Could add more constraints (based on cur position)


                        moveToSquare = options.ElementAt(0).Item2.ElementAt(0).Item1;
                        moveToSquareTwo = null;

                        bool twoElements = false;

                        if (options.ElementAt(0).Item2.Count() > 1)
                        {

                            moveToSquareTwo = options.ElementAt(0).Item2.ElementAt(1).Item1;
                            twoElements = true;

                        }

                        if (moveToSquare.get_has_pawn() && currentSquare.get_Type() != SquareKind.SAFE)
                        {

                            execute_update(options, 0, currentSquare, moveToSquare, 7, currentPlayer);

                        }
                        else if (twoElements)
                        {

                            if(moveToSquareTwo.get_has_pawn() && currentSquare.get_Type() != SquareKind.SAFE)
                            {

                                execute_update(options, 0, currentSquare, moveToSquareTwo, 7, currentPlayer);

                            }
                            /*else if(moveToSquareTwo.get_has_pawn() && currentSquare.get_Type() == SquareKind.SAFE)
                            {

                                execute_update(currentSquare, moveToSquareTwo, 9, currentPlayer);

                            }*///THIS SHOULD NEVER HAPPEN.
                            else
                            {
                                if(moveToSquare.get_Type() == SquareKind.SAFE)
                                {

                                    execute_update(options, 0, currentSquare, moveToSquare, 3, currentPlayer);

                                }
                                else
                                {

                                    execute_update(options, 0, currentSquare, moveToSquare, 6, currentPlayer);

                                }
                                

                            }

                        }
                        else
                        {
                            
                            if(currentSquare.get_Type() == SquareKind.SAFE)
                            {

                                if(moveToSquare.get_has_pawn() == true)
                                {

                                    execute_update(options, 0, currentSquare, moveToSquare, 9, currentPlayer);

                                }
                                else
                                {
                                    if(moveToSquare.get_Type() == SquareKind.SAFE)
                                    {

                                        execute_update(options, 0, currentSquare, moveToSquare, 2, currentPlayer);

                                    }
                                    else
                                    {

                                        execute_update(options, 0, currentSquare, moveToSquare, 8, currentPlayer);

                                    }
                                    
                                    
                                }
                            }
                            else
                            {

                                execute_update(options, 0, currentSquare, moveToSquare, 6, currentPlayer);

                            }
                            

                        }

                    }
                    else if (card.get_value() == 11)
                    {
                        // Need to update with priorities for game finished

                        moveToSquare = options.ElementAt(0).Item2.ElementAt(0).Item1;
                        
                        currentSquare = options.ElementAt(0).Item1.get_current_location();

                        int bestChoice = moveToSquare.get_index() - currentSquare.get_index();

                        bool noForward = true;

                        if(bestChoice < 0)
                        {

                            bestChoice = bestChoice + 60;

                        }
                        if (bestChoice == 11 || moveToSquare.get_Type() == SquareKind.SAFE)
                        {

                            if (moveToSquare.get_has_pawn() == true)
                            {

                                execute_update(options, 0, currentSquare, moveToSquare, 7, currentPlayer);

                            }
                            else if (moveToSquare.get_Type() == SquareKind.SAFE)
                            {

                                execute_update(options, 0, currentSquare, moveToSquare, 3, currentPlayer);

                            }
                            else
                            {

                                execute_update(options, 0, currentSquare, moveToSquare, 6, currentPlayer);

                            }
                            noForward = false;

                        }

                        int curBase;
                        curBase = game.board.get_start_square(currentSquare.get_pawn_in_square().get_color()) - 2;
                        int curDistToBase = curBase - currentSquare.get_index();
                        Square testToSquare;

                        if (curDistToBase < 0)
                        {

                            curDistToBase = curDistToBase + 60;

                        }

                        if (options.ElementAt(0).Item2.Count() > 1 && noForward == true)
                        {
                            
                            

                            int moveChoice = 0;
                            int chosenDist = 61;

                            int distToBaseTest = 61;

                            for (int i = 0; i < options.ElementAt(0).Item2.Count; i++)
                            {

                                testToSquare = options.ElementAt(0).Item2.ElementAt(i).Item1;

                                int testLocation;

                                testLocation = testToSquare.get_index();
                                distToBaseTest = curBase - testLocation;
                                if(distToBaseTest < 0)
                                {

                                    distToBaseTest = distToBaseTest + 60;

                                }

                                Debug.WriteLine("GOT HERREEEE AND THE DISTANCE ISSS " + distToBaseTest);

                                if(distToBaseTest < curDistToBase && distToBaseTest < chosenDist)
                                {

                                    chosenDist = distToBaseTest;
                                    moveChoice = i;

                                }

                            }

                            if(chosenDist < curDistToBase)
                            {

                                testToSquare = options.ElementAt(0).Item2.ElementAt(moveChoice).Item1;

                                execute_update(options, 0, currentSquare, testToSquare, 10, currentPlayer);

                            }

                        }
                        else if(noForward == true)
                        {

                            testToSquare = options.ElementAt(0).Item2.ElementAt(0).Item1;
                            int testLocation;

                            testLocation = testToSquare.get_index();
                            int distToBaseTest = curBase - testLocation;
                            if (distToBaseTest < 0)
                            {

                                distToBaseTest = distToBaseTest + 60;

                            }
                            if (distToBaseTest < curDistToBase)
                            {

                                execute_update(options, 0, currentSquare, testToSquare, 10, currentPlayer);

                            }


                        }
                        
                    }
                    else if(card.get_value() == 13)
                    {

                        
                        moveToSquare = options.ElementAt(0).Item2.ElementAt(0).Item1;
                        if(options.ElementAt(0).Item1.is_start() == true)
                        {

                            execute_update(options, 0, currentSquare, moveToSquare, 11, currentPlayer);

                        }
                        else if(currentSquare.get_Type() == SquareKind.SAFE)
                        {

                            execute_update(options, 0, currentSquare, moveToSquare, 12, currentPlayer);

                        }
                        else
                        {

                            execute_update(options, 0, currentSquare, moveToSquare, 13, currentPlayer);

                        }
                        

                    }

                }
                else if (options.Count > 1)
                {
                    Debug.WriteLine("HERE IT IS SEES OPTIONS");

                    int updateLevel; 
                    updateLevel = 15;

                    moveLevel = update_tuple(11, 0);
                    bestMove = update_tuple(5, 10);
                    for (int i = 0; i < options.Count; i++)
                    {
                        /*
                        if(options.Count > i)
                        {
                            Debug.WriteLine("TESTING THIS SHIATTTT");
                            int test = options.ElementAt(i + 1).Item1.get_id();
                            Debug.WriteLine("WELL THE ISSUE ISNT WITH I + 1 SINCE IT EQUALS PAWN: " + test);

                        }
                        */
                        currentSquare = options.ElementAt(i).Item1.get_current_location();

                        int curBase = game.board.get_start_square(color_of_current_turn) - 2;
                        int curDistToBase = 0;
                        if(options.ElementAt(i).Item1.is_start() != true)
                        {

                            curBase = game.board.get_start_square(currentSquare.get_pawn_in_square().get_color()) - 2;
                            curDistToBase = curBase - currentSquare.get_index();
                            if (curDistToBase < 0)
                            {

                                curDistToBase = curDistToBase + 60;

                            }

                        }
                        
                        
                        Debug.WriteLine("Our I value at this point is" + i);
                        for (int j = 0; j < options.ElementAt(i).Item2.Count; j++)
                        {
                            Debug.WriteLine("OUR COUNT FOR J IS : "+ options.ElementAt(i).Item2.Count);
                            Debug.WriteLine("OUT J VALUE AT THIS POINT IS: " + j);
                            moveToSquare = options.ElementAt(i).Item2.ElementAt(j).Item1;
                            int curCount;
                            if (card.get_value() != 11 && card.get_value() != 1 && card.get_value() != 2 && card.get_value() != 13)
                            {
                                if (moveToSquare.get_Type() == SquareKind.HOMESQ)
                                {
                                    levelOfMove = 1;
                                    countOfMove = 0;
                                    pawnOfBest = i;
                                    moveOfBest = j;

                                    //moveLevel = update_tuple(1, 0);
                                    //bestMove = update_tuple(i, j);
                                    Debug.WriteLine("Our PRESUMABLY UPDATED I IS: " + i);
                                    if(currentSquare.get_Type() == SquareKind.SAFE)
                                    {

                                        updateLevel = 4;

                                    }
                                    else
                                    {
                                        updateLevel = 5;

                                    }

                                }

                                else if (moveToSquare.get_Type() == SquareKind.SAFE && currentSquare.get_Type() != SquareKind.SAFE)
                                {
                                    curCount = curDistToBase;
                                    if (levelOfMove == 2)
                                    {

                                        if (countOfMove > curCount)
                                        {
                                            levelOfMove = 2;
                                            countOfMove = curCount;
                                            pawnOfBest = i;
                                            moveOfBest = j;
                                            //moveLevel = update_tuple(2, curCount);
                                            //bestMove = update_tuple(i, j);
                                            updateLevel = 3;
                                            Debug.WriteLine("Our PRESUMABLY UPDATED I IS: " + i);

                                        }
                                        
                                    }
                                    else if (levelOfMove > 2)
                                    {

                                        levelOfMove = 2;
                                        countOfMove = curCount;
                                        pawnOfBest = i;
                                        moveOfBest = j;
                                        updateLevel = 3;
                                        Debug.WriteLine("Our PRESUMABLY UPDATED I IS: " + i);

                                    }

                                }
                                else if (moveToSquare.get_has_pawn())
                                {
                                    curCount = curBase - moveToSquare.get_index();
                                    if(curCount < 0)
                                    {

                                        curCount = curCount + 60;

                                    }
                                    if (levelOfMove == 3)
                                    {

                                        if (countOfMove > curCount)
                                        {

                                            levelOfMove = 3;
                                            countOfMove = curCount;
                                            pawnOfBest = i;
                                            moveOfBest = j;
                                            if(currentSquare.get_Type() == SquareKind.SAFE)
                                            {

                                                updateLevel = 9;

                                            }
                                            else
                                            {

                                                updateLevel = 7;

                                            }
                                            Debug.WriteLine("Our PRESUMABLY UPDATED I IS: " + i);

                                        }

                                    }
                                    else if (levelOfMove > 3)
                                    {

                                        levelOfMove = 3;
                                        countOfMove = curCount;
                                        pawnOfBest = i;
                                        moveOfBest = j;
                                        if (currentSquare.get_Type() == SquareKind.SAFE)
                                        {

                                            updateLevel = 9;

                                        }
                                        else
                                        {

                                            updateLevel = 7;

                                        }

                                        Debug.WriteLine("Our PRESUMABLY UPDATED I IS: " + i);

                                    }


                                }
                                else
                                {

                                    curCount = curBase - moveToSquare.get_index();
                                    if (curCount < 0)
                                    {

                                        curCount = curCount + 60;

                                    }
                                    if (levelOfMove == 5)
                                    {

                                        if (countOfMove > curCount)
                                        {

                                            levelOfMove = 5;
                                            countOfMove = curCount;
                                            pawnOfBest = i;
                                            moveOfBest = j;
                                            if(currentSquare.get_Type() == SquareKind.SAFE)
                                            {

                                                updateLevel = 2;

                                            }
                                            else
                                            {

                                                updateLevel = 6;

                                            }

                                        }

                                    }
                                    else if (levelOfMove > 5)
                                    {

                                        levelOfMove = 5;
                                        countOfMove = curCount;
                                        pawnOfBest = i;
                                        moveOfBest = j;
                                        if (currentSquare.get_Type() == SquareKind.SAFE)
                                        {

                                            updateLevel = 2;

                                        }
                                        else
                                        {

                                            updateLevel = 6;

                                        }
                                    }

                                    

                                }
                                

                            }
                            else if (card.get_value() == 1 || card.get_value() == 2)
                            {

                                
                                if (options.ElementAt(i).Item1.is_start() == true)
                                {

                                    if (levelOfMove > 4)
                                    {
                                        curCount = 0;
                                        levelOfMove = 4;
                                        countOfMove = curCount;
                                        pawnOfBest = i;
                                        moveOfBest = j;
                                        if (moveToSquare.get_has_pawn() == true)
                                        {

                                            updateLevel = 1;

                                        }
                                        else
                                        {
                                            updateLevel = 0;

                                        }
                                    }

                                }
                                else if (moveToSquare.get_Type() == SquareKind.HOMESQ)
                                {

                                    levelOfMove = 1;
                                    countOfMove = 0;
                                    pawnOfBest = i;
                                    moveOfBest = j;

                                    if (currentSquare.get_Type() == SquareKind.SAFE)
                                    {

                                        updateLevel = 4;

                                    }
                                    else
                                    {
                                        updateLevel = 5;

                                    }

                                }
                                else if (moveToSquare.get_Type() == SquareKind.SAFE && currentSquare.get_Type() != SquareKind.SAFE)
                                {

                                    curCount = curDistToBase;
                                    if (curCount < 0)
                                    {

                                        curCount = curCount + 60;

                                    }
                                    if (levelOfMove == 2)
                                    {

                                        if (countOfMove > curCount)
                                        {
                                            levelOfMove = 2;
                                            countOfMove = curCount;
                                            pawnOfBest = i;
                                            moveOfBest = j;
                                            updateLevel = 3;

                                        }

                                    }
                                    else if (levelOfMove > 2)
                                    {

                                        levelOfMove = 2;
                                        countOfMove = curCount;
                                        pawnOfBest = i;
                                        moveOfBest = j;
                                        updateLevel = 3;

                                    }

                                }
                                else if (moveToSquare.get_has_pawn() == true)
                                {
                                    curCount = curBase - moveToSquare.get_index();
                                    if (curCount < 0)
                                    {

                                        curCount = curCount + 60;

                                    }
                                    if (levelOfMove == 3)
                                    {
                                        if (countOfMove > curCount)
                                        {

                                            levelOfMove = 3;
                                            countOfMove = curCount;
                                            pawnOfBest = i;
                                            moveOfBest = j;
                                            updateLevel = 7;

                                        }


                                    }
                                    else if (levelOfMove > 3)
                                    {

                                        levelOfMove = 3;
                                        countOfMove = curCount;
                                        pawnOfBest = i;
                                        moveOfBest = j;
                                        updateLevel = 7;

                                    }

                                }
                                else
                                {
                                    Debug.WriteLine("WELL AT LEAST IT GETS HERE RIGHT?");
                                    curCount = curBase - moveToSquare.get_index();
                                    if (curCount < 0)
                                    {

                                        curCount = curCount + 60;

                                    }
                                    if (levelOfMove == 5)
                                    {

                                        if (countOfMove > curCount)
                                        {

                                            levelOfMove = 5;
                                            countOfMove = curCount;
                                            pawnOfBest = i;
                                            moveOfBest = j;
                                            if (currentSquare.get_Type() == SquareKind.SAFE)
                                            {

                                                updateLevel = 2;

                                            }
                                            else
                                            {

                                                updateLevel = 6;

                                            }

                                        }

                                    }
                                    else if (levelOfMove > 5)
                                    {
                                        
                                        levelOfMove = 5;
                                        countOfMove = curCount;
                                        pawnOfBest = i;
                                        moveOfBest = j;
                                        if (currentSquare.get_Type() == SquareKind.SAFE)
                                        {

                                            updateLevel = 2;

                                        }
                                        else
                                        {

                                            updateLevel = 6;

                                        }
                                    }

                                }


                            }
                            else if(card.get_value() == 11)
                            {

                                int isEleven = moveToSquare.get_index() - currentSquare.get_index();
                                if(isEleven < 0)
                                {

                                    isEleven = isEleven + 60;

                                }
                                if(moveToSquare.get_Type() == SquareKind.HOMESQ)
                                {

                                    levelOfMove = 1;
                                    countOfMove = 0;
                                    pawnOfBest = i;
                                    moveOfBest = j;
                                    updateLevel = 5;


                                }
                                else if(moveToSquare.get_Type() == SquareKind.SAFE)
                                {

                                    curCount = curDistToBase;
                                    
                                    if (curCount < 0)
                                    {

                                        curCount = curCount + 60;

                                    }
                                    if (levelOfMove == 2)
                                    {

                                        if (countOfMove > curCount)
                                        {
                                            levelOfMove = 2;
                                            countOfMove = curCount;
                                            pawnOfBest = i;
                                            moveOfBest = j;
                                            updateLevel = 3;

                                        }

                                    }
                                    else if (levelOfMove > 2)
                                    {

                                        levelOfMove = 2;
                                        countOfMove = curCount;
                                        pawnOfBest = i;
                                        moveOfBest = j;
                                        updateLevel = 3;

                                    }


                                }
                                else if(isEleven == 11 && moveToSquare.get_has_pawn() == true)
                                {

                                    curCount = curBase - moveToSquare.get_index();
                                    if (curCount < 0)
                                    {

                                        curCount = curCount + 60;

                                    }
                                    if (levelOfMove == 3)
                                    {
                                        if (countOfMove > curCount)
                                        {

                                            levelOfMove = 3;
                                            countOfMove = curCount;
                                            pawnOfBest = i;
                                            moveOfBest = j;
                                            updateLevel = 7;

                                        }


                                    }
                                    else if (levelOfMove > 3)
                                    {

                                        levelOfMove = 3;
                                        countOfMove = curCount;
                                        pawnOfBest = i;
                                        moveOfBest = j;
                                        updateLevel = 7;

                                    }

                                }
                                else if(isEleven == 11)
                                {

                                    curCount = curBase - moveToSquare.get_index();
                                    if (curCount < 0)
                                    {

                                        curCount = curCount + 60;

                                    }
                                    if (levelOfMove == 5)
                                    {

                                        if (countOfMove > curCount)
                                        {

                                            levelOfMove = 5;
                                            countOfMove = curCount;
                                            pawnOfBest = i;
                                            moveOfBest = j;
                                            updateLevel = 6;


                                        }

                                    }
                                    else if (levelOfMove > 5)
                                    {

                                        levelOfMove = 5;
                                        countOfMove = curCount;
                                        pawnOfBest = i;
                                        moveOfBest = j;
                                        updateLevel = 6;

                                    }


                                }
                                else
                                {

                                    curCount = curBase - moveToSquare.get_index();
                                    if (curCount < 0)
                                    {

                                        curCount = curCount + 60;

                                    }
                                    if (levelOfMove == 6 || levelOfMove >6)
                                    {
                                        countOfMove = 25;
                                        if (countOfMove > curCount)
                                        {

                                            levelOfMove = 6;
                                            countOfMove = curCount;
                                            pawnOfBest = i;
                                            moveOfBest = j;
                                            updateLevel = 10;


                                        }

                                    }

                                }
                                  

                            }
                            if(card.get_value() == 13)
                            {

                                if (moveToSquare.get_has_pawn())
                                {
                                    curCount = curBase - moveToSquare.get_index();
                                    if (curCount < 0)
                                    {

                                        curCount = curCount + 60;

                                    }
                                    if (levelOfMove == 3)
                                    {

                                        if (countOfMove > curCount)
                                        {

                                            levelOfMove = 3;
                                            countOfMove = curCount;
                                            pawnOfBest = i;
                                            moveOfBest = j;
                                            if(options.ElementAt(i).Item1.is_start() == true)
                                            {

                                                updateLevel = 11;

                                            }
                                            else if (currentSquare.get_Type() == SquareKind.SAFE)
                                            {

                                                updateLevel = 12;

                                            }
                                            else
                                            {
                                                updateLevel = 13;
                                            }

                                        }

                                    }
                                    else if (levelOfMove > 3)
                                    {

                                        levelOfMove = 3;
                                        countOfMove = curCount;
                                        pawnOfBest = i;
                                        moveOfBest = j;
                                        if (options.ElementAt(i).Item1.is_start() == true)
                                        {

                                            updateLevel = 11;

                                        }
                                        else if (currentSquare.get_Type() == SquareKind.SAFE)
                                        {

                                            updateLevel = 12;

                                        }
                                        else
                                        {
                                            updateLevel = 13;
                                        }

                                    }


                                }
                                

                            }
                            
                        }

                    }
                    
                    Square fromChoice;
                    Square toChoice;
                    fromChoice = options.ElementAt(pawnOfBest).Item1.get_current_location();
                    toChoice = options.ElementAt(pawnOfBest).Item2.ElementAt(moveOfBest).Item1;

                    execute_update(options, pawnOfBest, fromChoice, toChoice, updateLevel, currentPlayer);
                }
                
            
                
            }
            else //if (game.get_player(color_of_current_turn).get_is_human())
            {
                Canvas current_canvas;
                Player currentPlayer = game.get_player(color_of_current_turn);
                if (card.get_value() != 13)
                {

                    int pawnIndex = 0;
                    int color_adjustment = 60 + 6 * ((int)color_of_current_turn);

                    if (options.Count != 0)
                    {
                        int pawnChoice = cur_pawn_selection;

                        if (card.can_move_forward() == 11 && current_move_type == ComboData.move.SWAP)
                        //if(false) //change back to above line once you switch the swapping issue
                        {

                            if (pawnChoice != -1)
                            {
                                Color temp_color;
                                //currentSquare = game.players[(int)color_of_current_turn].pawns[cur_pawn_selection].get_current_location();
                                currentSquare = game.get_player(color_of_current_turn).pawns[cur_pawn_selection].get_current_location();
                                moveToSquare = game.board.get_square_at(current_spot);
                                temp_color = moveToSquare.get_pawn_in_square().get_color();

                                //pawn num doesn't matter here
                                //remove me from UI
                                update_pawn_square(currentSquare.get_index(), color_of_current_turn, pawn_square_list, 0);

                                //send the visual pawn to start square
                                //update_pawn_square(moveToSquare.get_pawn_in_square().get_id(), moveToSquare.get_pawn_in_square().get_color(), start_lists[(int)moveToSquare.get_pawn_in_square().get_color()], moveToSquare.get_pawn_in_square().get_id() + 1);
                                //send the data pawn to start state
                                if (currentPlayer.pawns[cur_pawn_selection].is_in_safe_zone())
                                {   //this sould never get called?????
                                    update_pawn_square(currentSquare.get_index(), moveToSquare.get_pawn_in_square().get_color(), safe_zone_lists[(int)color_of_current_turn], moveToSquare.get_pawn_in_square().get_id() + 1);
                                }
                                else
                                {
                                    //put them in my UI spot
                                    update_pawn_square(currentSquare.get_index(), moveToSquare.get_pawn_in_square().get_color(), pawn_square_list, moveToSquare.get_pawn_in_square().get_id() + 1);
                                }

                                //put them in my spot programatically
                                game.players[(int)moveToSquare.get_pawn_in_square().get_color()].pawns[moveToSquare.get_pawn_in_square().get_id()].move_to(currentSquare);


                                //to keep update_pawn_the same i call it twice, we could just change the update pawn square function though
                                //first call sets square image brush to nill;
                                //pawn num doesn't matter here

                                //remove them from UI in my final spot
                                update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, 0);

                                //second call sets square image brush to pawn we want
                                //or first if no one was there in the first place
                                //but me in the final spot programatically
                                update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, currentPlayer.pawns[cur_pawn_selection].get_id() + 1);

                                //update_pawn_square(moveToSquare.get_index(), Color.BLUE);
                                //put  me in the final spot programatically
                                currentPlayer.pawns[cur_pawn_selection].move_to(moveToSquare);

                                if (currentSquare.get_color() != temp_color && currentSquare.get_Type() == SquareKind.SLIDE_START)
                                {
                                    //if the original square I was at, is a slide start for the pawn i swapped with
                                    int pawn_id = currentSquare.get_pawn_in_square().get_id();
                                    currentSquare = slide_baby_slide(currentSquare, game.get_player(temp_color), pawn_id);
                                    Player temp_player = game.get_player(temp_color);
                                    temp_player.pawns[pawn_id].move_to(currentSquare);
                                }

                                if (moveToSquare.get_color() != color_of_current_turn && moveToSquare.get_Type() == SquareKind.SLIDE_START)
                                {
                                    moveToSquare = slide_baby_slide(moveToSquare, currentPlayer, cur_pawn_selection);
                                    currentPlayer.pawns[cur_pawn_selection].move_to(moveToSquare);
                                }

                            }
                        }
                        else
                        {
                            //not an 11
                            currentSquare = currentPlayer.pawns[cur_pawn_selection].get_current_location();
                            //moveToSquare = options.ElementAt(pawnChoice).Item2.ElementAt(0);
                             moveToSquare = game.board.get_square_at(current_spot);
                            
                            //visually taking me off my current locations
                            if (!currentPlayer.pawns[cur_pawn_selection].is_start()) //home/safe zone
                            {
                                Debug.WriteLine("PAWN NOT AT START!");
                                if (currentSquare.get_Type() == SquareKind.SAFE)
                                {
                                    //update_pawn_square(currentSquare.get_index() - 66,color_of_current_turn, blue_safe_zone_list);
                                    //pawn num doesn't matter
                                    update_pawn_square(currentSquare.get_index() - color_adjustment, color_of_current_turn, safe_zone_lists[(int)color_of_current_turn], 0);
                                    from_safe_zone = true;
                                    current_canvas = safe_zone_lists[(int)color_of_current_turn][currentSquare.get_index() - color_adjustment];
                                }
                                else //regular square
                                {
                                    //pawn num doesn't matter
                                    update_pawn_square(currentSquare.get_index(), color_of_current_turn, pawn_square_list, 0);
                                    current_canvas = pawn_square_list[currentSquare.get_index()];
                                }

                            }
                            else //coming from start
                            {
                                //update_pawn_square(options.ElementAt(pawnChoice).Item1.get_id(), color_of_current_turn, blue_start_list);
                                //pawn num doesn't matter
                                update_pawn_square(currentPlayer.pawns[cur_pawn_selection].get_id(), color_of_current_turn, start_lists[(int)color_of_current_turn], 0);
                                current_canvas = start_lists[(int)color_of_current_turn][currentPlayer.pawns[cur_pawn_selection].get_id()];
                            }
                            Debug.WriteLine("PAWN 0's location: " + currentPlayer.pawns[cur_pawn_selection].get_current_location());
                            //if (options.ElementAt(0).Item2.Count != 0)
                            //{
                            //moved up into else
                            //    if (options.ElementAt(0).Item1.is_start())
                            //    {
                            //    }
                            //Debug.WriteLine("PAWN 0's move to location: " + options.ElementAt(pawnChoice).Item2.ElementAt(0).get_index());

                            if (moveToSquare.get_Type() == SquareKind.SAFE)//|| moveToSquare.get_Type() == SquareKind.HOMESQ)
                            {
                                update_pawn_square(moveToSquare.get_index() - color_adjustment, color_of_current_turn, safe_zone_lists[(int)color_of_current_turn], currentPlayer.pawns[cur_pawn_selection].get_id() + 1);
                                currentPlayer.pawns[cur_pawn_selection].set_in_safe_zone(true);

                            }
                            else if (moveToSquare.get_Type() == SquareKind.HOMESQ)
                            {
                                //update_pawn_square(moveToSquare.get_index() + options.ElementAt(pawnChoice).Item1.get_id() - 66, color_of_current_turn, blue_safe_zone_list);
                                update_pawn_square(moveToSquare.get_index() + currentPlayer.pawns[cur_pawn_selection].get_id() - color_adjustment, color_of_current_turn,
                                    safe_zone_lists[(int)color_of_current_turn], currentPlayer.pawns[cur_pawn_selection].get_id() + 1);

                                num_pawns_home[(int)color_of_current_turn] = num_pawns_home[(int)color_of_current_turn] + 1;
                            }
                            // This currently has a few issues. It is executing the move visually but not programatically. 
                            // It still has issues with ending in the start square.
                            // I also had an issue with index range.
                            else if (moveToSquare.get_Type() == SquareKind.SLIDE_START && moveToSquare.get_color() != color_of_current_turn)
                            {

                                /*update_pawn_square(moveToSquare.get_index(), color_of_current_turn,
                                                        pawn_square_list, cur_pawn_selection + 1);
                                currentPlayer.pawns[cur_pawn_selection].move_to(moveToSquare);*/

                                moveToSquare = slide_baby_slide(moveToSquare, currentPlayer, cur_pawn_selection);
                            }
                            else
                            {
                                //send someone home here if they are in the square you want!
                                if (moveToSquare.get_has_pawn())
                                {
                                    //send the visual pawn to start square
                                    update_pawn_square(moveToSquare.get_pawn_in_square().get_id(), moveToSquare.get_pawn_in_square().get_color(), start_lists[(int)moveToSquare.get_pawn_in_square().get_color()], moveToSquare.get_pawn_in_square().get_id() + 1);
                                    //send the data pawn to start state
                                    moveToSquare.get_pawn_in_square().sorry();

                                    //to keep update_pawn_the same i call it twice, we could just change the update pawn square function though
                                    //first call sets square image brush to nill;
                                    //pawn num doesn't matter here
                                    update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, 0);
                                }
                                //second call sets square image brush to pawn we want
                                //or first if no one was there in the first place
                                //update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, currentPlayer.pawns[cur_pawn_selection].get_id() + 1);
                                
                                //attempting animations!
                                update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list,
                                                            currentPlayer.pawns[cur_pawn_selection].get_id() + 1, true,
                                                            (int)Canvas.GetLeft(current_canvas),
                                                            (int)Canvas.GetTop(current_canvas));
                                
                                if(from_safe_zone)
                                {
                                    currentPlayer.pawns[cur_pawn_selection].set_in_safe_zone(false);
                                }
                            }
                            //update_pawn_square(moveToSquare.get_index(), Color.BLUE);
                            currentPlayer.pawns[cur_pawn_selection].move_to(moveToSquare);
                        }

                    }

                }
                else if (card.get_value() == 13)
                {
                    int pawnIndex = 0;
                    color_adjustment = 60 + 6 * ((int)color_of_current_turn);


                    if (options.Count != 0)
                    {
                        int pawnChoice = cur_pawn_selection;

                        if (pawnChoice != -1)
                        {
                            //game.players[(int)color_of_current_turn].pawns[cur_pawn_selection]
                            currentSquare = currentPlayer.pawns[cur_pawn_selection].get_current_location();
                            
                            //moveToSquare = options.ElementAt(pawnChoice).Item2.ElementAt(0);
                            moveToSquare = game.board.get_square_at(current_spot);


                            //pawn num doesn't matter here
                            if (currentPlayer.pawns[cur_pawn_selection].is_start())
                            {
                                update_pawn_square(currentPlayer.pawns[cur_pawn_selection].get_id(), color_of_current_turn, start_lists[(int)color_of_current_turn], 0);
                            }
                            else if (currentPlayer.pawns[cur_pawn_selection].is_in_safe_zone())
                            {
                                update_pawn_square(currentSquare.get_index() - color_adjustment, color_of_current_turn, safe_zone_lists[(int)color_of_current_turn], 0);
                                currentPlayer.pawns[cur_pawn_selection].set_in_safe_zone(false);

                            }
                            else
                            {
                                update_pawn_square(currentSquare.get_index(), color_of_current_turn, pawn_square_list, 0);
                            }

                            //send the visual pawn to start square
                            update_pawn_square(moveToSquare.get_pawn_in_square().get_id(), moveToSquare.get_pawn_in_square().get_color(), start_lists[(int)moveToSquare.get_pawn_in_square().get_color()], moveToSquare.get_pawn_in_square().get_id() + 1);
                            //send the data pawn to start state
                            moveToSquare.get_pawn_in_square().sorry();

                            //to keep update_pawn_the same i call it twice, we could just change the update pawn square function though
                            //first call sets square image brush to nill;
                            //pawn num doesn't matter here
                            update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, 0);

                            //second call sets square image brush to pawn we want
                            //or first if no one was there in the first place
                            update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, currentPlayer.pawns[cur_pawn_selection].get_id() + 1);

                            //update_pawn_square(moveToSquare.get_index(), Color.BLUE);
                            currentPlayer.pawns[cur_pawn_selection].move_to(moveToSquare);

                            if (moveToSquare.get_color() != color_of_current_turn && moveToSquare.get_Type() == SquareKind.SLIDE_START)
                            {
                                moveToSquare = slide_baby_slide(moveToSquare, currentPlayer, cur_pawn_selection);
                                currentPlayer.pawns[cur_pawn_selection].move_to(moveToSquare);
                            }

                            //sorry someone, get pawn options to sorry someone here 
                        }



                    }

                }

            }
            //change_turn();

            /*********************** NICK'S SECITION ***********************************/

        }
        private Square slide_baby_slide(Square moveToSquare, Player currentPlayer, int PawnNum)
        {
            Square endSlide = game.board.get_square_at(moveToSquare.get_index() + 3);
            Square current_square = moveToSquare;
            int sqIndex = moveToSquare.get_index();

            if (endSlide.get_Type() != SquareKind.SLIDE_END && endSlide.get_Type() != SquareKind.STARTC)
            {
                endSlide = game.board.get_square_at(endSlide.get_index() + 1);
            }

            if (moveToSquare.get_has_pawn() && moveToSquare.get_pawn_in_square() != currentPlayer.pawns[PawnNum])
            {
                //if someone else at the beginning of the slide, take them off that spot in UI land
                update_pawn_square(moveToSquare.get_index(), moveToSquare.get_pawn_in_square().get_color(), pawn_square_list, moveToSquare.get_pawn_in_square().get_id());
                
                //send them home in UI land
                update_pawn_square(moveToSquare.get_pawn_in_square().get_id(), moveToSquare.get_pawn_in_square().get_color(),
                            start_lists[(int)moveToSquare.get_pawn_in_square().get_color()], moveToSquare.get_pawn_in_square().get_id() + 1);
                
                //send them home in program land
                moveToSquare.get_pawn_in_square().sorry();
                moveToSquare.set_has_pawn(false);

            }
            if(!moveToSquare.get_has_pawn())
            {
                //if Im not already there, put me there
                //put me at the beginning of the slide, in UI and programatically

                //PawnNum and PlayerColor sent
                update_pawn_square(moveToSquare.get_index(), color_of_current_turn,
                            pawn_square_list, cur_pawn_selection + 1);

                currentPlayer.pawns[cur_pawn_selection].move_to(moveToSquare);
            }

            while (current_square.get_index() <= endSlide.get_index())
            {
                if (current_square.get_has_pawn())
                {
                    if (current_square.get_pawn_in_square() != currentPlayer.pawns[PawnNum]) 
                    {
                        update_pawn_square(current_square.get_pawn_in_square().get_id(), current_square.get_pawn_in_square().get_color(),
                            start_lists[(int)current_square.get_pawn_in_square().get_color()], current_square.get_pawn_in_square().get_id() + 1);
                        //ui update old square
                        update_pawn_square(current_square.get_index(), current_square.get_pawn_in_square().get_color(),
                                            pawn_square_list, current_square.get_pawn_in_square().get_id() + 1);

                        game.board.get_square_at(sqIndex).get_pawn_in_square().sorry();
                    }
                    else
                    {   // if you were at the end and get a backwards 4, you want to stay there ie dont call
                        //if you were at 1 away from beginning and get a -1 from a 10, you want to erase your image!
                        if (current_square.get_index() != endSlide.get_index())
                        {
                            update_pawn_square(current_square.get_index(), current_square.get_pawn_in_square().get_color(),
                                pawn_square_list, current_square.get_pawn_in_square().get_id() + 1);
                        }
                    }
                }
                sqIndex++;
                current_square = game.board.get_square_at(sqIndex);
            }
            
            update_pawn_square(endSlide.get_index(), currentPlayer.get_pawn_color(), pawn_square_list, currentPlayer.pawns[PawnNum].get_id() + 1);
            return endSlide;
        }

        private void toggle_grey_pawn_preview(int index, int pawn_num)
        {
            ImageBrush ib = null;
            string uri_string = "ms-appx:///Assets/Grey Pawns/Grey Pawn - ";
            uri_string += pawn_num.ToString();
            uri_string += ".png";
            ib = new ImageBrush();
            Uri uri = new Uri(uri_string, UriKind.Absolute);
            ib.ImageSource = new BitmapImage(uri);

            if (index >= 60)
            {
                if (index == game.board.get_home_square(color_of_current_turn))
                {
                    index += pawn_num - 1;
                }
                preview_safe_zone_lists[(int)color_of_current_turn][index - color_adjustment].Background = ib;
                return;
            }
            else
            {
                preview_square_list[index].Background = ib;
            }

        }

        private void toggle_sorry_preview(int index)
        {
            ImageBrush ib = null;
            if (preview_square_list[index].Background == ib)
            {
                string uri_string = "ms-appx:///Assets/Overlay Images/pink70.png";
                ib = new ImageBrush();
                Uri uri = new Uri(uri_string, UriKind.Absolute);
                ib.ImageSource = new BitmapImage(uri); 
            }
            preview_square_list[index].Background = ib;
            preview_square_list[index].Opacity = 0.75;
        }

        private void toggle_swap_preview(int index)
        {
            ImageBrush ib = null;
            if (preview_square_list[index].Background == ib)
            {
                string uri_string = "ms-appx:///Assets/Overlay Images/swap_pawn.jpg";
                ib = new ImageBrush();
                Uri uri = new Uri(uri_string, UriKind.Absolute);
                ib.ImageSource = new BitmapImage(uri);
            }
            preview_square_list[index].Background = ib;
            preview_square_list[index].Opacity = 0.75;
        }

        private void clear_preivew_canvas(int squarenum = -1)
        {
            ImageBrush ib = null;
            if (squarenum == -1)
            {
                for (int x = 0; x < preview_square_list.Count; x++)
                {
                    preview_square_list[x].Background = ib;
                    preview_square_list[x].Opacity = 1;
                }
                //write code to clear safe zones too
            }
            else
            {
                if (squarenum >= 60)
                {
                    if (squarenum == game.board.get_home_square(color_of_current_turn))
                    {
                        for (int i = 0; i < 4; i++)
                        {   //since clear doesn't get the pawn num, if clear is called on the home square clears the whole
                            //home square
                            preview_safe_zone_lists[(int)color_of_current_turn][squarenum - color_adjustment + i].Background = ib;
                        }
                    }
                    else
                    {
                        preview_safe_zone_lists[(int)color_of_current_turn][squarenum - color_adjustment].Background = ib;
                    }
                    
                    return;
                }
                else
                {
                    preview_square_list[squarenum].Background = ib;
                }
            }
        }

 
        private void change_turn(bool firstTurn = false)
        {
            /*
            if (color_of_current_turn == Color.BLUE)
            {
                color_of_current_turn = Color.YELLOW;
            }
            else if (color_of_current_turn == Color.YELLOW)
            {
                color_of_current_turn = Color.GREEN;
            }
            else if (color_of_current_turn == Color.GREEN)
            {
                color_of_current_turn = Color.RED;
            }
            else if (color_of_current_turn == Color.RED)
            {
                color_of_current_turn = Color.BLUE;
            }
            */

            int playerNumber = index_of_current_player + 1;

            Debug.WriteLine("Previous player's index: " + playerNumber);
            Debug.WriteLine("Previous player's color: " + color_of_current_turn);

            if(firstTurn || my_card.can_move_forward() != 2)
            {

            if (index_of_current_player == 3)
            {
                index_of_current_player = 0;
            }
            else
            {
                index_of_current_player = index_of_current_player + 1;
            }
            }


            color_of_current_turn = game.players[index_of_current_player].get_pawn_color();

            cover.Opacity = .60;
            card_drawn = false;
            player_turn.Text = game.players[index_of_current_player].get_player_name() + "'s Turn, Draw a Card!";

            int newPlayerNumber = index_of_current_player + 1;
            Debug.WriteLine("New player's index: " + newPlayerNumber);
            Debug.WriteLine("New player's color: " + color_of_current_turn);

        }

       

        private void animate(ImageBrush ib, double x1, double y1, double x2, double y2)
        {
            move_canvas.Background = ib;

            Storyboard sb = new Storyboard();
            Duration duration = new Duration(new TimeSpan(0, 0, 2));
            DoubleAnimation left_animation = new DoubleAnimation();
            DoubleAnimation top_animation = new DoubleAnimation();
            left_animation.Duration = duration;
            top_animation.Duration = duration;

            sb.Duration = duration;
            sb.Children.Add(left_animation);
            sb.Children.Add(top_animation);

            Storyboard.SetTarget(left_animation, move_canvas);
            Storyboard.SetTarget(top_animation, move_canvas);

            Storyboard.SetTargetProperty(left_animation, "(Canvas.Left)");
            Storyboard.SetTargetProperty(top_animation, "(Canvas.Top)");

            left_animation.From = x1;
            top_animation.From = y1;
            left_animation.To = x2;
            top_animation.To = y2;

            sb.Completed += finished_animation;

            sb.Begin();

            //Stopwatch stopwatch = Stopwatch.StartNew();
            //while (stopwatch.ElapsedMilliseconds < 2000) { }
            //stopwatch.Stop();
            //stopwatch.Reset();
            //move_canvas.Background = null;

        }

        private void finished_animation(object sender, object e)
        {
            move_canvas.Background = null;
        }

        public void update_pawn_square(int square_num, Color pawn_color, List<Canvas> list, int pawn_num, bool final = false, int orig_x = -1, int orig_y = -1) //could send exact pawn instead of color? just spitballin'
        {

            string uri_string = "ms-appx:///Assets/";
            //Pawn Images/";
            uri_string += pawn_color.ToString();
            uri_string += " Pawns/";
            uri_string += pawn_color.ToString();
            uri_string += " Pawn - ";
            uri_string += pawn_num.ToString();
            uri_string += ".png";

            ImageBrush ib = new ImageBrush();
            Uri uri = new Uri(uri_string, UriKind.Absolute);
            ib.ImageSource = new BitmapImage(uri);

            if (list[square_num].Background == null)
            {
                if (final)
                {
                    animate(ib, orig_x, orig_y, Canvas.GetLeft(list[square_num]), Canvas.GetTop(list[square_num]));
                    list[square_num].Background = ib;
                }
                else
                {
                    list[square_num].Background = ib;
                }
            }
            else
            {
                list[square_num].Background = null;
            }
            
        }

        private void initiate_toggle(int square_num, int pawn_num, bool swap)
        {
            if (square_num < 60 && game.board.get_square_at(square_num).get_has_pawn() && !swap)
            {
                toggle_sorry_preview(square_num);
            }
            else if (square_num < 60 && game.board.get_square_at(square_num).get_has_pawn() && swap)
            {
                toggle_swap_preview(square_num);
            }
            else
            {
                toggle_grey_pawn_preview(square_num, pawn_num);
            }

        }

        public void show_selected_move(int square_num, int pawn_num, Color pawn_color = Color.WHITE, bool swap = false, bool show_swap = false)
        {
            bool single_switch = true;
            if (square_num < 60) 
            {
                Square start_square, current_square, end_slide;
                start_square = current_square = game.board.get_square_at(square_num);

                if(start_square.get_Type() == SquareKind.SLIDE_START && start_square.get_color() != pawn_color){
                    single_switch = false;
                    end_slide = game.board.get_square_at(square_num + 3);
                    int sqIndex = square_num;

                    if (end_slide.get_Type() != SquareKind.SLIDE_END && end_slide.get_Type() != SquareKind.STARTC)
                    {
                        end_slide = game.board.get_square_at(end_slide.get_index() + 1);
                        //sqIndex++;
                    }
                    if (show_swap)
                    {
                        //sqIndex++;
                        current_square = game.board.get_square_at(++sqIndex);
                    }

                    while (sqIndex <= end_slide.get_index())
                    {
                        if(current_square == start_square){
                            initiate_toggle(sqIndex, pawn_num, swap);
                        }
                        else if (current_square.get_has_pawn() && current_square.get_pawn_in_square() != game.players[(int)pawn_color].pawns[pawn_num-1])
                        {
                            initiate_toggle(sqIndex, 0, false);
                        }
                        ///comment out this else to avoid filling slide with grey pawns
                        /*else
                        {
                            initiate_toggle(sqIndex, pawn_num, false);
                        }*/
                        
                        current_square = game.board.get_square_at(++sqIndex);
                    }
                }
            }
            
            if(single_switch && !show_swap)
            {
                initiate_toggle(square_num, pawn_num, swap);
            }

            cur_selected_square = square_num;
            
            /*string uri_string = "ms-appx:///Assets/Grey Pawns/Grey Pawn - ";
            uri_string += pawn_num.ToString();
            uri_string += ".png";
            //change to a gray pawn later

            ImageBrush ib = new ImageBrush();
            Uri uri = new Uri(uri_string, UriKind.Absolute);
            ib.ImageSource = new BitmapImage(uri);

            

            if(square_num >= 60)
            {
                cur_selected_img = safe_zone_lists[(int)color_of_current_turn][square_num - color_adjustment].Background;
                safe_zone_lists[(int)color_of_current_turn][square_num - color_adjustment].Background = ib;
            } else
            {
                cur_selected_img = pawn_square_list[square_num].Background;
                pawn_square_list[square_num].Background = ib;
            }

            //
            //cur_selected_img = list[square_num].Background;
            //list[square_num].Background = ib;
            //*/
        }

        public void hide_selected_move(int square_num)//, List<Canvas> list)
        {
            Square start_square;
            bool single_clear = true;
            if (square_num < 60 && square_num != -1)
            {
                start_square = game.board.get_square_at(square_num);
                if (start_square.get_Type() == SquareKind.SLIDE_START)
                {
                    clear_preivew_canvas();
                    single_clear = false;
                }
            }
            if (single_clear)
            {
                clear_preivew_canvas(square_num);
            }
            
            cur_selected_square = -1;
            /*if(square_num != -1)
            {
                cur_selected_square = -1;
                if(square_num >= 60)
                {
                    safe_zone_lists[(int)color_of_current_turn][square_num - color_adjustment].Background = cur_selected_img;
                } else
                {
                    pawn_square_list[square_num].Background = cur_selected_img;
                }
            }*/
        }

        private void init_start_zones()
        {
            ImageBrush ib = null;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Canvas cv1 = new Canvas();
                    cv1.Background = ib;
                    cv1.Height = 100;
                    cv1.Width = 100;
                    game_grid.Children.Add(cv1);
                    switch (i)
                    {
                        case (0):
                            blue_start_list.Add(cv1);
                            break;
                        case (1):
                            yellow_start_list.Add(cv1);
                            break;
                        case (2):
                            red_start_list.Add(cv1);
                            break;
                        case (3):
                            green_start_list.Add(cv1);
                            break;
                    }
                }
            }
            int x = 0;
            for (int i = 0; i < 2; i++)
            {
                x = i * 100;

                    Canvas.SetLeft(red_start_list[i], 400 + x);
                    Canvas.SetTop(red_start_list[i], 140);
                    Canvas.SetLeft(red_start_list[i + 2], 400 + x);
                    Canvas.SetTop(red_start_list[i + 2], 240);

                    Canvas.SetLeft(blue_start_list[i], 1360 + x);
                    Canvas.SetTop(blue_start_list[i], 370);
                    Canvas.SetLeft(blue_start_list[i + 2], 1360 + x);
                    Canvas.SetTop(blue_start_list[i + 2], 470);

                    Canvas.SetLeft(yellow_start_list[i], 1100 + x);
                    Canvas.SetTop(yellow_start_list[i], 1340);
                    Canvas.SetLeft(yellow_start_list[i + 2], 1100 + x);
                    Canvas.SetTop(yellow_start_list[i + 2], 1440);

                    Canvas.SetLeft(green_start_list[i], 140 + x);
                    Canvas.SetTop(green_start_list[i], 1075);
                    Canvas.SetLeft(green_start_list[i + 2], 140 + x);
                    Canvas.SetTop(green_start_list[i + 2], 1175);
            }
        }

        //public enum Color { RED, BLUE, YELLOW, GREEN, WHITE }
        private void init_safe_zones( List<List<Canvas> > list)
        {
            ImageBrush ib = null;
            /*string uri_string = "ms-appx:///Assets/Pawn Images/RED Pawn.png";
            ib = new ImageBrush();
            Uri uri = new Uri(uri_string, UriKind.Absolute);
            ib.ImageSource = new BitmapImage(uri); */
            int x = 0;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Canvas cv1 = new Canvas();
                    cv1.Background = ib;
                    cv1.Height = 100;
                    cv1.Width = 100;
                    game_grid.Children.Add(cv1);

                    list[i].Add(cv1);
                }
            }


            for (int i = 0; i < 5; i++)
            {
                x = i * 100;

                Canvas.SetLeft(list[(int)Color.BLUE][i], 1450 - x);
                Canvas.SetTop(list[(int)Color.BLUE][i], 245);


                Canvas.SetLeft(list[(int)Color.GREEN][i], 150 + x);
                Canvas.SetTop(list[(int)Color.GREEN][i], 1345);

                Canvas.SetLeft(list[(int)Color.RED][i], 250);
                Canvas.SetTop(list[(int)Color.RED][i], 145 + x);

                Canvas.SetLeft(list[(int)Color.YELLOW][i], 1350);
                Canvas.SetTop(list[(int)Color.YELLOW][i], 1450 - x);

                if (i < 2)
                {
                    Canvas.SetLeft(list[(int)Color.RED][i + 5], 200 + x);
                    Canvas.SetTop(list[(int)Color.RED][i + 5], 625);
                    Canvas.SetLeft(list[(int)Color.RED][i + 7], 200 + x);
                    Canvas.SetTop(list[(int)Color.RED][i + 7], 725);

                    Canvas.SetLeft(list[(int)Color.BLUE][i + 5], 860 + x);
                    Canvas.SetTop(list[(int)Color.BLUE][i + 5], 180);
                    Canvas.SetLeft(list[(int)Color.BLUE][i + 7], 865 + x);
                    Canvas.SetTop(list[(int)Color.BLUE][i + 7], 280);

                    Canvas.SetLeft(list[(int)Color.YELLOW][i + 5], 1300 + x);
                    Canvas.SetTop(list[(int)Color.YELLOW][i + 5], 940);
                    Canvas.SetLeft(list[(int)Color.YELLOW][i + 7], 1300 + x);
                    Canvas.SetTop(list[(int)Color.YELLOW][i + 7], 840);

                    Canvas.SetLeft(list[(int)Color.GREEN][i + 5], 635 + x);
                    Canvas.SetTop(list[(int)Color.GREEN][i + 5], 1280);
                    Canvas.SetLeft(list[(int)Color.GREEN][i + 7], 635 + x);
                    Canvas.SetTop(list[(int)Color.GREEN][i + 7], 1380);
                }

            }

        }

        private void init_pawn_square_list(List<Canvas> list)
        {
            ImageBrush ib = null;
            //uncomment this part below to see the magic!
            /*string uri_string = "ms-appx:///Assets/Pawn Images/RED Pawn.png";
            ib = new ImageBrush();
            Uri uri = new Uri(uri_string, UriKind.Absolute);
            ib.ImageSource = new BitmapImage(uri);  */
            for (int i = 0; i < 60; i++)
            {
                Canvas cv1 = new Canvas();
                cv1.Background = ib;
                cv1.Height = 100;
                cv1.Width = 100;
                game_grid.Children.Add(cv1);
                list.Add(cv1);
            }

            int x = 0;
            for (int i = 0; i < 15; i++)
            {

                Canvas.SetLeft(list[i], 50 + x);
                Canvas.SetTop(list[i], 45);

                Canvas.SetLeft(list[i + 15], 1550);
                Canvas.SetTop(list[i + 15], 45 + x);

                Canvas.SetLeft(list[i + 30], 1550 - x);
                Canvas.SetTop(list[i + 30], 1545);

                Canvas.SetLeft(list[i + 45], 50);
                Canvas.SetTop(list[i + 45], 1545 - x);

                x += 100;
            }

        }

        private void newGameMessage()
        {
            // Ask if user is sure
            MessageDialog message = new MessageDialog("Your current game will be lost. Are you sure you want to start a new game?");

            //message.Commands.Add(new Button());
            message.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(RespondToCommand)));
            message.Commands.Add(new UICommand("No", new UICommandInvokedHandler(RespondToCommand)));
            message.ShowAsync();
            return;
        }
 
        private void RespondToCommand(IUICommand command)
        {
            if (command.Label == "Yes")
            {
                // Remove current game
                MaKeyMeSorry.App.currentGame = null;
                game = null;
                Window.Current.Content.RemoveHandler(UIElement.KeyUpEvent, key_up_handler);
                this.Frame.Navigate(typeof(SetupPage));
            }
        }
        private void end_game_options_Click()
        {

            MessageDialog message = new MessageDialog("Congratulations!, Do you wish to start a new game?");

            message.Commands.Add(new UICommand("New Game", new UICommandInvokedHandler(EndGameCommand)));
            message.Commands.Add(new UICommand("Main Menu", new UICommandInvokedHandler(EndGameCommand)));
            message.ShowAsync();
            return;

        }

        private void EndGameCommand(IUICommand command)
        {

            //This has issues with null references when a new game is started.

            if (command.Label == "New Game")
            {
                MaKeyMeSorry.App.currentGame = null;
                game = null;
                Window.Current.Content.RemoveHandler(UIElement.KeyUpEvent, key_up_handler);
                this.Frame.Navigate(typeof(SetupPage));

            }
            else if (command.Label == "Main Menu")
            {

                MaKeyMeSorry.App.currentGame = null;
                game = null;
                Window.Current.Content.RemoveHandler(UIElement.KeyUpEvent, key_up_handler);
                this.Frame.Navigate(typeof(StartPage));

            }
        }
        private void highlight_forfeit_button()
        {
            pass_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
            pass_button.BorderThickness = new Thickness(10, 10, 10, 10);
            pass_button.Focus(FocusState.Keyboard);
            pass_highlighted = true;
        }

        private void deselect_all_buttons()
        {
            pass_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
            pass_button.BorderThickness = new Thickness(3, 3, 3, 3);
        }

        private int get_selected_UI()
        {
            int box_selected = -1;

            if (options_1.SelectedIndex != -1)
            {
                box_selected = 0;
                options_1.SelectedIndex = -1;
            }
            else if (options_2.SelectedIndex != -1)
            {
                box_selected = 1;
                options_2.SelectedIndex = -1;
            }
            else if (options_3.SelectedIndex != -1)
            {
                box_selected = 2;
                options_3.SelectedIndex = -1;
            }
            else if (options_4.SelectedIndex != -1)
            {
                box_selected = 3;
                options_4.SelectedIndex = -1;
            }
            else if (how_to_highlighted)
            {
                box_selected = 4; //button
            }
            else if (new_game_higlighted)
            {
                box_selected = 5;
            }
            else if (pass_highlighted)
            {
                box_selected = 6;
            }
            return box_selected;
        }

        private void pass_button_Click()
        {
            pass_highlighted = false;
            how_to_highlighted = false;
            new_game_higlighted = false;
            pass_button.IsEnabled = false;
            from_safe_zone = false;

            change_turn();
            pawn_1.Text = "";
            pawn_2.Text = "";
            pawn_3.Text = "";
            pawn_4.Text = "";
            options_1.Visibility = Visibility.Collapsed;
            options_1.Items.Clear();
            options_2.Visibility = Visibility.Collapsed;
            options_2.Items.Clear();
            options_3.Visibility = Visibility.Collapsed;
            options_3.Items.Clear();
            options_4.Visibility = Visibility.Collapsed;
            options_4.Items.Clear();
            no_options.Visibility = Visibility.Collapsed;
            this.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            deselect_all_buttons();
            pawns_available.Clear();
            pawns_available.Add(false);
            pawns_available.Add(false);
            pawns_available.Add(false);
            pawns_available.Add(false);

            //e.Handled = true;
        }

        private void AppBar_HowToPlay_ButtonClick(object sender, RoutedEventArgs e)
        {
            app_bar_open = false;
            this.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            how_to_play_menu_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            howToPlayMessage();
        }

        private void AppBar_NewGame_ButtonClick(object sender, RoutedEventArgs e)
        {
            app_bar_open = false;
            this.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            new_game_menu_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            newGameMessage();
        }

        private void AppBar_Quit_ButtonClick(object sender, RoutedEventArgs e)
        {
            app_bar_open = false;
            this.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            quit_menu_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
            quitMessage();
        }

        private void howToPlayMessage()
        {
            MessageDialog message = new MessageDialog("The object of the game: \nTo get all your pawns to the Home section of the board of the pawns' colors (e.g. If you have green pawns, and there is a green Home section, then that is your goal) before everyone else does. The route of the process goes clockwise. \n\nCards: \nThere are many kinds of cards in the deck. Make sure to read carefully when the options are displayed. \n\nTo start:\nThe cards 1 and 2 are the only cards that you can use to start a pawn. If any of your pawns are not on the board and you get a card other than a 1 or a 2, you must forfeit your turn.\n\nJumping and bumping: \nYou may jump over any pawn that is in your way. But...if you land on a space that is occupied by another person's pawn, bump it back to its own start space. \n\nMoving backward: \nThe cards 4 and 10 can move you backward. If you have successfully moved a pawn backward at least 2 spaces beyond your own start space, you may, on a subsequent turn, move into your own safety zone without moving all the way around the board.\n\nSlide: \nIf your pawns land on a slide space, slide to the end. You can bump any of the pawns that are in your pathway - including your own! - back to their start space. If you land on a slide that is your own color, don't slide.\n\nTo Win:\nThe first player to get all four of their pawns to their home space wins! If you win, and you play again with other people, the winner goes first.");
            message.ShowAsync();
        }


        private void quitMessage()
        {
            // Ask if user is sure
            MessageDialog message = new MessageDialog("Your current game will be lost. Are you sure you want to quit?");

            //message.Commands.Add(new Button());
            message.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(quitCommand)));
            message.Commands.Add(new UICommand("No", new UICommandInvokedHandler(quitCommand)));
            message.ShowAsync();
            return;
        }

        private void quitCommand(IUICommand command)
        {
            if (command.Label == "Yes")
            {
                // Remove current game
                MaKeyMeSorry.App.currentGame = null;
                game = null;
                Window.Current.Content.RemoveHandler(UIElement.KeyUpEvent, key_up_handler);
                Application.Current.Exit();
            }
        }

    }
}
