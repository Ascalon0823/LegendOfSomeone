using System;
using CGS.Grid;
using UnityEngine;
using UnityEngine.Tilemaps;

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
        public Sys.StageData CurrStageData => currStageData;
        public SquareGrid2D<Vector2Int> CurrGrid => currBuilder.Grid; 
        [SerializeField] private Sys.StageData currStageData;

        [SerializeField] private StageBuilder builderPrefab;
        [SerializeField] private StageBuilder currBuilder;

        private void Start()
        {
            LoadStage();
            BuildStage();
        }

        public void LoadStage()
        {
            currStageData = Sys.StageData.LoadStage(Sys.CurrState.stage, Sys.CurrState.level);
            if (currStageData != null) return;
            currStageData = new Sys.StageData
            {
                stageName = Sys.CurrState.stage,
                level = Sys.CurrState.level
            };
            currStageData.SaveStage();
        }

        public void BuildStage()
        {
            if (currBuilder) Destroy(currBuilder.gameObject);
            currBuilder = Instantiate(builderPrefab);
            currBuilder.Build(CurrStageData);
        }
    }
}