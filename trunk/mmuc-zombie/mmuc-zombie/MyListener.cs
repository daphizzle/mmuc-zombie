using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


    public interface MyListener
    {
        void onDataChange(List<MyParseObject> o);
        void onDataChange();
    }
