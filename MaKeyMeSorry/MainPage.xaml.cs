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

        public MainPage()
        {
            this.InitializeComponent();
            //Deck newDeck = new Deck(true);
            //Player player = new Player(Color.BLUE, true, true);

            Window.Current.Content.AddHandler(UIElement.KeyUpEvent, new KeyEventHandler(App_KeyUp), true);

            game = new Game(4);
            card_color = 2;
        }


        private void App_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            Debug.WriteLine("Keyboard button pressed");


            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Debug.WriteLine("Return button pressed");
                draw_card();
                update_pawn_square(0, Color.RED);
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
            switch (card_color)
            {
                case (1):
                    flipped_extenstion = "Red.png";
                    back_extenstion = "Blue Front.png";
                    card_color++;
                    break;
                case (2):
                    flipped_extenstion = "Blue.png";
                    back_extenstion = "Yellow Front.png";
                    card_color++;
                    break;
                case (3):
                    flipped_extenstion = "Yellow.png";
                    back_extenstion = "Green Front.png";
                    card_color++;
                    break;
                case (4):
                    flipped_extenstion = "Green.png";
                    back_extenstion = "Red Front.png";
                    card_color = 1;
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
            uri_string += back_extenstion;
            Uri uri2 = new Uri(uri_string, UriKind.Absolute);
            ImageBrush ib2 = new ImageBrush();
            ib2.ImageSource = new BitmapImage(uri2);
            full_deck.Background = ib2;

        }

        void update_pawn_square(int square_num, Color pawn_color) //could send exact pawn instead of color? just spitballin'
        {

            string uri_string = "ms-appx:///Assets/Pawn Images/";
            uri_string += pawn_color.ToString();
            uri_string += " Pawn.png";

            ImageBrush ib = new ImageBrush();
            Uri uri = new Uri(uri_string, UriKind.Absolute);
            ib.ImageSource = new BitmapImage(uri);

            switch (square_num)
            {
                case(0):
                    pawn_square_0.Background = ib;
                    break;
                case(1):
                    pawn_square_1.Background = ib;
                    break;
                case (2):
                    pawn_square_2.Background = ib;
                    break;
                case (3):
                    pawn_square_3.Background = ib;
                    break;
                case (4):
                    pawn_square_4.Background = ib;
                    break;
                case (5):
                    pawn_square_5.Background = ib;
                    break;
                case (6):
                    pawn_square_6.Background = ib;
                    break;
                /*case (7):
                    pawn_square_7.Background = ib;
                    break;
                case (8):
                    pawn_square_8.Background = ib;
                    break;
                case (9):
                    pawn_square_9.Background = ib;
                    break;
                case (10):
                    pawn_square_10.Background = ib;
                    break;
                case (11):
                    pawn_square_11.Background = ib;
                    break;
                case (12):
                    pawn_square_12.Background = ib;
                    break;
                case (13):
                    pawn_square_13.Background = ib;
                    break;
                case (14):
                    pawn_square_14.Background = ib;
                    break;
                case (15):
                    pawn_square_15.Background = ib;
                    break;
                case (16):
                    pawn_square_16.Background = ib;
                    break;
                case (17):
                    pawn_square_17.Background = ib;
                    break;
                case (18):
                    pawn_square_18.Background = ib;
                    break;
                case (19):
                    pawn_square_19.Background = ib;
                    break;
                case (20):
                    pawn_square_20.Background = ib;
                    break;
                case (21):
                    pawn_square_21.Background = ib;
                    break;
                case (22):
                    pawn_square_22.Background = ib;
                    break;
                case (23):
                    pawn_square_23.Background = ib;
                    break;
                case (24):
                    pawn_square_24.Background = ib;
                    break;
                case (25):
                    pawn_square_25.Background = ib;
                    break;
                case (26):
                    pawn_square_26.Background = ib;
                    break;
                case (27):
                    pawn_square_27.Background = ib;
                    break;
                case (28):
                    pawn_square_28.Background = ib;
                    break;
                case (29):
                    pawn_square_29.Background = ib;
                    break;
                case (30):
                    pawn_square_30.Background = ib;
                    break;
                case (31):
                    pawn_square_31.Background = ib;
                    break;
                case (32):
                    pawn_square_32.Background = ib;
                    break;
                case (33):
                    pawn_square_33.Background = ib;
                    break;
                case (34):
                    pawn_square_34.Background = ib;
                    break;
                case (35):
                    pawn_square_35.Background = ib;
                    break;
                case (36):
                    pawn_square_36.Background = ib;
                    break;
                case (37):
                    pawn_square_37.Background = ib;
                    break;
                case (38):
                    pawn_square_38.Background = ib;
                    break;
                case (39):
                    pawn_square_39.Background = ib;
                    break;
                case (40):
                    pawn_square_40.Background = ib;
                    break;
                case (41):
                    pawn_square_41.Background = ib;
                    break;
                case (42):
                    pawn_square_42.Background = ib;
                    break;
                case (43):
                    pawn_square_43.Background = ib;
                    break;
                case (44):
                    pawn_square_44.Background = ib;
                    break;
                case (45):
                    pawn_square_45.Background = ib;
                    break;
                case (46):
                    pawn_square_46.Background = ib;
                    break;
                case (47):
                    pawn_square_47.Background = ib;
                    break;
                case (48):
                    pawn_square_48.Background = ib;
                    break;
                case (49):
                    pawn_square_49.Background = ib;
                    break;
                case (50):
                    pawn_square_50.Background = ib;
                    break;
                case (51):
                    pawn_square_51.Background = ib;
                    break;
                case (52):
                    pawn_square_52.Background = ib;
                    break;
                case (53):
                    pawn_square_53.Background = ib;
                    break;
                case (54):
                    pawn_square_54.Background = ib;
                    break;
                case (55):
                    pawn_square_55.Background = ib;
                    break;
                case (56):
                    pawn_square_56.Background = ib;
                    break;
                case (57):
                    pawn_square_57.Background = ib;
                    break;
                case (58):
                    pawn_square_58.Background = ib;
                    break;
                case (59):
                    pawn_square_59.Background = ib;
                    break;*/
                default:
                    break;
            }


        }
    }
}
