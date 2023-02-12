using System;
using CGS.Grid;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Arkademy
{
    public class StageManager : MonoBehaviour
    {
        public static StageManager Curr;

        private void Awake()
        {
            if (Curr != null && Curr != this)
            {
                Destroy(gameObject);
                return;
            }

            Curr = this;
        }

        public static Action OnBuildComplete;
        public Sys.StageData StageData => currStageData;
        public SquareGrid2D<int> Grid => currBuilder.Grid;
        [SerializeField] private Sys.StageData currStageData;
        public StageBuilder Builder => currBuilder;
        [SerializeField] private StageBuilder currBuilder;
        [SerializeField] private StageBuilder builderPrefab;


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
            currBuilder.Build(StageData);
            OnBuildComplete?.Invoke();
        }
    }
}