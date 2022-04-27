using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Xceed.Wpf.Toolkit;
using static LaserGame.MainWindow;
using Newtonsoft.Json;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.IO;

namespace LaserGame
{
    public class Game
    {

        public List<Player> players = new List<Player>();
        private int ipStart = 2;
        public string node_ESP_COM_repository = @"";
        public DispatcherTimer timer = new DispatcherTimer();
        public int timerCountdownSec = 0;
        int timeGame = 0;
        int memoTimeGame = 0;

        int scoreKill = 0;
        int scoreDeath = 0;
        int scoreAutoKill = 0;
        int scoreTeamShot = 0;

        int _timeoutCommandMs = 1000;

        string public3I_Ip = "";
        int limitMinFilterSubIPPlayer = 2;
        int limitMaxFilterSubIPPlayer = 255;

        int limitMinFilterIdPlayer = 10;
        int limitMaxFilterIdPlayer = 99;

        public bool start = false;
        public bool startAndPlaying = false;
        public bool stop = false;

        public bool syncWifi = true;

        //int offsetActionTicker = 5; // < to remove
        
        MainWindow interfaceMW;
        public Game(MainWindow _mw)
        {
            interfaceMW = _mw;

            timer.Tick += timer_Tick;

            start = false;
            startAndPlaying = false;

            init();
        }

        public int get_memoTimeGame()
        {
            return memoTimeGame;
        }

        public bool isAllPlayersInit()
        {
            bool r = true;

            foreach (Player p in players)
                if (p.isInit != true) r = false;

            return r;
        }

        public void setPublic3I_IpFormat(string _public_ip3I, int _minSubIp, int _maxSubIp)
        {
            public3I_Ip = to3I_Ip(_public_ip3I);
            limitMinFilterSubIPPlayer = _minSubIp;
            limitMaxFilterSubIPPlayer = _maxSubIp;
        }

        public void setPublic3I_IpFormat(string _public_ip3I)
        {
            public3I_Ip = to3I_Ip(_public_ip3I);
        }

        public void setIdPlayersRange(int _min, int _max)
        {
            limitMinFilterIdPlayer = _min;
            limitMaxFilterIdPlayer = _max;
        }
        public void setIpStart(int _ipst)
        {
            ipStart = _ipst;
        }
        public void setTimeoutCMD(int _timeo)
        {
            _timeoutCommandMs = _timeo;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (start)
            {

                if(timerCountdownSec <= 0)
                    gameJustStopping_Tick();

                else if (timerCountdownSec == timeGame)
                    gameJustStarting_Tick();

                if (timerCountdownSec <= timeGame && timerCountdownSec >0)
                    gameOn_Tick();

                timerCountdownSec -= 1;

            }

            if (stop)
            {
                gameOff_Tick();
            }

            // actualise datagrids infos
            foreach(Player player in players)
            {
                player.gridlineRef.id_displayWPFInfo = player.id;
                player.gridlineRef.subIpAddr_displayWPFInfo = player.subIpAddr;
            }

        }



        private void gameOff_Tick()
        {
            //command_ESP_ComAsync_AllPlayers("STOP"); // everyone

            command_ESP_ComAsync_AllPlayers("GET_BUFFER");
        }

        private void gameJustStopping_Tick()
        {
            Stop();
        }
        private void gameJustStarting_Tick()
        {

            bufferCancelling();

            startAndPlaying = true;
            memoTimeGame = timerCountdownSec;


            command_ESP_ComAsync_AllPlayers("START"); // everyone

        }


        private void gameOn_Tick()
        {
            startAndPlaying = true;

            //command_ESP_ComAsync_AllPlayers("START"); // everyone

            //refresh kills deaths scores
            command_ESP_ComAsync_AllPlayers("GET_BUFFER");


            //test:  
            //foreach (Player p in players)
            //    setColorPlayer(p);
            //test:  
            //command_ESP_ComAsync_AllPlayers("START");
        }


        private void bufferCancelling()
        {

            command_ESP_ComSYNC_AllPlayers("GET_BUFFER");

            foreach (Player p in players){
                p.score = 0;
                p.kill = 0;
                p.death = 0;
            }
        }

