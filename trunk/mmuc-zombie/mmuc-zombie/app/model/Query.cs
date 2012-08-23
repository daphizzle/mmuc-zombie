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


public class Query
{
    static public void getUsersByGame(string gameId, Action<Response<ResultsResponse<User>>> callback)
    {
        var parse = new Driver();
        parse.Objects.Query<User>().Where(c => c.activeGame == gameId).Execute(callback);

    }
    static public void getGame(string gameId, Action<Response<Games>> callback)
    {
        var parse = new Driver();
        parse.Objects.Get<Games>(gameId, callback);

    }
    static public void getGameArea(string gameId, Action<Response<ResultsResponse<MyLocation>>> callback)
    {
        var parse = new Driver();
        parse.Objects.Query<MyLocation>().Where(c => c.gameId == gameId).Execute(callback);

    }
 
    static public void getLocations(string locationIds, Action<Response<ResultsResponse<MyLocation>>> callback)
    {
        var parse = new Driver();
        parse.Objects.Query<MyLocation>().Where(c => locationIds.Contains(c.Id)).Execute(callback);

    }
    static public void getRoles(string roleIds, Action<Response<ResultsResponse<Roles>>> callback)
    {
        var parse = new Driver();
        parse.Objects.Query<Roles>().Where(c => roleIds.Contains(c.Id)).Execute(callback);

    }
}

