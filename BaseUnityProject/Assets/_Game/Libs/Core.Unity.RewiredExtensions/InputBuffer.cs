using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

namespace Core.Unity.RewiredExtensions {

    public class InputBuffer {

        public InputBuffer(Rewired.Player player, int bufferCapacity) {
            _bufferCapacity = bufferCapacity;
            _pressedBuffer = new List<ButtonEvent>(bufferCapacity);
            _releasedBuffer = new List<ButtonEvent>(bufferCapacity);
            _heldSet = new HashSet<Button>();

            player.AddInputEventDelegate(this.OnButtonJustPressed, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed);
            player.AddInputEventDelegate(this.OnNegativeButtonJustPressed, UpdateLoopType.Update, InputActionEventType.NegativeButtonJustPressed);
            player.AddInputEventDelegate(this.OnButtonJustReleased, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased);
            player.AddInputEventDelegate(this.OnNegativeButtonJustReleased, UpdateLoopType.Update, InputActionEventType.NegativeButtonJustReleased);
        }

        /// <summary>
        /// Gets if the given action was pressed this frame. (positive direction)
        /// </summary>
        /// <param name="actionId">Id of the action.</param>
        public bool GetPressed(int actionId) {
            return this.GetPressed(actionId, true, 0);
        }

        /// <summary>
        /// Gets if the given action was pressed this frame.
        /// </summary>
        /// <param name="actionId">Id of the action.</param>
        /// <param name="positive">Direction of the action</param>
        public bool GetPressed(int actionId, bool positive) {
            return this.GetPressed(actionId, positive, 0);
        }

        /// <summary>
        /// Gets if the given action was pressed within the specified duration. (positive direction)
        /// </summary>
        /// <param name="actionId">Id of the action.</param>
        /// <param name="duration">Time from when the action was pressed (uses unscaled time)</param>
        public bool GetPressed(int actionId, float duration) {
            return this.GetPressed(actionId, true, duration);
        }

