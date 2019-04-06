using Improbable.Gdk.Core;
using Improbable.Gdk.Health;
using Unity.Collections;
using Unity.Entities;
using Pol;
using Improbable;

namespace Pol
{


    public class PolControllerUpdateSystem : ComponentSystem
    {
        private struct PolControllerData
        {
            public readonly int Length;
            public ComponentDataArray<Pol.PolController.Component> PolEntityComponents;
          
        }

        [Inject] private PolControllerData data;

        //Updates polController

        protected override void OnUpdate()
        {
           
            for (var j = 0; j < data.Length; ++j)
            {
                var component = data.PolEntityComponents[j];
                component.RobotsActive = component.RobotsActive + 1;
                data.PolEntityComponents[j] = component;
            }
        }
    }

}