using System;
using MelonLoader;

using AdvancedAvatarControl.BoneMenu;

using UnityEngine;

namespace AdvancedAvatarControl
{
    [RegisterTypeInIl2Cpp]
    public class RepEyeController : MonoBehaviour
    {
        private Quaternion targetRotation;

        private void Start()
        {
            targetRotation = transform.rotation;
        }

        private void Update()
        {
            Quaternion desiredRotation = gameObject.transform.rotation;
            
            float lerpParameter = 0;
            if (Time.timeScale != 0)
            {
                lerpParameter = Prefs.EyeMovementSpeed.Value * Time.deltaTime / Time.timeScale;
            }

            targetRotation = Quaternion.Lerp(targetRotation, desiredRotation, lerpParameter);
            
            transform.rotation = targetRotation;
        }
        
        public RepEyeController(IntPtr ptr) : base(ptr) { }
    }
}