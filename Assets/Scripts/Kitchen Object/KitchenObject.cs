using UnityEngine;

namespace KitchenChaos.Interactions
{
    public class KitchenObject : MonoBehaviour
    {
        [SerializeField] SO_KitchenObject _kitchenObjectSO;
        public SO_KitchenObject KitchenObjectSO => _kitchenObjectSO;

        IKitchenObjectHolder _kitchenObjectHolder;
        public IKitchenObjectHolder KitchenObjectHolder => _kitchenObjectHolder;

        int _cuttingTracker = 0;
        public int CuttingTracker
        {
            get { return _cuttingTracker; }
            set { _cuttingTracker = value; }
        }

        public void SetKitchenObjectHolder(IKitchenObjectHolder kitchenObjectHolder)
        {
            if (_kitchenObjectHolder != null)
                _kitchenObjectHolder.ClearKitchenObject();

            _kitchenObjectHolder = kitchenObjectHolder;

            if (_kitchenObjectHolder.HasKitchenObject())
                Debug.LogError("IKitchenObjectHolder already has a KitchenObject!");

            _kitchenObjectHolder.SetKitchenObject(this);

            transform.parent = _kitchenObjectHolder.GetKitchenObjectSpawnPoint();
            transform.localPosition = Vector3.zero;
        }

        public bool TryGetPlate(out PlateKitchenObject plate)
        {
            if (this is PlateKitchenObject)
            {
                plate = this as PlateKitchenObject;
                return true;
            }
            else
            {
                plate = null;
                return false;
            }
        }

        //for prototyping only - implement pooling
        public void DestroySelf()
        {
            _kitchenObjectHolder.ClearKitchenObject();
            Destroy(gameObject);
        }

        //for prototyping only - implement pooling
        public static KitchenObject SpawnKitchenObject(SO_KitchenObject kitchenObjectSO, IKitchenObjectHolder kitchenObjectHolder)
        {
            Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.KitchenObjectPrefab);
            KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
            kitchenObject.SetKitchenObjectHolder(kitchenObjectHolder);

            return kitchenObject;
        }
    }
}
