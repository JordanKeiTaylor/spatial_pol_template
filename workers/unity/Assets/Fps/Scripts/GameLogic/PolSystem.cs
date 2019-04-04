using Improbable.Gdk.Core;
using Improbable.Gdk.Health;
using Unity.Collections;
using Unity.Entities;
using Pol;

namespace Pol
{


    public class PolSystem : ComponentSystem
    {
        private struct Data
        {
            public readonly int Length;
            public ComponentDataArray<Pol.PolEntityData.Component> PolEntityDataComponents;
            [ReadOnly] public ComponentDataArray<Pol.PolController.ReceivedUpdates> Updates;
        }

        [Inject] private Data data;

        protected override void OnUpdate()
        {
            for (var i = 0; i < data.Length; ++i)
            {
                var updates = data.Updates[i].Updates;
                foreach (var update in updates)
                {
                    //assuming here there will only be one POLController update every 3 seconds, because of the logic in the POlController
                    //if there are more frequent updates, will need to change logic to stagger resulting updates to POL EntityData
                    for (var j = 0; j < data.Length; ++j)
                    {
                        var component = data.PolEntityDataComponents[i];
                        component.Status = update.RobotsActive * 2;
                        data.PolEntityDataComponents[i] = component;
                    }
                }
            }
        }
    }

}