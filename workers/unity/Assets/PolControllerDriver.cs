using System.Collections;using System.Collections.Generic;
using UnityEngine;
using Improbable.Gdk.Subscriptions;
using Pol;
namespace Fps
{
    //Class deprecated -- all significant logic moved to PolSystem class
    [WorkerType(WorkerUtils.UnityGameLogic)]
    public class PolControllerDriver : MonoBehaviour
    {
        [Require] private Improbable.PositionReader positionReader;
        [Require] private Pol.PolControllerWriter polControllerWriter;
      


        private Coroutine polUpdateCoroutine;

        private void Awake()
        {

        }


   

    }
}
