using MaKeyMeSorry;
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



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MaKeyMeSorry
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
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

        //variables for keeping state of a turn
        private Color color_of_current_turn;
        private bool card_drawn;
        private Card my_card;

        private List<Canvas> pawn_square_list;

        private List< List<Canvas>> start_lists;
        private List<List<Canvas>> safe_zone_lists;

        private List<Canvas> blue_start_list;
        private List<Canvas> yellow_start_list;
        private List<Canvas> green_start_list;
        private List<Canvas> red_start_list;

        private List<Canvas> blue_safe_zone_list;
        private List<Canvas> yellow_safe_zone_list;
        private List<Canvas> green_safe_zone_list;
        private List<Canvas> red_safe_zone_list;

        public MainPage()
        {
            this.InitializeComponent();
            color_of_current_turn = Color.RED;
            card_drawn = false;
            change_turn();
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


            Window.Current.Content.AddHandler(UIElement.KeyUpEvent, new KeyEventHandler(App_KeyUp), true);

            game = new Game(4);
            card_color = 0;
            pawn_square_list = new List<Canvas>();
           
            safe_zone_lists = new List<List<Canvas>>();
            start_lists = new List<List<Canvas>>();

            blue_start_list = new List<Canvas>();
            yellow_start_list = new List<Canvas>();
            green_start_list = new List<Canvas>();
            red_start_list = new List<Canvas>();

            blue_safe_zone_list = new List<Canvas>();
            yellow_safe_zone_list = new List<Canvas>();
            green_safe_zone_list = new List<Canvas>();
            red_safe_zone_list = new List<Canvas>();

            init_pawn_square_list();
            //public enum Color { RED, BLUE, YELLOW, GREEN, WHITE }
            //order here important, can use current turn color as index!
            safe_zone_lists.Add(red_safe_zone_list);
            safe_zone_lists.Add(blue_safe_zone_list);
            safe_zone_lists.Add(yellow_safe_zone_list);
            safe_zone_lists.Add(green_safe_zone_list);

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
                            update_pawn_square(j, (Color)i, red_start_list);
                            break;
                        case 1:
                            update_pawn_square(j, (Color)i, blue_start_list);
                            break;
                        case 2:
                            update_pawn_square(j, (Color)i, yellow_start_list);
                            break;
                        case 3:
                            update_pawn_square(j, (Color)i, green_start_list);
                            break;
                        default:
                            break;
                    }
                }
            }
            //CHEATING / WINNING
            Pawn testPawn = game.players[1].get_pawn_from_start();
            update_pawn_square(testPawn.get_id(), Color.BLUE, start_lists[1]);
            update_pawn_square(14, Color.BLUE, pawn_square_list);
            testPawn.move_to(game.board.get_square_at(14));

            how_to_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
            how_to_button.BorderThickness = new Thickness(3, 3, 3, 3);
            new_game_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
            new_game_button.BorderThickness = new Thickness(3, 3, 3, 3);
            pass_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
            pass_button.BorderThickness = new Thickness(3, 3, 3, 3);

        }
        

        void play_game()
        {
            if (!card_drawn)
            {
                color_adjustment = 60 + 6 * ((int)color_of_current_turn);
                Debug.WriteLine("Return button pressed");
                card_drawn = true;
                my_card = draw_card();
                pass_highlighted = true;
                how_to_highlighted = false;
                new_game_higlighted = false;
                pass_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
                pass_button.BorderThickness = new Thickness(10, 10, 10, 10);
                pass_button.Focus(FocusState.Keyboard);
                //apply_card(card);
                Debug.WriteLine("card value: " + my_card.get_value());
                List<Tuple<Pawn, List<Square>>> options = new List<Tuple<Pawn, List<Square>>>();
                options = game.get_move_options(color_of_current_turn, my_card);
                display_options(options);
                //
                cover.Opacity = 0;
            }
            else if (card_drawn)
            {
                
                hide_selected_move(cur_selected_square);//, pawn_square_list);
                apply_card(my_card);
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
            }
        }


        private void App_KeyUp(object sender, KeyRoutedEventArgs e)
        {

            Debug.WriteLine("Keyboard button pressed");


            if (e.Key == Windows.System.VirtualKey.Space)
            {
                Debug.WriteLine("Return button pressed");
                play_game();
            }

            if (e.Key == Windows.System.VirtualKey.Right && card_drawn)
            {
                change_selected_pawn_box();
            }

        }

        private void change_selected_pawn_box()
        {
            how_to_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
            how_to_button.BorderThickness = new Thickness(3, 3, 3, 3);
            new_game_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
            new_game_button.BorderThickness = new Thickness(3, 3, 3, 3);
            pass_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
            pass_button.BorderThickness = new Thickness(3, 3, 3, 3);

            int box_selected = -1;
            if(options_1.SelectedIndex != -1)
            {
                box_selected = 0;
                options_1.SelectedIndex = -1;

            } else if(options_2.SelectedIndex != -1)
            {
                box_selected = 1;
                options_2.SelectedIndex = -1;
            } else if(options_3.SelectedIndex != -1)
            {
                box_selected = 2;
                options_3.SelectedIndex = -1;
            } else if(options_4.SelectedIndex != -1)
            {
                box_selected = 3;
                options_4.SelectedIndex = -1;
            } else if(how_to_highlighted)
            {
                box_selected = 4; //button
            } else if(new_game_higlighted)
            {
                box_selected = 5;
            }
            else
            {
                box_selected = 6;
            }

            if(pawns_available[0] && box_selected == 6)
            {
                options_1.SelectedIndex = 0;
                options_1.Focus(FocusState.Keyboard);
                cur_pawn_selection = 0;
            }
            else if (pawns_available[1] && (box_selected == 0 || box_selected == 6))
            {
                options_2.SelectedIndex = 0;
                options_2.Focus(FocusState.Keyboard);
                cur_pawn_selection = 1;
            }
            else if (pawns_available[2] && (box_selected == 0 || box_selected == 1 || box_selected == 6))
            {
                options_3.SelectedIndex = 0;
                options_3.Focus(FocusState.Keyboard);
                cur_pawn_selection = 2;
            }
            else if (pawns_available[3] && (box_selected == 0 || box_selected == 1 || box_selected == 2 || box_selected == 6))
            {
                options_4.SelectedIndex = 0;
                options_4.Focus(FocusState.Keyboard);
                cur_pawn_selection = 3;
            }
            else if ((box_selected == 0 || box_selected == 1 || box_selected == 2 || box_selected == 3))
            {
                how_to_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
                how_to_button.BorderThickness = new Thickness(10, 10, 10, 10);
                how_to_button.Focus(FocusState.Keyboard);
                how_to_highlighted = true;
                hide_selected_move(cur_selected_square);
            } else if(box_selected == 4)
            {
                new_game_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
                new_game_button.BorderThickness = new Thickness(10, 10, 10, 10);
                new_game_button.Focus(FocusState.Keyboard);
                how_to_highlighted = false;
                new_game_higlighted = true;
            } else if(box_selected == 5)
            {
                pass_button.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
                pass_button.BorderThickness = new Thickness(10, 10, 10, 10);
                pass_button.Focus(FocusState.Keyboard);
                new_game_higlighted = false;
                pass_highlighted = true;
            }
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }


        private void ComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            color_adjustment = 60 + 6 * ((int)color_of_current_turn);
            ComboBox comboBox = (ComboBox)sender;
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
                    show_selected_move(Convert.ToInt32(comboBox.SelectedValue));// - color_adjustment, safe_zone_lists[(int)color_of_current_turn]);
                }
                else
                {
                    show_selected_move(Convert.ToInt32(comboBox.SelectedValue));//, pawn_square_list);
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

            uri_string = "ms-appx:///Assets/Card Images/Card Back ";

            if (card_color == 43)
            {
                uri_string += "3 Left ";
            }
            else if (card_color == 44)
            {
                uri_string += "2 Left ";
            }
            else if (card_color == 45)
            {
                uri_string += "1 Left ";
                card_color = 1;
            }

            uri_string += back_extenstion;
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
            pawn_1.Text = "Pawn 1 Options:";
            pawn_2.Text = "Pawn 2 Options:";
            pawn_3.Text = "Pawn 3 Options:";
            pawn_4.Text = "Pawn 4 Options:";
            pawns_available.Clear();
            pawns_available.Add(false);
            pawns_available.Add(false);
            pawns_available.Add(false);
            pawns_available.Add(false);


            foreach(Tuple<Pawn, List<Square>> option in options)
            {
                switch(option.Item1.get_id())
                {
                    case 0:
                        options_1.Visibility = Visibility.Visible;
                        foreach (Square mySquare in option.Item2)
                        {
                            options_1.Items.Add(mySquare.get_index());
                        }
                        pawns_available[0] = true;
                        break;
                    case 1:
                        options_2.Visibility = Visibility.Visible;
                        foreach (Square mySquare in option.Item2)
                        {
                            options_2.Items.Add(mySquare.get_index());
                        }
                        pawns_available[1] = true;
                        break;
                    case 2:
                        options_3.Visibility = Visibility.Visible;
                        foreach (Square mySquare in option.Item2)
                        {
                            options_3.Items.Add(mySquare.get_index());
                        }
                        pawns_available[2] = true;
                        break;
                    case 3:
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
                                update_pawn_square(currentSquare.get_index() - color_adjustment, color_of_current_turn, safe_zone_lists[(int)color_of_current_turn]);
                            }
                            else
                            {
                                update_pawn_square(currentSquare.get_index(), color_of_current_turn, pawn_square_list);
                            }

                        }
                        else
                        {
                            //update_pawn_square(options.ElementAt(pawnChoice).Item1.get_id(), color_of_current_turn, blue_start_list);
                            update_pawn_square(options.ElementAt(pawnChoice).Item1.get_id(), color_of_current_turn, start_lists[(int)color_of_current_turn]);

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
                            update_pawn_square(moveToSquare.get_index() - color_adjustment, color_of_current_turn, safe_zone_lists[(int)color_of_current_turn]);
                            options.ElementAt(pawnChoice).Item1.set_in_safe_zone(true);

                        }
                        else if (moveToSquare.get_Type() == SquareKind.HOMESQ)
                        {
                            //update_pawn_square(moveToSquare.get_index() + options.ElementAt(pawnChoice).Item1.get_id() - 66, color_of_current_turn, blue_safe_zone_list);
                            update_pawn_square(moveToSquare.get_index() + options.ElementAt(pawnChoice).Item1.get_id() - color_adjustment, color_of_current_turn,
                                safe_zone_lists[(int)color_of_current_turn]);
                        }
                        else
                        {
                            //send someone home here if they are in the square you want!
                            if (moveToSquare.get_has_pawn())
                            {
                                //send the visual pawn to start square
                                update_pawn_square(moveToSquare.get_pawn_in_square().get_id(), moveToSquare.get_pawn_in_square().get_color(), start_lists[(int)moveToSquare.get_pawn_in_square().get_color()]);
                                //send the data pawn to start state
                                moveToSquare.get_pawn_in_square().sorry();

                                //to keep update_pawn_the same i call it twice, we could just change the update pawn square function though
                                //first call sets square image brush to nill;
                                update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list);
                            }
                            //second call sets square image brush to pawn we want
                            //or first if no one was there in the first place
                            update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list);
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

                        update_pawn_square(currentSquare.get_index(), color_of_current_turn, pawn_square_list);

                        //send the visual pawn to start square
                        update_pawn_square(moveToSquare.get_pawn_in_square().get_id(), moveToSquare.get_pawn_in_square().get_color(), start_lists[(int)moveToSquare.get_pawn_in_square().get_color()]);
                        //send the data pawn to start state
                        moveToSquare.get_pawn_in_square().sorry();

                        //to keep update_pawn_the same i call it twice, we could just change the update pawn square function though
                        //first call sets square image brush to nill;
                        update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list);

                        //second call sets square image brush to pawn we want
                        //or first if no one was there in the first place
                        update_pawn_square(moveToSquare.get_index(), color_of_current_turn, pawn_square_list);

                        //update_pawn_square(moveToSquare.get_index(), Color.BLUE);
                        options.ElementAt(pawnChoice).Item1.move_to(options.ElementAt(pawnChoice).Item2.ElementAt(0));

                        //sorry someone, get pawn options to sorry someone here 
                    }


                    
                }

            }
            //change_turn();

            /*********************** NICK'S SECITION ***********************************/

        }

 
        private void change_turn()
        {
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
            cover.Opacity = .60;
            card_drawn = false;
            player_turn.Text = color_to_string(color_of_current_turn) + " Player's Turn";
        }

        public void update_pawn_square(int square_num, Color pawn_color, List<Canvas> list) //could send exact pawn instead of color? just spitballin'
        {

            string uri_string = "ms-appx:///Assets/Pawn Images/";
            uri_string += pawn_color.ToString();
            uri_string += " Pawn.png";

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

        public void show_selected_move(int square_num)//, List<Canvas> list)
        {
            string uri_string = "ms-appx:///Assets/Grey Pawns/Grey Pawn - 1.png";
            //change to a gray pawn later

            ImageBrush ib = new ImageBrush();
            Uri uri = new Uri(uri_string, UriKind.Absolute);
            ib.ImageSource = new BitmapImage(uri);

            cur_selected_square = square_num;
            
            //

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
            //
        }

        public void hide_selected_move(int square_num)//, List<Canvas> list)
        {
            if(square_num != -1)
            {
                cur_selected_square = -1;
                if(square_num >= 60)
                {
                    safe_zone_lists[(int)color_of_current_turn][square_num - color_adjustment].Background = cur_selected_img;
                } else
                {
                    pawn_square_list[square_num].Background = cur_selected_img;
                }
            }
        }

        private void init_pawn_square_list()
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
                pawn_square_list.Add(cv1);
            }

            int x = 0;
            for (int i = 0; i < 15; i++)
            {

                Canvas.SetLeft(pawn_square_list[i], 50 + x);
                Canvas.SetTop(pawn_square_list[i], 45);

                Canvas.SetLeft(pawn_square_list[i + 15], 1550);
                Canvas.SetTop(pawn_square_list[i + 15], 45 + x);

                Canvas.SetLeft(pawn_square_list[i + 30], 1550 - x);
                Canvas.SetTop(pawn_square_list[i + 30], 1545);

                Canvas.SetLeft(pawn_square_list[i + 45], 50);
                Canvas.SetTop(pawn_square_list[i + 45], 1545 - x);

                x += 100;
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Canvas cv1 = new Canvas();
                    cv1.Background = ib;
                    cv1.Height = 100;
                    cv1.Width = 100;
                    game_grid.Children.Add(cv1);
                    switch (i)
                    {
                        case (0):
                            blue_safe_zone_list.Add(cv1);
                            break;
                        case (1):
                            yellow_safe_zone_list.Add(cv1);
                            break;
                        case (2):
                            red_safe_zone_list.Add(cv1);
                            break;
                        case (3):
                            green_safe_zone_list.Add(cv1);
                            break;
                    }
                }
            }

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

            for (int i = 0; i < 5; i++)
            {
                x = i * 100;
                Canvas.SetLeft(blue_safe_zone_list[i], 1450 - x);
                Canvas.SetTop(blue_safe_zone_list[i], 245);


                Canvas.SetLeft(green_safe_zone_list[i], 150 + x);
                Canvas.SetTop(green_safe_zone_list[i], 1345);

                Canvas.SetLeft(red_safe_zone_list[i], 250);
                Canvas.SetTop(red_safe_zone_list[i], 145 + x);

                Canvas.SetLeft(yellow_safe_zone_list[i], 1350);
                Canvas.SetTop(yellow_safe_zone_list[i], 1450 - x);

                if (i < 2)
                {
                    Canvas.SetLeft(red_start_list[i], 400 + x);
                    Canvas.SetTop(red_start_list[i], 140);
                    Canvas.SetLeft(red_start_list[i + 2], 400 + x);
                    Canvas.SetTop(red_start_list[i + 2], 240);

                    Canvas.SetLeft(red_safe_zone_list[i + 5], 200 + x);
                    Canvas.SetTop(red_safe_zone_list[i + 5], 625);
                    Canvas.SetLeft(red_safe_zone_list[i + 7], 200 + x);
                    Canvas.SetTop(red_safe_zone_list[i + 7], 725);

                    Canvas.SetLeft(blue_start_list[i], 1360 + x);
                    Canvas.SetTop(blue_start_list[i], 370);
                    Canvas.SetLeft(blue_start_list[i + 2], 1360 + x);
                    Canvas.SetTop(blue_start_list[i + 2], 470);

                    Canvas.SetLeft(blue_safe_zone_list[i + 5], 860 + x);
                    Canvas.SetTop(blue_safe_zone_list[i + 5], 180);
                    Canvas.SetLeft(blue_safe_zone_list[i + 7], 865 + x);
                    Canvas.SetTop(blue_safe_zone_list[i + 7], 280);

                    Canvas.SetLeft(yellow_start_list[i], 1100 + x);
                    Canvas.SetTop(yellow_start_list[i], 1340);
                    Canvas.SetLeft(yellow_start_list[i + 2], 1100 + x);
                    Canvas.SetTop(yellow_start_list[i + 2], 1440);

                    Canvas.SetLeft(yellow_safe_zone_list[i + 5], 1300 + x);
                    Canvas.SetTop(yellow_safe_zone_list[i + 5], 940);
                    Canvas.SetLeft(yellow_safe_zone_list[i + 7], 1300 + x);
                    Canvas.SetTop(yellow_safe_zone_list[i + 7], 840);

                    Canvas.SetLeft(green_start_list[i], 140 + x);
                    Canvas.SetTop(green_start_list[i], 1075);
                    Canvas.SetLeft(green_start_list[i + 2], 140 + x);
                    Canvas.SetTop(green_start_list[i + 2], 1175);

                    Canvas.SetLeft(green_safe_zone_list[i + 5], 635 + x);
                    Canvas.SetTop(green_safe_zone_list[i + 5], 1280);
                    Canvas.SetLeft(green_safe_zone_list[i + 7], 635 + x);
                    Canvas.SetTop(green_safe_zone_list[i + 7], 1380);
                }

            }

        }

        private void how_to_button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
