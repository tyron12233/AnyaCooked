using KitchenChaos.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenChaos.Interactions
{
    public class PlatesCounter : BaseCounter
    {
        public event Action OnPlateSpawned;
        public event Action OnPlateRemoved;

        [SerializeField] SO_KitchenObject _plateSO;
        [SerializeField] float _spawnPlateTimerMax = 4f;
        [SerializeField] int _platesSpawnedAmountMax = 4;

        float spawnPlateTimer;
        int _platesSpawnedAmount;

        private void Update()
        {
            if (!GameManager.Instance.IsGamePlaying()) return;

            spawnPlateTimer += Time.deltaTime;

            if (spawnPlateTimer > _spawnPlateTimerMax)
            {
                spawnPlateTimer = 0f;

                if (_platesSpawnedAmount < _platesSpawnedAmountMax)
                {
                    _platesSpawnedAmount++;
                    OnPlateSpawned?.Invoke();
                }
            }
        }

        public override void Interact(PlayerInteractions player)
        {
            if (!player.HasKitchenObject())
            {
                if (_platesSpawnedAmount > 0)
                {
                    _platesSpawnedAmount--;
                    KitchenObject.SpawnKitchenObject(_plateSO, player);

                    OnPlateRemoved?.Invoke();
                }
            }
        }
    }
}
