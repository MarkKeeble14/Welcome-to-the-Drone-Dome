using System.Collections;
using UnityEngine;

public abstract class MortarProjectile : MonoBehaviour
{
    [SerializeField] private LayerMask explodeOnCollideWith;

    public void Set(Transform shootAt, float speed, float arcHeight)
    {
        StartCoroutine(Travel(shootAt, speed, arcHeight));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!LayerMaskHelper.IsInLayerMask(collision.gameObject, explodeOnCollideWith)) return;
        ArrivedAtPosition();
    }

    public abstract void ArrivedAtPosition();

    public IEnumerator Travel(Transform shootAt, float speed, float arcHeight)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = shootAt.position;

        Vector3 lastPos = transform.position;
        Vector3 currentPos;

        float counter = 0;
        bool direction = true;  // true = up, false = down
        bool reachedGoal = false;
        float allowedDistFromTarget = 1f;
        while (!reachedGoal)
        {
            currentPos = transform.position;

            // Checks if we've begun descending
            if (currentPos.y < lastPos.y)
            {
                direction = false;
            }
            lastPos = transform.position;

            // Limit arc going upwards
            float arcFudge = 1f;
            if (!direction)
            {
                arcFudge = 1f;
            }

            // Lerp X and Z change
            Vector3 landPos = new Vector3(targetPos.x, transform.position.y, targetPos.z);
            // Move the transform on X and Z axis
            transform.position = Vector3.Lerp(transform.position, landPos, speed * Time.deltaTime);

            // Lerp y change
            Vector3 heightPos = new Vector3(
                    transform.position.x,
                    startPos.y + (Mathf.Sin(Mathf.PI * 2 * counter / 360)
                        * ((Mathf.Abs(startPos.y - targetPos.y) * arcFudge * arcHeight))),
                    transform.position.z
             );
            // Move the transform on Y axis
            transform.position = Vector3.Lerp(transform.position, heightPos, speed * 2f);

            // Update Counter
            counter += speed;

            if (Vector3.Distance(transform.position, targetPos) < allowedDistFromTarget)
                reachedGoal = true;

            yield return null;
        }

        ArrivedAtPosition();
    }
}
