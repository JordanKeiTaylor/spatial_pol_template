using System.Collections.Generic;
using Improbable.Gdk.Core;
using Unity.Entities;
using UnityEngine;

namespace Pol
{
    public class PolSystem : ComponentSystem
    {

        private int frames = 0;
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

       

        private void DoPolBehavior()
        {

            for (var j = 0; j < data.Length; ++j)
            {

                var polData = data.PolEntityComponents[j];
                switch (polData.Behavior)
                {
                    case Behaviors.Behavior1:
                        MoveUp(j);
                        break;
                    case Behaviors.Behavior2:
                        MoveDown(j);
                        break;
                    case Behaviors.Behavior3:
                        MoveDown(j);
                        break;

                }
               
            }
        }

        //this function is an example of how a global "push" update, from a model controller, out to PolEntities, could be managed
        //Currently the function simply aggregates the number of each tribe that are out of bounds, and adjusts their behavior accordingly
        //It also (to demonstrate how the PolController could be updated) updates the Pol Controller, which triggers a set of updates to the status
        //of the PolEntities. The latter is simply to show how one would listen for PolController updates and respond to them
        private void GlobalPolControllerUpdate()
        {
            var out_of_bounds = new Dictionary<uint, uint>();


            for (var j = 0; j < controllerData.Length; ++j)
            {

                var controller = controllerData.PolControllerComponents[j];
                for(var i = 0; i < data.Length; ++i)
                {
                    var entityData = data.PolEntityComponents[i];
                    var tribe = entityData.Tribe;
                    var position = positionData.PositionComponents[i].Coords;
                    if(position.X > 200 || position.X < -200 || position.Z > 200 || position.Z <-200)
                    {
                        if (out_of_bounds.ContainsKey(tribe))
                        {
                            out_of_bounds.Add(tribe, out_of_bounds[tribe] + 1);
                        }
                        else
                        {
                            out_of_bounds.Add(tribe, 1);
                        }
                    }
                }



                for (var i = 0; i < data.Length; ++i)
                {
                    var entityData = data.PolEntityComponents[i];
                    var tribe = entityData.Tribe;
                    var behavior = entityData.Behavior;
                    if (out_of_bounds.ContainsKey(tribe) && out_of_bounds[tribe] > 15 && behavior == Behaviors.Behavior1)
                    {
                        entityData.Behavior = Behaviors.Behavior2;

                    }
                    if (out_of_bounds.ContainsKey(tribe) && out_of_bounds[tribe] > 15 && behavior == Behaviors.Behavior2)
                    {
                        entityData.Behavior = Behaviors.Behavior1;

                    }
                    data.PolEntityComponents[i] = entityData;
                }


                controller.RobotsActive = controller.RobotsActive + 1;
                controllerData.PolControllerComponents[j] = controller;
            }
        }

        private void MoveUp(int entityIndex)
        {
            
           
                var component = positionData.PositionComponents[entityIndex];
                var origin = component.Coords;
                

                component.Coords = new Improbable.Coordinates
                {
                    X = origin.X,
                    Z = origin.Z + 1,
                    Y = origin.Y

                };
                positionData.PositionComponents[entityIndex] = component;
            
        }

        private void MoveDown(int entityIndex)
        {
            var component = positionData.PositionComponents[entityIndex];
            var origin = component.Coords;


            component.Coords = new Improbable.Coordinates
            {
                X = origin.X,
                Z = origin.Z - 1,
                Y = origin.Y
            };
            positionData.PositionComponents[entityIndex] = component;

        }

        protected override void OnUpdate()
        {
            frames++;
            if(frames%60 == 0)
            {
                DoPolBehavior();
            }
            if(frames % 1200 == 0)
            {
                GlobalPolControllerUpdate();
            }

            //for loop below checks for PolController updates every frame and responds to them by updating the status on all PolEntities
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
