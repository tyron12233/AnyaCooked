using System;
using System.Collections.Generic;
using UnityEngine;
using KitchenChaos.Interactions;
using System.Collections;

namespace KitchenChaos.Core
{
    public class DeliveryManager : MonoBehaviour
    {
        public static DeliveryManager Instance { get; private set; }

        public event Action OnNewRecipeGenerated;
        public event Action OnRecipeDelivered;
        public event Action OnRecipeFailed;

        [SerializeField] SO_RecipeList _recipeListSO;
        [SerializeField] float _spawnRecipeTimerMax = 4f;
        [SerializeField] int _pendingRecipesMax = 4;

        List<SO_FinalRecipe> _finalRecipesList;
        List<SO_FinalRecipe> _pendingRecipesList = new List<SO_FinalRecipe>();
        public List<SO_FinalRecipe> PendingRecipesList => _pendingRecipesList;

        float _spawnRecipeTimer;
        int _successfulRecipesDelivered;
        public int SuccessfulRecipesDelivered => _successfulRecipesDelivered;

        int _deliveredRecipesScore;
        public int DeliveredRecipesScore => _deliveredRecipesScore;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            _finalRecipesList = _recipeListSO.FinalRecipesList;    
        }

        void Update()
        {
            if (!GameManager.Instance.IsGamePlaying()) return;

            _spawnRecipeTimer -= Time.deltaTime;

            GenerateNewRecipe();
        }

        //use coroutine, so the recipes don't spawn straight away?
        void GenerateNewRecipe()
        {
            if (_spawnRecipeTimer <= 0f)
            {
                _spawnRecipeTimer = _spawnRecipeTimerMax;

                if (_pendingRecipesList.Count < _pendingRecipesMax)
                {
                    SO_FinalRecipe recipe = _finalRecipesList[UnityEngine.Random.Range(0, _finalRecipesList.Count)];
                    Debug.Log(recipe.RecipeName);
                    _pendingRecipesList.Add(recipe);
                    OnNewRecipeGenerated?.Invoke();
                }
            }
        }

        public void DeliverRecipe(PlateKitchenObject plate)
        {
            foreach (var recipe in _pendingRecipesList)
            {
                if (DoIngredientsMatch(recipe, plate))
                {
                    Debug.Log("Correct Recipe Delivered!");
                    _successfulRecipesDelivered++;
                    _deliveredRecipesScore += recipe.DeliveryPoints;

                    _pendingRecipesList.Remove(recipe);
                    OnRecipeDelivered?.Invoke();
                    return;
                }
            }

            OnRecipeFailed?.Invoke();
            Debug.Log("Wrong Recipe Delivered!");
        }

        bool DoIngredientsMatch(SO_FinalRecipe recipe, PlateKitchenObject plate)
        {
            if (recipe.KitchenObjectSOList.Count != plate.KitchenObjectsList.Count) 
                return false;

            foreach (var recipeIngredient in recipe.KitchenObjectSOList)
            {
                if (!plate.KitchenObjectsList.Contains(recipeIngredient)) 
                    return false;
            }

            return true;
        }

    }
}
