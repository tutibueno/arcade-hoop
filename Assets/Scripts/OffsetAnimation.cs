using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetAnimation : MonoBehaviour {

	public Vector2 offsetSpeed;

	Vector2 offset;

	Material mat;

	// Use this for initialization
	void Start () {
		mat = GetComponent<Renderer> ().material;
	}
	
	// Update is called once per frame
	void Update () {

		offset = mat.mainTextureOffset;

		offset += offsetSpeed;

		mat.mainTextureOffset = offset;
	}
}
