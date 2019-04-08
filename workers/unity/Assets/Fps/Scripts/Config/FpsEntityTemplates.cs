using System.Collections.Generic;
using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.Guns;
using Improbable.Gdk.Health;
using Improbable.Gdk.Movement;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Gdk.StandardTypes;
using Improbable.PlayerLifecycle;
using Pol;
using UnityEngine;

namespace Fps
{
    public static class FpsEntityTemplates
    {
        public static EntityTemplate Spawner()
        {
            var position = new Position.Snapshot { Coords = new Vector3().ToSpatialCoordinates() };
            var metadata = new Metadata.Snapshot { EntityType = "PlayerCreator" };

            var template = new EntityTemplate();
            template.AddComponent(position, WorkerUtils.UnityGameLogic);
            template.AddComponent(metadata, WorkerUtils.UnityGameLogic);
            template.AddComponent(new Persistence.Snapshot(), WorkerUtils.UnityGameLogic);
            template.AddComponent(new PlayerCreator.Snapshot(), WorkerUtils.UnityGameLogic);

            template.SetReadAccess(WorkerUtils.UnityGameLogic);
            template.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.UnityGameLogic);

            return template;
        }

        public static EntityTemplate PolEntity(Vector3f position,uint tribe)
        {

            var metadata = new Metadata.Snapshot { EntityType = "PolEntity" };


            var rotationUpdate = new RotationUpdate
            {
                Yaw = 0,
                Pitch = 0
            };

            var serverResponse = new ServerResponse
            {
                Position = position.ToUnityVector3().ToIntAbsolute()
            };

            var template = new EntityTemplate();
            template.AddComponent(new Position.Snapshot(new Coordinates(position.X, position.Y, position.Z)), WorkerUtils.UnityGameLogic);
            template.AddComponent(metadata, WorkerUtils.UnityGameLogic);
            var clientRotation = new ClientRotation.Snapshot { Latest = rotationUpdate };
            template.AddComponent(new Persistence.Snapshot(), WorkerUtils.UnityGameLogic);
            template.AddComponent(new Pol.PolEntityData.Snapshot(true,tribe,1,Behaviors.Behavior1), WorkerUtils.UnityGameLogic);
            var serverMovement = new ServerMovement.Snapshot { Latest = serverResponse };
            var clientMovement = new ClientMovement.Snapshot { Latest = new ClientRequest() };

            template.SetReadAccess(WorkerUtils.UnityGameLogic, WorkerUtils.UnityClient);
            template.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.UnityGameLogic);

            var playerInterest = new ComponentInterest
            {
                Queries = new List<ComponentInterest.Query>
                {
                    new ComponentInterest.Query
                    {
                        Constraint = new ComponentInterest.QueryConstraint
                        {
                            ComponentConstraint = Pol.PolController.ComponentId,
                            AndConstraint = new List<ComponentInterest.QueryConstraint>(),
                            OrConstraint = new List<ComponentInterest.QueryConstraint>()
                        },FullSnapshotResult = true,
                        ResultComponentId = new List<uint>(),
                    }
                }
            };

            var interestComponent = new Interest.Snapshot
            {
                ComponentInterest = new Dictionary<uint, ComponentInterest>
                {
                    { PolEntityData.ComponentId, playerInterest },
                },
            };

            template.AddComponent(interestComponent, WorkerUtils.UnityGameLogic);
            template.AddComponent(serverMovement, WorkerUtils.UnityGameLogic);
            template.AddComponent(clientMovement, WorkerUtils.UnityGameLogic);
            template.AddComponent(clientRotation, WorkerUtils.UnityGameLogic);

