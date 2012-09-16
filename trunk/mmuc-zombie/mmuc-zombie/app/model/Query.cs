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
using Parse;
using mmuc_zombie.app.model;
using System.Linq;
using mmuc_zombie.app.helper;

public class Query
{
    static public void getUsersByGame(string gameId, Action<Response<ResultsResponse<User>>> callback)
    {
        var parse = new Driver();
        parse.Objects.Query<User>().Where(c => c.activeGame == gameId).Execute(callback);

    }
    static public void getBotsByGame(string gameId, Action<Response<ResultsResponse<User>>> callback)
    {
        var parse = new Driver();
        parse.Objects.Query<User>().Where(c => c.activeGame == gameId && c.bot == true ).Execute(callback);

    }
    static public void getRolesbyGame(string gameId, Action<Response<ResultsResponse<Roles>>> callback)
    {
        var parse = new Driver();
        parse.Objects.Query<Roles>().Where(c => c.gameId == gameId && c.alive==true).Execute(callback);
    }

    static public void getGame(string gameId, Action<Response<Game>> callback)
    {
        var parse = new Driver();
        parse.Objects.Get<Game>(gameId, callback);

    }
    static public void getGameArea(string gameId, Action<Response<ResultsResponse<MyLocation>>> callback)
    {
        var parse = new Driver();
        parse.Objects.Query<MyLocation>().Where(c => c.gameId == gameId).Execute(callback);

    }
    static public void getUser(string userId, Action<Response<User>> callback)
    {
        var parse = new Driver();
        parse.Objects.Get<User>(userId, callback);

    }
    
   static public void getRole(string roleId, Action<Response<Roles>> callback)
   {
       var parse = new Driver();
       parse.Objects.Get<Roles>(roleId, callback);

   }
   static public void getLocation(string locationId, Action<Response<MyLocation>> callback)
   {
       var parse = new Driver();
       parse.Objects.Get<MyLocation>(locationId, callback);
   }

   static public void getAllSurvivors(Action<Response<ResultsResponse<Roles>>> callback)
   {
       var parse = new Driver();
       string surv = Constants.ROLE_SURVIVOR;
       parse.Objects.Query<Roles>().Where(c => c.roleType == surv).SortDescending(c => c.questCount).Execute(callback);
   }
   static public void getAllZombies(Action<Response<ResultsResponse<Roles>>> callback)
   {
       var parse = new Driver();
       string zomb = Constants.ROLE_ZOMBIE;
       parse.Objects.Query<Roles>().Where(c => c.roleType == zomb).SortDescending(c => c.questCount).Execute(callback);
   }

   static public void getAllGames(Action<Response<ResultsResponse<Game>>> callback)
   {
       var parse = new Driver();
       parse.Objects.Query<Game>().Where(c => c.Id == c.Id).Execute(callback);
   }

   static public void getAllUser(Action<Response<ResultsResponse<User>>> callback)
   {
       var parse = new Driver();
       parse.Objects.Query<User>().Where(c => c.Id == c.Id).Execute(callback);
   }

   static public void getQuest(String gameId, Action<Response<ResultsResponse<Quest>>> callback)
   {
       var parse = new Driver();
       parse.Objects.Query<Quest>().Where(c => c.gameId == gameId).Execute(callback);
   }
            
   [Obsolete("doesn't work atm")]
   static public void getLocations(string[] locationIds, Action<Response<ResultsResponse<MyLocation>>> callback)
   {
       var parse = new Driver();
       parse.Objects.Query<MyLocation>().Where(c => locationIds.Contains(c.Id)).Execute(callback);

   }
   [Obsolete("doesn't work atm")]
   static public void getRoles(string[] roleIds, Action<Response<ResultsResponse<Roles>>> callback)
   {
       var parse = new Driver();
       parse.Objects.Query<Roles>().Where(c => roleIds.Contains(c.Id)).Execute(callback);

   }
}

