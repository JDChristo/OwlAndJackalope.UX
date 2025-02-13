﻿using System.Collections.Generic;
using System.Linq;
using OwlAndJackalope.UX.Runtime.Data;
using OwlAndJackalope.UX.Runtime.States;
using OwlAndJackalope.UX.Runtime.States.Serialized;
using UnityEngine;

namespace OwlAndJackalope.UX.Runtime.Modules
{
    /// <summary>
    /// Maintains the relationship between the reference's details and the various defined states.
    /// The states can be used like a state machine to setup different UX experiences. State relationships
    /// cannot be modified by external providers.
    /// </summary>
    [System.Serializable, RequireComponent(typeof(ReferenceModule))]
    public sealed class StateModule : MonoBehaviour, IReferenceDependentModule, IStateNameChangeHandler, IDetailNameChangeHandler
    {
        [SerializeField]
        private List<BaseSerializedState> _states;
        private readonly Dictionary<string, IState> _runtimeStates = new Dictionary<string, IState>();
        
        public void Initialize(IReference reference)
        {
            _runtimeStates.Clear();
            foreach (var state in _states)
            {
                var runtimeState = state.ConvertToState(reference);
                if (_runtimeStates.ContainsKey(runtimeState.Name))
                {
                    Debug.LogWarning($"State Name: {runtimeState.Name} is already in use.");
                }
                _runtimeStates[runtimeState.Name] = runtimeState;
            }
        }

        public IState GetState(string name)
        {
            if (name != null && _runtimeStates.TryGetValue(name, out var state))
            {
                return state;
            }

            return null;
        }

        public IEnumerable<string> GetStateNames()
        {
            return _states.Select(x => x.Name);
        }

        public void HandleStateNameChange(string previousName, string newName, IStateNameChangeHandler root)
        {
            if (root == null)
            {
                foreach (var handler in GetComponentsInChildren<IStateNameChangeHandler>())
                {
                    if (!ReferenceEquals(handler, this))
                    {
                        handler.HandleStateNameChange(previousName, newName, this);
                    }
                }
            }
        }

        public void HandleDetailNameChange(string previousName, string newName, IDetailNameChangeHandler root)
        {
            if (ReferenceEquals(GetComponentInParent<ReferenceModule>(), root))
            {
                foreach (var state in _states)
                {
                    state.HandleDetailNameChange(previousName, newName, this);
                }
            }

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}