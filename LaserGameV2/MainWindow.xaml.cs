using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
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
using System.IO;

namespace LaserGame
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        internal Game game;
        public DispatcherTimer timer_display = new DispatcherTimer();

        Brush btnSendAPIDefault = new SolidColorBrush(Color.FromRgb(70,70,70));
        Brush btnSendAPITick = new SolidColorBrush(Color.FromRgb(210, 205, 40));
        bool btnAPITickEnable = false;

        String lastPlayerColor = "A255000255";

        public MainWindow()
        { 
            InitializeComponent();

            tb_players_Color.Text = "A040000040";

            btn_GConfig_New.Click += GConfig_New;
            btn_GStatus_Start.Click += GStatus_Start;
            btn_GStatus_Stop.Click += GStatus_Stop;
            btn_GScoresServer_Send.Click += GScoresServer_Send;
            btn_players_AddBLUE.Click += GPlayers_AddBlue;
            btn_players_AddRED.Click += GPlayers_AddRed;
            btn_players_Add.Click += GPlayers_Add;
            btn_chooseColor.Click += GPlayers_ChooseColor;
            btn_sendcommand.Click += GExpert_sendCommand;
            btn_sendcommand_allplayer.Click += GExpert_sendCommandAllPlayers;

            //tb_GScoresServer_URL.Text = "";
            tb_players_Pseudo.Text = "";

            lbl_GStatus_TimeCountdown.Content = timeLabel(0);
            lbl_GStatus_TimeCountdown.Foreground = Brushes.White;

            cb_GConfig_AddingScore.Items.Add("5");
            cb_GConfig_AddingScore.Items.Add("10");
            cb_GConfig_AddingScore.Items.Add("20");
            cb_GConfig_AddingScore.Items.Add("50");
            cb_GConfig_AddingScore.Items.Add("100");
            cb_GConfig_AddingScore.Items.Add("200");
            cb_GConfig_AddingScore.Items.Add("500");
            cb_GConfig_AddingScore.Items.Add("1000");

            cb_GConfig_AddingScore.SelectedIndex = 4;

            cb_GConfig_AutoKilled.Items.Add("0");
            cb_GConfig_AutoKilled.Items.Add("5");
            cb_GConfig_AutoKilled.Items.Add("10");
            cb_GConfig_AutoKilled.Items.Add("20");
            cb_GConfig_AutoKilled.Items.Add("50");
            cb_GConfig_AutoKilled.Items.Add("100");
            cb_GConfig_AutoKilled.Items.Add("200");
            cb_GConfig_AutoKilled.Items.Add("500");
            cb_GConfig_AutoKilled.Items.Add("1000");
            cb_GConfig_AutoKilled.SelectedIndex = 0;

            cb_GConfig_ScoreDeath.Items.Add("0");
            cb_GConfig_ScoreDeath.Items.Add("5");
            cb_GConfig_ScoreDeath.Items.Add("10");
            cb_GConfig_ScoreDeath.Items.Add("20");
            cb_GConfig_ScoreDeath.Items.Add("50");
            cb_GConfig_ScoreDeath.Items.Add("100");
            cb_GConfig_ScoreDeath.Items.Add("200");
            cb_GConfig_ScoreDeath.Items.Add("500");
            cb_GConfig_ScoreDeath.Items.Add("1000");
            cb_GConfig_ScoreDeath.SelectedIndex = 2;

            cb_GConfig_ScoreTeamShot.Items.Add("0");
            cb_GConfig_ScoreTeamShot.Items.Add("5");
            cb_GConfig_ScoreTeamShot.Items.Add("10");
            cb_GConfig_ScoreTeamShot.Items.Add("20");
            cb_GConfig_ScoreTeamShot.Items.Add("50");
            cb_GConfig_ScoreTeamShot.Items.Add("100");
            cb_GConfig_ScoreTeamShot.Items.Add("200");
            cb_GConfig_ScoreTeamShot.Items.Add("500");
            cb_GConfig_ScoreTeamShot.Items.Add("1000");

            cb_GConfig_ScoreTeamShot.SelectedIndex = 4;

            cb_GConfig_TimeBeforeStart.Items.Add("00:00");
            cb_GConfig_TimeBeforeStart.Items.Add("00:15");
            cb_GConfig_TimeBeforeStart.Items.Add("00:30");
            cb_GConfig_TimeBeforeStart.Items.Add("00:45");
            cb_GConfig_TimeBeforeStart.Items.Add("01:00");
            cb_GConfig_TimeBeforeStart.Items.Add("01:30");
            cb_GConfig_TimeBeforeStart.Items.Add("02:00");
            cb_GConfig_TimeBeforeStart.Items.Add("03:00");
            cb_GConfig_TimeBeforeStart.Items.Add("04:00");
            cb_GConfig_TimeBeforeStart.Items.Add("--------");
            cb_GConfig_TimeBeforeStart.Items.Add("00:03");
            cb_GConfig_TimeBeforeStart.Items.Add("00:06");


            cb_GConfig_TimeBeforeStart.SelectedIndex = 0;

            cb_GConfig_TimeGame.Items.Add("05:00");
            cb_GConfig_TimeGame.Items.Add("10:00");
            cb_GConfig_TimeGame.Items.Add("15:00");
            cb_GConfig_TimeGame.Items.Add("20:00");
            cb_GConfig_TimeGame.Items.Add("25:00");
            cb_GConfig_TimeGame.Items.Add("30:00");
            cb_GConfig_TimeGame.Items.Add("45:00");
            cb_GConfig_TimeGame.Items.Add("60:00");
            /*cb_GConfig_TimeGame.Items.Add("--------");
            cb_GConfig_TimeGame.Items.Add("00:05");
            cb_GConfig_TimeGame.Items.Add("00:10");
            cb_GConfig_TimeGame.Items.Add("00:20");
            cb_GConfig_TimeGame.Items.Add("00:30");
            cb_GConfig_TimeGame.Items.Add("00:45");
            cb_GConfig_TimeGame.Items.Add("01:00");*/


            cb_GConfig_TimeGame.SelectedIndex = 2;


            tbBackgroundChange(hotspotTextBoxesChange(), tb_ipS1, 1);     
            tbBackgroundChange(hotspotTextBoxesChange(), tb_ipS2, 2);     
            tbBackgroundChange(hotspotTextBoxesChange(), tb_ipS3, 3);       
            tbBackgroundChange(hotspotTextBoxesChange(), tb_ID_min, 4);      
            tbBackgroundChange(hotspotTextBoxesChange(), tb_ID_max, 5);     
            tbBackgroundChange(hotspotTextBoxesChange(), tb_startIP, 6);
            tbBackgroundChange(hotspotTextBoxesChange(), tb_timeoutcmd, 7);


            lb_scoreBlueTeam.Content = "0";
            lb_scoreRedTeam.Content = "0";

            pb_countdown.Value = 0;

            DataGrid_Players.Items.Clear();

            DateTime_GConfig_IDGAME.Value = DateTime.Now;

            MinHeight = Height;
            MinWidth = Width;
            //MaxHeight = Height;
            MaxWidth = Width;

            hotspotTextBoxesChange(); // format exemple: "192.168.191.*";

            game = new Game(this);
            game.timer.Tick += timer_Game_Tick;
            timer_display.Tick += timer_Display_Tick;
            timer_display.Interval = TimeSpan.FromMilliseconds(500);

            timer_display.Start();


            //tb_ESPComFolder.Text = "";
            espcomtbchanged();

            enableElements(false);
            btn_GStatus_Start.IsEnabled = true;

            btn_GScoresServer_Send.IsEnabled = false;

            btn_players_AddBLUE.IsEnabled = true;
            btn_players_AddRED.IsEnabled = true;
            btn_players_Add.IsEnabled = true;

            checkSyncWifi();


            GConfig_New(null, null);

        }

        private void GExpert_sendCommand(object sender, RoutedEventArgs e)
        {
            try
            {
                triche();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Expert-o: " + ex.ToString());
            }
        }

        private void GExpert_sendCommandAllPlayers(object sender, RoutedEventArgs e)
        {
            try
            {
                triche2();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Expert-o2: " + ex.ToString());
            }
        }

     
        private void triche()
        {
            try
            {
                string cmd = tb_expert_cmd.Text;
                string subip = tb_expert_subip.Text;
                int subipi = Convert.ToInt32(subip);

                string result = game.command_ESP_Com_SYNC("COLOR", new Player(subipi, "internal", "internal"), cmd);
                MessageBox.Show("Result:  "+result);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Expert-i: " + ex.ToString());
            }
        }

        private void triche2()
        {
            try
            {
                string cmd = tb_expert_cmd.Text;
                string totalresult = "";
                foreach (Player p in game.players)
                {
                    string result = game.command_ESP_Com_SYNC("COLOR", p, cmd);
                    totalresult += "subip" + p.subIpAddr + " : " + result + " \n";
                }
                MessageBox.Show("Result:  \n " + totalresult);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Expert-i: " + ex.ToString());
            }
        }

        private void GPlayers_ChooseColor(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();
            dlg.ShowDialog();

           System.Drawing.Color clr = dlg.Color;

            lastPlayerColor = "A";
            lastPlayerColor += clr.R.ToString().PadLeft(3, '0');
            lastPlayerColor += clr.G.ToString().PadLeft(3, '0');
            lastPlayerColor += clr.B.ToString().PadLeft(3, '0');
            lastPlayerColor += "z";

            tb_players_Color.Text = lastPlayerColor;
        }

        private void TimerEventProcessor(object sender, EventArgs e)
        {
            
        }

        private void timer_Game_Tick(object sender, EventArgs e) // game tick 1s
        {

            foreach (Player p in game.players)
            {
                p.gridlineRef.Kill = p.kill;
                p.gridlineRef.Death = p.death;
                p.gridlineRef.Score = p.score;
            }
            
            DataGrid_Players.Items.Refresh();

            lb_scoreRedTeam.Content = game.getRedScore();
            lb_scoreBlueTeam.Content = game.getBlueScore();

        }

        private void timer_Display_Tick(object sender, EventArgs e) // display tick 
        {

            // tick send api button
            sendApiSpecialButton();

            if (game.startAndPlaying){
                lbl_GStatus_TimeCountdown.Foreground = Brushes.OrangeRed;
                // progress bar
                try { pb_countdown.Value = 100 - 100 * game.timerCountdownSec / game.get_memoTimeGame(); } catch { }
            }

            lbl_GStatus_TimeCountdown.Content = timeLabel(game.timerCountdownSec);
        }

        public class GridLine
        {
            public string Pseudo { get; set; }
            public string Team { get; set; }
            public string Color { get; set; }
            public int Score { get; set; }
            public int Kill { get; set; }
            public int Death { get; set; }
            public Button del { get; set; }
            public DataGrid Parent { get; set; }
            public Player playerRef { get; set; }
            public Game gameRef { get; set; }
            public int posit_DataGrid { get; set; }
            public int id_displayWPFInfo { get; set; }
            public int subIpAddr_displayWPFInfo { get; set; }
           
            public GridLine()
            {
                del = new Button();
                del.Height = 19;
                del.Width = 18;
                del.Content = "x";
                del.Click += delClick;

                id_displayWPFInfo = -2;
                subIpAddr_displayWPFInfo = -2;
            }

            private void delClick(object sender, RoutedEventArgs e)
            {
                gameRef.deletePlayer(playerRef);
                Parent.Items.Remove(this);
            }
        }

        private GridLine addPlayerGrid(string _pseudo, string _team, string _playerColor = "")
        {
            GridLine gl = new GridLine
            {
                Pseudo = _pseudo,
                Team = _team,
                Color = _playerColor,
                Score = 0,
                Kill = 0,
                Death = 0,
                Parent = DataGrid_Players,
                gameRef = game,
                posit_DataGrid = -1
            };

            DataGrid_Players.Items.Add(gl);

            gl.posit_DataGrid = DataGrid_Players.Items.IndexOf(gl);

            return gl;
        }

        private void removePlayerGrid(GridLine gl)
        {
            DataGrid_Players.Items.Remove(gl);
        }

        private void GConfig_New(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("New Game ?", "My App", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            //stopGame();
            List<Player> keepplayers = new List<Player>();

            bool keepPlayers = false;


            // comment todo recheck:
            /*if (game.players.Count > 0) { 
                string textBox = "Would you like to keep the players from previous game? \n";
                MessageBoxResult res = WPFCustomMessageBox.CustomMessageBox.ShowYesNoCancel(textBox, "Warning", "Cancel", "No", "Yes", MessageBoxImage.Question);


                if (res == MessageBoxResult.Yes || res == MessageBoxResult.None || res == MessageBoxResult.OK)
                    return;

                else if (res == MessageBoxResult.No) {}

                else if (res == MessageBoxResult.Cancel)
                {
                    keepPlayers = true;                 
                }
                else
                    return;

            }*/

            DateTime_GConfig_IDGAME.Value = DateTime.Now;
            lbl_GStatus_TimeCountdown.Content = timeLabel(0);

            game.Stop();

            if (keepPlayers)
                foreach (Player p in game.players)
                {
                    p.score = 0;
                    p.kill = 0;
                    p.death = 0;
                }
            else
                game.deletePlayers();

            //game.replay();

            if (!keepPlayers)
            {
                if (game.players.Count > 0)
                    game.deletePlayers();

                DataGrid_Players.Items.Clear();
            }


            string textBox2 = "\n GAME MODE X_O \n";
            MessageBoxResult res2 = WPFCustomMessageBox.CustomMessageBox.ShowYesNo(textBox2, "Game Mode", "TEAM Game", "SELF Game", MessageBoxImage.Question);


            grid_teamMode.Visibility = Visibility.Hidden;
            grid_selfMode.Visibility = Visibility.Hidden;

            if (res2 == MessageBoxResult.Yes) //Team Game
            {
                grid_teamMode.Visibility = Visibility.Visible;
            }
            else //Himself Game
            {
                grid_selfMode.Visibility = Visibility.Visible;
            }

            btn_GStatus_Start.IsEnabled = true;
            btn_GStatus_Start.IsEnabled = true;
            btn_players_AddBLUE.IsEnabled = true;
            btn_players_AddRED.IsEnabled = true;
            btn_players_Add.IsEnabled = true;
            btn_GScoresServer_Send.IsEnabled = false;

            btnAPITickEnable = false;
        }

        private void GStatus_Start(object sender, RoutedEventArgs e)
        {

            if (!game.isAllPlayersInit())
            {
                MessageBox.Show("Some user are not init.","Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string vTimeBeforeStart = cb_GConfig_TimeBeforeStart.SelectedValue.ToString();
            string vTimeGame = cb_GConfig_TimeGame.SelectedValue.ToString();

            try
            {
                int vMINBeforeStart = Convert.ToInt32(vTimeBeforeStart.Substring(0, 2));
                int vSECBeforeStart = Convert.ToInt32(vTimeBeforeStart.Substring(3, 2));

                int vMINTimeGame = Convert.ToInt32(vTimeGame.Substring(0, 2));
                int vSECTimeGame = Convert.ToInt32(vTimeGame.Substring(3, 2));

                int globalTimeSecond = vMINBeforeStart + vMINTimeGame;
                globalTimeSecond *= 60;
                globalTimeSecond += vSECBeforeStart + vSECTimeGame;

                int timeGameSecond = vMINTimeGame * 60 + vSECTimeGame;

                // data games
                int scoreKill = Convert.ToInt32(cb_GConfig_AddingScore.SelectedValue.ToString());
                int scoreDeath = - Convert.ToInt32(cb_GConfig_ScoreDeath.SelectedValue.ToString());
                int scoreAutoKill = - Convert.ToInt32(cb_GConfig_AutoKilled.SelectedValue.ToString());
                int scoreTeamShot = - Convert.ToInt32(cb_GConfig_ScoreTeamShot.SelectedValue.ToString());

                game.Start(globalTimeSecond, timeGameSecond, scoreKill, scoreDeath, scoreAutoKill, scoreTeamShot);

                lbl_GStatus_TimeCountdown.Foreground = Brushes.Yellow;

                btn_GStatus_Start.IsEnabled = false;

                btn_players_AddBLUE.IsEnabled = false;
                btn_players_AddRED.IsEnabled = false;
                btn_players_Add.IsEnabled = false;

                enableElements(true);
                

            }
            catch { }
        }

        private void GStatus_Stop(object sender, RoutedEventArgs e)
        {
            game.Stop();
            stop_ButItsDisplayOnly();
        }

        public void stop_ButItsDisplayOnly()
        {
            lbl_GStatus_TimeCountdown.Content = timeLabel(game.timerCountdownSec);

            lbl_GStatus_TimeCountdown.Foreground = Brushes.White;
            pb_countdown.Value = 0;


            enableElements(false);
            btn_GStatus_Start.IsEnabled = false;
            btn_players_AddBLUE.IsEnabled = false;
            btn_players_AddRED.IsEnabled = false;
            btn_players_Add.IsEnabled = false;

            btnAPITickEnable = true;
        }

        private void GScoresServer_Send(object sender, RoutedEventArgs e)
        {
            btnAPITickEnable = false;

            string urlApi = tb_GScoresServer_URL.Text;
            string idGame = DateTime_GConfig_IDGAME.Text;
            game.sendScoreToAPI(idGame, urlApi);

        }

        // team btn click

        private void GPlayers_AddRed(object sender, RoutedEventArgs e)
        {
            hotspotTextBoxesChange();

            GridLine gl = addPlayerGrid(tb_players_Pseudo.Text, "RED", "A060000000z");
            
            if (!game.createPlayer(gl))
                removePlayerGrid(gl);

            tb_players_Pseudo.Text = "";
        }

        private void GPlayers_AddBlue(object sender, RoutedEventArgs e)
        {
            hotspotTextBoxesChange();

            GridLine gl = addPlayerGrid(tb_players_Pseudo.Text, "BLUE", "A000000060z");

            if (!game.createPlayer(gl))
                removePlayerGrid(gl);

            tb_players_Pseudo.Text = "";
        }

        // self btn click
        private void GPlayers_Add(object sender, RoutedEventArgs e)
        {
            hotspotTextBoxesChange();

            GridLine gl = addPlayerGrid(tb_players_Pseudo.Text, "SELF", tb_players_Color.Text);

            if (!game.createPlayer(gl))
                removePlayerGrid(gl);

            tb_players_Pseudo.Text = "";
        }

        private string timeLabel(int _total_seconds)
        {
            if (_total_seconds < 0) _total_seconds = 0;

            int minuts = _total_seconds / 60;
            int seconds = _total_seconds % 60;
            string minutsStr = minuts.ToString().PadLeft(2, '0');
            string secondsStr = seconds.ToString().PadLeft(2, '0');
            return minutsStr + ":" + secondsStr;
        }


        private void DataGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (sender as DataGrid).SelectedItem = null;
        }

        private void tbBackgroundChange(Tuple<bool, int>  t, Object _tb, int errCode)
        {
            try { 
                if (t.Item2 >= 0)
                    if (!t.Item1 && t.Item2 == errCode)
                        ((TextBox)_tb).Background = Brushes.OrangeRed;
                    else
                        ((TextBox)_tb).Background = Brushes.LightGoldenrodYellow;
            }
            catch {
                try
                {
                    ((TextBox)_tb).Background = Brushes.OrangeRed;
                }
                catch
                { }
            }
        }
        private void Tb_ipS1_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbBackgroundChange(hotspotTextBoxesChange(), tb_ipS1, 1);
        }

        private void Tb_ipS2_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbBackgroundChange(hotspotTextBoxesChange(), tb_ipS2, 2);
        }

        private void Tb_ipS3_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbBackgroundChange(hotspotTextBoxesChange(), tb_ipS3, 3);
        }

        private void Tb_ipS4_min_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbBackgroundChange(hotspotTextBoxesChange(), tb_ID_min, 4);
        }

        private void Tb_ipS4_max_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbBackgroundChange(hotspotTextBoxesChange(), tb_ID_max, 5);
        }

        private void Tb_startID_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbBackgroundChange(hotspotTextBoxesChange(), tb_startIP, 6);
        }

        private void Tb_timeoutcmd_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbBackgroundChange(hotspotTextBoxesChange(), tb_timeoutcmd, 7);
        }

        private Tuple<bool, int> hotspotTextBoxesChange()
        {
            int errCode = 0;
            try
            {

                if (tb_ipS1 == null) return new Tuple<bool, int> (false, -1);
                if (tb_ipS2 == null) return new Tuple<bool, int>(false, -2);
                if (tb_ipS3 == null) return new Tuple<bool, int>(false, -3);
                if (tb_ID_min == null) return new Tuple<bool, int>(false, -4);
                if (tb_ID_max == null) return new Tuple<bool, int>(false, -5);
                if (tb_startIP == null) return new Tuple<bool, int>(false, -6);
                if (tb_timeoutcmd == null) return new Tuple<bool, int>(false, -7);

                if (tb_ipS1.Text.Length <= 0) return new Tuple<bool, int>(false, 1);
                if (tb_ipS2.Text.Length <= 0) return new Tuple<bool, int>(false, 2);
                if (tb_ipS3.Text.Length <= 0) return new Tuple<bool, int>(false, 3);
                if (tb_ID_min.Text.Length <= 0) return new Tuple<bool, int>(false, 4);
                if (tb_ID_max.Text.Length <= 0) return new Tuple<bool, int> (false, 5);
                if (tb_startIP.Text.Length <= 0) return new Tuple<bool, int>(false, 6);
                if (tb_timeoutcmd.Text.Length <= 0) return new Tuple<bool, int>(false, 7);

                errCode = 1;
                int ipSub1 = Convert.ToInt32(tb_ipS1.Text);
                errCode = 2;
                int ipSub2 = Convert.ToInt32(tb_ipS2.Text);
                errCode = 3;
                int ipSub3 = Convert.ToInt32(tb_ipS3.Text);
                errCode = 4;
                int idmin = Convert.ToInt32(tb_ID_min.Text);
                errCode = 5;
                int idmax = Convert.ToInt32(tb_ID_max.Text);
                errCode = 6;
                int ipstart = Convert.ToInt32(tb_startIP.Text);
                errCode = 7;
                int timeoutcmd = Convert.ToInt32(tb_timeoutcmd.Text);
                errCode = 0;

                tb_ipS1.Text = (ipSub1 = Math.Max(Math.Min(ipSub1, 255), 0)).ToString();
                tb_ipS2.Text = (ipSub2 = Math.Max(Math.Min(ipSub2, 255), 0)).ToString();
                tb_ipS3.Text = (ipSub3 = Math.Max(Math.Min(ipSub3, 255), 0)).ToString();
                tb_ID_max.Text = (idmax = Math.Max(Math.Min(idmax, 99), 0)).ToString();
                tb_ID_min.Text = (idmin = Math.Max(Math.Min(idmin, 99), 0)).ToString();
                tb_startIP.Text = (ipstart = Math.Max(Math.Min(ipstart, 255), 0)).ToString();
                tb_timeoutcmd.Text = (timeoutcmd = Math.Max(Math.Min(timeoutcmd, 99999), 0)).ToString();

                string ipStringPublicFormat = ipSub1 + "." + ipSub2 + "." + ipSub3 + "." + "*";

                if (game != null)
                {
                    game.setPublic3I_IpFormat(ipStringPublicFormat);
                    game.setIdPlayersRange(idmin, idmax);
                    game.setIpStart(ipstart);
                    game.setTimeoutCMD(timeoutcmd);
                }
                else throw new Exception("game entity is null");

                return new Tuple<bool,int>(true, 0);

            }
            catch {
                return new Tuple<bool, int>(false, errCode);
            }
            
        }

        internal static void showMessageBox(string v)
        {
            MessageBox.Show(v);
        }

        private void Tb_ESPComFolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            espcomtbchanged();
        }

        private void espcomtbchanged()
        {
            if (game == null) return;
            try
            {
                string folder = tb_ESPComFolder.Text;
                
                if (Directory.Exists(folder))
                {
                    tb_ESPComFolder.Background = Brushes.LightGoldenrodYellow;
                    game.node_ESP_COM_repository = folder;
                    return;
                }
            }
            catch
            {
            }
            tb_ESPComFolder.Background = Brushes.OrangeRed;
        }

        private void enableElements(bool _enable)
        {
            bool _disable = !_enable;

            btn_GConfig_New.IsEnabled = _disable;
            btn_GStatus_Stop.IsEnabled = _enable;
            tb_ESPComFolder.IsEnabled = _disable;
            tb_ID_max.IsEnabled = _disable;
            tb_ID_min.IsEnabled = _disable;
            tb_ipS1.IsEnabled = _disable;
            tb_ipS2.IsEnabled = _disable;
            tb_ipS3.IsEnabled = _disable;
            tb_startIP.IsEnabled = _disable;
            tb_timeoutcmd.IsEnabled = _disable;
            tb_players_Pseudo.IsEnabled = _disable;
            btn_GScoresServer_Send.IsEnabled = _disable;
            cb_GConfig_TimeBeforeStart.IsEnabled = _disable;
            cb_GConfig_TimeGame.IsEnabled = _disable;
            cb_GConfig_AddingScore.IsEnabled = _disable;
            cb_GConfig_AutoKilled.IsEnabled = _disable;
            cb_GConfig_ScoreDeath.IsEnabled = _disable;
            cb_GConfig_ScoreTeamShot.IsEnabled = _disable;

            foreach (Player p in game.players)
            {
                p.gridlineRef.del.IsEnabled = _disable;
            }
        }

        private void sendApiSpecialButton()
        {
            if (btnAPITickEnable) { 
                if (btn_GScoresServer_Send.Background == btnSendAPITick)
                    btn_GScoresServer_Send.Background = btnSendAPIDefault;
                else
                    btn_GScoresServer_Send.Background = btnSendAPITick;
            }else
                btn_GScoresServer_Send.Background = btnSendAPIDefault;

        }

        private void Cb_syncwifi_Checked(object sender, RoutedEventArgs e)
        {
            checkSyncWifi();
        }
        private void checkSyncWifi()
        {
            bool bl = false;
            if (cb_syncwifi.IsChecked == true)
                bl = true;
            if (game != null) game.syncWifi = bl;
        }
    }
}
