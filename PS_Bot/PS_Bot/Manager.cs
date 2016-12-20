using System;
using Bridge;
using Bridge.Html5;

namespace PS_Bot
{
    public class Manager
    {
        #region private data
        private GameStateAnalyser _gameAnalyser;
        private Bot _bot;
        #endregion

        public void Awake()
        {
            //initialize class members
            _gameAnalyser = new GameStateAnalyser();
            _bot = new Bot();
        }

        public void Start()
        {
            //tell bot to start it's components
            _bot.Start();
        }

        public void Update()
        {
            //get a "picture" of the current game scene and updates the bot with it
            var __report = _gameAnalyser.Analyse();
            _bot.Update(__report);
        }
    }
}
