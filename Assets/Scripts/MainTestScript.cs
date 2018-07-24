// MainTestScript.cs - Script for triggering all the tests.
// -----------------------------------------------------------------------------
// Copyright (c) 2018 Niall Moody
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

using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// Script for triggering all the tests.
public class MainTestScript : MonoBehaviour {

	#region Variables
	// Our main PD test patch.
	public LibPdInstance mainPatch;

	// The button we listen to for sending bangs to the patch.
	public Button bangButton;
	// The slider we listen to for sending floats to the patch.
	public Slider floatSlider;
	// The input field we listen to for sending symbols to the patch.
	public InputField symbolEditor;
	// The button we listen to for sending lists to the patch.
	public Button listButton;
	// The button we listen to for sending messages to the patch.
	public Button messageButton;

	// The button we listen to for sending MIDI Note Ons to the patch.
	public Button midiNoteButton;
	// The button we listen to for sending MIDI CCs to the patch.
	public Button midiCcButton;
	// The button we listen to for sending MIDI program changes to the patch.
	public Button midiProgButton;
	// The button we listen to for sending MIDI pitch bends to the patch.
	public Button midiBendButton;
	// The button we listen to for sending MIDI aftertouches to the patch.
	public Button midiAftertouchButton;
	// The button we listen to for sending MIDI polyphonic aftertouches to the patch.
	public Button midiPolyAftertouchButton;
	// The button we listen to for sending MIDI bytes to the patch.
	public Button midiByteButton;
	// The button we listen to for sending MIDI sysex messages to the patch.
	public Button midiSysexButton;
	// The button we listen to for sending MIDI realtime messages to the patch.
	public Button midiRealtimeButton;

	// The button we listen to for sending random array data to the patch.
	public Button arrayRandomButton;
	// The button we listen to for sending sine wave array data to the patch.
	public Button arraySineButton;

	// The button we listen to for triggering the spatialised sound test.
	public Button spatialiseButton;
	// The button we listen to for triggering the dynamic creation test.
	public Button dynamicCreationButton;
	// The button we listen to for triggering the automated tests.
	public Button automatedTestsButton;

	// We use this to display the list/message/etc. we're sending to the PD patch.
	public Text statusBar;

	// The Input Field we use as our output console.
	public InputField libPdReceiveConsole;

	// The LineRenderer we use to draw the libpd array.
	public LineRenderer libpdArray;

	// The CircleScript we're going to trigger for the spatialisation test.
	public CircleScript circleScript;
	// The prefab we're going to use to test dynamic creation.
	public Transform dynamicCreationPrefab;

	// Used to write test results to a text file.
	private bool writeToFile;
	// The filepath of the text file we're writing to.
	private string testFilePath;
	// The writer we use to write to our test text file.
	private StreamWriter testWriter;
	// Used for the automated tests. This is the input we sent to libpd.
	private string testInput;

	#endregion

