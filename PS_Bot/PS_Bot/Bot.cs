using System;
using Bridge;
using Bridge.Html5;


namespace PS_Bot
{
    public class Bot
    {
        #region private data
        private Interface _interface;
        private string _lastEnemyToMeSwitch = "";
        #endregion

        public void Start()
        {
            _interface = new Interface();
            _interface.Start();
            _interface.Log("succesfully_injected");
        }

        public void Update(GameStateAnalyser.Report p_gameStateReport)
        {
            //check if timer btn is availavle and off, and turn it on (foce enemy to move within 150sec)
            if(p_gameStateReport.isTimerBtnAvailable)
            {
                p_gameStateReport.timerBtn.Click();
            }

            //AI logic
            if(p_gameStateReport.isInBattle)
            {
                Action __doSwitchPokemon = () =>
                {
                    _interface.Log("choosing_new_pokemon");
                    var __availableTeamMembers = p_gameStateReport.availableTeamMembersBtns;

                    if ((new Random().Next(100) > 20))
                    {
                        GameStateAnalyser.PokemonTypeEffectivenessType __currentType = GameStateAnalyser.PokemonTypeEffectivenessType.NO_EFFECT;
                        var __currentBtn = __availableTeamMembers[0];
                        var __enemyPokemon = p_gameStateReport.currentEnemyPokemon;

                        foreach (var teamMemberBtn in __availableTeamMembers)
                        {
                            var __pokemon = teamMemberBtn.pokemonData;
                            var __result = GameStateAnalyser.SimulateCombatResult(__pokemon, __enemyPokemon);
                            if (__result == GameStateAnalyser.PokemonTypeEffectivenessType.NO_EFFECT)
                                continue;

                            if (__result == GameStateAnalyser.PokemonTypeEffectivenessType.NOT_VERY_EFFECTIVE)
                            {
                                if (__currentType == GameStateAnalyser.PokemonTypeEffectivenessType.NO_EFFECT)
                                {
                                    __currentType = __result;
                                    __currentBtn = teamMemberBtn;
                                }
                                continue;
                            }

                            if (__result == GameStateAnalyser.PokemonTypeEffectivenessType.NORMAL_EFFECTIVENESS)
                            {
                                if (__currentType == GameStateAnalyser.PokemonTypeEffectivenessType.NOT_VERY_EFFECTIVE
                                || __currentType == GameStateAnalyser.PokemonTypeEffectivenessType.NO_EFFECT)
                                {
                                    __currentType = __result;
                                    __currentBtn = teamMemberBtn;
                                }
                                continue;
                            }

                            if (__result == GameStateAnalyser.PokemonTypeEffectivenessType.SUPER_EFFECTIVE)
                            {
                                __currentType = __result;
                                __currentBtn = teamMemberBtn;
                            }
                        }

                        __currentBtn.Click();
                    }
                    else
                    {
                        __availableTeamMembers[(new Random().Next(__availableTeamMembers.Count))].Click();
                    }
                    _interface.Log("new_pokemon_choosed");
                };

                if (p_gameStateReport.isGameOver)
                {
                    _interface.Log("end_of_game_detected");
                    p_gameStateReport.closeGameBtn.Click();
                    _interface.Log("game_window_closed");                
                }
                else if (p_gameStateReport.isSkillsMenuAvailable)
                {
                    _interface.Log("choosing_skill");
                    var __availableSkills = p_gameStateReport.availableSkillsBtns;

                    bool __hasSuperEffectiveAttack = false;
                    GameStateAnalyser.SkillButton __superEffectAttack = null;
                    int __superEffectAttackBasePower = 0;
                    foreach(var skillBtn in __availableSkills)
                    {
                        if(GameStateAnalyser.SimulateCombatResult(
                            skillBtn.pokemonType, 
                            p_gameStateReport.currentEnemyPokemon.pokemonTypeMain,
                            p_gameStateReport.currentEnemyPokemon.pokemonTypeSub) == GameStateAnalyser.PokemonTypeEffectivenessType.SUPER_EFFECTIVE)
                        {
                            __hasSuperEffectiveAttack = true;
                            if(__superEffectAttackBasePower < skillBtn.basePower)
                            {
                                __superEffectAttack = skillBtn;
                                __superEffectAttackBasePower = skillBtn.basePower;
                            }
                        }
                    }

                    if (__hasSuperEffectiveAttack && __superEffectAttack != null)
                    {
                        _interface.Log("choosing_super_effective_skill");
                        __superEffectAttack.Click();
                    }
                    else
                    {
                        Action __doChooseRandomSkill = () =>
                        {
                            _interface.Log("choosing_random_skill");
                            var __randomBtn = __availableSkills[(new System.Random()).Next(__availableSkills.Count)];
                            __randomBtn.Click();
                        };

                        Action __doChooseStrongesAttack = () =>
                        {
                            _interface.Log("enemy_has_low_helth");
                            _interface.Log("choosing_strongest_attack");

                            int __skillsPower = 0;
                            var __skill = __availableSkills[0];

                            foreach (var skill in __availableSkills)
                            {
                                if (skill.basePower > __skillsPower)
                                {
                                    __skillsPower = skill.basePower;
                                    __skill = skill;
                                }
                            }
                            __skill.Click();
                        };

                        Action __doUseSkill = () =>
                        {
                            if (p_gameStateReport.currentMyPokemonHP <= 50 || p_gameStateReport.currentEnemyPokemonHP <= 20)
                                __doChooseStrongesAttack();
                            else
                                __doChooseRandomSkill();
                        };

                        if (p_gameStateReport.isTeamMenuAvailable && p_gameStateReport.currentEnemyPokemonHP > 20)
                        {
                            var __enemyPokemon = p_gameStateReport.currentEnemyPokemon;
                            var __myPokemon = p_gameStateReport.currentMyPokemon;

                            var __defenseSimulationResult = GameStateAnalyser.SimulateCombatResult(__enemyPokemon, __myPokemon);

                            bool __isEnemySuperEffectiveAgainstMe = __defenseSimulationResult == GameStateAnalyser.PokemonTypeEffectivenessType.SUPER_EFFECTIVE;
                            bool __isMeSuperEffectiveAgainstEnemy = __defenseSimulationResult == GameStateAnalyser.PokemonTypeEffectivenessType.NO_EFFECT;

                            if (__isMeSuperEffectiveAgainstEnemy || __isEnemySuperEffectiveAgainstMe == false || (_lastEnemyToMeSwitch == __enemyPokemon.pokemonName))
                                __doUseSkill();
                            else
                            {
                                _lastEnemyToMeSwitch = __enemyPokemon.pokemonName;
                                __doSwitchPokemon();
                            }
                        }
                        else
                            __doUseSkill();
                    }
                }
                else if (p_gameStateReport.isTeamMenuAvailable)
                {
                    if(p_gameStateReport.currentEnemyPokemon != null)
                        _lastEnemyToMeSwitch = p_gameStateReport.currentEnemyPokemon.pokemonName;
                    __doSwitchPokemon();
                }
            }
            else
            {
                if (p_gameStateReport.isBeingChallenged)
                {
                    _interface.Log("accepting_challenge");
                    p_gameStateReport.acceptChallengeBtn.Click();
                }                    
            }
        }
    }

