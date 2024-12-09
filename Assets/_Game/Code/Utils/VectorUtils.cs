using UnityEngine;

public static class VectorUtils
{
    public static Vector3 Unlerp(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 ret;
        ret.x = a.x != b.x ? (value.x - a.x) / (b.x - a.x) : 0.0f;
        ret.y = a.y != b.y ? (value.y - a.y) / (b.y - a.y) : 0.0f;
        ret.z = a.z != b.z ? (value.z - a.z) / (b.z - a.z) : 0.0f;
        return ret;
    }
    
    public static Vector3 Lerp(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 ret;
        ret.x = a.x + (b.x - a.x) * value.x;
        ret.y = a.y + (b.y - a.y) * value.y;
        ret.z = a.z + (b.z - a.z) * value.z;
        return ret;
    }
}