	#region Monobehaviour methods
	//--------------------------------------------------------------------------
	// Use this for initialization
	void Start () {
		//Libpd bindings.
		mainPatch.Bind("triggerOut");
		mainPatch.Bind("floatOut");
		mainPatch.Bind("stringOut");
		mainPatch.Bind("listOut");
		mainPatch.Bind("messageOut");

		//Libpd callback functions.
		//LibPdInstance.Bang += LibPdBangReceive;
		//LibPdInstance.Float += LibPdFloatReceive;
		//LibPdInstance.Symbol += LibPdSymbolReceive;
		//LibPdInstance.List += LibPdListReceive;
		LibPdInstance.Message += LibPdMessageReceive;
		LibPdInstance.MidiNoteOn += LibPdMidiNoteReceive;
		LibPdInstance.MidiControlChange += LibPdMidiCcReceive;
		LibPdInstance.MidiProgramChange += LibPdMidiProgReceive;
		LibPdInstance.MidiPitchBend += LibPdMidiBendReceive;
		LibPdInstance.MidiAftertouch += LibPdMidiAftertouchReceive;
		LibPdInstance.MidiPolyAftertouch += LibPdMidiPolyAftertouchReceive;
		LibPdInstance.MidiByte += LibPdMidiByteReceive;

		//Unity UI registration.
		bangButton.onClick.AddListener(() => ButtonCallback(bangButton));
		floatSlider.onValueChanged.AddListener(SliderCallback);
		symbolEditor.onEndEdit.AddListener(SymbolCallback);
		listButton.onClick.AddListener(() => ButtonCallback(listButton));
		messageButton.onClick.AddListener(() => ButtonCallback(messageButton));
		midiNoteButton.onClick.AddListener(() => ButtonCallback(midiNoteButton));
		midiCcButton.onClick.AddListener(() => ButtonCallback(midiCcButton));
		midiProgButton.onClick.AddListener(() => ButtonCallback(midiProgButton));
		midiBendButton.onClick.AddListener(() => ButtonCallback(midiBendButton));
		midiAftertouchButton.onClick.AddListener(() => ButtonCallback(midiAftertouchButton));
		midiPolyAftertouchButton.onClick.AddListener(() => ButtonCallback(midiPolyAftertouchButton));
		midiByteButton.onClick.AddListener(() => ButtonCallback(midiByteButton));
		midiSysexButton.onClick.AddListener(() => ButtonCallback(midiSysexButton));
		midiRealtimeButton.onClick.AddListener(() => ButtonCallback(midiRealtimeButton));
		arrayRandomButton.onClick.AddListener(() => ButtonCallback(arrayRandomButton));
		arraySineButton.onClick.AddListener(() => ButtonCallback(arraySineButton));
		spatialiseButton.onClick.AddListener(() => ButtonCallback(spatialiseButton));
		dynamicCreationButton.onClick.AddListener(() => ButtonCallback(dynamicCreationButton));
		automatedTestsButton.onClick.AddListener(() => ButtonCallback(automatedTestsButton));
	}
	
	//--------------------------------------------------------------------------
	// Used to clean up our callback functions.
	void OnApplicationQuit() {
		//Tell LibPdInstance to forget about our various receive functions.
		//LibPdInstance.Bang -= LibPdBangReceive;
		//LibPdInstance.Float -= LibPdFloatReceive;
		//LibPdInstance.Symbol -= LibPdSymbolReceive;
		//LibPdInstance.List -= LibPdListReceive;
		LibPdInstance.Message -= LibPdMessageReceive;
		LibPdInstance.MidiNoteOn -= LibPdMidiNoteReceive;
		LibPdInstance.MidiControlChange -= LibPdMidiCcReceive;
		LibPdInstance.MidiProgramChange -= LibPdMidiProgReceive;
		LibPdInstance.MidiPitchBend -= LibPdMidiBendReceive;
		LibPdInstance.MidiAftertouch -= LibPdMidiAftertouchReceive;
		LibPdInstance.MidiPolyAftertouch -= LibPdMidiPolyAftertouchReceive;
		LibPdInstance.MidiByte -= LibPdMidiByteReceive;
	}
	
	//--------------------------------------------------------------------------
	// Update is called once per frame
	void Update () {
		
	}
	#endregion
	
