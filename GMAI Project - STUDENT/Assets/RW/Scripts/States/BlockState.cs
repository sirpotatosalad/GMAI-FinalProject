using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class BlockState : State
    {
        public BlockState(Character character, StateMachine stateMachine) : base(character, stateMachine) { }
    }

}