            return template;
        }


        public static EntityTemplate PolControllerEntity(Vector3f position)
        {
            // Create a HealthPickup component snapshot which is initially active and grants "heathValue" on pickup.
            var polControllerComponent = new PolController.Snapshot(true, 0, new Dictionary<uint, EntityId>());

            var entityTemplate = new EntityTemplate();

           

            entityTemplate.AddComponent(new Position.Snapshot(new Coordinates(position.X, position.Y, position.Z)), WorkerUtils.UnityGameLogic);
            entityTemplate.AddComponent(new Metadata.Snapshot("PolController"), WorkerUtils.UnityGameLogic);
            entityTemplate.AddComponent(new Persistence.Snapshot(), WorkerUtils.UnityGameLogic);
            entityTemplate.AddComponent(polControllerComponent, WorkerUtils.UnityGameLogic);
            entityTemplate.SetReadAccess(WorkerUtils.UnityGameLogic, WorkerUtils.UnityClient,WorkerUtils.SimulatedPlayerCoordinator);

            entityTemplate.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.UnityGameLogic);
            entityTemplate.SetComponentWriteAccess(PolController.ComponentId, WorkerUtils.UnityGameLogic);

            var polEntityInterest = new ComponentInterest
            {
                Queries = new List<ComponentInterest.Query>
                {
                    new ComponentInterest.Query
                    {
                        Constraint = new ComponentInterest.QueryConstraint
                        {
                            ComponentConstraint = Pol.PolEntityData.ComponentId,
                            AndConstraint = new List<ComponentInterest.QueryConstraint>(),
                            OrConstraint = new List<ComponentInterest.QueryConstraint>()
                        },FullSnapshotResult = true,
                        ResultComponentId = new List<uint>(),
                    }
                }
            };

            var interestComponent = new Interest.Snapshot
            {
                ComponentInterest = new Dictionary<uint, ComponentInterest>
                {
                    { polControllerComponent.ComponentId, polEntityInterest },
                },
            };

           entityTemplate.AddComponent(interestComponent, WorkerUtils.UnityGameLogic);

            return entityTemplate;
        }

        public static EntityTemplate SimulatedPlayerCoordinatorTrigger()
        {
            var metadata = new Metadata.Snapshot { EntityType = "SimulatedPlayerCoordinatorTrigger" };

            var template = new EntityTemplate();
            template.AddComponent(new Position.Snapshot(), WorkerUtils.SimulatedPlayerCoordinator);
            template.AddComponent(metadata, WorkerUtils.SimulatedPlayerCoordinator);
            template.AddComponent(new Persistence.Snapshot(), WorkerUtils.SimulatedPlayerCoordinator);

            template.SetReadAccess(WorkerUtils.SimulatedPlayerCoordinator);
            template.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.SimulatedPlayerCoordinator);

            return template;
        }

        public static EntityTemplate Player(string workerId, byte[] args)
        {
            var client = $"workerId:{workerId}";

            var (spawnPosition, spawnYaw, spawnPitch) = SpawnPoints.GetRandomSpawnPoint();

            var serverResponse = new ServerResponse
            {
                Position = spawnPosition.ToIntAbsolute()
            };

            var rotationUpdate = new RotationUpdate
            {
                Yaw = spawnYaw.ToInt1k(),
                Pitch = spawnPitch.ToInt1k()
            };

            var pos = new Position.Snapshot { Coords = spawnPosition.ToSpatialCoordinates() };
            var serverMovement = new ServerMovement.Snapshot { Latest = serverResponse };
            var clientMovement = new ClientMovement.Snapshot { Latest = new ClientRequest() };
            var clientRotation = new ClientRotation.Snapshot { Latest = rotationUpdate };
            var shootingComponent = new ShootingComponent.Snapshot();
            var gunComponent = new GunComponent.Snapshot { GunId = PlayerGunSettings.DefaultGunIndex };
            var gunStateComponent = new GunStateComponent.Snapshot { IsAiming = false };
            var healthComponent = new HealthComponent.Snapshot
            {
                Health = PlayerHealthSettings.MaxHealth,
                MaxHealth = PlayerHealthSettings.MaxHealth,
            };

            var healthRegenComponent = new HealthRegenComponent.Snapshot
            {
                CooldownSyncInterval = PlayerHealthSettings.SpatialCooldownSyncInterval,
                DamagedRecently = false,
                RegenAmount = PlayerHealthSettings.RegenAmount,
                RegenCooldownTimer = PlayerHealthSettings.RegenAfterDamageCooldown,
                RegenInterval = PlayerHealthSettings.RegenInterval,
                RegenPauseTime = 0,
            };

            var playerInterest = new ComponentInterest
            {
                Queries = new List<ComponentInterest.Query>
                {
                    new ComponentInterest.Query
                    {
                        Constraint = new ComponentInterest.QueryConstraint
                        {
                            ComponentConstraint = Pol.PolController.ComponentId
                        }
                    }
                }
            };

            var interestComponent = new Interest.Snapshot
            {
                ComponentInterest = new Dictionary<uint, ComponentInterest>
                {
                    { ClientMovement.ComponentId, playerInterest },
                },
            };

            var template = new EntityTemplate();
            template.AddComponent(pos, WorkerUtils.UnityGameLogic);
            template.AddComponent(new Metadata.Snapshot { EntityType = "Player" }, WorkerUtils.UnityGameLogic);
            template.AddComponent(serverMovement, WorkerUtils.UnityGameLogic);
            template.AddComponent(clientMovement, client);
            template.AddComponent(clientRotation, client);
            template.AddComponent(shootingComponent, client);
            template.AddComponent(gunComponent, WorkerUtils.UnityGameLogic);
            template.AddComponent(gunStateComponent, client);
            template.AddComponent(healthComponent, WorkerUtils.UnityGameLogic);
            template.AddComponent(healthRegenComponent, WorkerUtils.UnityGameLogic);
            template.AddComponent(interestComponent, WorkerUtils.UnityClient);
            PlayerLifecycleHelper.AddPlayerLifecycleComponents(template, workerId, client, WorkerUtils.UnityGameLogic);

            template.SetReadAccess(WorkerUtils.UnityClient, WorkerUtils.UnityGameLogic, WorkerUtils.AndroidClient, WorkerUtils.iOSClient);
            template.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.UnityGameLogic);

            return template;
        }
    }
}
