using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

namespace Arkademy
{
    public class Sys
    {
        #region State Save Load

        private static string SaveFilePath => Path.Combine(Application.persistentDataPath, "save");
        public static State CurrState => _currState;
        private static State _currState;

        [Serializable]
        public class State
        {
            public int weeks;
            public int days;
            public int hours;

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

        public static Action<bool> OnPause;

        public static void Pause(bool pause)
        {
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
                    OnPerform = () => { SceneManager.LoadScene("Scenes/Stage"); },
                    CanPerform = () => Sys.CurrState.hours <= 14
                }
            }
        };

        #endregion
    }
}