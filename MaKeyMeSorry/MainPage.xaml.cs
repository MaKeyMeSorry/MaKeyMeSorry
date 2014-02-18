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
        private Canvas background;

        public MainPage()
        {
            this.InitializeComponent();
            //Deck newDeck = new Deck(true);
            //Player player = new Player(Color.BLUE, true, true);

            Window.Current.Content.AddHandler(UIElement.KeyUpEvent, new KeyEventHandler(App_KeyUp), true);

            game = new Game(4);
            card_color = 0;
            pawn_square_list = new List<Canvas>();
            init_pawn_square_list();
            background = new Canvas();

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
                    update_pawn_square(card_color, Color.RED);
                    card_color++;
                    break;
                case (2):
                    flipped_extenstion = "Blue.png";
                    back_extenstion = "Yellow Front.png";
                    update_pawn_square(card_color, Color.BLUE);
                    card_color++;
                    break;
                case (3):
                    flipped_extenstion = "Yellow.png";
                    back_extenstion = "Green Front.png";
                    update_pawn_square(card_color, Color.YELLOW);
                    card_color++;
                    break;
                case (0):
                    flipped_extenstion = "Green.png";
                    back_extenstion = "Red Front.png";
                    update_pawn_square(card_color, Color.GREEN);
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

        }

        private void init_pawn_square_list()
        {
            ImageBrush ib = null;
            //uncomment this part below to see the magic!
            /*string uri_string = "ms-appx:///Assets/Pawn Images/RED Pawn.png";
            ib = new ImageBrush();
            Uri uri = new Uri(uri_string, UriKind.Absolute);
            ib.ImageSource = new BitmapImage(uri);*/         
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

                Canvas.SetLeft(pawn_square_list[i+15], 1550);
                Canvas.SetTop(pawn_square_list[i+15], 45 + x);

                Canvas.SetLeft(pawn_square_list[i + 30], 1550-x);
                Canvas.SetTop(pawn_square_list[i + 30], 1545);

                Canvas.SetLeft(pawn_square_list[i + 45], 50);
                Canvas.SetTop(pawn_square_list[i + 45], 1545-x);

                x += 100;
            } 
        }

        public void update_pawn_square(int square_num, Color pawn_color) //could send exact pawn instead of color? just spitballin'
        {

            string uri_string = "ms-appx:///Assets/Pawn Images/";
            uri_string += pawn_color.ToString();
            uri_string += " Pawn.png";

            ImageBrush ib = new ImageBrush();
            Uri uri = new Uri(uri_string, UriKind.Absolute);
            ib.ImageSource = new BitmapImage(uri);

            if (pawn_square_list[square_num].Background == null)
            {
                pawn_square_list[square_num].Background = ib;
            }
            else
            {
                pawn_square_list[square_num].Background = null;
            }
            

        }
    }
}
