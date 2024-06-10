using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class SwingState : State
    {
        // Start is called before the first frame update
        public SwingState(Character character, StateMachine stateMachine) : base(character, stateMachine) { }
    }
}
