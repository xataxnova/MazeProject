using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventInjector : MonoBehaviour
{
    [System.Serializable]
    public class InjectEvent
    {
        public string TargetClip;
        public string FunctionName;
        public string stringParam;
        public int intParam;
        public float floatParam;
        [Space]
        [Range(0.0f, 1.0f)]
        public float InjectPosition;
        
        public UnityEngine.AnimationEvent GetAnimEvent(float total_length)
        {
            UnityEngine.AnimationEvent AnimEvent = new UnityEngine.AnimationEvent();
            AnimEvent.time = total_length * InjectPosition;
            AnimEvent.functionName = FunctionName;
            AnimEvent.intParameter = intParam;
            AnimEvent.stringParameter = stringParam;
            AnimEvent.floatParameter = floatParam;

            return AnimEvent;
        }
    }
    [SerializeField]
    InjectEvent[] inject_events;
    Dictionary<string, AnimationClip> clip_index = new Dictionary<string, AnimationClip>();

    public void Inject( Animator target_animator )
    {
        clip_index.Clear();
        
        if (target_animator != null)
        {
            RuntimeAnimatorController runtime_ctrl = target_animator.runtimeAnimatorController;
            AnimationClip[] clips = runtime_ctrl.animationClips;

            foreach (var clip in clips)
            {
                clip_index[clip.name] = clip;
            }
                        
            foreach (var injectE in inject_events)
            {
                AnimationClip clip = null;
                clip_index.TryGetValue(injectE.TargetClip, out clip);
                if (clip != null)
                {                    
                    clip.AddEvent(injectE.GetAnimEvent( clip.length ));
                    Debug.Log("Event injected for command " + injectE.stringParam );
                }
                else
                {
                    Debug.Log( "<color=red> Clip with name "+ injectE.TargetClip + " is not exist! </color>");
                }
            }

            target_animator.Rebind();
        }
    }

}
