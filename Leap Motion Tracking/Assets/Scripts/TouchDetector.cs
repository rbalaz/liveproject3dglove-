using InteractionEngineUtility;
using Leap;
using Leap.Unity;
using Leap.Unity.Interaction;
using Leap.Unity.Query;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchFinger
{
    public Finger.FingerType fingerType;

    public List<float> forces = new List<float>();

    private float _force = 0;
    public float Force
    {
        get { return _force; }
        set
        {
            if (value > 100) _force = 100;
            else if (value < 0) _force = 0;
            else _force = value;
        }
    }
}

public class TouchDetector : MonoBehaviour {

    private float CONTACT_POINT_SIZE = 0.02f;


    public delegate void TouchValueChanged(Finger.FingerType fingerType, bool isLeftHand, float force);
    public static event TouchValueChanged OnTouchValueChange;

    public static TouchFinger[] touchFingersLeft;
    public static TouchFinger[] touchFingersRight;

    private static InteractionManager manager;
    private static int controllerLayer;
    private static int groundLayer;

    private static TouchDetector _instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (_instance == null)
        {
            _instance = new GameObject("TouchDetector").AddComponent<TouchDetector>();
            DontDestroyOnLoad(_instance.gameObject);
            touchFingersLeft = new TouchFinger[] {
                new TouchFinger { fingerType = Finger.FingerType.TYPE_THUMB },
                new TouchFinger { fingerType = Finger.FingerType.TYPE_INDEX },
                new TouchFinger { fingerType = Finger.FingerType.TYPE_MIDDLE },
                new TouchFinger { fingerType = Finger.FingerType.TYPE_RING },
                new TouchFinger { fingerType = Finger.FingerType.TYPE_PINKY }
            };
            touchFingersRight = new TouchFinger[] {
                new TouchFinger { fingerType = Finger.FingerType.TYPE_THUMB },
                new TouchFinger { fingerType = Finger.FingerType.TYPE_INDEX },
                new TouchFinger { fingerType = Finger.FingerType.TYPE_MIDDLE },
                new TouchFinger { fingerType = Finger.FingerType.TYPE_RING },
                new TouchFinger { fingerType = Finger.FingerType.TYPE_PINKY }
            };
        }
    }

    void Start()
    {
        manager = InteractionManager.instance;
        controllerLayer = manager.contactBoneLayer.layerIndex;
        groundLayer = LayerMask.NameToLayer("GroundLayer");
    }

    void Update()
    {
        CheckForceBinary();
        //CheckForce();
    }


    void LateUpdate()
    {
        foreach (var finger in TouchDetector.touchFingersLeft)
        {
            SelectMaxForce(finger);
            OnTouchValueChange(finger.fingerType, true, finger.Force);
        }
        foreach (var finger in TouchDetector.touchFingersRight)
        {
            SelectMaxForce(finger);
            OnTouchValueChange(finger.fingerType, false, finger.Force);
        }
    }

    private void SelectMaxForce(TouchFinger finger)
    {
        if (finger.forces.Count != 0)
        {
            finger.forces.Sort();
            finger.Force = finger.forces[finger.forces.Count - 1]; // Select maximum
        }
        else
        {
            finger.Force = 0;
        }
        finger.forces = new List<float>();
    }

    public static void UpdateFinger(Finger.FingerType fingerType, bool isLeftHand, float force)
    {
        var hand = isLeftHand ? touchFingersLeft : touchFingersRight;
        var finger = hand[(int)fingerType];
        finger.forces.Add(force);
    }

    private void CheckForceBinary()
    {
        var intHands = manager.interactionControllers
                        .Query()
                        .Select(controller => controller.intHand)
                        .Where(intHand => intHand != null);
        //.Select(intHand => intHand.leapHand);

        foreach (var intHand in intHands)
        {
            var hand = intHand.leapHand;

            if (intHand.isTracked) // If hand is tracked, check for overlaping colliders
            {
                foreach (var finger in hand.Fingers)
                {
                    var fingertipPosition = finger.TipPosition.ToVector3();
                    var buffer = Physics.OverlapSphere(fingertipPosition, CONTACT_POINT_SIZE);
                    //Debug.DrawLine(fingertipPosition, Vector3.forward, Color.red, 1.0f);

                    var colliders = new List<Collider>();
                    foreach (var col in buffer)
                    {
                        if (col.gameObject.layer != controllerLayer) colliders.Add(col);
                        if (col.gameObject.layer == groundLayer) AutoHeight.OnHandContact();
                    }

                    float force = 0;
                    if (colliders.Count > 0)
                    {
                        force = 100;
                    }
                    UpdateFinger(finger.Type, hand.IsLeft, force);
                }
            }
            else // If hand is not tracked, set all forces to 0
            {
                if (hand.Fingers[3].Type == Finger.FingerType.TYPE_RING) // If hand was at least once on the scene and fingers are properly initialized
                {
                    foreach (var finger in hand.Fingers)
                    {
                        UpdateFinger(finger.Type, hand.IsLeft, 0);
                    }
                }
            }
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------


    private void CheckForce()
    {
        var intHands = manager.interactionControllers
                        .Query()
                        .Select(controller => controller.intHand)
                        .Where(intHand => intHand != null);

        foreach (var intHand in intHands)
        {
            var hand = intHand.leapHand;

            if (intHand.isTracked)
            {
                foreach (var finger in hand.Fingers)
                {
                    var fingertipPosition = finger.TipPosition.ToVector3();
                    var buffer = Physics.OverlapSphere(fingertipPosition, CONTACT_POINT_SIZE);
                    //Debug.DrawLine(fingertipPosition, Vector3.forward, Color.red, 1.0f);

                    var colliders = new List<Collider>();
                    foreach (var col in buffer)
                    {
                        if (col.gameObject.layer != controllerLayer) colliders.Add(col);
                        if (col.gameObject.layer == groundLayer) AutoHeight.OnHandContact();
                    }

                    foreach (var col in colliders)
                    {
                        RaycastHit[] hits;
                        Vector3 origin = fingertipPosition;
                        Vector3 destination = col.bounds.center;
                        hits = Physics.RaycastAll(origin, destination - origin, Mathf.Infinity);

                        int numHits = 0;
                        foreach (RaycastHit hitInfo in hits)
                        {
                            if (hitInfo.collider == col)
                            {
                                numHits++;
                            }
                        }

                        if (numHits == 0) // If object's collider wasn't hit (which means finger is inside of the object)
                        {
                            float distance = 0;
                            if (col is MeshCollider)
                            {
                                distance = (Physics.ClosestPoint(fingertipPosition,
                                                                     col,
                                                                     col.attachedRigidbody.position,
                                                                     col.attachedRigidbody.rotation)
                                                - fingertipPosition).magnitude;
                            }
                            else
                            {
                                distance = (col.transform.TransformPoint(
                                            col.ClosestPointOnSurface(
                                            col.transform.InverseTransformPoint(fingertipPosition)))
                                  - fingertipPosition).magnitude;
                            }

                            UpdateFinger(finger.Type, hand.IsLeft, distance * 1500);//(600 / Vector3.Magnitude(transform.lossyScale)));
                        }
                    }
                }
            }
            else // If hand is not tracked, set all forces to 0
            {
                if (hand.Fingers[3].Type == Finger.FingerType.TYPE_RING) // If hand was at least once on the scene and fingers are properly initialized
                {
                    foreach (var finger in hand.Fingers)
                    {
                        UpdateFinger(finger.Type, hand.IsLeft, 0);
                    }
                }
            }
        }

    }


    /*
    private InteractionManager manager;

    private Collider objCollider;
    private Mesh mesh;

    //private Collider[] _collidersBuffer = new Collider[16];
    private float _fingertipRadius = 0.01f; // 1 cm
    private float maxDistance = 0.2f;

    private void Start()
    {
        objCollider = this.GetComponent<Collider>();
        mesh = this.GetComponent<MeshFilter>().mesh;
        manager = InteractionManager.instance;
    }
    
    void Update()
    {
        foreach (var interactionHand in manager.interactionControllers
                                             .Query()
                                             .Select(controller => controller.intHand)
                                             .Where(intHand => intHand != null)
                                             .Select(intHand => intHand.leapHand))
        {

            foreach (var finger in interactionHand.Fingers)
            {
                var fingertipPosition = finger.TipPosition.ToVector3();

                    RaycastHit[] hits;
                    Vector3 origin = fingertipPosition;
                    Vector3 destination = objCollider.bounds.center;
                    hits = Physics.RaycastAll(origin, destination - origin, Mathf.Infinity);

                    int numHits = 0;
                    foreach (RaycastHit hitInfo in hits)
                    {
                        if (hitInfo.collider == objCollider)
                        {
                            numHits++;
                        }
                    }


                    if (numHits == 0)
                    {
                        Vector3 closestPoint = NearestVertexTo(fingertipPosition);
                        float distance = Vector3.Distance(fingertipPosition, closestPoint);

    float distance = (objCollider.transform.TransformPoint(
            objCollider.ClosestPointOnSurface(
              objCollider.transform.InverseTransformPoint(fingertipPosition)))
          - fingertipPosition).magnitude;

                        if (finger.Type == Finger.FingerType.TYPE_PINKY)
                    { 
                        Debug.Log(Vector3.Magnitude(transform.lossyScale));
                        Debug.DrawRay(origin, destination - origin, Color.red,1f);
                        //Debug.Log(Vector3.Distance(fingertipPosition, transform.position));
                    }


                    TouchDetector.UpdateFinger(finger.Type, interactionHand.IsLeft, distance * 1000);//(600 / Vector3.Magnitude(transform.lossyScale)));
                    }

    // If the distance from the fingertip and the object is less
    // than the 'fingertip radius', the fingertip is touching the object.
    if (intObj.GetHoverDistance(fingertipPosition) < _fingertipRadius)
    {
        //Debug.Log("Found collision for hand:" + contactingHand.IsLeft + "," + finger.HandId + ", fingertip: " + finger.Type);
        TouchDetector.UpdateFinger(finger.Type, contactingHand.IsLeft, true);
    }
    else
    {
        TouchDetector.UpdateFinger(finger.Type, contactingHand.IsLeft, false);
    }
    if (finger.Type == Finger.FingerType.TYPE_INDEX)
        Debug.Log(intObj.GetHoverDistance(fingertipPosition));

                
            }
        }
    }

    public Vector3 NearestVertexTo(Vector3 point)
    {
        // convert point to local space
        point = transform.InverseTransformPoint(point);


        float minDistanceSqr = Mathf.Infinity;
        Vector3 nearestVertex = Vector3.zero;
        // scan all vertices to find nearest
        foreach (Vector3 vertex in mesh.vertices)
        {
            Vector3 diff = point - vertex;
            float distSqr = diff.sqrMagnitude;
            if (distSqr < minDistanceSqr)
            {
                minDistanceSqr = distSqr;
                nearestVertex = vertex;
            }
        }
        // convert nearest vertex back to world space
        return transform.TransformPoint(nearestVertex);

    }
    */
}

