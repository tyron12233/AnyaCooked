using System;
using Unity.Netcode;
using UnityEngine;
using KitchenChaos.Core;
using KitchenChaos.PlayerInput;
using KitchenChaos.Interactions;

namespace KitchenChaos.Control
{
    public class PlayerController : NetworkBehaviour
    {
        public static PlayerController LocalInstance { get; private set; }

        public static event Action OnAnyPlayerSpawned;
        public event Action<BaseCounter> OnSelectedCounterChanged;

        [SerializeField] float _speed = 7f;
        [SerializeField] LayerMask _countersLayer;
        GameInput _playerInput;

        bool _isWalking;
        Vector3 _lastInteractDir;
        BaseCounter _selectedCounter;
        PlayerInteractions _player;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            LocalInstance = this;
            OnAnyPlayerSpawned?.Invoke();
            _player = GetComponent<PlayerInteractions>();

            Debug.Log("Subscribers: " + OnSelectedCounterChanged.GetInvocationList().Length);
        }

        void Start()
        {
            _playerInput = FindObjectOfType<GameInput>();
            _playerInput.OnInteractAction += _playerInput_OnInteractAction;
            _playerInput.OnInteractAltAction += _playerInput_OnInteractAltAction;
        }

        private void _playerInput_OnInteractAltAction()
        {
            if (!GameManager.Instance.IsGamePlaying()) return;

            if (_selectedCounter != null)
                _selectedCounter.InteractAlt(_player);
        }

        void _playerInput_OnInteractAction()
        {
            if (!GameManager.Instance.IsGamePlaying()) return;

            if (_selectedCounter != null)
                _selectedCounter.Interact(_player);
        }

        void Update()
        {
            if (!IsOwner) return;

            HandleMovement();
            HandleInteractions();
        }
        
        public bool IsWalking()
        {
            return _isWalking;
        }

        void HandleMovement()
        {
            Vector2 inputVector = _playerInput.GetMovementVectorNormalized();
            Vector3 movDir = new Vector3(inputVector.x, 0f, inputVector.y);

            float playerHeight = 2f;
            float playerRadius = .7f;
            float moveDistance = _speed * Time.deltaTime;
            bool canMove = !Physics.CapsuleCast(transform.position, transform.position + (Vector3.up * playerHeight), playerRadius, movDir, moveDistance);

            if (!canMove)
            {
                //cannot move towards movDir -> attempt movement along x only
                Vector3 movDirX = new Vector3(movDir.x, 0, 0).normalized;
                canMove = (movDir.x < -0.5f || movDir.x > 0.5f) && !Physics.CapsuleCast(transform.position, transform.position + (Vector3.up * playerHeight), playerRadius, movDirX, moveDistance);

                if (canMove)
                    movDir = movDirX;
                else
                {
                    //cannot move along X only -> attempt movement along Z only
                    Vector3 movDirZ = new Vector3(0, 0, movDir.z).normalized;
                    canMove = (movDir.z < -0.5f || movDir.z > 0.5f) && !Physics.CapsuleCast(transform.position, transform.position + (Vector3.up * playerHeight), playerRadius, movDirZ, moveDistance);

                    if (canMove)
                        movDir = movDirZ;
                }
            }

            if (canMove)
                transform.position += movDir * moveDistance;

            _isWalking = movDir != Vector3.zero;

            float rotateSpeed = 10f;
            transform.forward = Vector3.Slerp(transform.forward, movDir, Time.deltaTime * rotateSpeed);
        }
        
        void HandleInteractions()
        {
            if (!GameManager.Instance.IsGamePlaying()) return;

            Vector2 inputVector = _playerInput.GetMovementVectorNormalized();
            Vector3 movDir = new Vector3(inputVector.x, 0, inputVector.y);
            float interactDistance = 2f;

            if (movDir != Vector3.zero)
                _lastInteractDir = movDir;

            if (Physics.Raycast(transform.position, _lastInteractDir, out RaycastHit raycastHit, interactDistance, _countersLayer))
            {
                if (raycastHit.transform.TryGetComponent(out BaseCounter counter))
                {
                    if (counter != _selectedCounter)
                    {
                        SetSelectedCounter(counter);
                    }
                }
                else
                    SetSelectedCounter(null);
            }
            else
                SetSelectedCounter(null);
        }

        void SetSelectedCounter(BaseCounter selectedCounter)
        {
            _selectedCounter = selectedCounter;
            OnSelectedCounterChanged?.Invoke(_selectedCounter);
        }

        new void OnDestroy()
        {
            _playerInput.OnInteractAction -= _playerInput_OnInteractAction;
            _playerInput.OnInteractAltAction -= _playerInput_OnInteractAltAction;
        }

        public static void ResetStaticData()
        {
            OnAnyPlayerSpawned = null;
        }
    }
}