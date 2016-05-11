using System;
using System.Runtime.InteropServices;

namespace SDL
{
	public struct KeyboardEvent
	{
		uint type;
		uint timestamp;
		uint windowID;
		byte state;
		byte repeat;

		byte padding2;
		byte padding3;
		KeySym KeySym;

		public Event.EventType Type { get {return (Event.EventType)type; } }
		public ScanCode Scancode { get { return KeySym.Scancode; } }
		public KeyCode KeyCode { get { return KeySym.Keycode; } }
	}

	public struct KeySym
	{
		public ScanCode Scancode;
		public KeyCode Keycode;

		short mod;
		uint unused;
	}

	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 56)]
	public struct Event
	{
		public enum EventType
		{
			Quit = 0x100, 

			KeyDown = 0x300,
			KeyUp
		}

		[FieldOffset(0)]
		uint type;

		[FieldOffset(0)]
		public KeyboardEvent keyboardEvent;

		public EventType Type { get {return (EventType)type; } }
	}

}