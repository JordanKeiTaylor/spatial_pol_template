using Improbable.Gdk.Core;
using Unity.Entities;
using UnityEngine;

namespace Pol
{
    public class PolSystem : ComponentSystem
    {

        private readonly float last_update = -1;
        private struct PolEntityData
        {
            public readonly int Length;
            public ComponentDataArray<Pol.PolEntityData.Component> PolEntityComponents;
        }

        [Inject] private PolEntityData data;

        private ComponentUpdateSystem updateSystem;

        protected override void OnCreateManager()
        {
            base.OnCreateManager();

            updateSystem = World.GetExistingManager<ComponentUpdateSystem>();
        }

        protected override void OnUpdate()
        {
            if((int) last_update != -1 && (int)Time.time - (int)last_update < 5000)
            {
                return;
            }
            var updates = updateSystem.GetComponentUpdatesReceived<PolController.Update>();
            for (var k = 0; k < updates.Count; k++)
            {
                for (var j = 0; j < data.Length; ++j)
                {
                    var component = data.PolEntityComponents[j];
                    component.Status = updates[k].Update.RobotsActive;
                    data.PolEntityComponents[j] = component;
                }
            }
        }
    }
}