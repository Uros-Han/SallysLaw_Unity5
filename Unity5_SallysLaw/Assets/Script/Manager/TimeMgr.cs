using UnityEngine;
using System.Collections;

public static class TimeMgr {

	public static bool m_bFastForward;

	public static void Play()
	{
		Time.timeScale = 1;
	}

	public static void Pause()
	{
		Time.timeScale = 0;
	}

	public static void SlowMotion()
	{
		Time.timeScale = 0.1f;
	}

	public static void FastForward()
	{
		Time.timeScale = 2f;
	}
}
