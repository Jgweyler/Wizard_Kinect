using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Kinect.VisualGestureBuilder;
using Windows.Kinect;
using System.IO;


/// <summary>
/// This interface needs to be implemented by all visual gesture listeners
/// </summary>
public interface VisualGestureListenerInterface
{
	/// <summary>
	/// Invoked when a continuous gesture reports progress >= 0.1f
	/// </summary>
	/// <param name="userId">User ID</param>
	/// <param name="userIndex">User index</param>
	/// <param name="gesture">Gesture name</param>
	/// <param name="progress">Gesture progress [0..1]</param>
	void GestureInProgress(long userId, int userIndex, string gesture, float progress);
	
	/// <summary>
	/// Invoked when a discrete gesture is completed.
	/// </summary>
	/// <returns><c>true</c>, if the gesture detection must be restarted, <c>false</c> otherwise.</returns>
	/// <param name="userId">User ID</param>
	/// <param name="userIndex">User index</param>
	/// <param name="gesture">Gesture name</param>
	/// <param name="confidence">Gesture confidence [0..1]</param>
	bool GestureCompleted(long userId, int userIndex, string gesture, float confidence);
}

/// <summary>
/// Visual gesture data structure.
/// </summary>
public struct VisualGestureData
{
	public long userId;
	public float timestamp;
	public string gestureName;
	public bool isDiscrete;
	public bool isContinuous;
	public bool isComplete;
	public bool isResetting;
	public float confidence;
	public float progress;
}

