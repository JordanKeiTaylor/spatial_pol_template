using Improbable.Gdk.Core;
using Unity.Entities;

namespace Pol
{
    public class PolSystem : ComponentSystem
    {
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
            var updates = updateSystem.GetComponentUpdatesReceived<PolController.Update>();
            for (var k = 0; k < updates.Count; k++)
            {
                for (var j = 0; j < data.Length; ++j)
                {
                    var component = data.PolEntityComponents[j];
                    component.Status += 1;
                    data.PolEntityComponents[j] = component;
                }
            }
        }
    }
}