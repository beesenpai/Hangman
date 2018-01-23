/****************************************
 * HANGMAN GAME
 * Michael MacLean
 * January 14th, 2018
 *
 * Wordlist compiled by first20hours@github
 * https://github.com/first20hours/google-10000-english
 ****************************************/

using System;
using System.IO;
using System.Text;
using Gtk;

namespace Hangman
{
    internal class Program {
        public static MainWindow W;

        private const string Words = "wordlist.txt";
        private static readonly Random R = new Random();

        private static string _word; // we initialize to null and put back to null when the game is over
        private static uint _tries; // the number of guesses, initialized to 0
        
        /// <summary>
        /// Is there a game in progress?
        /// </summary>
        public static bool Started => _word != null;

        
        /// <summary>Returns a random word from the word list file.</summary>
        private static string SelectWord() {
            var lines = File.ReadAllLines(Words);
            var n = lines.Length;
            return lines[R.Next(0, n + 1)];
        }

        /// <summary>
        /// Start the gane
        /// </summary>
        /// <exception cref="FileNotFoundException">Thrown when the word list cannot be found.</exception>
        public static void StartGame() {
            _word = SelectWord().ToUpper();
            W.Word = "";
            for(var i = 0; i < _word.Length; i++) W.Word += "_ ";
            
            W.EnableButtons();
            W.Tries = 0;
            W.ToggleTriesLabel();
            W.StartButton = "Stop Game";
        }

        /// <summary>
        /// Stop the game and return the window to the default state.
        /// </summary>
        public static void StopGame() {
            _word = null; // no more game, get rid of word
            W.Word = "X X X X X X X X X X X X X X X";
            W.DisableButtons();
            W.Tries = 0;
            _tries = 0;
            W.ToggleTriesLabel();
            W.StartButton = "Start Game";
        }

        /// <summary>
        /// Play a given letter
        /// </summary>
        /// <param name="a">A string of length one.</param>
        /// <return>True on win</return>
        public static bool PlayLetter(char a) {
            _tries++;
            W.Tries = _tries;

            var s = new StringBuilder(W.Word);
            for(var i = 0; i < _word.Length; i++)
                if(_word[i] == a)
                    s[2 * i] = a;
            W.Word = s.ToString();

            return !W.Word.Contains("_");
        }
        
        
        public static void Main(string[] args) {
            Application.Init();
            W = new MainWindow();
            W.Show();
            Application.Run();
        }
    }
}
