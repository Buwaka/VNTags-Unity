using System;
using UnityEngine;

namespace VNTags.Components
{
    [RequireComponent(typeof(Animator))]
    public class VNTransitionComponent : MonoBehaviour
    {
        private Animator _animator;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void test()
        {
            
        }
    }
}