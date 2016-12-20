using System;
using Bridge;
using Bridge.Html5;

namespace PS_Bot
{
    public class Main
    {
        public void OnInjected()
        {
            //once we are injected into the PS webpage we run ours program ^.- 
            Program __newProgram = new Program();
            __newProgram.Run();
        }
    }
}
