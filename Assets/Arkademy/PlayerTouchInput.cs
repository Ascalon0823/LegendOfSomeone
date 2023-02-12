using UnityEngine;
using Lean.Touch;
namespace Arkademy
{
    public class PlayerTouchInput : MonoBehaviour
    {
        public enum InputState
        {
            None,
            Tap,
            Swipe,
            Hold,
            HoldUp,
            HoldSwipe,
            HoldDrag,
            HoldDragUp,
            HoldDragSwipe,
            Drag,
            DragSwipe,
            DragUp
        }
        private bool fingerInUse;
        private bool fingerDisposed;
        public bool tap;
        public bool swipe;
        public bool drag;
        public bool hold;
        public bool up;
        public Vector2 dragDistance;
        public Vector2 swipeDistance;
        public float holdTresh;
        public float holdTime;
        public bool postSwipeDrag;
        public float dragBeginAge;
        public InputState CurrInputState
        {
            get
            {
                if (tap) return InputState.Tap;
                if (swipe)
                {
                    if (!postSwipeDrag && !hold) return InputState.Swipe;
                    if (!postSwipeDrag) return InputState.HoldSwipe;
                    if (!hold) return InputState.DragSwipe;
                    return InputState.HoldDragSwipe;
                }

                if (up)
                {
                    if (!postSwipeDrag && !hold) return InputState.None;
                    if (!postSwipeDrag) return InputState.HoldUp;
                    if (!hold) return InputState.DragUp;
                    return InputState.HoldDragUp;
                }

                if (postSwipeDrag && hold) return InputState.HoldDrag;
                if (postSwipeDrag) return InputState.Drag;
                if (hold) return InputState.Hold;
                return InputState.None;
            }
        }
        

        private void Start()
        {
            LeanTouch.Instance.SwipeThreshold = Screen.width / 8;
        }

        // private void Update()
        // {
        //     if (ApplicationManager.Paused)
        //     {
        //         return;
        //     }
        //
        //     HandleInput();
        //     HandleMove();
        //     HandleInteractionCandidate();
        // }

        private LeanFinger GetFinger()
        {
            var fingers = LeanTouch.GetFingers(true, false, 1);
            if (fingers == null || fingers.Count == 0) return null;
            return fingers[0];
        }

        public void UpdateInput()
        {
            var finger = GetFinger();
            if (finger == null)
            {
                tap = false;
                up = false;
                hold = false;
                drag = false;
                swipe = false;
                postSwipeDrag = false;
                dragBeginAge = 0;
                return;
            }

            if (fingerDisposed)
            {
                if (finger.Up)
                {
                    fingerDisposed = false;
                }

                return;
            }

            tap = finger.Tap;
            up = finger.Up;
            swipe = finger.Swipe || (up && finger.GetSnapshotScreenDelta(LeanTouch.CurrentTapThreshold).magnitude >=
                LeanTouch.CurrentSwipeThreshold);
            var wasDrag = drag;
            drag = finger.GetScreenDistance(finger.StartScreenPosition) >
                LeanTouch.CurrentSwipeThreshold || drag;
            if (!wasDrag && drag)
            {
                dragBeginAge = finger.Age;
            }

            postSwipeDrag = drag && (finger.Age - dragBeginAge) > LeanTouch.Instance.TapThreshold;
            hold = finger.Age > holdTresh && finger.GetScreenDistance(finger.StartScreenPosition) <
                LeanTouch.CurrentSwipeThreshold && !drag || hold;


            if (swipe)
            {
                swipeDistance = finger.GetSnapshotScreenDelta(LeanTouch.CurrentTapThreshold);
            }

            if (drag)
            {
                dragDistance = finger.ScreenPosition - finger.StartScreenPosition;
            }

            if (hold)
            {
                holdTime = finger.Age - LeanTouch.CurrentTapThreshold;
            }

            if (CurrInputState != InputState.None)
                Debug.Log(CurrInputState);
        }

        public void DisposeAllInput()
        {
            fingerDisposed = true;
        }
    }
}
