using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using DebugDrawingExtension;

[ExecuteInEditMode]
public class DrawColliders : MonoBehaviour
{
    public bool AlwaysShowCollider = true;
    public bool IncludeCollidersInChildren = true;
    public Color GizmoColor = Color.red;
    private Collider[] _colliders;
    private BoxCollider _boxTemp;
    private SphereCollider _sphereTemp;
    private CapsuleCollider _capsuleTemp;
    private Color _prevColor;

    void OnDrawGizmos()
    {
        _colliders = IncludeCollidersInChildren ? GetComponentsInChildren<Collider>() : GetComponents<Collider>();

        if (_colliders == null || _colliders.Length == 0)
            return;

        if (AlwaysShowCollider)
        {
            _prevColor = Gizmos.color;
            Gizmos.color = GizmoColor;

            Collider _temp;
            for (int i = 0; i < _colliders.Length; i++)
            {
                _temp = _colliders[i];
                if (!_temp.enabled)
                    continue;
                if ((_boxTemp = _temp as BoxCollider) != null)
                {
                    Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

                    Gizmos.matrix = Matrix4x4.TRS(_boxTemp.transform.TransformPoint(_boxTemp.center), _boxTemp.transform.rotation, _boxTemp.transform.lossyScale);
                    Gizmos.DrawWireCube(Vector3.zero, _boxTemp.size);

                    Gizmos.matrix = oldGizmosMatrix;
                }
                else if ((_sphereTemp = _temp as SphereCollider) != null)
                {
                    Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

                    Gizmos.matrix = Matrix4x4.TRS(_sphereTemp.transform.TransformPoint(_sphereTemp.center), _sphereTemp.transform.rotation, Vector3.one *
                        Mathf.Max(Mathf.Abs(_sphereTemp.transform.lossyScale.x), Mathf.Max(Mathf.Abs(_sphereTemp.transform.lossyScale.y), Mathf.Abs(_sphereTemp.transform.lossyScale.z))));
                    Gizmos.DrawWireSphere(Vector3.zero, _sphereTemp.radius);

                    Gizmos.matrix = oldGizmosMatrix;
                }
                else if ((_capsuleTemp = _temp as CapsuleCollider) != null)
                {
                    Vector3 ls = _capsuleTemp.transform.lossyScale;
                    var centerHalfScale = new Vector3(_capsuleTemp.center.x * ls.x, _capsuleTemp.center.y * ls.y, _capsuleTemp.center.z * ls.z);
                    float halfHeight = _capsuleTemp.height * Mathf.Abs(ls.y) * 0.5f;
                    var directionVector = _capsuleTemp.transform.up;

                    // direction == 0 is the X-axis
                    if (_capsuleTemp.direction == 0)
                        directionVector = (Quaternion.AngleAxis(90, Vector3.forward) * Vector3.up);
                    // direction == 1 is the Y-axis
                    else if (_capsuleTemp.direction == 1)
                        directionVector = (Quaternion.AngleAxis(90, Vector3.up) * Vector3.up);
                    // direction == 2 is the Z-axis
                    else if (_capsuleTemp.direction == 2)
                        directionVector = (Quaternion.AngleAxis(90, Vector3.right) * Vector3.up);

                    Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
                    Gizmos.matrix = Matrix4x4.TRS(_capsuleTemp.transform.position, _capsuleTemp.transform.rotation, Vector3.one);
                    Vector3 capsuleBoundsStart = centerHalfScale + directionVector * halfHeight;
                    Vector3 capsuleBoundsEnd = centerHalfScale - directionVector * halfHeight;

                   /* DebugExtension.DrawCapsule(capsuleBoundsStart, capsuleBoundsEnd, GizmoColor,
                        _capsuleTemp.radius * Mathf.Max(Mathf.Abs(ls.x), Mathf.Abs(ls.z)) // scale radius by: capsule radius, multiplied by the largest of X and Z lossyScales
                        );*/

                    Gizmos.matrix = oldGizmosMatrix;
                }
            }

            Gizmos.color = _prevColor;
        }
    }
}