/// <summary>
/// Visual gesture manager is the component dealing with VGB gestures.
/// </summary>
public class VisualGestureManager : MonoBehaviour 
{
	[Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
	public int playerIndex = 0;

	[Tooltip("File name of the VG database, used by the visual gesture recognizer. The file will be copied from Resources, if does not exist.")]
	public string gestureDatabase = string.Empty;

	[Tooltip("List of the tracked visual gestures. If the list is empty, all gestures found in the database will be tracked.")]
	public List<string> gestureNames = new List<string>();

	[Tooltip("Minimum confidence required, to consider discrete gestures as completed. Confidence varies between 0.0 and 1.0.")]
	public float minConfidence = 0.1f;

	[Tooltip("List of the utilized visual gesture listeners. They must implement VisualGestureListenerInterface. If the list is empty, the available gesture listeners will be detected at start up.")]
	public List<MonoBehaviour> visualGestureListeners;
	
	[Tooltip("GUI-Text to display the VG-manager debug messages.")]
	public GUIText debugText;


	// primary user ID, as reported by KinectManager
	private long primaryUserID = 0;
	private PlayerMovement playerMovementScript;
	private SpellManager spellManagerScript;
	private MakeTarget playerTargetScript;
	private CastSpell castSpellScript;
	private Animator playerAnimator;

	private const int girar_left = 0;
	private const int girar_right = 1;
	private const int invocar_elemento = 3;
	private const int retroceder = 4;
	private const int caminar = 5;
	private const int fijar_objetivo = 6;
	private const int cargar_hechizo = 7;
	private const int lanzar = 8;

	//Number of gestures in our db.
	private int n_gestures = 9;
	private bool[] completed_gestures;

	// gesture data holders for each tracked gesture
	private Dictionary<string, VisualGestureData> gestureData = new Dictionary<string, VisualGestureData>();

	// gesture frame source which should be tied to a body tracking ID
	private VisualGestureBuilderFrameSource vgbFrameSource = null;
	
	// gesture frame reader which will handle gesture events
	private VisualGestureBuilderFrameReader vgbFrameReader = null;
	
	// primary sensor data structure
	//private KinectInterop.SensorData sensorData = null;
	
	// Bool to keep track of whether visual-gesture system has been initialized
	private bool isVisualGestureInitialized = false;
	
	// The single instance of VisualGestureManager
	private static VisualGestureManager instance;
	

	/// <summary>
	/// Gets the single VisualGestureManager instance.
	/// </summary>
	/// <value>The VisualGestureManager instance.</value>
	public static VisualGestureManager Instance
    {
        get
        {
            return instance;
        }
    }
	
	/// <summary>
	/// Determines whether the visual-gesture manager was successfully initialized.
	/// </summary>
	/// <returns><c>true</c> if visual-gesture manager was successfully initialized; otherwise, <c>false</c>.</returns>
	public bool IsVisualGestureInitialized()
	{
		return isVisualGestureInitialized;
	}
	
	/// <summary>
	/// Gets the skeleton ID of the tracked user, or 0 if no user was associated with the gestures.
	/// </summary>
	/// <returns>The skeleton ID of the tracked user.</returns>
	public long GetTrackedUserID()
	{
		return primaryUserID;
	}
	
	/// <summary>
	/// Gets the list of detected gestures.
	/// </summary>
	/// <returns>The list of detected gestures.</returns>
	public List<string> GetGesturesList()
	{
		return gestureNames;
	}
	
	/// <summary>
	/// Gets the count of detected gestures.
	/// </summary>
	/// <returns>The count of detected gestures.</returns>
	public int GetGesturesCount()
	{
		return gestureNames.Count;
	}

	/// <summary>
	/// Gets the gesture name at specified index, or empty string if the index is out of range.
	/// </summary>
	/// <returns>The gesture name at specified index.</returns>
	/// <param name="i">The index</param>
	public string GetGestureAtIndex(int i)
	{
		if(i >= 0 && i < gestureNames.Count)
		{
			return gestureNames[i];
		}

		return string.Empty;
	}
	
	/// <summary>
	/// Determines whether the given gesture is in the list of detected gestures.
	/// </summary>
	/// <returns><c>true</c> if the given gesture is in the list of detected gestures; otherwise, <c>false</c>.</returns>
	/// <param name="gestureName">Gesture name.</param>
	public bool IsTrackingGesture(string gestureName)
	{
		return gestureNames.Contains(gestureName);
	}
	
	/// <summary>
	/// Determines whether the specified discrete gesture is completed.
	/// </summary>
	/// <returns><c>true</c> if the specified discrete gesture is completed; otherwise, <c>false</c>.</returns>
	/// <param name="gestureName">Gesture name</param>
	/// <param name="bResetOnComplete">If set to <c>true</c>, resets the gesture state.</param>
	public bool IsGestureCompleted(string gestureName, bool bResetOnComplete)
	{
		if(gestureNames.Contains(gestureName))
		{
			VisualGestureData data = gestureData[gestureName];
			
			if(data.isDiscrete && data.isComplete && !data.isResetting && data.confidence >= minConfidence)
			{
				if(bResetOnComplete)
				{
					data.isResetting = true;
					gestureData[gestureName] = data;
				}

				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Gets the confidence of the specified discrete gesture, in range [0, 1].
	/// </summary>
	/// <returns>The gesture confidence.</returns>
	/// <param name="gestureName">Gesture name</param>
	public float GetGestureConfidence(string gestureName)
	{
		if(gestureNames.Contains(gestureName))
		{
			VisualGestureData data = gestureData[gestureName];
			
			if(data.isDiscrete)
			{
				return data.confidence;
			}
		}
		
		return 0f;
	}
	
	/// <summary>
	/// Gets the progress of the specified continuous gesture, in range [0, 1].
	/// </summary>
	/// <returns>The gesture progress.</returns>
	/// <param name="gestureName">Gesture name</param>
	public float GetGestureProgress(string gestureName)
	{
		if(gestureNames.Contains(gestureName))
		{
			VisualGestureData data = gestureData[gestureName];
			
			if(data.isContinuous)
			{
				return data.progress;
			}
		}
		
		return 0f;
	}


	//----------------------------------- end of public functions --------------------------------------//
	
	
	void Start() 
	{
		try 
		{
			// get sensor data
			KinectManager kinectManager = KinectManager.Instance;
			KinectInterop.SensorData sensorData = kinectManager != null ? kinectManager.GetSensorData() : null;

			if(sensorData == null || sensorData.sensorInterface == null)
			{
				throw new Exception("Visual gesture tracking cannot be started, because the KinectManager is missing or not initialized.");
			}

			if(sensorData.sensorInterface.GetSensorPlatform() != KinectInterop.DepthSensorPlatform.KinectSDKv2)
			{
				throw new Exception("Visual gesture tracking is only supported by Kinect SDK v2");
			}

			// ensure the needed dlls are in place and face tracking is available for this interface
			bool bNeedRestart = false;
			if(IsVisualGesturesAvailable(ref bNeedRestart))
			{
				if(bNeedRestart)
				{
					KinectInterop.RestartLevel(gameObject, "VG");
					return;
				}
			}
			else
			{
				throw new Exception("Visual gesture tracking is not supported!");
			}

			// initialize visual gesture tracker
			if (!InitVisualGestures())
	        {
				throw new Exception("Visual gesture tracking could not be initialized.");
	        }
			
			// try to automatically detect the available gesture listeners in the scene
			if(visualGestureListeners.Count == 0)
			{
				MonoBehaviour[] monoScripts = FindObjectsOfType(typeof(MonoBehaviour)) as MonoBehaviour[];
				
				foreach(MonoBehaviour monoScript in monoScripts)
				{
					if(typeof(VisualGestureListenerInterface).IsAssignableFrom(monoScript.GetType()) &&
					   monoScript.enabled)
					{
						visualGestureListeners.Add(monoScript);
					}
				}
			}

			// all set
			instance = this;
			playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
			playerTargetScript = GameObject.FindGameObjectWithTag("Player").GetComponent<MakeTarget>();
			castSpellScript = GameObject.FindGameObjectWithTag("Player").GetComponent<CastSpell>();
			spellManagerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<SpellManager>();
			playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
			completed_gestures = new bool[n_gestures];
			for (int i = 0; i < n_gestures; i++) {
				completed_gestures [i] = false;
			}
			isVisualGestureInitialized = true;
		} 
		catch(DllNotFoundException ex)
		{
			Debug.LogError(ex.ToString());
			if(debugText != null)
				debugText.GetComponent<GUIText>().text = "Please check the Kinect and FT-Library installations.";
		}
		catch (Exception ex) 
		{
			Debug.LogError(ex.ToString());
			if(debugText != null)
				debugText.GetComponent<GUIText>().text = ex.Message;
		}
	}

	void OnDestroy()
	{
		if(isVisualGestureInitialized)
		{
			// finish visual gesture tracking
			FinishVisualGestures();
		}

		isVisualGestureInitialized = false;
		instance = null;
	}
	
	void Update() 
	{
		if(isVisualGestureInitialized)
		{
			KinectManager kinectManager = KinectManager.Instance;
			if(kinectManager && kinectManager.IsInitialized())
			{
				primaryUserID = kinectManager.GetUserIdByIndex(playerIndex);
			}

			// update visual gesture tracking
			if(UpdateVisualGestures(primaryUserID))
			{
				// process the gestures
				foreach(string gestureName in gestureNames)
				{
					if(gestureData.ContainsKey(gestureName))
					{
						VisualGestureData data = gestureData[gestureName];

						if(data.isComplete && !data.isResetting && data.confidence >= minConfidence)
						{
							Debug.Log(gestureName + " detected.");
							int userIndex = kinectManager ? kinectManager.GetUserIndexById(data.userId) : 0;
							//Aquí especificamos qué hay que hacer según el tipo de gesto que se detecte!
							String gesto = gestureName;
							switch(gesto){
							case "girar_Left":
								completed_gestures [girar_left] = true;
								break;
							case "girar_Right":
								completed_gestures [girar_right] = true;
								break;
							case "retroceder":
								completed_gestures [retroceder] = true;
								break;
							case "fijar_objetivo":
								completed_gestures [fijar_objetivo] = true;
								break;
							case "caminar":
								completed_gestures [caminar] = true;
								break;
							case "invocar_elemento":
								completed_gestures [invocar_elemento] = true;
								break;
							case "cargar_hechizo":
								completed_gestures [cargar_hechizo] = true;
								break;
							case "lanzar":
								completed_gestures [lanzar] = true;
								break;
							default:
								break;
							}
							/*
							foreach(VisualGestureListenerInterface listener in visualGestureListeners)
							{
								if(listener.GestureCompleted(data.userId, userIndex, data.gestureName, data.confidence))
								{
									data.isResetting = true;
									gestureData[gestureName] = data;
								}
							}
							*/
						}
						else if(data.progress >= 0.1f)
						{
							int userIndex = kinectManager ? kinectManager.GetUserIndexById(data.userId) : 0;

							foreach(VisualGestureListenerInterface listener in visualGestureListeners)
							{
								listener.GestureInProgress(data.userId, userIndex, data.gestureName, data.progress);
							}
						}
					}
				}
			}
		}
	}

	void LateUpdate(){

        if (!isVisualGestureInitialized) return;
		for (int i = 0; i < n_gestures; i++) {
			switch (i) {
			case girar_left:
				
				if (completed_gestures [girar_left]) { // Si está girando...
					if (!playerTargetScript.pointed_target ())
						playerMovementScript.turnRight_Kinect (); //Detecta el brazo IZQUIERDO(desde la perspectiva de Kinect) levantado
					else
						playerMovementScript.combatMoveKinect (0);

					if (castSpellScript.getCasted_Kinect ()) { //Si se mueve, interrumpimos el casteo.
						castSpellScript.setCasted_Kinect (false);
						backToNormalPose ();
					}
				} else { //Si no está girando, paramos la animacion en el caso de que se estuviera ejecutando.
					if (playerAnimator.GetBool ("isTurning"))
						playerAnimator.SetBool ("isTurning", false);
				}

				break;
			case girar_right:
				
				if (completed_gestures [girar_right]) { // Si está girando
					if (!playerTargetScript.pointed_target ())
						playerMovementScript.turnLeft_Kinect (); //Detecta el brazo IZQUIERDO(desde la perspectiva de Kinect) levantado
					else
						playerMovementScript.combatMoveKinect (1);

					if (castSpellScript.getCasted_Kinect ()) { //Si se mueve, interrumpimos el casteo.
						castSpellScript.setCasted_Kinect (false);
						backToNormalPose ();
					}
				}else { //Si no está girando, paramos la animacion en el caso de que se estuviera ejecutando.
					if (playerAnimator.GetBool ("isTurning"))
						playerAnimator.SetBool ("isTurning", false);
				}

				break;
			case invocar_elemento:
				
				if (completed_gestures [invocar_elemento]) {
					spellManagerScript.switchElement_Kinect ();
					if (!castSpellScript.getCasted_Kinect ()) {
						castSpellScript.setCasted_Kinect (false);
					}
				}

				break;
			case retroceder:
				
				if (completed_gestures [retroceder]) { //Si detectó que se está retrocediendo
					playerMovementScript.goBack_Kinect ();
					if (castSpellScript.getCasted_Kinect ()) { //Si se mueve, interrumpimos el casteo.
						castSpellScript.setCasted_Kinect (false);
						backToNormalPose ();
					}
					//Puede pasar que se haya detectado el gesto de caminar, y a la vez se hace el movimiento de retroceder.
					//En tal caso, como tiene más importancia el de retroceder sobre el de caminar, anulamos el de caminar:
					completed_gestures[caminar] = false;
					completed_gestures [cargar_hechizo] = false;
				} else { // Si no detectó que está retrocediendo, nos aseguramos de parar la animacion de retroceder.
					if(playerAnimator.GetBool("goBack")){ //Comprobamos que se detenga correctamente la animación de correr.
						playerAnimator.SetBool("goBack", false);
					}
				}

				break;
			case caminar:
				if (completed_gestures [caminar]) { // Si se detectó que el usuario está caminando.
					playerMovementScript.moveForward_Kinect ();

					if (castSpellScript.getCasted_Kinect ()) { //Si se mueve, interrumpimos el casteo.
						castSpellScript.setCasted_Kinect (false);
						backToNormalPose ();
					}
				} else { //Si se detectó que no está caminando
					if(playerAnimator.GetBool("isMoving")){ //Comprobamos que se detenga correctamente la animación de correr.
						playerAnimator.SetBool("isMoving", false);
						Debug.Log("Correr pasa a false!");
					}
				}
				break;
			case fijar_objetivo:
				
				if (completed_gestures [fijar_objetivo]) {
					playerTargetScript.pointTarget_Kinect ();
					if (castSpellScript.getCasted_Kinect ()) { //Si se mueve, interrumpimos el casteo.
						castSpellScript.setCasted_Kinect (false);
						backToNormalPose ();
					}
				}

				break;
			case cargar_hechizo:
				
				if (completed_gestures [cargar_hechizo]) {
					if (!castSpellScript.getCasted_Kinect () && !playerAnimator.GetBool("isMoving")) {
						//playerAnimator.SetBool ("isCasting", true);
						playerAnimator.Play ("chargeSpell");
						castSpellScript.setCasted_Kinect (true);
					}
				}

				break;
			case lanzar:
				
				if (completed_gestures [lanzar]) {
					if (castSpellScript.getCasted_Kinect ()) {
						//playerAnimator.Play ("launchSpell");
						StartCoroutine(waitAnimation("launchSpell"));
						SpellManager.launchSpell (100f); //Cambiar este cableado. El parametro es 
						//la fuerza de lanzamiento.
						castSpellScript.setCasted_Kinect (false);
						//backToNormalPose ();
					}
				}

				break;
			default:
				break;
			}
		}

		for (int j = 0; j < n_gestures; j++) {
			completed_gestures [j] = false;
		}
	}

	private void backToNormalPose(){ //Cancela el casteo debido a que el jugador no completó el lanzamiento de hechizo
		//playerAnimator.SetBool("isCasting", false);
		if (playerAnimator.GetBool ("hasTarget")) {
			playerAnimator.Play ("combatPos");
		} else {
			playerAnimator.Play ("idle");
		}
	}

	private IEnumerator waitAnimation( String paramAnimationState){
		/*playerAnimator.SetBool(paramAnimationState, true);
		yield return null;
		playerAnimator.SetBool(paramAnimationState, false);
		*/
		playerAnimator.Play(paramAnimationState);
		yield return new WaitForSeconds(10);
		/*
		while (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95f)
		{
			yield return null;
		}*/
	}
	private bool IsVisualGesturesAvailable(ref bool bNeedRestart)
	{
		bool bOneCopied = false, bAllCopied = true;
		string sTargetPath = ".";
		
		if(!KinectInterop.Is64bitArchitecture())
		{
			// 32 bit
			sTargetPath = KinectInterop.GetTargetDllPath(".", false) + "/";
			
			Dictionary<string, string> dictFilesToUnzip = new Dictionary<string, string>();
			dictFilesToUnzip["Kinect20.VisualGestureBuilder.dll"] = sTargetPath + "Kinect20.VisualGestureBuilder.dll";
			dictFilesToUnzip["KinectVisualGestureBuilderUnityAddin.dll"] = sTargetPath + "KinectVisualGestureBuilderUnityAddin.dll";
			dictFilesToUnzip["vgbtechs/AdaBoostTech.dll"] = sTargetPath + "vgbtechs/AdaBoostTech.dll";
			dictFilesToUnzip["vgbtechs/RFRProgressTech.dll"] = sTargetPath + "vgbtechs/RFRProgressTech.dll";
			dictFilesToUnzip["msvcp110.dll"] = sTargetPath + "msvcp110.dll";
			dictFilesToUnzip["msvcr110.dll"] = sTargetPath + "msvcr110.dll";
			
			KinectInterop.UnzipResourceFiles(dictFilesToUnzip, "KinectV2UnityAddin.x86.zip", ref bOneCopied, ref bAllCopied);
		}
		else
		{
			//Debug.Log("Face - x64-architecture.");
			sTargetPath = KinectInterop.GetTargetDllPath(".", true) + "/";
			
			Dictionary<string, string> dictFilesToUnzip = new Dictionary<string, string>();
			dictFilesToUnzip["Kinect20.VisualGestureBuilder.dll"] = sTargetPath + "Kinect20.VisualGestureBuilder.dll";
			dictFilesToUnzip["KinectVisualGestureBuilderUnityAddin.dll"] = sTargetPath + "KinectVisualGestureBuilderUnityAddin.dll";
			dictFilesToUnzip["vgbtechs/AdaBoostTech.dll"] = sTargetPath + "vgbtechs/AdaBoostTech.dll";
			dictFilesToUnzip["vgbtechs/RFRProgressTech.dll"] = sTargetPath + "vgbtechs/RFRProgressTech.dll";
			dictFilesToUnzip["msvcp110.dll"] = sTargetPath + "msvcp110.dll";
			dictFilesToUnzip["msvcr110.dll"] = sTargetPath + "msvcr110.dll";
			
			KinectInterop.UnzipResourceFiles(dictFilesToUnzip, "KinectV2UnityAddin.x64.zip", ref bOneCopied, ref bAllCopied);
		}

		bNeedRestart = (bOneCopied && bAllCopied);
		
		return true;
	}
	
	private bool InitVisualGestures()
	{
		KinectManager kinectManager = KinectManager.Instance;
		KinectInterop.SensorData sensorData = kinectManager != null ? kinectManager.GetSensorData() : null;

		Kinect2Interface kinectInterface = sensorData.sensorInterface as Kinect2Interface;
		KinectSensor kinectSensor = kinectInterface != null ? kinectInterface.kinectSensor : null;

		if(kinectSensor == null)
			return false;

		if(gestureDatabase == string.Empty)
		{
			Debug.LogError("Please specify gesture database file!");
			return false;
		}

		//gestureDatabase = Directory.GetCurrentDirectory () + @"\Assets\Resources\" + gestureDatabase;
		// copy the gesture database file from Resources, if available
		if (!File.Exists (gestureDatabase)) {
			Debug.Log (Directory.GetCurrentDirectory ());
			Debug.Log ("El nombre de la base de datos es: " + gestureDatabase);
			TextAsset textRes = Resources.Load (gestureDatabase, typeof(TextAsset)) as TextAsset;
			
			if (textRes != null && textRes.bytes.Length != 0) {
				File.WriteAllBytes (gestureDatabase, textRes.bytes);
			}
		}
		
		// create the vgb source
		vgbFrameSource = VisualGestureBuilderFrameSource.Create(kinectSensor, 0);

		// open the reader
		vgbFrameReader = vgbFrameSource != null ? vgbFrameSource.OpenReader() : null;
		if(vgbFrameReader != null)
		{
			vgbFrameReader.IsPaused = true;
		}
		
		using (VisualGestureBuilderDatabase database = VisualGestureBuilderDatabase.Create(gestureDatabase))
		{
			if(database == null)
			{
				Debug.LogError("Gesture database not found: " + gestureDatabase);
				return false;
			}

			// check if we need to load all gestures
			bool bAllGestures = (gestureNames.Count == 0);

			foreach (Gesture gesture in database.AvailableGestures)
			{
				bool bAddGesture = bAllGestures || gestureNames.Contains(gesture.Name);

				if(bAddGesture)
				{
					string sGestureName = gesture.Name;
					vgbFrameSource.AddGesture(gesture);

					if(!gestureNames.Contains(sGestureName))
					{
						gestureNames.Add(sGestureName);
					}

					if(!gestureData.ContainsKey(sGestureName))
					{
						VisualGestureData data = new VisualGestureData();
						data.gestureName = sGestureName;
						data.isDiscrete = (gesture.GestureType == GestureType.Discrete);
						data.isContinuous = (gesture.GestureType == GestureType.Continuous);
						data.timestamp = Time.realtimeSinceStartup;
						
						gestureData.Add(sGestureName, data);
					}
				}
			}
		}

		return true;
	}
	
	private void FinishVisualGestures()
	{
		if (vgbFrameReader != null)
		{
			vgbFrameReader.Dispose();
			vgbFrameReader = null;
		}
		
		if (vgbFrameSource != null)
		{
			vgbFrameSource.Dispose();
			vgbFrameSource = null;
		}

		if(gestureData != null)
		{
			gestureData.Clear();
		}
	}
	
	private bool UpdateVisualGestures(long userId)
	{
		if(vgbFrameSource == null || vgbFrameReader == null)
			return false;

		bool wasPaused = vgbFrameReader.IsPaused;
		vgbFrameSource.TrackingId = (ulong)userId;
		vgbFrameReader.IsPaused = (userId == 0);

		if(vgbFrameReader.IsPaused)
		{
			if(!wasPaused)
			{
				// clear the gesture states
				foreach (Gesture gesture in vgbFrameSource.Gestures)
				{
					if(gestureData.ContainsKey(gesture.Name))
					{
						VisualGestureData data = gestureData[gesture.Name];

						data.userId = 0;
						data.isComplete = false;
						data.isResetting = false;
						data.confidence = 0f;
						data.progress = 0f;
						data.timestamp = Time.realtimeSinceStartup;
						
						gestureData[gesture.Name] = data;
					}
				}
			}

			return false;
		}

		VisualGestureBuilderFrame frame = vgbFrameReader.CalculateAndAcquireLatestFrame();

		if(frame != null)
		{
			Dictionary<Gesture, DiscreteGestureResult> discreteResults = frame.DiscreteGestureResults;
			Dictionary<Gesture, ContinuousGestureResult> continuousResults = frame.ContinuousGestureResults;

			if (discreteResults != null)
			{
				foreach (Gesture gesture in discreteResults.Keys)
				{
					if(gesture.GestureType == GestureType.Discrete && gestureData.ContainsKey(gesture.Name))
					{
						DiscreteGestureResult result = discreteResults[gesture];
						VisualGestureData data = gestureData[gesture.Name];

						data.userId = vgbFrameSource.IsTrackingIdValid ? (long)vgbFrameSource.TrackingId : 0;
						data.isComplete = result.Detected;
						data.confidence = result.Confidence;
						data.timestamp = Time.realtimeSinceStartup;

						//Debug.Log(string.Format ("{0} - {1}, confidence: {2:F0}%", data.gestureName, data.isComplete ? "Yes" : "No", data.confidence * 100f));

						if(data.isResetting && !data.isComplete)
						{
							data.isResetting = false;
						}

						gestureData[gesture.Name] = data;
					}
				}
			}

			if (continuousResults != null)
			{
				foreach (Gesture gesture in continuousResults.Keys)
				{
					if(gesture.GestureType == GestureType.Continuous && gestureData.ContainsKey(gesture.Name))
					{
						ContinuousGestureResult result = continuousResults[gesture];
						VisualGestureData data = gestureData[gesture.Name];

						data.userId = vgbFrameSource.IsTrackingIdValid ? (long)vgbFrameSource.TrackingId : 0;
						data.progress = result.Progress;
						data.timestamp = Time.realtimeSinceStartup;

						gestureData[gesture.Name] = data;
					}
				}
			}
			
			frame.Dispose();
			frame = null;
		}

		return true;
	}
	
}
