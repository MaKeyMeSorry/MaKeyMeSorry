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
        private List<Canvas> pawn_square_list;

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
            //Deck newDeck = new Deck(true);
            //Player player = new Player(Color.BLUE, true, true);

            Window.Current.Content.AddHandler(UIElement.KeyUpEvent, new KeyEventHandler(App_KeyUp), true);

            game = new Game(4);
            card_color = 0;
            pawn_square_list = new List<Canvas>();

            blue_start_list = new List<Canvas>();
            yellow_start_list = new List<Canvas>();
            green_start_list = new List<Canvas>();
            red_start_list = new List<Canvas>();

            blue_safe_zone_list = new List<Canvas>();
            yellow_safe_zone_list = new List<Canvas>();
            green_safe_zone_list = new List<Canvas>();
            red_safe_zone_list = new List<Canvas>();

            init_pawn_square_list();
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

        }


        private void App_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            Debug.WriteLine("Keyboard button pressed");


            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Debug.WriteLine("Return button pressed");
                draw_card();

            }

        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }


        private void draw_card(object sender, TappedRoutedEventArgs e)
        {
            draw_card();
        }

        private void draw_card()
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

            //game.players.ElementAt(1).get_pawn_from_start()

            //update_pawn_square(game.players.ElementAt(1).get_pawn_from_start().get_id()+5, Color.BLUE, blue_safe_zone_list);
            //game.players.ElementAt(1).get_pawn_from_start().move_to(game.board.get_square_at(game.players.ElementAt(1).get_pawn_from_start().get_id()));
            /*************************** NICK'S SECITION *******************************/
            Square currentSquare;
            Square moveToSquare;

            Debug.WriteLine("card value: " + card.get_value());
            if (card.get_value() != 13)
            {
                List<Tuple<Pawn, List<Square>>> options = new List<Tuple<Pawn, List<Square>>>();
                options = game.get_move_options(Color.BLUE, card);
                int pawnIndex = 0;

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

                    //for(int i = 0; i < options.Count; i++)
                    //{
                    //    if(options.ElementAt(i).Item2.Count > 0)
                    //    {
                    //        pawnChoice = i;
                    //        break;
                    //    }
                    //}

                    if (options.ElementAt(pawnChoice).Item2.Count != 0)
                    {

                        currentSquare = options.ElementAt(pawnChoice).Item1.get_current_location();
                        moveToSquare = options.ElementAt(pawnChoice).Item2.ElementAt(0);

                        if (!options.ElementAt(pawnChoice).Item1.is_start())
                        {
                            Debug.WriteLine("PAWN NOT AT START!");
                            if (currentSquare.get_Type() == SquareKind.SAFE)
                            {
                                update_pawn_square(currentSquare.get_index() - 66, Color.BLUE, blue_safe_zone_list);
                            }
                            else
                            {
                                update_pawn_square(currentSquare.get_index(), Color.BLUE, pawn_square_list);
                            }

                        }
                        else
                        {
                            update_pawn_square(options.ElementAt(pawnChoice).Item1.get_id(), Color.BLUE, blue_start_list);
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
                            update_pawn_square(moveToSquare.get_index() - 66, Color.BLUE, blue_safe_zone_list);
                            options.ElementAt(pawnChoice).Item1.set_in_safe_zone(true);

                        }
                        else if (moveToSquare.get_Type() == SquareKind.HOMESQ)
                        {
                            update_pawn_square(moveToSquare.get_index() + options.ElementAt(pawnChoice).Item1.get_id() - 66, Color.BLUE, blue_safe_zone_list);
                        }
                        else
                        {
                            update_pawn_square(moveToSquare.get_index(), Color.BLUE, pawn_square_list);
                        }
                        //update_pawn_square(moveToSquare.get_index(), Color.BLUE);
                        options.ElementAt(pawnChoice).Item1.move_to(options.ElementAt(pawnChoice).Item2.ElementAt(0));
                    }


                    //}
                    //else
                    //{
                    //    options.ElementAt(0).Item1.sorry();
                    //}

                    //options.ElementAt(0).Item2.ElementAt(0).place_pawn(options.ElementAt(0).Item1);
                }
            }


            /*********************** NICK'S SECITION ***********************************/

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
    }
}