        private void actionOnCommand_Tick(Player _player, string cmd, string resultOutput)
        {

            //if (resultOutput == "TIMEOUT") return;
            //if (resultOutput == "ERROR") return;

            if (cmd == "GET_BUFFER")
            {
                // replace buffer
                if (_player.last_buffer.Length < resultOutput.Length)
                {
                    try { 
                        List<Player> killers = bufferToKillers(resultOutput, _player.position_buffer);

                        _player.death += killers.Count;
                        _player.score += scoreDeath * killers.Count;

                        // give points to all killers
                        foreach (Player killer in killers)
                        {
                            try
                            {
                                if (_player.id != killer.id)
                                   {


                                    if (_player.team == killer.team && (_player.team.ToUpper() == "BLUE" || _player.team.ToUpper() == "RED"))
                                            killer.score += scoreTeamShot;
                                    else
                                    {
                                        killer.score += scoreKill;
                                        killer.kill += 1;
                                    }
                                }
                                else
                                {
                                    _player.score -= scoreDeath; // < cancel
                                    _player.score += scoreAutoKill;
                                }
                            }
                            catch (Exception ex){ System.Windows.MessageBox.Show("ingameerror: "+ex.ToString()); }
                        }
                        _player.last_buffer = resultOutput;
                        _player.position_buffer = _player.last_buffer.Length;


                    }
                    catch { }

                    
                }

            }
        }

        private List<Player> bufferToKillers(string _resultOutput, int _withStartPositionBuffer)
        {
            List<Player> killers = new List<Player>();

            for(int i = _withStartPositionBuffer-1; i < _resultOutput.Length; i++)
            {
                try
                {
                    if (_resultOutput[i] == 'K')
                    {
                        string idT = "" + _resultOutput[i + 1] + _resultOutput[i + 2];
                        int id = Convert.ToInt32(idT);

                        Debug.WriteLine("killer id detected: "+ id); 
                        Player killer = findById(id);
                        if(killer != null)
                        {
                            killers.Add(killer);
                        }

                    }
                }
                catch { }
            }

            return killers;
        }

        private Player findById(int _id)
        {
            foreach(Player p in players)
            {
                if (_id == p.id) return p;
            }
            return null;
        }

        internal void init()
        {
            players = new List<Player>();

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }

        internal void Start(int _timeSecGlobal, int _timeSecGame, 
            int _score_Kill, int _score_Death, int _score_AutoKill, int _score_TeamShot)
        {

            timerCountdownSec = _timeSecGlobal;
            timeGame = _timeSecGame;

            stop = false;

            // data games         
            ChangeDataGames(_score_Kill, _score_Death, _score_AutoKill, _score_TeamShot);


            // reset data for current players 
            foreach (Player p in players)
            {
                p.score = 0;
                p.death = 0;
                p.kill = 0;
                

                // setColorPlayer(p);
            }

            start = true;
            timer.Start();
        }

        internal void ChangeDataGames(int _score_Kill, int _score_Death, int _score_AutoKill, int _score_TeamShot)
        {
            // data games         
            scoreKill = _score_Kill;
            scoreDeath = _score_Death;
            scoreAutoKill = _score_AutoKill;
            scoreTeamShot = _score_TeamShot;

        }

        internal void Stop()
        {
            command_ESP_ComAsync_AllPlayers("STOP"); // everyone

            timerCountdownSec = 0;
            start = false;
            startAndPlaying = false;
            stop = true;

            //timer.Stop();

            interfaceMW.stop_ButItsDisplayOnly();
        }

        public int getRedScore()
        {
            int score = 0;
            foreach (Player p in players)
                if (p.team == "RED") score += p.score;
            return score;
        }

        public int getBlueScore()
        {
            int score = 0;
            foreach (Player p in players)
                if (p.team == "BLUE") score += p.score;
            return score;
        }


        public bool createPlayer(GridLine _gl)
        {
            return createPlayer(_gl, 0);
        }

