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
        [Require] private Pol.PolControllerWriter polControllerWriter;
      


        private Coroutine polUpdateCoroutine;

        private void Awake()
        {

        }

        private void OnEnable()
        {


            polUpdateCoroutine = StartCoroutine(sendPOLUpdateRoutine());
            polControllerWriter.SendUpdate(new PolController.Update
            {
                RobotsActive = 2
            });
            InvokeRepeating("SendPolUpdate", 4.0f, 4.0f);

        }

        private void SendPolUpdate()
        {
            polControllerWriter.SendUpdate(new PolController.Update
            {
                RobotsActive = polControllerWriter.Data.RobotsActive + 1
            });
        }

        private void OnDisable()
        {
            if (polUpdateCoroutine != null)
            {
                StopCoroutine(polUpdateCoroutine);
            }
        }

       

        private IEnumerator sendPOLUpdateRoutine()
        {
            yield return new WaitForSeconds(5f);
            SendPolUpdate();
        }

    }
}
