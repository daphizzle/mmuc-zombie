using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace mmuc_zombie.app.helper
{
    public class Constants
    {
        public const string NONICKNAME = "nickname";
        public const string AVATARPATH = "/mmuc-zombie;component/ext/img/avatar.png";
        public const string FBCOMMENT = "Write your comment here...";

        public enum USERGAMEMODES : int { INIT = 0, IDLE, LOBBY, INGAME };
        public enum GAMEMODES : int { PENDING = 0, ACTIVE, FINISHED };
        public enum GAMESIZE : int { SMALL = 0, MEDIUM, BIG };
        
        public const string ROLE_ZOMBIE = "Zombie";
        public const string ROLE_SURVIVOR = "Survivor";
        public const string ROLE_OBSERVER = "Observer";

        public const int TOPK = 6;


        public const int SMALL_GAME_SIZE = 500;
        public const int MEDIUM_GAME_SIZE = 2000;
        public const int BIG_GAME_SIZE = 4000;

        public const int SMALL_GAME_INFECTION_RANGE = 25;
        public const int MEDIUM_GAME_INFECTION_RANGE = 100;
        public const int BIG_GAME_INFECTION_RANGE = 200;



        public const int SMALL_GAME_ZOOMFACTOR = 17;
        public const int MEDIUM_GAME_ZOOMFACTOR = 16;
        public const int BIG_GAME_ZOOMFACTOR = 15;

        public const int BOT_MOVEMENT = 10;

        public const int SMALL_GAME_INFECTION_RANGE_INC = 5;
        public const int MEDIUM_GAME_INFECTION_RANGE_INC = 10;
        public const int BIG_GAME_INFECTION_RANGE_INC = 20;

        public static Random random= new Random();
        

        
    }
}
