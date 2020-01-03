using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HaloParticle {
	public HaloParticle(float r = 0, float a = 0) {
		radius = r;
		angle = a;
	}
	public float radius {
		get;
		set;
	}
	public float angle {
		get;
		set;
	}
}