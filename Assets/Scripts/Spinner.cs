using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float rotationSpeed;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    public void OnMouseDown()
    {
        GeospatialManager.Instance.OnCoinClick();
    }
}
