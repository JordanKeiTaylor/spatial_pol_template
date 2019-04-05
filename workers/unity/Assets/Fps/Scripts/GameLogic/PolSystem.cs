using Improbable.Gdk.Core;
using Improbable.Gdk.Health;
using Unity.Collections;
using Unity.Entities;
using Pol;
using Improbable;

namespace Pol
{


    public class PolSystem : ComponentSystem
    {
        private struct PolEntityData
        {
            public readonly int Length;
            public ComponentDataArray<Pol.PolEntityData.Component> PolEntityComponents;
          
        }
        
        private struct UpdatesData
        {
            public readonly int Length;
            public ComponentDataArray<PolController.ReceivedUpdates> interestUpdates;

        }



        [Inject] private PolEntityData data;
        [Inject] private UpdatesData updatesData;


        protected override void OnUpdate()
        {
            for (var i = 0; i < updatesData.Length; ++i)
            {
                var updates = updatesData.interestUpdates[i].Updates;
                foreach (var update in updates)
                {
                    for (var j = 0; j < data.Length; ++j)
                    {
                        var component = data.PolEntityComponents[j];
                        component.Status = component.Status + 1;
                        data.PolEntityComponents[j] = component;
                    }
                }
            }


        }
    }

}