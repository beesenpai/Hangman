using System;
using System.IO;
using Gtk;
using Pango;

namespace Hangman {
	public class MainWindow : Window {

		private VBox _container, _vb1, _vb2, _vb3, _vb4;
		private VButtonBox _vb5;
		private Table _buttons;
		private Label _greeting;
		private Label _word;
		private Label _tries;
		private Button[] _letters;
		private Button _startButton;

		/// <summary>
		/// The displayed word progress on the window.
		/// </summary>
		public string Word {
			set => _word.Text = value;
			get => _word.Text;
		}
		
		/// <summary>
		/// The current start button label.
		/// </summary>
		public string StartButton {
			set => _startButton.Label = value;
		}

		/// <summary>
		/// The current number of tries.
		/// </summary>
		public uint Tries {
			set => _tries.Text = "Tries: " + value;
		}

		public MainWindow() : base("Hangman") {
			SetDefaultSize(730, 400);
			SetPosition(WindowPosition.Center);
			
			Instantiate();
			Build();

			DeleteEvent += OnDelete;
			_startButton.Clicked += OnStart;
			ShowAll();
			ToggleTriesLabel(); // ShowAll makes it visible agian
		}

		/// <summary>
		/// Enable the on-screen keyboard.
		/// </summary>
		public void EnableButtons() {
			foreach(var b in _letters)
				b.Sensitive = true;
		}
		
		/// <summary>
		/// Disable the on-screen keyboard.
		/// </summary>
		public void DisableButtons() {
			foreach(var b in _letters)
				b.Sensitive = false;
		}

		/// <summary>
		/// Toggle the visibility of the tries display.
		/// </summary>
		public void ToggleTriesLabel() {
			_tries.Visible = !_tries.Visible;
		}

		// create the window elements
		private void Instantiate() {
			_container = new VBox();
			_vb1 = new VBox();
			_vb2 = new VBox();
			_vb3 = new VBox();
			_vb4 = new VBox();
			_vb5 = new VButtonBox();
			
			
			_greeting = new Label() {
				Text = "Welcome to Hangman",
				Selectable = false,
				Justify  = Justification.Center
			};
			_greeting.ModifyFont(FontDescription.FromString("Semibold 16"));
			
			_word = new Label() {
				Text = "X X X X X X X X X X X X X X X X",
				Selectable = false,
				Justify = Justification.Center,
			};
			_word.ModifyFont(FontDescription.FromString("Ultrabold 25"));

			_tries = new Label() {
				Text = "Tries: 0",
				Selectable = false,
				Justify = Justification.Left
			};
			
			_buttons = new Table(3, 10, true);

			_letters = new Button[26];
			var i = 0;
			foreach(var c in "QWERTYUIOPASDFGHJKLZXCVBNM") {
				_letters[i] = new Button() {
					Label = c.ToString(),
					Sensitive = false
				};
				_letters[i].Clicked += OnLetter;
				i++;
			}

			_startButton = new Button("Start Game");
		}
		
		// add the window elements to the window
		private void Build() {
			Add(_container);
			_container.Add(_vb1);
			_vb1.Add(_vb2);
			_vb2.Add(_greeting);
			_vb1.Add(_vb3);
			_vb3.Add(_word);
			_container.Add(_vb4);
			
			// lets fill the button table
			var n = 0; // array pointer
			for(uint i = 0; i < 2; i++)
				for(uint j = 0; j < 10; j++) {
					_buttons.Attach(_letters[n], j, j+1, i, i+1);
					n++;
				}
			for(uint j = 2; j < 8; j++) { // padding the last row by two cells
				_buttons.Attach(_letters[n], j, j+1, 2, 3);
				n++;
			}
			_vb4.Add(_buttons);
			
			_container.Add(_vb5);
			_vb5.Add(_startButton);
			
			_container.PackEnd(_tries);
		}

		// when the start/stop button is clicked
		private static void OnStart(object o, EventArgs args) {
			if(!Program.Started) {
				try {
					Program.StartGame();
				}
				catch(FileNotFoundException) {
					var dialog = new MessageDialog(Program.W, DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Ok,
						"Could not find the word list file.");
					dialog.Run();
					dialog.Destroy();
					Application.Quit();
				}
			}
			else { // A game is already in progress
				Program.StopGame();
			}
		}

		// Play the letter and disable the button
		private static void OnLetter(object o, EventArgs args) {
			var b = (Button) o;
			
			// if the game is won
			if(Program.PlayLetter(b.Label.ToCharArray()[0])) {
				Program.W.DisableButtons();
				var dialog = new MessageDialog(Program.W, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.Ok,
					"Congratulations! You found the word!");
				dialog.Run();
				dialog.Destroy();
			}
			else b.Sensitive = false;
		}
		
		private static void OnDelete(object o, DeleteEventArgs args) {
			Application.Quit();
		}
	}
}