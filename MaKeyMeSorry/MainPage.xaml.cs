﻿using MaKeyMeSorry.Common;
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
        private int card_color;
        private Brush cur_selected_img;
        private int cur_selected_square;
        private List<Canvas> cur_selected_list;
        private int cur_pawn_selection;
        private List<bool> pawns_available;
        private int color_adjustment;
        private bool how_to_highlighted;
        private bool new_game_higlighted;
        private bool pass_highlighted;
        private List<Tuple<Pawn, List<Square>>> myOptions;
        private int current_spot;
        private bool forefeit_disabled;
        private bool first;
        private bool from_safe_zone;

        //variables for keeping state of a turn
        private Color color_of_current_turn;
        private int index_of_current_player;

        private bool card_drawn;
        private Card my_card;

        private List<Canvas> pawn_square_list;
        private List<Canvas> preview_square_list;

        private List< List<Canvas>> start_lists;
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
            index_of_current_player = -1;
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
            myOptions = new List<Tuple<Pawn, List<Square>>>();
            current_spot = -1;
            forefeit_disabled = false;

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
            card_color = 0;

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
                                how_to_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
            how_to_button.BorderThickness = new Thickness(3, 3, 3, 3);
            new_game_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
            new_game_button.BorderThickness = new Thickness(3, 3, 3, 3);
            pass_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
            pass_button.BorderThickness = new Thickness(3, 3, 3, 3);
                    pass_button.IsEnabled = false;
                    //this.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                }
            
            }
            //

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
            change_turn(true);
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
            if(first)
            {
                /*
                for (int i = 0; i < 4; i++)
                {
                    int color_test = 60 + 6 * (i);
                    for (int j = 0; j < 4; j++)
                    {
                        //testing card 11
                        update_pawn_square(j, Color.RED, start_lists[i], 0);
                        update_pawn_square(j, (Color)i, safe_zone_lists[i], j + 1);
                        game.players[i].pawns[j].set_in_safe_zone(true);
                        game.players[i].pawns[j].move_to(game.board.get_square_at(color_test + j));
                    }
                }
                first = false;*/
            }
            if (!card_drawn && (FocusManager.GetFocusedElement() != new_game_button) && (FocusManager.GetFocusedElement() != how_to_button))
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
                myOptions = new List<Tuple<Pawn, List<Square>>>();
                myOptions = game.get_move_options(color_of_current_turn, my_card);
                display_options(myOptions);
                if(game.forfeit_enabled)
                {
                    pass_button.IsEnabled = true;
                }
                cover.Opacity = 0;
                player_turn.Text = game.players[index_of_current_player].get_player_name() + "'s Turn, Choose a Move!";
                deselect_all_buttons();
            }
            else if (card_drawn && (FocusManager.GetFocusedElement() != new_game_button) && (FocusManager.GetFocusedElement() != how_to_button))
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
                play_game();
            }

            if (e.Key == Windows.System.VirtualKey.Right)
            {
                change_selected_pawn_box_right();
                e.Handled = true;
            }

            if (e.Key == Windows.System.VirtualKey.Left)
            {
                change_selected_pawn_box_left();
                e.Handled = true;
        }

        }

        private void change_selected_pawn_box_left()
            {
            //if any buttons are highlighted...change them back to normal
            deselect_all_buttons();
            int box_selected = get_selected_UI();

            Debug.WriteLine("box selected: " + box_selected);

            if (pawns_available[3] && (box_selected == 4))
            {
                options_4.SelectedIndex = 0;
                options_4.Focus(FocusState.Keyboard);
                cur_pawn_selection = 3;
                how_to_highlighted = false;
                pass_highlighted = false;
            }
            else if (pawns_available[2] && (box_selected == 3 || box_selected == 4))
            {
                options_3.SelectedIndex = 0;
                options_3.Focus(FocusState.Keyboard);
                cur_pawn_selection = 2;
                how_to_highlighted = false;
                pass_highlighted = false;
            }
            else if (pawns_available[1] && (box_selected == 2 || box_selected == 3 || box_selected == 4))
            {
                options_2.SelectedIndex = 0;
                options_2.Focus(FocusState.Keyboard);
                cur_pawn_selection = 1;
                how_to_highlighted = false;
                pass_highlighted = false;
            }
            else if (pawns_available[0] && (box_selected == 1 || box_selected == 2 || box_selected == 3 || box_selected == 4))
            {
                options_1.SelectedIndex = 0;
                options_1.Focus(FocusState.Keyboard);
                cur_pawn_selection = 0;
                how_to_highlighted = false;
                pass_highlighted = false;
            }
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
        }

        private void change_selected_pawn_box_right()
        {
            //if any buttons are highlighted...change them back to normal
            deselect_all_buttons();
            int box_selected = get_selected_UI();
            

            Debug.WriteLine("box selected: " + box_selected);

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
            else if ((box_selected == 0 || box_selected == 1 || box_selected == 2 || box_selected == 3))
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

            if(comboBox.SelectedIndex != -1)
            {
                if(cur_selected_square >= 60)
                {
                    hide_selected_move(cur_selected_square);// - color_adjustment, safe_zone_lists[(int)color_of_current_turn]);
                }
                else
                {
                    hide_selected_move(cur_selected_square);//, pawn_square_list);
                }

                if (Convert.ToInt32(comboBox.SelectedValue) >= 60)
                {
                    Debug.WriteLine(Convert.ToInt32(comboBox.SelectedValue) - color_adjustment);
                    show_selected_move(Convert.ToInt32(comboBox.SelectedValue), pawnSelected);// - color_adjustment, safe_zone_lists[(int)color_of_current_turn]);
                }
                else
                {
                    show_selected_move(Convert.ToInt32(comboBox.SelectedValue), pawnSelected);//, pawn_square_list);
                }
                Debug.WriteLine("Current Sel val: " + Convert.ToInt32(comboBox.SelectedValue));
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
            string back_extenstion;
            int temp = card_color % 4;
            switch (temp)
            {
                case (1):
                    flipped_extenstion = "Red.png";
                    back_extenstion = "Blue Front.png";
                    //update_pawn_square(card_color, Color.RED);
                    card_color++;
                    break;
                case (2):
                    flipped_extenstion = "Blue.png";
                    back_extenstion = "Yellow Front.png";
                    //update_pawn_square(card_color, Color.BLUE);
                    card_color++;
                    break;
                case (3):
                    flipped_extenstion = "Yellow.png";
                    back_extenstion = "Green Front.png";
                    //update_pawn_square(card_color, Color.YELLOW);
                    card_color++;
                    break;
                case (0):
                    flipped_extenstion = "Green.png";
                    back_extenstion = "Red Front.png";
                    //update_pawn_square(card_color, Color.GREEN);
                    card_color++;
                    break;
                default:
                    flipped_extenstion = "";
                    back_extenstion = "";
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

            uri_string = "ms-appx:///Assets/Card Back/Card Back ";

            if (card_color == 43)
            {
                uri_string += "3 Left.png";
            }
            else if (card_color == 44)
            {
                uri_string += "2 Left.png";
            }
            else if (card_color == 45)
            {
                uri_string += "1 Left.png";
                card_color = 1;
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

        void display_options(List<Tuple<Pawn, List<Square>>> options)
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

            foreach(Tuple<Pawn, List<Square>> option in options)
            {
                switch(option.Item1.get_id())
                {
                    case 0:
                        options_1.Visibility = Visibility.Visible;
                        pawn_1.Text = "Pawn 1 Options:";
                        foreach (Square mySquare in option.Item2)
                        {
                            options_1.Items.Add(mySquare.get_index());
                        }
                        pawns_available[0] = true;
                        break;
                    case 1:
                        pawn_2.Text = "Pawn 2 Options:";
                        options_2.Visibility = Visibility.Visible;
                        foreach (Square mySquare in option.Item2)
                        {
                            options_2.Items.Add(mySquare.get_index());
                        }
                        pawns_available[1] = true;
                        break;
                    case 2:
                        pawn_3.Text = "Pawn 3 Options:";
                        options_3.Visibility = Visibility.Visible;
                        foreach (Square mySquare in option.Item2)
                        {
                            options_3.Items.Add(mySquare.get_index());
                        }
                        pawns_available[2] = true;
                        break;
                    case 3:
                        pawn_4.Text = "Pawn 4 Options:";
                        options_4.Visibility = Visibility.Visible;
                        foreach (Square mySquare in option.Item2)
                        {
                            options_4.Items.Add(mySquare.get_index());
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
            List<Tuple<Pawn, List<Square>>> options = new List<Tuple<Pawn, List<Square>>>();
            options = game.get_move_options(color_of_current_turn, card);

            //display_options(options);


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

            }
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

        private void execute_update(List<Tuple<Pawn, List<Square>>> options, Square currentSquare, Square moveToSquare, int levelMove) 
        {

            switch(levelMove)
        {

                case 0:
                    // Initiate a pawn to a location without any other pawns in it

                    update_pawn_square(game.players[(int)color_of_current_turn].pawns[cur_pawn_selection].get_id(), color_of_current_turn, start_lists[(int)color_of_current_turn], 0);

                    update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, game.players[(int)color_of_current_turn].pawns[cur_pawn_selection].get_id() + 1);

                    game.players[(int)color_of_current_turn].pawns[cur_pawn_selection].move_to(moveToSquare);


                    break;
                case 1:
                    // Initiate a pawn to a location with a pawn in it

                    update_pawn_square(game.players[(int)color_of_current_turn].pawns[cur_pawn_selection].get_id(), color_of_current_turn, start_lists[(int)color_of_current_turn], 0);

                            update_pawn_square(moveToSquare.get_pawn_in_square().get_id(), moveToSquare.get_pawn_in_square().get_color(), start_lists[(int)moveToSquare.get_pawn_in_square().get_color()], moveToSquare.get_pawn_in_square().get_id() + 1);

                            moveToSquare.get_pawn_in_square().sorry();

                            update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, 0);

                    update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, game.players[(int)color_of_current_turn].pawns[cur_pawn_selection].get_id() + 1);

                    game.players[(int)color_of_current_turn].pawns[cur_pawn_selection].move_to(moveToSquare);

                    break;
                case 2:
                    // Move a pawn to a safe location from a safe location

                    update_pawn_square(currentSquare.get_index() - color_adjustment, color_of_current_turn, safe_zone_lists[(int)color_of_current_turn], 0);

                    update_pawn_square(moveToSquare.get_index() - color_adjustment, color_of_current_turn, safe_zone_lists[(int)color_of_current_turn], game.players[(int)color_of_current_turn].pawns[cur_pawn_selection].get_id() + 1);
                    
                    game.players[(int)color_of_current_turn].pawns[cur_pawn_selection].set_in_safe_zone(true);
                    
                    game.players[(int)color_of_current_turn].pawns[cur_pawn_selection].move_to(moveToSquare);

                    break;
                case 3:
                    // Move a pawn to Safe Location from not safe

                    update_pawn_square(currentSquare.get_index(), color_of_current_turn, pawn_square_list, 0);

                    update_pawn_square(moveToSquare.get_index() - color_adjustment, color_of_current_turn, safe_zone_lists[(int)color_of_current_turn], game.players[(int)color_of_current_turn].pawns[cur_pawn_selection].get_id() + 1);

                    game.players[(int)color_of_current_turn].pawns[cur_pawn_selection].set_in_safe_zone(true);

                    game.players[(int)color_of_current_turn].pawns[cur_pawn_selection].move_to(moveToSquare);

                    break;
                case 4:
                    // Move a pawn to home location from safe 

                    update_pawn_square(currentSquare.get_index() - color_adjustment, color_of_current_turn, safe_zone_lists[(int)color_of_current_turn], 0);

                    update_pawn_square(moveToSquare.get_index() + game.players[(int)color_of_current_turn].pawns[cur_pawn_selection].get_id() - color_adjustment, color_of_current_turn, safe_zone_lists[(int)color_of_current_turn], game.players[(int)color_of_current_turn].pawns[cur_pawn_selection].get_id() + 1);

                    num_pawns_home[index_of_current_player] = num_pawns_home[index_of_current_player] + 1;

                    game.players[(int)color_of_current_turn].pawns[cur_pawn_selection].move_to(moveToSquare);

                    break;
                case 5:
                    // Move a pawn to home from not safe

                            update_pawn_square(currentSquare.get_index(), color_of_current_turn, pawn_square_list, 0);

                    update_pawn_square(moveToSquare.get_index() + game.players[(int)color_of_current_turn].pawns[cur_pawn_selection].get_id() - color_adjustment, color_of_current_turn,
safe_zone_lists[(int)color_of_current_turn], game.players[(int)color_of_current_turn].pawns[cur_pawn_selection].get_id() + 1);

                    num_pawns_home[index_of_current_player] = num_pawns_home[index_of_current_player] + 1;

                    game.players[(int)color_of_current_turn].pawns[cur_pawn_selection].move_to(moveToSquare);
                    
                    break;    
                case 6:
                    // Move a pawn to square with no other pawns

                            update_pawn_square(currentSquare.get_index(), color_of_current_turn, pawn_square_list, 0);

                    update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, game.players[(int)color_of_current_turn].pawns[cur_pawn_selection].get_id() + 1);

                    game.players[(int)color_of_current_turn].pawns[cur_pawn_selection].move_to(moveToSquare);

                    break;

                case 7:
                    // Move a pawn to a square with a pawn in it

                                update_pawn_square(currentSquare.get_index(), color_of_current_turn, pawn_square_list, 0);

                                update_pawn_square(moveToSquare.get_pawn_in_square().get_id(), moveToSquare.get_pawn_in_square().get_color(), start_lists[(int)moveToSquare.get_pawn_in_square().get_color()], moveToSquare.get_pawn_in_square().get_id() + 1);

                                moveToSquare.get_pawn_in_square().sorry();

                                update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, 0);

                    update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, game.players[(int)color_of_current_turn].pawns[cur_pawn_selection].get_id() + 1);

                    game.players[(int)color_of_current_turn].pawns[cur_pawn_selection].move_to(moveToSquare);

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
            List<Tuple<Pawn, List<Square>>> options = new List<Tuple<Pawn, List<Square>>>();
            options = game.get_move_options(color_of_current_turn, card);

            /************************* Zoltan's Section *******************************/
            /**************************************************************************/

            //Are we removing pawns from safe if they get a backwards move which takes them off of it?
            //What is the win scenario if cards end and no one reaches 4 pawns?

            //AI_on = true;

            if ((!game.get_player(color_of_current_turn).get_is_human()) && AI_on)
                    {

                var moveLevel = new Tuple<int, int>(0, 0);
                var bestMove = new Tuple<int, int>(0, 0);

                if (options.Count == 1 && (card.get_value() == 1 || card.get_value() == 2))
                {
                    // This will happen if there isnt any other pawn in the board (for the computer whose turn it is)
                

                    currentSquare = options.ElementAt(0).Item1.get_current_location();
                        moveToSquare = options.ElementAt(0).Item2.ElementAt(0);
                    int color_adjustment = 60 + 6 * ((int)color_of_current_turn);

                    if (moveToSquare.get_has_pawn())
                        {

                        execute_update(options, currentSquare, moveToSquare, 1);

                    }
                    else
                    {

                        execute_update(options, currentSquare, moveToSquare, 0);

                    }


                        }
                else if (options.Count == 1 || (options.Count != 0 && true))
                        {

                    //This will yield the adequate move if there is already a single pawn on the board for the computer who's current turn it is

                    currentSquare = options.ElementAt(0).Item1.get_current_location();

                    int color_adjustment = 60 + 6 * ((int)color_of_current_turn);

                    Debug.WriteLine("AT LEAST HERE");

                    if ((card.get_value() != 7 && card.get_value() < 10) || card.get_value() == 12)
                    {
                        //This executes a normal movement for the pawn ( cards: 3, 4, 5, 8, 12 )
                        Debug.WriteLine("gets here");

                        moveToSquare = options.ElementAt(0).Item2.ElementAt(0);

                        if (moveToSquare.get_Type() == SquareKind.SAFE)
                        {

                            if(currentSquare.get_Type() == SquareKind.SAFE)
                            {

                                execute_update(options, currentSquare, moveToSquare, 2);

                            }
                            else
                            {
                                
                                execute_update(options, currentSquare, moveToSquare, 3);

                        }

                    }
                        else if (moveToSquare.get_Type() == SquareKind.HOMESQ)
                    {
                            if(currentSquare.get_Type() == SquareKind.SAFE)
                            {

                                execute_update(options, currentSquare, moveToSquare, 4);
                                num_pawns_home[index_of_current_player] = num_pawns_home[index_of_current_player]+1;

                            }
                            else
                        {

                                execute_update(options, currentSquare, moveToSquare, 5);
                                num_pawns_home[index_of_current_player] = num_pawns_home[index_of_current_player]+1;

                            }

                        }
                        else if (moveToSquare.get_has_pawn())
                        {

                            execute_update(options, currentSquare, moveToSquare, 7);

                        }
                        else
                        {
                            Debug.WriteLine("FAIL HERE");
                            execute_update(options, currentSquare, moveToSquare, 6);

                        }

                    }
                    else if (card.get_value() == 10)
                            {

                        //Could add more constraints (based on cur position)

                        
                        moveToSquare = options.ElementAt(0).Item2.ElementAt(0);
                        moveToSquareTwo = null;

                        bool twoElements = false;
                        
                        if(options.ElementAt(0).Item2.Count() > 1)
                        {

                            moveToSquareTwo = options.ElementAt(0).Item2.ElementAt(1);
                            twoElements = true;

                        }

                        if (moveToSquare.get_has_pawn())
                        {

                            execute_update(options, currentSquare, moveToSquare, 7);

                            }
                        else if (twoElements && moveToSquareTwo.get_has_pawn())
                        {
                        
                            execute_update(options, currentSquare, moveToSquareTwo, 7);

                        }
                        else if (currentSquare.get_index() == game.board.get_start_square(color_of_current_turn) && twoElements)
                        {

                            execute_update(options, currentSquare, moveToSquareTwo, 6);

                        }
                        else
                        {

                            execute_update(options, currentSquare, moveToSquare, 6);

                        }

                    }
                    else if (card.get_value() == 11)
                    {
                        // Need to update with priorities for game finished

                        int bestChoice = 11;
                        Square bestMoveSquare;
                        bestMoveSquare = options.ElementAt(0).Item2.ElementAt(0);
                        int moveChoice = 0;
                        moveToSquare = bestMoveSquare;

                        //Will this execute correctly if only one choice? Verify.

                        for (int i = 1; i < options.ElementAt(0).Item2.Count; i++)
                        {

                            moveToSquare = options.ElementAt(0).Item2.ElementAt(i);

                            //This can be placed in function to be more efficient

                            int curPawn = 0;
                            int curHome = 0;
                            int enemyPawn = 0;
                            int enemyHome = 0;
                            int CalcVal = 0;

                            curPawn = currentSquare.get_index();
                            curHome = game.board.get_start_square(color_of_current_turn) - 1;
                            enemyPawn = moveToSquare.get_index();
                            enemyHome = game.board.get_start_square(moveToSquare.get_pawn_in_square().get_color()) - 1;

                            CalcVal = get_AI_sorry_choice(curPawn, curHome, enemyPawn, enemyHome);

                            if (CalcVal > bestChoice)
                            {

                                bestChoice = CalcVal;
                                bestMoveSquare = moveToSquare; // Possible Seg Fault?
                                moveChoice = i;

                            }

                        }
                        if (bestChoice == 11)
                        {

                            //If bumps normally should be prioritized

                            if (bestMoveSquare.get_has_pawn())
                            {

                                execute_update(options, currentSquare, moveToSquare, 7);

                                }
                            else
                            {

                                execute_update(options, currentSquare, moveToSquare, 6);

                            }

                        }
                        else
                        {

                            execute_update(options, currentSquare, moveToSquare, 7);

                        }

                    }

                }
                else if (false)
                {

                    for (int i = 0; i < options.Count; i++)
                    {

                        //This could be changed to move on upon moveLevel comparison
                        // curCount position change possibly?


                        currentSquare = options.ElementAt(i).Item1.get_current_location();

                        for (int j = 0; j < options.ElementAt(i).Item2.Count; i++)
                        {

                            moveToSquare = options.ElementAt(i).Item2.ElementAt(j);

                            if (card.get_value() == 3 || card.get_value() == 5 || card.get_value() == 8 || card.get_value() == 12 || card.get_value() == 10)
                            {
                                if (moveToSquare.get_Type() == SquareKind.HOMESQ)
                                {

                                    moveLevel = update_tuple(9, 0);
                                    bestMove = update_tuple(i, j);

                                    // should double break. or execute normally (inefficient)

                                }

                                else if (moveToSquare.get_Type() == SquareKind.SAFE && currentSquare.get_Type() != SquareKind.SAFE)
                                {

                                    int curCount = (card.get_value());

                                    if (moveLevel.Item1 == 8)
                                    {

                                        if (moveLevel.Item2 < curCount)
                                        {

                                            moveLevel = update_tuple(8, curCount);
                                            bestMove = update_tuple(i, j);
                                        }

                                    }
                                    else if (moveLevel.Item1 < 8)
                                    {

                                        moveLevel = update_tuple(8, curCount);
                                        bestMove = update_tuple(i, j);

                                    }
                                }
                                else if (moveToSquare.get_has_pawn() && (moveToSquare.get_pawn_in_square().get_color() == color_of_current_turn))
                                {
                                    int curPawn = 0;
                                    int curHome = 0;
                                    int enemyPawn = 0;
                                    int enemyHome = 0;
                                    int enemyStart = 0;
                                    int CalcVal = 0;

                                    curPawn = currentSquare.get_index();
                                    curHome = game.board.get_start_square(color_of_current_turn) - 1;
                                    enemyPawn = moveToSquare.get_index();
                                    enemyHome = game.board.get_start_square(moveToSquare.get_pawn_in_square().get_color()) - 1;
                                    enemyStart = game.board.get_start_square(moveToSquare.get_pawn_in_square().get_color());

                                    CalcVal = ((enemyHome - enemyStart) - (enemyHome - enemyPawn)) + ((curHome - enemyPawn) - (curHome - curPawn));

                                    int curCount = CalcVal;

                                    if (moveLevel.Item1 == 5)
                                    {

                                        if (moveLevel.Item2 < curCount)
                                        {

                                            moveLevel = update_tuple(5, curCount);
                                            bestMove = update_tuple(i, j);

                                        }

                                    }
                                    else if (moveLevel.Item1 < 5)
                                    {

                                        moveLevel = update_tuple(5, curCount);
                                        bestMove = update_tuple(i, j);

                                    }

                                }
                                else if (currentSquare.get_Type() == SquareKind.STARTC && (currentSquare.get_color() == color_of_current_turn))
                                {

                                    if (moveLevel.Item1 < 4)
                                    {

                                        moveLevel = update_tuple(4, 0);
                                        bestMove = update_tuple(i, j);

                                    }

                                }
                                else if (currentSquare.get_Type() == SquareKind.STARTC && (currentSquare.get_color() != color_of_current_turn))
                                {

                                    int curHome = game.board.get_start_square(color_of_current_turn);

                                    int curCount = curHome - currentSquare.get_index();

                                    if (moveLevel.Item1 == 3)
                                    {

                                        if (moveLevel.Item2 < curCount)
                                        {

                                            moveLevel = update_tuple(3, curCount);
                                            bestMove = update_tuple(i, j);

                                        }

                                    }
                                    else if (moveLevel.Item1 < 3)
                                    {

                                        moveLevel = update_tuple(3, curCount);
                                        bestMove = update_tuple(i, j);

                                    }

                                }
                                else
                                {

                                    int curCount = card.get_value();

                                    if (moveLevel.Item1 == 2)
                                    {

                                        if (moveLevel.Item2 < curCount)
                                        {

                                            moveLevel = update_tuple(2, curCount);
                                            bestMove = update_tuple(i, j);

                                        }

                                    }
                                    else if (moveLevel.Item1 < 2)
                                    {

                                        moveLevel = update_tuple(2, curCount);
                                        bestMove = update_tuple(i, j);

                                    }
                                }

                            }
                            else if (card.get_value() == 4)
                            {
                                int curCount = (game.board.get_start_square(color_of_current_turn) - currentSquare.get_index());

                                //Add the possible bumping by updating first statement criteria

                                if (curCount < 4)
                                {

                                    if (moveLevel.Item1 == 6)
                                    {

                                        if (moveLevel.Item2 < curCount)
                                        {

                                            moveLevel = update_tuple(6, curCount);
                                            bestMove = update_tuple(i, j);

                                        }


                                    }
                                    else if (moveLevel.Item1 < 6)
                                    {

                                        moveLevel = update_tuple(6, curCount);
                                        bestMove = update_tuple(i, j);

                                    }


                                }

                            }
                            else if (card.get_value() == 7)
                            {
                                //7 is more complex based on separation 

                            }
                            else if (card.get_value() == -1)//This is incorrect I but I need to take a better look at how 10's option is displayed
                            {

                                //Add bump

                                int curCount = (game.board.get_start_square(color_of_current_turn) - currentSquare.get_index());

                                if (curCount < 2)
                                {

                                    if (moveLevel.Item1 < 6)
                                    {

                                        moveLevel = update_tuple(6, curCount);
                                        bestMove = update_tuple(i, j);

                                    }


                                }

                            }
                            else if (card.get_value() == 11)
                            {

                                //ADD the levels associated with going forward.

                                if (moveToSquare.get_has_pawn() && (moveToSquare.get_pawn_in_square().get_color() == color_of_current_turn))
                                {

                                    //Verify correctness

                                    int curPawn = 0;
                                    int curHome = 0;
                                    int enemyPawn = 0;
                                    int enemyHome = 0;
                                    int CalcVal = 0;

                                    curPawn = currentSquare.get_index();
                                    curHome = game.board.get_start_square(color_of_current_turn) - 1;
                                    enemyPawn = moveToSquare.get_index();
                                    enemyHome = game.board.get_start_square(moveToSquare.get_pawn_in_square().get_color()) - 1;

                                    CalcVal = get_AI_sorry_choice(curPawn, curHome, enemyPawn, enemyHome);

                                    int curCount = CalcVal;

                                    if (moveLevel.Item1 == 5)
                                    {

                                        if (moveLevel.Item2 < curCount)
                                        {

                                            moveLevel = update_tuple(5, curCount);
                                            bestMove = update_tuple(i, j);

                                        }

                                    }
                                    else if (moveLevel.Item1 < 5)
                                    {

                                        moveLevel = update_tuple(5, curCount);
                                        bestMove = update_tuple(i, j);

                                    }

                                }

                            }

                        }


                        Square fromChoice;
                        Square toChoice;
                        fromChoice = options.ElementAt(bestMove.Item1).Item1.get_current_location();
                        toChoice = options.ElementAt(bestMove.Item1).Item2.ElementAt(bestMove.Item2);
                    }
                    //private void execute_update(fromChoice, toChoice, bestMove.Item1);
                }
                
            
                
            }
            else //if (game.get_player(color_of_current_turn).get_is_human())
            {
                Player currentPlayer = game.get_player(color_of_current_turn);
                if (card.get_value() != 13)
                {

                    int pawnIndex = 0;
                    int color_adjustment = 60 + 6 * ((int)color_of_current_turn);

                    if (options.Count != 0)
                    {
                        int pawnChoice = cur_pawn_selection;

                        //if (card.can_move_forward() == 11)
                        if(false) //change back to above line once you switch the swapping issue
                        {

                            if (pawnChoice != -1)
                            {
                                //currentSquare = game.players[(int)color_of_current_turn].pawns[cur_pawn_selection].get_current_location();
                                currentSquare = game.get_player(color_of_current_turn).pawns[cur_pawn_selection].get_current_location();
                                moveToSquare = game.board.get_square_at(current_spot);

                                //pawn num doesn't matter here
                                update_pawn_square(currentSquare.get_index(), color_of_current_turn, pawn_square_list, 0);

                                //send the visual pawn to start square
                                //update_pawn_square(moveToSquare.get_pawn_in_square().get_id(), moveToSquare.get_pawn_in_square().get_color(), start_lists[(int)moveToSquare.get_pawn_in_square().get_color()], moveToSquare.get_pawn_in_square().get_id() + 1);
                                //send the data pawn to start state
                                if (currentPlayer.pawns[cur_pawn_selection].is_in_safe_zone())
                                {
                                    update_pawn_square(currentSquare.get_index(), moveToSquare.get_pawn_in_square().get_color(), safe_zone_lists[(int)color_of_current_turn], moveToSquare.get_pawn_in_square().get_id() + 1);
                                }
                                else
                                {
                                    update_pawn_square(currentSquare.get_index(), moveToSquare.get_pawn_in_square().get_color(), pawn_square_list, moveToSquare.get_pawn_in_square().get_id() + 1);
                                }

                                game.players[(int)moveToSquare.get_pawn_in_square().get_color()].pawns[moveToSquare.get_pawn_in_square().get_id()].move_to(currentSquare);


                                //to keep update_pawn_the same i call it twice, we could just change the update pawn square function though
                                //first call sets square image brush to nill;
                                //pawn num doesn't matter here
                                update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, 0);

                                //second call sets square image brush to pawn we want
                                //or first if no one was there in the first place
                                update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, currentPlayer.pawns[cur_pawn_selection].get_id() + 1);

                                //update_pawn_square(moveToSquare.get_index(), Color.BLUE);
                                currentPlayer.pawns[cur_pawn_selection].move_to(moveToSquare);

                                //sorry someone, get pawn options to sorry someone here 
                            }



                        }
                        else
                        {


                            currentSquare = currentPlayer.pawns[cur_pawn_selection].get_current_location();
                        //moveToSquare = options.ElementAt(pawnChoice).Item2.ElementAt(0);
                        moveToSquare = game.board.get_square_at(current_spot);

                        if (!currentPlayer.pawns[cur_pawn_selection].is_start())
                        {
                            Debug.WriteLine("PAWN NOT AT START!");
                            if (currentSquare.get_Type() == SquareKind.SAFE)
                            {
                                //update_pawn_square(currentSquare.get_index() - 66,color_of_current_turn, blue_safe_zone_list);
                                //pawn num doesn't matter
                                update_pawn_square(currentSquare.get_index() - color_adjustment, color_of_current_turn, safe_zone_lists[(int)color_of_current_turn], 0);
                                from_safe_zone = true;
                            }
                            else
                            {
                                //pawn num doesn't matter
                                update_pawn_square(currentSquare.get_index(), color_of_current_turn, pawn_square_list, 0);
                            }

                        }
                        else
                        {
                            //update_pawn_square(options.ElementAt(pawnChoice).Item1.get_id(), color_of_current_turn, blue_start_list);
                            //pawn num doesn't matter
                            update_pawn_square(currentPlayer.pawns[cur_pawn_selection].get_id(), color_of_current_turn, start_lists[(int)color_of_current_turn], 0);

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
                        else if (moveToSquare.get_Type() == SquareKind.SLIDE_START && false)
                        {

                            Square endSlide = game.board.get_square_at(moveToSquare.get_index());
                            int sqIndex = moveToSquare.get_index();
                            int counter = 1;
                            while (endSlide.get_Type() != SquareKind.SLIDE_END || endSlide.get_Type() != SquareKind.STARTC)
                            {
                                
                                sqIndex = sqIndex + counter;
                                endSlide = game.board.get_square_at(sqIndex);

                                if (endSlide.get_has_pawn())
                                {

                                    update_pawn_square(endSlide.get_pawn_in_square().get_id(), endSlide.get_pawn_in_square().get_color(), start_lists[(int)endSlide.get_pawn_in_square().get_color()], endSlide.get_pawn_in_square().get_id() + 1);

                                    endSlide.get_pawn_in_square().sorry();

                                    update_pawn_square(endSlide.get_index(), color_of_current_turn, pawn_square_list, 0);

                                }
                                counter++;
                                
                        }

                            update_pawn_square(endSlide.get_index(), color_of_current_turn, pawn_square_list, currentPlayer.pawns[cur_pawn_selection].get_id() + 1);

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
                            update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list, currentPlayer.pawns[cur_pawn_selection].get_id() + 1);
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

                            //sorry someone, get pawn options to sorry someone here 
                        }



                    }

                }

            }
            //change_turn();

            /*********************** NICK'S SECITION ***********************************/

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
                string uri_string = "ms-appx:///Assets/Overlay Images/Pink Outline.png";
                ib = new ImageBrush();
                Uri uri = new Uri(uri_string, UriKind.Absolute);
                ib.ImageSource = new BitmapImage(uri); 
            }
            preview_square_list[index].Background = ib;
        }

        private void clear_preivew_canvas(int squarenum = -1)
        {
            ImageBrush ib = null;
            if (squarenum == -1)
            {
                for (int x = 0; x < preview_square_list.Count; x++)
                {
                    preview_square_list[x].Background = ib;
                }
            }
            else
            {
                if (squarenum >= 60)
                {
                    preview_safe_zone_lists[(int)color_of_current_turn][squarenum - color_adjustment].Background = ib;
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

        public void update_pawn_square(int square_num, Color pawn_color, List<Canvas> list, int pawn_num) //could send exact pawn instead of color? just spitballin'
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
                list[square_num].Background = ib;
            }
            else
            {
                list[square_num].Background = null;
            }
            
        }

        public void show_selected_move(int square_num, int pawn_num)//, List<Canvas> list)
        {
            if (square_num < 60 && game.board.get_square_at(square_num).get_has_pawn())
            {
                toggle_sorry_preview(square_num);
            }
            else
            {
                toggle_grey_pawn_preview(square_num, pawn_num);
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
            
            clear_preivew_canvas(square_num);
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

        private void how_to_button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void new_game_button_Click(object sender, RoutedEventArgs e)
        {
            // Ask if user is sure
            MessageDialog message = new MessageDialog("Are you sure you want to start a new game?");

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
                this.Frame.Navigate(typeof(SetupPage));

            }
            else if (command.Label == "Main Menu")
            {

                MaKeyMeSorry.App.currentGame = null;
                game = null;
                this.Frame.Navigate(typeof(StartPage));

            }
        }

        private void highlight_how_to_button()
        {
            Debug.WriteLine("highlight how to called");
            how_to_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
            how_to_button.BorderThickness = new Thickness(10, 10, 10, 10);
            how_to_button.Focus(FocusState.Keyboard);
            how_to_highlighted = true;
        }

        private void highlight_new_game_button()
        {
            new_game_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
            new_game_button.BorderThickness = new Thickness(10, 10, 10, 10);
            new_game_button.Focus(FocusState.Keyboard);
            new_game_higlighted = true;
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
            how_to_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
            how_to_button.BorderThickness = new Thickness(3, 3, 3, 3);
            new_game_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
            new_game_button.BorderThickness = new Thickness(3, 3, 3, 3);
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
        
    }
}
