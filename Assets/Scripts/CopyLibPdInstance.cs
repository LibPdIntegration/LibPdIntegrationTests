// CopyLibPdInstance.cs - Script to ensure changes to LibPdInstance get
//						  automatically copied to our sister projects, as Unity
//						  doesn't let us keep a central shared code repository.
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

using UnityEngine;
using UnityEditor;

public class CopyLibPdInstance : MonoBehaviour
{
	#if UNITY_EDITOR

	/// We use this to ensure any changes to LibPdInstance get copied across to
	/// our main repository, and our Unity example project.
	///	(Unity requires all scripts to be within their project directory, which
	///  causes problems for us, as we are using a single script across multiple
	//   projects)
	[UnityEditor.Callbacks.DidReloadScripts]
	private static void OnScriptsReloaded()
	{
		Debug.Log("Scripts reloaded. Copying LibPdInstance across to sister projects.");

		//Copy to LibPdIntegration project.
		FileUtil.ReplaceFile("Assets/Scripts/LibPdInstance.cs",
							 "../LibPdIntegration/Assets/Scripts/LibPdInstance.cs");

		//Copy to LibPdIntegrationExamples project.
		FileUtil.ReplaceFile("Assets/Scripts/LibPdInstance.cs",
							 "../LibPdIntegrationExamples/Assets/Scripts/LibPdInstance.cs");
	}

	#endif
}
