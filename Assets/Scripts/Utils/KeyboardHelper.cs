using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class KeyboardHelper {

	[DllImport("user32.dll", EntryPoint = "GetKeyboardState", SetLastError = true)]
	private static extern bool GetKeyboardState(byte[] pbKeyState);

	[DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
	private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

	private const byte VK_NUMLOCK = 0x90; // Virtual key code for Num Lock
	private const byte VK_CAPSLOCK = 0x14;
	private const uint KEYEVENTF_EXTENDEDKEY = 0x0001; // Extended key flag
	private const uint KEYEVENTF_KEYUP = 0x0002; // Key up flag

	public static void SetNumLock(bool enable)
	{
		byte[] keyState = new byte[256];
		GetKeyboardState(keyState);

		// Check if Num Lock is already in the desired state
		if ((enable && (keyState[VK_NUMLOCK] & 1) == 0) || (!enable && (keyState[VK_NUMLOCK] & 1) != 0))
		{
			// Simulate a Num Lock key press
			keybd_event(VK_NUMLOCK, 0x45, KEYEVENTF_EXTENDEDKEY, UIntPtr.Zero);
			// Simulate a Num Lock key release
			keybd_event(VK_NUMLOCK, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, UIntPtr.Zero);
		}
	}

	public static void SetCapsLock(bool enable)
	{
		byte[] keyState = new byte[256];
		GetKeyboardState(keyState);

		// Check if Num Lock is already in the desired state
		if ((enable && (keyState[VK_CAPSLOCK] & 1) == 0) || (!enable && (keyState[VK_CAPSLOCK] & 1) != 0))
		{
			// Simulate a Caps Lock key press
			keybd_event(VK_CAPSLOCK, 0x45, KEYEVENTF_EXTENDEDKEY, UIntPtr.Zero);
			// Simulate a Caps Lock key release
			keybd_event(VK_CAPSLOCK, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, UIntPtr.Zero);
		}
	}

	public static bool GetCapslockState()
	{
		byte[] keyState = new byte[256];
		GetKeyboardState(keyState);
		return (keyState[VK_CAPSLOCK] & 1) == 1;
	}

	public static bool GetNumlockState()
	{
		byte[] keyState = new byte[256];
		GetKeyboardState(keyState);
		return (keyState[VK_NUMLOCK] & 1) == 1;
	}
}