        public bool createPlayer(GridLine _gl, int check_nextIp)
        {
            string _pseudo = _gl.Pseudo;
            string _color = _gl.Team;

            if(_color == "SELF" && _gl.Color.Length > 0)
            {
                _color = _gl.Color;
            }

            // asign ip player
            int ip = getFreeIp(check_nextIp);

            Player p = new Player(ip, _pseudo, _color);
            _gl.playerRef = p;
            p.gridlineRef = _gl;

            // init player
            string isInitComplete = command_ESP_Com_SYNC("INIT", p);

            p.isInit = false;
            if (isInitComplete != "TIMEOUT" && isInitComplete != "ERROR")
                p.isInit = true;

            // get id player

            //to remove:        Task<


            /*if (isInitComplete == "TIMEOUT")
                return ShowCancelRetryNext(_pseudo, "Command 'INIT' to ESP board too long (Timeout).", "", _gl, check_nextIp);

            if (isInitComplete == "ERROR")
                return ShowCancelRetryNext(_pseudo, "Command 'INIT' to ESP board has return an error (Internal Error).", "", _gl, check_nextIp);*/


            string idPlayerOutput = command_ESP_Com_SYNC("GET_ID", p);


            //to remove:        idPlayerOutputTask.Wait(); //.Result;

            if (idPlayerOutput == "TIMEOUT")
                return ShowCancelRetryNext(_pseudo, "Command 'GET ID' to ESP board too long (Timeout).", "", _gl, check_nextIp);

            if (idPlayerOutput == "ERROR")
                return ShowCancelRetryNext(_pseudo, "Command 'GET ID' to ESP board has return an error (Internal Error).", "", _gl, check_nextIp);


            if (idPlayerOutput == null) p.id = -1;
            else
                if (idPlayerOutput.Length > 0)
                {
                    try
                    {
                        p.id = Convert.ToInt32(idPlayerOutput);
                    }
                    catch
                    {
                        p.id = -1;
                    }
                }
                else p.id = -1;

            // is on limit IDs
            if (p.id >= 0)
            {

                if(p.id >= limitMinFilterIdPlayer && p.id <= limitMaxFilterIdPlayer)
                {
                    _gl.Pseudo = _gl.Pseudo;

                    // set color
                    setColorPlayer(p);

                    // add on listf
                    players.Add(p);
                }
                else
                {
                    string textBox = "The player <" + _pseudo + "> cannot be added to the list. \n";
                    textBox += "The id is not on range (Client Limitation Error). \n";
                    textBox += "id: " + p.id + " ; ";
                    textBox += "sub ip: " + ip + " \n\n";
                    textBox += "What do we do ?\n\n";

                    MessageBoxResult res = WPFCustomMessageBox.CustomMessageBox.ShowYesNo(textBox, "Warning", "Cancel", "NEXT GUN");

                    if (res == MessageBoxResult.Yes || res == MessageBoxResult.None || 
                        res == MessageBoxResult.OK || res == MessageBoxResult.Cancel)
                        return false;

                    if (res == MessageBoxResult.No) { 
                        check_nextIp++;
                        return createPlayer(_gl, check_nextIp);
                    }

                }

            }
            else
                return ShowCancelRetryNext(_pseudo, "The id is not correct (Material Error).",
                    "id: " + p.id + " ; " + "sub ip: " + ip + " \n\n",
                    _gl, check_nextIp
                );

            return true;

        }



        private bool ShowCancelRetryNext(string _pseu, string _msg, string _complementMsg, GridLine _gl, int _check_next_ip)
        {
            string textBox = "The player <" + _pseu + "> cannot be added to the list. \n";
            textBox += _msg + " \n";
            textBox += _complementMsg + " \n";
            textBox += "What do we do ?\n\n";
            textBox += "IF YOU DON'T KNOW, PLEASE CLICK ''NEXT GUN'' !";

            MessageBoxResult res = WPFCustomMessageBox.CustomMessageBox.ShowYesNoCancel(textBox, "Warning", "Cancel", "Retry", "NEXT GUN", MessageBoxImage.Question);

            if (res == MessageBoxResult.Yes || res == MessageBoxResult.None || res == MessageBoxResult.OK)
                return false;

            else if (res == MessageBoxResult.No)
                return createPlayer(_gl, _check_next_ip);

            else if (res == MessageBoxResult.Cancel)
            {
                _check_next_ip++;
                return createPlayer(_gl, _check_next_ip);
            }
            else
                return false;
        }

