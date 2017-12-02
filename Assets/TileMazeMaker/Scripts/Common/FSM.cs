using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMazeMaker
{
    //默认0是该状态的默认下一个状态
    public interface iFSMState
    {
        void EnterState();
        void ExecuteState( float delta_time );
        void ExitState();
    }

    public class FSMStateTransitions
    {
        public Dictionary<int, iFSMState> transition_table = new Dictionary<int, iFSMState>();

        public void AddTransition(iFSMState state, int event_code = 0)
        {
            transition_table[event_code] = state;
        }

        public iFSMState MakeTransition(int event_code = 0)
        {
            iFSMState next = null;
            transition_table.TryGetValue(event_code, out next);
            return next;           
        }
    }

    public class FSM
    {
        bool is_started = false;
        public void StartFSM(iFSMState first_state) 
        {   
            active_state = first_state;
            active_state.EnterState();
            ResumeFSM();
        }

        public void PauseFSM() 
        {
            is_started = false;
        }

        public void ResumeFSM() 
        {
            is_started = true;
        }

        public Dictionary<iFSMState, FSMStateTransitions> transition_index = new Dictionary<iFSMState, FSMStateTransitions>();

        //active state = first state
        public iFSMState active_state;

        public void AddTransition(iFSMState from, iFSMState to, int event_code = 0) 
        {
            if (transition_index.ContainsKey(from) == false) 
            {
                transition_index[from] = new FSMStateTransitions();
            }
            transition_index[from].AddTransition(to, event_code);
        }

        public bool MakeTransition( int event_code = 0 ) 
        {
            iFSMState next = transition_index[active_state].MakeTransition(event_code);
            if (next == null) 
            {
                Debug.Log("Can not make transition from " + active_state + " for event " + event_code);
                return false;
            }

            active_state.ExitState();
            active_state = next;
            active_state.EnterState();
            return true;
        }

        public void UpdateFSM(float delta_time) 
        {
            if ( is_started && active_state != null ) 
            {
                active_state.ExecuteState(delta_time);
            }
        }
    }
}
