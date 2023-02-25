using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lean.Touch;
using UnityEngine;

namespace Arkademy
{
    public class Player : MonoBehaviour
    {
        public static Player LocalPlayer;
        public Actor playerActorPrefab;
        public Actor currActor;

        public PlayerTouchInput playerTouchInput;

        private void Awake()
        {
            if (LocalPlayer != null)
            {
                Destroy(this);
                return;
            }

            LocalPlayer = this;
            Application.targetFrameRate = 60;
            StageManager.OnBuildComplete += OnStageBuildCompleted;
        }

        private void OnDestroy()
        {
            StageManager.OnBuildComplete -= OnStageBuildCompleted;
        }

        private void OnStageBuildCompleted()
        {
            if (currActor == null)
            {
                currActor = Instantiate(playerActorPrefab);
            }

            currActor.transform.position =
                (Sys.CurrState.prevLevel > StageManager.Curr.StageData.level
                    ? StageManager.Curr.Builder.Exit
                    : StageManager.Curr.Builder.Enter).transform.position;
        }

        private void Start()
        {
            LeanTouch.Instance.SwipeThreshold = Screen.width / 8;
        }

        private void Update()
        {
            if (Sys.Paused)
            {
                return;
            }

            playerTouchInput.UpdateInput();
            HandleMove();
            HandleStageInteract();
        }


        private void HandleMove()
        {
            if (!currActor) return;
            if (playerTouchInput.tap || playerTouchInput.up)
            {
                currActor.wantToMove = Vector2.zero;
                return;
            }

            if (playerTouchInput.drag)
            {
                currActor.wantToMove = playerTouchInput.dragDistance * 4 / Screen.width;
                currActor.wantToMove = Vector2.ClampMagnitude(currActor.wantToMove, 1f);
            }
        }

        private void HandleStageInteract()
        {
            if (!currActor) return;
            if (!StageManager.Curr) return;
            var coord = StageManager.Curr.Grid?.FromPos(currActor.transform.position);

            if (playerTouchInput.tap && coord.HasValue)
            {
                if (coord.Value == StageManager.Curr.StageData.enter && !StageManager.Curr.FirstStage())
                {
                    StageManager.Curr.GoToPreviousStage();
                    return;
                }

                if (coord.Value == StageManager.Curr.StageData.exit)
                {
                    StageManager.Curr.GoToNextStage();
                }
            }
        }
    }
}