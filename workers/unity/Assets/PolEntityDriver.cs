using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Gdk.Subscriptions;
using Pol;
namespace Fps {

    [WorkerType(WorkerUtils.UnityGameLogic)]
    public class PolEntityDriver : MonoBehaviour
    {
        [Require] private Improbable.PositionReader positionReader;
        [Require] private Pol.PolEntityDataReader polEntityDataReader;
        [Require] private Pol.PolEntityDataWriter polEntityDataWriter;
        [Require] private Pol.PolControllerReader polControllerReader;
        private MeshRenderer cubeMeshRenderer;

        private void OnEnable()
        {
            cubeMeshRenderer = GetComponentInChildren<MeshRenderer>();
            cubeMeshRenderer.enabled = true;
            polControllerReader.OnUpdate += OnPolControllerUpdate;
            UpdateVisibility();

        }

        private void UpdateVisibility()
        {
            cubeMeshRenderer.enabled = polEntityDataReader.Data.IsActive;
        }

        private void OnPolControllerUpdate(PolController.Update update)
        {
            polEntityDataWriter.SendUpdate(new PolEntityData.Update
            {Status = update.RobotsActive * polEntityDataReader.Data.Tribe
            });
        }
    }
}
