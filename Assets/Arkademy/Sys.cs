using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Arkademy
{
    public class Sys
    {
        #region State Save Load

        private static string SaveFilePath => Path.Combine(Application.persistentDataPath, "save");

        public static State CurrState
        {
            get
            {
                if (_currState == null && HasSave()) Load();
                return _currState;
            }
        }

        private static State _currState;

        [Serializable]
        public class State
        {
            public int weeks;
            public int days;
            public int hours;

            public Dictionary<string, int> StageLevel = new Dictionary<string, int>();
            public string stage;
            public int level;
            public int prevLevel;

            public void AddHours(int hour)
            {
                hours += hour;
            }

            public void NextDay()
            {
                days += 1;
                if (days == 7)
                {
                    weeks += 1;
                }

                days %= 7;
                hours = 0;
            }
        }

        public static void Save()
        {
            File.WriteAllText(SaveFilePath, JsonConvert.SerializeObject(_currState));
        }

        public static void Load()
        {
            _currState = JsonConvert.DeserializeObject<State>(File.ReadAllText(SaveFilePath));
        }

        public static bool HasSave()
        {
            return File.Exists(SaveFilePath);
        }

        public static void DeleteSave()
        {
            File.Delete(SaveFilePath);
        }

        public static void NewGame()
        {
            _currState = new State();
            Save();
            SceneManager.LoadScene("Scenes/Campus");
        }

        public static void Continue()
        {
            Load();
            SceneManager.LoadScene("Scenes/Campus");
        }

        #endregion

        public static Action<bool> OnPause = (pause) =>
        {
            if (pause)
            {
                SceneManager.activeSceneChanged += OnSceneChanged;
            }
            else
            {
                SceneManager.activeSceneChanged -= OnSceneChanged;
            }
        };

        private static readonly UnityAction<Scene, Scene> OnSceneChanged = (pre, next) => { Pause(false); };
        public static bool Paused;

        public static void Pause(bool pause)
        {
            Paused = pause;
            Time.timeScale = pause ? 0 : 1;
            OnPause?.Invoke(pause);
        }


        #region Campus

        [Serializable]
        public class CampusAction
        {
            public string displayName;
            public Action OnPerform;
            public Func<bool> CanPerform = () => true;
        }


        public static readonly Dictionary<string, CampusAction> CampusActions = new Dictionary<string, CampusAction>
        {
            {
                "sleep", new CampusAction
                {
                    displayName = "Sleep",
                    OnPerform = () =>
                    {
                        Sys.CurrState.NextDay();
                        Sys.Save();
                    },
                }
            },
            {
                "rest", new CampusAction
                {
                    displayName = "Rest",
                    OnPerform = () => { Sys.CurrState.AddHours(1); },
                    CanPerform = () => Sys.CurrState.hours <= 15
                }
            },
            {
                "stage", new CampusAction
                {
                    displayName = "Stage",
                    OnPerform = () => { GoToStage("forest", 0); },
                    CanPerform = () => Sys.CurrState.hours <= 14
                }
            }
        };

        #endregion

        #region Stage

        [Serializable]
        public class Stage
        {
            public string name;
            public string displayName;
        }

        [Serializable]
        public class StageData
        {
            public string stageName;
            public int level;
            public Vector2Int size;
            public Vector2Int enter;
            public Vector2Int exit;
            public int[,] mapData;

            private static string GetSavePath(string stageName, int level)
            {
                return Path.Combine(Application.persistentDataPath, $"{stageName}_{level}.stage");
            }


            public void SaveStage()
            {
                JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
                    {ReferenceLoopHandling = ReferenceLoopHandling.Ignore};
                File.WriteAllText(GetSavePath(stageName, level), JsonConvert.SerializeObject(this));
            }

            public static StageData LoadStage(string stageName, int level)
            {
                var path = GetSavePath(stageName, level);
                if (File.Exists(path))
                {
                    return JsonConvert.DeserializeObject<StageData>(File.ReadAllText(path));
                }

                return null;
            }
        }

        public static void GoToStage(string stage, int level)
        {
            CurrState.prevLevel = stage == CurrState.stage ? CurrState.level : level;
            CurrState.StageLevel[stage] = Mathf.Max(level,CurrState.StageLevel[stage]);
            CurrState.stage = stage;
            CurrState.level = level;
            
            Save();
            if (SceneManager.GetActiveScene().name != "Stage")
            {
                SceneManager.LoadScene("Scenes/Stage");
                return;
            }
            StageManager.Curr.LoadStage();
            StageManager.Curr.BuildStage();
        }

        public static readonly Dictionary<string, Stage> Stages = new Dictionary<string, Stage>
        {
            {
                "forest", new Stage()
                {
                    name = "forest",
                    displayName = "Forest"
                }
            },
            {
                "desert", new Stage()
                {
                    name = "desert",
                    displayName = "Desert"
                }
            },
            {
                "snow_mountain", new Stage()
                {
                    name = "snow_mountain",
                    displayName = "Snow Mountain"
                }
            },
        };

        #endregion
    }
}