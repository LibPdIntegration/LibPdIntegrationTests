// CircleMotion.cs - Script used to move an object in a circle.
// -----------------------------------------------------------------------------
// Copyright (c) 2019 Niall Moody
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// -----------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Script used to move an object in a circle.
public class CircleScript : MonoBehaviour
{

	// Our PD spatialise test patch.
	public LibPdInstance spatialisePatch;

	// The Transform of the sphere we're going to move.
	public Transform sphereTransform;

	// Used to move the sphere.
	float index;
	// True if the sphere is moving.
	bool moving;
	
	// Update is called once per frame
	void Update ()
	{
		if(moving)
		{
			Vector3 tempPos = new Vector3(Mathf.Sin(index) * 10.0f, 0.0f, -10.0f - Mathf.Cos(index) * 10.0f);

			sphereTransform.position = tempPos;

			index += 0.01f;
			if(index > (Mathf.PI * 2.0f))
			{
				moving = false;
				index = 0.0f;
				spatialisePatch.SendFloat("level", 0.0f);
				spatialisePatch.SendFloat("toggle", 01.0f);
			}
		}
	}

	// Call this to trigger the spatialised sound to circle round the origin.
	public void Trigger()
	{
		moving = true;
		spatialisePatch.SendFloat("level", 1.0f);
		spatialisePatch.SendFloat("toggle", 1.0f);
	}
}
