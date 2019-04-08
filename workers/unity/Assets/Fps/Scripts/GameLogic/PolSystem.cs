using Improbable.Gdk.Core;
using Unity.Entities;
using UnityEngine;

namespace Pol
{
    public class PolSystem : ComponentSystem
    {

        private float last_update = Time.time;
        private float rough_framerate = 1.0f / Time.deltaTime;
        private uint current_robots_active;
        private struct PolEntityData
        {
            public readonly int Length;
            public ComponentDataArray<Pol.PolEntityData.Component> PolEntityComponents;
        }

        private struct PositionData
        {
            public readonly int Length;
            public ComponentDataArray<Improbable.Position.Component> PositionComponents;
        }

        private struct PolControllerData
        {
            public readonly int Length;
            public ComponentDataArray<PolController.Component> PolControllerComponents;
        }

        [Inject] private PolEntityData data;
        [Inject] private PolControllerData controllerData;
        [Inject] private PositionData positionData;

        private ComponentUpdateSystem updateSystem;

        protected override void OnCreateManager()
        {
            base.OnCreateManager();

            updateSystem = World.GetExistingManager<ComponentUpdateSystem>();
        }

        private void OnEnable()
        {

        }

        private void MoveTowards(Improbable.Coordinates destination)
        {

            for (var j = 0; j < data.Length; ++j)
            {
                var component = positionData.PositionComponents[j];
                var origin = component.Coords;
                component.Coords = new Improbable.Coordinates
                {
                    X = origin.X + 0.05,
                    Y =  origin.Y + 0.001,
                    Z = origin.Z + 0.05
                };
                positionData.PositionComponents[j] = component;
            }
        }

        protected override void OnUpdate()
        {
            MoveTowards(new Improbable.Coordinates(0, 0, 0));
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