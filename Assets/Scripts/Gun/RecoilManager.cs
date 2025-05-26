using UnityEngine;

public class RecoilManager : MonoBehaviour
{
    // Rotations
    private Vector3 currentRotation = Vector3.zero;
    private Vector3 targetRotation = Vector3.zero;

    // Recoil
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;

    // Settings
    [SerializeField] private float snapiness;
    [SerializeField] private float returnSpeed;

    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snapiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void fireRecoil()
    {
        targetRotation += new Vector3(-recoilX, Random.Range(-recoilY,recoilY), Random.Range(-recoilZ,recoilZ));
    }
}