        private void setColorPlayer(Player p)
        {
            try { 
                string _color = p.gridlineRef.Team;
                string _colorRGB = p.gridlineRef.Color;

                if (_colorRGB[0] != 'A') _colorRGB = "";
                if (_colorRGB.Length > 10) _colorRGB.Substring(1,10);

                //if (_color == "RED") command_ESP_ComAsync("RED", p);
                //else if (_color == "BLUE") command_ESP_ComAsync("BLUE", p);
                //else if (_color == "SELF") command_ESP_ComAsync("COLOR", p, _colorRGB);
                command_ESP_ComAsync("COLOR", p, _colorRGB);
                //else command_ESP_ComAsync("NO_COLOR", p);
            }
            catch { }
        }
        
        private int getFreeIp(int check_nextIp)
        {
            int z;
            z = check_nextIp;

            for (z = check_nextIp; z < players.Count + check_nextIp; z++)
            {
                Player playerF = players[z- check_nextIp];
                if ( !checkIpExist(z + ipStart) ) return z + ipStart;
            }

            return z + ipStart;
        }

        private int getFreeIp()
        {
            return getFreeIp(0);
        }

        private bool checkIpExist(int _ip)
        {
            for (int z = 0; z < players.Count; z++)
            {
                if(players[z] != null)
                    if (players[z].getSubIpAddr() == _ip)
                    {
                        return true;
                    }
            }
            return false;
        }

        public void deletePlayer(Player _playerToDelete)
        {
            command_ESP_ComAsync("NO_COLOR", _playerToDelete);
            players.Remove(_playerToDelete);
            _playerToDelete.gridlineRef = null;
            _playerToDelete = null;

        }

        public void deletePlayers()
        {
            for( int i = 0; i < players.Count; i++)
            {
                Player p = players[i];
                command_ESP_ComAsync("NO_COLOR", p);
                p.gridlineRef = null;
                p = null;
            }

            players = new List<Player>();
        }


        private string to3I_Ip(string ip)
        {
            try
            {
                string[] w = ip.Split('.');
                return w[0] + "." + w[1] + "." + w[2] + ".";
            }
            catch { }
            return "";
        }

        delegate string ReadLineDelegate();

        public async void command_ESP_ComAsync(string _cmd,  Player _p, string _args = "")  //  cmd_<NAMEFILE>  ,    _subIp  0 = *
        {

            int _timeout = _timeoutCommandMs;
            Task.Run(() => timedCommandVoid(_cmd, _p, _timeout, _args));

        }

        public async void command_ESP_ComAsync_AllPlayers(string _cmd, string _args = "")  //  cmd_<NAMEFILE>  ,    _subIp  0 = *
        {
            int _timeout = _timeoutCommandMs;
            foreach (Player p in players)
                Task.Run(() => timedCommandVoid(_cmd, p, _timeout, _args));

        }
        public async void command_ESP_ComSYNC_AllPlayers(string _cmd, string _args = "")  //  cmd_<NAMEFILE>  ,    _subIp  0 = *
        {
            int _timeout = _timeoutCommandMs;
            foreach (Player p in players)
                await Task.Run(() => timedCommandVoid(_cmd, p, _timeout, _args));

        }

        private void timedCommandVoid(string _cmd, Player _p, int _timeout, string _args = "")
        {
            string resultOutput = timedCommand(_cmd, _p, _timeout, _args).Result;
            actionOnCommand_Tick(_p, _cmd, resultOutput);
        }

        public /*async Task<*/string command_ESP_Com_SYNC(string _cmd, Player _p, string _args = "")
        {
            int _timeout = _timeoutCommandMs;
            string resultOutput = timedCommand(_cmd, _p, _timeout, _args).Result;
            actionOnCommand_Tick(_p, _cmd, resultOutput);
            return resultOutput;
        }


