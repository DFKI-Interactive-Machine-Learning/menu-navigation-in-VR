using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

namespace IML.Gaze
{
    /// <summary>
    /// Datastructure that tracks gaze hits registered by colliders
    /// </summary>
    [System.Serializable]
    public struct eyeTrackerHit {
        public Vector3 TargetCenter;
        public Vector3 HitPoint;

        public eyeTrackerHit(Vector3 t,Vector3 h){
            TargetCenter = t;

            HitPoint = h;
        }
    }

    /// <summary>
    /// Datastructure that holds the gaze hits registered by menus. Tracks every menu level.
    /// </summary>
    [System.Serializable]
    public struct MenuHits
    {
        public List<eyeTrackerHit> menu1;
        public List<eyeTrackerHit> menu2;
        public List<eyeTrackerHit> menu3;

        // previously simple assignment was used (menu=a) but this resulted in a shallow copy/call by reference, that caused the arrays to be empty at saving time
        // current version generates hard copy 
        public void SetMenu1(List<eyeTrackerHit> a)
        {
            menu1 = new List<eyeTrackerHit>(a);
        }
        public void SetMenu2(List<eyeTrackerHit> a)
        {
            menu2 = new List<eyeTrackerHit>(a);
        }
        public void SetMenu3(List<eyeTrackerHit> a)
        {
            menu3 = new List<eyeTrackerHit>(a);
        }
    }

    [System.Serializable]
    public class SerializableList<T> {
        public List<T> list;
    }

    /// <summary>
    /// Logic of Gaze Accuracy Grid
    /// </summary>
    public class GazeAccuracy : MonoBehaviour
    {
        public GameObject prefab;
        private SerializableList<eyeTrackerHit> hitInfos = new SerializableList<eyeTrackerHit>();

        private List<float> hitPointAngles = new List<float>();
        private XRRayInteractor gazeInteractor;

        private float skipInterval = 1f;

        private Time startTime;

        private float timeT;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            gazeInteractor = GameObject.Find("Gaze Interactor").GetComponent<XRGazeInteractor>();
            hitInfos.list = new List<eyeTrackerHit>();
        }


        void OnEnable()
        {   
            // // It is wrong to ask the position of the target onEnable since it is a moving target the position could be different at inference
            // targetCenter = transform.position;

            //track start time (appearance of menu) for measurements
            timeT = Time.time;
            hitInfos.list = new List<eyeTrackerHit>();
            hitPointAngles = new List<float>();

        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable()
        {
            CalculateAccuracyAndPrecisioAngl();
        }


        void Update()
        {
            var timeElapsed = Time.time - timeT;
            // timeT += Time.deltaTime;
            if (timeElapsed>=skipInterval)
            {
                UpdateHitPoints();
            }
        }

        /// <summary>
        /// logging function. because JsonUtility is used all datastructures (structs, lists, etc) need to be serializable.
        /// </summary>
        public void StoreAccGridResults(){
            string hits = JsonUtility.ToJson(hitInfos);
            string saveFile = Application.persistentDataPath + $"/{gameObject.name}ACCGRID.json";
            File.WriteAllText(saveFile, hits);
            UnityEngine.Debug.Log($"saved @ {saveFile}");

        }

        /// <summary>
        /// Debugging function to visualize the gaze hits on accuracy grid. 
        /// </summary>
        public void VisualizeResults(){
            foreach(var point in hitInfos.list){
                var currentCenter = transform.position;
                GameObject go = Instantiate(prefab) as GameObject;
                go.transform.SetParent(transform, true);
                go.transform.position = point.HitPoint + (currentCenter - point.TargetCenter);
            }
            StoreAccGridResults();
        }

        /// <summary>
        /// gaze hit track funtion. is called in update loop. 
        /// </summary>
        private void UpdateHitPoints()
        {
            RaycastHit? raycastHit;
            int raycastHitIndex;
            RaycastResult? uiRaycastHit;
            int uiRaycastHitIndex;
            bool isUIHitClosest;

            var eyeOrigin = gazeInteractor.rayOriginTransform.position;
            var targetCenter = transform.position;

            gazeInteractor.TryGetCurrentRaycast(out raycastHit, out raycastHitIndex, out uiRaycastHit, out uiRaycastHitIndex, out isUIHitClosest);
            
            if (raycastHit.HasValue)
            {            
                var a = targetCenter - eyeOrigin;
                var b = raycastHit.Value.point - eyeOrigin;

                var point = raycastHit.Value.point;
                hitInfos.list.Add(new eyeTrackerHit(targetCenter,point));
                var cross = Vector3.Cross(a, b).magnitude;
                var dotProd = Vector3.Dot(a, b);
                hitPointAngles.Add(Mathf.Atan2(cross, dotProd));
                //Debug.Log($"a:{a} b:{b} cross:{cross} dot:{dotProd} Target{targetCenter} point{raycastHit.Value.point}");
            }
        }



       
        /// <summary>
        /// calculation of accuracy and precision
        /// </summary>
        private void CalculateAccuracyAndPrecisioAngl(){
            if (hitPointAngles.Count == 0)
            {
                Debug.LogWarning("No hit points recorded.");
                return;
            }

            string saveFile = Application.persistentDataPath + "/ACCGRID.txt";


            // Accuracy: average distance from the hit points to the target center
            float accuracy = hitPointAngles.Average();

            // Precision: standard deviation of distances from hit points to the target center
            float meanDistance = hitPointAngles.Average();
            float precision = Mathf.Sqrt(hitPointAngles.Sum(ang => Mathf.Pow(ang - meanDistance, 2))/hitPointAngles.Count);

            // hitPointAngles.Reverse();
            // foreach(var ang in hitPointAngles.Take(5)){

            // Debug.Log(ang);
            // }
            string msg = $"Accuracy: {accuracy}, Precision: {precision}, #angles: {hitPointAngles.Count}, from: {gameObject.name}";
            Debug.LogError(msg);
            File.AppendAllText(saveFile, msg + Environment.NewLine);


        }
    }
}