        /// <summary>
        /// Gets if the given action was pressed within the specified duration.
        /// </summary>
        /// <param name="actionId">Id of the action.</param>
        /// <param name="positive">Direction of the action</param>
        /// <param name="duration">Time from when the action was pressed (uses unscaled time)</param>
        public bool GetPressed(int actionId, bool positive, float duration) {
            float timestampMin = Time.unscaledTime - duration - Mathf.Epsilon;
            for (int i = _pressedBuffer.Count - 1; i >= 0; i--) {
                ButtonEvent buttonEvent = _pressedBuffer[i];
                if (buttonEvent.Timestamp < timestampMin)
                    return false;

                if (buttonEvent.ActionId == actionId && buttonEvent.Positive == positive)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets if the given action was pressed within this fixed update time duration. (positive direction)
        /// </summary>
        /// <param name="actionId">Id of the action.</param>
        /// <param name="positive">Direction of the action</param>
        public bool GetPressedFixedUpdate(int actionId) {
            return this.GetPressedFixedUpdate(actionId, true);
        }

        /// <summary>
        /// Gets if the given action was pressed within this fixed update time duration.
        /// </summary>
        /// <param name="actionId">Id of the action.</param>
        /// <param name="positive">Direction of the action</param>
        public bool GetPressedFixedUpdate(int actionId, bool positive) {
            return this.GetPressed(actionId, positive, Time.fixedUnscaledDeltaTime);
        }

        /// <summary>
        /// Gets if any action was pressed this frame (does not include input not bound to an action).
        /// </summary>
        /// <param name="actionId">Out param for the id of the action</param>
        /// <param name="positive">Out param for the direction of the action</param>
        public bool GetAnyActionPressed(out int actionId, out bool positive) {
            return this.GetAnyActionPressed(0, out actionId, out positive);
        }

        /// <summary>
        /// Gets if any action was pressed within the specified duration (does not include input not bound to an action).
        /// </summary>
        /// <param name="duration">Time from when the action was pressed (uses unscaled time)</param>
        /// <param name="actionId">Out param for the id of the action</param>
        /// <param name="positive">Out param for the direction of the action</param>
        public bool GetAnyActionPressed(float duration, out int actionId, out bool positive) {
            if (_pressedBuffer.Count > 0) {
                ButtonEvent buttonEvent = _pressedBuffer[_pressedBuffer.Count - 1];
                if (buttonEvent.Timestamp >= Time.unscaledTime - duration - Mathf.Epsilon) {
                    actionId = buttonEvent.ActionId;
                    positive = buttonEvent.Positive;
                    return true;
                }
            }

            actionId = 0;
            positive = false;
            return false;
        }

        /// <summary>
        /// Gets the timestamp (in unscaled time) when the given action was last pressed.
        /// Returns -1 if action wasn't pressed, or was pressed but dropped out of the buffer.
        /// </summary>
        /// <param name="actionId">Id of the action</param>
        /// <param name="positive">Direction of the action</param>
        public float GetUnscaledTimeLastPressed(int actionId, bool positive) {
            for (int i = _pressedBuffer.Count - 1; i >= 0; i--) {
                ButtonEvent buttonEvent = _pressedBuffer[i];
                if (buttonEvent.ActionId == actionId && buttonEvent.Positive == positive) {
                    return buttonEvent.Timestamp;
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets if the given action is currently held. (positive direction)
        /// </summary>
        /// <param name="actionId">Id of the action.</param>
        public bool GetHeld(int actionId) {
            return this.GetHeld(actionId, true);
        }

        /// <summary>
        /// Gets if the given action is currently held.
        /// </summary>
        /// <param name="actionId">Id of the action.</param>
        /// <param name="positive">Direction of the action</param>
        public bool GetHeld(int actionId, bool positive) {
            return _heldSet.Contains(new Button(actionId, positive));
        }

        /// <summary>
        /// Gets if the given action was released this frame. (positive direction)
        /// </summary>
        /// <param name="actionId">Id of the action.</param>
        /// <param name="positive">Direction of the action</param>
        public bool GetReleased(int actionId) {
            return this.GetReleased(actionId, true, 0);
        }

        /// <summary>
        /// Gets if the given action was released this frame.
        /// </summary>
        /// <param name="actionId">Id of the action.</param>
        /// <param name="positive">Direction of the action</param>
        public bool GetReleased(int actionId, bool positive) {
            return this.GetReleased(actionId, positive, 0);
        }

        /// <summary>
        /// Gets if the given action was released within the specified duration.
        /// </summary>
        /// <param name="actionId">Id of the action.</param>
        /// <param name="positive">Direction of the action</param>
        /// <param name="duration">Time from when the action was released (uses unscaled time)</param>
        public bool GetReleased(int actionId, bool positive, float duration) {
            float timestampMin = Time.unscaledTime - duration - Mathf.Epsilon;
            for (int i = _releasedBuffer.Count - 1; i >= 0; i--) {
                ButtonEvent buttonEvent = _releasedBuffer[i];
                if (buttonEvent.Timestamp < timestampMin)
                    return false;

                if (buttonEvent.ActionId == actionId && buttonEvent.Positive == positive)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets if the given action was released within this fixed update time duration.
        /// </summary>
        /// <param name="actionId">Id of the action.</param>
        /// <param name="positive">Direction of the action</param>
        public bool GetReleasedFixedUpdate(int actionId, bool positive) {
            return this.GetReleased(actionId, positive, Time.fixedUnscaledDeltaTime);
        }

        /// <summary>
        /// Manually adds a pressed event to the buffer at the current unscaled time and sets the action as held.
        /// </summary>
        /// <param name="actionId">Id of the action.</param>
        /// <param name="positive">Direction of the action</param>
        public void SimulatePressAndHold(int actionId, bool positive) {
            this.AddEventToBuffer(_pressedBuffer, new ButtonEvent(actionId, positive, Time.unscaledTime));
            _heldSet.Add(new Button(actionId, positive));
        }

        /// <summary>
        /// Manually adds a released event to the buffer at the current unscaled time and sets the action as not held.
        /// </summary>
        /// <param name="actionId">Id of the action.</param>
        /// <param name="positive">Direction of the action</param>
        public void SimulateRelease(int actionId, bool positive) {
            this.AddEventToBuffer(_releasedBuffer, new ButtonEvent(actionId, positive, Time.unscaledTime));
            _heldSet.Remove(new Button(actionId, positive));
        }

        #region Private

        private struct ButtonEvent {
            public ButtonEvent(int actionId, bool positive, float timestamp) {
                this.ActionId = actionId;
                this.Positive = positive;
                this.Timestamp = timestamp;
            }
            public readonly int ActionId;
            public readonly bool Positive;
            public readonly float Timestamp;
        }

        private struct Button {
            public Button(int actionId, bool positive) {
                this.ActionId = actionId;
                this.Positive = positive;
            }
            public readonly int ActionId;
            public readonly bool Positive;
        }

        /// <summary>
        /// Adds the given event to the buffer, preserving ascending timestamp order.
        /// </summary>
        private void AddEventToBuffer(List<ButtonEvent> buffer, ButtonEvent buttonEvent) {
            // remove earliest event if new event would exceed the buffer capacity
            if (buffer.Count >= _bufferCapacity) {
                if (buttonEvent.Timestamp <= buffer[0].Timestamp)
                    return;

                buffer.RemoveAt(0);
            }

            // most common case: event has the latest timestamp
            if (buffer.Count <= 0 || buttonEvent.Timestamp >= buffer[buffer.Count - 1].Timestamp) {
                buffer.Add(buttonEvent);
                return;
            }

            // insert event at place to preserve ascending timestamp order
            for (int i = 0; i < buffer.Count; i++) {
                if (buttonEvent.Timestamp < buffer[i].Timestamp) {
                    buffer.Insert(i, buttonEvent);
                    return;
                }
            }
        }

        /// <summary>
        /// Event invoked by rewired when a button is pressed in the positive direction.
        /// This includes axes that are treated as buttons.
        /// This is invoked for each action that is bound to the same input.
        /// </summary>
        private void OnButtonJustPressed(InputActionEventData data) {
            this.SimulatePressAndHold(data.actionId, true);
        }

        /// <summary>
        /// Event invoked by rewired when a button is pressed in the negative direction.
        /// This includes axes that are treated as buttons.
        /// This is invoked for each action that is bound to the same input.
        /// </summary>
        private void OnNegativeButtonJustPressed(InputActionEventData data) {
            this.SimulatePressAndHold(data.actionId, false);
        }

        /// <summary>
        /// Event invoked by rewired when a button is released in the positive direction.
        /// This includes axes that are treated as buttons.
        /// This is invoked for each action that is bound to the same input.
        /// </summary>
        private void OnButtonJustReleased(InputActionEventData data) {
            this.SimulateRelease(data.actionId, true);
        }

        /// <summary>
        /// Event invoked by rewired when a button is released in the negative direction.
        /// This includes axes that are treated as buttons.
        /// This is invoked for each action that is bound to the same input.
        /// </summary>
        private void OnNegativeButtonJustReleased(InputActionEventData data) {
            this.SimulateRelease(data.actionId, false);
        }

        private int _bufferCapacity;

        /// <summary>
        /// List of buttons pressed, in ascending timestamp order 
        /// </summary>
        private List<ButtonEvent> _pressedBuffer;
        /// <summary>
        /// List of buttons released, in ascending timestamp order 
        /// </summary>
        private List<ButtonEvent> _releasedBuffer;
        /// <summary>
        /// Set of buttons that are currently held
        /// </summary>
        private HashSet<Button> _heldSet;

        #endregion
    }
}