using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    [SerializeField] GameObject target;
    [SerializeField] GameObject startLocation;
    public float travelTime = 2f;

    public GameObject Target
    {
        get
        {
            return target;
        }

        set
        {
            target = value;
        }
    }

    public GameObject StartLocation
    {
        get
        {
            return startLocation;
        }

        set
        {
            startLocation = value;
        }
    }

    private void Start()
    {
    }

    public IEnumerator FlyProjectile()
    {

        Vector3 a, b, c = new Vector3();
        a = StartLocation.transform.position;
        c = Target.transform.position;
        b = (c + a) * 0.5f;
        b.y += 5;
        c.y += 5;
        a.y += 5;
        LookAt(Target.transform.position);
        float t = Time.deltaTime * travelTime;
        for (; t < 1f; t += Time.deltaTime * travelTime)
        {
            transform.position = Bezier.GetPoint(a, b, c, t);
            Vector3 d = Bezier.GetDerivative(a, b, c, t);
            d.y = 0f;
            //transform.localRotation = Quaternion.LookRotation(d);
            yield return null;
        }
        Destroy(gameObject);
    }

    public IEnumerator LookAt(Vector3 point)
    {
        if (HexMetrics.Wrapping)
        {
            float xDistance = point.x - transform.localPosition.x;
            if (xDistance < -HexMetrics.innerRadius * HexMetrics.wrapSize)
            {
                point.x += HexMetrics.innerDiameter * HexMetrics.wrapSize;
            }
            else if (xDistance > HexMetrics.innerRadius * HexMetrics.wrapSize)
            {
                point.x -= HexMetrics.innerDiameter * HexMetrics.wrapSize;
            }
        }

        point.y = transform.localPosition.y;
        Quaternion fromRotation = transform.localRotation;
        Quaternion toRotation =
            Quaternion.LookRotation(point - transform.localPosition);
        float angle = Quaternion.Angle(fromRotation, toRotation);

        if (angle > 0f)
        {
            float speed = 540f / angle;
            for (
                float t = Time.deltaTime * speed;
                t < 1f;
                t += Time.deltaTime * speed
            )
            {
                transform.localRotation =
                    Quaternion.Slerp(fromRotation, toRotation, t);
                yield return null;
            }
        }

        //transform.LookAt(point);
        //Orientation = transform.localRotation.eulerAngles.y;
    }
}
