using System;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

namespace Arkademy
{
    public class Sys
    {
        private static State _currState;

        [Serializable]
        public class State
        {
        }

        public static Action<bool> OnPause;
        private static string SaveFilePath => Path.Combine(Application.persistentDataPath, "save");

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

        public static void Pause(bool pause)
        {
            Time.timeScale = pause ? 0 : 1;
            OnPause?.Invoke(pause);
        }
    }
}