	#region UI callbacks
	//--------------------------------------------------------------------------
	// Called when we click on the bang button.
	private void ButtonCallback(Button b) {
		if(b == bangButton) {
			WriteInputText("Sent triggerIn: ", "bang");
			mainPatch.SendBang("triggerIn");
		}
		else if(b == listButton) {
			WriteInputText("Sent listIn: ", "0; 15.99; test;");
			mainPatch.SendList("listIn", 0, 15.99, "test");
		}
		else if(b == messageButton) {
			WriteInputText("Sent messageIn: ", "test 1;");
			mainPatch.SendMessage("messageIn", "test", 1);
		}
		else if(b == midiNoteButton) {
			WriteInputText("Sent MIDI Note: ", "channel = 0; note = 60; velocity = 127");
			mainPatch.SendMidiNoteOn(0, 60, 127);
		}
		else if(b == midiCcButton) {
			WriteInputText("Sent MIDI CC: ", "channel = 0; controller = 0; value = 127");
			mainPatch.SendMidiCc(0, 0, 127);
		}
		else if(b == midiProgButton) {
			WriteInputText("Sent MIDI Program Change: ", "channel = 0; program = 0");
			mainPatch.SendMidiProgramChange(0, 0);
		}
		else if(b == midiBendButton) {
			WriteInputText("Sent MIDI Pitch Bend: ", "channel = 0; value = 8191");
			mainPatch.SendMidiPitchBend(0, 8191);
		}
		else if(b == midiAftertouchButton) {
			WriteInputText("Sent MIDI Aftertouch: ", "channel = 0; value = 127");
			mainPatch.SendMidiAftertouch(0, 127);
		}
		else if(b == midiPolyAftertouchButton) {
			WriteInputText("Sent MIDI Poly Aftertouch: ", "channel = 0; note = 60; value = 127");
			mainPatch.SendMidiPolyAftertouch(0, 60, 127);
		}
		else if(b == midiByteButton) {
			WriteInputText("Sent MIDI Byte: ", "port = 0; value = 127");
			mainPatch.SendMidiByte(0, 127);
		}
		else if(b == midiSysexButton) {
			WriteInputText("Sent MIDI Sysex: ", "port = 0; value = 127");
			mainPatch.SendMidiSysex(0, 127);
		}
		else if(b == midiRealtimeButton) {
			WriteInputText("Sent MIDI Realtime: ", "port = 0; value = 250");
			mainPatch.SendMidiSysRealtime(0, 250);
		}
		else if(b == arrayRandomButton) {
			float[] tempArr = new float[] {Random.Range(-1.0f, 1.0f),
										   Random.Range(-1.0f, 1.0f),
										   Random.Range(-1.0f, 1.0f),
										   Random.Range(-1.0f, 1.0f),
										   Random.Range(-1.0f, 1.0f),
										   Random.Range(-1.0f, 1.0f),
										   Random.Range(-1.0f, 1.0f),
										   Random.Range(-1.0f, 1.0f),
										   Random.Range(-1.0f, 1.0f),
										   Random.Range(-1.0f, 1.0f)};

			string arrayData = "";
			if(writeToFile) {
				for(int i=0;i<10;++i)
					arrayData += tempArr[i].ToString() + " ";
			}

			WriteInputText("Sent random array data:", arrayData);
			mainPatch.WriteArray("TestArray", 0, tempArr, 10);

			for(int i=0;i<10;++i)
				tempArr[i] = 0.0f;
			mainPatch.ReadArray(tempArr, "TestArray", 0, 10);

			for(int i=0;i<10;++i) {
				libpdArray.SetPosition(i,
									   new Vector3((float)i * 40.0f,
												   tempArr[i] * 100.0f,
												   0.0f));
			}

			if(writeToFile) {
				arrayData = "";
				for(int i=0;i<10;++i)
					arrayData += tempArr[i].ToString() + " ";
				
				testWriter.WriteLine("Received Random array:\r\n" + arrayData);

				if(arrayData == testInput)
					AddToConsole("Random Array\t\t\t\t\t" + " <color=green>PASSED</color>");
				else
					AddToConsole("Random Array\t\t\t\t\t" + " <color=red>FAILED</color>");
			}
		}
		else if(b == arraySineButton) {
			float[] tempArr = new float[10];

			for(int i=0;i<10;++i) {
				tempArr[i] = Mathf.Sin(((float)i/9.0f) * 2.0f * Mathf.PI);
			}

			string arrayData = "";
			if(writeToFile) {
				for(int i=0;i<10;++i)
					arrayData += tempArr[i].ToString() + " ";
			}

			WriteInputText("Sent sine wave array data:", arrayData);
			mainPatch.WriteArray("TestArray", 0, tempArr, 10);

			for(int i=0;i<10;++i)
				tempArr[i] = 0.0f;
			mainPatch.ReadArray(tempArr, "TestArray", 0, 10);

			for(int i=0;i<10;++i) {
				libpdArray.SetPosition(i,
									   new Vector3((float)i * 40.0f,
												   tempArr[i] * 100.0f,
												   0.0f));
			}

			if(writeToFile) {
				arrayData = "";
				for(int i=0;i<10;++i)
					arrayData += tempArr[i].ToString() + " ";

				testWriter.WriteLine("Received Sine array:\r\n" + arrayData);

				if(arrayData == testInput)
					AddToConsole("Sine Array\t\t\t\t\t\t" + " <color=green>PASSED</color>");
				else
					AddToConsole("Sine Array\t\t\t\t\t\t" + " <color=red>FAILED</color>");
			}
		}
		else if(b == spatialiseButton) {
			statusBar.text = "Running spatialisation test";
			circleScript.Trigger();
		}
		else if(b == dynamicCreationButton) {
			if(dynamicCreationButton.GetComponentInChildren<Text>().text == "Dynamic Creation") {
				Instantiate(dynamicCreationPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);	

				dynamicCreationButton.GetComponentInChildren<Text>().text = "Dynamic Deletion";
				statusBar.text = "Instantiated LibPdInstance prefab";
			}
			else {
				GameObject obj = GameObject.Find("DynamicCreationSphere(Clone)");

				if(obj)
					Destroy(obj);

				dynamicCreationButton.GetComponentInChildren<Text>().text = "Dynamic Creation";
				statusBar.text = "Deleted LibPdInstance instance";
			}
		}
		else if(b == automatedTestsButton) {
			writeToFile = true;
			testFilePath = Path.GetFullPath(".") + "\\TestResults " + System.DateTime.Now.ToString("dd.MM.yyy - H.mm.ss") + ".txt";

			if(!File.Exists(testFilePath)) {
				testWriter = new StreamWriter(File.Create(testFilePath));
				testWriter.AutoFlush = true;
			}

			StartCoroutine("AutomatedTests");
		}
	}
	
