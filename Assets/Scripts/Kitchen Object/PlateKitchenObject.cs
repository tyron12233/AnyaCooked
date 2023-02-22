using System;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenChaos.Interactions
{
    public class PlateKitchenObject : KitchenObject
    {
        public event Action<SO_KitchenObject> OnIngredientAdded;

        [SerializeField] List<SO_KitchenObject> _acceptableKitchenObjects;

        List<SO_KitchenObject> _kitchenObjects = new List<SO_KitchenObject>();
        public List<SO_KitchenObject> KitchenObjectsList => _kitchenObjects;

        public bool TryAddIngredient(SO_KitchenObject kitchenObjectSO)
        {
            if (!_acceptableKitchenObjects.Contains(kitchenObjectSO))
                return false;
            else
            {
                _kitchenObjects.Add(kitchenObjectSO);
                OnIngredientAdded?.Invoke(kitchenObjectSO);
                return true;
            }
        }
    }
}
