using System;
using UnityEngine;

namespace Arkademy
{
    public class StageManager : MonoBehaviour
    {
        // public static StageManager Curr;
        // private void Awake()
        // {
        //     if (Curr != null && Curr != this)
        //     {
        //         Destroy(gameObject);
        //         return;
        //     }
        //
        //     Curr = this;
        // }
        public string CurrStage => currStage;
        public int CurrLevel => currLevel;
        [SerializeField] private string currStage;
        [SerializeField] private int currLevel;
        private void Start()
        {
            BuildStage();
        }

        public void BuildStage()
        {
            currStage = Sys.CurrState.stage;
            currLevel = Sys.CurrState.level;
        }
    }
}
