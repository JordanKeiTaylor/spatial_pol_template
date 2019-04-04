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
        }

        [Inject] private Data data;

        protected override void OnUpdate()
        {
            for (var i = 0; i < data.Length; ++i)
            {
                var component = data.PolEntityDataComponents[i];
                component.Status = component.Status + 1;
                data.PolEntityDataComponents[i] = component;
            }
        }
    }

}