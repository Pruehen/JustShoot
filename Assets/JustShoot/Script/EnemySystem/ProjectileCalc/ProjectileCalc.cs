using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProjectileCalc
{

    public static Vector3 CalculateInitialVelocity(Transform target, Transform origin, float initialSpeed, Vector3 offset)
    {
        return (-origin.position + (target.position + offset)).normalized * initialSpeed;
    }

    public static Vector3 CalcLaunch(Vector3 origin, Vector3 target, float angle)
    {
        float g = Physics.gravity.magnitude;
        Vector3 dirToTarget = -origin + target;
        float distH = new Vector3(dirToTarget.x,0f,dirToTarget.z).magnitude;
        float distV = dirToTarget.y;
        float D = distH + distV;
        float angleV = angle * Mathf.Deg2Rad;

        float v0 = Mathf.Sqrt((D * g) / (Mathf.Sin(2f * angleV))); 
        

        dirToTarget.y = 0f;
        dirToTarget = dirToTarget.normalized;

        Quaternion lookRotation = Quaternion.LookRotation(dirToTarget);
        Quaternion additionalAngle = Quaternion.Euler(angle, 0f, 0f);
        Vector3 lauchDir=(lookRotation*additionalAngle) * Vector3.forward;
        lauchDir.y = -lauchDir.y;
        Vector3 res = lauchDir *v0;
        if(float.IsNaN(res.x))
            return Vector3.zero;
        return res;
    }

    //chatGpt가 만들어준거 약간 수정

    //근데 타겟과 거리 계산에서 움직인 이후의 거리와 투사체 교차 시간이 서로 필요해서 약간의 오차는 있을듯
    //public static Vector3 CalculateInitialVelocity(Transform target, Transform origin, float initialSpeed, Vector3 offset)
    //{
    //    // Get target's position and velocity
    //    Vector3 targetPosition = target.position + offset;
    //    Vector3 originPosition = origin.position;
    //    Vector3 targetVelocity = target.GetComponent<CharacterController>().velocity;

    //    // Calculate time to intercept with gravity
    //    float timeToIntercept = CalculateTimeToIntercept(targetPosition, originPosition, initialSpeed);

    //    // Calculate intercept point
    //    Vector3 interceptPoint = CalculateInterceptPoint(targetPosition, targetVelocity, timeToIntercept);

    //    // Calculate direction to intercept point with gravity
    //    Vector3 directionToIntercept = CalculateDirectionWithGravity(originPosition, interceptPoint, initialSpeed, timeToIntercept);

    //    // Set velocity magnitude to initialSpeed
    //    Vector3 velocity = directionToIntercept.normalized * initialSpeed;

    //    return velocity;
    //}

    //private static float CalculateTimeToIntercept(Vector3 targetPosition, Vector3 originPosition, float initialSpeed)
    //{
    //    // Calculate relative position
    //    Vector3 relativePosition = targetPosition - originPosition;

    //    // Calculate horizontal and vertical distances
    //    float horizontalDistance = new Vector3(relativePosition.x, 0f, relativePosition.z).magnitude;
    //    float verticalDistance = relativePosition.y;

    //    // Calculate time to intercept considering horizontal distance
    //    float horizontalTimeToIntercept = horizontalDistance / initialSpeed;

    //    // Calculate time to intercept considering vertical distance (with gravity)
    //    float verticalTimeToIntercept = Mathf.Sqrt(2f * verticalDistance / -Physics.gravity.y);

    //    // Calculate total time to intercept
    //    float timeToIntercept = Mathf.Max(horizontalTimeToIntercept, verticalTimeToIntercept);

    //    return timeToIntercept;
    //}
    //private static Vector3 CalculateInterceptPoint(Vector3 targetPosition, Vector3 targetVelocity, float timeToIntercept)
    //{
    //    // Predict target's future position
    //    Vector3 predictedPosition = targetPosition + targetVelocity * timeToIntercept;

    //    return predictedPosition;
    //}

    //private static Vector3 CalculateDirectionWithGravity(Vector3 originPosition, Vector3 interceptPoint, float initialSpeed, float timeToIntercept)
    //{
    //    // Calculate direction considering gravity
    //    Vector3 displacement = interceptPoint - originPosition;
    //    Vector3 horizontalDirection = new Vector3(displacement.x, 0f, displacement.z).normalized;

    //    // Calculate vertical speed
    //    float verticalSpeed = Physics.gravity.magnitude * timeToIntercept;

    //    // Combine horizontal and vertical directions while maintaining initial magnitude
    //    Vector3 directionToIntercept = (horizontalDirection * initialSpeed + Vector3.up * verticalSpeed).normalized;

    //    return directionToIntercept;
    //}
}
