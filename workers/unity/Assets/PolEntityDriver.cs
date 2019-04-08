using System.Collections;
using Improbable.Common;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using Improbable.Gdk.Guns;
using Improbable.Gdk.Health;
using Improbable.Gdk.Movement;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;
using Pol;

namespace Fps {

    [WorkerType(WorkerUtils.UnityGameLogic)]
    public class PolEntityDriver : MonoBehaviour
    {
   
        [Require] private Pol.PolEntityDataWriter polEntityDataWriter;
        [Require] private Improbable.PositionWriter positionDataWriter;

      

      

        private void Update()
        {
           
           
        }

      
    }
}
