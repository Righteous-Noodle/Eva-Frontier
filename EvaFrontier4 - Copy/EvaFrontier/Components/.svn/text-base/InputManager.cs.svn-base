using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvaFrontier.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EvaFrontier.Controllers
{
    public class InputManager {

        public InputManager(InputType type, PlayerIndex player)
        {
            this.inputType = type;
            this.playerIndex = player;

            //Fill dictionaries, TODO: bring this out into an xml file
            inputToKeys = new Dictionary<Inputs, Keys>(10);
            inputToKeys.Add(Inputs.A, Keys.A);
            inputToKeys.Add(Inputs.B, Keys.S);
            inputToKeys.Add(Inputs.Back, Keys.Escape);
            inputToKeys.Add(Inputs.Down, Keys.Down);
            inputToKeys.Add(Inputs.Left, Keys.Left);
            inputToKeys.Add(Inputs.Right, Keys.Right);
            inputToKeys.Add(Inputs.Start, Keys.Space);
            inputToKeys.Add(Inputs.Up, Keys.Up);
            inputToKeys.Add(Inputs.X, Keys.Z);
            inputToKeys.Add(Inputs.Y, Keys.X);
            inputToKeys.Add(Inputs.LeftTrigger, Keys.OemMinus);
            inputToKeys.Add(Inputs.RightTrigger, Keys.OemPlus);

            inputToButtons = new Dictionary<Inputs, Buttons>(10);
            inputToButtons.Add(Inputs.A, Buttons.A);
            inputToButtons.Add(Inputs.B, Buttons.B);
            inputToButtons.Add(Inputs.Back, Buttons.Back);
            inputToButtons.Add(Inputs.Down, Buttons.DPadDown);
            inputToButtons.Add(Inputs.Left, Buttons.DPadLeft);
            inputToButtons.Add(Inputs.Right, Buttons.DPadRight);
            inputToButtons.Add(Inputs.Start, Buttons.Start);
            inputToButtons.Add(Inputs.Up, Buttons.DPadUp);
            inputToButtons.Add(Inputs.X, Buttons.X);
            inputToButtons.Add(Inputs.Y, Buttons.Y);
            inputToButtons.Add(Inputs.LeftTrigger, Buttons.LeftTrigger);
            inputToButtons.Add(Inputs.RightTrigger, Buttons.RightTrigger);
            //note that left, right, down and up are also mapped
            //to the left thumbstick
        }

        public bool IsInputPressed(Inputs input)
        {
            if (inputType == InputType.Keyboard)
            {
                return (curState.IsKeyDown(inputToKeys[input])
                    && !prevState.IsKeyDown(inputToKeys[input]));
            }
            else //inputType == InputType.GamePad
            {
                //Check both left thumbstick dpad and buttons
                return (StickDirectionDown(curPadState, input)
                    && !StickDirectionDown(prevPadState, input))

                    || (curPadState.IsButtonDown(inputToButtons[input])
                    && !prevPadState.IsButtonDown(inputToButtons[input]));
            }
        }

        public bool IsInputDown(Inputs input)
        {
            if (inputType == InputType.Keyboard)
            {
                return curState.IsKeyDown(inputToKeys[input]);
            }
            else //inputType == InputType.GamePad
            {
                return (StickDirectionDown(curPadState, input)

                || curPadState.IsButtonDown(inputToButtons[input]));
            }
        }

        public bool IsInputUp(Inputs input)
        {
            if (inputType == InputType.Keyboard)
            {
                return prevState.IsKeyUp(inputToKeys[input]);
            }
            else //inputType == InputType.GamePad
            {
                return (!StickDirectionDown(curPadState, input)

                    && prevPadState.IsButtonUp(inputToButtons[input]));
            }
        }

        public bool StickDirectionDown(GamePadState gamePadState, Inputs input)
        {
            if (input == Inputs.Left)
            {
                return (gamePadState.ThumbSticks.Left.X < -thumbStickDeadzone);
            }
            else if (input == Inputs.Right)
            {
                return (gamePadState.ThumbSticks.Left.X > thumbStickDeadzone);
            }
            else if (input == Inputs.Up)
            {
                return (gamePadState.ThumbSticks.Left.Y > thumbStickDeadzone);
            }
            else if (input == Inputs.Down)
            {
                return (gamePadState.ThumbSticks.Left.Y < -thumbStickDeadzone);
            }

            return false;
        }

        public bool InputIsDirection(Inputs input)
        {
            return (input == Inputs.Left || input == Inputs.Right ||
                input == Inputs.Up || input == Inputs.Down);
        }

        public void Update()
        {
            if (inputType == InputType.Keyboard)
            {
                prevState = curState;
                curState = Keyboard.GetState();
            }
            else if (inputType == InputType.GamePad)
            {
                prevPadState = curPadState;
                curPadState = GamePad.GetState(playerIndex);
            }

        }

        #region FieldsAndProperties
        public float thumbStickDeadzone = 0.5f;

        private Dictionary<Inputs, Keys> inputToKeys;
        private Dictionary<Inputs, Buttons> inputToButtons;

        private KeyboardState curState, prevState;
        private GamePadState curPadState, prevPadState;
        private InputType inputType;
        private PlayerIndex playerIndex;
        #endregion
    }

    public enum Inputs
    {
        A, B, X, Y, Left, Right, Up, Down, Start, Back, LeftTrigger, RightTrigger
    }

    public enum InputType
    {
        Keyboard, GamePad, Touch
    }
}
