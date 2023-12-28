
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
	public bool useSimulatorView = false;

	private void Awake()
	{
		FitSafeArea();
	}

	[ContextMenu("Fit")]
	public void FitSafeArea()
	{
		Rect safeArea = GetSafeArea();
		Vector2 size = GetCurrentResolution();

		Vector2 zeroPoint = new Vector2(safeArea.xMin, safeArea.yMin);
		//Anchor Min
		float btAdj = 0;
		if(UnityEngine.iOS.Device.generation >= UnityEngine.iOS.DeviceGeneration.iPhoneX) {
			btAdj = 10;
		}
		Vector2 newAnchorMin = new Vector2(safeArea.xMin, btAdj);
		newAnchorMin.x /= size.x;
		newAnchorMin.y /= size.y;

		//Anchor Max
		float y = safeArea.height + safeArea.yMin;
		Vector2 adj = new Vector2(safeArea.width, y);
		Vector2 newAnchorMax = zeroPoint + adj;
		newAnchorMax.x /= size.x;
		newAnchorMax.y /= size.y;

		RectTransform rect = gameObject.GetComponent<RectTransform>();
		rect.anchorMin = newAnchorMin;
		rect.anchorMax = newAnchorMax;
		Debug.Log($"Width: {size.x}, Height: {size.y}, SafeArea: {safeArea} -> Min: {newAnchorMin}, Max: {newAnchorMax}");
	}

	public Rect GetSafeArea()
	{
#if UNITY_IOS || UNITY_ANDROID
		//Set MobileDevice Safearea
		return Screen.safeArea;
#else
		//Set PC Safearea(Match screen) 
		Vector2 size = GetMainGameViewSize();
		if (useSimulatorView) {
			//Set SimulatorView
			return Screen.safeArea;
		}
		return new Rect(0, 0, size.x, size.y);
#endif
	}

	public Vector2 GetCurrentResolution()
	{
#if UNITY_IOS || UNITY_ANDROID
		//Set MobileDevice Resolution(IOS, ANDROID)
		return new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
#else
		//Set SimulatorView Resolution
		if (useSimulatorView) {
			return new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
		}
		//Set GameView Resolution(PC Editor)
		return GetMainGameViewSize();
#endif
	}

	public static Vector2 GetMainGameViewSize()
	{
		System.Type editor = System.Type.GetType("UnityEditor.GameView,UnityEditor");
		System.Reflection.MethodInfo GetSizeOfMainGameView = editor.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
		System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
		return (Vector2)Res;
	}
}