using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forcer : MonoBehaviour
{
    private Vector3 forcePosition = Vector3.zero;
    private float forceRadius = 0f;
    public Vector2 forceRadiusRange = new Vector2(0f, 4f);
    private Vector3 force = Vector3.zero;
    private float timeLeft = 0f;
    public float ForceStrength = 20f;
    public float maxTime = 2f;
    private bool isRunning = false;

    [SerializeField] private List<GrassTest> _grassTests = new List<GrassTest>();
    [SerializeField] private List<GrassTest> _triggeredGrass = new List<GrassTest>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            timeLeft = maxTime;
            forcePosition = GetMouseWorld();
            isRunning = true;
        }
        
        forceRadius = Mathf.Lerp(forceRadiusRange.x, forceRadiusRange.y, easeOut(1f - (timeLeft / maxTime)));

        if (isRunning)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                forceRadius = 0f;
                timeLeft = 0;
                isRunning = false;
                _triggeredGrass.Clear();
            }
        }
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isRunning)
        {
            foreach (GrassTest grass in _grassTests)
            {
                if (!_triggeredGrass.Contains(grass))
                {
                    float distFrom = Vector3.Distance(grass.transform.position, forcePosition);
                    if (distFrom <= forceRadius)
                    {
                        grass.ApplyForce((grass.transform.position - forcePosition).normalized * ForceStrength);
                        _triggeredGrass.Add(grass);
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(forcePosition,0.3f);
        
        Gizmos.DrawWireSphere(forcePosition, forceRadius);
        
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(forcePosition, forceRadiusRange.y);
    }

    private Vector3 GetMouseWorld()
    {
        Plane plane = new Plane(Vector3.up, 0);
        
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }
        
        return Vector3.zero;
    }

    private float easeOut(float x)
    {
        return 1f - Mathf.Pow(1f - x, 3f);
    }
}