        private async Task<string> timedCommand(string _cmd, Player _p, int _timeoutms, string _argcmd = "")
        {
            try {

                return timedCommandForceTimeout(_cmd, _p, _timeoutms, _argcmd);

            }
            catch (Exception ex)
            {
                return "ERROR";

            }

        }

        private string timedCommandForceTimeout(string _cmd, Player _p, int _timeoutms, string _argcmd = "") { 
        
            // WTF

        if (!syncWifi) _timeoutms = 1;

        int _subIp = _p.subIpAddr;

        bool isConsoleHide = true;

        string preCommand = "";
        preCommand += "cmd_" + _cmd + ".js ";
        preCommand += _argcmd;
        if (_argcmd.Length > 0) preCommand += " ";
        preCommand += "" + public3I_Ip + (_subIp > 1 ? _subIp.ToString() : "*");

        string output = "...";

        try
        {
            string cibleDirectoryESPCOM = node_ESP_COM_repository;

            Process P = new Process();
            P.StartInfo.WorkingDirectory = cibleDirectoryESPCOM;
            P.StartInfo.FileName = "node.exe";
            P.StartInfo.Arguments = preCommand;
            P.StartInfo.RedirectStandardOutput = true;
            P.StartInfo.RedirectStandardInput = true;
            P.StartInfo.UseShellExecute = false;
            if(isConsoleHide == true) {
                P.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                P.StartInfo.CreateNoWindow = true;
            }
            P.Start();

            //var output = P.StandardOutput.ReadToEndAsync();

            /////////////// output = exeAsyncProcess(P, _timeoutms);


            ReadLineDelegate d = P.StandardOutput.ReadToEnd;

            IAsyncResult result = d.BeginInvoke(null, null);
            result.AsyncWaitHandle.WaitOne(_timeoutms);

            if (result.IsCompleted)
            {
                string resultstr = d.EndInvoke(result);
                output = resultstr;
                Console.WriteLine("out delgt: " + output);

                try
                {
                    P.Kill();
                }
                catch { }
                try
                {
                    P.Close();
                }
                catch { }


                return output;

            }
            else
            {
                Console.WriteLine("Timed out!");

                try
                {
                    P.Kill();
                }
                catch { }
                try
                {
                    P.Close();
                }
                catch { }

                return "TIMEOUT"; 
            }

        }
        catch (Exception e)
        {
            Console.WriteLine("ERR CMD: node.exe -- " + preCommand);
            Console.WriteLine(":: " + e.ToString());

            return "ERROR";
        };
            
            /*


        // get config player

        List<Tuple<string, string, string>> commands = new List<Tuple<string, string, string>>{
                new Tuple<string, string, string>("player", "init","I"),
                new Tuple<string, string, string>("player", "start","S"),
                new Tuple<string, string, string>("player", "stop","E"),
                new Tuple<string, string, string>("player", "color_red","R"),
                new Tuple<string, string, string>("player", "color_blue","B"),
                new Tuple<string, string, string>("player", "color_green","G"),
                new Tuple<string, string, string>("player", "color_none","N"),
                //new Tuple<string, string>("A*********","A*********"),
                new Tuple<string, string, string>("player", "get_id_arduino","P"),
                //new Tuple<string, string, string>("player", "get_id","P"),
                new Tuple<string, string, string>("player", "buffer_esp_id","C"),
                new Tuple<string, string, string>("player", "C","C"),
                //new Tuple<string, string>("set_id","D**"),
                //new Tuple<string, string, string>("server", "id_killer","K"),
                //new Tuple<string, string, string>("server", "id_player","D"),
            };
  
            int _subIp = _p.subIpAddr;

            // get buffer

            if (_subIp > 1) // one player
            {
                HttpRequestMessage hrm = new HttpRequestMessage();
                hrm.Method = HttpMethod.Post;

                string key = "";
                string cmndId = "";

                if (_cmd.ToUpper() == "GET_ID")
                {
                    key = "player";
                    cmndId = "PCP";
                }
                else if (_cmd[0] == 'A')
                {
                    key = "player";
                    cmndId = "A";
                }
                else if (_cmd[0] == 'D')
                {
                    key = "player";
                    cmndId = "D";
                    if (cmndId.Length > 3) cmndId.Substring(1, 3);
                }
                else
                    foreach (Tuple<string, string, string> tc in commands)
                    {
                        string fkey = tc.Item1;
                        string fcmndKey = tc.Item2;
                        string fcmndId = tc.Item3;

                        if(fcmndKey.ToUpper() == _cmd.ToUpper())
                        {
                            key = fkey;
                            cmndId = fcmndId;
                        }
                   

                    }


                string url = "http://" + public3I_Ip + _subIp.ToString();
                url += "?";
                url += key;
                url += "=";
                url += cmndId;
                url += "&server";



                HttpResponseMessage response = httpClient.PostAsync(String.Empty, new StringContent("")).Result;
                var responseString = response.Content.ReadAsStringAsync();

                string s = responseString.Result;

                return s;
            }
            else // all players
            {
                throw new Exception("TIMEOUT");
            }
            */

        }

