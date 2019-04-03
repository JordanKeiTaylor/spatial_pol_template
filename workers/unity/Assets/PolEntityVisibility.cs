using Improbable.Gdk.Subscriptions;
using Pol;
using UnityEngine;
 
namespace Fps
{
    [WorkerType(WorkerUtils.UnityClient)]
    public class PolEntityVisibility : MonoBehaviour
    {
        [Require] private Improbable.PositionReader positionReader;
        [Require] private PolEntityDataReader polEntityDataReader;
 
        private MeshRenderer cubeMeshRenderer;
 
        private void OnEnable()
        {
            cubeMeshRenderer = GetComponentInChildren<MeshRenderer>();
            cubeMeshRenderer.enabled = true;
            polEntityDataReader.OnUpdate += OnPolUpdate;
            UpdateVisibility();

        }

        private void UpdateVisibility()
        {
            cubeMeshRenderer.enabled = polEntityDataReader.Data.IsActive;
        }

        private void OnPolUpdate(PolEntityData.Update update)
        {
            UpdateVisibility();
        }
    }
}