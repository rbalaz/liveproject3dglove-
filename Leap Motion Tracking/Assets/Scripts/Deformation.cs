using UnityEngine;

public class Deformation : MonoBehaviour
{

    // Initial scale
    private float uniformScale = 1f;
    // Mesh contains vertices that form the game object 
    private Mesh deformingMesh;
    // We need to store the original vertices to restore the shape of the object when the interaction ends
    Vector3[] originalVertices, displacedVertices;
    // Vertex velocities change during interaction
    Vector3[] vertexVelocities;
    // Trigger for update to change Mesh
    bool collisionActive;

    // Use this for initialization
    void Start()
    {
        // Mesh is aquired during initializing stage of the game
        deformingMesh = GetComponent<MeshFilter>().mesh;
        // Original object shape is stored
        originalVertices = deformingMesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];
        for (int i = 0; i < originalVertices.Length; i++)
        {
            displacedVertices[i] = originalVertices[i];
        }
        vertexVelocities = new Vector3[originalVertices.Length];
        collisionActive = false;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (collisionActive)
        {
            //Debug.Log("Updating: " + gameObject.transform.name);
            // Every update influences the current object scale
            uniformScale = transform.localScale.x;
            // Every vertix in the mesh will be updated when force is applied to object
            if (displacedVertices != null)
            {
                bool changeHappened = false;
                for (int i = 0; i < displacedVertices.Length; i++)
                {
                    if (UpdateVertex(i))
                        changeHappened = true;
                }
                if (!changeHappened)
                {
                    collisionActive = false;
                    //Debug.Log("Deformations disabled.");
                }
                // Changed vertices are reassigned to mesh to cause the object to change in the game
                deformingMesh.vertices = displacedVertices;
                deformingMesh.RecalculateNormals();
            }
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
       // Debug.Log("Collision: " + collision.transform.name);
        if (collision.transform.name.ToLower().Contains("index") ||
            collision.transform.name.ToLower().Contains("thumb") ||
            collision.transform.name.ToLower().Contains("pinky") ||
            collision.transform.name.ToLower().Contains("ring") ||
            collision.transform.name.ToLower().Contains("middle"))
        {
            float force = 10f;
            Vector3 point = collision.contacts[0].point;
            AddDeformingForce(point, force);
            //Debug.Log("Deformations active");
            collisionActive = true;
        }
    }

    public void AddDeformingForce(Vector3 point, float force)
    {
        // This ensures that the deformation is invariant to rotation or translation
        point = transform.InverseTransformPoint(point);
        // Force is applied to all vertices of the object
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            AddForceToVertex(i, point, force);
        }
    }

    void AddForceToVertex(int i, Vector3 point, float force)
    {
        // Direction vector defined by contact point and the point being displaced in the mesh
        Vector3 pointToVertex = displacedVertices[i] - point;
        // Deformation has to be invariant to scaling
        pointToVertex *= uniformScale;
        // Force needs to degrade with increasing distance from the contact point
        // Force is at full strength when distance is zero
        float attenuatedForce = force / (1f + 150*pointToVertex.sqrMagnitude);
        // Force is coverted into velocity change
        float velocity = attenuatedForce * Time.deltaTime;
        // Velocity direction
        vertexVelocities[i] += pointToVertex.normalized * velocity;
    }

    bool UpdateVertex(int i)
    {
        float springForce = 75f;
        float damping = 10f;

        Vector3 velocity = vertexVelocities[i];
        Vector3 displacement = displacedVertices[i] - originalVertices[i];
        // Deformation has to be invariant to scaling
        displacement *= uniformScale;
        // This applies a reversed force that works similar to spring
        // When the applied's force effect diminishes, the object quickly regains its original shape
        // Spring needs to be damped so the vertices eventually return to their original position
        // instead of being locked in an endless springing loop
        velocity -= displacement * springForce * Time.deltaTime;
        velocity *= 1f - damping * Time.deltaTime;
        vertexVelocities[i] = velocity;
        // Rule for changing position = velocity * deltaTime
        displacedVertices[i] += velocity * (Time.deltaTime / uniformScale);

        if (velocity.magnitude < 0.0001)
            return false;
        else
            return true;

    }
}