        internal async void sendScoreToAPI(string idGame, string urlApi)
        {
            Task.Run(() => command_API_ComAsync(urlApi, idGame, players));
        }




        private async Task<bool> command_API_ComAsync(string _urlApi, string _idGame, List<Player> _players)
        {
            try
            {
                HttpClient clientToken = new HttpClient();

                string JsonTokenR = "{"+
                                    "\"email\" : \"admin@lasergame.com\"," +
                                    "\"password\" : \"ynHh*Z^K^eM9Ke*Krm@fx5FQf6%frv6n\"" +
                                    "}";

                var contentToken = new StringContent(JsonTokenR.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage resultToken = clientToken.PostAsync(_urlApi + "/auth/login", contentToken).Result;

                string messageToken = await resultToken.Content.ReadAsStringAsync();

                JObject jsonToken = JObject.Parse(messageToken);

                string token = jsonToken.Value<string>("access_token") ?? "";


                Debug.WriteLine(token);

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                JObject jsonGlobal = new JObject();

                JArray jsonPlayers = new JArray();

                foreach (Player p in _players)
                {
                    JObject jsonPlayer = new JObject();

                    // /// / / / / // jsonPlayer.Add("id", p.id.ToString());
                    jsonPlayer.Add("pseudo", p.pseudo);
                    jsonPlayer.Add("team", p.team);
                    jsonPlayer.Add("score", p.score.ToString());
                    jsonPlayer.Add("kills", p.kill.ToString());
                    jsonPlayer.Add("deaths", p.death.ToString());

                    jsonPlayers.Add(jsonPlayer);
                }

                string jsonPlayersSTR = JsonConvert.SerializeObject(jsonPlayers);

                //DateTime _idGameDate = DateTime.Parse(_idGame);

                _idGame = _idGame.Replace("/", "-");

                jsonGlobal.Add("date", _idGame);
                jsonGlobal.Add("players", jsonPlayers);

                string content = jsonGlobal.ToString();

                Debug.WriteLine("");
                Debug.WriteLine(content);
                Debug.WriteLine("");

                // var response = await client.PostAsync(_urlApi + "/player/", content);
                StringContent sc = new StringContent(content, Encoding.UTF8, "application/json");

                //var response = await client.PostAsync(_urlApi + "/games", sc);
                HttpResponseMessage response = await client.PostAsync(_urlApi + "/games", sc);

                if (response.IsSuccessStatusCode) {
                    System.Windows.MessageBox.Show("Success.");
                }
                else
                {
                    System.Windows.MessageBox.Show("Error: "+ response.StatusCode +" .. ");
                }

                var responseString = await response.Content.ReadAsStringAsync();

                Debug.WriteLine("----");
                Debug.WriteLine(responseString);

                return true;
            }

            catch (Exception ex) {
                Debug.WriteLine(ex);
                System.Windows.MessageBox.Show("Error: \n " + ex );

                return false;
            }

        }
    }
}
