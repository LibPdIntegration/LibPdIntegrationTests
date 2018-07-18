using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CopyLibPdInstance : MonoBehaviour {
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
