using System.Collections;using System.Collections.Generic;
using UnityEngine;
using Improbable.Gdk.Subscriptions;
using Pol;
namespace Fps
{

    [WorkerType(WorkerUtils.UnityGameLogic)]
    public class PolControllerDriver : MonoBehaviour
    {
        [Require] private Improbable.PositionReader positionReader;
        [Require] private Pol.PolEntityDataReader polEntityDataReader;
        [Require] private Pol.PolControllerWriter polControllerWriter;
        [Require] private Pol.PolControllerReader polControllerReader;
        private MeshRenderer cubeMeshRenderer;
        private Coroutine polUpdateCoroutine;

        private void OnEnable()
        {
            cubeMeshRenderer = GetComponentInChildren<MeshRenderer>();
            cubeMeshRenderer.enabled = true;
            UpdateVisibility();
            polUpdateCoroutine = StartCoroutine(sendPOLUpdateRoutine());
            polControllerWriter.SendUpdate(new PolController.Update
            {
                RobotsActive = 2
            });

        }

        private void SendPolUpdate()
        {
            polControllerWriter.SendUpdate(new PolController.Update
            {
                RobotsActive = polControllerReader.Data.RobotsActive + 1
            });
        }

        private void OnDisable()
        {
            if (polUpdateCoroutine != null)
            {
                StopCoroutine(polUpdateCoroutine);
            }
        }

        private void UpdateVisibility()
        {
            cubeMeshRenderer.enabled = polEntityDataReader.Data.IsActive;
        }

        private IEnumerator sendPOLUpdateRoutine()
        {
            yield return new WaitForSeconds(5f);
            SendPolUpdate();
        }

    }
}
