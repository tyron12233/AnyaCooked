using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenChaos.Interactions
{
    [CreateAssetMenu()]
    public class SO_FinalRecipe : ScriptableObject
    {
        [SerializeField] List<SO_KitchenObject> _kitchenObjectSOList;
        public List<SO_KitchenObject> KitchenObjectSOList => _kitchenObjectSOList;

        [SerializeField] string _recipeName;
        public string RecipeName => _recipeName;

        //should have a score field! - track this instead of number of recipes delivered!
    }
}
