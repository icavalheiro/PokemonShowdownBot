using System;
using Bridge;
using Bridge.Html5;

namespace PS_Bot
{
    //our plugin will inject the bridge.js and PS_Bot.js into the PS web page
    class Injector
    {
        //static void main is called by bridge once injected
        public static void Main()
        {
            Main __newMain = new PS_Bot.Main();
            __newMain.OnInjected();
        }
    }
}
