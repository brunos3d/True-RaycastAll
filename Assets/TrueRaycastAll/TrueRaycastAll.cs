using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TrueRaycastAll {

	static int hitIndex;
	static bool isTheFirstRay = true;
	static List<RaycastHit> hits = new List<RaycastHit>();

	public static List<RaycastHit> GetAllHits(Vector3 origin, Vector3 direction, Vector3 forward, int hitLimit = 0, float maxDistance = Mathf.Infinity) {
		return GetAllHits(new Ray(origin, direction), forward, hitLimit, maxDistance);
	}

	public static List<RaycastHit> GetAllHits(Ray ray, Vector3 forward, int hitLimit = 0, float maxDistance = Mathf.Infinity) {
		hitIndex = 0;
		isTheFirstRay = true;
		hits = new List<RaycastHit>();
		hits.Add(GetRaycastHit(ray, forward, hitLimit, maxDistance));
		// remove the last and empty hit
		hits.RemoveAt(hits.Count - 1);
		return hits;
	}

	public static RaycastHit GetRaycastHit(Ray ray, Vector3 forward, int hitLimit = 0, float maxDistance = Mathf.Infinity) {
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, maxDistance)) {
			//if the number of hits has been exceeded
			if (hitIndex >= hitLimit) {
				return hit;
			}
			hits.Add(hit);

			//the first ray does not detect the back
			if (!isTheFirstRay) {
				Ray inverseRay = new Ray(hit.point, -ray.direction);
				RaycastHit inverseHit;
				// exit point of the hit
				if (Physics.Raycast(inverseRay, out inverseHit)) {
					hits.Add(inverseHit);
				}
			}
			isTheFirstRay = false;
			// the next position for the new origin of the next ray
			Ray newRay = new Ray(hit.point + forward / 10000.0f, ray.direction);
			hitIndex++;
			return GetRaycastHit(newRay, forward, hitLimit, maxDistance);
		}
		return hit;
	}
}
