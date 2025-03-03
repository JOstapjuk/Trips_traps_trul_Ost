namespace Trips_traps_trul_Ost
{
    public partial class MainPage : ContentPage
    {
        // UI elemendid
        private Grid gameBoard;
        private Label currentPlayerLabel;
        private Button randomPlayerButton;
        private Button newGameButton;
        private Button settingsButton;
        private Button rulesButton;
        private List<Button> gameButtons = new List<Button>();

        // Mängu olek
        private string currentPlayer = "X";
        private string[,] board;
        private int boardSize = 3;
        private bool gameEnded = false;
        private bool playingWithBot = false;
        private Random random = new Random();

        // Teema seaded
        private string xSymbol = "X";
        private string oSymbol = "O";
        private Color xColor = Colors.Blue;
        private Color oColor = Colors.Red;
        private Color defaultButtonColor = Colors.LightGray;

        public MainPage()
        {
            Title = "Trips traps trull";
            CreateUI();
            InitializeGame();
        }

        private void CreateUI()
        {
            // Põhiline paigutus
            var scrollView = new ScrollView();
            var mainLayout = new VerticalStackLayout
            {
                Spacing = 10,
                Padding = new Thickness(20)
            };
            scrollView.Content = mainLayout;
            Content = scrollView;

            // Ülemised juhtnupud
            var topButtonLayout = new HorizontalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 10,
                Margin = new Thickness(0, 0, 0, 10)
            };

            randomPlayerButton = new Button
            {
                Text = "Juhuslik esimene mängija"
            };
            randomPlayerButton.Clicked += RandomPlayerButton_Clicked;

            newGameButton = new Button
            {
                Text = "Uus mäng"
            };
            newGameButton.Clicked += NewGameButton_Clicked;

            topButtonLayout.Children.Add(randomPlayerButton);
            topButtonLayout.Children.Add(newGameButton);
            mainLayout.Children.Add(topButtonLayout);

            // Seadete ja reeglite nupud
            var secondButtonLayout = new HorizontalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 10,
                Margin = new Thickness(0, 0, 0, 20)
            };

            settingsButton = new Button
            {
                Text = "Seaded"
            };
            settingsButton.Clicked += SettingsButton_Clicked;

            rulesButton = new Button
            {
                Text = "Reeglid"
            };
            rulesButton.Clicked += RulesButton_Clicked;

            secondButtonLayout.Children.Add(settingsButton);
            secondButtonLayout.Children.Add(rulesButton);
            mainLayout.Children.Add(secondButtonLayout);

            // Praegune mängija silt
            currentPlayerLabel = new Label
            {
                Text = "Praegune mängija: X",
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 18,
                Margin = new Thickness(0, 0, 0, 10)
            };
            mainLayout.Children.Add(currentPlayerLabel);

            // Mängulaud
            gameBoard = new Grid
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                ColumnSpacing = 5,
                RowSpacing = 5
            };
            mainLayout.Children.Add(gameBoard);
        }

        private void InitializeGame()
        {
            // Initsialiseerin tahvli massiivi
            board = new string[boardSize, boardSize];

            // Mängu oleku lähtestamine
            gameEnded = false;
            gameButtons.Clear();

            // Tühjenda grid
            gameBoard.Clear();
            gameBoard.RowDefinitions.Clear();
            gameBoard.ColumnDefinitions.Clear();

            // Ruudustiku ridade ja veergude seadistamine
            for (int i = 0; i < boardSize; i++)
            {
                gameBoard.RowDefinitions.Add(new RowDefinition { Height = new GridLength(80) });
                gameBoard.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
            }

            // Loo nupud
            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    var button = new Button
                    {
                        FontSize = 36,
                        BackgroundColor = defaultButtonColor,
                        CornerRadius = 0,
                        CommandParameter = $"{row},{col}"
                    };

                    button.Clicked += GameButton_Clicked;

                    gameBoard.Add(button, col, row);
                    gameButtons.Add(button);
                    board[row, col] = string.Empty;
                }
            }

            // UI uuendamine
            UpdatePlayerLabel();
        }

        private void UpdatePlayerLabel()
        {
            currentPlayerLabel.Text = $"Praegune mängija: {currentPlayer}";
        }

        private void GameButton_Clicked(object sender, EventArgs e)
        {
            if (gameEnded)
                return;

            var button = (Button)sender;

            // Saada rida ja veerg nupu parameetrist
            var position = ((string)button.CommandParameter).Split(',');
            int row = int.Parse(position[0]);
            int col = int.Parse(position[1]);

            // Kontrolli, kas lahter on tühi
            if (string.IsNullOrEmpty(board[row, col]))
            {
                // Värskenda tahvlit ja kasutajaliidest
                board[row, col] = currentPlayer;
                button.Text = currentPlayer == "X" ? xSymbol : oSymbol;
                button.TextColor = currentPlayer == "X" ? xColor : oColor;

                // Kontrolli võitmist
                if (CheckForWin())
                {
                    gameEnded = true;
                    AnnounceWinner($"Mängija {currentPlayer} võitis!");
                    return;
                }

                // Kontrollida, kas see on tõmmatud
                if (CheckForDraw())
                {
                    gameEnded = true;
                    AnnounceWinner("Tie!");
                    return;
                }

                // Mängija vahetamine
                currentPlayer = currentPlayer == "X" ? "O" : "X";
                UpdatePlayerLabel();

                // Boti liikumine
                if (playingWithBot && currentPlayer == "O" && !gameEnded)
                {
                    MakeBotMove();
                }
            }
        }

        private async void MakeBotMove()
        {
            await Task.Delay(500);

            // Leia tühjad lahtrid
            var emptyCells = new List<(int row, int col)>();
            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    if (string.IsNullOrEmpty(board[row, col]))
                    {
                        emptyCells.Add((row, col));
                    }
                }
            }

            if (emptyCells.Count > 0)
            {
                // vali juhuslik tühi lahter
                var move = emptyCells[random.Next(emptyCells.Count)];

                // Leia nupp ja klõpsa sellele
                foreach (var button in gameButtons)
                {
                    if ((string)button.CommandParameter == $"{move.row},{move.col}")
                    {
                        GameButton_Clicked(button, EventArgs.Empty);
                        break;
                    }
                }
            }
        }

        private bool CheckForWin()
        {
            // Kontrollida ridu
            for (int row = 0; row < boardSize; row++)
            {
                bool rowWin = true;
                for (int col = 1; col < boardSize; col++)
                {
                    if (string.IsNullOrEmpty(board[row, 0]) ||
                        board[row, 0] != board[row, col])
                    {
                        rowWin = false;
                        break;
                    }
                }
                if (rowWin) return true;
            }

            // Veergude kontrollimine
            for (int col = 0; col < boardSize; col++)
            {
                bool colWin = true;
                for (int row = 1; row < boardSize; row++)
                {
                    if (string.IsNullOrEmpty(board[0, col]) ||
                        board[0, col] != board[row, col])
                    {
                        colWin = false;
                        break;
                    }
                }
                if (colWin) return true;
            }

            // Kontrollida diagonaale
            bool diagWin1 = true;
            bool diagWin2 = true;

            for (int i = 1; i < boardSize; i++)
            {
                if (string.IsNullOrEmpty(board[0, 0]) ||
                    board[0, 0] != board[i, i])
                {
                    diagWin1 = false;
                }

                if (string.IsNullOrEmpty(board[0, boardSize - 1]) ||
                    board[0, boardSize - 1] != board[i, boardSize - 1 - i])
                {
                    diagWin2 = false;
                }
            }

            return diagWin1 || diagWin2;
        }

        private bool CheckForDraw()
        {
            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    if (string.IsNullOrEmpty(board[row, col]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private async void AnnounceWinner(string message)
        {
            bool playAgain = await DisplayAlert("Mäng on läbi.", $"{message}\nKas soovite uuesti mängida?", "Jah", "Ei");

            if (playAgain)
            {
                NewGameButton_Clicked(null, EventArgs.Empty);
            }
        }

        private void NewGameButton_Clicked(object sender, EventArgs e)
        {
            currentPlayer = "X";
            InitializeGame();
        }

        private void RandomPlayerButton_Clicked(object sender, EventArgs e)
        {
            currentPlayer = random.Next(2) == 0 ? "X" : "O";
            UpdatePlayerLabel();

            if (playingWithBot && currentPlayer == "O")
            {
                MakeBotMove();
            }
        }

        private async void SettingsButton_Clicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Seaded",
                "Tühista", null,
                "Välja suuruse muutmine",
                "Robotiga mängimine: " + (playingWithBot ? "Jah" : "Ei"),
                "Muuda sümboleid",
                "Värvide muutmine",
                "Muuda teemat");

            switch (action)
            {
                case "Välja suuruse muutmine":
                    await ChangeBoardSize();
                    break;
                case "Robotiga mängimine: Jah":
                case "Robotiga mängimine: Ei":
                    playingWithBot = !playingWithBot;
                    if (playingWithBot && currentPlayer == "O")
                    {
                        MakeBotMove();
                    }
                    break;
                case "Muuda sümboleid":
                    await ChangeSymbols();
                    break;
                case "Värvide muutmine":
                    await ChangeColors();
                    break;
                case "Muuda teemat":
                    await ChangeTheme();
                    break;
            }
        }

        private async Task ChangeBoardSize()
        {
            string result = await DisplayPromptAsync(
                "Välja suurus",
                "Sisestage välja suurus (3-5):",
                initialValue: boardSize.ToString());

            if (int.TryParse(result, out int size) && size >= 3 && size <= 5)
            {
                boardSize = size;
                NewGameButton_Clicked(null, EventArgs.Empty);
            }
            else
            {
                await DisplayAlert("Viga", "Välja suurus peaks olema vahemikus 3 kuni 5", "OK");
            }
        }

        private async Task ChangeSymbols()
        {
            string xResult = await DisplayPromptAsync("Sümbolid", "X-i sümbol:", initialValue: xSymbol);
            if (!string.IsNullOrEmpty(xResult))
            {
                xSymbol = xResult;
            }

            string oResult = await DisplayPromptAsync("Sümbolid", "O-i sümbol:", initialValue: oSymbol);
            if (!string.IsNullOrEmpty(oResult))
            {
                oSymbol = oResult;
            }

            UpdateBoardSymbols();
        }

        private void UpdateBoardSymbols()
        {
            foreach (var button in gameButtons)
            {
                if (button.Text != null)
                {
                    if (button.Text == "X" || button.Text == xSymbol)
                    {
                        button.Text = xSymbol;
                    }
                    else if (button.Text == "O" || button.Text == oSymbol)
                    {
                        button.Text = oSymbol;
                    }
                }
            }
        }

        private async Task ChangeColors()
        {
            var colors = new List<Color>
            {
                Colors.Red, Colors.Blue, Colors.Green, Colors.Purple,
                Colors.Orange, Colors.Pink, Colors.Teal, Colors.Brown,
                Colors.Magenta, Colors.Cyan, Colors.Indigo, Colors.Lime
            };

            // Valige juhuslikud värvid
            Random random = new Random();
            xColor = colors[random.Next(colors.Count)];
            colors.Remove(xColor); // Eemaldage esimene värv, et tagada nende erinevus.
            oColor = colors[random.Next(colors.Count)];

            string GetColorName(Color color)
            {
                if (color == Colors.Red) return "Punane";
                if (color == Colors.Blue) return "Sinine";
                if (color == Colors.Green) return "Roheline";
                if (color == Colors.Purple) return "Lilla";
                if (color == Colors.Orange) return "Oranž";
                if (color == Colors.Pink) return "Roosa";
                if (color == Colors.Teal) return "Türkiissinine";
                if (color == Colors.Brown) return "Pruun";
                if (color == Colors.Magenta) return "Magenta";
                if (color == Colors.Cyan) return "Hele sinine";
                if (color == Colors.Indigo) return "Indigo";
                if (color == Colors.Lime) return "Lime";
                return "Teadmata";
            }

            await DisplayAlert("Värvid on muudetud",
                $"X: {GetColorName(xColor)}\nO: {GetColorName(oColor)}",
                "OK");

            UpdateBoardColors();
        }

        private void UpdateBoardColors()
        {
            foreach (var button in gameButtons)
            {
                if (button.Text == xSymbol)
                {
                    button.TextColor = xColor;
                }
                else if (button.Text == oSymbol)
                {
                    button.TextColor = oColor;
                }
            }
        }

        private async Task ChangeTheme()
        {
            string action = await DisplayActionSheet("Valige teema",
                "Tühista", null,
                "Helendav",
                "Tume",
                "Värviline");

            switch (action)
            {
                case "Helendav":
                    Application.Current.UserAppTheme = AppTheme.Light;
                    defaultButtonColor = Colors.LightGray;
                    break;
                case "Tume":
                    Application.Current.UserAppTheme = AppTheme.Dark;
                    defaultButtonColor = Colors.DarkGray;
                    break;
                case "Värviline":
                    defaultButtonColor = Colors.LightYellow;
                    break;
            }

            NewGameButton_Clicked(null, EventArgs.Empty);
        }

        private async void RulesButton_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Mängureeglid",
                "Tic-tac-toe on kahe mängija mäng NxN-suurusel väljal..\n\n" +
                "Mängureeglid:\n" +
                "• Mängijad mängivad kordamööda\n" +
                "• Mängija asetab oma sümboli (X või O) vabale ruudule.\n" +
                "• Kes esimesena moodustab oma sümbolitest horisontaalselt, vertikaalselt või diagonaalselt rea, võidab.\n" +
                "• Kui kõik ruudud on täidetud, kuid keegi ei võida, lõpeb mäng viik.",
                "Selge");
        }

    }
}
