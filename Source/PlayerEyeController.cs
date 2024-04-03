using System;
using MelonLoader;

using AdvancedAvatarControl.BoneMenu;

using UnityEngine;

namespace AdvancedAvatarControl
{
    [RegisterTypeInIl2Cpp]
    public class PlayerEyeController : MonoBehaviour
    {
        private Quaternion targetRotation;

        private void Start()
        {
            targetRotation = transform.rotation;
        }

        private void Update()
        {
            Quaternion desiredRotation = gameObject.transform.rotation;
            
            targetRotation = Quaternion.Lerp(targetRotation, desiredRotation, Prefs.EyeMovementSpeed.Value * Time.deltaTime * Time.timeScale);
            
            transform.rotation = targetRotation;
        }
        
        public PlayerEyeController(IntPtr ptr) : base(ptr) { }
    }
}