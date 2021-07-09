using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RouletteGuide
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int initialBet;
        int currentBet;
        int round = 0;
        List<int> blackBet = new List<int>() { 0, 3, 4 };
        List<int> redBet = new List<int>() { 1, 2, 5 };
        int currentBetColor = 0;
        double startingFunds;
        double currentFunds;
        double profit = 0;
        double profitPerHour = 0;
        Stopwatch timer = new Stopwatch();
        int winStreak = 0;
        int loseStreak = 0;
        int longestWinStreak = 0;
        int longestLoseStreak = 0;
        int rollsPerHour = 0;

        public MainWindow()
        {
            InitializeComponent();
            btnStop.IsEnabled = false;
            btnWin.IsEnabled = false;
            btnLose.IsEnabled = false;
            btnReset.IsEnabled = false;
        }

        public void btnWin_Click(object sender, EventArgs e)
        {
            winStreak++;
            loseStreak = 0;
            if(winStreak > longestWinStreak)
            {
                longestWinStreak = winStreak;
            }

            currentFunds += currentBet;
            profit = currentFunds - startingFunds;
            currentBet = initialBet;
            round++;
            IncrementBetColor();
            CalculateProfitPerHour();
            CalculateRollsPerHour();
            SetItems();
        }

        public void btnLose_Click(object sender, EventArgs e)
        {
            loseStreak++;

            if(loseStreak > longestLoseStreak)
            {
                longestLoseStreak = loseStreak;
            }

            winStreak = 0;
            currentFunds -= currentBet;
            profit = currentFunds - startingFunds;
            round++;
            DoubleBet();
            IncrementBetColor();
            CalculateProfitPerHour();
            CalculateRollsPerHour();
            SetItems();
        }

        public void btnReset_Click(object sender, EventArgs e)
        {
            round = 0;
            profit = 0;
            profitPerHour = 0;
            currentFunds = startingFunds;
            currentBetColor = 0;
            currentBet = initialBet;
            winStreak = 0;
            longestWinStreak = 0;
            loseStreak = 0;
            longestLoseStreak = 0;
            rollsPerHour = 0;
            timer.Restart();
            SetItems();
        }

        public void btnStart_Click(object sender, EventArgs e)
        {
            if (InputIsValid())
            {
                btnReset_Click(this, new EventArgs());
                timer.Start();
                tbFunds.IsEnabled = false;
                tbInitialBet.IsEnabled = false;
                btnWin.IsEnabled = true;
                btnLose.IsEnabled = true;
                btnStop.IsEnabled = true;
                btnReset.IsEnabled = true;
                btnStart.IsEnabled = false;
                SetItems();
            }
            else
            {
                MessageBox.Show("Make sure that the \"Initial Bet\" & \"Initial Funds\" text boxes contain an integer value in them before starting.", "Validation Failure", MessageBoxButton.OK);
            }
        }

        private bool InputIsValid()
        {
            return int.TryParse(tbFunds.Text, out _) && int.TryParse(tbInitialBet.Text, out _);
        }

        public void btnStop_Click(object sender, EventArgs e)
        {
            timer.Stop();
            tbFunds.IsEnabled = true;
            tbInitialBet.IsEnabled = true;
            btnWin.IsEnabled = false;
            btnLose.IsEnabled = false;
            btnStop.IsEnabled = false;
            btnReset.IsEnabled = false;
            btnStart.IsEnabled = true;
            SetItems();
        }

        private void CalculateProfitPerHour()
        {
            TimeSpan ts = timer.Elapsed;

            try
            {
                if (ts.Minutes == 0 && ts.Seconds != 0)
                {
                    profitPerHour = (60 / (60 / ts.Seconds)) * profit;
                }
                else if (ts.Hours == 0 && ts.Minutes != 0)
                {
                    profitPerHour = (60 / ts.Minutes) * profit;
                }
                else
                {
                    profitPerHour = profit / ts.Hours;
                }
            }
            catch { }
        }

        private void CalculateRollsPerHour()
        {
            TimeSpan ts = timer.Elapsed;

            try
            {
                if (ts.Minutes == 0 && ts.Seconds != 0)
                {
                    rollsPerHour = (60 / (60 / ts.Seconds)) * round;
                }
                else if (ts.Hours == 0 && ts.Minutes != 0)
                {
                    rollsPerHour = (60 / ts.Minutes) * round;
                }
                else
                {
                    rollsPerHour = round / ts.Hours;
                }
            }
            catch { }
        }

        private void tbInitialBet_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                initialBet = Convert.ToInt32(tbInitialBet.Text);
                currentBet = initialBet;
                currentBetColor = 0;
                round = 0;
                SetItems();
            }
            catch
            {

            }
        }

        private void SetItems()
        {
            lblCurrentBet.Content = "$" + currentBet.ToString();
            lblRound.Content = round.ToString();
            lblProfitPerHour.Content = "$" + profitPerHour.ToString();
            lblProfit.Content = "$" + profit.ToString();
            lblCurrentFunds.Content = "$" + currentFunds.ToString();
            lblCurrentWinStreak.Content = winStreak.ToString();
            lblLongestWinStreak.Content = longestWinStreak.ToString();
            lblCurrentLoseStreak.Content = loseStreak;
            lblLongestLoseStreak.Content = longestLoseStreak;
            lblRollsPerHour.Content = rollsPerHour.ToString();

            if (blackBet.Contains(currentBetColor))
            {
                btnColor.Background = Brushes.Black;
            }
            else
            {
                btnColor.Background = Brushes.Red;
            }

            SetUpcommingColors();

            TimeSpan ts = timer.Elapsed;
            lblPlayTime.Content = string.Format("{0}H:{1}M:{2}S", ts.Hours, ts.Minutes, ts.Seconds);
        }

        private void DoubleBet()
        {
            currentBet *= 2;
        }

        private void IncrementBetColor()
        {
            currentBetColor++;

            if(currentBetColor > 5)
            {
                currentBetColor = 0;
            }
        }

        private void SetUpcommingColors()
        {
            int colorsSet = 0;
            int nextColor = currentBetColor + 1;

            while(colorsSet != 5)
            {
                if(nextColor > 5)
                {
                    nextColor = 0;
                }

                Brush b = blackBet.Contains(nextColor) ? Brushes.Black : Brushes.Red;

                switch (colorsSet)
                {
                    case 0:
                        btnUpcommingOne.Background = b;
                        break;
                    case 1:
                        btnUpcommingTwo.Background = b;
                        break;
                    case 2:
                        btnUpcommingThree.Background = b;
                        break;
                    case 3:
                        btnUpcommingFour.Background = b;
                        break;
                    case 4:
                        btnUpcommingFive.Background = b;
                        break;
                }

                nextColor++;
                colorsSet++;
            }
        }

        private void tbFunds_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                startingFunds = Convert.ToInt32(((TextBox)sender).Text);
                profit = 0;
                profitPerHour = 0;
                currentFunds = startingFunds;
                SetItems();
            }
            catch { }
        }
    }
}
