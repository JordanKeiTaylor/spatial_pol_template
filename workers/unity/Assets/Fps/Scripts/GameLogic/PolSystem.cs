using Improbable.Gdk.Core;
using Unity.Entities;
using UnityEngine;

namespace Pol
{
    public class PolSystem : ComponentSystem
    {

        private float last_update = -1;
        private float rough_framerate = 1.0f / Time.deltaTime;
        private uint current_robots_active;
        private struct PolEntityData
        {
            public readonly int Length;
            public ComponentDataArray<Pol.PolEntityData.Component> PolEntityComponents;
        }

        private struct PolControllerData
        {
            public readonly int Length;
            public ComponentDataArray<PolController.Component> PolControllerComponents;
        }

        [Inject] private PolEntityData data;
        [Inject] private PolControllerData controllerData;

        private ComponentUpdateSystem updateSystem;

        protected override void OnCreateManager()
        {
            base.OnCreateManager();

            updateSystem = World.GetExistingManager<ComponentUpdateSystem>();
        }


        protected override void OnUpdate()
        {

            var updates = updateSystem.GetComponentUpdatesReceived<PolController.Update>();
            for (var k = 0; k < updates.Count; k++)
            {
                current_robots_active = updates[0].Update.RobotsActive;
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