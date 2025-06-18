using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder {
    public static List<Vector3> BuildPath(Vector3 start, Vector3 end,float evadeDistance, float stopDistance = 0.5f,  int maxSteps = 10) {
        List<Vector3> path = new();
        Vector3 current = start;
        path.Add(start);

        for (int i = 0; i < maxSteps; i++) {
            Vector3 toTarget = end - current;
            float distance = toTarget.magnitude;

            if (distance <= stopDistance) {
                break;
            }

            Vector3 direction = toTarget.normalized;
            float rayLength = distance - stopDistance;

            if (!Physics.Raycast(current, direction, out RaycastHit hit, rayLength)) {
                Vector3 stopPoint = end - direction * stopDistance;
                path.Add(stopPoint);
                break;
            }

            Vector3 right = Vector3.Cross(Vector3.up, direction);
            current = hit.point + right.normalized * evadeDistance;
            path.Add(current);
        }

        path.Add(end);
        return path;
    }
}
