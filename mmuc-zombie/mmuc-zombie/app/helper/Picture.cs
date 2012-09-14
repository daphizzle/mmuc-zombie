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
using mmuc_zombie.app.model;
using Parse;
using System.Diagnostics;

namespace mmuc_zombie.app.helper
{   
    public class Picture
    {
        public User user;
        public ParseFile avatar { get; set; }
        public byte[] avatarBytes { get; set; }

        public Picture(User user)
        {
            this.user = user;
        }

        public ParseFile savePicture()
        {
            if (this.avatarBytes != null)
            {
                var parse = new Driver();
                string newname = this.user.Id + ".png";
                parse.Files.Save(newname, this.avatarBytes, "image/png", r =>
                {
                    if (r.Success)
                    {
                        var url = r.Data.Url;
                        var name = r.Data.Name;
                        user.picture = url;
                    }
                    else
                    {
                        Debug.WriteLine(r.Error.Message);
                    }
                });
                return new ParseFile(newname);
            }
            return null;
        }


    }
}
