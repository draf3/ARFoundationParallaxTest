using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Parallax
{
    [ExecuteInEditMode]
    public class OffAxisProjectionPortrait : MonoBehaviour
	{
        [SerializeField] private Camera deviceCamera, eyeCamera, arCamera;
        [SerializeField] private float left, right, bottom, top, near, far;
		[SerializeField] private Vector2 moveAmount = new Vector2(2f, 2f);

        private GameObject arFaceObj;
        private ARFace arFace;

        private void LateUpdate()
        {
            if (deviceCamera == null ||
				eyeCamera == null ||
                arCamera == null)
				return;

            Quaternion q = deviceCamera.transform.rotation * Quaternion.Euler(Vector3.up * 180f);
            eyeCamera.transform.rotation = q;

            if (arFaceObj == null || arFace == null) {
                try
                {
                    arFaceObj = GameObject.FindWithTag("ARFace");
                    arFace = arFaceObj.GetComponent<ARFace>();
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning(e);
                }
            }
            else
            {
                Vector2 eyePos = arCamera.WorldToViewportPoint(arFace.leftEye.position);
                eyePos.x -= 0.5f;
                eyePos.y -= 0.5f;
                eyePos.x = Mathf.Clamp(eyePos.x, -moveAmount.x, moveAmount.x);
                eyePos.y = Mathf.Clamp(eyePos.y, -moveAmount.y, moveAmount.y);
                eyeCamera.transform.localPosition = new Vector3(-eyePos.x, eyePos.y, 0f);
            }

            Vector3 deviceCamPos = eyeCamera.transform.worldToLocalMatrix.MultiplyPoint(deviceCamera.transform.position);
			Vector3 fwd = eyeCamera.transform.worldToLocalMatrix.MultiplyVector (deviceCamera.transform.forward); 
			var devicePlane = new Plane(fwd, deviceCamPos);
			Vector3 close = devicePlane.ClosestPointOnPlane(Vector3.zero);
			near = close.magnitude;

			// iPhoneのサイズを設定する
			left = deviceCamPos.x - 0.040f;
			right = deviceCamPos.x + 0.022f;
			top = deviceCamPos.y + 0.000f;
			bottom = deviceCamPos.y - 0.135f;
			far = 10f;

			float scale_factor = 0.01f / near;
			near *= scale_factor;
			left *= scale_factor;
			right *= scale_factor;
			top *= scale_factor;
			bottom *= scale_factor;

			Matrix4x4 m = PerspectiveOffCenter(left, right, bottom, top, near, far);
			eyeCamera.projectionMatrix = m;
		}

		private static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
		{
			float x = 2.0f * near / (right - left);
			float y = 2.0f * near / (top - bottom);
			float a = (right + left) / (right - left);
			float b = (top + bottom) / (top - bottom);
			float c = -(far + near) / (far - near);
			float d = -(2.0f * far * near) / (far - near);
			float e = -1.0f;
			var m = new Matrix4x4();
			m[0, 0] = x; m[0, 1] = 0; m[0, 2] = a; m[0, 3] = 0;
			m[1, 0] = 0; m[1, 1] = y; m[1, 2] = b; m[1, 3] = 0;
			m[2, 0] = 0; m[2, 1] = 0; m[2, 2] = c; m[2, 3] = d;
			m[3, 0] = 0; m[3, 1] = 0; m[3, 2] = e; m[3, 3] = 0;
			return m;
		}
	}
}