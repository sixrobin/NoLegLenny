namespace JuiceJam
{
    using UnityEngine;

    public struct ExplosionData
    {
        public Vector3 Source;
        public float Force;
        public float ForceMin;
        public float Radius;

        public float ComputeForceAtPosition(Vector3 position)
        {
            float radiusPercentage = 1f - RSLib.Maths.Maths.ComputeBase1Percentage(Vector3.Distance(position, Source), Radius);
            radiusPercentage = Mathf.Clamp01(radiusPercentage);
            return Mathf.Max(ForceMin, radiusPercentage * Force); 
        }

        public Vector3 ComputeRelativeDirection(Vector3 position)
        {
            return (position - Source).normalized;
        }
    }
}