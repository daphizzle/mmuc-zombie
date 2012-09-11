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

        public enum USERGAMEMODES : int { INIT = 0, IDLE, LOBBY, INGAME };

        //int=0: "idle mode" he is doing nothing, he ha no pending games; no timertask is running, if he joins a game he switch to status 1
        //int=1: "lobby-mode" user has joined a game , timertask checks if gameowner creates a game. if he does user switch to status 3. user can leave:if he has pending games left he switch to status 1, if not he switch to status 0.
        //int=2: "ingame" mode  many things are checked ...(infection, userlocation ...)if he leaves a game: it is checked if he has pending events if yes he switches to status 1 else 0

    }
}
