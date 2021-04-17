using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BikeController : MonoBehaviour
{
    [Header("General settings")]
    public GameObject centerOfMass;

    [Header("Hover settings")]
    [SerializeField]
    private float _hoverHeight = 1.5f;
    [SerializeField]
    private float _hoverForce = 250f;
    [SerializeField]
    private float _dampening = 1.0f;
    public GameObject[] _hoverEngines;

    [Header("Propulsor settings")]
    [SerializeField]
    private Vector3 _propulsorForwardForceDir = new Vector3(0, 0.5f, 0.5f);
    [SerializeField]
    private float _propulsorForwardForce = 1000f;
    [SerializeField]
    private float _propulsorTurnForce = 150f;
    [SerializeField]
    private float _boostParticlesLifetime = 6.09f;
    public GameObject propulsor;

    private Rigidbody _rb;
    private float _horizontalInput, _verticalInput;
    private float[] _lastHitDistances = new float[4];
    private float[] _currentHoverForce = new float[4];

    public ParticleSystem boost;

    void Start()
    {
        Cursor.visible = false;
        _rb = GetComponent<Rigidbody>();

        // The center of mass of the hover craft is below it to improve stability 
        _rb.centerOfMass = centerOfMass.transform.localPosition;

        // The propulsor force dir is normalized because its strength comes from _propulsorForwardForce
        _propulsorForwardForceDir.Normalize();

        boost.Stop();
    }

    private void Update()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        CalculatePropulsion();
        CalculateHover();
    }

    private void CalculatePropulsion()
    {
        // To move forward we apply a force at the propulsor position
        _rb.AddForceAtPosition(Time.fixedDeltaTime * transform.TransformDirection(_propulsorForwardForceDir)
         * _verticalInput * _propulsorForwardForce, 
         propulsor.transform.position);

        // To rotate we add a torque
        _rb.AddTorque(Time.fixedDeltaTime * transform.TransformDirection(Vector3.up) * _horizontalInput * _propulsorTurnForce);
        
        // If there is any forward input then show the boost particles
        if (_verticalInput > 0)
            boost.Play();
        else
            boost.Stop();
    }

    private void CalculateHover()
    {
        RaycastHit hit;

        int currHitDistanceIdx = 0;

        // We iterate over each of the hover engines and cast a raycast down from their position,
        // the hit distance is then used to calculate the force that causes the hovercraft to float
        foreach (GameObject hoverEngine in _hoverEngines)
        {
            Debug.DrawRay(hoverEngine.transform.position, hoverEngine.transform.TransformDirection(Vector3.down) * _hoverHeight, Color.green.linear);

            if (Physics.Raycast(hoverEngine.transform.position, hoverEngine.transform.TransformDirection(Vector3.down), out hit, _hoverHeight))
            {
                // Adding the hover force in the up direction in world space at the hover position
                _rb.AddForceAtPosition(transform.up
                 * HoverForce(hit.distance, currHitDistanceIdx), 
                 hoverEngine.transform.position);
                _lastHitDistances[currHitDistanceIdx] = hit.distance;
            }
            else
            {
                _lastHitDistances[currHitDistanceIdx] = _hoverHeight;
            }
            ++currHitDistanceIdx;
        }
    }

    private float HoverForce(float hitDistance, int hitDistanceIdx)
    {
        // As the hovercraft gets closer to the ground the hover forces increases
        // The distance to the ground is the raycast hitdistance
        float hoverForce = _hoverForce * (_hoverHeight - hitDistance) / _hoverHeight;
        
        // We then use Hooke's law to calculate a dampen force so that it bounces less
        float dampeningForce = _dampening * (_lastHitDistances[hitDistanceIdx] - hitDistance);
        return hoverForce + dampeningForce;
    }
}
