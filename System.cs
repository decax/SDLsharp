using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SDL
{
	public enum PixelFormat
	{
		UNKNOWN,

		INDEX_1LSB   = 0x11100100,
		INDEX_1MSB   = 0x11200100,
		INDEX_4LSB   = 0x12100400,
		INDEX_4MSB   = 0x12200400,
		INDEX_8      = 0x13000801,
		RGB_332      = 0x14110801,
		RGB_444      = 0x15120c02,
		RGB_555      = 0x15130f02,
		BGR_555      = 0x15530f02,
		ARGB_4444    = 0x15321002,
		RGBA_4444    = 0x15421002,
		ABGR_4444    = 0x15721002,
		BGRA_4444    = 0x15821002,
		ARGB_1555    = 0x15331002,
		RGBA_5551    = 0x15441002,
		ABGR_1555    = 0x15731002,
		BGRA_5551    = 0x15841002,
		RGB_565      = 0x15151002,
		BGR_565      = 0x15551002,
		RGB_24       = 0x17101803,
		BGR_24       = 0x17401803,
		RGB_888      = 0x16161804,
		RGBX_8888    = 0x16261804,
		BGR_888      = 0x16561804,
		BGRX_8888    = 0x16661804,
		ARGB_8888    = 0x16362004,
		RGBA_8888    = 0x16462004,
		ABGR_8888    = 0x16762004,
		BGRA_8888    = 0x16862004,
		ARGB_2101010 = 0x16372004,
		YV_12        = 0x32315659,
		IYUV         = 0x56555949,
		YUY_2        = 0x32595559,
		UYVY         = 0x59565955,
		YVYU         = 0x55595659
	}

	public class System
	{
		[Flags]
		public enum Init
		{
			Timer          = 0x00000001, 
			Audio          = 0x00000010, 
			Video          = 0x00000020, 
			Joystick       = 0x00000200, 
			Haptic         = 0x00001000, 
			GameController = 0x00002000,
			Events         = 0x00004000,
			NoParachute    = 0x00100000, 

			Everything     = Timer | Audio | Video | Joystick | Haptic | GameController | Events
		}

		public System(Init init = Init.Everything)
		{
			SDL_Init((uint)init);
		}

		public void Dispose()
		{
			SDL_Quit();
		}

		public uint GetTicks()
		{
			return SDL_GetTicks();
		}

		public event Action OnQuit = () => {};
		public event Action<KeyboardEvent> OnKeyboard = (_) => {};

		public List<Event> Events = new List<Event>();

		public void PollEvent()
		{
			Event e;
			SDL_PollEvent(out e);

			switch (e.Type) {

			case Event.EventType.Quit:
				OnQuit();
				break;
			
			case Event.EventType.KeyDown:
			case Event.EventType.KeyUp:
				OnKeyboard(e.keyboardEvent);
				break;
			}

			Events.Add(e);
		}

		internal static string GetError()
		{
			var errorMsg = SDL_GetError();
			return Marshal.PtrToStringAnsi(errorMsg);
		}

		#region Native
		const string sdlDLL = "/Library/Frameworks/SDL2.framework/SDL2";

		[DllImport(sdlDLL)]
		private static extern int SDL_Init(uint flags);

		[DllImport(sdlDLL)]
		private static extern void SDL_Quit();

		[DllImport(sdlDLL)]
		private static extern int SDL_PollEvent(out Event sdlEvent);

		[DllImport(sdlDLL)]
		private static extern uint SDL_GetTicks();

		[DllImport(sdlDLL)]
		private static extern IntPtr SDL_GetError();

		#endregion
	}
}