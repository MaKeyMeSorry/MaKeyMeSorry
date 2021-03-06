﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI ;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MaKeyMeSorry
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartPage : Page
    {
        KeyEventHandler key_up_handler;
        int focus_index;

        public StartPage()
        {
            key_up_handler = new KeyEventHandler(App_KeyUp);
            Window.Current.Content.AddHandler(UIElement.KeyUpEvent, key_up_handler, true);
            Loaded += delegate { StartGameButton.Focus(FocusState.Keyboard); };
            this.InitializeComponent();
            StartGameButton.BorderBrush = new SolidColorBrush(Windows.UI.Colors.White);
            focus_index = 1;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window.Current.Content.RemoveHandler(UIElement.KeyUpEvent, key_up_handler);
            this.Frame.Navigate(typeof(SetupPage));
        }
        private void exit_App(object sender, RoutedEventArgs e)
        {
            
            Window.Current.Content.RemoveHandler(UIElement.KeyUpEvent, key_up_handler);
            Application.Current.Exit();

        }
        
        private void App_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            
            if (e.Key == Windows.System.VirtualKey.Space)
            {
                if (focus_index == 1)
                {
                    Debug.WriteLine("Space button pressed");
                    Window.Current.Content.RemoveHandler(UIElement.KeyUpEvent, key_up_handler);
                    this.Frame.Navigate(typeof(SetupPage));
                }
                else if (focus_index == 0)
                {
                    MessageDialog message1 = new MessageDialog("The object of the game: \nTo get all your pawns to the Home section of the board of the pawns' colors (e.g. If you have green pawns, and there is a green Home section, then that is your goal) before everyone else does. The route of the process goes clockwise. \n\nCards: \nThere are many kinds of cards in the deck. Make sure to read carefully when the options are displayed. \n\nTo start:\nThe cards 1 and 2 are the only cards that you can use to start a pawn. If any of your pawns are not on the board and you get a card other than a 1 or a 2, you must forfeit your turn.\n\nJumping and bumping: \nYou may jump over any pawn that is in your way. But...if you land on a space that is occupied by another person's pawn, bump it back to its own start space. \n\nMoving backward: \nThe cards 4 and 10 can move you backward. If you have successfully moved a pawn backward at least 2 spaces beyond your own start space, you may, on a subsequent turn, move into your own safety zone without moving all the way around the board.\n\nSlide: \nIf your pawns land on a slide space, slide to the end. You can bump any of the pawns that are in your pathway - including your own! - back to their start space. If you land on a slide that is your own color, don't slide.\n\nTo Win:\nThe first player to get all four of their pawns to their home space wins! If you win, and you play again with other people, the winner goes first.");
                    message1.ShowAsync();
                }
                else if (focus_index == 2)
                {
                    Window.Current.Content.RemoveHandler(UIElement.KeyUpEvent, key_up_handler);
                    Application.Current.Exit();
                }

            }
            else if (e.Key == Windows.System.VirtualKey.Left)
            {
                Debug.WriteLine("Right or Left button pressed");
                //howToPlayMessage();
                if (focus_index == 0)
                {
                    // On howToPlay, going to exit game
                    ExitButton.Focus(FocusState.Keyboard);
                    ExitButton.BorderBrush = new SolidColorBrush(Windows.UI.Colors.White);

                    focus_index = 2;

                    HowToPlayButton.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 237, 28, 127));
                    StartGameButton.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 237, 28, 127));
                    
                }
                else if (focus_index == 1)
                {
                    // On start new game, how to play
                    HowToPlayButton.Focus(FocusState.Keyboard);
                    HowToPlayButton.BorderBrush = new SolidColorBrush(Windows.UI.Colors.White);

                    focus_index = 0;
                    
                    StartGameButton.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 237, 28, 127));
                    ExitButton.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 237, 28, 127));
                }
                else if (focus_index == 2)
                {
                    // On exit game, going to Start Game
                    StartGameButton.Focus(FocusState.Keyboard);
                    StartGameButton.BorderBrush = new SolidColorBrush(Windows.UI.Colors.White);

                    focus_index = 1;

                    ExitButton.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 237, 28, 127));
                    HowToPlayButton.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 237, 28, 127));
                    
                }
            }
            else if (e.Key == Windows.System.VirtualKey.Right)
            {
                Debug.WriteLine("Right or Left button pressed");
                //howToPlayMessage();
                if (focus_index == 0)
                {
                    // on how to play, going to start
                    StartGameButton.Focus(FocusState.Keyboard);
                    StartGameButton.BorderBrush = new SolidColorBrush(Windows.UI.Colors.White);

                    focus_index = 1;

                    HowToPlayButton.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 237, 28, 127));
                    ExitButton.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 237, 28, 127));

                }
                else if (focus_index == 1)
                {
                    // On start going to exit
                    ExitButton.Focus(FocusState.Keyboard);
                    ExitButton.BorderBrush = new SolidColorBrush(Windows.UI.Colors.White);

                    focus_index = 2;

                    StartGameButton.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 237, 28, 127));
                    HowToPlayButton.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 237, 28, 127));
                }
                else if (focus_index == 2)
                {
                    // on exit going to how to
                    HowToPlayButton.Focus(FocusState.Keyboard);
                    HowToPlayButton.BorderBrush = new SolidColorBrush(Windows.UI.Colors.White);

                    focus_index = 0;

                    StartGameButton.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 237, 28, 127));
                    ExitButton.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 237, 28, 127));

                }
            }
            else
            {
                Debug.WriteLine("Unrecognized Key Pressed");
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //MessageDialog message = new MessageDialog("The object of the game: \nTo get all your pawns to the Home section of the board of the pawns' colors (e.g. If you have green pawns, and there is a green Home section, then that is your goal) before everyone else does. The route of the process goes clockwise. \n\nCards: \nThere are many kinds of cards in the deck. Make sure to read carefully when the options are displayed. \n\nTo start:\nThe cards 1 and 2 are the only cards that you can use to start a pawn. If any of your pawns are not on the board and you get a card other than a 1 or a 2, you must forfeit your turn.\n\nJumping and bumping: \nYou may jump over any pawn that is in your way. But...if you land on a space that is occupied by another person's pawn, bump it back to its own start space. \n\nMoving backward: \nThe cards 4 and 10 can move you backward. If you have successfully moved a pawn backward at least 2 spaces beyond your own start space, you may, on a subsequent turn, move into your own safety zone without moving all the way around the board.\n\nSlide: \nIf your pawns land on a slide space, slide to the end. You can bump any of the pawns that are in your pathway - including your own! - back to their start space. If you land on a slide that is your own color, don't slide.\n\nTo Win:\nThe first player to get all four of their pawns to their home space wins! If you win, and you play again with other people, the winner goes first.");
            //message.ShowAsync();
            howToPlayMessage();
        }

        private void howToPlayMessage()
        {
            //MessageDialog message = new MessageDialog("The object of the game: \nTo get all your pawns to the Home section of the board of the pawns' colors (e.g. If you have green pawns, and there is a green Home section, then that is your goal) before everyone else does. The route of the process goes clockwise. \n\nCards: \nThere are many kinds of cards in the deck. Make sure to read carefully when the options are displayed. \n\nTo start:\nThe cards 1 and 2 are the only cards that you can use to start a pawn. If any of your pawns are not on the board and you get a card other than a 1 or a 2, you must forfeit your turn.\n\nJumping and bumping: \nYou may jump over any pawn that is in your way. But...if you land on a space that is occupied by another person's pawn, bump it back to its own start space. \n\nMoving backward: \nThe cards 4 and 10 can move you backward. If you have successfully moved a pawn backward at least 2 spaces beyond your own start space, you may, on a subsequent turn, move into your own safety zone without moving all the way around the board.\n\nSlide: \nIf your pawns land on a slide space, slide to the end. You can bump any of the pawns that are in your pathway - including your own! - back to their start space. If you land on a slide that is your own color, don't slide.\n\nTo Win:\nThe first player to get all four of their pawns to their home space wins! If you win, and you play again with other people, the winner goes first.");
            //message.ShowAsync();
        }
    }
}