	//--------------------------------------------------------------------------
	// Called when we drag the float slider.
	private void SliderCallback(float value) {
		WriteInputText("Sent floatIn: ", value.ToString());
		mainPatch.SendFloat("floatIn", value);
	}
	
	//--------------------------------------------------------------------------
	// Called when we enter text into the symbol editor.
	private void SymbolCallback(string value) {
		WriteInputText("Sent stringIn: ", value);
		mainPatch.SendSymbol("stringIn", value);
	}
	#endregion
	
	#region libpd callbacks
	//--------------------------------------------------------------------------
	// Called from LibPdInstance when we receive a bang.
	public void LibPdBangReceive(string name) {
		if(name == "triggerOut")
			WriteOutputText("triggerOut: ", "bang", "Bang\t\t\t\t\t\t\t\t");
	}
	
	//--------------------------------------------------------------------------
	// Called from LibPdInstance when we receive a float.
	public void LibPdFloatReceive(string name, float value) {
		if(name == "floatOut")
			WriteOutputText("floatOut: ", value.ToString(), "Float\t\t\t\t\t\t\t\t");
	}
	
	//--------------------------------------------------------------------------
	// Called from LibPdInstance when we receive a symbol.
	public void LibPdSymbolReceive(string name, string value) {
		if(name == "stringOut")
			WriteOutputText("stringOut: ", value, "Symbol\t\t\t\t\t\t\t");
	}
	
	//--------------------------------------------------------------------------
	// Called from LibPdInstance when we receive a list.
	public void LibPdListReceive(string name, object[] values) {
		if(name == "listOut") {
			string tempstr = "";

			foreach(object value in values) {
				if(tempstr.Length > 0)
					tempstr += " ";
				tempstr += value + ";";
			}

			WriteOutputText("listOut: ", tempstr, "List\t\t\t\t\t\t\t\t\t");
		}
	}
	
	//--------------------------------------------------------------------------
	// Called from LibPdInstance when we receive a message.
	void LibPdMessageReceive(string name, string symbol, object[] args) {
		if(name == "messageOut") {
			string tempstr = symbol;

			foreach(object arg in args) {
				if(tempstr.Length > 0)
					tempstr += " ";
				tempstr += arg;
			}
			tempstr += ";";

			WriteOutputText("messageOut: ", tempstr, "Message\t\t\t\t\t\t\t");
		}
	}
	
	//--------------------------------------------------------------------------
	// Called from LibPdInstance when we receive a MIDI Note.
	void LibPdMidiNoteReceive(int channel, int pitch, int velocity) {
		WriteOutputText("MIDI Note: ",
						"channel = " + channel + "; note = " + pitch + "; velocity = " + velocity,
						"MIDI Note\t\t\t\t\t\t");
	}
	
	//--------------------------------------------------------------------------
	// Called from LibPdInstance when we receive a MIDI CC.
	void LibPdMidiCcReceive(int channel, int controller, int value) {
		WriteOutputText("MIDI CC: ",
						"channel = " + channel + "; controller = " + controller + "; value = " + value,
						"MIDI CC\t\t\t\t\t\t\t");
	}
	
	//--------------------------------------------------------------------------
	// Called from LibPdInstance when we receive a MIDI program change.
	void LibPdMidiProgReceive(int channel, int program) {
		WriteOutputText("MIDI Program Change: ",
						"channel = " + channel + "; program = " + program,
						"MIDI Program Change\t");
	}
	
