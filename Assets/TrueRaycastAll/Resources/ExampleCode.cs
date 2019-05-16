using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using System.Linq;
#endif
using UnityEngine;

public class ExampleCode : MonoBehaviour {

	// decal settings
	[Header("Decal Settings")]
	public int decalLimit = 50;
	public GameObject decalPrefab;
	private List<GameObject> decals = new List<GameObject>();

	[Header("Weapon Settings")]
	public float power = 100.0f;
	public int penetrationRate = 3;

	[Header("Camera Settings")]
	public Transform cam;
	public int currentPos;
	public List<Transform> points = new List<Transform>();

	[Header("Debug")]
	public bool debugDrawLines = true;
	public List<DebugLine> debugLines = new List<DebugLine>();

	[System.Serializable]
	public class DebugLine {
		public List<Vector3> points = new List<Vector3>();

		public DebugLine(List<Vector3> points) {
			this.points = points;
		}
	}

	// Update is called once per frame
	void Update() {
		cam.position = Vector3.Lerp(cam.position, points[currentPos].position, 2.0f * Time.deltaTime);
		cam.rotation = Quaternion.Lerp(cam.rotation, points[currentPos].rotation, 2.0f * Time.deltaTime);

		if (Input.GetMouseButtonDown(0)) {
			Fire();
		}
	}

	public void SetCurrentPos(float value) {
		currentPos = (int)value;
	}

	void Fire() {
		Ray ray = cam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
		List<RaycastHit> hits = TrueRaycastAll.GetAllHits(ray, cam.forward, penetrationRate);

		if (hits.Count > 0) {
			int hitIndex = 0;
			DebugLine debugLine = new DebugLine(new List<Vector3>());
			foreach (RaycastHit hit in hits) {

				GameObject instance = Instantiate(decalPrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
				instance.transform.SetParent(hit.transform);

				decals.Add(instance);
				debugLine.points.Add(hit.point);

				Rigidbody hitRb = hit.rigidbody;
				// Check if it is the object in the direction of the shot and not the return
				if (hitIndex % 2 == 0) {
					if (hitRb) {
						// Adds force only in the direction of the shot
						hitRb.AddForceAtPosition(cam.forward * power, hit.point);
					}
				}

				if (decals.Count > decalLimit) {
					Destroy(decals[0]);
					decals.RemoveAt(0);
				}
				hitIndex++;
			}
			debugLines.Add(debugLine);
		}
	}

#if UNITY_EDITOR
	private void OnDrawGizmos() {
		if (!debugDrawLines || debugLines.Count == 0) return;
		foreach (DebugLine dline in debugLines) {
			UnityEditor.Handles.color = Color.red;
			UnityEditor.Handles.DrawPolyLine(dline.points.ToArray());
			foreach (Vector3 point in dline.points) {
				UnityEditor.Handles.color = Color.blue;
				UnityEditor.Handles.DrawWireCube(point, Vector3.one / 10.0f);
			}

		}
#endif
	}
}
