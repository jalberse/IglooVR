using Igloo.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_INPUTSYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace Igloo.UI
{
    /// <summary>
    /// Assists with swapping between the OnScreen Canvas system, and the igloo world UI canvas system.
    /// Sets the correct input module to active, and disables the one that isn't required.
    /// Takes an assembly definition based on the new InputModule system for UI input. 
    /// </summary>
    public class IglooCanvasController : Singleton<IglooCanvasController>
    {

        [HideInInspector] public VirtualInputModule vim;
#if UNITY_INPUTSYSTEM
        [HideInInspector] public InputSystemUIInputModule isuiim;
#else
        [HideInInspector] public StandaloneInputModule isuiim;
        
#endif

        protected override void AwakeInternal()
        {
            base.AwakeInternal();
#if UNITY_INPUTSYSTEM
            isuiim = this.GetComponent<InputSystemUIInputModule>();
#else
            isuiim = this.GetComponent<StandaloneInputModule>();
#endif
            vim = this.GetComponent<VirtualInputModule>();
        }

        public void SwitchToIglooMode()
        {
            isuiim.enabled = false;
            vim.enabled = true;
        }

        public void SwitchToScreenMode()
        {
            isuiim.enabled = true;
            vim.enabled = false;
        }
    }
}

