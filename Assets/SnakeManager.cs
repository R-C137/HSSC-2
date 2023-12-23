using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeManager : MonoBehaviour
{
    [SerializeField] float distanceBetween = .2f;
    [SerializeField] float speed = 280;
    [SerializeField] float turnSpeed = 180;
    [SerializeField] List<GameObject> bodyParts = new List<GameObject>();
    [SerializeField] List<GameObject> snakeBody = new List<GameObject>();

    float countUp = 0;

    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    public Transform targetPos;
    public Transform headPos;
    bool targetInCone, turningRight;

    public Coroutine targetCoroutine;

    private void Start()
    {
        CreateBodyParts();

        targetCoroutine = StartCoroutine(FindTargetsWithDelay(0));
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();

        if (headPos == null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Vector3 dirToTarget = (targetPos.position - headPos.position).normalized;

            targetInCone = (Vector3.Angle(headPos.forward, dirToTarget) < viewAngle / 2);

            if (!targetInCone)
            {
                Vector3 relativePoint = headPos.InverseTransformPoint(targetPos.position);
                if (relativePoint.x < 0f && Mathf.Abs(relativePoint.x) > Mathf.Abs(relativePoint.z))
                    turningRight = true;
                if (relativePoint.x > 0f && Mathf.Abs(relativePoint.x) > Mathf.Abs(relativePoint.z))
                    turningRight = false;
            }
        }

    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private void FixedUpdate()
    {
        if (bodyParts.Count > 0)
        {
            CreateBodyParts();
        }

        if (headPos != null) SnakeMovement();
    }

    void SnakeMovement()
    {
        snakeBody[0].GetComponent<Rigidbody>().velocity = snakeBody[0].transform.forward * speed * Time.deltaTime;

        if (!turningRight)
            snakeBody[0].transform.Rotate(new Vector3(0, turnSpeed * Time.deltaTime * 1, 0));
        else
            snakeBody[0].transform.Rotate(new Vector3(0, turnSpeed * Time.deltaTime * -1, 0));

        if (snakeBody.Count > 1)
        {
            for (int i = 1; i < snakeBody.Count; i++)
            {
                TrainToyTrap markM = snakeBody[i - 1].GetComponent<TrainToyTrap>();
                snakeBody[i].transform.position = markM.markerList[0].position;
                snakeBody[i].transform.rotation = markM.markerList[0].rotation;
                markM.markerList.RemoveAt(0);
            }
        }
    }

    void CreateBodyParts()
    {
        if (snakeBody.Count == 0)
        {
            GameObject temp1 = Instantiate(bodyParts[0], transform.position, transform.rotation, transform);

            if (!temp1.GetComponent<TrainToyTrap>())
                temp1.AddComponent<TrainToyTrap>();

            if (!temp1.GetComponent<Rigidbody>())
            {
                temp1.AddComponent<Rigidbody>();
                temp1.GetComponent<Rigidbody>().useGravity = false;
            }

            headPos = temp1.transform;
            snakeBody.Add(temp1);
            bodyParts.RemoveAt(0);
        }

        TrainToyTrap markM = snakeBody[snakeBody.Count - 1].GetComponent<TrainToyTrap>();

        if (countUp == 0)
        {
            markM.ClearMarkerList();

        }

        countUp += Time.deltaTime;

        if (countUp >= distanceBetween)
        {
            GameObject temp = Instantiate(bodyParts[0], transform.position, transform.rotation, transform);

            if (!temp.GetComponent<TrainToyTrap>())
                temp.AddComponent<TrainToyTrap>();

            if (!temp.GetComponent<Rigidbody>())
            {
                temp.AddComponent<Rigidbody>();
                temp.GetComponent<Rigidbody>().useGravity = false;
            }

            snakeBody.Add(temp);
            bodyParts.RemoveAt(0);
            temp.GetComponent<TrainToyTrap>().ClearMarkerList();
            countUp = 0;
        }
    }
}
