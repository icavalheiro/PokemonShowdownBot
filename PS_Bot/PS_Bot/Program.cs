using System;
using Bridge;
using Bridge.Html5;

namespace PS_Bot
{
    public class Program
    {
        #region static data
        public static int TIME_BETWEEN_FRAMES_MILLISECONDS = 10000;
        public static string BOT_ID_TAG = "PS Bot v.0.1 by Ivan SC";
        #endregion

        public void Run()
        {
            //creates an instance of the bot manager
            Manager __instance = new Manager();
            __instance.Awake();//initialize componants
            __instance.Start();//initiale references to other classes, etc...

            Window.SetInterval(() =>//update funciton (simulate a frame rate)
            {
                __instance.Update();
            }, TIME_BETWEEN_FRAMES_MILLISECONDS);
        }
    }
}
