
using System;

namespace KitchenChaos.Interactions
{
    public class CuttingCounter : BaseCounter, IHasProgress
    {
        public static event Action<CuttingCounter> OnAnyCut;
        public event Action<float> OnProgressChanged;
        public event Action OnCut;

        SO_CuttingRecipe _currentCuttingRecipe;

        public override void Interact(PlayerInteractions player)
        {
            if (!HasKitchenObject())
            {
                if (player.HasKitchenObject())
                {
                    player.GetKitchenObject().SetKitchenObjectHolder(this);

                    if (CanBeCut())
                    {
                        _currentCuttingRecipe = GetKitchenObject().KitchenObjectSO.GetCuttingRecipe();
                        
                        if (GetKitchenObject().CuttingTracker != 0)
                            OnProgressChanged?.Invoke(GetProgressNormalized());
                    }
                }
            }
            else
            {
                if (!player.HasKitchenObject())
                {
                    if (CanBeCut())
                    {
                        OnProgressChanged?.Invoke(0);
                        _currentCuttingRecipe = null;
                    }

                    GetKitchenObject().SetKitchenObjectHolder(player);
                }
                else
                {
                    if (!AttemptTransferFromCounterToPlate(player))
                        AttemptTransferFromPlayerToPlate(player);
                }
            }
        }

        public override void InteractAlt(PlayerInteractions player)
        {
            if (!HasKitchenObject() || player.HasKitchenObject() || !CanBeCut()) return;

            float cuttingProgress = AddCuts();
            OnCut?.Invoke();
            OnAnyCut?.Invoke(this);
            OnProgressChanged?.Invoke(cuttingProgress);
            
            if (FinishedCutting())
            {
                GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKitchenObject(GetCuttingOutput(), this);
                _currentCuttingRecipe = null; 
            }
        }

        bool FinishedCutting()
        {
            return GetKitchenObject().CuttingTracker >= GetCuttingProgressMax();
        }

        SO_KitchenObject GetCuttingOutput()
        {
            return _currentCuttingRecipe.Output;
        }

        bool CanBeCut()
        {
            return GetKitchenObject().KitchenObjectSO.GetCuttingRecipe() != null;
        }

        int GetCuttingProgressMax()
        {
            return _currentCuttingRecipe.CuttingProgressMax;
        }

        float AddCuts()
        {
            GetKitchenObject().CuttingTracker++;
            return GetProgressNormalized();
        }

        float GetProgressNormalized()
        {
            return (float)GetKitchenObject().CuttingTracker / _currentCuttingRecipe.CuttingProgressMax;
        }

        new public static void ResetStaticData()
        {
            OnAnyCut = null;
        }
    }
}
