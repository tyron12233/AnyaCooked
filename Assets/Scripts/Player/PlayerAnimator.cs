using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenChaos.Control
{
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] PlayerController _player;
        
        const string IS_WALKING = "IsWalking";
        Animator _animator;

        void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        void Update()
        {
            _animator.SetBool(IS_WALKING, _player.IsWalking());
        }
    }
}
