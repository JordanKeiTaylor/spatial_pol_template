using System.IO;
using Improbable.Gdk.Core;
using Improbable.Gdk.DeploymentManager;
using UnityEditor;
using UnityEngine;
using Improbable;

namespace Fps
{
    public class SnapshotMenu : MonoBehaviour
    {
        public static readonly string DefaultSnapshotPath =
            Path.Combine(Application.dataPath, "../../../snapshots/default.snapshot");

        public static readonly string CloudSnapshotPath =
            Path.Combine(Application.dataPath, "../../../snapshots/cloud.snapshot");

        private static void GenerateSnapshot(Snapshot snapshot)
        {
            var spawner = FpsEntityTemplates.Spawner();
            snapshot.AddEntity(spawner);
        }

        [MenuItem("SpatialOS/Generate FPS Snapshot")]
        private static void GenerateFpsSnapshot()
        {
            var localSnapshot = new Snapshot();
            var cloudSnapshot = new Snapshot();

            GenerateSnapshot(localSnapshot);
            GenerateSnapshot(cloudSnapshot);

            // The local snapshot is identical to the cloud snapshot, but also includes a simulated player coordinator
            // trigger.
            var simulatedPlayerCoordinatorTrigger = FpsEntityTemplates.SimulatedPlayerCoordinatorTrigger();
            var polController = FpsEntityTemplates.PolController(new Improbable.Vector3f(5, 0, 0));

            cloudSnapshot.AddEntity(polController);
            cloudSnapshot.AddEntity(simulatedPlayerCoordinatorTrigger);
            AddPolEntities(cloudSnapshot, 100);

            localSnapshot.AddEntity(polController);
            AddPolEntities(localSnapshot,100);
            localSnapshot.AddEntity(simulatedPlayerCoordinatorTrigger);

            SaveSnapshot(DefaultSnapshotPath, localSnapshot);
            SaveSnapshot(CloudSnapshotPath, cloudSnapshot);
        }


        private static void AddPolEntities(Snapshot snapshot, int numEntities)
        {


            
            
            for(int i = 10;i< numEntities; i++)
            {
                var x = Random.Range(-150, 150);
                var z = Random.Range(-150, 150);
                var polEntity = Fps.FpsEntityTemplates.PolEntity(new Vector3f(x,0,z),(uint) i%4);
                // Add the entity template to the snapshot.
                snapshot.AddEntity(polEntity);

            }
         
        }

        private static void SaveSnapshot(string path, Snapshot snapshot)
        {
            snapshot.WriteToFile(path);
            Debug.LogFormat("Successfully generated initial world snapshot at {0}", path);
        }
    }
}