    #region Interface
    public class Interface
    {
        private HTMLElement _logWindow;
        private HTMLElement _logDiv;
        private int _messageCounter = 0;

        public void Log(string p_message)
        {
            var __p = Document.CreateElement("p");
            _messageCounter++;
            __p.InnerHTML = p_message + "." + _messageCounter.ToString("000");
            if (_logDiv.FirstChild != null)
                _logDiv.InsertBefore(__p, _logDiv.FirstChild);
            else
                _logDiv.AppendChild(__p);
        }

        public void Start()
        {
            _logWindow = CreateLogWindow();
            _logDiv = CreateLogDiv(_logWindow);
        }

        //invisble div inside the black window that servers to hold the log elements
        private static HTMLElement CreateLogDiv(HTMLElement p_logWindow)
        {
            var __logDiv = Document.CreateElement("div");
            p_logWindow.AppendChild(__logDiv);

            __logDiv.Style.CssFloat = Float.Left;
            __logDiv.Style.Width = "calc(100% - 10px)";
            __logDiv.Style.Height = "calc(100% - 8px)";
            __logDiv.Style.Overflow = Overflow.Hidden;

            return __logDiv;
        }


        //black window that appears in the left of the screen
        private static HTMLElement CreateLogWindow()
        {
            var __logWindow = Document.CreateElement("div");
            Document.Body.AppendChild(__logWindow);

            __logWindow.Style.Position = Position.Fixed;
            __logWindow.Style.ZIndex = "100000";
            __logWindow.Style.BackgroundColor = "#000";
            __logWindow.Style.Color = "#0f0";
            __logWindow.Style.BorderRadius = "7px 0 0 7px";
            __logWindow.Style.Height = "300px";
            __logWindow.Style.Width = "200px";
            __logWindow.Style.Top = "calc(100vh - 330px)";
            __logWindow.Style.Right = "0";
            __logWindow.Style.BoxShadow = "5px 3px 5px #777";
            __logWindow.Style.FontFamily = "Arial";
            __logWindow.Style.FontSize = "9px";
            __logWindow.Style.TextAlign = TextAlign.Right;
            __logWindow.Style.PaddingTop = "10px";
            __logWindow.Style.PaddingBottom = "10px";
            __logWindow.Style.PaddingLeft = "10px";


            var __logWindowHeader = Document.CreateElement("div");
            __logWindow.AppendChild(__logWindowHeader);

            __logWindowHeader.Style.Width = "100%";
            __logWindowHeader.Style.Position = Position.Relative;
            __logWindowHeader.Style.BorderBottom = "solid 1px #333";
            __logWindowHeader.Style.CssFloat = Float.Left;
            __logWindowHeader.InnerHTML = Program.BOT_ID_TAG;
            __logWindowHeader.Style.TextAlign = TextAlign.Center;
            __logWindowHeader.Style.PaddingBottom = "5px";

            return __logWindow;
        }
    }

    #endregion
}