	//--------------------------------------------------------------------------
	// Called from LibPdInstance when we receive a MIDI pitch bend.
	void LibPdMidiBendReceive(int channel, int value) {
		WriteOutputText("MIDI Pitch Bend: ",
						"channel = " + channel + "; value = " + value,
						"MIDI Pitch Bend\t\t\t\t");
	}
	
	//--------------------------------------------------------------------------
	// Called from LibPdInstance when we receive a MIDI aftertouch.
	void LibPdMidiAftertouchReceive(int channel, int value) {
		WriteOutputText("MIDI Aftertouch: ",
						"channel = " + channel + "; value = " + value,
						"MIDI Aftertouch\t\t\t\t");
	}
	
	//--------------------------------------------------------------------------
	// Called from LibPdInstance when we receive a MIDI aftertouch.
	void LibPdMidiPolyAftertouchReceive(int channel, int note, int value) {
		WriteOutputText("MIDI Poly Aftertouch: ",
						"channel = " + channel + "; note = " + note + "; value = " + value,
						"MIDI Poly Aftertouch\t\t");
	}
	
	//--------------------------------------------------------------------------
	// Called from LibPdInstance when we receive a MIDI byte (includes sysex and realtime messages).
	void LibPdMidiByteReceive(int port, int value) {
		WriteOutputText("MIDI Byte: ",
						"port = " + port + "; value = " + value,
						"MIDI Byte\t\t\t\t\t\t");
	}
	#endregion
	
	#region Misc
	//--------------------------------------------------------------------------
	// Coroutine used to run the automated tests.
	IEnumerator AutomatedTests() {
		ButtonCallback(bangButton);
		yield return null;

		ButtonCallback(listButton);
		yield return null;

		ButtonCallback(messageButton);
		yield return null;

		SliderCallback(0.5f);
		yield return null;

		SymbolCallback("Automated test string");
		yield return null;

		ButtonCallback(midiNoteButton);
		yield return null;

		ButtonCallback(midiCcButton);
		yield return null;

		ButtonCallback(midiProgButton);
		yield return null;

		ButtonCallback(midiBendButton);
		yield return null;

		ButtonCallback(midiAftertouchButton);
		yield return null;

		ButtonCallback(midiPolyAftertouchButton);
		yield return null;

		ButtonCallback(midiByteButton);
		yield return null;

		ButtonCallback(midiSysexButton);
		yield return null;

		ButtonCallback(midiRealtimeButton);
		yield return null;

		ButtonCallback(arrayRandomButton);
		yield return null;

		ButtonCallback(arraySineButton);
		yield return null;

		ButtonCallback(spatialiseButton);
		yield return null;

		ButtonCallback(dynamicCreationButton);
		yield return new WaitForSeconds(1.0f);

		ButtonCallback(dynamicCreationButton);
		yield return null;
	}

	//--------------------------------------------------------------------------
	// Helper method. Handles the console output.
	void AddToConsole(string text) {
		//Check whether we need to add a new line.
		if(libPdReceiveConsole.text.Length > 0) {
			libPdReceiveConsole.text += "\n";
		}

		libPdReceiveConsole.text += text;

		//Check whether we need to delete the oldest line.
		int numLines = libPdReceiveConsole.text.Split('\n').Length - 1;

		if(numLines > 15) {
			string tempstr = libPdReceiveConsole.text;

			while(numLines > 15)
			{
				tempstr = tempstr.Substring(tempstr.IndexOf("\n") + 1);

				--numLines;
			}

			libPdReceiveConsole.text = tempstr;
		}
	}

	//--------------------------------------------------------------------------
	// Helper method. Writes text to the status bar and/or a test file.
	void WriteInputText(string preamble, string text) {
		if(writeToFile) 
		{
			testInput = text;

			testWriter.WriteLine(preamble + text);
		}
		else
			statusBar.text = preamble + text;
	}

	//--------------------------------------------------------------------------
	// Helper method. Writes text to the console and/or a test file.
	void WriteOutputText(string preamble, string text, string test) {
		if(writeToFile) {
			if(text == testInput)
				AddToConsole(test + " <color=green>PASSED</color>");
			else
			{
				AddToConsole(test + " <color=red>FAILED</color>");
				AddToConsole("\t> expected <color=red>" + testInput + "</color>");
				AddToConsole("\t> received <color=red>" + text + "</color>");
			}

			testWriter.WriteLine(preamble + text);
		}
		else
			AddToConsole(preamble + text);
	}
	#endregion
}
